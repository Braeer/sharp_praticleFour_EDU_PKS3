using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_web.Data;
using Practice_web.Models;

namespace Practice_web.Controllers
{
    public class MessagesController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IActionResult> Index(int userId, string filterSender, string filterStartDate, string filterEndDate, string filterStatus = "all", string sortOrder = "asc")
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.UserFullName = user.FullName;
            ViewBag.UserId = user.Id;
            ViewBag.FilterSender = filterSender;
            ViewBag.FilterStartDate = filterStartDate;
            ViewBag.FilterEndDate = filterEndDate;
            ViewBag.FilterStatus = filterStatus;
            ViewBag.SortOrder = sortOrder;

            var messages = _context.Messages
                .Include(m => m.FromUser)
                .Include(m => m.ToUser)
                .Where(m => m.FromUserId == userId
                    || m.ToUserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filterSender))
            {
                messages = messages.Where(m => m.FromUser.Login == filterSender || m.FromUserId == userId);
            }

            if (!string.IsNullOrEmpty(filterStartDate) && DateTime.TryParse(filterStartDate, out DateTime startDate))
            {
                messages = messages.Where(m => m.SentDate >= startDate.ToUniversalTime().AddDays(1));
            }

            if (!string.IsNullOrEmpty(filterEndDate) && DateTime.TryParse(filterEndDate, out DateTime endDate))
            {
                messages = messages.Where(m => m.SentDate <= endDate.ToUniversalTime().AddDays(1));
            }

            if (filterStatus == "unread")
            {
                messages = messages.Where(m => !m.IsRead || m.ToUserId == userId);
            }

            messages = sortOrder == "asc" ? messages.OrderBy(m => m.SentDate) : messages.OrderByDescending(m => m.SentDate);

            var filteredMessages = await messages.ToListAsync();

            return View(filteredMessages);
        }

        public IActionResult Send()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send(int fromUserId, string recipient, string subject, string messageText)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == recipient);

            if (user == null)
            {
                ViewBag.ErrorMessage = "Получатель не существует";
                return View();
            }

            _context.Messages.Add(new Message
            {
                FromUserId = fromUserId,
                ToUserId = user.Id,
                SentDate = DateTime.UtcNow,
                Title = subject,
                Body = messageText,
                IsRead = false
            });
            await _context.SaveChangesAsync();

            return Ok(new {userId = fromUserId});
        }

        [HttpGet]
        public async Task<IActionResult> GetMessage(int id)
        {
            var message = await _context.Messages
                .Include(m => m.FromUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound();
            }

            return Json(message.Body);
        }
    }
}

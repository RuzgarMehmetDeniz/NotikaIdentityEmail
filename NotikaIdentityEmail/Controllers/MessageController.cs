using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.MessageViewModels;
using X.PagedList.Extensions;

namespace NotikaIdentityEmail.Controllers
{
    public class MessageController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MessageController(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Inbox(int page = 1)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            int pageSize = 10; // Her sayfada gösterilecek mesaj sayısı

            var query = from m in _context.Messages
                        join u in _context.Users on m.SenderEmail equals u.Email into userGroup
                        from sender in userGroup.DefaultIfEmpty()
                        join c in _context.Categories on m.CategoryId equals c.CategoryId into categoryGroup
                        from category in categoryGroup.DefaultIfEmpty()
                        where m.ReceiverEmail == user.Email
                        orderby m.SendDate descending // En yeni mesajlar üstte görünsün
                        select new MessageWithSenderInfoViewModel
                        {
                            MessageId = m.MessageId,
                            MessageDetail = m.MessageDetail,
                            Subject = m.Subject,
                            SendDate = m.SendDate,
                            SenderEmail = m.SenderEmail,
                            SenderName = sender != null ? sender.Name : "Bilinmeyen",
                            SenderSurname = sender != null ? sender.Surname : "Kullanıcı",
                            CategoryName = category != null ? category.CategoryName : "Kategori Yok"
                        };

            // ToList() yerine ToPagedList() kullanarak veritabanı seviyesinde sayfalama yapıyoruz
            var values = query.ToPagedList(page, pageSize);

            return View(values);
        }

        public async Task<IActionResult> Sendbox(int page = 1)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            int pageSize = 10; // Her sayfada gösterilecek mesaj sayısı

            var query = from m in _context.Messages
                        join u in _context.Users on m.ReceiverEmail equals u.Email into userGroup
                        from receiver in userGroup.DefaultIfEmpty()
                        join c in _context.Categories on m.CategoryId equals c.CategoryId into categoryGroup
                        from category in categoryGroup.DefaultIfEmpty()
                        where m.SenderEmail == user.Email
                        orderby m.SendDate descending // En yeni mesajlar üstte görünsün
                        select new MessageWithReceiverInfoViewModel
                        {
                            MessageId = m.MessageId,
                            MessageDetail = m.MessageDetail,
                            Subject = m.Subject,
                            SendDate = m.SendDate,
                            ReceiverEmail = m.ReceiverEmail,
                            ReceiverName = receiver != null ? receiver.Name : "Bilinmeyen",
                            ReceiverSurname = receiver != null ? receiver.Surname : "Kullanıcı",
                            CategoryName = category != null ? category.CategoryName : "Kategori Yok"
                        };

            var values = query.ToPagedList(page, pageSize);

            return View(values);
        }

        public IActionResult MessageDetail(int id)
        {

            var value = _context.Messages.Where(x => x.MessageId == id).FirstOrDefault();
            return View(value);
        }

        [HttpGet]
        public IActionResult ComposeMessage()
        {

            var categories = _context.Categories.ToList();
            ViewBag.v = categories.Select(c => new SelectListItem
            {
                Text = c.CategoryName,
                Value = c.CategoryId.ToString()
            }).ToList();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ComposeMessage(Message message)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            message.SenderEmail = user.Email;
            message.SendDate = DateTime.Now;
            message.IsRead = false;
            _context.Messages.Add(message);
            _context.SaveChanges();
            return RedirectToAction("Sendbox");
        }


        public async Task<IActionResult> GetMessageListByCategory(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var values = (from m in _context.Messages
                          join u in _context.Users
                          on m.SenderEmail equals u.Email into userGroup
                          from sender in userGroup.DefaultIfEmpty()

                          join c in _context.Categories
                          on m.CategoryId equals c.CategoryId into categoryGroup
                          from category in categoryGroup.DefaultIfEmpty()

                          where m.ReceiverEmail == user.Email && m.CategoryId == id
                          select new MessageWithSenderInfoViewModel
                          {
                              MessageId = m.MessageId,
                              MessageDetail = m.MessageDetail,
                              Subject = m.Subject,
                              SendDate = m.SendDate,
                              SenderEmail = m.SenderEmail,
                              SenderName = sender != null ? sender.Name : "Bilinmeyen",
                              SenderSurname = sender != null ? sender.Surname : "Kullanıcı",
                              CategoryName = category != null ? category.CategoryName : "Kategori Yok"
                          }).ToList();

            return View(values);
        }
    }
}
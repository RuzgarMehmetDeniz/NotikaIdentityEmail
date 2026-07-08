using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace NotikaIdentityEmail.Controllers
{
    public class CategoryController : Controller
    {
        private readonly EmailContext _context;

        public CategoryController(EmailContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> CategoryList()
        {
            var token = Request.Cookies["JWToken"];
            if (string.IsNullOrEmpty(token))
            {
                TempData["Error"] = "Giriş yapmadan giremezsiniz.";
                return RedirectToAction("UserLogin", "Login");
            }
            JwtSecurityToken jwt;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                jwt = handler.ReadJwtToken(token);
            }
            catch 
            {
                TempData["Error"] = "Geçersiz token. Lütfen tekrar giriş yapın.";
                return RedirectToAction("UserLogin", "Login");


            }

            var value = _context.Categories.ToList();
            return View(value);
        }
        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("CategoryList");
        }
        public IActionResult DeleteCategory(int id)
        {
            var value = _context.Categories.Find(id);
            _context.Categories.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("CategoryList");
        }
        [HttpGet]
        public IActionResult UpdateCategory(int id)
        {
            var value = _context.Categories.Find(id);
            return View(value);
        }
        [HttpPost]
        public IActionResult UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
            return RedirectToAction("CategoryList");
        }
    }
}

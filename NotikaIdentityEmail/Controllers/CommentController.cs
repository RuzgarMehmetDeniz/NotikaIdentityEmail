using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NotikaIdentityEmail.Controllers
{
    public class CommentController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;
        public CommentController(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult UserComments()
        {
            var values = _context.Comments.Include(x => x.AppUser).ToList();
            return View(values);
        }

        public IActionResult UserCommentList()
        {
            var values = _context.Comments.Include(x => x.AppUser).ToList();
            return View(values);
        }

        [HttpGet]
        public PartialViewResult CreateComment()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(Comment comment)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            comment.AppUserId = user.Id;
            comment.CommentDate = DateTime.Now;
            comment.CommentStatus = "Onay Bekliyor";
            _context.Comments.Add(comment);
            _context.SaveChanges();
            return RedirectToAction("UserCommentList");





            using (var client = new HttpClient())
            {
                var apiKey = "";

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                try

                {

                    var translateRequestBody = new

                    {

                        inputs = comment.CommentDetail

                    };

                    var translateJson = JsonSerializer.Serialize(translateRequestBody);

                    var translateContent = new StringContent(translateJson, Encoding.UTF8, "application/json");

                    var translateResponse = await client.PostAsync("https://api-inference.huggingface.co/models/Helsinki-NLP/opus - mt - tr - en", translateContent);


                    var translateResponseString = await translateResponse.Content.ReadAsStringAsync();

                    string englishText = comment.CommentDetail;

                    if (translateResponseString.TrimStart().StartsWith("["))

                    {

                        var translateDoc = JsonDocument.Parse(translateResponseString);

                        englishText = translateDoc.RootElement[0].GetProperty("translation_text").GetString();

                    }


                    var toxicRequestBody = new

                    {

                        inputs = englishText

                    };

                    var toxicjson = JsonSerializer.Serialize(toxicRequestBody);

                    var toxicContent = new StringContent(toxicjson, Encoding.UTF8, "application/json");

                    var toxicresponse = await client.PostAsync("https://api-inference.huggingface.co/models/unitary/toxic-bert", toxicContent);

                    var toxicresponseString = await toxicresponse.Content.ReadAsStringAsync();

                    if (toxicresponseString.TrimStart().StartsWith("["))

                    {

                        var toxicdoc = JsonDocument.Parse(toxicresponseString);

                        foreach (var item in toxicdoc.RootElement[0].EnumerateArray())

                        {

                            string label = item.GetProperty("label").GetString();

                            double score = item.GetProperty("score").GetDouble();

                            if (score > 0.5)

                            {

                                comment.CommentStatus = "Toxic Comment";

                                break;

                            }

                        }

                    }

                    if (string.IsNullOrEmpty(comment.CommentStatus))
                    {
                        comment.CommentStatus = "Onay Bekliyor";
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the API calls
                    comment.CommentStatus = "Onay Bekliyor";
                }


                _context.Comments.Add(comment);
                _context.SaveChanges();
                return RedirectToAction("UserCommentList");

            }
        }

        public IActionResult DeleteComment(int id)
        {
            var comment = _context.Comments.Find(id);

            _context.Comments.Remove(comment);
            _context.SaveChanges();
            return RedirectToAction("UserCommentList");
        }

        public IActionResult CommentStatusChangeToToxic(int id)
        {
            var comment = _context.Comments.Find(id);
            comment.CommentStatus = "Toxic Yorum";
            _context.SaveChanges();
            return RedirectToAction("UserCommentList");
        }
        public IActionResult CommentStatusChangeToTPassive(int id)
        {
            var comment = _context.Comments.Find(id);
            comment.CommentStatus = "Yorum Kaldırıldı";
            _context.SaveChanges();
            return RedirectToAction("UserCommentList");
        }
        public IActionResult CommentStatusChangeToTActive(int id)
        {
            var comment = _context.Comments.Find(id);
            comment.CommentStatus = "Yorum Onaylandı";
            _context.SaveChanges();
            return RedirectToAction("UserCommentList");
        }
    }
}

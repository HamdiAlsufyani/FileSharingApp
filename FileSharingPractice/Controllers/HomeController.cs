using FileSharingPractice.Data;
using FileSharingPractice.Helpers.Mail;
using FileSharingPractice.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FileSharingPractice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IMailService _imailservice;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,IMailService imailservice)
        {
            _logger = logger;
            this._db = context;
            this._imailservice = imailservice;
        }

        public IActionResult Index()
        {
            var highDownload = _db.Uploads.OrderByDescending(u => u.DownloadCount).Select(u => new UploadViewModel
            {

                OriginalfileName = u.OriginalFileNmae,
                FileName = u.FileName,
                ContentType = u.ContentType,
                Size = u.Size,
                Uploaddate = u.Uploadedate,
                DownloadCount = u.DownloadCount

            })
            .Take(3);
            ViewBag.Popular = highDownload;
            return View();
        }
        private string UserId
        {
            get
            {
                return User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Info()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SetCulture(string lang ,string ReturnUrl=null)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)), new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            }
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                return LocalRedirect(ReturnUrl);
            }
            return RedirectToAction("Index");
        }
        public IActionResult About()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Contact ()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Contact(ContactViewModel contact)
        {
            if (ModelState.IsValid)
            {
                //Save
                _db.Contact.Add(new Data.Contact
                {
                    Email = contact.Email,
                    Name = contact.Name,
                    Supject = contact.Supject,
                    Message = contact.Message,
                    UserId=UserId
                });
                await _db.SaveChangesAsync();
                TempData["Message"] = "Message has been sent successfully";
                //send Mail
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<h1>File Sharing -Unread Message </h1>");
                sb.AppendFormat("Name:{0}", contact.Name);
                sb.AppendFormat("Email:{0}", contact.Email);
                sb.AppendLine();
              
                sb.AppendFormat("subject:{0}", contact.Supject);
                sb.AppendFormat("Message:{0}", contact.Message);
                _imailservice.SendEmail(new InputEmailMessage
                {
                    Subject = "You have Unread Message",
                    Email = "Hamsufy@gmail.com",
                    Body = sb.ToString()

                }) ;
                return RedirectToAction("Contact");

            }
            return View(contact);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

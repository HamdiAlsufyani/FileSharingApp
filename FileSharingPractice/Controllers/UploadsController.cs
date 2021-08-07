using FileSharingPractice.Data;
using FileSharingPractice.Models;
using FileSharingPractice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FileSharingPractice.Controllers
{  [Authorize]
    public class UploadsController : Controller
    {
        private readonly IUploadservice uploadservice;
        private readonly IWebHostEnvironment _env;

        public UploadsController(IUploadservice uploadservice,IWebHostEnvironment env)
        {
            this.uploadservice = uploadservice;
            this._env = env;
        }
        public IActionResult Index()
        {

            var result = uploadservice.Getby(UserId);
            return View(result);
        }
        private async Task<List<UploadViewModel>> GetPageData(IQueryable<UploadViewModel> result,int RequiredPage = 1)
        {
            const int PageSize = 2;

            decimal rowCounts = await uploadservice.GetUploadCount();

            var pageCounts = Math.Ceiling(rowCounts / PageSize);
            if (RequiredPage > pageCounts)
            {
                RequiredPage = 1;
            }
            if (RequiredPage <= 0)
            {
                RequiredPage = 1;
            }
            RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
            ViewBag.CurentPage = 0;
            int Skipcount = (RequiredPage - 1) * PageSize;


            var pageData = await result.Skip(Skipcount)
                      .Take(PageSize).
                      ToListAsync();
            ViewBag.CurentPage = RequiredPage;
            ViewBag.Pagecount = pageCounts;
            return pageData ;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Browse(int RequiredPage=1)
        {
            var result = uploadservice.GetAll();
            var model=await  GetPageData( result,RequiredPage);
            return View(model);
        }
        private string UserId
        {
            get
            {
                return User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Results(string term,int RequiredPage=1)
        {
            var result = uploadservice.Search(term);

            var model = await GetPageData(result, RequiredPage);
            return View(model);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(InputViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newName = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(model.File.FileName);
                var filename =string.Concat( newName , extension);
                var root = _env.WebRootPath;
                var path = Path.Combine(root, "Uploads", filename);
                using (var fs = System.IO.File.Create(path))
                {
                  await  model.File.CopyToAsync(fs);
                }
               await  uploadservice.CreateAsync(new Inputupload 
                {
                   OriginalfileName = model.File.FileName,
                   FileName=filename,
                   ContentType=model.File.ContentType,
                   Size=model.File.Length,
                   Userid=UserId 

                });
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var selletedupload =await uploadservice.FindAsync(id,UserId);
            if (selletedupload == null)
            {
                return NotFound();
            }
        
            return View(selletedupload);
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var selectedupload = await uploadservice.FindAsync(id, UserId);
            if (selectedupload == null)
            {
                return NotFound();
            }

            await uploadservice.DeleteAsync(id, UserId);
            return RedirectToAction("Index");
        }
        //Method For Download
        [HttpGet]
        public async Task<IActionResult> Download(string id)
        {
            var selectedFile = await uploadservice.FindAsync(id);
            if (selectedFile == null)
            {
                return NotFound();
            }
            await uploadservice.IncreamentDownloadCount(id);         
            var path = "~/Uploads" + selectedFile.FileName;
            Response.Headers.Add("Expires", DateTime.Now.AddDays(-3).ToLongDateString());
            Response.Headers.Add("Cache-Control", "no-cache");
            return File(path,selectedFile.ContentType,selectedFile.OriginalfileName);
        }
    }
}

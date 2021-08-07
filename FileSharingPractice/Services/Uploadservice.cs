using AutoMapper;
using AutoMapper.QueryableExtensions;
using FileSharingPractice.Data;
using FileSharingPractice.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileSharingPractice.Services
{
    public class Uploadservice : IUploadservice
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public Uploadservice(ApplicationDbContext context,IMapper mapper)
        {
            this._db = context;
            this._mapper = mapper;
        }
        public async Task CreateAsync(Inputupload model)
        {
            var mapedObj = _mapper.Map<Uploads>(model);
            await _db.Uploads.AddAsync(mapedObj);

            //await _db.Uploads.AddAsync(new Uploads
            //{
            //    OriginalFileNmae = model.OriginalfileName,
            //    FileName = model.FileName,
            //    ContentType = model.ContentType,
            //    Size = model.Size,
            //    UserId = model.Userid,
            //    Uploadedate=DateTime.Now

            //});
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id, string userid)
        {
            var selectedupload = await _db.Uploads.FirstOrDefaultAsync(u => u.Id == id && u.UserId == userid);
            if (selectedupload != null)
            {
                _db.Uploads.Remove(selectedupload);
                await _db.SaveChangesAsync();
            }
        }
        public async  Task<UploadViewModel> FindAsync(string id,string userid)
        {
            var sleectedUpload = await _db.Uploads.FirstOrDefaultAsync(u=>u.Id==id&&u.UserId==userid);
            if (sleectedUpload != null)
            {
                //Auto Mapper
                return _mapper.Map<UploadViewModel>(sleectedUpload);
            //    return new UploadViewModel
            //    {
            //        Id=sleectedUpload.Id,
            //        OriginalfileName = sleectedUpload.OriginalFileNmae,
            //        FileName = sleectedUpload.FileName,
            //        ContentType = sleectedUpload.ContentType,
            //        size = sleectedUpload.Size,
            //        Uploaddate = sleectedUpload.Uploadedate,
            //        DownloadCount = sleectedUpload.DownloadCount
            //    };
            }
            return null;
        }

        public async  Task<UploadViewModel> FindAsync(string id)
        {
            var sleectedUpload = await _db.Uploads.FindAsync(id);
            if (sleectedUpload != null)
            {
                //Auto Mapper
                return _mapper.Map<UploadViewModel>(sleectedUpload);
            }
            return null;
        }

        public IQueryable<UploadViewModel> GetAll()
        {
            var result = _db.Uploads.OrderByDescending(U => U.DownloadCount).ProjectTo<UploadViewModel>(_mapper.ConfigurationProvider);
                //Select(u => new UploadViewModel
                //{
                //    OriginalfileName = u.OriginalFileNmae,
                //    FileName = u.FileName,
                //    ContentType = u.ContentType,
                //    size = u.Size,
                //    Uploaddate = u.Uploadedate,
                //    DownloadCount = u.DownloadCount
                //});
            return result;
        }

        public IQueryable<UploadViewModel> Getby(string userId)
        {
          var result=  _db.Uploads.Where(u => u.UserId == userId).OrderByDescending(U => U.Uploadedate)
                .ProjectTo<UploadViewModel>(_mapper.ConfigurationProvider);
            //.Select(u => new UploadViewModel
            //{
            //    Id = u.Id,
            //    OriginalfileName = u.OriginalFileNmae,
            //    FileName = u.FileName,
            //    ContentType = u.ContentType,
            //    size = u.Size,
            //    Uploaddate = u.Uploadedate,
            //    DownloadCount = u.DownloadCount


            //});
            return result;
        }

        public async Task<int> GetUploadCount()
        {

            return await _db.Uploads.CountAsync();
        } 

        public async Task IncreamentDownloadCount(string id)
        {
            var selectedFile = await _db.Uploads.FindAsync(id);
            if (selectedFile != null)
            {
                selectedFile.DownloadCount++;
                _db.Update(selectedFile);
                await _db.SaveChangesAsync();
            }
        }

        public IQueryable<UploadViewModel> Search(string term)
        {
            var result = _db.Uploads.Where(u => u.OriginalFileNmae.Contains(term))
                .ProjectTo<UploadViewModel>(_mapper.ConfigurationProvider);
            //Select(u => new UploadViewModel
            //{
            //    OriginalfileName = u.OriginalFileNmae,
            //    FileName = u.FileName,
            //    ContentType = u.ContentType,
            //    size = u.Size,
            //    Uploaddate = u.Uploadedate,
            //    DownloadCount = u.DownloadCount
            //});
            return result;
        }
    }
}

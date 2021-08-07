using FileSharingPractice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileSharingPractice.Services
{
  public  interface IUploadservice
    {
        IQueryable<UploadViewModel> GetAll();
        IQueryable<UploadViewModel> Getby(string userId);
        IQueryable<UploadViewModel> Search(string term);
    
        Task CreateAsync(Inputupload model);
        Task<UploadViewModel> FindAsync(string id);
        Task<UploadViewModel> FindAsync(string id,string userid);
        Task DeleteAsync(string id,string userid);
        Task IncreamentDownloadCount(string id);
        Task<int> GetUploadCount();
    }
}

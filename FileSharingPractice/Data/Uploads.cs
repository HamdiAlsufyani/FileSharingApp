using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace FileSharingPractice.Data
{
    public class Uploads
    {
        public Uploads()
        {
            Id = Guid.NewGuid().ToString();
            Uploadedate = DateTime.Now;
        }
        public string Id { get; set; }
        public string OriginalFileNmae { get; set; }
        public String FileName { get; set; }
        public string ContentType { get; set; }
        public decimal Size { get; set; }
        public string UserId { get; set; }
        public DateTime Uploadedate  { get; set; }
        public ApplicationUser User { get; set; }
        public long  DownloadCount { get; set; }
    }
}

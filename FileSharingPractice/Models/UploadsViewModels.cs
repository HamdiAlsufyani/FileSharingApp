using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FileSharingPractice.Models
{
    public class InputViewModel
    {
        [Required]
        public IFormFile File { get; set; }
    }
    public class Inputupload
    {
        public string OriginalfileName  { get; set; }
        public string  FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public string Userid { get; set; }
    }
    public class UploadViewModel
    {
        public string Id { get; set; }
        public string OriginalfileName { get; set; }
        public string FileName { get; set; }
        public decimal  Size { get; set; }
        public string ContentType { get; set; }
        public DateTime Uploaddate { get; set; }
        public long DownloadCount { get; internal set; }
    }
}

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileSharingPractice
{
    public class Uplodprofile:Profile
    {
        public Uplodprofile()
        {
            CreateMap<Models.Inputupload, Data.Uploads>().ForMember(u=>u.Id,op=>op.Ignore()).ForMember(u=>u.Uploadedate,op=>op.Ignore());
            CreateMap<Data.Uploads, Models.UploadViewModel>();
        }
    }
   public  class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Data.ApplicationUser, Models.UserViewModel>().ForMember(u=>u.HasPassword,op=>op.MapFrom(u=>u.PasswordHash!=null));
        }
    }
}

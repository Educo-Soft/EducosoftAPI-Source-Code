using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EducosoftAPI.Models.User
{
    public class UserForgetPasswordDetails
    {
        public string userId { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}
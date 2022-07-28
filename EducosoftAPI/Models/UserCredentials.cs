using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EducosoftAPI.Models
{
    public class UserCredential
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string MobileNo { get; set; }
    }

    public class UserCredentialSurveyFeedback
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Feedback { get; set; }
    }

    public class UserVerifyLoginCredential
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
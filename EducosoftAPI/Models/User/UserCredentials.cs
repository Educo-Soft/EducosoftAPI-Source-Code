using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EducosoftAPI.Models.User
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
   

    public class UserViewCredential
    {
        [Required]
        public string UserId { get; set; }
    }

    public class UserProfileEditCredential
    {
        [Required]
        public string UserId { get; set; }

        //[Required]
        //public string Time_Zone { get; set; }

        //[Required]
        //public string EMail_Id { get; set; }

        //[Display(Name = "Student Id")]
        //public string StudentId { get; set; }

        [Required]
        public string Salutation { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        //[Required]
        //public string LanguageId { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        [Required]
        public string CountryId { get; set; }
       
        [Required]
        public string StateId { get; set; }

        public string ZipCode { get; set; }

        public string Phone { get; set; }

        public string Mobile { get; set; }

        public string strImage { get; set; }
        public string strImageType { get; set; }

    }

    public class UserForgetPasswordCredential
    {
        [Required]
        public string Email { get; set; }
    }

    public class UserResetPasswordCredential
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    
    }
    public class UserSupportCredential
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string SectionId { get; set; }
        [Required]
        public string UserType { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string TellAboutText { get; set; }
        public string AttachmentFile { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

    }


}
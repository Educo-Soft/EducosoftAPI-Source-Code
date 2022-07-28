using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EducosoftAPI.Models
{
    public class CourseCredential
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string UserType { get; set; }
    }

    public class CourseLevelCredential
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string SectionId{ get; set; }
        [Required]
        public string CourseId { get; set; }
        [Required]
        public string CRLevelId { get; set; }
    }
   
}
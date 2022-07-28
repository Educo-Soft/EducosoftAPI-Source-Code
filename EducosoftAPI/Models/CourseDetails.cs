using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EducosoftAPI.Models
{
    public class CourseDetails
    {
        public string CourseId { get; set;  }
        public string CRLevelId { get; set; }
        public string Course_Name { get; set; }
        public string TermId { get; set; }
        public string Term_Name { get; set; }
        public string SectionId { get; set; }
        public string Section_Name { get; set; }
    }

    public class CourseLevelSheduledDetails
    {
        public string ScheduledLevelId {get; set;}
        public string ScheduledFromdate { get; set; }
        public string ScheduledTodate { get; set; }
    }
}
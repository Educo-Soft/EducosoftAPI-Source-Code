using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EducosoftAPI.Models.Announcement
{
    public class AnnouncementCredential
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string SectionId { get; set; }
        //[Required]
        //public string PortalID { get; set; }
        [Required]
        public string Announcement_Type { get; set; }      //Announcement_Type 0-Genral_Announ,3-Course_Announ
        [Required]
        public string Announcement_OptSel { get; set; }    //Announcement_OptSel 0-All,1-Today,2-Last 7days,3-Last 30days 
        [Required]
        public string UserCurrZoneDate { get; set; }
    }
}
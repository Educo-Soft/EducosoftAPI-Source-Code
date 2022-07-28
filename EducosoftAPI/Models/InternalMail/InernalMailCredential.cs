using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EducosoftAPI.Models.InternalMail
{
    public class InternalMailCredential
    {
        [Required]
        public string UserId { get; set; }
    }

    public class InternalMailViewCredential
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string MsgSenderId { get; set; }
    }
    public class DeleteInternalMailCredential
    {
        [Required]
        public string MsgRecieverId { get; set; }
    }

    public class SendInternalMailCredential
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string SectionId { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public string MsgSendToUsersIds { get; set; }
        [Required]
        public string MsgSendBccUsersIds { get; set; }
        [Required]
        public string AttachmentBase64StringFiles { get; set; }
        [Required]
        public string AttachmentOriginalFiles { get; set; }
        
    }
}
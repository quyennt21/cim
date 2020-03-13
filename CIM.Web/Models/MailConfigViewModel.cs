using System;
using System.ComponentModel.DataAnnotations;

namespace CIM.Web.Models
{
    [MetadataType(typeof(MailConfigViewModel))]
    public class MailConfigViewModel
    {
        public int ID { get; set; }

        //[Remote("IsMailConfig", "MailConfigs", ErrorMessage = "Email exists.")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Please enter correct email")]
        [StringLength(255)]
        [Required]
        public string EmailAddress { get; set; }

        [Display(Name = ("Password"))]
        public string Password { get; set; }

        public string Port { get; set; }

        public string Host { get; set; }

        public bool EnabledSSL { get; set; }

        public int TemplateID { get; set; }
        public int Count { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateSend { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedAt { get; set; }

        public bool Active { get; set; }
    }
}
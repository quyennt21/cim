using CIM.Model.Abstract;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIM.Model.Models
{
    [Table("mail_config")]
    public class MailConfig : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(256)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter correct email")]
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(256)]
        public string Password { get; set; }

        [Required]
        [MaxLength(256)]
        public string Port { get; set; }

        [Required]
        [MaxLength(256)]
        public string Host { get; set; }

        [Required]
        public bool EnabledSSL { get; set; }

        [DefaultValue(0)]
        public int Count { get; set; }

        //        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? DateSend { get; set; }

        public int TemplateID { get; set; }
    }
}
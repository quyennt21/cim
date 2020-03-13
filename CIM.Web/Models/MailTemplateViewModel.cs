using System;

namespace CIM.Web.Models
{
    public class MailTemplateViewModel
    {
        public int ID { get; set; }
        public string Template { get; set; }
        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool Active { get; set; }
    }
}
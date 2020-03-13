using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CIM.Web.Models
{
    public class AssetLogViewModel
    {
        public int ID { get; set; }

        [DataType(DataType.MultilineText)]
        public string Message { get; set; }

        public virtual UserViewModel ApplicationUser { get; set; }

        public virtual AssetViewModel Asset { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedAt { get; set; }

        [DisplayName("Enable")]
        public bool Active { get; set; }
    }
}
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CIM.Web.Models
{
    public class MaintenanceDiaryViewModel
    {
        public int ID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime MaintenanceDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public virtual AssetViewModel Asset { get; set; }

        [DataType(DataType.Date)]
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }

        [DataType(DataType.Date)]
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedAt { get; set; }

        [DisplayName("Enable")]
        public bool Active { get; set; }
    }
}
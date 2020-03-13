using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CIM.Web.Models
{
    public class AreaViewModel
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(256)]
        [DisplayName("Area Code")]
        public string AreaCode { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public int LocationID { get; set; }
        public virtual LocationViewModel Location { get; set; }

        public virtual IEnumerable<AssetViewModel> Asset { get; set; }

        public virtual IEnumerable<PICViewModel> PIC { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedAt { get; set; }

        [DisplayName("In Service")]
        public bool Active { get; set; }
    }
}
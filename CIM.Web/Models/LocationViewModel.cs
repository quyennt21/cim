using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CIM.Web.Models
{
    public class LocationViewModel
    {
        public int ID { get; set; }

        [DisplayName("Location Code")]
        [Required]
        [MaxLength(256)]
        public string LocationCode { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public virtual CampusViewModel Campus { get; set; }
        public int CampusID { get; set; }
        public virtual IEnumerable<AreaViewModel> Area { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedAt { get; set; }

        public bool Active { get; set; }
    }
}
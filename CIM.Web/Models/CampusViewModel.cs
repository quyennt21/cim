using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CIM.Web.Models
{
    public class CampusViewModel
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(256)]
        [StringLength(256)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        public string Telephone { get; set; }

        public virtual IEnumerable<LocationViewModel> Location { get; set; }

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
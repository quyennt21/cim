using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIM.Web.Models
{
    public class AssetViewModel
    {
        public int ID { get; set; }

        public string AssetCode { get; set; }

        [Required]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter an integer number greater than 0")]
        public int Quantity { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartDate { get; set; }

        public string Image { get; set; }
        public int AssetTypeID { get; set; }
        public int AreaID { get; set; }
        public int ApplicationUserID { get; set; }

        public virtual AssetTypeViewModel AssetType { get; set; }

        public virtual UserViewModel ApplicationUser { get; set; }

        public virtual AreaViewModel Area { get; set; }

        public virtual IEnumerable<AssetAttributeValueViewModel> AssetAttributeValue { get; set; }

        public virtual IEnumerable<WarrantyViewModel> Warranty { get; set; }

        public virtual IEnumerable<MaintenanceDiaryViewModel> MaintenanceDiary { get; set; }

        public virtual IEnumerable<ReportViewModel> Report { get; set; }

        public virtual IEnumerable<AssetLogViewModel> Log { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedAt { get; set; }

        public bool Active { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace CIM.Web.Models
{
    public class AssetAttributeValueViewModel
    {
        public int ID { get; set; }

        public string Value { get; set; }
        public int AssetAttributeID { get; set; }
        public virtual AssetViewModel Asset { get; set; }

        public virtual AssetTypeAttributeViewModel AssetTypeAttribute { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedAt { get; set; }

        public bool Active { get; set; }
    }
}
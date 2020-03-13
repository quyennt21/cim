using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CIM.Web.Models
{
    public class AssetTypeAttributeViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public int AssetTypeID { get; set; }

        public virtual IEnumerable<AssetAttributeValueViewModel> AssetAttributeValue { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedAt { get; set; }

        public bool Active { get; set; }
    }
}
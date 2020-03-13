using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CIM.Web.Models
{
    public class AssetTypeViewModel
    {
        public int ID { get; set; }

        [Remote("IsAssetType", "AssetTypes", HttpMethod = "POST", ErrorMessage = "Asset Type Name exists.")]
        public string Name { get; set; }

        public virtual IEnumerable<AssetViewModel> Asset { get; set; }

        public virtual IEnumerable<PICViewModel> PIC { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public bool Active { get; set; }
    }
}
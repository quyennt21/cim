using System;
using System.ComponentModel.DataAnnotations;

namespace CIM.Web.Models
{
    public class ReportViewModel
    {
        public int ID { get; set; }

        public int status { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReportAt { get; set; }

        public string Picture { get; set; }

        public string UserReport { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        public string RequestManager { get; set; }

        public virtual AssetViewModel Asset { get; set; }

        public virtual RateViewModel Rate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedAt { get; set; }

        public bool Active { get; set; }
    }
}
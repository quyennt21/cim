using System;
using System.ComponentModel.DataAnnotations;

namespace CIM.Web.Models
{
    public class RateViewModel
    {
        public int ID { get; set; }

        public decimal Value { get; set; }

        public string Comment { get; set; }

        public virtual ReportViewModel Report { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedAt { get; set; }

        public bool Active { get; set; }
    }
}
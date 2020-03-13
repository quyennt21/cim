using CIM.Model.Abstract;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIM.Model.Models
{
    [Table("report")]
    public class Report : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int status { get; set; }

        [Required]
        public DateTime ReportAt { get; set; }

        [MaxLength(256)]
        public string Picture { get; set; }

        [MaxLength(100)]
        public string UserReport { get; set; }

        [MaxLength(500)]
        public string Comment { get; set; }

        [MaxLength(100)]
        public string RequestManager { get; set; }

        public int? AssetID { get; set; }

        [ForeignKey("AssetID")]
        public virtual Asset Asset { get; set; }

        public virtual Rate Rate { get; set; }
    }
}
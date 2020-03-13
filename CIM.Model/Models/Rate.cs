using CIM.Model.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIM.Model.Models
{
    [Table("rate")]
    public class Rate : Auditable
    {
        [Required]
        public decimal Value { get; set; }

        [MaxLength(500)]
        public string Comment { get; set; }

        [Key, ForeignKey("Report")]
        public int ReportID { get; set; }

        public virtual Report Report { get; set; }
    }
}
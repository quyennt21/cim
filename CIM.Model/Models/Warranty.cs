using CIM.Model.Abstract;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIM.Model.Models
{
    [Table("warranty")]
    public class Warranty : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public DateTime DateWarranty { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Reason { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Result { get; set; }

        [Required]
        public int AssetID { get; set; }

        [ForeignKey("AssetID")]
        public virtual Asset Asset { get; set; }
    }
}
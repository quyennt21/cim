using CIM.Model.Abstract;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIM.Model.Models
{
    [Table("maintenance_diary")]
    public class MaintenanceDiary : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public DateTime MaintenanceDate { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public int AssetID { get; set; }

        [ForeignKey("AssetID")]
        public virtual Asset Asset { get; set; }
    }
}
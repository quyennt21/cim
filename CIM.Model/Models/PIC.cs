using CIM.Model.Abstract;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIM.Model.Models
{
    [Table("PIC")]
    public class PIC : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int AssetTypeID { get; set; }

        [ForeignKey("AssetTypeID")]
        public virtual AssetType AssetType { get; set; }

        [Required]
        public int ApplicationUserID { get; set; }

        [ForeignKey("ApplicationUserID")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public int AreaID { get; set; }

        [ForeignKey("AreaID")]
        public virtual Area Area { get; set; }
    }
}
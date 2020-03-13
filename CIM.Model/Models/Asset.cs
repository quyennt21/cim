using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIM.Model.Models
{
    [Table("asset")]
    public class Asset
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(256)]
        public string AssetCode { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public string Image { get; set; }

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

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool Active { get; set; }

        public virtual IEnumerable<AssetAttributeValue> AssetAttributeValue { get; set; }

        public virtual IEnumerable<Warranty> Warranty { get; set; }

        public virtual IEnumerable<MaintenanceDiary> MaintenanceDiary { get; set; }

        public virtual IEnumerable<Report> Report { get; set; }

        public virtual IEnumerable<AssetLog> AssetLog { get; set; }
    }
}
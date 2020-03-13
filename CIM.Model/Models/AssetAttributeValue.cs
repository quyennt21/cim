using CIM.Model.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIM.Model.Models
{
    [Table("asset_attribute_value")]
    public class AssetAttributeValue : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(500)]
        public string Value { get; set; }

        [Required]
        public int AssetID { get; set; }

        [ForeignKey("AssetID")]
        public virtual Asset Asset { get; set; }

        [Required]
        public int AssetAttributeID { get; set; }

        [ForeignKey("AssetAttributeID")]
        public virtual AssetTypeAttribute AssetTypeAttribute { get; set; }
    }
}
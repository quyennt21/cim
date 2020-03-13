using CIM.Model.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIM.Model.Models
{
    [Table("asset_type_attribute")]
    public class AssetTypeAttribute : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        [Required]
        public int AssetTypeID { get; set; }

        public virtual IEnumerable<AssetAttributeValue> AssetAttributeValue { get; set; }
    }
}
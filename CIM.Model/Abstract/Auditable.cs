using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIM.Model.Abstract
{
    public abstract class Auditable : IAuditable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdatedAt { get; set; }

        public bool Active { get; set; }
    }
}
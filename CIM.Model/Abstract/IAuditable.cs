using System;

namespace CIM.Model.Abstract
{
    public interface IAuditable
    {
        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        bool Active { get; set; }
    }
}
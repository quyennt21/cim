using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CIM.Web.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[a-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Please enter correct email")]
        public string Email { get; set; }

        public string FullName { get; set; }

        public virtual RoleViewModel Role { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedAt { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedAt { get; set; }

        [DisplayName("Enable")]
        public bool Active { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace ShopTheThao.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string FullName { get; set; }
        [Required, StringLength(150)] public string Email { get; set; }
        [Required] public string PasswordHash { get; set; }
        [StringLength(15)] public string Phone { get; set; }
        [StringLength(250)] public string Address { get; set; }
        public string AvatarUrl { get; set; }
        [Required, StringLength(20)] public string Role { get; set; } = "Customer"; // Admin | Customer
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        public string? FirstName { get; set; }
        
        [StringLength(100)]
        public string? LastName { get; set; }
    }
}

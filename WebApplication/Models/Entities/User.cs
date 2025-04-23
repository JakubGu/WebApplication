using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace WebApplication.Models.Entities
{
    public class User : IdentityUser
    {
        public virtual ICollection<Translation> Translations { get; set; } = new List<Translation>();
    }
}
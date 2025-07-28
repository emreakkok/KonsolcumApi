using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Domain.Entities.Identity
{
    public class AppRole : IdentityRole<string>
    {
        public virtual ICollection<Endpoint> Endpoints { get; set; } = new List<Endpoint>();
    }
}

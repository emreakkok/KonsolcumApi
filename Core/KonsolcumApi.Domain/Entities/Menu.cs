using KonsolcumApi.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Domain.Entities
{
    public class Menu : BaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<Endpoint> Endpoints { get; set; } = new List<Endpoint>();
    }
}

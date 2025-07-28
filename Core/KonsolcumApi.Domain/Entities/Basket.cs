using KonsolcumApi.Domain.Entities.Common;
using KonsolcumApi.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Domain.Entities
{
    public class Basket : BaseEntity
    {
        public string UserId { get; set; }

        public AppUser User { get; set; }
        public Order Order { get; set; }
        public virtual ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Domain.Entities.Common
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // Birincil anahtar (GUID)
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;      // Oluşturulma tarihi
        virtual public DateTime? UpdatedDate { get; set; }     // Güncellenme tarihi (nullable)
    }
}

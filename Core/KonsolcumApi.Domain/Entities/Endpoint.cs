﻿using KonsolcumApi.Domain.Entities.Common;
using KonsolcumApi.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Domain.Entities
{
    public class Endpoint : BaseEntity
    {
        public Endpoint() 
        { 
            Roles = new HashSet<AppRole>();
        }
        public string ActionType { get; set; }
        public string HttpType { get; set; }
        public string Definition { get; set; }
        public string Code { get; set; }
        public Menu Menu { get; set; }
        public virtual ICollection<AppRole> Roles { get; set; }
    }
}

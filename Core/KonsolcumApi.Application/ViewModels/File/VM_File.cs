using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.ViewModels.File
{
    public class VM_File
    {
        public Guid id { get; set; }
        public string fileName { get; set; }
        public string path { get; set; } // Dosyaya erişim URL'i
        public DateTime createdDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.DTOs.File
{
    public class FileDTO
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }
        public bool Showcase { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ProductId { get; set; }
        public string CategoryId { get; set; }
    }
}

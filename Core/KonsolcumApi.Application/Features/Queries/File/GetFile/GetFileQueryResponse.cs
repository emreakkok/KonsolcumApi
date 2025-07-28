using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.File.GetFile
{
    public class GetFileQueryResponse
    {
        public List<FileDto> Files { get; set; }

        public class FileDto
        {
            public Guid id { get; set; }
            public string fileName { get; set; }
            public string path { get; set; }
            public DateTime createdDate { get; set; }
        }
    }
}

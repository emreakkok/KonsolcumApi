using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.File.UploadFile
{
    public class UploadFileCommandResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<object> Files { get; set; }
    }
}

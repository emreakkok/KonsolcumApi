using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.File.UploadFile
{
    public class UploadFileCommandRequest : IRequest<UploadFileCommandResponse>
    {
        public string id { get; set; } // Category ID
        public string type { get; set; } // "category" veya "product"
        public IFormFileCollection files { get; set; } // Dosyalar
    }
}

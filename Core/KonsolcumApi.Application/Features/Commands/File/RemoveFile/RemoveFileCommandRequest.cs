using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.File.RemoveFile
{
    public class RemoveFileCommandRequest : IRequest<RemoveFileCommandResponse>
    {
        public Guid ImageId { get; set; }
    }
}

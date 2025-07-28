using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.File.GetFile
{
    public class GetFileQueryRequest : IRequest<GetFileQueryResponse>
    {
        public Guid Id { get; set; }
    }
}

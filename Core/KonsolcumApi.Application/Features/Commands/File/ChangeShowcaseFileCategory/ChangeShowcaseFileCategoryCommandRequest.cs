using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.File.ChangeShowcaseFileCategory
{
    public class ChangeShowcaseFileCategoryCommandRequest : IRequest<ChangeShowcaseFileCategoryCommandResponse>
    {
        public string ImageId { get; set; }
        public string CategoryId { get; set; }
    }
}

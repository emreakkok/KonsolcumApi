using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Commands.Category.UpdateCategory
{
    public class UpdateCategoryCommandRequest : IRequest<UpdateCategoryCommandResponse>
    {
        public string id { get; set; }

        public string name { get; set; }

        public string description { get; set; }


        public bool isActive { get; set; }
    }
}

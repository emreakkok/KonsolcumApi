
using MediatR;

namespace KonsolcumApi.Application.Features.Commands.File.ChangeShowcaseFile
{
    public class ChangeShowcaseFileCommandRequest : IRequest<ChangeShowcaseFileCommandResponse>
    {
        public string ImageId { get; set; }
        public string ProductId { get; set; }
    }
}
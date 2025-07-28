using KonsolcumApi.Application.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.File.GetFile
{
    public class GetFileQueryHandler : IRequestHandler<GetFileQueryRequest, GetFileQueryResponse>
    {
        private readonly IFileRepository _fileRepository;

        public GetFileQueryHandler(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<GetFileQueryResponse> Handle(GetFileQueryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Önce CategoryId ile dene
                var files = await _fileRepository.GetFilesByCategoryIdAsync(request.Id);

                // Eğer category dosyası bulunamazsa ProductId ile dene
                if (files == null || !files.Any())
                {
                    files = await _fileRepository.GetFilesByProductIdAsync(request.Id);
                }

                var response = new GetFileQueryResponse
                {
                    Files = files?.Select(f => new GetFileQueryResponse.FileDto
                    {
                        id = f.Id,
                        fileName = f.FileName,
                        path = f.Path,
                        createdDate = f.CreatedDate
                    }).ToList() ?? new List<GetFileQueryResponse.FileDto>()
                };

                return response;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error getting files for ID {request.Id}: {ex.Message}");
                return new GetFileQueryResponse
                {
                    Files = new List<GetFileQueryResponse.FileDto>()
                };
            }
        }
    }
}

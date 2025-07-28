using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Application.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace KonsolcumApi.Application.Features.Commands.File.RemoveFile
{
    public class RemoveFileCommandHandler : IRequestHandler<RemoveFileCommandRequest, RemoveFileCommandResponse>
    {
        readonly IFileService _fileService;
        readonly IFileRepository _fileRepository;
        public RemoveFileCommandHandler(IFileService fileService, IFileRepository fileRepository)
        {
            _fileService = fileService;
            _fileRepository = fileRepository;
        }
        public async Task<RemoveFileCommandResponse> Handle(RemoveFileCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var fileEntity = await _fileRepository.GetByIdAsync(request.ImageId);
                if (fileEntity == null)
                {
                    return new RemoveFileCommandResponse
                    {
                        Success = false,
                        Message = "Image not found."
                    };
                }

                var isDeleted = await _fileRepository.DeleteAsync(request.ImageId);
                if (isDeleted)
                {
                    await _fileService.DeleteFileAsync(fileEntity.Path);

                    return new RemoveFileCommandResponse
                    {
                        Success = true,
                        Message = "Image deleted successfully."
                    };
                }

                return new RemoveFileCommandResponse
                {
                    Success = false,
                    Message = "Failed to delete image from database."
                };
            }
            catch (Exception ex)
            {
                return new RemoveFileCommandResponse
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}"
                };
            }
        }
    }
}

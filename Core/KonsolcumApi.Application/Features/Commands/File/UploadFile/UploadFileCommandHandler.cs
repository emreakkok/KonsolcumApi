using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Application.Services;
using MediatR;

namespace KonsolcumApi.Application.Features.Commands.File.UploadFile
{
    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommandRequest, UploadFileCommandResponse>
    {
        readonly IFileService _fileService;
        readonly IFileRepository _fileRepository;

        public UploadFileCommandHandler(IFileService fileService, IFileRepository fileRepository)
        {
            _fileService = fileService;
            _fileRepository = fileRepository;
        }

        public async Task<UploadFileCommandResponse> Handle(UploadFileCommandRequest request, CancellationToken cancellationToken)
        {
            System.Console.WriteLine($"UploadImages metodu çağrıldı. Gelen Category ID: {request.id}");

            // Dosya kontrolü
            if (request.files == null || request.files.Count == 0)
            {
                return new UploadFileCommandResponse
                {
                    Success = false,
                    Message = "Yüklenecek dosya bulunamadı.",
                    Files = new List<object>()
                };
            }

            // ID kontrolü
            if (!Guid.TryParse(request.id, out Guid entityId))
            {
                return new UploadFileCommandResponse
                {
                    Success = false,
                    Message = "Geçersiz kimlik.",
                    Files = new List<object>()
                };
            }

            if (string.IsNullOrEmpty(request.type) ||
                (request.type.ToLower() != "category" && request.type.ToLower() != "product"))
            {
                return new UploadFileCommandResponse
                {
                    Success = false,
                    Message = "Geçersiz dosya tipi. 'category' veya 'product' olmalıdır.",
                    Files = new List<object>()
                };
            }


            try
            {
                // Type'a göre upload path'i belirle
                string uploadPath = request.type.ToLower() == "category"
                    ? $"resources/category-images/{entityId}"
                    : $"resources/product-images/{entityId}";

                // FileService ile dosyaları fiziksel olarak yükle
                var uploadedFiles = await _fileService.UploadAsync(uploadPath, request.files);

                if (uploadedFiles == null || !uploadedFiles.Any())
                {
                    return new UploadFileCommandResponse
                    {
                        Success = false,
                        Message = "Dosya yükleme başarısız.",
                        Files = new List<object>()
                    };
                }

                // Type'a göre veritabanına kaydet
                var fileEntities = uploadedFiles.Select(f => new KonsolcumApi.Domain.Entities.File.File
                {
                    Id = Guid.NewGuid(),
                    FileName = f.fileName,
                    Path = f.path,
                    CategoryId = request.type.ToLower() == "category" ? entityId : null,
                    ProductId = request.type.ToLower() == "product" ? entityId : null,
                    CreatedDate = DateTime.UtcNow
                }).ToList();

                await _fileRepository.CreateRangeAsync(fileEntities);

                string successMessage = request.type.ToLower() == "category"
                    ? "Kategori resimleri başarıyla yüklendi."
                    : "Ürün resimleri başarıyla yüklendi.";

                return new UploadFileCommandResponse
                {
                    Success = true,
                    Message = successMessage,
                    Files = uploadedFiles.Select(f => new { f.fileName, f.path }).ToList<object>()
                };
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error uploading {request.type} images: {ex.Message}");
                return new UploadFileCommandResponse
                {
                    Success = false,
                    Message = $"Internal server error: {ex.Message}",
                    Files = new List<object>()
                };
            }
        }
    }
}
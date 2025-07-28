using KonsolcumApi.Application.Services;
using KonsolcumApi.Infrastructure.Operations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KonsolcumApi.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileService> _logger;

        public FileService(IWebHostEnvironment webHostEnvironment, ILogger<FileService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<bool> CopyFileAsync(string path, IFormFile file)
        {
            try
            {
                // Dosya boyutu kontrolü (örneğin 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    _logger.LogWarning($"File size too large: {file.Length} bytes");
                    return false;
                }

                // Dizin varsa kontrol et, yoksa oluştur
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: true);
                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();

                _logger.LogInformation($"File successfully copied to: {path}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error copying file to path: {path}");
                return false; // throw yerine false döndür
            }
        }

        private async Task<string> FileRenameAsync(string path, string fileName, bool first = true)
        {
            return await Task.Run(() =>
            {
                string extension = Path.GetExtension(fileName);
                string newFileName = string.Empty;

                if (first)
                {
                    string oldName = Path.GetFileNameWithoutExtension(fileName);
                    newFileName = $"{NameOperation.CharacterRegulatory(oldName)}{extension}";
                }
                else
                {
                    newFileName = fileName;
                    int indexNo1 = newFileName.LastIndexOf("-");

                    if (indexNo1 == -1)
                    {
                        newFileName = $"{Path.GetFileNameWithoutExtension(newFileName)}-2{extension}";
                    }
                    else
                    {
                        int indexNo2 = newFileName.LastIndexOf(".");
                        if (indexNo2 > indexNo1)
                        {
                            string fileNo = newFileName.Substring(indexNo1 + 1, indexNo2 - indexNo1 - 1);
                            if (int.TryParse(fileNo, out int _fileNo))
                            {
                                _fileNo++;
                                newFileName = newFileName.Remove(indexNo1 + 1, indexNo2 - indexNo1 - 1).Insert(indexNo1 + 1, _fileNo.ToString());
                            }
                            else
                            {
                                newFileName = $"{Path.GetFileNameWithoutExtension(newFileName)}-2{extension}";
                            }
                        }
                        else
                        {
                            newFileName = $"{Path.GetFileNameWithoutExtension(newFileName)}-2{extension}";
                        }
                    }
                }

                if (File.Exists(Path.Combine(path, newFileName)))
                    return FileRenameAsync(path, newFileName, false).Result;
                else
                    return newFileName;
            });
        }

        public async Task<List<(string fileName, string path)>> UploadAsync(string path, IFormFileCollection files)
        {
            try
            {
                string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, path);

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                List<(string fileName, string path)> datas = new();

                foreach (IFormFile file in files)
                {
                    if (file.Length > 0)
                    {
                        string fileNewName = await FileRenameAsync(uploadPath, file.FileName);
                        string fullPath = Path.Combine(uploadPath, fileNewName);

                        bool result = await CopyFileAsync(fullPath, file);

                        if (result)
                        {
                            // Path'i web için uygun formata çevir - wwwroot prefix'i EKLEME
                            string relativePath = Path.Combine(path, fileNewName).Replace('\\', '/');
                            datas.Add((fileNewName, relativePath));

                            _logger.LogInformation($"File uploaded successfully: {fileNewName}, Path: {relativePath}");
                        }
                        else
                        {
                            _logger.LogError($"Failed to copy file: {file.FileName}");
                        }
                    }
                }

                return datas.Any() ? datas : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UploadAsync method");
                return null;
            }
        }
        public async Task<bool> DeleteFileAsync(string relativePath)
        {
            try
            {
                string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.Replace('/', '\\'));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation($"File deleted: {fullPath}");
                    return true;
                }

                _logger.LogWarning($"File not found for deletion: {fullPath}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file: {relativePath}");
                return false;
            }
        }
    }
}
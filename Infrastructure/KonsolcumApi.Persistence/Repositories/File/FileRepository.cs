using Dapper;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Domain.Entities.File;
using KonsolcumApi.Persistence.Contexts.DapperContext;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KonsolcumApi.Persistence.Repositories
{
    public class FileRepository : Repository<KonsolcumApi.Domain.Entities.File.File>, IFileRepository
    {
        public FileRepository(Context context) : base(context)
        {
        }

        public async Task<IEnumerable<KonsolcumApi.Domain.Entities.File.File>> GetFilesByCategoryIdAsync(Guid categoryId)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE CategoryId = @CategoryId ORDER BY CreatedDate DESC";
            return await connection.QueryAsync<KonsolcumApi.Domain.Entities.File.File>(sql, new { CategoryId = categoryId });
        }

        public async Task<IEnumerable<KonsolcumApi.Domain.Entities.File.File>> GetFilesByProductIdAsync(Guid productId)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE ProductId = @ProductId ORDER BY CreatedDate DESC";
            return await connection.QueryAsync<KonsolcumApi.Domain.Entities.File.File>(sql, new { ProductId = productId });
        }

        public override async Task<KonsolcumApi.Domain.Entities.File.File> CreateAsync(KonsolcumApi.Domain.Entities.File.File entity)
        {
            using var connection = _context.CreateConnection();
            entity.Id = Guid.NewGuid();
            entity.CreatedDate = DateTime.UtcNow;
            entity.UpdatedDate = DateTime.UtcNow;

            var sql = @"INSERT INTO Files (Id, FileName, Path, CategoryId, ProductId, CreatedDate, UpdatedDate) 
                       VALUES (@Id, @FileName, @Path, @CategoryId, @ProductId, @CreatedDate, @UpdatedDate)";

            await connection.ExecuteAsync(sql, entity);
            return entity;
        }

        public override async Task<IEnumerable<KonsolcumApi.Domain.Entities.File.File>> CreateRangeAsync(IEnumerable<KonsolcumApi.Domain.Entities.File.File> entities)
        {
            using var connection = _context.CreateConnection();
            var now = DateTime.UtcNow;

            foreach (var entity in entities)
            {
                entity.Id = Guid.NewGuid();
                entity.CreatedDate = now;
                entity.UpdatedDate = now;
            }

            var sql = @"INSERT INTO Files (Id, FileName, Path, CategoryId, ProductId, CreatedDate, UpdatedDate) 
                       VALUES (@Id, @FileName, @Path, @CategoryId, @ProductId, @CreatedDate, @UpdatedDate)";

            await connection.ExecuteAsync(sql, entities);
            return entities;
        }
    }
}
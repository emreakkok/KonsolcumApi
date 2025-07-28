using Dapper;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Domain.Entities.Common;
using KonsolcumApi.Persistence.Contexts.DapperContext;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly Context _context;
        protected readonly string _tableName;

        public Repository(Context context)
        {
            _context = context;
            var entityName = typeof(T).Name;
            _tableName = entityName == "Category" ? "Categories" : entityName + "s";
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<T>(sql, new { Id = id });
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            var sql = $"SELECT * FROM {_tableName}";
            return await connection.QueryAsync<T>(sql);
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            using var connection = _context.CreateConnection();
            entity.Id = Guid.NewGuid();
            entity.CreatedDate = DateTime.UtcNow;
            entity.UpdatedDate = DateTime.UtcNow;

            //entitydeki diğer verileri getirmek için
            // YALNIZCA VERİTABANI SÜTUNLARINA KARŞILIK GELEN ÖZELLİKLERİ SEÇ
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != "Id" && !p.PropertyType.IsGenericType && !p.GetGetMethod().IsVirtual) // Navigasyon özelliklerini filtrele
                .ToList();

            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", properties.Select(p => "@" + p.Name));

            var sql = $"INSERT INTO {_tableName} (Id, {columns}) VALUES (@Id, {values})";

            await connection.ExecuteAsync(sql, entity);
            return entity;
        }

        public virtual async Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> entities)
        {
            using var connection = _context.CreateConnection();
            var now = DateTime.UtcNow;

            foreach (var entity in entities)
            {
                entity.Id = Guid.NewGuid();
                entity.CreatedDate = now;
                entity.UpdatedDate = now;
            }

            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != "Id" && !p.PropertyType.IsGenericType && !p.GetGetMethod().IsVirtual)
                .ToList();

            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", properties.Select(p => "@" + p.Name));

            var sql = $"INSERT INTO {_tableName} (Id, {columns}) VALUES (@Id, {values})";

            await connection.ExecuteAsync(sql, entities); // Dapper, IEnumerable<T> ile toplu eklemeyi destekler
            return entities;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            using var connection = _context.CreateConnection();
            entity.UpdatedDate = DateTime.UtcNow;

            // Daha güvenli property filtreleme
            var properties = typeof(T).GetProperties()
                .Where(p => p.Name != "Id" &&
                           p.Name != "CreatedDate" &&
                           !p.PropertyType.IsGenericType &&
                           !p.GetGetMethod().IsVirtual &&
                           p.CanWrite &&
                           p.GetSetMethod() != null &&
                           !p.PropertyType.IsClass || p.PropertyType == typeof(string)) // Sadece primitive types ve string
                .ToList();

            var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
            var sql = $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id";

            Console.WriteLine($"Update SQL: {sql}"); // Debug için
            Console.WriteLine($"Entity type: {typeof(T).Name}"); // Debug için

            try
            {
                await connection.ExecuteAsync(sql, entity);
                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update error: {ex.Message}");
                Console.WriteLine($"SQL: {sql}");
                throw;
            }
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            using var connection = _context.CreateConnection();
            var sql = $"DELETE FROM {_tableName} WHERE Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
}

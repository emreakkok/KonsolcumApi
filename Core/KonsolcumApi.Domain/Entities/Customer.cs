using KonsolcumApi.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KonsolcumApi.Domain.Entities
{
    public class Customer : BaseEntity
    {
        // Kullanıcı bilgilerini tutan entity
        public string Email { get; set; } = string.Empty;        // Kullanıcı email adresi
        public string PasswordHash { get; set; } = string.Empty; // Şifrelenmiş parola
        public string FirstName { get; set; } = string.Empty;    // Ad
        public string LastName { get; set; } = string.Empty;     // Soyad
        public string? PhoneNumber { get; set; }                 // Telefon numarası (opsiyonel)
        public bool IsActive { get; set; } = true;               // Hesap aktif mi?

    }
}

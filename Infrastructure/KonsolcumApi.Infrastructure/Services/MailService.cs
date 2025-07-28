using KonsolcumApi.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Infrastructure.Services
{
    public class MailService : IMailService
    {
        readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendMailAsync(new[] { to }, subject, body, isBodyHtml);
        }

        public async Task SendMailAsync(string[] tos, string subject, string body, bool isBodyHtml = true)
        {
            try
            {
                MailMessage mail = new();
                mail.IsBodyHtml = isBodyHtml;
                foreach (var to in tos)
                    mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                mail.From = new(_configuration["Mail:Username"], "Konsolcum", Encoding.UTF8);

                SmtpClient smtp = new();
                smtp.Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]);
                smtp.Port = int.Parse(_configuration["Mail:Port"]);
                smtp.EnableSsl = true;
                smtp.Host = _configuration["Mail:Host"];

                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mail gönderim hatası: " + ex.Message); // logla veya tekrar fırlat
                throw;
            }
        }

        public async Task SendPasswordResetMailAsync(string to, string userId, string resetToken)
        {
            StringBuilder mail = new();
            mail.AppendLine("Merhaba<br>Eğer yeni şifre talebinde bulunduysanız aşağıdaki linkten şifrenizi yenileyebilirsiniz.<br>");

            // Link'i daha basit bir şekilde oluştur
            string resetUrl = $"{_configuration["AngularClientUrl"]}/update-password/{userId}/{resetToken}";

            mail.AppendLine($"<p><strong>Şifre sıfırlama linki:</strong></p>");
            mail.AppendLine($"<p><a href=\"{resetUrl}\" target=\"_blank\" style=\"display: inline-block; padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px;\">Şifrenizi Sıfırlayın</a></p>");

            // Alternatif olarak link'i metin halinde de ver
            mail.AppendLine($"<p>Eğer yukarıdaki buton çalışmıyorsa, bu linki kopyalayıp tarayıcınıza yapıştırın:</p>");
            mail.AppendLine($"<p style=\"word-break: break-all; background-color: #f8f9fa; padding: 10px; border: 1px solid #dee2e6;\">{resetUrl}</p>");

            mail.AppendLine("<br><span style=\"font-size:12px;\">NOT : Eğer ki bu talep tarafınızca gerçekleştirilmemişse lütfen bu maili ciddiye almayınız.</span><br>Saygılarımızla...<br><br><br>Konsolcum");

            await SendMailAsync(to, "Şifre Yenileme Talebi", mail.ToString());
        }

        public async Task SendCompletedOrderMailAsync(string to, string orderCode, DateTime orderDate, string userName)
        {
            string mail = $"Sayın {userName} Merhaba<br>" +
                $"{orderDate} tarihinde vermiş olduğunuz {orderCode} kodlu siparişiniz tamamlanmış ve kargo firmasına verilmiştir.";

            await SendMailAsync(to, $"{orderCode} Sipariş Numaralı Siparişiniz Tamamlandı", mail);

        }

    }
}

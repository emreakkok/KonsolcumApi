using FluentValidation;
using KonsolcumApi.Application.Features.Commands.Product.CreateProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Validators.Products
{
    public class CreateProductValidator : AbstractValidator<CreateProductCommandRequest>
    {
        public CreateProductValidator()
        {
            RuleFor(p => p.name)
                .NotEmpty().WithMessage("Ürün adı boş olamaz.")
                .MinimumLength(2).WithMessage("Ürün adı en az 2 karakter olmalıdır.")
                .MaximumLength(100).WithMessage("Ürün adı en fazla 100 karakter olmalıdır.");

            RuleFor(p => p.description)
                .NotEmpty().WithMessage("Ürün açıklaması boş olamaz.")
                .MaximumLength(800).WithMessage("Ürün açıklaması en fazla 800 karakter olabilir.");

            RuleFor(p => p.price)
                .GreaterThan(0).WithMessage("Ürün fiyatı 0'dan büyük olmalıdır.");

            RuleFor(p => p.stockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stok miktarı negatif olamaz.");

            RuleFor(p => p.categoryId)
                .NotEmpty().WithMessage("Kategori seçimi zorunludur.");
        }
    }
}

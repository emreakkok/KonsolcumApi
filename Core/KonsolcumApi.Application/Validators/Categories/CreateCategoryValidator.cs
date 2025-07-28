using FluentValidation;
using KonsolcumApi.Application.Features.Commands.Category.CreateCategory;
using KonsolcumApi.Application.ViewModels.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Validators.Categories
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommandRequest>
    {
        public CreateCategoryValidator()
        {
            RuleFor(c => c.name).NotEmpty().WithMessage("Kategori adı boş geçilemez")
                .MaximumLength(150).MinimumLength(2).WithMessage("Kategori adı 2-150 karakter arasında olmalıdır");
            RuleFor(c => c.description).NotEmpty().WithMessage("Kategori açıklaması boş geçilemez")
                .MaximumLength(150).MinimumLength(1).WithMessage("Kategori açıklaması 1-150 karakter arasında olmalıdır");
        }
    }
}

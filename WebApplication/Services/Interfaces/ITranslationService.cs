using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models.DTOs;
using WebApplication.Models.Entities;

namespace WebApplication.Services
{
    public interface ITranslationService
    {
        Task<IEnumerable<TranslationDto>> GetAllTranslationsAsync();
        Task AddTranslationFromExternalApiAsync(TranslationInputDto translationInputDto, string userId);
    }
}

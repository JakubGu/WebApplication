using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WebApplication.Models;
using WebApplication.Models.Entities;
using System.Data.Entity;
using System.Linq;
using WebApplication.Models.DTOs;
using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace WebApplication.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;

        public TranslationService(AppDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<TranslationDto>> GetAllTranslationsAsync()
        {
            return await _context.Database
                                 .SqlQuery<TranslationDto>("EXEC GetTranslations")
                                 .ToListAsync();
        }

        // Dodawanie nowego tłumaczenia z zewnętrznego API
        public async Task AddTranslationFromExternalApiAsync(TranslationInputDto translationInputDto, string userId)
        {
            var translatedText = await TranslateTextAsync(translationInputDto.SourceText, translationInputDto.TranslationLanguage);
            //var translatedText = "Test";

            var query = @"
                EXEC [dbo].[InsertTranslation]
                    @SourceText = @SourceText,
                    @TranslatedText = @TranslatedText,
                    @TranslationLanguage = @TranslationLanguage,
                    @UserId = @UserId";

            try
            {
                await _context.Database.ExecuteSqlCommandAsync(query,
                    new SqlParameter("@SourceText", translationInputDto.SourceText),
                    new SqlParameter("@TranslatedText", translatedText),
                    new SqlParameter("@TranslationLanguage", translationInputDto.TranslationLanguage),
                    new SqlParameter("@UserId", userId));
            }
            catch (Exception ex)
            {
                // Logowanie błędu
                throw new Exception("Error executing stored procedure: " + ex.Message, ex);
            }
        }

        // Metoda wysyłająca zapytanie do zewnętrznego API w celu tłumaczenia na dany język
        private async Task<string> TranslateTextAsync(string text, string language)
        {
            var url = $"https://api.funtranslations.com/translate/{language}.json";
            var parameters = new { text = text };
            var content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");

            // Wysyłanie zapytania POST do API
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                // Parsowanie odpowiedzi JSON
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
                return apiResponse?.Contents?.Translated ?? "Translation failed";
            }

            // Jeśli wystąpił błąd, zwróć odpowiedni komunikat
            return "Error while translating";
        }

        // Definicja odpowiedzi z API
        public class ApiResponse
        {
            public Contents Contents { get; set; }
        }

        public class Contents
        {
            public string Translated { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using WebApplication.Models.Entities;

namespace WebApplication.Models
{
    public class Seed
    {
        // Seedowanie danych
        public static async Task SeedData(AppDbContext context, UserManager<User> userManager)
        {
            // Sprawdzenie, czy użytkownicy już istnieją
            if (userManager.Users.Any())
                return; // Jeśli użytkownicy już istnieją, nie dodawaj nowych.

            // Tworzenie przykładowych użytkowników
            var users = new List<User>
            {
                new User
                {
                    UserName = "admin@domain.com",
                    Email = "admin@domain.com"
                },
                new User
                {
                    UserName = "user1@domain.com",
                    Email = "user1@domain.com"
                },
                new User
                {
                    UserName = "user2@domain.com",
                    Email = "user2@domain.com"
                },
                new User
                {
                    UserName = "user3@domain.com",
                    Email = "user3@domain.com"
                }
            };

            foreach (var user in users)
            {
                var result = await userManager.CreateAsync(user, "Password123!");

                if (result.Succeeded)
                {
                    // Tworzenie przykładowych danych w tabeli Translations przez wywołanie procedury InsertTranslation

                    // Tłumaczenia dla usera 1 (admin)
                    await CreateTranslation(context, user.Id, "Hello", "Hola", "Spanish");
                    await CreateTranslation(context, user.Id, "Goodbye", "Adiós", "Spanish");
                    await CreateTranslation(context, user.Id, "Thank you", "Gracias", "Spanish");

                    // Tłumaczenia dla usera 2 (user1)
                    await CreateTranslation(context, user.Id, "Hello", "Bonjour", "French");
                    await CreateTranslation(context, user.Id, "Goodbye", "Au revoir", "French");
                    await CreateTranslation(context, user.Id, "Thank you", "Merci", "French");

                    // Tłumaczenia dla usera 3 (user2)
                    await CreateTranslation(context, user.Id, "Hello", "Ciao", "Italian");
                    await CreateTranslation(context, user.Id, "Goodbye", "Arrivederci", "Italian");
                    await CreateTranslation(context, user.Id, "Thank you", "Grazie", "Italian");

                    // Tłumaczenia dla usera 4 (user3)
                    await CreateTranslation(context, user.Id, "Hello", "Hallo", "German");
                    await CreateTranslation(context, user.Id, "Goodbye", "Auf Wiedersehen", "German");
                    await CreateTranslation(context, user.Id, "Thank you", "Danke", "German");
                }
            }
        }
        // Pomocnicza metoda do dodawania tłumaczeń
        private static async Task CreateTranslation(AppDbContext context, string userId, string sourceText, string translatedText, string language)
        {
            // Task.Run uruchamia synchroniczną metodę w osobnym wątku
            await Task.Run(() =>
            {
                context.Database.ExecuteSqlCommand(@"
                    EXEC [dbo].[InsertTranslation]
                        @SourceText = N'" + sourceText + @"',
                        @TranslatedText = N'" + translatedText + @"',
                        @TranslationLanguage = N'" + language + @"',
                        @UserId = N'" + userId + @"'
                ");
            });
        }
    }
}
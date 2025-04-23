using System;
using System.Web;
using System.Web.Services.Description;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using WebApplication.Models;
using WebApplication.Models.Entities;
using WebApplication.Services;

[assembly: OwinStartup(typeof(WebApplication.Startup))]

namespace WebApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Ustawienie migracji i stworzenie bazy danych oraz procedur
            EnsureDatabaseAndProceduresCreated();

            // Konfiguracja autentykacji za pomocą ciasteczek
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"), // Ścieżka logowania
                LogoutPath = new PathString("/Account/Logout"), // Ścieżka wylogowania
                ExpireTimeSpan = TimeSpan.FromDays(7),
                SlidingExpiration = true
            });

            // Konfiguracja Identity
            app.CreatePerOwinContext(AppDbContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);

            UnityConfig.RegisterComponents();
        }

        private void EnsureDatabaseAndProceduresCreated()
        {
            using (var context = new AppDbContext())
            {
                context.Database.Initialize(force: false);

                context.Database.ExecuteSqlCommand(@"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetTranslations]') AND type IN (N'P', N'PC'))
                    BEGIN
                        EXEC('
                            CREATE PROCEDURE [dbo].[GetTranslations]
                            AS
                            BEGIN
                                SELECT 
                                    t.Id,
                                    t.SourceText,
                                    t.TranslatedText,
                                    t.TranslationLanguage,
                                    t.CreatedAt,
                                    u.UserName
                                FROM Translations t
                                INNER JOIN AspNetUsers u ON u.Id = t.UserId
                            END
                        ')
                    END
                ");

                context.Database.ExecuteSqlCommand(@"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertTranslation]') AND type IN (N'P', N'PC'))
                    BEGIN
                        EXEC('
                            CREATE PROCEDURE [dbo].[InsertTranslation]
                                @SourceText NVARCHAR(MAX),
                                @TranslatedText NVARCHAR(MAX),
                                @TranslationLanguage NVARCHAR(50),
                                @UserId NVARCHAR(128)
                            AS
                            BEGIN
                                INSERT INTO Translations (SourceText, TranslatedText, TranslationLanguage, CreatedAt, UserId)
                                VALUES (@SourceText, @TranslatedText, @TranslationLanguage, GETUTCDATE(), @UserId)
                            END
                        ')
                    END
                ");
            }
        }
    }

}
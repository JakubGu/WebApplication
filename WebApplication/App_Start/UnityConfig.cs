using System.Data.Entity.Core.Metadata.Edm;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;
using WebApplication.Services; // Namespace z ITranslationService

public static class UnityConfig
{
    public static void RegisterComponents()
    {
        var container = new UnityContainer();

        // Rejestracja zależności
        container.RegisterType<ITranslationService, TranslationService>();

        // Ustawienie resolvera w MVC
        DependencyResolver.SetResolver(new UnityDependencyResolver(container));
    }
}

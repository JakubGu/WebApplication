using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebApplication.Models.DTOs;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class TranslationsController : Controller
    {
        private readonly ITranslationService _translationService;

        public TranslationsController(ITranslationService translationService)
        {
            _translationService = translationService;
        }

        [Authorize]
        public ActionResult Index()
        {
            return View(); // Tylko ładuje stronę i widok
        }

        [Authorize]
        [HttpGet]
        public async Task<JsonResult> GetTranslations()
        {
            var translations = await _translationService.GetAllTranslationsAsync();
            return Json(new { data = translations }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Translations/AddFromExternalApi
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddFromExternalApi(TranslationInputDto translationInputDto)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();

                await _translationService.AddTranslationFromExternalApiAsync(translationInputDto, userId);

                return RedirectToAction("Index");
            }

            return View(translationInputDto);
        }
    }
}
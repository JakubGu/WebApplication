using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.DTOs
{
    public class TranslationInputDto
    {
        public string SourceText { get; set; }
        public string TranslationLanguage { get; set; }

        // Te pola będą dodane przez backend, jeśli dane przyjdą z zewnętrznego API
        public string TranslatedText { get; set; }
        public string UserId { get; set; }
    }
}
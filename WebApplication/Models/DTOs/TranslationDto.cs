using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.DTOs
{
    public class TranslationDto
    {
        public int Id { get; set; }
        public string SourceText { get; set; }
        public string TranslatedText { get; set; }
        public string TranslationLanguage { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
    }
}
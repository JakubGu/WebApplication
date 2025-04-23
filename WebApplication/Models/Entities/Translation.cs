using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models.Entities
{
    [Table("Translations")]
    public class Translation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SourceText { get; set; }

        [Required]
        public string TranslatedText { get; set; }

        [Required]
        public string TranslationLanguage { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual User User { get; set; }
    }
}
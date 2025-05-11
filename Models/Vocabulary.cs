namespace VocabularyApp.Models
{
    public class Vocabulary
    {
        public int Id { get; set; }
        public string Kana { get; set; } = null!;
        public string? Kanji { get; set; }
        public string Translation { get; set; } = null!;
        public int Importance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public VocabularyTestResult? TestResult { get; set; }
    }
}

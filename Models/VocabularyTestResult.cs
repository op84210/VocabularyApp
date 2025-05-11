namespace VocabularyApp.Models
{
    public class VocabularyTestResult
    {
        public int Id { get; set; }
        public int VocabularyId { get; set; }
        public DateTime? LastTestedAt { get; set; }
        public int CorrectCount { get; set; }
        public int WrongCount { get; set; }

        public Vocabulary? Vocabulary { get; set; } 
    }
}

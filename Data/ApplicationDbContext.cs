using Microsoft.EntityFrameworkCore;
using VocabularyApp.Models;

namespace VocabularyApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vocabulary> Vocabularies { get; set; }
        public DbSet<VocabularyTestResult> VocabularyTestResults { get; set; }
    }
}

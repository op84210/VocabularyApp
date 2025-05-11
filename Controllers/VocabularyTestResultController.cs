using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VocabularyApp.Data;
using VocabularyApp.Models;

namespace VocabularyApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VocabularyTestResultController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VocabularyTestResultController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("load")]
        public async Task<ActionResult<IEnumerable<VocabularyTestResultsDto>>> LoadTestResults()
        {
            return await _context.VocabularyTestResults
                .Select(v => new VocabularyTestResultsDto
                {
                    Id = v.Id,
                    CorrectCount = v.CorrectCount,
                    WrongCount = v.WrongCount
                })
                .ToListAsync();
        }

        public class VocabularyTestResultsDto
        {
            public int Id { get; set; }
            public int CorrectCount { get; set; }
            public int WrongCount { get; set; }
        }

        [HttpGet("load/{vocabularyId}")]
        public async Task<ActionResult<VocabularyTestResultsDto>> LoadTestResult(int vocabularyId)
        {
            var vocabulary = await _context.Vocabularies
                .Include(v => v.TestResult)
                .FirstOrDefaultAsync(v => v.Id == vocabularyId);

            if (vocabulary == null)
            {
                return NotFound(new { message = $"找不到 Id {vocabularyId} 的單字。" });
            }

            var testResult = vocabulary.TestResult;
            
            if (testResult == null)
            {
                return NotFound();
            }

            return Ok(new VocabularyTestResultsDto
            {
                Id = testResult.Id,
                CorrectCount = testResult.CorrectCount,
                WrongCount = testResult.WrongCount
            });
        }

        [HttpPost("update")]
        public async Task<ActionResult<VocabularyTestResult>> UpdateTestResult(VocabularyTestResultsDto testResultDto)
        {
            try
            {
                var testResult = await _context.VocabularyTestResults.FindAsync(testResultDto.Id);
                if (testResult == null)
                {
                    return NotFound();
                }

                testResult.LastTestedAt = DateTime.UtcNow;
                testResult.CorrectCount = testResultDto.CorrectCount;
                testResult.WrongCount = testResultDto.WrongCount;

                _context.Entry(testResult).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return Ok(testResult);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "更新時發生錯誤", error = ex.Message });
            }
        }
    
    }
}

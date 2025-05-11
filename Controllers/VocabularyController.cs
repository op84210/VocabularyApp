using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VocabularyApp.Data;
using VocabularyApp.Models;

namespace VocabularyApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class VocabularyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VocabularyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("load")]
        public async Task<ActionResult<IEnumerable<VocabularyDto>>> LoadVocabularies()
        {
            return await _context.Vocabularies
                .Select(v => new VocabularyDto
                {
                    Id = v.Id,
                    Kana = v.Kana,
                    Kanji = v.Kanji,
                    Translation = v.Translation,
                    Importance = v.Importance
                })
                .ToListAsync();
        }

        public class VocabularyDto
        {
            public int Id { get; set; }
            public string Kana { get; set; } = null!;
            public string? Kanji { get; set; }
            public string Translation { get; set; } = null!;
            public int Importance { get; set; }
        }

        [HttpGet("load/{id}")]
        public async Task<ActionResult<VocabularyDto>> LoadVocabulary(int id)
        {
            var vocabulary = await _context.Vocabularies.FindAsync(id);

            if (vocabulary == null)
            {
                return NotFound();
            }

            return Ok(new VocabularyDto
            {
                Id = vocabulary.Id,
                Kana = vocabulary.Kana,
                Kanji = vocabulary.Kanji,
                Translation = vocabulary.Translation,
                Importance = vocabulary.Importance
            });
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateVocabulary(CreateVocabularyDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var now = DateTime.UtcNow;

                var vocabulary = new Vocabulary
                {
                    Kana = dto.Kana,
                    Kanji = dto.Kanji,
                    Translation = dto.Translation,
                    Importance = dto.Importance,
                    CreatedAt = now
                };

                _context.Vocabularies.Add(vocabulary);
                await _context.SaveChangesAsync();

                var testResult = new VocabularyTestResult
                {
                    VocabularyId = vocabulary.Id,
                    LastTestedAt = now,
                    WrongCount = 0,
                    CorrectCount = 0
                };

                _context.VocabularyTestResults.Add(testResult);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // 提交事務
                return Ok(new
                {
                    success = true,
                    message = "單字新增成",
                    data = new VocabularyDto
                    {
                        Id = vocabulary.Id,
                        Kana = vocabulary.Kana,
                        Kanji = vocabulary.Kanji,
                        Translation = vocabulary.Translation,
                        Importance = vocabulary.Importance
                    }
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    success = false,
                    message = "伺服器錯誤，請稍後再試",
                    errorCode = 5000,
                    detail = ex.Message
                });
            }
        }
        public class CreateVocabularyDto
        {
            public string Kana { get; set; } = null!;
            public string? Kanji { get; set; }
            public string Translation { get; set; } = null!;
            public int Importance { get; set; }
        }

        [HttpPost("update")]
        public async Task<ActionResult<VocabularyDto>> UpdateVocabulary(VocabularyDto dto)
        {
            try
            {
                var vocabulary = await _context.Vocabularies.FindAsync(dto.Id);
                if (vocabulary == null)
                {
                    return NotFound(new { message = $"找不到 Id {dto.Id} 的單字。" });
                }

                // 更新欄位
                vocabulary.Kana = dto.Kana;
                vocabulary.Kanji = dto.Kanji;
                vocabulary.Translation = dto.Translation;
                vocabulary.Importance = dto.Importance;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = $"已更新 Id {dto.Id} 的單字。",
                    vocabulary = new
                    {
                        vocabulary.Id,
                        vocabulary.Kana,
                        vocabulary.Kanji,
                        vocabulary.Translation,
                        vocabulary.Importance,
                        vocabulary.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "更新時發生錯誤", error = ex.Message });
            }
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteVocabulary(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var vocabulary = await _context.Vocabularies.FindAsync(id);
                if (vocabulary == null)
                {
                    return NotFound(new { message = $"找不到 Id {id} 的單字。" });
                }

                // 先刪除關聯的 VocabularyTestResult
                var testResult = await _context.VocabularyTestResults
                    .FirstOrDefaultAsync(t => t.VocabularyId == id);

                if (testResult != null)
                {
                    _context.VocabularyTestResults.Remove(testResult);
                }

                _context.Vocabularies.Remove(vocabulary);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { message = $"已刪除 Id {id} 的單字。" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "刪除時發生錯誤", error = ex.Message });
            }
        }

    }
}

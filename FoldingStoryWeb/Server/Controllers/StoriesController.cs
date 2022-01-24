#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoldingStoryWeb.Server.DAL;
using FoldingStoryWeb.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoldingStoryWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        private readonly MainDbContext _context;

        public StoriesController(MainDbContext context)
        {
            _context = context;
        }

        // GET: api/Stories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoryDto>>> GetStories()
        {
            return await _context.Stories.Select(t=> new StoryDto()
            {
                Id = t.Id,
                CharacterLimit = t.CharacterLimit,
                SequenceLimit = t.SequenceLimit,
                SnippetCount = t.Snippets.Count(),
                TimeLimit = t.TimeLimit,
                Title = t.Title,
                Type = t.Type
            }).ToListAsync();
        }

        // GET: api/Stories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StoryDto>> GetStory(int id)
        {
            try
            {
                var story = await _context.Stories.FindAsync(id);

                if (story == null)
                {
                    return NotFound();
                }

                return story.ToDto();
            } catch(Exception ex)
            {
                var a = 3;
            }
            return new StoryDto();
        }

        // PUT: api/Stories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStory(int id, StoryDto story)
        {
            if (id != story.Id)
            {
                return BadRequest();
            }

            var dbStory = await _context.Stories.FirstOrDefaultAsync(t => t.Id == id);
            //_context.Entry(story).State = EntityState.Modified;
            dbStory.FromDto(story);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Stories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Story>> PostStory(StoryDto story)
        {
            var dbStory = new Story();
            dbStory.FromDto(story);
            dbStory.CreatedAt = DateTime.UtcNow;
            _context.Stories.Add(dbStory);
            var result = await _context.SaveChangesAsync();

            return CreatedAtAction("GetStory", new { id = dbStory.Id }, dbStory.ToDto());
        }

        // DELETE: api/Stories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStory(int id)
        {
            var story = await _context.Stories.FindAsync(id);
            if (story == null)
            {
                return NotFound();
            }

            _context.Stories.Remove(story);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StoryExists(int id)
        {
            return _context.Stories.Any(e => e.Id == id);
        }
    }
}

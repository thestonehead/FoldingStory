#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoldingStoryWeb.Server.DAL;
using FoldingStoryWeb.Server.Infrastructure;
using FoldingStoryWeb.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoldingStoryWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        private readonly MainDbContext context;

        public StoriesController(MainDbContext context)
        {
            this.context = context;
        }

        // GET: api/Stories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoryDto>>> GetStories(StoryFilter storyFilter = StoryFilter.Public)
        {
            var userId = this.GetUserId();

            var query = context.Stories.AsQueryable();

            switch (storyFilter)
            {
                case StoryFilter.All:
                    query = query.Where(t => t.Type == StoryType.Public || t.Type == StoryType.PublicReadOnly || (t.Type == StoryType.Private && userId != null && t.Snippets.Any(s => s.UserId == userId)));
                    break;
                case StoryFilter.Public:
                    query = query.Where(t => t.Type == StoryType.Public || t.Type == StoryType.PublicReadOnly);
                    break;
                case StoryFilter.MyStories:
                    if (String.IsNullOrEmpty(userId))
                    {
                        return Unauthorized();
                    }
                    query = query.Where(t => t.CreatedBy == userId);
                    break;
                case StoryFilter.RecentlyContributed:
                    query = query.Where(t => t.Snippets.Any(s => s.CreatedAt > DateTime.UtcNow.AddMonths(-1) && s.UserId == userId));
                    break;
                default:
                    return BadRequest();
            }

            query = query.OrderByDescending(t => t.Snippets.OrderBy(s=>s.Id).Last().CreatedAt);

            return await query.Select(t => new StoryDto()
            {
                Id = t.Id,
                CharacterLimit = t.CharacterLimit,
                SequenceLimit = t.SequenceLimit,
                SnippetCount = t.Snippets.Count(),
                TimeLimit = t.TimeLimit,
                Title = t.Title,
                Type = t.Type,
                CreatedBy = t.CreatedBy
            }).ToListAsync();
        }

        // GET: api/Stories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StoryDto>> GetStory(int id)
        {
            try
            {
                var story = await context.Stories.FindAsync(id);

                //var userId = this.GetUserId();
                //if (story.Type == StoryType.Private && (story.CreatedBy != userId ||  ))

                if (story == null)
                {
                    return NotFound();
                }

                return story.ToDto();
            }
                catch (Exception ex)
            {
                var a = 3;
            }
            return new StoryDto();
        }

        // PUT: api/Stories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutStory(int id, StoryDto story)
        {
            if (id != story.Id)
            {
                return BadRequest();
            }

            var dbStory = await context.Stories.FirstOrDefaultAsync(t => t.Id == id);
            //_context.Entry(story).State = EntityState.Modified;
            dbStory.FromDto(story);

            try
            {
                await context.SaveChangesAsync();
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
        [Authorize]
        public async Task<ActionResult<StoryDto>> PostStory(StoryDto story)
        {
            var dbStory = new Story();
            dbStory.FromDto(story);
            dbStory.CreatedAt = DateTime.UtcNow;
            dbStory.CreatedBy = this.GetUserId();
            context.Stories.Add(dbStory);
            var result = await context.SaveChangesAsync();

            return dbStory.ToDto();
            //return CreatedAtAction("GetStory", new { id = dbStory.Id }, dbStory.ToDto());
        }

        // DELETE: api/Stories/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteStory(int id)
        {
            var story = await context.Stories.FindAsync(id);
            if (story == null)
            {
                return NotFound();
            }

            context.Stories.Remove(story);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool StoryExists(int id)
        {
            return context.Stories.Any(e => e.Id == id);
        }
    }
}

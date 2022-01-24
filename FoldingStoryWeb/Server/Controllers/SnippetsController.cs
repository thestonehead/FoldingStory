#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoldingStoryWeb.Server.DAL;
using FoldingStoryWeb.Shared;

namespace FoldingStoryWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SnippetsController : ControllerBase
    {
        private readonly MainDbContext _context;

        public SnippetsController(MainDbContext context)
        {
            _context = context;
        }

        // GET: api/Snippets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SnippetDto>>> GetSnippets()
        {
            return await _context.Snippets.Select(t=>t.ToDto()).ToListAsync();
        }

        [HttpGet("byStory/{id}")]
        public async Task<ActionResult<IEnumerable<SnippetDto>>> GetSnippetsByStory(int id, [FromQuery]GetSnippetsByStoryRequest request)
        {
            try
            {
                IQueryable<Snippet> snippetsQuery = _context.Snippets.Where(s => s.StoryId == id);
                if (request.Since.HasValue)
                    snippetsQuery = snippetsQuery.Where(t => t.CreatedAt >= request.Since.Value);
                if (request.Head.HasValue)
                {
                    snippetsQuery = snippetsQuery.OrderBy(s => s.Id);
                    if (request.Skip.HasValue)
                        snippetsQuery = snippetsQuery.Skip(request.Skip.Value);
                    snippetsQuery = snippetsQuery.Take(request.Head.Value);
                }
                else if (request.Tail.HasValue)
                {
                    snippetsQuery = snippetsQuery.OrderByDescending(t => t.Id);
                    if (request.Skip.HasValue)
                        snippetsQuery = snippetsQuery.Skip(request.Skip.Value);
                    snippetsQuery = snippetsQuery.Take(request.Tail.Value);
                }
                var snippets = await snippetsQuery.ToListAsync();

                return snippets.Select(t => t.ToDto()).ToList();
            } catch( Exception ex)
            {
                throw;
            }
        }

        // GET: api/Snippets/5
        [HttpGet("byId/{id}")]
        public async Task<ActionResult<SnippetDto>> GetSnippet(int id)
        {
            var snippet = await _context.Snippets.FindAsync(id);

            if (snippet == null)
            {
                return NotFound();
            }

            return snippet.ToDto();
        }

        // PUT: api/Snippets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("byId/{id}")]
        public async Task<IActionResult> PutSnippet(int id, SnippetDto snippet)
        {
            if (id != snippet.Id)
            {
                return BadRequest();
            }

            var dbSnippet = await _context.Snippets.FindAsync(id);
            dbSnippet.FromDto(snippet);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SnippetExists(id))
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

        // POST: api/Snippets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Snippet>> PostSnippet(SnippetDto snippet, [FromQuery]int lastId)
        {
            var username = User.Claims.FirstOrDefault(t => t.Type == System.Security.Claims.ClaimTypes.Name)?.Value;
            var userId = User.Claims.FirstOrDefault(t => t.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (username == null || snippet.Username != username)
                return BadRequest("Username is not valid.");
            if (userId == null || snippet.UserId != userId)
                return BadRequest("User is id not valid.");

            var story = await _context.Stories.FindAsync(snippet.StoryId);

            if (story == null)
                return NotFound("Story doesn't exist.");
            if (snippet.Text.Length > story.CharacterLimit)
                return BadRequest("Snippet exceeds story character limit.");
            if (story.SequenceLimit.HasValue && story.Snippets.OrderByDescending(t => t.Id).Take(story.SequenceLimit.Value).Any(t => t.UserId == snippet.UserId))
                return BadRequest("Snippet posted by the same user too soon after previous snippet from the same user.");

            var maxLastId = _context.Snippets.Where(t => t.StoryId == story.Id).Max(t => t.Id);

            if (maxLastId != lastId)
                return Conflict("Someone posted a snippet to the story before you.");

            var dbSnippet = new Snippet();
            dbSnippet.FromDto(snippet);
            _context.Snippets.Add(dbSnippet);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSnippet", new { id = snippet.Id }, snippet);
        }

        // DELETE: api/Snippets/5
        [HttpDelete("byId/{id}")]
        public async Task<IActionResult> DeleteSnippet(int id)
        {
            var snippet = await _context.Snippets.FindAsync(id);
            if (snippet == null)
            {
                return NotFound();
            }

            _context.Snippets.Remove(snippet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SnippetExists(int id)
        {
            return _context.Snippets.Any(e => e.Id == id);
        }
    }
}

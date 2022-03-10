using FoldingStoryWeb.Server.DAL;
using FoldingStoryWeb.Server.Infrastructure;
using FoldingStoryWeb.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoldingStoryWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class LinksController : ControllerBase
    {

        private readonly MainDbContext context;

        public LinksController(MainDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{id}", Name ="GetLink")]
        public async Task<ActionResult> GetLink(string id)
        {
            try
            {
                var link = await context.Links.FindAsync(id);

                if (link == null)
                {
                    return NotFound();
                }

                var storyCode = CodeHelper.IntToString(link.StoryId);

                return Redirect($"/story/{storyCode}");
            }
            catch (Exception ex)
            {
                var a = 3;
            }
            return Ok();
        }

        [HttpPost()]
        public async Task<IActionResult> CreateLink(int storyId, int? snippetId)
        {
            var story = await context.Stories.FindAsync(storyId);
            if (story == null)
            {
                return NotFound();
            }

            var generatedLink = new GeneratedLink()
            {
                Id = Guid.NewGuid().ToString(),
                GeneratedBy = this.GetUserId(),
                GeneratedOn = DateTime.UtcNow,
                StoryId = storyId,
                SnippetId = snippetId,
                ValidTo = DateTime.UtcNow.AddMonths(1)
            };
            context.Links.Add(generatedLink);
            var result = await context.SaveChangesAsync() == 1;

            return CreatedAtAction("GetLink", nameof(LinksController), generatedLink.Id);
        }

    }
}

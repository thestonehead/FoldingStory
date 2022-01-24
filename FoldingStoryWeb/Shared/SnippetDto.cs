using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoldingStoryWeb.Shared
{
    public class SnippetDto
    {
        public int? Id { get; set; }
        public int? StoryId { get; set; }

        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }

    }
}

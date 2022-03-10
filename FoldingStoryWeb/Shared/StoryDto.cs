using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoldingStoryWeb.Shared
{
    public class StoryDto

    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public int? CharacterLimit { get; set; }
        public TimeSpan? TimeLimit { get; set; }
        public int? SequenceLimit { get; set; }
        public StoryType Type { get; set; }
        public int SnippetCount { get; set; }
        public string CreatedBy { get; set; }
    }

    public enum StoryType
    {
        Public,
        Private,
        PublicReadOnly
    }
}

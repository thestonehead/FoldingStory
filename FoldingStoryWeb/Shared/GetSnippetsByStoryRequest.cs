using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoldingStoryWeb.Shared
{
    public class GetSnippetsByStoryRequest
    {
        public int? Tail { get; set; }
        public int? Head { get; set; }
        public DateTime? Since { get; set; }
        public int? Skip { get; set; }
    }
}

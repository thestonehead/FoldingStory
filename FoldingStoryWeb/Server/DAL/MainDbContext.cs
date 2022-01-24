using FoldingStoryWeb.Shared;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoldingStoryWeb.Server.DAL
{
    public class MainDbContext : DbContext
    {

        public DbSet<Story> Stories { get; set; }
        public DbSet<Snippet> Snippets { get; set; }

        public MainDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Snippet>(build =>
            {
                build.HasKey(nameof(Snippet.Id), nameof(Snippet.StoryId));
                build.ToTable("Snippet");
            });
            modelBuilder.Entity<Story>(build =>
            {
                build.ToTable("Story");
                build.Property(t => t.Type).HasConversion<int>();
            });
        }

        
    }


    public class Story
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int? CharacterLimit { get; set; }
        public TimeSpan? TimeLimit { get; set; }
        public int? SequenceLimit { get; set; }
        public DateTime CreatedAt { get; set; }
        public StoryType Type { get; set; }

        public virtual ICollection<Snippet> Snippets { get; set; }

        public Story()
        {
            Snippets = new HashSet<Snippet>();
        }

        public StoryDto ToDto()
        {
            var dto = new StoryDto();
            dto.Id = Id;
            dto.Title = Title;
            dto.TimeLimit = TimeLimit;
            dto.SequenceLimit = SequenceLimit;
            dto.CharacterLimit = CharacterLimit;
            dto.SnippetCount = this.Snippets.Count();
            return dto;
        }

        public Story FromDto(StoryDto dto)
        {
            this.Title = dto.Title;
            this.CharacterLimit = dto.CharacterLimit;
            this.TimeLimit = dto.TimeLimit;
            this.SequenceLimit = dto.SequenceLimit;
            this.Type = dto.Type;
            return this;
        }
    }

    public class Snippet
    {
        public int Id { get; set; }
        [ForeignKey("Story")]
        public int StoryId { get; set; }

        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Username { get; set; }
        public string UserId { get; set; }

        public Story Story { get; set; }

        public SnippetDto ToDto()
        {
            var dto = new SnippetDto();
            dto.Id = Id;
            dto.Text = Text;
            dto.CreatedAt = CreatedAt;
            dto.StoryId = StoryId;
            dto.Username = Username;
            dto.UserId = UserId;
            return dto;
        }

        public Snippet FromDto(SnippetDto dto)
        {
            if (dto.StoryId.HasValue)
                this.StoryId = dto.StoryId.Value;
            this.Text = dto.Text;
            this.CreatedAt = dto.CreatedAt;
            this.UserId = dto.UserId;
            this.Username = dto.Username;
            return this;
        }
    }
}

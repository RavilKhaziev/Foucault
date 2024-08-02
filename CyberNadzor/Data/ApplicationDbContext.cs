using CyberNadzor.Entities.Statistic;
using CyberNadzor.Entities.Survey;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CyberNadzor.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Survey> Surveys { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<TopicChoise> TopicChoises {get; set;}
    public DbSet<TopicText> TopicTexts { get; set; }
    public DbSet<SurveyAnswers> SurveyAnswers { get; set; }
    public DbSet<TopicAnswer> TopicAnswers { get; set; }
    public DbSet<TopicTextAsnwer> TopicTextAnswers { get; set; }
    public DbSet<TopicChoiseAnswer> TopicChoiseAnswers { get; set; }
    public DbSet<AISummarization> AISummarizations { get; set; }
    public DbSet<AISummarizationBatch> AISummarizationBatches { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        base.OnConfiguring(optionsBuilder);

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

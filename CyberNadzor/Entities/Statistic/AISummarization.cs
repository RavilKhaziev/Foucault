using System.ComponentModel.DataAnnotations;

namespace CyberNadzor.Entities.Statistic
{
    public class AISummarization 
    {
        [Key]
        public Guid AISummarizationId { get; set; }
        [Required]
        public Survey.Survey Survey { get; set; } 

        public List<AISummarizationBatch> Batches { get; set; } = new();

        public string? Summ { get; set; } = null;
    }
}


using CyberNadzor.Entities.Survey;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace CyberNadzor.Entities.Statistic
{
    public class AISummarizationBatch 
    {
        [Key]
        public Guid AISummarizationBatchId {  get; set; }

        public Guid AISummarizationId { get; set; }

        [Required]
        public List<TopicTextAsnwer> TopicAnswers { get; set; } = new List<TopicTextAsnwer>();

        public string? Result { get; set; } = null;

        public bool IsProcessed { get; set; } = true;
    }
}

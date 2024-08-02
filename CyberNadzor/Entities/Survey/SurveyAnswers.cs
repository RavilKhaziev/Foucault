using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CyberNadzor.Entities.Survey
{
    public class SurveyAnswers
    {
        [Key]
        public Guid SurveyAnswersId { get; set; }

        [Required]
        public IdentityUser User { get; set; } = null!;

        [Required]
        public Survey Survey { get; set; } = null!;

        public List<TopicAnswer> TopicAnswers { get; set; } = new();

        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        
        public DateTime? FinishTime { get; set; }

    }
}

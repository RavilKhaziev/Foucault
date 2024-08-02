using System.ComponentModel.DataAnnotations;

namespace CyberNadzor.Entities.Survey
{
    public class TopicAnswer
    {
        [Key]
        public Guid TopicAnswerId { get; set; }
        public Topic Topic { get; set; }    
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;

    }
}

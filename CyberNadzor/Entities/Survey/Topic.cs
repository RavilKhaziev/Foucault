using System.ComponentModel.DataAnnotations;

namespace CyberNadzor.Entities.Survey
{
    public class Topic
    {
        public enum TopicType
        {
            Text,
            Choise
        }
        [Key]
        public Guid Id { get; set; }
        public string Description { get; set; } = null!;

        public TopicType Type { get; set; }
    }
}

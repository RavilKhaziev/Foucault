using System.ComponentModel.DataAnnotations;

namespace CyberNadzor.Entities.Survey
{
    public class Topic
    {
        /// <summary>
        /// TEXT = 0,
        /// CHOISE = 1
        /// </summary>
        public enum TopicType
        {
            Text,
            Choise
        }
        [Key]
        public Guid TopicId { get; set; }
        public string Description { get; set; } = null!;

        [Required]
        public bool IsRequired { get; set; } = true;

        public TopicType Type { get; set; }
    }
}

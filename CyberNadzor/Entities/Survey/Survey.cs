using System.ComponentModel.DataAnnotations;

namespace CyberNadzor.Entities.Survey
{
    public class Survey
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        public bool IsEnable { get; set; } = false;

        public bool IsAnonymous { get; set; } = true;
        
        public List<Topic> Topics { get; set; } = new();
    }
}

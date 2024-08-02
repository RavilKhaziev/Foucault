using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;

namespace CyberNadzor.Entities.Survey
{
    public class Survey
    {
        [Key]
        public Guid SurveyId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsDeleted { get; set; } = false;

        public bool IsEnable { get; set; } = false;

        public bool IsAnonymous { get; set; } = true;

        [Required]
        public IdentityUser UserOwner { get; set; } = null!;
        
        public DateTime CreateTime { get ; set; } = DateTime.UtcNow;

        public List<Topic> Topics { get; set; } = new();
    }
}

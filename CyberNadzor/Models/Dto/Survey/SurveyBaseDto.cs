using System.ComponentModel.DataAnnotations;

namespace CyberNadzor.Models.Dto.Survey
{
    public abstract class SurveyBaseDto
    {
        [Key]
        abstract public Guid Id { get; set; }

        abstract public bool IsEnable { get; set; }
    }
}

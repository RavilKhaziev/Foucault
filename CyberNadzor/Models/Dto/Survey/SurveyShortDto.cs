using System.ComponentModel.DataAnnotations;

namespace CyberNadzor.Models.Dto.Survey
{
    public class SurveyShortDto : SurveyBaseDto
    {
        public override Guid Id { get ; set ; }
        override public bool IsEnable { get; set; }

        public string Name { get; set; }    

        public string Description { get;set; }

        public static implicit operator SurveyShortDto(Entities.Survey.Survey c) =>
            new()
            {
                Description = c.Description,
                Name = c.Name
            };
    }
}

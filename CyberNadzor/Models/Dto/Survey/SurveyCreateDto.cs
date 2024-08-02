using CyberNadzor.Entities.Survey;
using Mapster;
using System.ComponentModel.DataAnnotations;

namespace CyberNadzor.Models.Dto.Survey
{
    public class SurveyCreateDto 
    {

        [Required]
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public bool IsEnable { get; set; } = true;

        public List<TopicCreateDto> Topics { get; set; } = null;

        //public static implicit operator SurveyCreateDto(Entities.Survey.Survey c) =>
        //    new()
        //    {
        //        Description = c.Description,
        //        Id = c.SurveyId,
        //        IsEnable = c.IsEnable,
        //        Name = c.Name,
        //        Topics = c.Topics?.ConvertAll(x=> new()
        //        {
                    
        //        });
        //    };
    }
}

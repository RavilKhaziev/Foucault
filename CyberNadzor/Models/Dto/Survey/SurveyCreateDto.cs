using CyberNadzor.Entities.Survey;
using Mapster;
using System.ComponentModel.DataAnnotations;

namespace CyberNadzor.Models.Dto.Survey
{
    public class SurveyCreateDto : SurveyBaseDto
    {
        [Key]
        override public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        override public bool IsEnable { get; set; } = true;

        public List<TopicModel> Topics { get; set; } = new List<TopicModel>();

        public static implicit operator SurveyCreateDto(Entities.Survey.Survey c) =>
            new()
            {
                Description = c.Description,
                Id = c.Id,
                IsEnable = c.IsEnable,
                Name = c.Name,
                Topics = c.Topics.ConvertAll(x=> (TopicModel)x)
            };
    }
}

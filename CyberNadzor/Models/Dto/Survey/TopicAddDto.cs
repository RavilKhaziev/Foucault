using Microsoft.AspNetCore.Mvc;

namespace CyberNadzor.Models.Dto.Survey
{
    public class TopicAddDto
    {
        public Guid SurveyId { get; set; }
        public TopicCreateDto Topic { get; set; }
    }
}

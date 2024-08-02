using CyberNadzor.Entities.Survey;

namespace CyberNadzor.Models.Dto.Survey
{
    public class TopicAnswerCreateDto
    {
        public Guid TopicId { get; set; }
        public Topic.TopicType TopicType {  get; set; }
    }
}

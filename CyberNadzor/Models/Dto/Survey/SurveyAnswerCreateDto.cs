namespace CyberNadzor.Models.Dto.Survey
{
    public class SurveyAnswerCreateDto
    {
        public Guid SurveyId { get; set; }
        public List<TopicAnswerCreateDto> Answers { get; set; }
        
    }
}

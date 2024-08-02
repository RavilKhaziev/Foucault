namespace CyberNadzor.Models.Dto.Survey
{
    public class SurveyTopicsViewDto
    {
        public string SurveyName { get; set; }

        public Guid Id { get; set; }

        public List<TopicModel> Topics { get; set; }

    }
}

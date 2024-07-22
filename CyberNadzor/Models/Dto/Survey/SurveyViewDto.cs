
namespace CyberNadzor.Models.Dto.Survey
{
    public class SurveyViewDto : SurveyBaseDto
    {
        public override Guid Id { get ; set ; }
        public override bool IsEnable { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = null!;
        public bool IsAnonymous { get; set; } = true;
        public List<TopicModel> Topics {  get; set; }

    }
}

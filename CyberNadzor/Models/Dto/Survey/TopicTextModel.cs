using static CyberNadzor.Entities.Survey.Topic;

namespace CyberNadzor.Models.Dto.Survey
{
    public class TopicTextModel : TopicModel
    {
        public TopicTextModel()
        {
            Type = TopicType.Text;
        }

        public string Value { get; set; } = string.Empty;

        public static implicit operator TopicTextModel(Entities.Survey.TopicText c) =>
            new()
            {
                Description = c.Description,
                Type = c.Type,
                Value = c.Value,
            };
    }
}

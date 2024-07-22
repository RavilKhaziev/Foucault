using static CyberNadzor.Entities.Survey.Topic;

namespace CyberNadzor.Models.Dto.Survey
{
    public class TopicChoiseModel : TopicModel
    {
        public TopicChoiseModel()
        {
            Type = TopicType.Choise;
        }

        public List<string> Value { get; set; } = new();
        public static implicit operator TopicChoiseModel(Entities.Survey.TopicChoise c) =>
            new()
            {
                Description = c.Description,
                Type = c.Type,
                Value = c.Value,
            };
    }
}

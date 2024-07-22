namespace CyberNadzor.Entities.Survey
{
    public class TopicChoise : Topic
    {
        public TopicChoise()
        {
            base.Type = TopicType.Choise;
        }

        public List<string> Value { get; set; } = new();
    }
}

namespace CyberNadzor.Entities.Survey
{
    public class TopicText : Topic
    {
        public TopicText() 
        { 
            base.Type = TopicType.Text;
            base.IsRequired = false;
        }

        public string Value { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace CyberNadzor.Models.Dto.Survey
{
    public class TopicTextAnswerCreateDto : TopicAnswerCreateDto
    {
        public TopicTextAnswerCreateDto()
        {
            base.TopicType = Entities.Survey.Topic.TopicType.Text;
        }

        [Required]
        public string TopicAnswer { get; set; }
    }
}

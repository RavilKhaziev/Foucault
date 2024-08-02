using System.ComponentModel.DataAnnotations;

namespace CyberNadzor.Models.Dto.Survey
{
    public class TopicChoiseAnswerCreateDto : TopicAnswerCreateDto
    {
        public TopicChoiseAnswerCreateDto()
        {
            base.TopicType = Entities.Survey.Topic.TopicType.Choise;
        }

        [Required]
        public uint TopicAnswer { get; set; }
    }
}

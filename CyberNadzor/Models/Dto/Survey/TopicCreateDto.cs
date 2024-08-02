using CyberNadzor.Entities.Survey;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static CyberNadzor.Entities.Survey.Topic;

namespace CyberNadzor.Models.Dto.Survey
{
    public class TopicCreateDto
    {

        /// <summary>
        /// TEXT - 0, CHOISE - 1
        /// </summary>
        /// 
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public TopicType Type { get; set; }

        public List<string>? Value { get; set; }

        [Required]
        public bool IsRequired { get; set; }

        public static Topic MapTo(TopicCreateDto topicModel)
        {
            switch (topicModel.Type)
            {
                case TopicType.Text:
                    {
                        return new TopicText() { Description = topicModel.Description, Type = topicModel.Type, IsRequired = topicModel.IsRequired, Value = string.Empty };
                    }
                case TopicType.Choise:
                    {
                        return new TopicChoise() { Description = topicModel.Description, Type = topicModel.Type, IsRequired = topicModel.IsRequired, Value = topicModel?.Value ?? new() };
                    }
                default:
                    {
                        return new Topic() { Description = topicModel.Description, Type = topicModel.Type };
                    }
            }
        }
    }
}

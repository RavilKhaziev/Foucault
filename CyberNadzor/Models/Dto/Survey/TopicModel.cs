using CyberNadzor.Entities.Survey;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;
using static CyberNadzor.Entities.Survey.Topic;

namespace CyberNadzor.Models.Dto.Survey
{

    public class TopicModel
    {
        public string Description { get; set; } = null!;

        public TopicType Type { get; set; }

        public static implicit operator TopicModel(Entities.Survey.Topic c)
        {
            switch(c.Type)
            {
                case TopicType.Text:
                    {
                        return new TopicTextModel() { Description = c.Description, Type = c.Type, Value = ((TopicText)c).Value };
                    }
                case TopicType.Choise:
                    {
                        return new TopicChoiseModel() { Description = c.Description, Type = c.Type, Value = ((TopicChoise)c).Value };
                    }
                default:
                    {
                        return new TopicModel() { Description = c.Description, Type = c.Type };
                    }
            }
        }

    }
}

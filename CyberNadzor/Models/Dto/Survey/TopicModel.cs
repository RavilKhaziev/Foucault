using CyberNadzor.Entities.Survey;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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


        public static Topic MapTo(TopicModel topicModel) 
        {
            switch (topicModel.Type)
            {
                case TopicType.Text:
                    {
                        return new TopicText() { Description = topicModel.Description, Type = topicModel.Type, Value = ((TopicTextModel)topicModel).Value };
                    }
                case TopicType.Choise:
                    {
                        return new TopicChoise() { Description = topicModel.Description, Type = topicModel.Type, Value = ((TopicChoiseModel)topicModel).Value };
                    }
                default:
                    {
                        return new Topic() { Description = topicModel.Description, Type = topicModel.Type };
                    }
            }
        }

    }
}

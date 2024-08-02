using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberNadzor.Entities.Survey
{
    public class TopicTextAsnwer : TopicAnswer
    {
        [Required]
        [Column("TopicTextAsnwer_Value")]
        public string Value { get; set; }
    }
}

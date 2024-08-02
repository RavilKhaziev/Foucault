using CyberNadzor.Entities.Survey;
using NPOI.HSSF.EventUserModel.DummyRecord;

namespace CyberNadzor.Models.Dto.Statistic
{
    public class AISummarizationDto 
    {
        public string IK { get; set; }
        //public List<TopicAnswerBatchDto> Answers { get; set; }
        public string Question { get; set; }
        public List<string> Answers { get; set; }
        //  public string? LastSumm { get; set; } = null;
    }
}

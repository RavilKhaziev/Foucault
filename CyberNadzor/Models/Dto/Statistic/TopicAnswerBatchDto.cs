namespace CyberNadzor.Models.Dto.Statistic
{
    public class TopicAnswerBatchDto
    {
        public string Question { get; set; }
        public string? LastSumm { get; set; }
        public List<string> Answers { get; set; }
    }
}

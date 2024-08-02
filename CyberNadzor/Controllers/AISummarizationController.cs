using CyberNadzor.Data;
using CyberNadzor.Services;
using Microsoft.AspNetCore.Mvc;

namespace CyberNadzor.Controllers
{
    [Route("api/AI/")]
    public class AISummarizationController : Controller
    {
        private readonly ILogger<AISummarizationController> _logger;
        private readonly AIStatisticService _AIStatisticService;
        private readonly ApplicationDbContext _applicationDbContext;
        public AISummarizationController(ILogger<AISummarizationController> logger,
            AIStatisticService aIStatisticService,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _AIStatisticService = aIStatisticService;
            _applicationDbContext = dbContext;
        }

        [HttpPost("test")]
        public async Task<string> AITest()
        {
            var survey = _applicationDbContext.Surveys.Where(x => x.Name == "Анкета студента ФРГФ").FirstOrDefault();
            await _AIStatisticService.StartProcessingSurveyAsync(survey);
            return "Анкета студента ФРГФ";
        }



        [HttpPut("processed")]
        public async Task<bool> AIPackageProcessed([FromBody] Guid IK, [FromBody]string message)
        {

            return await _AIStatisticService.ProcessingWasFinishAsync(IK, message);
        }
    }
}

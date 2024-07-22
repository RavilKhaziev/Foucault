using CyberNadzor.Entities.Survey;
using CyberNadzor.Models.Dto.Survey;
using CyberNadzor.Services;
using Microsoft.AspNetCore.Mvc;

namespace CyberNadzor.Controllers
{
    [Route("/api/survey/")]
    public class SurveyController : Controller
    {
        private readonly ILogger<SurveyController> _logger;
        private readonly SurveyService _surveyService;
        public SurveyController(ILogger<SurveyController> logger, SurveyService surveyService)
        {
            _logger = logger;
            _surveyService = surveyService;
        }

        /// <summary>
        /// Получить все доступные опросники
        /// </summary>
        /// <returns>Все доступные опросники</returns>
        [HttpGet("all")]
        public async Task<List<SurveyShortDto>> GetAllSurvey()
        {
            return await _surveyService.GetSurveyAllAsync();
        }

        [HttpPost("getById")]
        public async Task<SurveyViewDto?> GetSurveyById(Guid id)
        {
            return await _surveyService.GetSurveyByIdAsync(id);
        }
        [HttpPut("create")]
        public IActionResult SurveyCreate()
        {
            return View();
        }
    }
}

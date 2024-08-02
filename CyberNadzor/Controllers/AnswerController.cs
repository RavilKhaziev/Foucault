using CyberNadzor.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CyberNadzor.Controllers
{
    /// <summary>
    /// API Для ответов на опросники.
    /// </summary>
    [Route("/api/answer/")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AnswerController : Controller
    {
        private readonly ILogger<AnswerController> _logger;
        private readonly SurveyService _surveyService;
        private readonly UserManager<IdentityUser> _userManager;

        public AnswerController(ILogger<AnswerController> logger,
            SurveyService surveyService,
            UserManager<IdentityUser> userManager
            )
        {
            _logger = logger;
            _surveyService = surveyService;
            _userManager = userManager;
        }

        public async Task<bool> StartAnswer()
        {
            return false;
        }
    }
}

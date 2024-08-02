using CyberNadzor.Entities.Survey;
using CyberNadzor.Models.Dto.Survey;
using CyberNadzor.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats;
using Org.BouncyCastle.Ocsp;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using static CyberNadzor.Entities.Survey.Topic;

namespace CyberNadzor.Controllers
{
    /// <summary>
    /// API Для создания и управления опросниками.
    /// </summary>
    [Route("/api/survey/")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SurveyController : Controller
    {
        private readonly ILogger<SurveyController> _logger;
        private readonly SurveyService _surveyService;
        private readonly UserManager<IdentityUser> _userManager;
        public SurveyController(ILogger<SurveyController> logger,
            SurveyService surveyService,
            UserManager<IdentityUser> userManager
            )
        {
            _logger = logger;
            _surveyService = surveyService;
            _userManager = userManager;
        }

        /// <summary>
        /// Получить все доступные опросники для пользователя
        /// </summary>
        /// <returns>Все доступные опросники</returns>
        [HttpGet("all")]
        public async Task<List<SurveyShortDto>> GetSurveyAllForUserAsync()
        {
            if (User?.Identity?.Name == null)
            {
                StatusCode(401);
                return new();
            }
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return new();
            }
            return await _surveyService.GetSurveyAllForUserAsync(user);
        }
        /// <summary>
        /// Получить все опросники, созданные пользователем
        /// </summary>
        /// <returns>Все доступные опросники</returns>
        [HttpGet("user/all")]
        public async Task<List<SurveyShortDto>> GetUserCreatedSurveyAllAsync()
        {
            if (User?.Identity?.Name == null)
            {
                return new();
            }
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return new();
            }
            return await _surveyService.GetUserCreatedSurveyAllAsync(user);
        }
        /// <summary>
        /// Получить все опросники, пройденные пользователем 
        /// </summary>
        /// <returns></returns>
        [HttpGet("user/answered/all")]
        public async Task<List<SurveyShortDto>> GetAllAnsweredSurveys()
        {
            if (User?.Identity?.Name == null)
            {
                return new();
            }
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return new();
            }
            return await _surveyService.GetAllAnsweredSurveys(user);
        }
        /// <summary>
        /// Получить опросник по ID
        /// </summary>
        /// <returns>Все доступные опросники</returns>
        [HttpGet("getById")]
        public async Task<SurveyViewDto?> GetSurveyById([FromQuery]Guid id)
        {
            var result = await _surveyService.GetSurveyByIdAsync(id);
            return result;
        }
        
        /// <summary>
        /// Добавление опросника
        /// </summary>
        /// <param name="surveyCreateDto"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<Guid?> PutSurveyCreate([FromBody]SurveyCreateDto surveyCreateDto)
        {
            if (User?.Identity?.Name == null)
                return new();
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
                return new();
            return await _surveyService.CreateSurveyAsync(surveyCreateDto, user);
        }

        /// <summary>
        /// Добавление топика в опросник
        /// </summary>
        /// <param name="topicDto"></param>
        /// <returns></returns>
        [HttpPut("topic")]
        public async Task<Guid?> PutTopicCreate([FromBody]TopicAddDto topicDto)
        {
            
            Topic topic = new Topic();
            switch (topicDto.Topic.Type)
            {
                case TopicType.Text:
                    {
                        topic = new TopicText() { Description = topicDto.Topic.Description, IsRequired = topicDto.Topic.IsRequired, Type = topicDto.Topic.Type };
                        break;
                    }
                case TopicType.Choise:
                    {
                        topic = new TopicChoise() { Description = topicDto.Topic.Description, IsRequired = topicDto.Topic.IsRequired, Type = topicDto.Topic.Type, Value = topicDto.Topic.Value };
                        break;
                    }
                default:
                    {
                        return null;
                    }
            }
            return await _surveyService.CreateTopicAsync(topicDto.SurveyId, topic);
        }
    }
}

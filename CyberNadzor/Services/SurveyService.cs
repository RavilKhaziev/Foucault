using CyberNadzor.Data;
using CyberNadzor.Entities.Survey;
using CyberNadzor.Models.Dto.Survey;
using CyberNadzor.Repositories.Interfaces;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Spreadsheet;
using System.Runtime.CompilerServices;

namespace CyberNadzor.Services
{
    public class SurveyService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly ILogger<SurveyService> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        public event Action<Guid> AnswerWasAdded;
        public SurveyService(ILogger<SurveyService> logger,
            ISurveyRepository surveyRepository,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext applicationDbContext
            ) 
        {
            _logger = logger;
            _surveyRepository = surveyRepository;
            _db = applicationDbContext;
            _userManager = userManager;
        }

        public async Task<Guid?> CreateSurveyAsync(SurveyCreateDto surveyModel, IdentityUser identityUser)
        {
            var survey = await _db.Surveys.AddAsync(new()
            {
                Description = surveyModel.Description,
                IsEnable = surveyModel.IsEnable,
                Name  = surveyModel.Name,
                UserOwner = identityUser,
                
            });
            foreach (var item in surveyModel.Topics)
            {
                survey.Entity.Topics.Add(TopicCreateDto.MapTo(item));
            }
            await _db.SaveChangesAsync();
            return survey.Entity.SurveyId;
        }

        public async Task<Guid?> CreateTopicAsync(Guid surveyId, Topic topic)
        {
            var survey = await _db.Surveys.FindAsync(surveyId);
            if (survey == null) return null;
            survey.Topics.Add(topic);
            await _db.SaveChangesAsync();
            return survey.Topics.LastOrDefault()?.TopicId;
        }

        public async Task<SurveyViewDto?> GetSurveyByIdAsync(Guid id)
        {
            return await _surveyRepository.GetByIdAsync(id);
        }

        public async Task<List<SurveyShortDto>> GetSurveyAllForUserAsync(IdentityUser user)
        {
            var answeredSurveys = _db.SurveyAnswers.Include(x=>x.Survey)
                .AsNoTracking().Where(x => x.Survey.IsEnable && x.User == user).Select(x=>x.Survey);
            var listNotAnswered = await _db.Surveys.AsNoTracking().Except(answeredSurveys).Select(x=>new SurveyShortDto()
            {
                Description = x.Description,
                IsEnable = x.IsEnable,
                Name = x.Name,
                Id = x.SurveyId
            }).ToListAsync();
            return listNotAnswered;
        }        
        
        public async Task<List<SurveyShortDto>> GetAllAnsweredSurveys(IdentityUser user) =>
             await _db.SurveyAnswers
            .Include(x => x.Survey)
                .AsNoTracking().Where(x => x.User == user).Select(x => new SurveyShortDto()
                {
                    Description = x.Survey.Description,
                    IsEnable = x.Survey.IsEnable,
                    Name = x.Survey.Name,
                    Id = x.Survey.SurveyId
                }).ToListAsync();    

        public async Task<List<SurveyShortDto>> GetUserCreatedSurveyAllAsync(IdentityUser user) => 
            await _db.Surveys
            .AsNoTracking()
            .Where(x=>x.UserOwner == user)
            .Select(x=> new SurveyShortDto()
            {
                Description = x.Description,
                IsEnable = x.IsEnable,
                Name = x.Name,
                Id = x.SurveyId
            }).ToListAsync();

        public async Task<bool> SurveyAnswerCreateDto(SurveyAnswerCreateDto dto, IdentityUser user)
        {
            var survey = await _db.Surveys.Include(x=>x.Topics).Where(x=>x.IsEnable && x.SurveyId == dto.SurveyId).FirstOrDefaultAsync();
            if (survey == null)
            {
                return false;
            }
            // проверяем есть ли такие топики 
            survey.Topics.All(x=> dto.Answers.Any(y=>y.TopicId == x.TopicId));
            var answer = await _db.SurveyAnswers.AddAsync(new Entities.Survey.SurveyAnswers()
            {
                Survey = survey,
                User = user
            });
            foreach (var answerDto in dto.Answers)
            {
                switch (answerDto.TopicType)
                {
                    case Entities.Survey.Topic.TopicType.Text:
                        answer.Entity.TopicAnswers.Add(new Entities.Survey.TopicTextAsnwer()
                        {
                            Topic = survey.Topics.Find(x => x.TopicId == answerDto.TopicId)!,
                            Value = ((TopicTextAnswerCreateDto)answerDto).TopicAnswer
                        });
                        break;
                    case Entities.Survey.Topic.TopicType.Choise:
                        answer.Entity.TopicAnswers.Add(new Entities.Survey.TopicChoiseAnswer()
                        {
                            Topic = survey.Topics.Find(x => x.TopicId == answerDto.TopicId)!,
                            Value = ((TopicChoiseAnswerCreateDto)answerDto).TopicAnswer
                        });
                        break;
                    default:
                        return false;
                }
            }
            await _db.SaveChangesAsync();
            AnswerWasAdded.Invoke(answer.Entity.SurveyAnswersId);
            return true;
        }

    }
}

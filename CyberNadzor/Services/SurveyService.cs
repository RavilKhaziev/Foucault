using CyberNadzor.Models.Dto.Survey;
using CyberNadzor.Repositories.Interfaces;

namespace CyberNadzor.Services
{
    public class SurveyService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly ILogger<SurveyService> _logger;
        public SurveyService(ILogger<SurveyService> logger, ISurveyRepository surveyRepository) 
        {
            _logger = logger;
            _surveyRepository = surveyRepository;
        }

        public async Task<bool> CreateSurvey(SurveyCreateDto surveyModel)
        {
            return true;
        }

        public async Task<SurveyViewDto?> GetSurveyByIdAsync(Guid id)
        {
            return await _surveyRepository.GetByIdAsync(id);
        }

        public async Task<List<SurveyShortDto>> GetSurveyAllAsync()
        {
            return await _surveyRepository.GetAllAsync();
        }        
    }
}

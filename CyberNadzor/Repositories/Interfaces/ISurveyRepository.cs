using CyberNadzor.Entities.Survey;
using CyberNadzor.Models.Dto.Survey;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CyberNadzor.Repositories.Interfaces
{
    public interface ISurveyRepository
    {
        public Task<Guid?> AddAsync(SurveyCreateDto model);

        public Task AddManyAsync(IList<SurveyCreateDto> models);

        public Task<SurveyViewDto?> GetByIdAsync(Guid? guid);

        public Task<List<SurveyShortDto>> GetAllAsync(bool withEnabled = true);
    }
}

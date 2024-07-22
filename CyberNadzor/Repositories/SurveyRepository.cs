using CyberNadzor.Data;
using CyberNadzor.Entities.Survey;
using CyberNadzor.Models.Dto.Survey;
using CyberNadzor.Repositories.Interfaces;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CyberNadzor.Repositories
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly ILogger<SurveyRepository> _logger;
        private readonly ApplicationDbContext _db;
        public SurveyRepository(ILogger<SurveyRepository> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _db = dbContext;
        }

        public Task<Guid?> AddAsync(SurveyCreateDto model)
        {
            throw new NotImplementedException();
        }

        public async Task AddManyAsync(IList<SurveyCreateDto> models)
        {

            await _db.Surveys.AddRangeAsync(models.ToList().ConvertAll(x =>
                new Survey()
                {
                    Description = x.Description,
                    IsEnable = x.IsEnable,
                    Name = x.Name,
                    Topics = x.Topics.ConvertAll<Topic>(y =>
                    {
                        switch (y.Type)
                        {
                            case Topic.TopicType.Choise: return new TopicChoise() { Description = y.Description, Type = y.Type, Value = ((TopicChoiseModel)y).Value };
                            case Topic.TopicType.Text: return new TopicText() { Description = y.Description, Type = y.Type, Value = ((TopicTextModel)y).Value };
                            default: return new TopicText() { Type = y.Type, Description = "ошибка" };
                        }
                    })
                }
            ));
            await _db.SaveChangesAsync();
        }

        public async Task<List<SurveyShortDto>> GetAllAsync(bool withEnabled = true) 
        {
            if (withEnabled)
                return await _db.Surveys.AsNoTracking().Select(x=>new SurveyShortDto()
                {
                    Description = x.Description,
                    IsEnable = x.IsEnable,
                    Id = x.Id,
                    Name = x.Name
                }).Where(x => x.IsEnable).ToListAsync();
            else
                return await _db.Surveys.AsNoTracking().Select(x => new SurveyShortDto()
                {
                    Description = x.Description,
                    IsEnable = x.IsEnable,
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync();
        }

        public async Task<SurveyViewDto> GetByIdAsync(Guid? guid)
        {
            return await _db.Surveys.AsNoTracking().Include(x=>x.Topics).Select(x => new SurveyViewDto()
            {
                Id = x.Id,
                IsEnable = x.IsEnable,
                Description = x.Description,
                IsAnonymous = x.IsAnonymous,
                Name = x.Name,
                Topics = x.Topics.ConvertAll(x=>(TopicModel)x)
            }).Where(x=>x.Id == guid).FirstAsync();
        }

    }
}

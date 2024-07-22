using CyberNadzor.Entities.Survey;
using CyberNadzor.Models.Dto.Survey;
using Mapster;

namespace CyberNadzor.Mapper
{
    public class RegisterMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Survey, SurveyCreateDto>()
                .RequireDestinationMemberSource(true);
            config.NewConfig<Survey, SurveyShortDto>()
                .RequireDestinationMemberSource(true);
        }
        
    }
}

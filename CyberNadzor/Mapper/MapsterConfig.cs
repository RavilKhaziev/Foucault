using CyberNadzor.Entities.Survey;
using CyberNadzor.Models.Dto.Survey;
using Mapster;

namespace CyberNadzor.Mapper
{
    public class MapsterConfig
    {
        public MapsterConfig()
        {
            TypeAdapterConfig<Survey, SurveyBaseDto>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.IsEnable, src => src.IsEnable);

            TypeAdapterConfig<Survey, SurveyCreateDto>.NewConfig()
                 .Map(dest => dest.Id, src => src.Id)
                 .Map(dest => dest.IsEnable, src => src.IsEnable)
                 .Map(dest => dest.Description, src => src.Description)
                 .Map(dest => dest.Name, src => src.Name)
                 .Map(dest => dest.Topics, src => src.Topics);

            TypeAdapterConfig<Survey, SurveyShortDto>.NewConfig()
                 .Map(dest => dest.Id, src => src.Id)
                 .Map(dest => dest.IsEnable, src => src.IsEnable)
                 .Map(dest => dest.Description, src => src.Description)
                 .Map(dest => dest.Name, src => src.Name);

            
        }
    }
}

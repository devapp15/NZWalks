using AutoMapper;

namespace NZWalks.API.Profiles
{
    public class RegionsProfile: Profile
    {
        public RegionsProfile()
        {
            CreateMap<Models.Domain.Region, Models.Dto.Region>()
                //.ForMember(dest => dest.Id, options => options.MapFrom(src => src.Id));
                .ReverseMap();
        }
    }
}

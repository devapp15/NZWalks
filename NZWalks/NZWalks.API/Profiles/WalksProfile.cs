using AutoMapper;

namespace NZWalks.API.Profiles
{
    public class WalksProfile: Profile
    {
        public WalksProfile()
        {
            CreateMap<Models.Domain.Walk, Models.Dto.WalkDto>()
                .ReverseMap();

            CreateMap<Models.Domain.WalkDifficulty, Models.Dto.WalkDifficultyDto>()
                .ReverseMap();
        }
    }
}

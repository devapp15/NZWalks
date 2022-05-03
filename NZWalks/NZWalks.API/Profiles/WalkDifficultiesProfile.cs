using AutoMapper;

namespace NZWalks.API.Profiles
{
    public class WalkDifficultiesProfile: Profile
    {
        public WalkDifficultiesProfile()
        {
            CreateMap<Models.Domain.WalkDifficulty, Models.Dto.WalkDifficultyDto>()
                .ReverseMap();
            CreateMap<Models.Domain.WalkDifficulty, Models.Dto.AddwalkDifficultyDto>()
                .ReverseMap();
            CreateMap<Models.Domain.WalkDifficulty, Models.Dto.UpdateWalkDifficultyDto>()
                .ReverseMap();
        }
    }
}

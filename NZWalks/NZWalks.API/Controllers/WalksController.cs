using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("walks")]
    public class WalksController : Controller
    {
        private readonly IWalkRepository _walkRepository;
        private readonly IMapper _mapper;

        public WalksController(IWalkRepository walkRepository, IMapper mapper)
        {
            _walkRepository = walkRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalksAsync()
        {
            var walks = await _walkRepository.GetAllAsync();
            //var walksDto = _mapper.Map<List<Models.Dto.WalkDto>>(walks);

            var walksDto = new List<Models.Dto.WalkDto>();
            walks.ToList().ForEach(walk =>
            {
                var walkDto = new Models.Dto.WalkDto();
                walkDto.Id = walk.Id;
                walkDto.Name = walk.Name;
                walkDto.WalkDifficultyId = walk.WalkDifficultyId;
                walkDto.RegionId = walk.RegionId;
                walkDto.Length = walk.Length;

                walkDto.Region = new Models.Dto.Region();
                walkDto.Region.Id = walk.Region.Id;
                walkDto.Region.Code = walk.Region.Code;
                walkDto.Region.Name = walk.Region.Name;
                walkDto.Region.Area = walk.Region.Area;
                walkDto.Region.Population = walk.Region.Population;
                walkDto.Region.Lat = walk.Region.Lat;
                walkDto.Region.Long = walk.Region.Long;

                walkDto.WalkDifficulty = new Models.Dto.WalkDifficultyDto();
                walkDto.WalkDifficulty.Id = walk.WalkDifficulty.Id;
                walkDto.WalkDifficulty.Code = walk.WalkDifficulty.Code;


                walksDto.Add(walkDto);
            });

            return Ok(walksDto);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkAsync")]
        public async Task<IActionResult> GetWalkAsync(Guid id)
        {
            //** Get Walk Domain object from database
            var walkDomain = await _walkRepository.GetAsync(id);
            if (walkDomain == null)
                return NotFound();

            //** Convert Domain object to DTO

            var walkDto = _mapper.Map<Models.Dto.WalkDto>(walkDomain);
            //var walkDto = new Models.Dto.WalkDto();
            //walkDto.Id = walkDomain.Id;
            //walkDto.Name = walkDomain.Name;
            //walkDto.WalkDifficultyId = walkDomain.WalkDifficultyId;
            //walkDto.RegionId = walkDomain.RegionId;
            //walkDto.Length = walkDomain.Length;

            //** Return response
            return Ok(walkDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkAsync([FromBody] Models.Dto.AddWalkDto addWalkRequest)
        {
            // Convert DTO to Domain Object
            var walk = new Models.Domain.Walk()
            {
                Name = addWalkRequest.Name,
                WalkDifficultyId = addWalkRequest.WalkDifficultyId,
                Length = addWalkRequest.Length,
                RegionId = addWalkRequest.RegionId
            };

            // Pass domain object to Repository to persist this
            walk = await _walkRepository.AddAsync(walk);

            // Convert domain object back to DTO
            var walkDto = new Models.Dto.WalkDto()
            {
                Id = walk.Id,
                Name = walk.Name,
                Length = walk.Length,
                RegionId = walk.RegionId,
                WalkDifficultyId = walk.WalkDifficultyId
            };

            return CreatedAtAction(nameof(GetWalkAsync), new { id = walkDto.Id }, walkDto);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkAsync ([FromRoute] Guid id, [FromBody] Models.Dto.UpdateWalkDto updateWalkRequest)
        {
            // Convert DTO to Domain object
            var walk = new Models.Domain.Walk()
            {
                Name = updateWalkRequest.Name,
                Length = updateWalkRequest.Length,
                RegionId = updateWalkRequest.RegionId,
                WalkDifficultyId = updateWalkRequest.WalkDifficultyId
            };

            // Pass details to Repository - Get Domain object in response (or null)
            walk = await _walkRepository.UpdateAsync(id, walk);

            // Handle Null (not founde)
            if (walk == null)
                return NotFound();

            // Convert back Domain to DTO
            var walkDto = new Models.Dto.WalkDto()
            {
                Id = walk.Id,
                Name = walk.Name,
                Length = walk.Length,
                RegionId = walk.RegionId,
                WalkDifficultyId = walk.WalkDifficultyId
            };

            // Return Response
            return Ok(walkDto);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkAsync([FromRoute] Guid id)
        {
            // call Repository to delete walk
            var walk = await _walkRepository.DeleteAsync(id);


            if (walk == null)
                return NotFound();

            //var walkDto = new Models.Dto.WalkDto()
            //{
            //    Id = walk.Id,
            //    Name = walk.Name,
            //    Length = walk.Length,
            //    RegionId = walk.RegionId,
            //    WalkDifficultyId = walk.WalkDifficultyId
            //};
            var walkDto = _mapper.Map<Models.Dto.WalkDto>(walk);

            return Ok(walkDto);
        }
    }
}

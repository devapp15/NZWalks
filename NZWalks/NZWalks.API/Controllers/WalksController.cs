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
        private readonly IRegionRepository _regionRepository;
        private readonly IWalkDifficultyRepository _walkDifficultyRepository;

        public WalksController(IWalkRepository walkRepository, IMapper mapper, IRegionRepository regionRepository, IWalkDifficultyRepository walkDifficultyRepository)
        {
            _walkRepository = walkRepository;
            _mapper = mapper;
            _regionRepository = regionRepository;
            _walkDifficultyRepository = walkDifficultyRepository;
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
            // Validate the incomming request
            if (!await ValidateAddWalkAsync(addWalkRequest))
                return BadRequest(ModelState);

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
            // Validate the incoming request
            if (!await ValidateUpdateWalkAsync(id, updateWalkRequest))
                return BadRequest(ModelState);

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

        #region Private methods

        private async Task<bool> ValidateAddWalkAsync(Models.Dto.AddWalkDto addWalkRequest)
        {
            if (addWalkRequest == null)
            {
                ModelState.AddModelError(nameof(addWalkRequest),
                    $"{nameof(addWalkRequest)} cannot be empty.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(addWalkRequest.Name))
                ModelState.AddModelError(nameof(addWalkRequest.Name),
                    $"{nameof(addWalkRequest.Name)} is required.");

            if (addWalkRequest.Length <= 0)
                ModelState.AddModelError(nameof(addWalkRequest.Length),
                    $"{nameof(addWalkRequest.Length)} should be greater than zero.");

            var region = await _regionRepository.GetAsync(addWalkRequest.RegionId);
            if (region == null)
                ModelState.AddModelError(nameof(addWalkRequest.RegionId),
                    $"{nameof(addWalkRequest.RegionId)} is invalid.");

            var walkDifficulty = await _walkDifficultyRepository.GetAsync(addWalkRequest.WalkDifficultyId);
            if (walkDifficulty == null)
                ModelState.AddModelError(nameof(addWalkRequest.WalkDifficultyId),
                    $"{nameof(addWalkRequest.WalkDifficultyId)} is invalid.");

            if (ModelState.ErrorCount > 0)
                return false;

            return true;
        }

        private async Task<bool> ValidateUpdateWalkAsync(Guid id,Models.Dto.UpdateWalkDto updateWalkRequest)
        {
            if (updateWalkRequest == null)
            {
                ModelState.AddModelError(nameof(updateWalkRequest),
                    $"{nameof(updateWalkRequest)} cannot be empty.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(updateWalkRequest.Name))
                ModelState.AddModelError(nameof(updateWalkRequest.Name),
                    $"{nameof(updateWalkRequest.Name)} is required.");

            if (updateWalkRequest.Length <= 0)
                ModelState.AddModelError(nameof(updateWalkRequest.Length),
                    $"{nameof(updateWalkRequest.Length)} should be greater than zero.");

            var region = await _regionRepository.GetAsync(updateWalkRequest.RegionId);
            if (region == null)
                ModelState.AddModelError(nameof(updateWalkRequest.RegionId),
                    $"{nameof(updateWalkRequest.RegionId)} is invalid.");

            var walkDifficulty = await _walkDifficultyRepository.GetAsync(updateWalkRequest.WalkDifficultyId);
            if (walkDifficulty == null)
                ModelState.AddModelError(nameof(updateWalkRequest.WalkDifficultyId),
                    $"{nameof(updateWalkRequest.WalkDifficultyId)} is invalid.");

            if (ModelState.ErrorCount > 0)
                return false;

            return true;
        }

        #endregion
    }
}

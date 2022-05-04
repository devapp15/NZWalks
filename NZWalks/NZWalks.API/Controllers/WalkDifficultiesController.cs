using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("walkdifficulties")]
    public class WalkDifficultiesController : Controller
    {
        private readonly IWalkDifficultyRepository _walkDifficultyRepository;
        private readonly IMapper _mapper;

        public WalkDifficultiesController(IWalkDifficultyRepository walkDifficultyRepository, IMapper mapper)
        {
            _walkDifficultyRepository = walkDifficultyRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalkDifficultiesAsync()
        {
            var walkDifficultiesDomain = await _walkDifficultyRepository.GetAllAsync();

            //var walkDifficultiesDto = new List<Models.Dto.WalkDifficultyDto>();

            //walkDifficultiesDomain.ToList().ForEach(walkDifficulty =>
            //{
            //    var walkDifficultyDto = new Models.Dto.WalkDifficultyDto
            //    {
            //        Id = walkDifficulty.Id,
            //        Code = walkDifficulty.Code
            //    };
            //    walkDifficultiesDto.Add(walkDifficultyDto);
            //});

            var walkDifficultiesDto = _mapper.Map<List<Models.Dto.WalkDifficultyDto>>(walkDifficultiesDomain);

            return Ok(walkDifficultiesDto);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetWalkDifficultyAsync")]
        public async Task<IActionResult> GetWalkDifficultyAsync(Guid id)
        {
            var walkDifficultyDomain = await _walkDifficultyRepository.GetAsync(id);

            if (walkDifficultyDomain == null)
                return NotFound();

            //var walkDifficultyDto = new Models.Dto.WalkDifficultyDto()
            //{
            //    Id = walkDifficultyDomain.Id,
            //    Code = walkDifficultyDomain.Code
            //};

            var walkDifficultyDto = _mapper.Map<Models.Dto.WalkDifficultyDto>(walkDifficultyDomain);

            return Ok(walkDifficultyDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddWalkDifficultyAsync([FromBody] Models.Dto.AddwalkDifficultyDto walkDifficulty)
        {
            // Validate incoming request
            if (!ValidateAddWalkDifficultyAsync(walkDifficulty))
                return BadRequest(ModelState);

            // Convert DTO to Domain model
            var walkDifficultyDomain = _mapper.Map<Models.Domain.WalkDifficulty>(walkDifficulty);

            // Call repository
            walkDifficultyDomain = await _walkDifficultyRepository.AddAsync(walkDifficultyDomain);

            // Convert Domain to DTO
            var walkDifficultyDto = _mapper.Map<Models.Dto.WalkDifficultyDto>(walkDifficultyDomain);

            // Return response
            return CreatedAtAction(nameof(GetWalkDifficultyAsync), new { Id = walkDifficultyDomain.Id }, walkDifficultyDto);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateWalkDifficulty([FromRoute] Guid id, [FromBody] Models.Dto.UpdateWalkDifficultyDto walkDifficulty)
        {
            // Validate incoming request
            if (!ValidateUpdateWalkDifficultyAsync(id, walkDifficulty))
                return BadRequest(ModelState);

            // Convert DTO to Domain Model
            var walkDifficultyDomain = _mapper.Map<Models.Domain.WalkDifficulty>(walkDifficulty);

            // Call repository
            walkDifficultyDomain = await _walkDifficultyRepository.UpdateAsync(id, walkDifficultyDomain);

            if (walkDifficultyDomain == null)
                return NotFound();

            // Convert Domain to Dto
            var walkDifficultyDto = _mapper.Map<Models.Dto.WalkDifficultyDto>(walkDifficultyDomain);

            // Return response
            return Ok(walkDifficultyDto);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkDifficulty([FromRoute] Guid id)
        {
            var walkDifficultyDomain = await _walkDifficultyRepository.DeleteAsync(id);

            if (walkDifficultyDomain == null)
                return NotFound();

            var walkDifficultyDto = _mapper.Map<Models.Dto.WalkDifficultyDto>(walkDifficultyDomain);

            return Ok(walkDifficultyDto);
        }

        #region Private methods

        private bool ValidateAddWalkDifficultyAsync(Models.Dto.AddwalkDifficultyDto addWalkDifficulty)
        {
            if (addWalkDifficulty == null)
            {
                ModelState.AddModelError(nameof(addWalkDifficulty),
                    $"{nameof(addWalkDifficulty)} difficulty is required.");
            }

            if (string.IsNullOrWhiteSpace(addWalkDifficulty.Code))
                ModelState.AddModelError(nameof(addWalkDifficulty),
                    $"{nameof(addWalkDifficulty.Code)} is required.");

            if (ModelState.ErrorCount > 0)
                return false;

            return true;
        }

        private bool ValidateUpdateWalkDifficultyAsync(Guid id, Models.Dto.UpdateWalkDifficultyDto updateWalkDifficulty)
        {
            if (updateWalkDifficulty == null)
            {
                ModelState.AddModelError(nameof(updateWalkDifficulty),
                    $"{nameof(updateWalkDifficulty)} is required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(updateWalkDifficulty.Code))
                ModelState.AddModelError(nameof(updateWalkDifficulty.Code),
                    $"{nameof(updateWalkDifficulty.Code)} is required.");

            if (ModelState.ErrorCount > 0)
                return false;

            return true;
        }

        #endregion
    }
}

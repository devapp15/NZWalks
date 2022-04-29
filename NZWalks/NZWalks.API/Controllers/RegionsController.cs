using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [ApiController]
    [Route("Regions")]
    public class RegionsController : Controller
    {
        private readonly IRegionRepository _regionRepository;
        private readonly IMapper _mapper;

        public RegionsController(IRegionRepository regionRepository, IMapper mapper)
        {
            _regionRepository = regionRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegionsAsync()
        {
            var regions = await _regionRepository.GetAllAsync();

            // return DTO regions
            //var regionsDTO = new List<Models.Dto.Region>();
            //regions.ToList().ForEach(region =>
            //{
            //    var regionDTO = new Models.Dto.Region()
            //    {
            //        Id = region.Id,
            //        Code = region.Code,
            //        Name = region.Name,
            //        Area = region.Area,
            //        Lat = region.Lat,
            //        Long = region.Long,
            //        Population = region.Population
            //    };

            //    regionsDTO.Add(regionDTO);
            //});

            var regionsDTO = _mapper.Map<List<Models.Dto.Region>>(regions);

            return Ok(regionsDTO);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [ActionName("GetRegionAsync")]
        public async Task<IActionResult> GetRegionAsync(Guid id)
        {
            var region = await _regionRepository.GetAsync(id);

            if (region == null)
                return NotFound();

            var regionDTO = _mapper.Map<Models.Dto.Region>(region);
            return Ok(regionDTO);
        }

        [HttpPost]
        public async Task<IActionResult> AddRegionAsync([FromBody] Models.Dto.AddRegionRequest addRegionRequest)
        {
            // Request(DTO) to Domain model
            var region = new Models.Domain.Region()
            {
                Code = addRegionRequest.Code,
                Area = addRegionRequest.Area,
                Lat = addRegionRequest.Lat,
                Long = addRegionRequest.Long,
                Name = addRegionRequest.Name,
                Population = addRegionRequest.Population
            };

            // Pass details to Repository
            region = await _regionRepository.AddAsync(region);

            // Convert back to DTO
            var regionDTO = new Models.Dto.Region()
            {
                Id = region.Id,
                Code = region.Code,
                Area = region.Area,
                Lat = region.Lat,
                Long = region.Long,
                Name = region.Name,
                Population = region.Population
            };

            return CreatedAtAction(nameof(GetRegionAsync), new {id = regionDTO.Id}, regionDTO);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteRegionAsync(Guid id)
        {
            // Get region from database
            var region = await _regionRepository.DeleteAsync(id);

            // If null NotFound
            if (region == null)
                return NotFound();

            // Convert response back to DTO
            var regionDTO = new Models.Dto.Region()
            {
                Id = region.Id,
                Code = region.Code,
                Area = region.Area,
                Lat = region.Lat,
                Long = region.Long,
                Name = region.Name,
                Population = region.Population
            };

            // return Ok response
            return Ok(regionDTO);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateRegionAsync([FromRoute] Guid id, [FromBody] Models.Dto.UpdateRegionRequest updateRegion)
        {
            // Convert DTO to Domain model
            var region = new Models.Domain.Region()
            {
                Code = updateRegion.Code,
                Name = updateRegion.Name,
                Area = updateRegion.Area,
                Lat = updateRegion.Lat,
                Long = updateRegion.Long,
                Population = updateRegion.Population
            };

            // Update Region using repository
            region = await _regionRepository.UpdateAsync(id, region);

            // if Null then NotFound
            if (region == null)
                return NotFound();

            // Convert Domain back to DTO
            var regionDTO = new Models.Dto.Region()
            {
                Id = region.Id,
                Code = region.Code,
                Area = region.Area,
                Name = region.Name,
                Lat = region.Lat,
                Long = region.Long,
                Population = region.Population
            };

            // Return Ok response
            return Ok(regionDTO);
        }
    }
}

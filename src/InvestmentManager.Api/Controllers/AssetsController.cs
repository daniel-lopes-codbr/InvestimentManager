using InvestmentManager.Api.Data;
using InvestmentManager.Api.Entities;
using InvestmentManager.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvestmentManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AssetsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAssets()
        {
            var assets = await _context.Assets.ToListAsync();
            return Ok(assets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAssetById(int id)
        {
            var asset = await _context.Assets.FindAsync(id);

            if(asset == null)
            {
                return NotFound();
            }

            var response = new AssetResponse
            {
                Id = asset.Id,
                Symbol = asset.Symbol,
                Name = asset.Name
            };

            return Ok(asset);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsset([FromBody] CreateAssetRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assetExists = await _context.Assets
                .AnyAsync(a => a.Symbol.ToUpper() == request.Symbol.ToUpper());

            if (assetExists)
            {
                return Conflict(new {message = $"O ativo '{request.Symbol}' já esta cadastrado"});
            }

            var newAsset = new Asset
            {
                Symbol = request.Symbol.ToUpper(),
                Name = request.Name
            };

            _context.Assets.Add(newAsset);

           await _context.SaveChangesAsync();

           var response = new AssetResponse
           {
               Id = newAsset.Id,
               Symbol = newAsset.Symbol,
               Name = newAsset.Name
           };

            return CreatedAtAction(nameof(GetAssetById), new {id = response.Id}, response);
        }
    }
}
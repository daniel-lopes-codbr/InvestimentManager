using InvestmentManager.Api.Data;
using InvestmentManager.Api.Entities;
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

            return Ok(asset);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsset([FromBody] Asset asset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var assetExists = await _context.Assets
                .AnyAsync(a => a.Symbol.ToUpper() == asset.Symbol.ToUpper());

            if (assetExists)
            {
                return Conflict(new {message = $"O ativo '{asset.Symbol}' já esta cadastrado"});
            }

            _context.Assets.Add(asset);

           await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAssetById), new {id = asset.Id}, asset);
        }
    }
}
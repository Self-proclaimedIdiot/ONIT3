using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProject.Server.Models;

namespace MyProject.Server.Controllers
{
    [Route("api/citizen")]
    [ApiController]
    public class CitizenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context = new AppDbContext(new DbContextOptions<AppDbContext>());
        public CitizenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/Citizens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Citizen>>> GetCitizens([FromQuery] string? name)
        {
            // Начинаем с подготовки запроса ко всей таблице
            IQueryable<Citizen> query = _context.citizens;

            // Если параметр name передан и он не пустой
            if (!string.IsNullOrWhiteSpace(name))
            {
                // Добавляем фильтрацию к запросу
                // ILike игнорирует регистр (PostgreSQL)
                query = query.Where(c => EF.Functions.ILike(c.FirstName, $"%{name}%"));
            }

            // Выполняем запрос и возвращаем результат (отфильтрованный или полный)
            return await query.ToListAsync();
        }
        // GET: api/Citizens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Citizen>> GetCitizen(int id)
        {
            var citizen = await _context.citizens.FindAsync(id);

            if (citizen == null) return NotFound();

            return citizen;
        }

        // POST: api/Citizens
        [HttpPost]
        public async Task<ActionResult<Citizen>> PostCitizen(Citizen citizen)
        {
            _context.citizens.Add(citizen);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCitizen), new { id = citizen.Id }, citizen);
        }

        // PUT: api/Citizens/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCitizen(int id, Citizen citizen)
        {
            if (id != citizen.Id) return BadRequest();

            _context.Entry(citizen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.citizens.Any(e => e.Id == id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Citizens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCitizen(int id)
        {
            var citizen = await _context.citizens.FindAsync(id);
            if (citizen == null) return NotFound();

            _context.citizens.Remove(citizen);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

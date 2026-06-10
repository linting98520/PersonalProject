using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalInfoApi.Data;
using PersonalInfoApi.Helpers;
using PersonalInfoApi.Models;

namespace PersonalInfoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PersonsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var persons = await _context.Persons.ToListAsync();
            return Ok(persons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null) return NotFound("查無此人");
            return Ok(person);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Person person)
        {
            if (!TaiwanIdValidator.IsValid(person.IdNumber))
                return BadRequest("身分證字號格式錯誤");

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new {id = person.Id}, person);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Person person)
        {
            if (id != person.Id) return BadRequest("ID 不一致");

            if (!TaiwanIdValidator.IsValid(person.IdNumber))
                return BadRequest("身分證字號格式錯誤");

            _context.Entry(person).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null) return NotFound("查無此人");

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
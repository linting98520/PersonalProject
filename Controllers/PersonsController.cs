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
        public async Task<IActionResult> Create(CreatePersonDto dto)
        {
            if (!TaiwanIdValidator.IsValid(dto.IdNumber))
            {
                ModelState.AddModelError("IdNumber", "身分證字號格式錯誤");
                return ValidationProblem(ModelState);
            }

            if (await _context.Persons.AnyAsync(p => dto.IdNumber == p.IdNumber))
            {
                ModelState.AddModelError("IdNumber", "身份證字號已存在");
                return ValidationProblem(ModelState);
            }

            var person = new Person
            {
                IdNumber = dto.IdNumber,
                Name = dto.Name,
                Gender = dto.Gender,
                Birthday = dto.Birthday,
                City = dto.City,
                District = dto.District,
                Address = dto.Address,
                Phone = dto.Phone
            };

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = person.Id }, person);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Person person)
        {
            if (id != person.Id)
            {
                ModelState.AddModelError("Id", "ID 不一致");
                return ValidationProblem(ModelState);
            }

            if (!TaiwanIdValidator.IsValid(person.IdNumber))
            {
                ModelState.AddModelError("IdNumber", "身分證字號格式錯誤");
                return ValidationProblem(ModelState);
            }

            if (await _context.Persons.AnyAsync(p => p.IdNumber == person.IdNumber && p.Id != id))
            {
                ModelState.AddModelError("IdNumber", "身份證字號已存在");
                return ValidationProblem(ModelState);
            }

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

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string idNumber)
        {
            if (string.IsNullOrWhiteSpace(idNumber))
            {
                ModelState.AddModelError("IdNumber", "請輸入身分證字號");
                return ValidationProblem(ModelState);
            }

            var person = await _context.Persons.Where(p => p.IdNumber.Contains(idNumber)).ToListAsync();
            return Ok(person);
        }
    }
}
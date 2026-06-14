using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PersonalInfoApi.Helpers;
using PersonalInfoApi.Models;

namespace PersonalInfoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly string[] _allowedColumns = new[] { "IdNumber", "Name", "Birthday" };

        public PersonsController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string sortBy = "IdNumber", [FromQuery] string sortOrder = "asc")
        {
            var persons = new List<Person>();
            if (!_allowedColumns.Contains(sortBy))
            
                sortBy = "IdNumber";
            var direction = sortOrder.ToLower() == "desc" ? "DESC" : "ASC";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sql = $"SELECT Id, IdNumber, Name, Gender, Birthday, City, District, Address, Phone FROM Persons ORDER BY {sortBy} {direction}";

                using (var command = new SqlCommand(sql, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        persons.Add(new Person
                        {
                            Id = reader.GetInt32(0),
                            IdNumber = reader.GetString(1),
                            Name = reader.GetString(2),
                            Gender = reader.GetString(3),
                            Birthday = reader.GetDateTime(4),
                            City = reader.GetString(5),
                            District = reader.GetString(6),
                            Address = reader.GetString(7),
                            Phone = reader.GetString(8)
                        });
                    }
                }
            }
            return Ok(persons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = "SELECT TOP(1) Id, IdNumber, Name, Gender, Birthday, City, District, Address, Phone FROM Persons WHERE Id = @Id";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var person = new Person
                            {
                                Id = reader.GetInt32(0),
                                IdNumber = reader.GetString(1),
                                Name = reader.GetString(2),
                                Gender = reader.GetString(3),
                                Birthday = reader.GetDateTime(4),
                                City = reader.GetString(5),
                                District = reader.GetString(6),
                                Address = reader.GetString(7),
                                Phone = reader.GetString(8)
                            };
                            return Ok(person);
                        }
                    }
                }
            }
            return NotFound(new { message = "查無此人" });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePersonDto dto)
        {
            if (!TaiwanIdValidator.IsValid(dto.IdNumber))
            {
                ModelState.AddModelError("IdNumber", "身分證字號格式錯誤");
                return ValidationProblem(ModelState);
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var checkSql = "SELECT COUNT(1) FROM Persons WHERE IdNumber = @IdNumber";
                using (var checkCommand = new SqlCommand(checkSql, connection))
                {
                    checkCommand.Parameters.AddWithValue("@IdNumber", dto.IdNumber);
                    var count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
                    if (count > 0)
                    {
                        ModelState.AddModelError("IdNumber", "身份證字號已存在");
                        return ValidationProblem(ModelState);
                    }
                }

                var sql = @"INSERT INTO Persons (IdNumber, Name, Gender, Birthday, City, District, Address, Phone) OUTPUT INSERTED.Id VALUES (@IdNumber, @name, @gender, @birthday, @city, @district, @address, @phone)";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdNumber", dto.IdNumber);
                    command.Parameters.AddWithValue("@name", dto.Name);
                    command.Parameters.AddWithValue("@gender", dto.Gender);
                    command.Parameters.AddWithValue("@birthday", dto.Birthday);
                    command.Parameters.AddWithValue("@city", dto.City);
                    command.Parameters.AddWithValue("@district", dto.District);
                    command.Parameters.AddWithValue("@address", dto.Address);
                    command.Parameters.AddWithValue("@phone", dto.Phone);

                    var newId = Convert.ToInt32(await command.ExecuteScalarAsync());
                    var person = new Person
                    {
                        Id = newId,
                        IdNumber = dto.IdNumber,
                        Name = dto.Name,
                        Gender = dto.Gender,
                        Birthday = dto.Birthday,
                        City = dto.City,
                        District = dto.District,
                        Address = dto.Address,
                        Phone = dto.Phone
                    };
                    return CreatedAtAction(nameof(GetById), new { id = newId }, person);
                }
            }
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

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var checkSql = "SELECT COUNT(1) FROM Persons WHERE IdNumber = @idNumber AND Id != @id";
                using (var checkCommand = new SqlCommand(checkSql, connection))
                {
                    checkCommand.Parameters.AddWithValue("@idNumber", person.IdNumber);
                    checkCommand.Parameters.AddWithValue("@id", id);
                    var count = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
                    if (count > 0)
                    {
                        ModelState.AddModelError("IdNumber", "身份證字號已存在");
                        return ValidationProblem(ModelState);
                    }
                }

                var sql = @"UPDATE Persons SET IdNumber=@idNumber, Name=@name, Gender=@gender, Birthday=@birthday, City=@city, District=@district, Address=@address, Phone=@phone WHERE Id=@id";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@idNumber", person.IdNumber);
                    command.Parameters.AddWithValue("@name", person.Name);
                    command.Parameters.AddWithValue("@gender", person.Gender);
                    command.Parameters.AddWithValue("@birthday", person.Birthday);
                    command.Parameters.AddWithValue("@city", person.City);
                    command.Parameters.AddWithValue("@district", person.District);
                    command.Parameters.AddWithValue("@address", person.Address);
                    command.Parameters.AddWithValue("@phone", person.Phone);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0) return NotFound(new { message = "查無此人" });
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sql = "DELETE FROM Persons WHERE Id = @id";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    if (rowsAffected == 0) return NotFound(new { message = "查無此人" });
                }
            }
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

            var persons = new List<Person>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sql = "SELECT Id, IdNumber, Name, Gender, Birthday, City, District, Address, Phone FROM Persons WHERE IdNumber LIKE @keyword";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@keyword", "%" + idNumber + "%");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            persons.Add(new Person
                            {
                                Id = reader.GetInt32(0),
                                IdNumber = reader.GetString(1),
                                Name = reader.GetString(2),
                                Gender = reader.GetString(3),
                                Birthday = reader.GetDateTime(4),
                                City = reader.GetString(5),
                                District = reader.GetString(6),
                                Address = reader.GetString(7),
                                Phone = reader.GetString(8)
                            });
                        }
                    }
                }
            }
            return Ok(persons);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;


namespace WebApiLab6.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        private IStudentsRepository _studentsRepository;

        public StudentsController(IStudentsRepository studentsRepository)
        {
            _studentsRepository = studentsRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<StudentEntity>> Get()
        {
            return await _studentsRepository.GetAllStudents();
        }

        [HttpGet("{id}")]
        public async Task<StudentEntity> GetStudent([FromRoute] string id)
        {
            return await _studentsRepository.GetStudent(id);
        }

        [HttpPost]
        public async Task<string> Post([FromBody] StudentEntity student)
        {
            try
            {
                await _studentsRepository.InsertNewStudent(student);

                return "Studentul s-a adaugat";
            }
            catch (System.Exception e)
            {
                return "Eroare: " + e.Message;
            }
        }

        [HttpDelete("{id}")]
        public async Task<string> Delete([FromRoute] string id)
        {
            try
            {
                await _studentsRepository.DeleteStudent(id);

                return "Studentul a fost sters";
            }
            catch (System.Exception e)
            {
                return "Eroare: " + e.Message;
            }

        }

        [HttpPut]
        public async Task<string> Edit([FromBody] StudentEntity student)
        {
            try
            {
                await _studentsRepository.EditStudent(student);

                return "Studentul a fost modificat";
            }
            catch (System.Exception e)
            {
                return "Eroare: " + e.Message;
            }
        }
    }
}
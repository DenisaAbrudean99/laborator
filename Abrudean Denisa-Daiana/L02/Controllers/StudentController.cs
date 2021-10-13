using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Repositories;

namespace Lab2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        

        [HttpGet]
        public IEnumerable<Student> Get()
        {
            return StudentRepo.Students;

            
        }

        [HttpGet("{id}")]

        public Student GetStudent(int id)
        {
            return StudentRepo.Students.FirstOrDefault(s => s.Id == id);
        }

        
        

        [HttpPost]
        public void Post([FromBody] Student student)
        {
            StudentRepo.Students.Add(student);
        }
  
        [HttpPut]

        public void Put([FromBody] Student student)
        {
            Console.Write(student.Id);
            var stud=StudentRepo.Students.FirstOrDefault(s => s.Id == student.Id);
            stud.Nume=student.Nume;
            stud.Prenume=student.Prenume;
            stud.Facultate=student.Facultate;
            stud.An=student.An;
        }

        [HttpDelete("{id}")]

        public void Delete(int id){
           StudentRepo.Students.Remove(StudentRepo.Students.FirstOrDefault(s=>s.Id==id)) ;

        }
    }
}


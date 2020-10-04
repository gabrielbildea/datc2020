using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace students_api.Controllers
{
    [ApiController]
    [Route("[controller]")] 
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;

        [HttpGet]
        public IEnumerable<Student> Get()
        {
            return StudentRepo.StudentsList;
        }

        [HttpGet("{id}")]
        public Student afiseazaStudent(int id) 
        {
            //returnam primul student din lista de studenti "StudentsList" cu id-ul preluat din apelul functiei
            return StudentRepo.StudentsList.FirstOrDefault(s => s.Id == id); 
        }

        [HttpPost]
        public string adaugaStudent([FromBody] Student student)
        {
            try
            {
                StudentRepo.StudentsList.Add(student); //adaugam studentul in lista de studenti
                return "Studentul a fost adaugat cu succes!";
            }
            catch(System.Exception e)
            {
                return "Eroare la adaugare: " + e.Message;
                throw;
            }
        }
        
        [HttpPut]
        public string actualizeazaDate([FromBody] Student student){
            int idActualizare = StudentRepo.StudentsList.FindIndex(s => s.Id == student.Id); //cautam studentul cu id-ul dat
            try
            {
                //actualizam informatia
                StudentRepo.StudentsList[idActualizare].Nume = student.Nume;
                StudentRepo.StudentsList[idActualizare].Prenume = student.Prenume;
                StudentRepo.StudentsList[idActualizare].Facultate = student.Facultate;
                StudentRepo.StudentsList[idActualizare].An_de_studiu = student.An_de_studiu;
                return "Modificare executata cu succes!";
            }
            catch(System.Exception e)
            {
                return "Eroare la modificare: " + e.Message;
                throw;
            } 
            
        }

        [HttpDelete("{id}")]
        public string stergeStudent([FromRoute] int id)
        {
            try
            {
                Student studentToDelete = StudentRepo.StudentsList.First(s => s.Id == id); // cautam studentul in lista de studenti
                StudentRepo.StudentsList.Remove(studentToDelete); // stergem studentul din lista
                return "Studentul a fost sters cu succes!";
            }
            catch(System.Exception e)
            {
                return "Eroare la stergere: " + e.Message;
                throw;
            }
        }

        public StudentController(ILogger<StudentController> logger)
        {
            _logger = logger;
        }
    }
}
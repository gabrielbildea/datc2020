using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace L04.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        private IStudentRepository _studentRepository;
        public StudentsController(IStudentRepository studentRepository) 
        {
            _studentRepository = studentRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<StudentEntity>> Afiseaza()
        {
           return await _studentRepository.AfisareStudenti();
        }

        [HttpPost]
        public async Task<string> Adauga([FromBody] StudentEntity student)
        {
            try
            {
                await _studentRepository.AdaugareStudent(student);
                return "Studentul a fost adaugat cu succes!";
            }
            catch(System.Exception e)
            {
                return "Eroare la adaugare: " + e.Message;
                throw;
            }
        }

        [HttpPut("{partitionKey}/{rowKey}")]
        public async Task<string> Update([FromRoute] string partitionKey, [FromRoute] string rowKey, [FromBody] StudentEntity student)
        {
            try
            {
                await _studentRepository.ModificareStudent(partitionKey, rowKey, student);
                return "Modificare executata cu succes!";
            }
            catch(System.Exception e)
            {
                return "Eroare la modificare: " + e.Message;
                throw;
            } 
        }

        [HttpDelete("{partitionKey}/{rowKey}")]
        public async Task<string> Delete([FromRoute] string partitionKey, [FromRoute] string rowKey)
        {
            try
            {
                await _studentRepository.StergereStudent(partitionKey, rowKey);
                return "Studentul a fost sters cu succes!";
            }
            catch(System.Exception e)
            {
                return "Eroare la stergere: " + e.Message;
                throw;
            }
        }
    }
}

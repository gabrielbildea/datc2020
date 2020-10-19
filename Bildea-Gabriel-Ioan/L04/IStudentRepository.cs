using System.Collections.Generic;
using System.Threading.Tasks;
using L04;

public interface IStudentRepository
{
    Task<List<StudentEntity>> AfisareStudenti();

    Task AdaugareStudent(StudentEntity student);

    Task ModificareStudent(string partitionKey, string rowKey, StudentEntity student);

    Task StergereStudent(string partitionKey, string rowKey);
}
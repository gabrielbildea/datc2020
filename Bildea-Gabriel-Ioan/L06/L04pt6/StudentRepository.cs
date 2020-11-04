using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Azure.Storage.Queues;

namespace L04
{
    public class StudentRepository : IStudentRepository 
    {
        private string _connectionString;
        private CloudTableClient _tableClient;
        private CloudTable _studentsTable;
        private List<StudentEntity> students;
        public StudentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue<string>("AzureStorageAccountConnectionString");
            Task.Run(async () => { await InitializeTable(); })
            .GetAwaiter().GetResult();
        }

        private async Task InitializeTable()
        {
            var account = CloudStorageAccount.Parse(_connectionString);
            _tableClient = account.CreateCloudTableClient();
            _studentsTable = _tableClient.GetTableReference("studenti");
            await _studentsTable.CreateIfNotExistsAsync();
        }

        public async Task AdaugareStudent(StudentEntity student)
        {
            //var insertOperation = TableOperation.Insert(student);
            //await _studentsTable.ExecuteAsync(insertOperation);

            string jsonStudent = JsonConvert.SerializeObject(student);
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jsonStudent);
            string base64String = System.Convert.ToBase64String(plainTextBytes);

            QueueClient queueClient = new QueueClient(
                _connectionString,
                "students-queue"
            );
            queueClient.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            await queueClient.SendMessageAsync(base64String);
        }

        public async Task StergereStudent(string partitionKey, string rowKey)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<StudentEntity>(partitionKey, rowKey);
            TableResult resultRetrieve = await _studentsTable.ExecuteAsync(retrieveOperation);
            StudentEntity deleteStudent = (StudentEntity)resultRetrieve.Result;
            TableOperation deleteOperation = TableOperation.Delete(deleteStudent);
            await _studentsTable.ExecuteAsync(deleteOperation);
        }

        public async Task ModificareStudent(string partitionKey, string rowKey, StudentEntity student)
        {
            student.PartitionKey = partitionKey;
            student.RowKey = rowKey;
            student.ETag = "*"; //nu vom primi eroare atunci cand ETag-ul nu se potriveste
            TableOperation updateOperation = TableOperation.Replace(student);
            await _studentsTable.ExecuteAsync(updateOperation);
        }

        public async Task<List<StudentEntity>> AfisareStudenti()
        {
            students = new List<StudentEntity>();
            TableQuery<StudentEntity> query = new TableQuery<StudentEntity>();
            TableContinuationToken token = null; 
            do
            {
                TableQuerySegment<StudentEntity> resultSegment = await _studentsTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;
                students.AddRange(resultSegment.Results);

            } while(token != null);
            return students;
        }
    }
}
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;


namespace L05
{
    public class MetricsRepository
    {
        private string _connectionString;
        private CloudTable _studentsTable, _metricsTable;
        private CloudTableClient _tableClient;

        private async Task InitializeTable()
        {
            var account = CloudStorageAccount.Parse(_connectionString);
            _tableClient = account.CreateCloudTableClient();
            _studentsTable = _tableClient.GetTableReference("studenti");
            await _studentsTable.CreateIfNotExistsAsync();

            _metricsTable = _tableClient.GetTableReference("statistica");
            await _metricsTable.CreateIfNotExistsAsync();
        }
        public MetricsRepository()
        {
            _connectionString = "DefaultEndpointsProtocol=https;AccountName=datcgabi;AccountKey=kscPR5DlPY3wBokDDdvlZkc4kfQYM/mEiTxDChWWcvsP/TFItBuR38TFvQ4HrNcj6OP7gGYk189wvWqbPLHlZQ==;EndpointSuffix=core.windows.net";
            Task.Run(async () => 
            { 
                await InitializeTable();
            }).GetAwaiter().GetResult();

        }

        public void InsertStatistic(string partitionKey, int count)
        {
            MetricsEntity newMetrics = new MetricsEntity(partitionKey, DateTime.UtcNow.ToString().Replace("/", "."));
            newMetrics.Count = count;

            TableOperation generalInsert = TableOperation.Insert(newMetrics);
            TableResult generalResult = _metricsTable.ExecuteAsync(generalInsert).GetAwaiter().GetResult();
        }

        public async Task GetStatistics()
        {
            List<StudentEntity> studentsList = new List<StudentEntity>();
            TableQuery<StudentEntity> query = new TableQuery<StudentEntity>();
            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<StudentEntity> resultSegment = await _studentsTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;
                studentsList.AddRange(resultSegment.Results);

            } while (token != null);
            InsertStatistic("General", studentsList.Count); //general
            
            List<StudentEntity> SortedList = new List<StudentEntity>(); 
            SortedList = studentsList.OrderBy(student => student.Faculty).ToList();
            List<List<StudentEntity>> listOfList = SortedList.GroupBy(student => student.Faculty).Select(group => group.ToList()).ToList();
            listOfList.ForEach( list =>
            {
                Console.WriteLine("Fac.: "+ list[0].Faculty +", Count: "+ list.Count );
                 InsertStatistic(list[0].Faculty, list.Count); //university
            });
        }

       
    }
}
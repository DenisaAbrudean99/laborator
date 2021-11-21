using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Models;
using Newtonsoft.Json;
using Azure.Storage.Queues;

namespace WebApi
{

    public class StudentsRepository : IStudentsRepository
    {
        private string _connectionString;

        private CloudTableClient _tableClient;

        private CloudTable _studentsTable;


        private async Task InitializeTable()
        {
            var account = CloudStorageAccount.Parse(_connectionString);
            _tableClient = account.CreateCloudTableClient();
            _studentsTable = _tableClient.GetTableReference("student");
            await _studentsTable.CreateIfNotExistsAsync();

        }

        public StudentsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue(typeof(string), "AzureStorageAccountConnectionString").ToString();

            Task.Run(async () => { await InitializeTable(); }).GetAwaiter().GetResult();
        }

        public async Task<List<StudentEntity>> GetAllStudents()
        {
            var students = new List<StudentEntity>();

            TableQuery<StudentEntity> query = new TableQuery<StudentEntity>();

            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<StudentEntity> resultSegment = await _studentsTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                students.AddRange(resultSegment.Results);

            } while (token != null);

            return students;
        }

        public async Task<StudentEntity> GetStudent(string id)
        {
            var parsedId = ParseStudentId(id);

            var partitionKey = parsedId.Item1;
            var rowKey = parsedId.Item2;

            var query = TableOperation.Retrieve<StudentEntity>(partitionKey, rowKey);

            var result = await _studentsTable.ExecuteAsync(query);

            return (StudentEntity)result.Result;
        }

        public async Task InsertNewStudent(StudentEntity student)
        {


            var jsonStudent = JsonConvert.SerializeObject(student);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jsonStudent);
            var base64String = System.Convert.ToBase64String(plainTextBytes);

            QueueClient queueClient = new QueueClient(
                _connectionString,
                "coadalab6"
            );

            queueClient.CreateIfNotExists();
            await queueClient.SendMessageAsync(base64String);

        }

        public async Task DeleteStudent(string id)
        {
            var parsedId = ParseStudentId(id);

            var partitionKey = parsedId.Item1;
            var rowKey = parsedId.Item2;

            var entity = new DynamicTableEntity(partitionKey, rowKey) { ETag = "*" };

            await _studentsTable.ExecuteAsync(TableOperation.Delete(entity));
        }

        public async Task EditStudent(StudentEntity student)
        {

            var editOperation = TableOperation.Merge(student);

            try
            {
                await _studentsTable.ExecuteAsync(editOperation);
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.PreconditionFailed)
                    throw new System.Exception("Studentul a fost modificat deja");
            }
        }


        private (string, string) ParseStudentId(string id)
        {
            var elements = id.Split('-');

            return (elements[0], elements[1]);
        }
    }
}
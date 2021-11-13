using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Models;
namespace L05
{
    class Program
    {
        private static CloudTableClient tableClient;
        private static CloudTable studentsTable;
        private static CloudTable studentsTable2;
        static void Main(string[] args)
        {
            Task.Run(async () => { await Initialize(); })
            .GetAwaiter()
            .GetResult();
        }
        static async Task Initialize()
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;"
            + "AccountName=azurestoragetema"
            +";AccountKey=bFDVYOJUiMArERq+pIC1AsLm9yJtjgUrwFD3jUO1d6EDtojszArlVkB18E6hDtarePTMPYKS+kJim9Gv4S8kHw=="
            +";EndpointSuffix=core.windows.net";



            var account = CloudStorageAccount.Parse(storageConnectionString);

            tableClient = account.CreateCloudTableClient();

            studentsTable = tableClient.GetTableReference("studenti");
            studentsTable2 = tableClient.GetTableReference("student2");

            await studentsTable2.CreateIfNotExistsAsync();


            await GetAllStudentsFromOld();
            await GetAllStudentsFromNew();
        }

        private static async Task AddNewStudent(string pk, int c)
        {
            var student2 = new StudentEntity(pk);
            student2.Count = c;

            var insertOperation = TableOperation.Insert(student2);
            await studentsTable2.ExecuteAsync(insertOperation);
        }

        private static async Task GetAllStudentsFromOld()
        {
            int count = 0;
            string[] university = new string[6];
            int[] countarray = new int[6];
            int n = 0;


            TableQuery<StudentEntity> query = new TableQuery<StudentEntity>();
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<StudentEntity> resultSegment = await studentsTable.ExecuteQuerySegmentedAsync(query, token);

                token = resultSegment.ContinuationToken;

                foreach (StudentEntity entity in resultSegment.Results)
                {

                    count++;

                    if (university[0] == null)
                    {
                        university[0] = entity.PartitionKey;
                        countarray[0] = 0;
                        n = 1;
                    }
                    for (int i = 0; i < n; i++)
                    {
                        if (university[i].Equals(entity.PartitionKey))
                        {

                            countarray[i]++;
                        }
                        else
                        {

                            university[n] = entity.PartitionKey;
                            n++;
                        }
                    }

                }
            } while (token != null);

            await AddNewStudent("Generald", count);
            for (int i = 0; i < n; i++)
            {
                await AddNewStudent(university[i], countarray[i]);
            }

        }


        private static async Task GetAllStudentsFromNew()
        {
            Console.WriteLine("PartitionKey\tRowKey\tCount");

            TableQuery<StudentEntity> query = new TableQuery<StudentEntity>();
            TableContinuationToken token = null;

            do
            {

                TableQuerySegment<StudentEntity> resultSegment = await studentsTable2.ExecuteQuerySegmentedAsync(query, token);

                token = resultSegment.ContinuationToken;

                foreach (StudentEntity entity in resultSegment.Results)
                {
                    Console.WriteLine("{0}\t{1}\t{2}", entity.PartitionKey, entity.RowKey, entity.Count);

                }
            } while (token != null);
        }
    }
}

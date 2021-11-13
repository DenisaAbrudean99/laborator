using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Models
{
    public class StudentEntity : TableEntity
    {
        public StudentEntity(string university)
        {
            this.PartitionKey = university;
            this.RowKey = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public StudentEntity() { }

        public int Count { get; set; }


    }
}

using Microsoft.WindowsAzure.Storage.Table;

namespace L05
{
    public class MetricsEntity : TableEntity
    {
        public MetricsEntity(){ }
        public MetricsEntity(string university, string timestamp)
        {
            this.PartitionKey = university;
            this.RowKey = timestamp;
        }
        public int Count {get; set;}
    }
}
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace ProcessLogger
{
    public class LogRecord
    {
        private readonly string connectionString;
        private const string Insert = @"INSERT INTO [dbo].[Logging]
           ([Id]
           ,[Processor]
           ,[MessageId]
           ,[Created])
     VALUES
           (@Id
           ,@Processor
           ,@MessageId
           ,@Created)";

        public LogRecord(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task Store(LogRecord.Entity record)
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(Insert, new { Id = record.Id, Processor = record.Processor, MessageId = record.MessageId, Created = record.Created });
        }

        public class Entity
        {
            public Guid Id = Guid.NewGuid();
            public DateTime Created { get; } = DateTime.UtcNow;

            public int MessageId { get; }
            public string Processor { get; }

            public Entity(int messageId, string processor)
            {
                MessageId = messageId;
                Processor = processor;
            }
        }
    }
}

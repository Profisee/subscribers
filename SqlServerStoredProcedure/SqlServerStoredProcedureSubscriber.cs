using Profisee.Common.EventProcessing.DataContracts;
using Profisee.Common.EventProcessing.Enums;
using Profisee.Common.EventProcessing.Interfaces;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;

namespace Profisee.EventSubscribers.SqlServerStoredProcedureSubscriber
{
    [Export(typeof(ISubscribe))]
    public class SqlServerStoredProcedureSubscriber : SubscriberBase
    {
        public const string ConnectionStringPropertyName = "Connection String";
        public const string StoredProcPropertyName = "Stored Procedure Name";

        private string _connectionString = string.Empty;
        private string _storedProcName = string.Empty;

        public SqlServerStoredProcedureSubscriber() : base("SqlServerStoredProcedureSubscriber")
        {
            this.AddConfigurationProperty(new ConfigurationProperty
            {
                Name = ConnectionStringPropertyName,
                Description = "SQL Connection String",
                Type = EventSubscriberConfigurationPropertyType.Text,
                IsRequired = true,
            });

            this.AddConfigurationProperty(new ConfigurationProperty
            {
                Name = StoredProcPropertyName,
                Description = "SQL Stored Procedure Name",
                Type = EventSubscriberConfigurationPropertyType.Text,
                IsRequired = true,
            });
        }

        public override ValidationResult ValidateSubscriber()
        {
            var result = new ValidationResult();

            _connectionString = this.ConfigurationPropertyValues.GetEventSubscriberConfigurationPropertyValueAsString(ConnectionStringPropertyName);
            _storedProcName = this.ConfigurationPropertyValues.GetEventSubscriberConfigurationPropertyValueAsString(StoredProcPropertyName);

            return result;
        }

        public override ValidationResult ExecuteSubscriber(SubscriberContext context)
        {
            var result = new ValidationResult();

            result = ExecuteStoredProcedure(_connectionString, _storedProcName, context.ExecuteContext);

            return result;
        }

        private ValidationResult ExecuteStoredProcedure(string connectionString, string storedProcName, IExecuteContext context)
        {
            var result = new ValidationResult();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProcName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@EntityName", SqlDbType.NVarChar).Value = context.Member.EntityId.Name;
                    cmd.Parameters.Add("@MemberCode", SqlDbType.NVarChar).Value = context.Member.MemberCode;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return result;
        }
    }
}

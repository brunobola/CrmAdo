using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SQLGeneration.Builders;
using SQLGeneration.Generators;

namespace DynamicsCrmDataProvider
{
    public class CrmCommandExecutor : ICrmCommandExecutor
    {
        private ICrmQueryExpressionProvider _CrmQueryExpressionProvider;

        #region Constructor
        public CrmCommandExecutor()
            : this(new CrmQueryExpressionProvider())
        {
        }

        public CrmCommandExecutor(ICrmQueryExpressionProvider queryExpressionProvider)
        {
            _CrmQueryExpressionProvider = queryExpressionProvider;
        }
        #endregion
        
        public EntityCollection ExecuteCommand(CrmDbCommand command)
        {
            //TODO: Should process the command text, and execute a query to dynamics, returning the Entity Collection results.
            // what would these command types mean in terms of dynamics queries?
            EntityCollection results = null;
            switch (command.CommandType)
            {
                case CommandType.Text:
                    results = ProcessTextCommand(command);
                    break;
                case CommandType.TableDirect:
                    results = ProcessTableDirectCommand(command);
                    break;
                case CommandType.StoredProcedure:
                    results = ProcessStoredProcedureCommand(command);
                    break;
            }
            return results;
        }

        private EntityCollection ProcessTableDirectCommand(CrmDbCommand command)
        {
            // The command should be the name of a single entity.
            var entityName = command.CommandText;
            if (entityName.Contains(" "))
            {
                throw new ArgumentException("When CommandType is TableDirect, CommandText should be the name of an entity.");
            }

            var orgService = command.CrmDbConnection.OrganizationService;
            // Todo: possibly support paging by returning a PagedEntityCollection implementation? 
            var results = orgService.RetrieveMultiple(new QueryExpression(entityName) { ColumnSet = new ColumnSet(true) });
            return results;
        }

        private EntityCollection ProcessTextCommand(CrmDbCommand command)
        {
            //  string commandText = "SELECT CustomerId, FirstName, LastName, Created FROM Customer";
            var commandText = command.CommandText;
            var commandBuilder = new CommandBuilder();
            var cmd = commandBuilder.GetCommand(commandText);
            var queryExpression = _CrmQueryExpressionProvider.CreateQueryExpression(cmd as SelectBuilder);
            var orgService = command.CrmDbConnection.OrganizationService;
            var results = orgService.RetrieveMultiple(queryExpression);
            return results;
        }

        private EntityCollection ProcessStoredProcedureCommand(CrmDbCommand command)
        {
            // What would a stored procedure be in terms of Dynamics Crm SDK?
            // Perhaps this could be used for exectuign fetch xml commands...?
            throw new System.NotImplementedException();
        }

        public int ExecuteNonQueryCommand(CrmDbCommand command)
        {
            // You can use ExecuteNonQuery to perform catalog operations (for example, querying the structure of a database or creating database objects such as tables), or to change the data in a database by executing UPDATE, INSERT, or DELETE statements.
            // Although ExecuteNonQuery does not return any rows, any output parameters or return values mapped to parameters are populated with data.
            // For UPDATE, INSERT, and DELETE statements, the return value is the number of rows affected by the command. For all other types of statements, the return value is -1.
            return -1;
        }
    }
}
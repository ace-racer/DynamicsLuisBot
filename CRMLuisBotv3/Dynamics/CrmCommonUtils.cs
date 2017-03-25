using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMLuisBotv3.Dynamics
{
    public static class CrmCommonUtils
    {
        /// <summary>
        /// Retrieves the entity by unique values.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="queryColumns">The query columns.</param>
        /// <param name="requiredColumns">The required columns.</param>
        /// <param name="service">The service.</param>
        /// <returns>The entity as per the provided parameters</returns>
        public static Entity RetrieveEntityByUniqueValues(string entityName, Dictionary<string, object> queryColumns, IEnumerable<string> requiredColumns, IOrganizationService service)
        {
            if (!string.IsNullOrWhiteSpace(entityName) && queryColumns != null && requiredColumns != null &&
                service != null)
            {
                var query = new QueryExpression(entityName);
                query.ColumnSet = new ColumnSet();
                foreach (var requiredColumn in requiredColumns)
                {
                    query.ColumnSet.AddColumn(requiredColumn);
                }

                foreach (var queryColumn in queryColumns)
                {
                    query.Criteria.AddCondition(queryColumn.Key, ConditionOperator.Equal, queryColumn.Value);
                }

                var recordsCollection = service.RetrieveMultiple(query);
                if (recordsCollection != null && recordsCollection.Entities != null &&
                    recordsCollection.Entities.Count > 0)
                {
                    return recordsCollection.Entities[0];
                }
            }

            return null;
        }
    }
}
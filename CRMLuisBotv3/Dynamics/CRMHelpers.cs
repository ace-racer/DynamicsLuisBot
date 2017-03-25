using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CRMLuisBotv3.Dynamics
{
    public static class CRMHelpers
    {
        public static bool TryGetContactDetailsFromDynamics(string phoneNumber, out Entity contactEntity)
        {
            contactEntity = null;
            if(!string.IsNullOrWhiteSpace(phoneNumber))
            {
                CrmServiceClient crmSvcClient;
                var connectionString = ConfigurationManager.ConnectionStrings["CRMConnectionString"].ConnectionString;
                using (crmSvcClient = new CrmServiceClient(connectionString))
                {
                    contactEntity = CrmCommonUtils.RetrieveEntityByUniqueValues("contact", new Dictionary<string, object>()
                    {
                        { "telephone1", phoneNumber }
                    }, new List<string>()
                    {
                        "fullname", "telephone1"
                    }, crmSvcClient);

                    if(contactEntity != null)
                    {
                        return true;
                    }                    
                }               
            }

            return false;
        }
    }
}
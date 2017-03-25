using CRMLuisBotv3.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
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
        /// <summary>
        /// Tries the get details from dynamics.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="crmDetail">The CRM detail.</param>
        /// <returns>Whether the contact is present in Dynamics</returns>
        public static bool TryGetDetailsFromDynamics(string phoneNumber, out CrmDetail crmDetail)
        {
            crmDetail = new CrmDetail(); 
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                CrmServiceClient crmSvcClient;
                var connectionString = ConfigurationManager.ConnectionStrings["CRMConnectionString"].ConnectionString;
                using (crmSvcClient = new CrmServiceClient(connectionString))
                {
                    var contactEntity = CrmCommonUtils.RetrieveEntityByUniqueValues("contact", new Dictionary<string, object>()
                    {
                        { "telephone1", phoneNumber }
                    }, new List<string>()
                    {
                        "fullname", "telephone1"
                    }, crmSvcClient);

                    if (contactEntity != null)
                    {
                        crmDetail.ContactDetails.FullName = contactEntity.GetAttributeValue<string>("fullname");
                        crmDetail.ContactDetails.PhoneNumber = contactEntity.GetAttributeValue<string>("telephone1");
                        crmDetail.ContactCaseDetails = GetCasesAssociatedToContact(contactEntity, crmSvcClient);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the cases associated to contact.
        /// </summary>
        /// <param name="contactEntity">The contact entity.</param>
        /// <param name="orgService">The org service.</param>
        /// <returns>The cases from CRM related to the contact</returns>
        private static List<CaseDetails> GetCasesAssociatedToContact(Entity contactEntity, IOrganizationService orgService)
        {
            var allCaseDetails = new List<CaseDetails>();
            if (contactEntity != null)
            {
                var casesFetchXml = "<?xml version='1.0'?>" +
                "<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>" +
                "<entity name='incident'>" +
                "<attribute name='title'/>" +
                "<attribute name='ticketnumber'/>" +
                "<attribute name='createdon'/>" +
                "<attribute name='incidentid'/>" +
                "<attribute name='caseorigincode'/>" +
                "<attribute name='statuscode'/>" +
                "<attribute name='statecode'/>" +
                "<attribute name='modifiedon'/>" +
                "<attribute name='description'/>" +
                "<order descending='false' attribute='title'/>" +
                "<filter type='and'>" +
                "<condition attribute='customerid' value='" + contactEntity.Id + "' uitype='contact' operator='eq'/>" +
                "</filter>" +
                "</entity>" +
                "</fetch>";
                var casesAssociatedWithContact = orgService.RetrieveMultiple(new FetchExpression(casesFetchXml));
                if (casesAssociatedWithContact != null && casesAssociatedWithContact.Entities != null)
                {
                    foreach (var caseAssociatedWithContact in casesAssociatedWithContact.Entities)
                    {
                        var caseDetails = new CaseDetails()
                        {
                            CaseId = caseAssociatedWithContact.GetAttributeValue<string>("ticketnumber"),
                            CaseTitle = caseAssociatedWithContact.GetAttributeValue<string>("ticketnumber"),
                            CaseDescription = caseAssociatedWithContact.GetAttributeValue<string>("description"),
                            ModifiedOn = caseAssociatedWithContact.GetAttributeValue<DateTime>("modifiedon")
                        };
                        allCaseDetails.Add(caseDetails);
                    }
                }

            }

            return allCaseDetails;
        }
    }
}
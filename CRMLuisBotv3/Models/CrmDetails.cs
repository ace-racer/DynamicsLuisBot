using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMLuisBotv3.Models
{
    public class CrmDetail
    {
        public ContactDetails ContactDetails { get; set; }

        public List<CaseDetails> ContactCaseDetails { get; set; }

        public CrmDetail()
        {
            this.ContactCaseDetails = new List<CaseDetails>();
            this.ContactDetails = new ContactDetails();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMLuisBotv3.Models
{
    public class CaseDetails
    {
        public string CaseId { get; set; }

        public string CaseTitle { get; set; }

        public string CaseDescription { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public bool IsCaseOpen { get; set; }

        public string CurrentStatus { get; set; }      
    }
}
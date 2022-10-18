using Hrmis.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Common
{
    public class Filter : Paginator
    {
        public string Query { get; set; }
        public int DesignationId { get; set; }

    }
    public class StatusChange
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public int InterviewViewMarks { get; set; }
        public AdhocApplicantLog adhocApplicantLog { get; set; }
    }
}
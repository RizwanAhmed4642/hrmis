using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Common
{
    public class SearchQuery
    {
        public string Query { get; set; }
        public string HFMISCode { get; set; }
        public int designationId { get; set; }
    }
    public class SearchResult
    {
        public int Id { get; set; }
        public string PhotoPath { get; set; }
        public string Name { get; set; }
        public string CNIC { get; set; }
        public string HFMISCode { get; set; }
        public int Type { get; set; }
        public string ResultType { get; set; }
    }
}
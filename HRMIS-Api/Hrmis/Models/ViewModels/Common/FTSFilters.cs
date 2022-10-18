using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.ViewModels.Common
{
    public class FTSFilters
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public DateTime? FixedDate { get; set; }
        public string Program { get; set; }
        public string Type { get; set; }
        public int OfficerId { get; set; }
        public int TypeId { get; set; }
        public int SourceId { get; set; }
        public int Skip { get; set; }
        public int PageSize { get; set; }
        public string UserId { get; set; }
        public string SignedBy { get; set; }
        public string DistrictCode { get; set; }
    }
}
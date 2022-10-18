using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Dto
{
    public class PaginatedFilterDTO
    {
        public int PageNumber { get; set; } = 1;
        public int TotalRecords { get; set; }
        public int Size { get; set; }
        public int PageCount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string QueryString { get; set; }
        public string DivisionCode { get; set; }
        public string DistrictCode { get; set; }
        public string TehsilCode { get; set; }
        public string healthfacilityCode { get; set; }
        public int PatientListId { get; set; }
        public bool IsForExcel { get; set; }
        public virtual IEnumerable<int> testID { get; set; }
    }
}
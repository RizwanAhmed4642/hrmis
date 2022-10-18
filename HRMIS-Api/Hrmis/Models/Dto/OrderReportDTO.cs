using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Dto
{
    public class OrderReportDTO
    {
        public int ESRId { get; set; }
        public string EmployeeName { get; set; }
        public string CNIC { get; set; }
        public string Designation { get; set; }
        public int Scale { get; set; }
        public string Contact { get; set; }
        public string OrderType { get; set; }
        public string DistrictName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string NotingPath { get; set; }
        public string OrderHTML { get; set; }
        public DateTime DateTime { get; set; }
    }
}
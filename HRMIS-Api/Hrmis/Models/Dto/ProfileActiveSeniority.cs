using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Dto
{
    public class ProfileActiveSeniority
    {
        public string EmployeeName { get; set; }
        public string CNIC { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? LengthOfService { get; set; }
        public string Tenure {
            get {
                if (LengthOfService == null) return string.Empty;
                var totalDays = LengthOfService ?? 0.0;
                var totalYears = Math.Truncate(totalDays / 365);
                var totalMonths = Math.Truncate((totalDays % 365) / 30);
                var remainingDays = Math.Truncate((totalDays % 365) % 30);
                return $"{totalYears} Years {totalMonths} Months {remainingDays} Days";
            }
        }
        public string HealthFacility { get; set; }
        public string WorkingHealthFacility { get; set; }

        public string Designation { get; set; }
        public string WorkingDesignation { get; set; }
        public DateTime? DateOfFirstAppointment { get; set; }
    }
}
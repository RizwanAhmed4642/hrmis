using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.ViewModels
{
    public class ApplicationProfileViewModel
    {
        public int Profile_Id { get; set; }
        public string EmployeeName { get; set; }
        public string FatherName { get; set; }
        public string CNIC { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string MobileNo { get; set; }
        public string EMaiL { get; set; }
        public Nullable<int> Department_Id { get; set; }
        public Nullable<int> Designation_Id { get; set; }
        public Nullable<int> JoiningGradeBPS { get; set; }
        public Nullable<int> EmpMode_Id { get; set; }
        public Nullable<int> EmpStatus_Id { get; set; }
        public string HfmisCode { get; set; }
        public string HealthFacility { get; set; }
        public string Designation_Name { get; set; }
        public string StatusName { get; set; }
        public Nullable<int> HealthFacility_Id { get; set; }
        public Nullable<int> CurrentGradeBPS { get; set; }
        public string FileNumber { get; set; }
        public string Address { get; set; }
        public string SeniorityNo { get; set; }
        public Nullable<System.DateTime> JoiningDate { get; set; }
    }
}
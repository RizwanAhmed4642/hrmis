using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.ViewModels.Common
{
    public class ConsultantProfile
    {
        public string EmployeeName { get; set; }
        public string FatherName { get; set; }
        public int Domicile_Id { get; set; }
        public string CNIC { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int HF_Id { get; set; }
        public int designationId { get; set; }
        public int Status_Id { get; set; }
        public DateTime DateOfRegularization { get; set; }
        public string SeniorityNo { get; set; }
        public string PermanentAddress { get; set; }
        public string MobileNo { get; set; }
        public string EMaiL { get; set; }
    }
}



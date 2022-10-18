using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.ViewModels.Application
{
    public class ApplicationListViewModel
    {
        public int Id { get; set; }
        public Nullable<int> ApplicationSource_Id { get; set; }
        public Nullable<int> TrackingNumber { get; set; }
        public Nullable<int> ApplicationType_Id { get; set; }
        public Nullable<int> Status_Id { get; set; }
        public Nullable<int> ForwardingOfficer_Id { get; set; }
        public Nullable<int> PandSOfficer_Id { get; set; }
        public Nullable<bool> IsPending { get; set; }
        public Nullable<bool> FileRequested { get; set; }
        public Nullable<int> FileRequest_Id { get; set; }
        public Nullable<bool> IsPersonAppeared { get; set; }
        public Nullable<int> PersonAppeared_Id { get; set; }
        public string RawText { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<int> TotalDays { get; set; }
        public Nullable<int> LeaveType_Id { get; set; }
        public Nullable<int> CurrentScale { get; set; }
        public Nullable<int> RetirementType_Id { get; set; }
        public Nullable<int> FromHF_Id { get; set; }
        public string ToHFCode { get; set; }
        public Nullable<int> ToHF_Id { get; set; }
        public Nullable<int> FromDept_Id { get; set; }
        public Nullable<int> ToDept_Id { get; set; }
        public Nullable<int> FromDesignation_Id { get; set; }
        public Nullable<int> ToDesignation_Id { get; set; }
        public string SeniorityNumber { get; set; }
        public Nullable<System.DateTime> AdhocExpireDate { get; set; }
        public Nullable<int> ComplaintType_id { get; set; }
        public string Remarks { get; set; }
        public string EmployeeName { get; set; }
        public string FatherName { get; set; }
        public string CNIC { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string Gender { get; set; }
        public Nullable<int> Department_Id { get; set; }
        public Nullable<int> Designation_Id { get; set; }
        public Nullable<int> HealthFacility_Id { get; set; }
        public string HfmisCode { get; set; }
        public string MobileNo { get; set; }
        public string EMaiL { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public string Created_By { get; set; }
        public string Users_Id { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}
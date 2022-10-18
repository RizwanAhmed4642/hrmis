using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.ViewModels.OrderNotification
{
    public class OrderSaveModel
    {
        public int Id { get; set; }
        public Nullable<int> ApplicationSource_Id { get; set; }
        public Nullable<int> TrackingNumber { get; set; }
        public Nullable<int> ApplicationType_Id { get; set; }
        public Nullable<bool> IsSigned { get; set; }
        public Nullable<int> SignededAppAttachement_Id { get; set; }
        public Nullable<System.DateTime> RemarksTime { get; set; }
        public Nullable<int> Status_Id { get; set; }
        public Nullable<int> StatusByOfficer_Id { get; set; }
        public string StatusByOfficerName { get; set; }
        public Nullable<System.DateTime> StatusTime { get; set; }
        public Nullable<int> ForwardingOfficer_Id { get; set; }
        public Nullable<int> FromOfficer_Id { get; set; }
        public string FromOfficerName { get; set; }
        public Nullable<System.DateTime> ForwardTime { get; set; }
        public Nullable<int> PandSOfficer_Id { get; set; }
        public string PandSOfficerName { get; set; }
        public Nullable<bool> IsPending { get; set; }
        public Nullable<System.DateTime> RecieveTime { get; set; }
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
        public Nullable<int> ToScale { get; set; }
        public string SeniorityNumber { get; set; }
        public Nullable<System.DateTime> AdhocExpireDate { get; set; }
        public Nullable<int> ComplaintType_id { get; set; }
        public string DispatchNumber { get; set; }
        public Nullable<System.DateTime> DispatchDated { get; set; }
        public string DispatchFrom { get; set; }
        public string DispatchSubject { get; set; }
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
        public string FileNumber { get; set; }
        public string MobileNo { get; set; }
        public string EMaiL { get; set; }
        public Nullable<int> JoiningGradeBPS { get; set; }
        public Nullable<int> CurrentGradeBPS { get; set; }
        public Nullable<int> EmpMode_Id { get; set; }
        public Nullable<int> EmpStatus_Id { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public string Created_By { get; set; }
        public string Users_Id { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> Profile_Id { get; set; }
        public string ForwardingOfficerName { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<int> CurrentLog_Id { get; set; }
        public Nullable<System.DateTime> FileRequestTime { get; set; }
        public string FileRequestStatus { get; set; }
        public Nullable<int> FileRequestStatus_Id { get; set; }
        public Nullable<int> FileUpdated_Id { get; set; }
        public Nullable<int> DDS_Id { get; set; }

        public Nullable<bool> AppointmentEffect { get; set; }
        public Nullable<System.DateTime> AppointmentDate { get; set; }

    }
}
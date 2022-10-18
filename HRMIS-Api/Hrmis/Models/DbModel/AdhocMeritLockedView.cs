
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Hrmis.Models.DbModel
{

using System;
    using System.Collections.Generic;
    
public partial class AdhocMeritLockedView
{

    public int Id { get; set; }

    public Nullable<int> AdhocMerit_Id { get; set; }

    public Nullable<int> Applicant_Id { get; set; }

    public Nullable<int> Application_Id { get; set; }

    public Nullable<int> BatchApplication_Id { get; set; }

    public string DistrictCode { get; set; }

    public Nullable<bool> DistrictSelected { get; set; }

    public Nullable<bool> DistrictCodeAllotted { get; set; }

    public Nullable<int> MeritNumber { get; set; }

    public Nullable<int> MeritNumberChanging { get; set; }

    public Nullable<int> MatricMarks_Id { get; set; }

    public Nullable<int> MatricTotal { get; set; }

    public Nullable<int> MatricObtained { get; set; }

    public Nullable<double> MatricPercent { get; set; }

    public Nullable<int> Matriculation { get; set; }

    public Nullable<int> InterMarks_Id { get; set; }

    public Nullable<int> InterTotal { get; set; }

    public Nullable<int> InterObtained { get; set; }

    public Nullable<double> InterPercent { get; set; }

    public Nullable<int> Intermediate { get; set; }

    public Nullable<int> MasterMarks_Id { get; set; }

    public Nullable<int> MasterTotal { get; set; }

    public Nullable<int> MasterObtained { get; set; }

    public Nullable<double> MasterPercent { get; set; }

    public Nullable<int> Master { get; set; }

    public Nullable<int> FirstHigher { get; set; }

    public Nullable<int> FirstHigherMarks_Id { get; set; }

    public Nullable<int> SecondHigher { get; set; }

    public Nullable<int> SecondHigherMarks_Id { get; set; }

    public Nullable<int> ThirdHigher { get; set; }

    public Nullable<int> ThirdHigherMarks_Id { get; set; }

    public Nullable<int> FirstPosition { get; set; }

    public Nullable<int> FirstPositionMarks_Id { get; set; }

    public Nullable<int> SecondPosition { get; set; }

    public Nullable<int> SecondPositionMarks_Id { get; set; }

    public Nullable<int> ThirdPosition { get; set; }

    public Nullable<int> ThirdPositionMarks_Id { get; set; }

    public Nullable<double> OneYearExp { get; set; }

    public Nullable<double> TwoYearExp { get; set; }

    public Nullable<double> ThreeYearExp { get; set; }

    public Nullable<double> FourYearExp { get; set; }

    public Nullable<double> FivePlusYearExp { get; set; }

    public Nullable<double> ExperienceMarks_Id { get; set; }

    public Nullable<int> Hafiz { get; set; }

    public Nullable<int> HafizMarks_Id { get; set; }

    public Nullable<double> Interview { get; set; }

    public Nullable<double> Total { get; set; }

    public string WhyAboveReason { get; set; }

    public string WhyBelowReason { get; set; }

    public Nullable<bool> IsLocked { get; set; }

    public Nullable<bool> IsValid { get; set; }

    public Nullable<int> TotalApplicants { get; set; }

    public string Remarks { get; set; }

    public Nullable<bool> Selected { get; set; }

    public Nullable<int> VacantSeats { get; set; }

    public Nullable<System.DateTime> VacantSeatsAsOn { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> CreatedDate { get; set; }

    public string UserId { get; set; }

    public string CreatedBy { get; set; }

    public Nullable<bool> IsPosted { get; set; }

    public Nullable<int> Phase { get; set; }

    public Nullable<System.DateTime> PostingDateTime { get; set; }

    public Nullable<int> Batch_Id { get; set; }

    public string Name { get; set; }

    public string FatherName { get; set; }

    public string MobileNumber { get; set; }

    public string CNIC { get; set; }

    public string CNICDoc { get; set; }

    public Nullable<bool> IsHafiz { get; set; }

    public string HifzDocument { get; set; }

    public Nullable<bool> HifzVerified { get; set; }

    public string DomicileName { get; set; }

    public Nullable<int> Designation_Id { get; set; }

    public string DesignationName { get; set; }

    public Nullable<System.DateTime> DOB { get; set; }

    public Nullable<int> TotalDays { get; set; }

    public Nullable<int> Days { get; set; }

    public Nullable<int> Months { get; set; }

    public Nullable<int> Years { get; set; }

    public string DistrictName { get; set; }

    public string PMDCDoc { get; set; }

    public string PMDCNumber { get; set; }

    public Nullable<System.DateTime> Datetime { get; set; }

    public string Venue { get; set; }

    public Nullable<bool> PositionHolder { get; set; }

    public Nullable<bool> BatchAppIsLocked { get; set; }

    public string LockedBy { get; set; }

    public string LockedByUserId { get; set; }

    public Nullable<System.DateTime> LockedDatetime { get; set; }

    public Nullable<int> Position { get; set; }

    public string PositionDoc { get; set; }

    public Nullable<int> PositionMarks { get; set; }

    public Nullable<int> InterviewId { get; set; }

    public Nullable<bool> IsPostedOnAdhoc { get; set; }

    public Nullable<System.DateTime> AdhocPostingDatetime { get; set; }

    public Nullable<System.DateTime> CurrentDate { get; set; }

}

}

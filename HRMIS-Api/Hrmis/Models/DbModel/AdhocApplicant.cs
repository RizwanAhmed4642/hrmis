
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
    
public partial class AdhocApplicant
{

    public int Id { get; set; }

    public Nullable<int> Status_Id { get; set; }

    public Nullable<int> ApplicationNumber { get; set; }

    public string CNIC { get; set; }

    public string Email { get; set; }

    public string Name { get; set; }

    public string FatherName { get; set; }

    public string MobileNumber { get; set; }

    public string Address { get; set; }

    public Nullable<int> Domicile_Id { get; set; }

    public Nullable<int> Religion_Id { get; set; }

    public Nullable<System.DateTime> DOB { get; set; }

    public string MaritalStatus { get; set; }

    public string MobileSec { get; set; }

    public string PMDCNumber { get; set; }

    public Nullable<bool> Experience { get; set; }

    public Nullable<bool> RelevantExperience { get; set; }

    public Nullable<bool> IsDisabled { get; set; }

    public Nullable<int> MatricMarks { get; set; }

    public Nullable<int> InterMarks { get; set; }

    public Nullable<int> GraduationMarks { get; set; }

    public Nullable<int> MasterMarks { get; set; }

    public Nullable<int> FirstHigherMarks { get; set; }

    public Nullable<int> SecondHigherMarks { get; set; }

    public Nullable<int> ThirdHigherMarks { get; set; }

    public Nullable<int> HifzMarks { get; set; }

    public Nullable<int> Position { get; set; }

    public Nullable<int> PositionMarks { get; set; }

    public Nullable<int> RelevantExpMarks { get; set; }

    public Nullable<System.DateTime> MarksDatetime { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> CreatedDate { get; set; }

    public string UserId { get; set; }

    public string CreatedBy { get; set; }

    public Nullable<System.DateTime> PMDCRegDate { get; set; }

    public Nullable<System.DateTime> PMDCValidUpto { get; set; }

    public Nullable<bool> Hafiz { get; set; }

    public string HifzDocumentOld { get; set; }

    public string HifzDocument { get; set; }

    public Nullable<int> TotalMarks { get; set; }

    public string PMDCDocOld { get; set; }

    public string PMDCDoc { get; set; }

    public Nullable<int> ExperienceMarks { get; set; }

    public Nullable<int> InterviewMarks { get; set; }

    public string Gender { get; set; }

    public string DomicileDocOld { get; set; }

    public string DomicileDoc { get; set; }

    public string CNICDoc { get; set; }

    public string ProfilePic { get; set; }

    public string PositionDoc { get; set; }

    public Nullable<bool> HifzVerified { get; set; }

    public string HifzVerifiedUserId { get; set; }

    public string HifzVerifiedBy { get; set; }

    public Nullable<System.DateTime> HifzVerifiedDatetime { get; set; }

    public Nullable<bool> PositionVerified { get; set; }

    public string PositionVerifiedUserId { get; set; }

    public string PositionVerifiedBy { get; set; }

    public Nullable<System.DateTime> PositionVerifiedDatetime { get; set; }

    public Nullable<bool> AgeVerified { get; set; }

}

}

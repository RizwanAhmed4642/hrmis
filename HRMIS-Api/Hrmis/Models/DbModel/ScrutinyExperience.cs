
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
    
public partial class ScrutinyExperience
{

    public int Id { get; set; }

    public Nullable<int> ApplicantId { get; set; }

    public Nullable<int> ApplicationId { get; set; }

    public Nullable<int> ExperienceId { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> Datetime { get; set; }

    public string UserId { get; set; }

    public string Username { get; set; }

}

}
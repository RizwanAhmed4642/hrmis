
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
    
public partial class ApplicationMutualCode
{

    public int Id { get; set; }

    public Nullable<int> FirstProfile_Id { get; set; }

    public Nullable<int> SecondProfile_Id { get; set; }

    public Nullable<int> FirstCode { get; set; }

    public Nullable<int> SecondCode { get; set; }

    public Nullable<System.DateTime> FirstCodeTime { get; set; }

    public Nullable<System.DateTime> SecondCodeTime { get; set; }

    public Nullable<bool> Verified { get; set; }

    public Nullable<System.DateTime> VerifyTime { get; set; }

    public Nullable<bool> SecondVerified { get; set; }

    public Nullable<System.DateTime> SecondVerifyTime { get; set; }

    public Nullable<bool> IsActive { get; set; }

}

}


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
    
public partial class NotificationLog
{

    public int Id { get; set; }

    public Nullable<int> TypeId { get; set; }

    public Nullable<int> AlertNumber { get; set; }

    public Nullable<int> ProfileId { get; set; }

    public string PhoneNumber { get; set; }

    public Nullable<int> DayAfter { get; set; }

    public Nullable<System.DateTime> Datetime { get; set; }

    public Nullable<int> DayBefore { get; set; }

    public string Message { get; set; }

}

}

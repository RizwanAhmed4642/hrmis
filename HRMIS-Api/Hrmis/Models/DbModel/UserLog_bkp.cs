
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
    
public partial class UserLog_bkp
{

    public int ID { get; set; }

    public Nullable<int> MachineNumber { get; set; }

    public Nullable<int> IndRegID { get; set; }

    public Nullable<System.DateTime> DateTimeRecord { get; set; }

    public Nullable<System.DateTime> DateOnlyRecord { get; set; }

    public Nullable<System.DateTime> TimeOnlyRecord { get; set; }

    public string LogStatus { get; set; }

    public string Time_In { get; set; }

    public string Time_Out { get; set; }

    public Nullable<int> Seq_No { get; set; }

    public Nullable<int> IsLate { get; set; }

    public string EmpType { get; set; }

    public string Remarks { get; set; }

}

}

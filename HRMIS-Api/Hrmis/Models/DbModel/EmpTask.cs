
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
    
public partial class EmpTask
{

    public int Task_Id { get; set; }

    public Nullable<System.DateTime> Task_Date { get; set; }

    public string Task_title { get; set; }

    public string Task_Detail { get; set; }

    public Nullable<int> TaskStatus_Id { get; set; }

    public Nullable<System.DateTime> Date_Updated { get; set; }

    public Nullable<int> Project_Id { get; set; }

    public string ProfileID { get; set; }

    public string Entered_By { get; set; }

    public Nullable<System.DateTime> Entered_DateTime { get; set; }

    public string Update_By { get; set; }

    public Nullable<System.DateTime> Updated_DateTime { get; set; }

}

}


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
    
public partial class HrFile
{

    public long Id { get; set; }

    public Nullable<int> HrProfileId { get; set; }

    public Nullable<int> FileType_Id { get; set; }

    public string CNIC { get; set; }

    public string FileNo { get; set; }

    public Nullable<int> TotalPages { get; set; }

    public string DeptName { get; set; }

    public string Room { get; set; }

    public string Rack { get; set; }

    public string Row { get; set; }

    public Nullable<System.DateTime> Created_Date { get; set; }

    public string Created_By { get; set; }

    public string User_Id { get; set; }

    public Nullable<bool> IsActive { get; set; }

}

}

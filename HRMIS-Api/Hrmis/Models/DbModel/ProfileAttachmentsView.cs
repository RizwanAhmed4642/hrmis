
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
    
public partial class ProfileAttachmentsView
{

    public int Id { get; set; }

    public Nullable<long> Profile_Id { get; set; }

    public string Number { get; set; }

    public string DocumentTitle { get; set; }

    public string FilePath { get; set; }

    public Nullable<long> EntityLifecycle_Id { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public string Created_By { get; set; }

    public string Users_Id { get; set; }

}

}

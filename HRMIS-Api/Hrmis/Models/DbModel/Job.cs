
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
    
public partial class Job
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Job()
    {

        this.JobApplications = new HashSet<JobApplication>();

        this.JobHFs = new HashSet<JobHF>();

        this.JobPostings = new HashSet<JobPosting>();

    }


    public int Id { get; set; }

    public string Title { get; set; }

    public Nullable<int> Designation_Id { get; set; }

    public Nullable<int> SeatsOpen { get; set; }

    public Nullable<System.DateTime> StartDate { get; set; }

    public Nullable<System.DateTime> EndDate { get; set; }

    public string OfferLetter { get; set; }

    public string AcceptanceLetter { get; set; }

    public Nullable<int> Experience { get; set; }

    public Nullable<int> RelevantExperience { get; set; }

    public Nullable<int> AgeLimit { get; set; }

    public Nullable<int> TotalPreferences { get; set; }

    public Nullable<int> OrderBy { get; set; }

    public Nullable<bool> IsActive { get; set; }

    public Nullable<System.DateTime> CreatedDate { get; set; }

    public string UserId { get; set; }

    public string CreatedBy { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<JobApplication> JobApplications { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<JobHF> JobHFs { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<JobPosting> JobPostings { get; set; }

}

}

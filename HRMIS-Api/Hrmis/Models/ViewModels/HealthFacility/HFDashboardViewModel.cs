using Hrmis.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.ViewModels.HealthFacility
{
    public class HFDashboardViewModel
    {
        public int Id { get; set; }
        public string HFMISCode { get; set; }
        public string FullName { get; set; }
        public HFPhoto HFPhoto { get; set; }
        public int HFAC { get; set; }
        public string HFCategoryName { get; set; }
        public string ImagePath { get; set; }
        public string CategoryCode { get; set; }
        public string HFTypeName { get; set; }
        public string HFTypeCode { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public Nullable<double> CoveredArea { get; set; }
        public Nullable<double> UnCoveredArea { get; set; }
        public Nullable<double> ResidentialArea { get; set; }
        public Nullable<double> NonResidentialArea { get; set; }
        public string NA { get; set; }
        public string PP { get; set; }
        public string Mauza { get; set; }
        public string UcName { get; set; }
        public string UcNo { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }

        public string Name { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public string TehsilCode { get; set; }
        public string TehsilName { get; set; }
        public Nullable<int> OrderBy { get; set; }
        public Nullable<long> Entity_Lifecycle_Id { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> Created_Date { get; set; }
        public string Created_By { get; set; }
        public string Last_Modified_By { get; set; }
        public string Users_Id { get; set; }
        public string HfmisOldCode { get; set; }

        public HFVacancy HFVacancyCount { get; set; }
        public HFUCMView HFUCMView { get; set; }
        public List<HFPhoto> HFPhotos { get; set; }
        public List<HFWard> HFWardsList { get; set; }
        public List<HFServicesViewModel> HFServicesList { get; set; }
        public ProfileDetailsView HeadOfDepartment { get; set; }
        public List<ProfileDetailsView> Heads { get; set; }
        public List<ProfileDetailsView> EmoloyeeProfiles { get; set; }
        public List<VpMasterProfileView> Vacancies { get; set; }

    }
    public class HFServicesViewModel
    {
        public string Name { get; set; }
        public string HfmisCode { get; set; }
        public int HF_Id { get; set; }
        public int Services_Id { get; set; }
    }
    public class HFEmp_Profile
    {
        public string EmployeeName { get; set; }
        public string FatherName { get; set; }
        public string CNIC { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string WDesignation_Name { get; set; }
        public string MobileNo { get; set; }
        public string MobileNoOfficial { get; set; }
        public string EMaiL { get; set; }
    }
    public class HFEMpProfile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public List<ProfileDetailsView> Profiles { get; set; }
    }
    public class HFVacancy
    {
        public int? Working { get { return Working; } set { Working = 0; } }
        public int? Sanctioned { get { return Sanctioned; } set { Sanctioned = 0; } }
    }
}
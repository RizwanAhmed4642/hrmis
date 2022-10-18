using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hrmis.Models.Services
{
    public class RootService
    {
        private readonly UserService _userService;
        private C_User currentUser;
        private HealthFacilityService _healthFacilityService;
        public List<SearchResult> searchResults;
        public RootService()
        {
            _userService = new UserService();
            _healthFacilityService = new HealthFacilityService();
            searchResults = new List<SearchResult>();
        }
        public List<SearchResult> Search(string query, string userId, string roleName, string userName)
        {
            currentUser = _userService.GetUser(userId);
            SearchProfile(query, roleName);
            SearchHealthFacility(query, roleName);
            if (roleName.Equals("Deputy Secretary") || roleName.Equals("Hisdu Order Team") || roleName.Equals("Chief Executive Officer") || roleName.Equals("Districts") || roleName.Equals("Administrator"))// new role "Districts" added by Adnan 17/10/2022
            {
                if (roleName.Equals("Deputy Secretary") && userName.Equals("ordercell"))
                {
                    SearchOrder(query);
                }
                else
                {
                    SearchOrder(query);
                }
            }
            return searchResults;
        }
        public void SearchProfile(string query, string roleName)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    IQueryable<ProfileListView> queryDb = _db.ProfileListViews.AsQueryable();
                    if (roleName == "PHFMC Admin" || roleName.Equals("PHFMC"))
                    {
                        var hfIds = _db.HealthFacilities.Where(x => x.HFAC == 2).Select(k => k.Id).ToList();
                        queryDb = queryDb.Where(x => hfIds.Contains((int)x.HealthFacility_Id)).AsQueryable();
                    }
                    if (roleName == "PACP")
                    {
                        var hfIds = _db.HFListPs.Where(x => x.HFTypeCode.Equals("063")).Select(k => k.Id).ToList();
                        queryDb = queryDb.Where(x => hfIds.Contains((int)x.HealthFacility_Id)).AsQueryable();
                    }
                    if (roleName == "South Punjab")
                    {
                        queryDb = queryDb.Where(x => Common.Common.southDistrictNames.Contains(x.District)).AsQueryable();
                    }
                    var profiles = queryDb.Where(x => x.HfmisCode.StartsWith(currentUser.hfmiscode) || x.WorkingHFMISCode.StartsWith(currentUser.hfmiscode) || x.AddToEmployeePool == true).AsQueryable();
                    if (IsCNIC(query))
                    {
                        query = query.Replace("-", "");
                        var profile = profiles.FirstOrDefault(x => x.CNIC.Equals(query));
                        if (profile != null)
                        {
                            searchResults.Add(new SearchResult()
                            {
                                Id = profile.Id,
                                Name = profile.EmployeeName + " (" + profile.WDesignation_Name + " - " + profile.CurrentGradeBPS + ")",
                                PhotoPath = @"https://hrmis.pshealth.punjab.gov.pk/Uploads/ProfilePhotos/" + profile.CNIC?.ToString() + "_23.jpg",
                                CNIC = profile.CNIC,
                                ResultType = "Profile",
                                Type = 2
                            });
                        }
                    }
                    else
                    {
                        searchResults = profiles.Where(x => x.EmployeeName.ToLower().Contains(query.ToLower()))
                            .OrderBy(x => x.EmployeeName).Select(p => new SearchResult
                            {
                                Id = p.Id,
                                Name = p.EmployeeName + " (" + p.WDesignation_Name + " - " + p.CurrentGradeBPS + ")",
                                PhotoPath = @"https://hrmis.pshealth.punjab.gov.pk/Uploads/ProfilePhotos/" + p.CNIC + "_23.jpg",
                                CNIC = p.CNIC,
                                ResultType = "Profile",
                                Type = 2
                            }).ToList();
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public void SearchHealthFacility(string query, string roleName)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    query = query.ToLower();

                    if (query.Equals("dhq") || query.Contains("dhq")) query = query.Replace("dhq", "District Headquarter Hospital");
                    else if (query.Equals("thq") || query.Contains("thq")) query = query.Replace("thq", "Tehsil Headquarter Hospital");
                    else if (query.Equals("rhc") || query.Contains("rhc")) query = query.Replace("rhc", "Rural Health Center");
                    else if (query.Equals("bhu") || query.Contains("bhu")) query = query.Replace("bhu", "Basic Health Unit");
                    else if (query.Equals("grd") || query.Contains("grd")) query = query.Replace("grd", "Government Rural Dispensary");

                    var queryDb = _db.HFListPs.Where(x => x.IsActive == true).AsQueryable();

                    if (roleName.Equals("PHFMC Admin") || roleName.Equals("PHFMC"))
                    {
                        queryDb = queryDb.Where(x => x.HFAC == 2).AsQueryable();
                    }
                    if (roleName.Equals("PACP"))
                    {
                        queryDb = queryDb.Where(x => x.HFTypeCode.Equals("063")).AsQueryable();
                    }
                    if (roleName.Equals("South Punjab"))
                    {
                        queryDb = queryDb.Where(x => Common.Common.southDistricts.Contains(x.DistrictCode)).AsQueryable();
                    }
                    queryDb = queryDb.Where(x => (x.FullName.StartsWith(query) || x.FullName.Contains(query)) &&
                        (x.HFMISCode.Equals(currentUser.hfmiscode) || x.HFMISCode.StartsWith(currentUser.hfmiscode))).AsQueryable();

                    var hfs = queryDb.Select(p => new SearchResult
                    {
                        Id = p.Id,
                        PhotoPath = @"https://hrmis.pshealth.punjab.gov.pk/Uploads/Files/HFPics/" + p.ImagePath,
                        Name = p.FullName,
                        HFMISCode = p.HFMISCode,
                        ResultType = "Health Facility",
                        Type = 1
                    }).ToList();
                    searchResults.AddRange(hfs);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public void SearchOrder(string query)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    if (query.StartsWith("ESR-"))
                    {
                        query = query.Replace("ESR-", "");
                        int barcode = Convert.ToInt32(query);
                        var esrs = _db.ESRReportViews.Where(x => (x.ESRId == barcode) && x.IsActive == true)
                       .Select(p => new SearchResult
                       {
                           Id = (int)p.ESRId,
                           Name = p.EmployeeName + " (" + p.Designation + " - " + p.Scale + ")",
                           CNIC = p.CNIC,
                           ResultType = p.OrderType,
                           Type = 3
                       }).ToList();
                        searchResults.AddRange(esrs);
                    }
                    else if (query.StartsWith("ELR-"))
                    {
                        query = query.Replace("ELR-", "");
                        int barcode = Convert.ToInt32(query) - 1003;
                        var elrs = _db.LeaveOrderViews.Where(x => (x.Id == barcode) && x.IsActive == true)
                       .Select(p => new SearchResult
                       {
                           Id = p.Id,
                           Name = p.EmployeeName + " (" + p.Designation + " - " + p.Scale + ")",
                           CNIC = p.CNIC,
                           ResultType = "Leave Order",
                           Type = 3
                       }).ToList();
                        searchResults.AddRange(elrs);
                    }

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<HFList> SearchHealthFacilities(string query, string userId)
        {
            currentUser = _userService.GetUser(userId);
            return SearchHealthFacilityOnly(query);
        }
        public List<HFList> SearchHealthFacilityOnly(string query)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    query = query.ToLower();

                    if (query.Equals("dhq") || query.Contains("dhq")) query = query.Replace("dhq", "District Headquarter Hospital");
                    else if (query.Equals("thq") || query.Contains("thq")) query = query.Replace("thq", "Tehsil Headquarter Hospital");
                    else if (query.Equals("rhc") || query.Contains("rhc")) query = query.Replace("rhc", "Rural Health Center");
                    else if (query.Equals("bhu") || query.Contains("bhu")) query = query.Replace("bhu", "Basic Health Unit");
                    else if (query.Equals("grd") || query.Contains("grd")) query = query.Replace("grd", "Government Rural Dispensary");

                    var hfs = _db.HFLists.Where(x => (x.HFMISCode.Equals(query) || x.FullName.StartsWith(query) || x.FullName.Contains(query)) &&
                        (x.HFMISCode.Equals(currentUser.hfmiscode) || x.HFMISCode.StartsWith(currentUser.hfmiscode)) && x.IsActive == true)
                        .ToList();
                    return hfs;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<HFList> SearchHealthFacilitiesAll(string query)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    query = query.ToLower();

                    if (query.Equals("dhq") || query.Contains("dhq")) query = query.Replace("dhq", "District Headquarter Hospital");
                    else if (query.Equals("thq") || query.Contains("thq")) query = query.Replace("thq", "Tehsil Headquarter Hospital");
                    else if (query.Equals("rhc") || query.Contains("rhc")) query = query.Replace("rhc", "Rural Health Center");
                    else if (query.Equals("bhu") || query.Contains("bhu")) query = query.Replace("bhu", "Basic Health Unit");
                    else if (query.Equals("grd") || query.Contains("grd")) query = query.Replace("grd", "Government Rural Dispensary");

                    var hfs = _db.HFLists.Where(x => (x.HFMISCode.Equals(query) || x.FullName.StartsWith(query) || x.FullName.Contains(query)) && x.IsActive == true)
                        .ToList();
                    return hfs;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<VpMastProfileView> SearchVacancy(string query, int designationId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    query = query.ToLower();

                    if (query.Equals("dhq") || query.Contains("dhq")) query = query.Replace("dhq", "District Headquarter Hospital");
                    else if (query.Equals("thq") || query.Contains("thq")) query = query.Replace("thq", "Tehsil Headquarter Hospital");
                    else if (query.Equals("rhc") || query.Contains("rhc")) query = query.Replace("rhc", "Rural Health Center");
                    else if (query.Equals("bhu") || query.Contains("bhu")) query = query.Replace("bhu", "Basic Health Unit");
                    else if (query.Equals("grd") || query.Contains("grd")) query = query.Replace("grd", "Government Rural Dispensary");

                    var hfs = _db.VpMastProfileViews.Where(x => (x.HFName.StartsWith(query) || x.HFName.Contains(query)) && (x.Desg_Id == designationId || x.Desg_Id == 2404) && !x.DistrictCode.Equals("035002") && x.HFAC == 1)
                        .OrderBy(k => k.Vacant).OrderByDescending(l => l.TotalSanctioned).ToList();
                    return hfs;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public Entity_Log SaveEntityLog(Entity_Log entity_Log, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    entity_Log.IsActive = true;
                    entity_Log.UserId = userId;
                    entity_Log.Username = userName;
                    entity_Log.Datetime = DateTime.UtcNow.AddHours(5);
                    _db.Entity_Log.Add(entity_Log);
                    _db.SaveChanges();
                    return entity_Log;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        
        public MobileApp GetMobileAppVersion()
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var data = _db.MobileApps.FirstOrDefault();
                    return data;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public TableResponse<ProfileListView> GetEmployeeProfiles(long imei)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 2;
                    var dR = _db.DeviceRegistrations.FirstOrDefault(x => x.IMEI == imei);
                    if (dR == null) return null;
                    var hf = _db.HealthFacilities.FirstOrDefault(x => x.Id == dR.HF_Id);
                    if (hf == null) return null;
                    IQueryable<ProfileListView> query = _db.ProfileListViews.Where(x => x.Status_Id != 16).AsQueryable();

                    if (hf.HfmisCode != null)
                    {
                        query = query.Where(x => x.HfmisCode.StartsWith(hf.HfmisCode)).AsQueryable();
                    }
                    var list = query.OrderBy(x => Guid.NewGuid()).ThenByDescending(x => x.Designation_HrScale_Id).ThenBy(x => x.EmployeeName).ThenBy(x => x.WDesignation_Name).ToList();
                    return new TableResponse<ProfileListView> { List = list };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool GetProfileExist(string cnic)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var data = _db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(cnic));
                    if (data != null) return true;
                    return false;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public bool GetProfileExist(int id, string cnic, string mobileNo, string email)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var data = _db.HrProfiles.FirstOrDefault(x => x.Id == id && x.CNIC.Equals(cnic) && x.MobileNo.Equals(mobileNo) && x.EMaiL.Equals(email) && !x.EMaiL.Equals("abc@gmail.com"));
                    if (data != null) return true;
                    return false;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public bool GetProfileExistDesignation(string cnic, List<int?> designationIds)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    //var data = _db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(cnic) && designationIds.Contains(x.Designation_Id));
                    var data = _db.HrProfiles.FirstOrDefault(x => x.CNIC.Equals(cnic));
                    if (data != null) return true;
                    return false;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public ProfileApplicantView GetProfileApplicant(string cnic, int id)
        {
            using (var _db = new HR_System())
            {
                _db.Configuration.ProxyCreationEnabled = false;
                var profile = _db.ProfileDetailsViews.FirstOrDefault(x => x.CNIC.Equals(cnic) && id == x.Id);
                if (profile != null)
                {
                    var hf = _db.HFListPs.FirstOrDefault(x => x.Id == profile.HealthFacility_Id && x.IsActive == true);
                    var applications = _db.ApplicationViews.Where(x => x.CNIC.Equals(cnic) && id == x.Profile_Id && x.IsActive == true).ToList();
                    return new ProfileApplicantView { profile = profile, hf = hf, applications = applications };
                }
                return new ProfileApplicantView { profile = profile };
            }
        }
        public ProfileDetailsView GetProfileByCNIC(string cnic)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var data = _db.ProfileDetailsViews.FirstOrDefault(x => x.CNIC.Equals(cnic));
                    return data;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public PromotionApplyDto GetPromotionApplyData(string cnic)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var profile = _db.ProfileDetailsViews.FirstOrDefault(x => x.CNIC.Equals(cnic));
                    var promotionApply = _db.PromotionApplies.FirstOrDefault(x => x.Profile_Id == profile.Id);
                    return new PromotionApplyDto { Profile = profile, PromotionApply = promotionApply };
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public TableResponse<PromotionApplyView> GetPromotionApply(Filter filter)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var query = _db.PromotionApplyViews.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.Query))
                    {
                        query = query.Where(x => x.EmployeeName.Contains(filter.Query) || x.CNIC.Equals(filter.Query));
                    }
                    var count = query.Count();
                    var list = query.OrderBy(x => x.SeniorityNo).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    var result = new TableResponse<PromotionApplyView> { List = list, Count = count };
                    return result;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public OrderDesignationResponse GetOrderDesignations(int hf_Id, string hfmisCode, int Designation_Id, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    OrderDesignationResponse orderDesignationResponse = new OrderDesignationResponse();
                    _db.Configuration.ProxyCreationEnabled = false;
                    orderDesignationResponse.vpMaster = _db.VpMasterProfileViews.FirstOrDefault(x => x.HF_Id == hf_Id && x.HFMISCode.Equals(hfmisCode) && x.Desg_Id == Designation_Id);
                    //var vpDetails = _db.VpMProfileViews.Where(x => x.HF_Id == hf_Id && x.HFMISCode.Equals(hfmisCode)).ToList();

                    return orderDesignationResponse;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public CheckP VerifyProfileForTransfer(int Id, int resons_Id)
        {
            List<CheckP> checks = new List<CheckP>();

            using (var db = new HR_System())
            {
                var profile = db.ProfileDetailsViews.FirstOrDefault(x => x.Id == Id);
                if (profile == null) return null;

                DateTime dateNow = DateTime.Now;

                if (resons_Id == 1)
                {
                    #region OneYear
                    CheckP checkP = new CheckP();
                    if (profile.PresentPostingDate < dateNow.AddYears(-1))
                    {
                        if (profile.PresentPostingDate < dateNow.AddYears(-3))
                        {
                            checkP.Status = CheckPStatus.VerifiedForLahore;
                        }
                        else
                        {
                            checkP.Status = CheckPStatus.Verified;
                        }
                    }
                    else
                    {
                        checkP.Status = CheckPStatus.NotEligible;
                    }
                    if (profile.HealthFacility_Id != null)
                    {
                        var hf = db.HFListPs.FirstOrDefault(x => x.Id == profile.HealthFacility_Id && x.IsActive == true);
                        if (hf != null)
                        {
                            if (hf.HFTypeCode.Equals("011") || hf.HFTypeCode.Equals("012"))
                            {
                                checkP.VacancyCheck = _healthFacilityService.HealthFacility_CalculateVacancy(profile.HealthFacility_Id, 0, profile.Designation_Id);
                            }
                        }
                    }
                    //if (profile.PresentPostingDate != profile.DateOfFirstAppointment)
                    //{
                    //    if (profile.PresentPostingDate < dateNow.AddYears(-5))
                    //    {
                    //        checkP.Status = CheckPStatus.Verified;
                    //    }
                    //    else
                    //    {
                    //        checkP.Status = CheckPStatus.NotEligible;
                    //        checkP.Description = "Initial Service must be more than 3 years";
                    //    }
                    //}
                    //else
                    //{
                    //    if (profile.PresentPostingDate < dateNow.AddYears(-3))
                    //    {
                    //        checkP.Status = CheckPStatus.Verified;
                    //    }
                    //    else
                    //    {
                    //        checkP.Status = CheckPStatus.NotEligible;
                    //    }
                    //}
                    #endregion
                    return checkP;
                }
                else if (resons_Id == 2)
                {
                    #region Disability

                    #endregion
                }
                else if (resons_Id == 3)
                {
                    #region SpouseDeath

                    #endregion
                }
                else if (resons_Id == 4)
                {
                    #region Medical

                    #endregion
                }
                else if (resons_Id == 5)
                {
                    #region WedLock

                    #endregion
                }
            }
            return null;
        }
        #region Dashboard
        //public DashboardTotalsViewModel GetDashboardTotalCounts()
        //{
        //    var applicationService = new ApplicationService();
        //    var 
        //}
        #endregion

        #region helpers
        public bool IsCNIC(string query)
        {
            query = query.Replace("-", "");
            if (query.Length == 13)
            {
                var isNumber = long.TryParse(query, out long cnic);
                return isNumber;
            }
            else
            {
                return false;
            }
        }
        public bool IsOrder(string query)
        {
            if (query.StartsWith("ESR-") || query.StartsWith("ELR-"))
            {
                var isNumber = int.TryParse(query, out int orderId);
                return isNumber;
            }
            else
            {
                return false;
            }
        }
        #endregion

    }

    public class PromotionApplyDto
    {
        public ProfileDetailsView Profile { get; set; }
        public PromotionApply PromotionApply { get; set; }
    }
    public class ProfileApplicantView
    {
        public ProfileDetailsView profile { get; set; }
        public HFListP hf { get; set; }
        public List<ApplicationView> applications { get; set; }
    }
    public class CheckP
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public CheckPStatus Status;
        public DateTime time;
        public FacilityVacancyPercentage VacancyCheck { get; set; }
    }
    public enum CheckPStatus
    {
        Checking,
        Verified,
        NotEligible,
        VerifiedForLahore,
        Completed,
        Error
    }
}
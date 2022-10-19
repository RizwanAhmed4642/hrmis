using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Hrmis.Models.Services
{
    public class DailyWagesService
    {
        string path = System.Web.Hosting.HostingEnvironment.MapPath("~/DailyWagerUploads");
        public DailyWagesProfile AddDailyWagerProfile(DailyWagesProfileClass dailywagesProfile, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {

                    var dbHrProfile = _db.DailyWagesProfiles.FirstOrDefault(x => x.CNIC.Equals(dailywagesProfile.CNIC));
                    try
                    {
                        if (dbHrProfile == null)
                        {
                            DailyWagesProfile dailyWages = new DailyWagesProfile();
                            var filePersonImage = $"Person_{Guid.NewGuid().ToString("N")}_{DateTime.UtcNow.AddHours(5).ToString("ddMMyyyyHHmmssffffff")}.jpg";

                            if (!string.IsNullOrEmpty(dailywagesProfile.PersonImage))
                            {
                                File.WriteAllBytes($"{path}/{filePersonImage}", Convert.FromBase64String(dailywagesProfile.PersonImage));
                                dailyWages.PersonImage = filePersonImage;
                            }
                            dailyWages.UserId = dailywagesProfile.UserId;
                            dailyWages.UserName = dailywagesProfile.UserName;
                            dailyWages.Category = dailywagesProfile.Category;
                            dailyWages.EmployementMode = dailywagesProfile.EmployementMode;
                            dailyWages.MobileNumber = dailywagesProfile.MobileNumber;
                            dailyWages.Gender = dailywagesProfile.Gender;
                            dailyWages.FatherName = dailywagesProfile.FatherName;
                            dailyWages.Name = dailywagesProfile.Name;
                            dailyWages.Address = dailywagesProfile.Address;
                            dailyWages.CreatedBy = userId;
                            dailyWages.CNIC = dailywagesProfile.CNIC;
                            dailyWages.CreatedDate = DateTime.UtcNow.AddHours(5);
                            dailyWages.Designation = dailywagesProfile.Designation;
                            dailyWages.RecordStatus = true;
                            if (dailywagesProfile.DistirctCode == null)
                            {
                                dailyWages.DistirctCode = dailywagesProfile.TehsilCode.Substring(0, 6);
                            }
                            else
                            {
                                dailyWages.DistirctCode = dailywagesProfile.DistirctCode;
                            }
                            if (dailywagesProfile.DivisionCode == null)
                            {
                                dailyWages.DivisionCode = dailywagesProfile.TehsilCode.Substring(0, 3);
                            }
                            else 
                            {
                                dailyWages.DivisionCode = dailywagesProfile.DivisionCode;
                            }
                            dailyWages.TehsilCode = dailywagesProfile.TehsilCode;
                            dailyWages.DateOfBirth = dailywagesProfile.DateOfBirth;

                            dailyWages.UcCode = dailywagesProfile.UcCode;


                            if (string.IsNullOrEmpty(dailywagesProfile.UcCode) && string.IsNullOrEmpty(dailywagesProfile.HfmisCode))
                            {

                                dailyWages.HfmisCode = dailywagesProfile.TehsilCode;
                            }


                            if (string.IsNullOrEmpty(dailywagesProfile.HfmisCode))
                            {
                                dailyWages.HfmisCode = dailywagesProfile.UcCode;
                            }
                            else
                            {
                                dailyWages.HfmisCode = dailywagesProfile.HfmisCode;
                            }
                            _db.DailyWagesProfiles.Add(dailyWages);
                            _db.SaveChanges();


                            if (dailywagesProfile.dailyWagesAccountDetail != null)
                            {
                                DailyWagerBankDetail bankDetail = new DailyWagerBankDetail();
                                bankDetail.BankId = dailywagesProfile.dailyWagesAccountDetail.BankId;
                                bankDetail.DailyWagerProfileId = dailyWages.Id;
                                bankDetail.AccountNumber = dailywagesProfile.dailyWagesAccountDetail.AccountNumber;
                                bankDetail.AccountTitle = dailywagesProfile.dailyWagesAccountDetail.AccountTitle;
                                _db.DailyWagerBankDetails.Add(bankDetail);
                                _db.SaveChanges();
                            }

                            if (dailywagesProfile.dailyWagesContractDetail != null)
                            {

                                DailyWagerContractDetail contractDetail = new DailyWagerContractDetail();
                                var fileContractImage = $"Contract_{Guid.NewGuid().ToString("N")}_{DateTime.UtcNow.AddHours(5).ToString("ddMMyyyyHHmmssffffff")}.jpg";
                                if (!string.IsNullOrEmpty(dailywagesProfile.dailyWagesContractDetail.ContractImagePath))
                                {
                                    File.WriteAllBytes($"{path}/{fileContractImage}", Convert.FromBase64String(dailywagesProfile.dailyWagesContractDetail.ContractImagePath));
                                    contractDetail.ContractImagePath = fileContractImage;
                                }


                                contractDetail.ContractStatus = dailywagesProfile.dailyWagesContractDetail.ContractStatus;
                                contractDetail.ContractStartDate = dailywagesProfile.dailyWagesContractDetail.ContractStartDate;
                                contractDetail.ContractEndDate = dailywagesProfile.dailyWagesContractDetail.ContractEndDate;
                                contractDetail.WagerProfileId = dailyWages.Id;
                                _db.DailyWagerContractDetails.Add(contractDetail);
                                _db.SaveChanges();
                            }

                            return dailyWages;

                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception EX)
                    {
                        throw EX;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public DailyWagesProfile UpdateDailyWagerById(DailyWagesProfileClass dailywagesProfile, string userName, string userId)
        {
                var _db = new HR_System();
                using (System.Data.Entity.DbContextTransaction transaction = _db.Database.BeginTransaction())
                {
                    //var dbHrProfile = _db.DailyWagesProfiles.FirstOrDefault(x => x.CNIC.Equals(dailywagesProfile.CNIC));
                    try{
                        transaction.Commit();
                        DailyWagesProfile dailyWages = _db.DailyWagesProfiles.FirstOrDefault(x => x.Id == dailywagesProfile.Id);
                        if (!string.IsNullOrEmpty(dailywagesProfile.PersonImage))
                        {
                            if (!dailywagesProfile.PersonImage.Contains("https://"))
                            {
                                var filePersonImage = $"Person_{Guid.NewGuid().ToString("N")}_{DateTime.UtcNow.AddHours(5).ToString("ddMMyyyyHHmmssffffff")}.jpg";
                                if (!string.IsNullOrEmpty(dailywagesProfile.PersonImage))
                                {
                                    File.WriteAllBytes($"{path}/{filePersonImage}", Convert.FromBase64String(dailywagesProfile.PersonImage));
                                    dailyWages.PersonImage = filePersonImage;
                                }
                            }
                            else
                            {
                                dailyWages.PersonImage = dailywagesProfile.PersonImage;
                            }
                        }
                    dailyWages.UserId = dailywagesProfile.UserId;
                    dailyWages.UserName = dailywagesProfile.UserName;
                    dailyWages.Gender = dailywagesProfile.Gender;
                        dailyWages.FatherName = dailywagesProfile.FatherName;
                        dailyWages.Name = dailywagesProfile.Name;
                        dailyWages.Address = dailywagesProfile.Address;
                    dailyWages.Category = dailywagesProfile.Category;
                    dailyWages.EmployementMode = dailywagesProfile.EmployementMode;
                    dailyWages.CreatedBy = userId;
                        dailyWages.CNIC = dailywagesProfile.CNIC;
                        dailyWages.CreatedDate = DateTime.UtcNow.AddHours(5);
                        dailyWages.Designation = dailywagesProfile.Designation;
                        dailyWages.DistirctCode = dailywagesProfile.DistirctCode;
                        dailyWages.DivisionCode = dailywagesProfile.DivisionCode;
                        dailyWages.TehsilCode = dailywagesProfile.TehsilCode;
                        dailyWages.DateOfBirth = dailywagesProfile.DateOfBirth;
                        dailyWages.UcCode = dailywagesProfile.UcCode;


                        if (string.IsNullOrEmpty(dailywagesProfile.UcCode) && string.IsNullOrEmpty(dailywagesProfile.HfmisCode))
                        {

                            dailyWages.HfmisCode = dailywagesProfile.TehsilCode;
                        }


                        if (string.IsNullOrEmpty(dailywagesProfile.HfmisCode))
                        {
                            dailyWages.HfmisCode = dailywagesProfile.UcCode;
                        }
                        else
                        {
                            dailyWages.HfmisCode = dailywagesProfile.HfmisCode;
                        }
                        _db.DailyWagesProfiles.Add(dailyWages);
                        _db.Entry(dailyWages).State = System.Data.Entity.EntityState.Modified;

                        _db.SaveChanges();

                        if (dailywagesProfile.dailyWagesAccountDetail != null)
                        {
                            DailyWagerBankDetail bankDetail = _db.DailyWagerBankDetails.FirstOrDefault(x => x.Id == dailywagesProfile.dailyWagesAccountDetail.Id);
                        if (bankDetail != null)
                        {
                            bankDetail.Id = dailywagesProfile.dailyWagesAccountDetail.Id;
                            bankDetail.BankId = dailywagesProfile.dailyWagesAccountDetail.BankId;
                            bankDetail.DailyWagerProfileId = dailyWages.Id;
                            bankDetail.AccountNumber = dailywagesProfile.dailyWagesAccountDetail.AccountNumber;
                            bankDetail.AccountTitle = dailywagesProfile.dailyWagesAccountDetail.AccountTitle;
                            _db.DailyWagerBankDetails.Add(bankDetail);
                            _db.Entry(bankDetail).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }
                        else
                        {
                            if (dailywagesProfile.dailyWagesAccountDetail != null)
                            {
                                DailyWagerBankDetail bankDetailObj = new DailyWagerBankDetail();
                                bankDetailObj.DailyWagerProfileId = dailyWages.Id;
                                bankDetailObj.AccountNumber = dailywagesProfile.dailyWagesAccountDetail.AccountNumber;
                                bankDetailObj.AccountTitle = dailywagesProfile.dailyWagesAccountDetail.AccountTitle;
                                _db.DailyWagerBankDetails.Add(bankDetailObj);
                                _db.SaveChanges();
                            }
                        }
                        }

                        if (dailywagesProfile.dailyWagesContractDetail != null)
                        {
                        DailyWagerContractDetail contractDetail = _db.DailyWagerContractDetails.FirstOrDefault(x => x.Id == dailywagesProfile.dailyWagesContractDetail.Id);
                        if (contractDetail != null)
                        {
                            if (!string.IsNullOrEmpty(dailywagesProfile.dailyWagesContractDetail.ContractImagePath))
                            {
                                if (!dailywagesProfile.dailyWagesContractDetail.ContractImagePath.Contains("https://"))
                                {
                                    var fileContractImage = $"Contract_{Guid.NewGuid().ToString("N")}_{DateTime.UtcNow.AddHours(5).ToString("ddMMyyyyHHmmssffffff")}.jpg";
                                    if (!string.IsNullOrEmpty(dailywagesProfile.dailyWagesContractDetail.ContractImagePath))
                                    {
                                        File.WriteAllBytes($"{path}/{fileContractImage}", Convert.FromBase64String(dailywagesProfile.dailyWagesContractDetail.ContractImagePath));
                                        contractDetail.ContractImagePath = fileContractImage;
                                    }
                                }
                                else
                                {
                                    contractDetail.ContractImagePath = dailywagesProfile.dailyWagesContractDetail.ContractImagePath;
                                }
                            }
                            contractDetail.Id = dailywagesProfile.dailyWagesContractDetail.Id;
                            contractDetail.ContractStatus = dailywagesProfile.dailyWagesContractDetail.ContractStatus;
                            contractDetail.ContractStartDate = dailywagesProfile.dailyWagesContractDetail.ContractStartDate;
                            contractDetail.ContractEndDate = dailywagesProfile.dailyWagesContractDetail.ContractEndDate;
                            contractDetail.WagerProfileId = dailyWages.Id;
                            _db.DailyWagerContractDetails.Add(contractDetail);
                            _db.Entry(contractDetail).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }
                        else
                        {
                            if (dailywagesProfile.dailyWagesContractDetail != null)
                            {

                                DailyWagerContractDetail contractDetailObj = new DailyWagerContractDetail();
                                var fileContractImage = $"Contract_{Guid.NewGuid().ToString("N")}_{DateTime.UtcNow.AddHours(5).ToString("ddMMyyyyHHmmssffffff")}.jpg";
                                if (!string.IsNullOrEmpty(dailywagesProfile.dailyWagesContractDetail.ContractImagePath))
                                {
                                    File.WriteAllBytes($"{path}/{fileContractImage}", Convert.FromBase64String(dailywagesProfile.dailyWagesContractDetail.ContractImagePath));
                                    contractDetailObj.ContractImagePath = fileContractImage;
                                }
                                contractDetailObj.ContractStatus = dailywagesProfile.dailyWagesContractDetail.ContractStatus;
                                contractDetailObj.ContractStartDate = dailywagesProfile.dailyWagesContractDetail.ContractStartDate;
                                contractDetailObj.ContractEndDate = dailywagesProfile.dailyWagesContractDetail.ContractEndDate;
                                contractDetailObj.WagerProfileId = dailyWages.Id;
                                _db.DailyWagerContractDetails.Add(contractDetailObj);
                                _db.SaveChanges();
                            }
                        }
                       }

                        return dailyWages;
                    }
                    catch (Exception EX)
                    {
                    transaction.Rollback();
                    throw EX;
                    }
                }
           
        }

        public TableResponse<DailyWagesProfile> GetDailyWagesInPool(ProfileFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 2;
                    IQueryable<DailyWagesProfile> query = _db.DailyWagesProfiles.AsQueryable(); ;
                    if (filters.hfmisCode != null)
                    {
                        query = query.Where(x => x.HfmisCode.StartsWith(filters.hfmisCode)).AsQueryable();
                    }

                    if (filters.designations != null && filters.designations.Count > 0)
                    {
                        query = query.Where(x => filters.Designation.Contains(x.Designation)).AsQueryable();
                    }
                    if (filters.searchTerm != null && filters.searchTerm.Length >= 2)
                    {
                        if (new RootService().IsCNIC(filters.searchTerm))
                        {
                            filters.searchTerm = filters.searchTerm.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filters.searchTerm)).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Name.ToLower().Contains(filters.searchTerm.ToLower())).AsQueryable();
                        }
                    }
                    var count = query.Count();
                    var list = query.OrderBy(x => Guid.NewGuid()).ThenByDescending(x => x.Designation).ThenBy(x => x.Name).ThenBy(x => x.Designation).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    return new TableResponse<DailyWagesProfile> { List = list, Count = count };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public TableResponse<DailyWagerDetailView> GetDailyWages(ProfileFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 4;
                    IQueryable<DailyWagerDetailView> query = _db.DailyWagerDetailViews.AsQueryable();
                    //if (filters.roleName == "PHFMC Admin")
                    //{
                    //    var hfIds = _db.HealthFacilities.Where(x => x.HFAC == 2).Select(k => k.).ToList();
                    //    query = query.Where(x => hfIds.Contains(x.HfmisCode)).AsQueryable();
                    //}
                    //if (filters.roleName == "PACP")
                    //{
                    //    var hfIds = _db.HFListPs.Where(x => x.HFTypeCode.Equals("063")).Select(k => k.HFMISCode).ToList();
                    //    query = query.Where(x => hfIds.Contains(x.HfmisCode)).AsQueryable();
                    //}
                    //if (filters.roleName == "South Punjab")
                    //{
                    //    query = query.Where(x => Common.Common.phfmcDistrictCodes.Contains(x.DistirctCode)).AsQueryable();
                    //}
                    //if (filters.hfmisCode != null)
                    //{
                    //    query = query.Where(x => x.HfmisCode.StartsWith(filters.hfmisCode)).AsQueryable();
                    //}

                    if (filters.hfmisCode == "0" || filters.hfmisCode == null)
                    {
                        query = query.AsQueryable();
                    }
                    else if (filters.hfmisCode.Length == 3) //For Division
                    {
                        query = query.Where(x => x.DivisionCode == filters.hfmisCode).AsQueryable();
                    }
                    else if ((filters.hfmisCode.Length == 5 || filters.hfmisCode.Length == 6) && filters.hfmisCode.StartsWith("0")) //For District
                    {
                        query = query.Where(x => x.DistirctCode == filters.hfmisCode).AsQueryable();
                    }
                    else if (filters.hfmisCode.Length == 5) //For HFMIS or UC
                    {
                        query = query.Where(x => x.HfmisCode == filters.hfmisCode || x.UcCode == filters.hfmisCode).AsQueryable();
                    }
                    else if (filters.hfmisCode.Length == 9) //For Tehsil
                    {
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode).AsQueryable();
                    }
                    if (!string.IsNullOrEmpty(filters.Designation))
                    {
                        query = query.Where(x => x.Category == filters.Designation && x.Category != null && x.Category != "").AsQueryable();
                    }
                    if (!string.IsNullOrEmpty(filters.searchTerm) && filters.searchTerm.Length >= 2)
                    {
                        if (new RootService().IsCNIC(filters.searchTerm))
                        {
                            filters.searchTerm = filters.searchTerm.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filters.searchTerm)).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Name.ToLower().Contains(filters.searchTerm.ToLower()) || x.CNIC.Contains(filters.searchTerm)
                            ||
                            x.Designation.ToLower().Contains(filters.searchTerm.ToLower())).AsQueryable();
                        }
                    }
                    if (filters.value == "Daily Wages" || filters.value == "Regular" || filters.value == "Contract")
                    {
                        query = query.Where(x => x.EmployementMode == filters.value && x.EmployementMode != null && x.EmployementMode != "").AsQueryable();
                    }
                    if(filters.value == "Withlogin")
                    {
                        query = query.Where(x => x.UserName != "NA" && x.UserName != null && x.UserName != "").AsQueryable();
                    }
                    if (filters.value == "Withoutlogin")
                    {
                        query = query.Where(x => x.UserName == "NA" || x.UserName == null || x.UserName == "").AsQueryable();
                    }



                    var dailyWagesAccountDetail = _db.DailyWagerBankDetails.ToList();
                    var dailyWagesContractDetail = _db.DailyWagerContractDetails.ToList();
                    var count = query.Count();
                    var list = query.OrderBy(x => Guid.NewGuid()).ThenByDescending(x => x.Designation).ThenBy(x => x.Name).ThenBy(x => x.Designation).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    return new TableResponse<DailyWagerDetailView> { List = list, Count = count };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public TableResponse<DailyWagesCountViewModel> GetDailyWagesCount(ProfileFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 4;
                    IQueryable<DailyWagerDetailView> query = _db.DailyWagerDetailViews.AsQueryable();

                    if (filters.searchTerm != null && filters.searchTerm.Length >= 2)
                    {
                        if (new RootService().IsCNIC(filters.searchTerm))
                        {
                            filters.searchTerm = filters.searchTerm.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filters.searchTerm)).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Name.ToLower().Contains(filters.searchTerm.ToLower()) || x.CNIC.Contains(filters.searchTerm)
                            ||
                            x.Category.ToLower().Contains(filters.searchTerm.ToLower())).AsQueryable();
                        }
                    }
                    if (filters.hfmisCode.Length == 3 && !string.IsNullOrEmpty(filters.Category)) //For Division
                    {
                        query = query.Where(x => x.DivisionCode == filters.hfmisCode && x.Category == filters.Category).AsQueryable();
                    }
                    else if ((filters.hfmisCode.Length == 5 || filters.hfmisCode.Length == 6) && filters.hfmisCode.StartsWith("0") && string.IsNullOrEmpty(filters.Category)) //For District
                    {
                        query = query.Where(x => x.DistirctCode == filters.hfmisCode).AsQueryable();
                    }
                    else if ((filters.hfmisCode.Length == 5 || filters.hfmisCode.Length == 6) && filters.hfmisCode.StartsWith("0") && !string.IsNullOrEmpty(filters.Category)) //For District
                    {
                        query = query.Where(x => x.DistirctCode == filters.hfmisCode && x.Category == filters.Category).AsQueryable();
                    }
                    else if (filters.hfmisCode.Length == 5 && !string.IsNullOrEmpty(filters.Category)) //For HFMIS or UC
                    {
                        query = query.Where(x => (x.HfmisCode == filters.hfmisCode || x.UcCode == filters.hfmisCode)
                        && x.Category == filters.Category).AsQueryable();
                    }
                    else if (filters.hfmisCode.Length == 9 && !string.IsNullOrEmpty(filters.Category)) //For Tehsil
                    {
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode && x.Category == filters.Category).AsQueryable();
                    }
                    else if (!string.IsNullOrEmpty(filters.Category))
                    {
                        query = query.Where(x => x.Category == filters.Category).AsQueryable();

                    }

                    else if (filters.hfmisCode.Length == 3 && string.IsNullOrEmpty(filters.Category)) //For Division
                    {
                        query = query.Where(x => x.DivisionCode == filters.hfmisCode).AsQueryable();
                    }
                    else if ((filters.hfmisCode.Length == 5 || filters.hfmisCode.Length == 6) && filters.hfmisCode.StartsWith("0")) //For District
                    {
                        query = query.Where(x => x.DistirctCode == filters.hfmisCode).AsQueryable();
                    }
                    else if (filters.hfmisCode.Length == 5 && string.IsNullOrEmpty(filters.Category)) //For HFMIS or UC
                    {
                        query = query.Where(x => x.HfmisCode == filters.hfmisCode || x.UcCode == filters.hfmisCode).AsQueryable();
                    }
                    else if (filters.hfmisCode.Length == 9 && string.IsNullOrEmpty(filters.Category)) //For Tehsil
                    {
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode).AsQueryable();
                    }


                    var dailyWagesAccountDetail = _db.DailyWagerBankDetails.ToList();
                    var dailyWagesContractDetail = _db.DailyWagerContractDetails.ToList();

                    var count = query.Count();
                    //	var list = query.OrderBy(x => Guid.NewGuid()).ThenByDescending(x => x.Designation).ThenBy(x => x.Name).ThenBy(x => x.Designation).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    var list = query.ToList();
                    //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                    //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                    //	 .Select(g => new {
                    //		 g.Key.Division,
                    //		 g.Key.District)}).ToList();

                    var results = list.GroupBy(n => new { n.Division, n.District, n.Tehsil, n.Designation })
                      .Select(g => new {
                          g.Key.Division,
                          g.Key.District,
                          g.Key.Tehsil,
                          g.Key.Designation,
                          Count = g.Count()
                      }).ToList();

                    List<DailyWagesCountViewModel> DailyWagesCountList = new List<DailyWagesCountViewModel>();
                    foreach (var item in results)
                    {
                        DailyWagesCountViewModel model = new DailyWagesCountViewModel();
                        model.Division = item.Division;
                        model.District = item.District;
                        model.Tehsil = item.Tehsil;
                        model.Designation = item.Designation;
                        model.Count = item.Count;
                        DailyWagesCountList.Add(model);

                    }

                    return new TableResponse<DailyWagesCountViewModel> { List = DailyWagesCountList, Count = count };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public TableResponse<DailyWagesDistirictCountViewModel> GetDailyWagesDistrict(ProfileFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 4;
                    IQueryable<DailyWagerDetailView> query = _db.DailyWagerDetailViews.AsQueryable();

                    if (filters.searchTerm != null && filters.searchTerm.Length >= 2)
                    {
                        if (new RootService().IsCNIC(filters.searchTerm))
                        {
                            filters.searchTerm = filters.searchTerm.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filters.searchTerm)).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Name.ToLower().Contains(filters.searchTerm.ToLower()) || x.CNIC.Contains(filters.searchTerm)
                            ||
                            x.Category.ToLower().Contains(filters.searchTerm.ToLower())).AsQueryable();
                        }
                    }
                    if (filters.hfmisCode.Length == 3 && !string.IsNullOrEmpty(filters.Category)) //For Division
                    {
                        query = query.Where(x => x.DivisionCode == filters.hfmisCode && x.Category == filters.Category).AsQueryable();
                    }
                    else if ((filters.hfmisCode.Length == 5 || filters.hfmisCode.Length == 6) && filters.hfmisCode.StartsWith("0") && string.IsNullOrEmpty(filters.Category)) //For District
                    {
                        query = query.Where(x => x.DistirctCode == filters.hfmisCode).AsQueryable();
                    }
                    else if ((filters.hfmisCode.Length == 5 || filters.hfmisCode.Length == 6) && filters.hfmisCode.StartsWith("0") && !string.IsNullOrEmpty(filters.Category)) //For District
                    {
                        query = query.Where(x => x.DistirctCode == filters.hfmisCode && x.Category == filters.Category).AsQueryable();
                    }
                    else if (filters.hfmisCode.Length == 5 && !string.IsNullOrEmpty(filters.Category)) //For HFMIS or UC
                    {
                        query = query.Where(x => (x.HfmisCode == filters.hfmisCode || x.UcCode == filters.hfmisCode)
                        && x.Category == filters.Category).AsQueryable();
                    }
                    else if (filters.hfmisCode.Length == 9 && !string.IsNullOrEmpty(filters.Category)) //For Tehsil
                    {
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode && x.Category == filters.Category).AsQueryable();
                    }


                    else if (filters.hfmisCode.Length == 3 && string.IsNullOrEmpty(filters.Category)) //For Division
                    {
                        query = query.Where(x => x.DivisionCode == filters.hfmisCode).AsQueryable();
                    }
                    else if ((filters.hfmisCode.Length == 5 || filters.hfmisCode.Length == 6) && filters.hfmisCode.StartsWith("0")) //For District
                    {
                        query = query.Where(x => x.DistirctCode == filters.hfmisCode).AsQueryable();
                    }
                    else if (filters.hfmisCode.Length == 5 && string.IsNullOrEmpty(filters.Category)) //For HFMIS or UC
                    {
                        query = query.Where(x => x.HfmisCode == filters.hfmisCode || x.UcCode == filters.hfmisCode).AsQueryable();
                    }
                    else if (filters.hfmisCode.Length == 9 && string.IsNullOrEmpty(filters.Category)) //For Tehsil
                    {
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode).AsQueryable();
                    }
                    else if (!string.IsNullOrEmpty(filters.Category))
                    {
                        query = query.Where(x => x.Category == filters.Category).AsQueryable();

                    }


                    var dailyWagesAccountDetail = _db.DailyWagerBankDetails.ToList();
                    var dailyWagesContractDetail = _db.DailyWagerContractDetails.ToList();

                    var count = query.Count();
                    //	var list = query.OrderBy(x => Guid.NewGuid()).ThenByDescending(x => x.Designation).ThenBy(x => x.Name).ThenBy(x => x.Designation).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    var list = query.ToList();
                    // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                    //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                    //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                    //	 .Select(g => new {
                    //		 g.Key.Division,
                    //		 g.Key.District)}).ToList();


                    var results = list.GroupBy(n => new { n.Division, n.District, n.Tehsil, n.Designation, n.Category, n.UserName })
                      .Select(g => new {
                          g.Key.Division,
                          g.Key.District,
                          g.Key.Tehsil,
                          g.Key.Designation,
                          g.Key.Category,
                          g.Key.UserName,
                          Count = g.Count(),
                          WithLogin = g.Sum(e => e.UserName.Contains("NA") ? 0 : 1),
                          Withoutlogin = g.Sum(c => g.Key.Designation.Contains("NA") ? 1 : 0)
                      }).ToList();

                    List<DailyWagesDistirictCountViewModel> DailyWagesCountList = new List<DailyWagesDistirictCountViewModel>();
                    foreach (var item in results)
                    {
                        DailyWagesDistirictCountViewModel model = new DailyWagesDistirictCountViewModel();
                        model.Division = item.Division;
                        model.District = item.District;
                        model.Tehsil = item.Tehsil;
                        model.Designation = item.Designation;
                        model.Count = item.Count;
                        model.WithLogin = item.WithLogin;
                        model.WithLogin = item.Withoutlogin;
                        DailyWagesCountList.Add(model);

                    }

                    return new TableResponse<DailyWagesDistirictCountViewModel> { List = DailyWagesCountList, Count = count };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public TableResponse<DailyWagesMapCountViewModel> GetDailyWagesForMap(ProfileFilters filters)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 4;

                    IQueryable<DailyWagerDetailView> query = _db.DailyWagerDetailViews.AsQueryable();




                    if (filters.searchTerm != null && filters.searchTerm.Length >= 2)
                    {
                        if (new RootService().IsCNIC(filters.searchTerm))
                        {
                            filters.searchTerm = filters.searchTerm.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filters.searchTerm)).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Name.ToLower().Contains(filters.searchTerm.ToLower()) || x.CNIC.Contains(filters.searchTerm)
                            ||
                            x.Category.ToLower().Contains(filters.searchTerm.ToLower())).AsQueryable();
                        }
                    }
                    if (filters.hfmisCode == "0" && string.IsNullOrEmpty(filters.Category))   //All null
                    {
                        query = _db.DailyWagerDetailViews.AsQueryable();
                        //var countttt = query.Count();
                        //query = query.Where(x => (x.EmployementMode != null && x.EmployementMode != "")).AsQueryable();
                        var Tehsilandcategory = query.ToList();
                        var countt = query.Count();

                        // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                        //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                        //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                        //	 .Select(g => new {
                        //		 g.Key.Division,
                        //		 g.Key.District)}).ToList();


                        var Tehsilandcategoryresults = Tehsilandcategory.GroupBy(n => new { n.Category })
                          .Select(g => new
                          {

                              // g.Key.District,
                              //g.Key.Tehsil,
                              g.Key.Category,
                              Count = g.Count(),
                              dailywages = (int?)g.Sum(e => e.EmployementMode.Equals("Daily Wages") ? 1 : 0),
                              Regular = (int?)g.Sum(e => e.EmployementMode.Equals("Regular") ? 1 : 0),
                              Contracts = (int?)g.Sum(c => c.EmployementMode.Equals("Contract") ? 1 : 0),
                              WithLogin = (int?)g.Sum(e => e.UserName != "NA" && !string.IsNullOrEmpty(e.UserName) ? 1 : 0),
                              Withoutlogin = (int?)g.Sum(c => c.UserName == "NA" || string.IsNullOrEmpty(c.UserName) ? 1 : 0)
                          }).ToList();

                        List<DailyWagesMapCountViewModel> TehsilandcategoryDailyWagesCountList = new List<DailyWagesMapCountViewModel>();
                        foreach (var item in Tehsilandcategoryresults)
                        {
                            DailyWagesMapCountViewModel model = new DailyWagesMapCountViewModel();

                            // model.District = item.District;
                            //model.Tehsil = item.Tehsil;
                            model.Category = item.Category;
                            model.Count = item.Count;
                            model.Regular = item.Regular;
                            model.Contracts = item.Contracts;
                            model.Withlogin = item.WithLogin;
                            model.Withoutlogin = item.Withoutlogin;
                            model.dailywages = item.dailywages;
                            TehsilandcategoryDailyWagesCountList.Add(model);

                        }

                        return new TableResponse<DailyWagesMapCountViewModel> { List = TehsilandcategoryDailyWagesCountList, Count = 1 };

                    }
                    else if (filters.hfmisCode.Length == 3 && !string.IsNullOrEmpty(filters.Category)) //For Division and  with Category
                    {
                        query = query.Where(x => x.DivisionCode == filters.hfmisCode && x.Category == filters.Category).AsQueryable();

                    }
                    else if ((filters.hfmisCode.Length == 5 || filters.hfmisCode.Length == 6) && filters.hfmisCode.StartsWith("0") && string.IsNullOrEmpty(filters.Category)) //For District with out CateGory
                    {
                        query = query.Where(x => x.DistirctCode == filters.hfmisCode).AsQueryable();
                        var Tehsilandcategoryout = query.ToList();
                        // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                        //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                        //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                        //	 .Select(g => new {
                        //		 g.Key.Division,
                        //		 g.Key.District)}).ToList();

                        var Tehsilandcategoryresults = Tehsilandcategoryout.GroupBy(n => new { n.Category })
                      .Select(g => new
                      {

                          // g.Key.District,
                          //g.Key.Tehsil,
                          g.Key.Category,
                          Count = g.Count(),
                          dailywages = g.Sum(e => e.EmployementMode.Equals("Daily Wages") ? 1 : 0),
                          Regular = g.Sum(e => e.EmployementMode.Equals("Regular") ? 1 : 0),
                          Contracts = g.Sum(c => c.EmployementMode.Equals("Contract") ? 1 : 0),
                          WithLogin = (int?)g.Sum(e => e.UserName != "NA" && !string.IsNullOrEmpty(e.UserName) ? 1 : 0),
                          Withoutlogin = g.Sum(c => c.UserName.Contains("NA") ? 1 : 0)
                      }).ToList();

                        List<DailyWagesMapCountViewModel> TehsilandcategoryDailyWagesCountList = new List<DailyWagesMapCountViewModel>();
                        foreach (var item in Tehsilandcategoryresults)
                        {
                            DailyWagesMapCountViewModel model = new DailyWagesMapCountViewModel();

                            // model.District = item.District;
                            //model.Tehsil = item.Tehsil;
                            model.Category = item.Category;
                            model.Count = item.Count;
                            model.Regular = item.Regular;
                            model.Contracts = item.Contracts;
                            model.Withlogin = item.WithLogin;
                            model.Withoutlogin = item.Withoutlogin;
                            model.dailywages = item.dailywages;
                            TehsilandcategoryDailyWagesCountList.Add(model);

                        }

                        return new TableResponse<DailyWagesMapCountViewModel> { List = TehsilandcategoryDailyWagesCountList, Count = 1 };
                    }
                    else if (filters.hfmisCode.Length == 9 && !string.IsNullOrEmpty(filters.Category)) //For Tehsil with Category
                    {
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode).AsQueryable();
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode && x.Category == filters.Category).AsQueryable();
                        var Tehsilandcategoryout = query.ToList();
                        // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                        //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                        //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                        //	 .Select(g => new {
                        //		 g.Key.Division,
                        //		 g.Key.District)}).ToList();

                        List<DailyWagesMapCountViewModel> TehsilandcategoryDailyWagesCountList = new List<DailyWagesMapCountViewModel>();

                        var Tehsilandcategoryresults = Tehsilandcategoryout.GroupBy(n => new { n.Category })
                                              .Select(g => new
                                              {

                                                  // g.Key.District,
                                                  //g.Key.Tehsil,
                                                  g.Key.Category,
                                                  Count = g.Count(),
                                                  dailywages = g.Sum(e => e.EmployementMode.Equals("Daily Wages") ? 1 : 0),
                                                  Regular = g.Sum(e => e.EmployementMode.Equals("Regular") ? 1 : 0),
                                                  Contracts = g.Sum(c => c.EmployementMode.Equals("Contract") ? 1 : 0),
                                                  WithLogin = (int?)g.Sum(e => e.UserName != "NA" && !string.IsNullOrEmpty(e.UserName) ? 1 : 0),
                                                  Withoutlogin = g.Sum(c => c.UserName.Contains("NA") ? 1 : 0)
                                              }).ToList();

                        foreach (var item in Tehsilandcategoryresults)
                        {
                            DailyWagesMapCountViewModel model = new DailyWagesMapCountViewModel();

                            // model.District = item.District;
                            //model.Tehsil = item.Tehsil;
                            model.Category = item.Category;
                            model.Count = item.Count;
                            model.Regular = item.Regular;
                            model.Contracts = item.Contracts;
                            model.Withlogin = item.WithLogin;
                            model.Withoutlogin = item.Withoutlogin;
                            model.dailywages = item.dailywages;
                            TehsilandcategoryDailyWagesCountList.Add(model);

                        }

                        return new TableResponse<DailyWagesMapCountViewModel> { List = TehsilandcategoryDailyWagesCountList, Count = 1 };
                    }
                    else if ((filters.hfmisCode.Length == 5 || filters.hfmisCode.Length == 6) && filters.hfmisCode.StartsWith("0") && !string.IsNullOrEmpty(filters.Category)) //For District without Category
                    {
                        query = query.Where(x => x.DistirctCode == filters.hfmisCode && x.Category == filters.Category).AsQueryable();
                        var Tehsilandcategoryout = query.ToList();
                        // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                        //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                        //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                        //	 .Select(g => new {
                        //		 g.Key.Division,
                        //		 g.Key.District)}).ToList();



                        var Tehsilandcategoryresults = Tehsilandcategoryout.GroupBy(n => new { n.Category })
                      .Select(g => new
                      {

                          // g.Key.District,
                          //g.Key.Tehsil,
                          g.Key.Category,
                          Count = g.Count(),
                          dailywages = g.Sum(e => e.EmployementMode.Equals("Daily Wages") ? 1 : 0),
                          Regular = g.Sum(e => e.EmployementMode.Equals("Regular") ? 1 : 0),
                          Contracts = g.Sum(c => c.EmployementMode.Equals("Contract") ? 1 : 0),
                          WithLogin = (int?)g.Sum(e => e.UserName != "NA" && !string.IsNullOrEmpty(e.UserName) ? 1 : 0),
                          Withoutlogin = g.Sum(c => c.UserName.Contains("NA") ? 1 : 0)
                      }).ToList();

                        List<DailyWagesMapCountViewModel> TehsilandcategoryDailyWagesCountList = new List<DailyWagesMapCountViewModel>();
                        foreach (var item in Tehsilandcategoryresults)
                        {
                            DailyWagesMapCountViewModel model = new DailyWagesMapCountViewModel();

                            // model.District = item.District;
                            //model.Tehsil = item.Tehsil;
                            model.Category = item.Category;
                            model.Count = item.Count;
                            model.Regular = item.Regular;
                            model.Contracts = item.Contracts;
                            model.Withlogin = item.WithLogin;
                            model.Withoutlogin = item.Withoutlogin;
                            model.dailywages = item.dailywages;
                            TehsilandcategoryDailyWagesCountList.Add(model);

                        }

                        return new TableResponse<DailyWagesMapCountViewModel> { List = TehsilandcategoryDailyWagesCountList, Count = 1 };
                    }
                    else if (filters.hfmisCode.Length == 9 && !string.IsNullOrEmpty(filters.Category)) //For Tehsil with Category
                    {
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode).AsQueryable();
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode && x.Category == filters.Category).AsQueryable();
                        var Tehsilandcategoryout = query.ToList();
                        // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                        //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                        //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                        //	 .Select(g => new {
                        //		 g.Key.Division,
                        //		 g.Key.District)}).ToList();


                        var Tehsilandcategoryresults = Tehsilandcategoryout.GroupBy(n => new { n.District, n.Tehsil, n.Category })
                          .Select(g => new
                          {

                              g.Key.District,
                              g.Key.Tehsil,
                              g.Key.Category,
                              Count = g.Count(),
                              Regular = g.Sum(e => e.Designation.Contains("Regular") ? 1 : 0),
                              Contracts = g.Sum(c => c.Designation.Contains("Contract") ? 1 : 0)
                          }).ToList();

                        List<DailyWagesMapCountViewModel> TehsilandcategoryDailyWagesCountList = new List<DailyWagesMapCountViewModel>();
                        foreach (var item in Tehsilandcategoryresults)
                        {
                            DailyWagesMapCountViewModel model = new DailyWagesMapCountViewModel();

                            model.District = item.District;
                            model.Tehsil = item.Tehsil;
                            model.Category = item.Category;
                            model.Count = item.Count;
                            model.Regular = item.Regular;
                            model.Contracts = item.Contracts;
                            TehsilandcategoryDailyWagesCountList.Add(model);

                        }

                        return new TableResponse<DailyWagesMapCountViewModel> { List = TehsilandcategoryDailyWagesCountList, Count = 1 };
                    }
                    else if (filters.hfmisCode.Length == 5 && !string.IsNullOrEmpty(filters.Category)) //For HFMIS or UC
                    {
                        query = query.Where(x => (x.HfmisCode == filters.hfmisCode || x.UcCode == filters.hfmisCode)
                        && x.Category == filters.Category).AsQueryable();
                    }
                    else if (filters.hfmisCode.Length == 9 && !string.IsNullOrEmpty(filters.Category)) //For Tehsil and category
                    {
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode && x.Category == filters.Category).AsQueryable();
                        var Tehsilandcategory = query.ToList();
                        // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                        //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                        //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                        //	 .Select(g => new {
                        //		 g.Key.Division,
                        //		 g.Key.District)}).ToList();


                        var Tehsilandcategoryresults = Tehsilandcategory.GroupBy(n => new { n.Category })
                      .Select(g => new
                      {

                          // g.Key.District,
                          //g.Key.Tehsil,
                          g.Key.Category,
                          Count = g.Count(),
                          dailywages = g.Sum(e => e.EmployementMode.Equals("Daily Wages") ? 1 : 0),
                          Regular = g.Sum(e => e.EmployementMode.Equals("Regular") ? 1 : 0),
                          Contracts = g.Sum(c => c.EmployementMode.Equals("Contract") ? 1 : 0),
                          WithLogin = (int?)g.Sum(e => e.UserName != "NA" && !string.IsNullOrEmpty(e.UserName) ? 1 : 0),
                          Withoutlogin = g.Sum(c => c.UserName.Contains("NA") ? 1 : 0)
                      }).ToList();

                        List<DailyWagesMapCountViewModel> TehsilandcategoryDailyWagesCountList = new List<DailyWagesMapCountViewModel>();
                        foreach (var item in Tehsilandcategoryresults)
                        {
                            DailyWagesMapCountViewModel model = new DailyWagesMapCountViewModel();

                            // model.District = item.District;
                            //model.Tehsil = item.Tehsil;
                            model.Category = item.Category;
                            model.Count = item.Count;
                            model.Regular = item.Regular;
                            model.Contracts = item.Contracts;
                            model.Withlogin = item.WithLogin;
                            model.Withoutlogin = item.Withoutlogin;
                            model.dailywages = item.dailywages;
                            TehsilandcategoryDailyWagesCountList.Add(model);

                        }

                        return new TableResponse<DailyWagesMapCountViewModel> { List = TehsilandcategoryDailyWagesCountList, Count = 1 };
                    }
                    else if (filters.hfmisCode.Length == 9 && !string.IsNullOrEmpty(filters.Category)) //For Tehsil with Category
                    {
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode).AsQueryable();
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode && x.Category == filters.Category).AsQueryable();
                        var Tehsilandcategoryout = query.ToList();
                        // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                        //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                        //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                        //	 .Select(g => new {
                        //		 g.Key.Division,
                        //		 g.Key.District)}).ToList();



                        var Tehsilandcategoryresults = Tehsilandcategoryout.GroupBy(n => new { n.Category })
                      .Select(g => new
                      {

                          // g.Key.District,
                          //g.Key.Tehsil,
                          g.Key.Category,
                          Count = g.Count(),
                          dailywages = g.Sum(e => e.EmployementMode.Equals("Daily Wages") ? 1 : 0),
                          Regular = g.Sum(e => e.EmployementMode.Equals("Regular") ? 1 : 0),
                          Contracts = g.Sum(c => c.EmployementMode.Equals("Contract") ? 1 : 0),
                          WithLogin = (int?)g.Sum(e => e.UserName != "NA" && !string.IsNullOrEmpty(e.UserName) ? 1 : 0),
                          Withoutlogin = g.Sum(c => c.UserName.Contains("NA") ? 1 : 0)
                      }).ToList();

                        List<DailyWagesMapCountViewModel> TehsilandcategoryDailyWagesCountList = new List<DailyWagesMapCountViewModel>();
                        foreach (var item in Tehsilandcategoryresults)
                        {
                            DailyWagesMapCountViewModel model = new DailyWagesMapCountViewModel();

                            // model.District = item.District;
                            //model.Tehsil = item.Tehsil;
                            model.Category = item.Category;
                            model.Count = item.Count;
                            model.Regular = item.Regular;
                            model.Contracts = item.Contracts;
                            model.Withlogin = item.WithLogin;
                            model.Withoutlogin = item.Withoutlogin;
                            model.dailywages = item.dailywages;
                            TehsilandcategoryDailyWagesCountList.Add(model);

                        }

                        return new TableResponse<DailyWagesMapCountViewModel> { List = TehsilandcategoryDailyWagesCountList, Count = 1 };
                    }
                    else if (filters.hfmisCode.Length == 9 && !string.IsNullOrEmpty(filters.Category)) //For Tehsil with Category
                    {
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode).AsQueryable();
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode && x.Category == filters.Category).AsQueryable();
                        var Tehsilandcategoryout = query.ToList();
                        // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                        //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                        //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                        //	 .Select(g => new {
                        //		 g.Key.Division,
                        //		 g.Key.District)}).ToList();


                        var Tehsilandcategoryresults = Tehsilandcategoryout.GroupBy(n => new { n.Category })
                          .Select(g => new
                          {

                              // g.Key.District,
                              //g.Key.Tehsil,
                              g.Key.Category,
                              Count = g.Count(),
                              dailywages = g.Sum(e => e.EmployementMode.Equals("Daily Wages") ? 1 : 0),
                              Regular = g.Sum(e => e.EmployementMode.Equals("Regular") ? 1 : 0),
                              Contracts = g.Sum(c => c.EmployementMode.Equals("Contract") ? 1 : 0),
                              WithLogin = (int?)g.Sum(e => e.UserName != "NA" && !string.IsNullOrEmpty(e.UserName) ? 1 : 0),
                              Withoutlogin = g.Sum(c => c.UserName.Contains("NA") ? 1 : 0)
                          }).ToList();

                        List<DailyWagesMapCountViewModel> TehsilandcategoryDailyWagesCountList = new List<DailyWagesMapCountViewModel>();
                        foreach (var item in Tehsilandcategoryresults)
                        {
                            DailyWagesMapCountViewModel model = new DailyWagesMapCountViewModel();

                            // model.District = item.District;
                            //model.Tehsil = item.Tehsil;
                            model.Category = item.Category;
                            model.Count = item.Count;
                            model.Regular = item.Regular;
                            model.Contracts = item.Contracts;
                            model.Withlogin = item.WithLogin;
                            model.Withoutlogin = item.Withoutlogin;
                            model.dailywages = item.dailywages;
                            TehsilandcategoryDailyWagesCountList.Add(model);

                        }

                        return new TableResponse<DailyWagesMapCountViewModel> { List = TehsilandcategoryDailyWagesCountList, Count = 1 };
                    }


                    else if (filters.hfmisCode.Length == 3 && string.IsNullOrEmpty(filters.Category)) //For Division with outCateGory
                    {
                        query = query.Where(x => x.DivisionCode == filters.hfmisCode).AsQueryable();
                    }
                    else if ((filters.hfmisCode.Length == 5 || filters.hfmisCode.Length == 6) && filters.hfmisCode.StartsWith("0")) //only For District
                    {
                        query = query.Where(x => x.DistirctCode == filters.hfmisCode).AsQueryable();
                        var Tehsilandcategory = query.ToList();
                        // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                        //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                        //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                        //	 .Select(g => new {
                        //		 g.Key.Division,
                        //		 g.Key.District)}).ToList();


                        var Tehsilandcategoryresults = Tehsilandcategory.GroupBy(n => new { n.Category })
                      .Select(g => new
                      {

                          // g.Key.District,
                          //g.Key.Tehsil,
                          g.Key.Category,
                          Count = g.Count(),
                          dailywages = g.Sum(e => e.EmployementMode.Equals("Daily Wages") ? 1 : 0),
                          Regular = g.Sum(e => e.EmployementMode.Equals("Regular") ? 1 : 0),
                          Contracts = g.Sum(c => c.EmployementMode.Equals("Contract") ? 1 : 0),
                          WithLogin = (int?)g.Sum(e => e.UserName != "NA" && !string.IsNullOrEmpty(e.UserName) ? 1 : 0),
                          Withoutlogin = g.Sum(c => c.UserName.Contains("NA") ? 1 : 0)
                      }).ToList();

                        List<DailyWagesMapCountViewModel> TehsilandcategoryDailyWagesCountList = new List<DailyWagesMapCountViewModel>();
                        foreach (var item in Tehsilandcategoryresults)
                        {
                            DailyWagesMapCountViewModel model = new DailyWagesMapCountViewModel();

                            // model.District = item.District;
                            //model.Tehsil = item.Tehsil;
                            model.Category = item.Category;
                            model.Count = item.Count;
                            model.Regular = item.Regular;
                            model.Contracts = item.Contracts;
                            model.Withlogin = item.WithLogin;
                            model.Withoutlogin = item.Withoutlogin;
                            model.dailywages = item.dailywages;
                            TehsilandcategoryDailyWagesCountList.Add(model);

                        }

                        return new TableResponse<DailyWagesMapCountViewModel> { List = TehsilandcategoryDailyWagesCountList, Count = 1 };
                    }
                    else if (filters.hfmisCode.Length == 9 && !string.IsNullOrEmpty(filters.Category)) //For Tehsil with Category
                    {
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode).AsQueryable();
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode && x.Category == filters.Category).AsQueryable();
                        var Tehsilandcategoryout = query.ToList();
                        // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                        //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                        //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                        //	 .Select(g => new {
                        //		 g.Key.Division,
                        //		 g.Key.District)}).ToList();


                        var Tehsilandcategoryresults = Tehsilandcategoryout.GroupBy(n => new { n.Category })
                      .Select(g => new
                      {

                          // g.Key.District,
                          //g.Key.Tehsil,
                          g.Key.Category,
                          Count = g.Count(),
                          dailywages = g.Sum(e => e.EmployementMode.Equals("Daily Wages") ? 1 : 0),
                          Regular = g.Sum(e => e.EmployementMode.Equals("Regular") ? 1 : 0),
                          Contracts = g.Sum(c => c.EmployementMode.Equals("Contract") ? 1 : 0),
                          WithLogin = (int?)g.Sum(e => e.UserName != "NA" && !string.IsNullOrEmpty(e.UserName) ? 1 : 0),
                          Withoutlogin = g.Sum(c => c.UserName.Contains("NA") ? 1 : 0)
                      }).ToList();

                        List<DailyWagesMapCountViewModel> TehsilandcategoryDailyWagesCountList = new List<DailyWagesMapCountViewModel>();
                        foreach (var item in Tehsilandcategoryresults)
                        {
                            DailyWagesMapCountViewModel model = new DailyWagesMapCountViewModel();

                            // model.District = item.District;
                            //model.Tehsil = item.Tehsil;
                            model.Category = item.Category;
                            model.Count = item.Count;
                            model.Regular = item.Regular;
                            model.Contracts = item.Contracts;
                            model.Withlogin = item.WithLogin;
                            model.Withoutlogin = item.Withoutlogin;
                            model.dailywages = item.dailywages;
                            TehsilandcategoryDailyWagesCountList.Add(model);

                        }

                        return new TableResponse<DailyWagesMapCountViewModel> { List = TehsilandcategoryDailyWagesCountList, Count = 1 };

                    }
                    else if (filters.hfmisCode.Length == 5 && string.IsNullOrEmpty(filters.Category)) //For HFMIS or UC
                    {
                        query = query.Where(x => x.HfmisCode == filters.hfmisCode || x.UcCode == filters.hfmisCode).AsQueryable();
                    }
                    else if (filters.hfmisCode.Length == 9 && string.IsNullOrEmpty(filters.Category)) //For Tehsil without Category
                    {
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode).AsQueryable();

                        var Tehsilandcategoryout = query.ToList();
                        // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                        //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                        //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                        //	 .Select(g => new {
                        //		 g.Key.Division,
                        //		 g.Key.District)}).ToList();


                        var Tehsilandcategoryresults = Tehsilandcategoryout.GroupBy(n => new { n.Category })
                              .Select(g => new
                              {

                                  // g.Key.District,
                                  //g.Key.Tehsil,
                                  g.Key.Category,
                                  Count = g.Count(),
                                  dailywages = g.Sum(e => e.EmployementMode.Equals("Daily Wages") ? 1 : 0),
                                  Regular = g.Sum(e => e.EmployementMode.Equals("Regular") ? 1 : 0),
                                  Contracts = g.Sum(c => c.EmployementMode.Equals("Contract") ? 1 : 0),
                                  WithLogin = (int?)g.Sum(e => e.UserName != "NA" && !string.IsNullOrEmpty(e.UserName) ? 1 : 0),
                                  Withoutlogin = g.Sum(c => c.UserName.Contains("NA") ? 1 : 0)
                              }).ToList();

                        List<DailyWagesMapCountViewModel> TehsilandcategoryDailyWagesCountList = new List<DailyWagesMapCountViewModel>();
                        foreach (var item in Tehsilandcategoryresults)
                        {
                            DailyWagesMapCountViewModel model = new DailyWagesMapCountViewModel();

                            // model.District = item.District;
                            //model.Tehsil = item.Tehsil;
                            model.Category = item.Category;
                            model.Count = item.Count;
                            model.Regular = item.Regular;
                            model.Contracts = item.Contracts;
                            model.Withlogin = item.WithLogin;
                            model.Withoutlogin = item.Withoutlogin;
                            model.dailywages = item.dailywages;
                            TehsilandcategoryDailyWagesCountList.Add(model);

                        }

                        return new TableResponse<DailyWagesMapCountViewModel> { List = TehsilandcategoryDailyWagesCountList, Count = 1 };
                    }
                    else if (filters.hfmisCode.Length == 9 && !string.IsNullOrEmpty(filters.Category)) //For Tehsil with Category
                    {
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode).AsQueryable();
                        query = query.Where(x => x.TehsilCode == filters.hfmisCode && x.Category == filters.Category).AsQueryable();
                        var Tehsilandcategoryout = query.ToList();
                        // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                        //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                        //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                        //	 .Select(g => new {
                        //		 g.Key.Division,
                        //		 g.Key.District)}).ToList();


                        var Tehsilandcategoryresults = Tehsilandcategoryout.GroupBy(n => new { n.District, n.Tehsil, n.Category })
                          .Select(g => new
                          {

                              g.Key.District,
                              g.Key.Tehsil,
                              g.Key.Category,
                              Count = g.Count(),
                              Regular = g.Sum(e => e.Designation.Contains("Regular") ? 1 : 0),
                              Contracts = g.Sum(c => c.Designation.Contains("Contract") ? 1 : 0)
                          }).ToList();

                        List<DailyWagesMapCountViewModel> TehsilandcategoryDailyWagesCountList = new List<DailyWagesMapCountViewModel>();
                        foreach (var item in Tehsilandcategoryresults)
                        {
                            DailyWagesMapCountViewModel model = new DailyWagesMapCountViewModel();

                            model.District = item.District;
                            model.Tehsil = item.Tehsil;
                            model.Category = item.Category;
                            model.Count = item.Count;
                            model.Regular = item.Regular;
                            model.Contracts = item.Contracts;
                            TehsilandcategoryDailyWagesCountList.Add(model);

                        }

                        return new TableResponse<DailyWagesMapCountViewModel> { List = TehsilandcategoryDailyWagesCountList, Count = 1 };
                    }

                    else if (filters.hfmisCode == "0" && !string.IsNullOrEmpty(filters.Category))   //For Only CateGory
                    {
                        query = query.Where(x => x.Category == filters.Category).AsQueryable();
                        var Tehsilandcategory = query.ToList();
                        // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                        //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                        //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                        //	 .Select(g => new {
                        //		 g.Key.Division,
                        //		 g.Key.District)}).ToList();


                        var Tehsilandcategoryresults = Tehsilandcategory.GroupBy(n => new { n.Category })
                         .Select(g => new
                         {

                             // g.Key.District,
                             //g.Key.Tehsil,
                             g.Key.Category,
                             Count = g.Count(),
                             dailywages = g.Sum(e => e.EmployementMode.Equals("Daily Wages") ? 1 : 0),
                             Regular = g.Sum(e => e.EmployementMode.Equals("Regular") ? 1 : 0),
                             Contracts = g.Sum(c => c.EmployementMode.Equals("Contract") ? 1 : 0),
                             WithLogin = g.Sum(e => e.UserName.Contains("NA") ? 0 : 1),
                             Withoutlogin = g.Sum(c => c.UserName.Contains("NA") ? 1 : 0)
                         }).ToList();

                        List<DailyWagesMapCountViewModel> TehsilandcategoryDailyWagesCountList = new List<DailyWagesMapCountViewModel>();
                        foreach (var item in Tehsilandcategoryresults)
                        {
                            DailyWagesMapCountViewModel model = new DailyWagesMapCountViewModel();

                            // model.District = item.District;
                            //model.Tehsil = item.Tehsil;
                            model.Category = item.Category;
                            model.Count = item.Count;
                            model.Regular = item.Regular;
                            model.Contracts = item.Contracts;
                            model.Withlogin = item.WithLogin;
                            model.Withoutlogin = item.Withoutlogin;
                            model.dailywages = item.dailywages;
                            TehsilandcategoryDailyWagesCountList.Add(model);

                        }

                        return new TableResponse<DailyWagesMapCountViewModel> { List = TehsilandcategoryDailyWagesCountList, Count = 1 };

                    }
                    //          else if (filters.hfmisCode == "0" && string.IsNullOrEmpty(filters.Category))   //All null
                    //          {
                    //              query = query.Where(x=>(x.UserName !=null && x.UserName!="") && (x.EmployementMode != null && x.EmployementMode != "")).AsQueryable();
                    //              var Tehsilandcategory = query.ToList();
                    //        var countt = query.Count();

                    //              // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                    //              //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                    //              //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                    //              //	 .Select(g => new {
                    //              //		 g.Key.Division,
                    //              //		 g.Key.District)}).ToList();


                    //              var Tehsilandcategoryresults = Tehsilandcategory.GroupBy(n => new {n.Category })
                    //                .Select(g => new {

                    //                   // g.Key.District,
                    //                    //g.Key.Tehsil,
                    //                    g.Key.Category,
                    //                    Count = g.Count(),
                    // dailywages = (int?)g.Sum(e => e.EmployementMode.Equals("Daily Wages") ? 1 : 0),
                    // Regular = (int?)g.Sum(e => e.EmployementMode.Equals("Regular") ? 1 : 0),
                    //                    Contracts = (int?)g.Sum(c => c.EmployementMode.Equals("Contract") ? 1 : 0),
                    //                  WithLogin = (int?)g.Sum(e => e.UserName.Contains("NA") ? 0 : 1),
                    // Withoutlogin = (int?)g.Sum(c =>c.UserName.Contains("NA") ? 1 : 0)
                    //}).ToList();

                    //              List<DailyWagesMapCountViewModel> TehsilandcategoryDailyWagesCountList = new List<DailyWagesMapCountViewModel>();
                    //              foreach (var item in Tehsilandcategoryresults)
                    //              {
                    //                  DailyWagesMapCountViewModel model = new DailyWagesMapCountViewModel();

                    //                 // model.District = item.District;
                    //                  //model.Tehsil = item.Tehsil;
                    //                  model.Category = item.Category;
                    //                  model.Count = item.Count;
                    //                  model.Regular = item.Regular;
                    //                  model.Contracts = item.Contracts;
                    //                  model.Withlogin = item.WithLogin;
                    //                  model.Withoutlogin = item.Withoutlogin;
                    //                  model.dailywages = item.dailywages;
                    //                  TehsilandcategoryDailyWagesCountList.Add(model);

                    //              }

                    //              return new TableResponse<DailyWagesMapCountViewModel> { List = TehsilandcategoryDailyWagesCountList, Count = 1 };

                    //          }


                    var dailyWagesAccountDetail = _db.DailyWagerBankDetails.ToList();
                    var dailyWagesContractDetail = _db.DailyWagerContractDetails.ToList();

                    var count = query.Count();
                    //	var list = query.OrderBy(x => Guid.NewGuid()).ThenByDescending(x => x.Designation).ThenBy(x => x.Name).ThenBy(x => x.Designation).Skip(filters.Skip).Take(filters.PageSize).ToList();
                    var list = query.ToList();
                    // var lis = list.Where(x => x.Designation.Contains("Regular")).ToList();
                    //var results = list.GroupBy(p => p.Division, p => p.District,p => p.Tehsil)	
                    //var results = list.GroupBy(n => new { n.Division, n.District,n.Tehsil}
                    //	 .Select(g => new {
                    //		 g.Key.Division,
                    //		 g.Key.District)}).ToList();


                    var results = list.GroupBy(n => new { n.Division, n.District, n.Tehsil, n.Designation, n.Category })
                      .Select(g => new
                      {
                          g.Key.Division,
                          g.Key.District,
                          g.Key.Tehsil,
                          g.Key.Designation,
                          g.Key.Category,
                          Count = g.Count(),
                          Regular = g.Sum(e => e.Designation.Contains("Regular") ? 1 : 0),
                          Contracts = g.Sum(c => c.Designation.Contains("Contract") ? 1 : 0)
                      }).ToList();

                    List<DailyWagesMapCountViewModel> DailyWagesCountList = new List<DailyWagesMapCountViewModel>();
                    foreach (var item in results)
                    {
                        DailyWagesMapCountViewModel model = new DailyWagesMapCountViewModel();
                        model.Division = item.Division;
                        model.District = item.District;
                        model.Tehsil = item.Tehsil;
                        model.Designation = item.Designation;
                        model.Count = item.Count;
                        model.Regular = item.Regular;
                        model.Contracts = item.Contracts;
                        DailyWagesCountList.Add(model);

                    }

                    return new TableResponse<DailyWagesMapCountViewModel> { List = DailyWagesCountList, Count = count };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DailyWagesProfileClass GetDailyWagerbyId(int wagerId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 4;
                    DailyWagesProfile query = _db.DailyWagesProfiles.FirstOrDefault(x => x.Id == wagerId);

                    var dailyWagesAccountDetail = _db.DailyWagerBankDetails.ToList();
                    var dailyWagesContractDetail = _db.DailyWagerContractDetails.ToList();


                    DailyWagesProfileClass obj = new DailyWagesProfileClass();
                    obj.UserId = query.UserId;
                    obj.UserName = query.UserName;
                    obj.Name = query.Name;
                    obj.FatherName = query.FatherName;
                    obj.Address = query.Address;
                    obj.CreatedBy = query.CreatedBy;
                    obj.CreatedDate = query.CreatedDate;
                    obj.CNIC = query.CNIC;
                    obj.DateOfBirth = query.DateOfBirth;
                    obj.Designation = query.Designation;
                    obj.Category = query.Category;
                    obj.EmployementMode = query.EmployementMode;
                    obj.DistirctCode = query.DistirctCode;
                    obj.DivisionCode = query.DivisionCode;
                    obj.TehsilCode = query.TehsilCode;
                    obj.UcCode = query.UcCode;
                    obj.PersonImage = query.PersonImage;
                    obj.MobileNumber = query.MobileNumber;
                    obj.Id = query.Id;
                    obj.Gender = query.Gender;
                    obj.HfmisCode = query.HfmisCode;
                    obj.dailyWagesAccountDetail = dailyWagesAccountDetail.FirstOrDefault(x => x.DailyWagerProfileId == query.Id);
                    obj.dailyWagesContractDetail = dailyWagesContractDetail.FirstOrDefault(x => x.WagerProfileId == query.Id);

                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }




        public object GetProfileById(int? Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    return _db.DailyWagerDetailViews.FirstOrDefault(x => x.Id == Id);



                }


            }
            catch (Exception ex)
            {

                throw ex;
            }





        }

		public List<DailyWagerDesignation> GetDesignationList(string name)
		{
			try
			{
				int id = 0;
				using (var _db = new HR_System())
				{
					if (name == "Dengue")
					{

						id = 1;
						return _db.DailyWagerDesignations.Where(x => x.CategoryId == id).ToList();
					}
					else if (name == "Polio")
					{
						id = 4;
						return _db.DailyWagerDesignations.Where(x => x.CategoryId == id).ToList(); ;

					}



					else if (name == "Madadgaar")
					{
						id = 2;
						return _db.DailyWagerDesignations.Where(x => x.CategoryId == id).ToList(); ;


					}

					else if (name == "PMIS")
					{
						id = 3;
						return _db.DailyWagerDesignations.Where(x => x.CategoryId == id).ToList(); ;


					}
					else if (name == "Other")
					{
						id = 5;
						return _db.DailyWagerDesignations.Where(x => x.CategoryId == id).ToList(); ;


					}
					else
					{

						return null;
					}
				}
			}
			catch (Exception ex)
			{

				throw ex;
			}



		}





		public DailyWagerContractDetail AddContractFileById(int Id, string imageFile)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    _db.Database.CommandTimeout = 60 * 4;
                    var fileContractImage = $"Contract_{Guid.NewGuid().ToString("N")}_{DateTime.UtcNow.AddHours(5).ToString("ddMMyyyyHHmmssffffff")}.jpg";
                    if (!string.IsNullOrEmpty(imageFile))
                    {
                        File.WriteAllBytes($"{path}/{fileContractImage}", Convert.FromBase64String(imageFile));
                        //contractDetail.ContractImagePath = fileContractImage;
                    }
                    DailyWagerContractDetail query = _db.DailyWagerContractDetails.FirstOrDefault(x => x.Id == Id);
                    if (query != null)
                    {
                        query.ContractImagePath = fileContractImage;
                        _db.DailyWagerContractDetails.Add(query);
                        _db.Entry(query).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }

                    return query;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<CorridinateClass> GetCooridinate(string name, string Type)
        {
            try
            {
                List<CorridinateClass> list = new List<CorridinateClass>();
                using (var _db = new HR_System())
                {
                    if (name == "null" && Type == "null")
                    {
                        list = _db.GetCoordinates.Select(x => new CorridinateClass { lat = x.Latitude, lng = x.Longitude }).ToList();
                    }

                    else if (name != null && Type == "division")
                    {
                        list = _db.GetCoordinates.Where(x=>x.DivisionName==name).Select(x => new CorridinateClass {  lat = x.Latitude, lng = x.Longitude }).ToList();
                    }
                    else if (name != null && Type.ToLower() == "district")
                    {
                        list = _db.GetCoordinates.Where(x => x.DistrictName == name).Select(x => new CorridinateClass {  lat = x.Latitude, lng = x.Longitude }).ToList();
                    }
                    else if (name != null && Type == "tehsil")
                    {
                        list = _db.GetCoordinates.Where(x => x.TehsilName == name).Select(x => new CorridinateClass { lat = x.Latitude, lng = x.Longitude }).ToList();

                    }
                }

                return list;
            }
            catch (Exception)
            {
                throw;
            }
            



        }

        public class CorridinateClass
        {
            //public string name { get; set; }
            //public string Type {get;set; }

            public double? lat {get; set; }  
            public double? lng    {get; set; }

        }

        //public (dynamic, long, long) GetByFilter(PaginatedFilter paginatedFilterDTO, string userName, string hfCode)
        //{
        //    try
        //    {

        //        var code = "0";
        //        if (paginatedFilterDTO.FromDate == null && paginatedFilterDTO.ToDate == null)
        //        {
        //            paginatedFilterDTO.FromDate = DateTime.Parse("2001-01-01");
        //            paginatedFilterDTO.ToDate = DateTime.Parse("2050-01-01");

        //        }
        //        if (paginatedFilterDTO.FromDate != null && paginatedFilterDTO.ToDate == null)
        //        {
        //            paginatedFilterDTO.FromDate = paginatedFilterDTO.FromDate;
        //            paginatedFilterDTO.ToDate = DateTime.Parse("2050-01-01");

        //        }
        //        if (paginatedFilterDTO.FromDate == null && paginatedFilterDTO.ToDate != null)
        //        {
        //            paginatedFilterDTO.FromDate = DateTime.Parse("2001-01-01");
        //            paginatedFilterDTO.ToDate = paginatedFilterDTO.ToDate.Value.AddDays(1);

        //        }
        //        if (paginatedFilterDTO.FromDate != null && paginatedFilterDTO.ToDate != null)
        //        {
        //            paginatedFilterDTO.FromDate = paginatedFilterDTO.FromDate;
        //            paginatedFilterDTO.ToDate = paginatedFilterDTO.ToDate.Value.AddDays(1);

        //        }
        //        if (paginatedFilterDTO.DivisionCode == null && paginatedFilterDTO.DistrictCode == null && paginatedFilterDTO.TehsilCode == null && paginatedFilterDTO.healthfacilityCode == null)
        //        {

        //            code = hfCode;

        //        }
        //        if (paginatedFilterDTO.DivisionCode != null && paginatedFilterDTO.DistrictCode == null && paginatedFilterDTO.TehsilCode == null && paginatedFilterDTO.healthfacilityCode == null)
        //        {

        //            code = paginatedFilterDTO.DivisionCode;

        //        }
        //        if (paginatedFilterDTO.DivisionCode != null && paginatedFilterDTO.DistrictCode != null && paginatedFilterDTO.TehsilCode == null && paginatedFilterDTO.healthfacilityCode == null)
        //        {

        //            code = paginatedFilterDTO.DistrictCode;

        //        }
        //        if (paginatedFilterDTO.DivisionCode != null && paginatedFilterDTO.DistrictCode != null && paginatedFilterDTO.TehsilCode != null && paginatedFilterDTO.healthfacilityCode == null)
        //        {

        //            code = paginatedFilterDTO.TehsilCode;

        //        }
        //        if (paginatedFilterDTO.DistrictCode != null && paginatedFilterDTO.DivisionCode != null && paginatedFilterDTO.TehsilCode != null && paginatedFilterDTO.healthfacilityCode != null)
        //        {

        //            code = paginatedFilterDTO.healthfacilityCode;

        //        }




        //        if (string.IsNullOrEmpty(hfCode))
        //        {
        //            hfCode = "0";


        //        }

        //        //paginatedFilterDTO.FromDate = (paginatedFilterDTO.FromDate == null ? DateTime.Now.Date : (DateTime)paginatedFilterDTO.FromDate);
        //        //paginatedFilterDTO.ToDate = (paginatedFilterDTO.ToDate == null ? DateTime.Now.Date : ((DateTime)paginatedFilterDTO.ToDate).AddHours(23).AddMinutes(59).AddSeconds(59));

        //        using var _db = new HR_System();
        //        if (paginatedFilterDTO.QueryString != "")
        //        {
        //            (List<DailyWagesProfileClass>, long, long) PatientDataList;
        //            var resultData = _db.Patient
        //                   .Where(p => p.Record_Status == true &&
        //                    (p.Health_Facility_Code.StartsWith(code)) &&
        //                   (p.Name.Trim().ToLower().Contains(paginatedFilterDTO.QueryString.Trim().ToLower()) || p.Cnic.Trim().ToLower().Contains(paginatedFilterDTO.QueryString.Trim().ToLower())))
        //                   .OrderByDescending(p => p.Creation_Date).Select
        //                   (p => new DailyWagesProfileClass
        //                   {


        //                   });

        //            if (!paginatedFilterDTO.IsForExcel)
        //            {
        //                return PatientDataList = CommonMethods<Patient>.GetPagedData(resultData, paginatedFilterDTO.Size, paginatedFilterDTO.PageNumber);
        //            }
        //        }
        //        if (paginatedFilterDTO.PatientListId == 1) // by calling myFunction(1) onclick in dashboard.component.html
        //        {
        //            var result = _db.Patient
        //                        .Where(p => p.Status != null && p.Record_Status == true && p.Health_Facility_Code.StartsWith(code) &&
        //                        p.Creation_Date >= paginatedFilterDTO.FromDate && p.Creation_Date <= paginatedFilterDTO.ToDate)
        //                        .OrderByDescending(p => p.Creation_Date).Select
        //                        (p => new Patient
        //                        {
        //                            Emr_Registration_No = p.Emr_Registration_No,
        //                            Name = p.Name,
        //                            Father_Name = p.Father_Name,
        //                            HealthFacilityDivision = p.HealthFacilityDivision,
        //                            HealthFacilityDistrict = p.HealthFacilityDistrict,
        //                            HealthFacilityTehsil = p.HealthFacilityTehsil,
        //                            HealthFacilityName = p.HealthFacilityName,
        //                            Cnic = p.Cnic,
        //                            Cnic_Type = p.Cnic_Type,
        //                            Age = p.Age,
        //                            Gender = p.Gender,
        //                            Phone_Number = p.Phone_Number,
        //                            //Weight = p.Weight,
        //                            //HIVResult = p.HIVResult,

        //                            //Tb_Type = p.Tb_Type,
        //                            Address = p.Address,
        //                            //CreatedByName = p.full,
        //                            Creation_Date = p.Creation_Date,
        //                            Updated_By = p.Updated_By,
        //                            Updated_Date = p.Updated_Date,
        //                            Status = p.Status,
        //                            Health_Facility_Code = p.Health_Facility_Code,
        //                            GUID = p.GUID,

        //                        });
        //            if (!paginatedFilterDTO.IsForExcel)
        //            {
        //                return PatientDataList = CommonMethods<Patient>.GetPagedData(result, paginatedFilterDTO.Size, paginatedFilterDTO.PageNumber);
        //            }
        //            else
        //            {
        //                return (result.ToList(), 0, 0);
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        while (ex.InnerException != null)
        //        {
        //            ex = ex.InnerException;
        //        }

        //        throw ex;
        //    }
        //}



        public class DailyWagesProfileClass
        {

            public int Id { get; set; }
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string Name { get; set; }
            public string FatherName { get; set; }
            public string CNIC { get; set; }
            public string MobileNumber { get; set; }
            public string Category { get; set; }
            public string Designation { get; set; }
            public string EmployementMode { get; set; }

            public string Address { get; set; }

            public string DivisionCode { get; set; }

            public string DistirctCode { get; set; }

            public string TehsilCode { get; set; }

            public string UcCode { get; set; }

            public string HfmisCode { get; set; }

            public string CreatedBy { get; set; }

            public Nullable<System.DateTime> CreatedDate { get; set; }

            public string UpdateBy { get; set; }

            public string UpdationDate { get; set; }

            public Nullable<System.DateTime> DateOfBirth { get; set; }

            public string Gender { get; set; }

            public string PersonImage { get; set; }

            public DailyWagerBankDetail dailyWagesAccountDetail { get; set; }

            public DailyWagerContractDetail dailyWagesContractDetail { get; set; }
        }
        public class DailyWagesCountViewModel
        {
            public string Designation { get; set; }
            public string Division { get; set; }

            public string District { get; set; }

            public string Tehsil { get; set; }
            public int Count { get; set; }
        }
        public class DailyWagesMapCountViewModel
        {
            public string Designation { get; set; }
            public string Division { get; set; }

            public string District { get; set; }
            public string Category { get; set; }

            public string Tehsil { get; set; }
            public int? Count { get; set; }
            public int? Regular { get; set; }
            public int? Contracts { get; set; }
            public int? Withlogin { get; set; }
            public int? Withoutlogin { get; set; }
            public int? dailywages { get; set; }
        }
        public class DailyWagesDistirictCountViewModel
        {
            public string Designation { get; set; }
            public string Division { get; set; }

            public string District { get; set; }

            public string Tehsil { get; set; }
            public int Count { get; set; }
            public int WithLogin { get; set; }
            public int WithoutLogin { get; set; }
        }

        public class DailyWagesAccountDetail
        {

            public int Id { get; set; }

            public Nullable<int> DailyWagerProfileId { get; set; }

            public Nullable<int> BankId { get; set; }

            public string AccountTitle { get; set; }

            public string AccountNumber { get; set; }


        }


        public class DailyWagesContractDetail
        {
            public int Id { get; set; }

            public Nullable<int> WagerProfileId { get; set; }

            public Nullable<System.DateTime> ContractStartDate { get; set; }

            public Nullable<System.DateTime> ContractEndDate { get; set; }

            public string ContractImagePath { get; set; }

            public string ContractStatus { get; set; }

            public string CreatedBy { get; set; }

            public Nullable<System.DateTime> CreatedDate { get; set; }

        }

        public class PaginatedFilter
        {
            public int PageNumber { get; set; } = 1;
            public int TotalRecords { get; set; }
            public int Size { get; set; }
            public int PageCount { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public string QueryString { get; set; }
            public string DivisionCode { get; set; }
            public string DistrictCode { get; set; }
            public string TehsilCode { get; set; }
            public string healthfacilityCode { get; set; }
            public int PatientListId { get; set; }
            public bool IsForExcel { get; set; }
            public virtual IEnumerable<int> testID { get; set; }
        }

        public class ContractFileDTO
        {
            public int Id { get; set; }

            public string imageFile { get; set; }
        }
    }
}

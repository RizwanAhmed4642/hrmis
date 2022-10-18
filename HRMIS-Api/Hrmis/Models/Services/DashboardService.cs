using Hrmis.Controllers.HrmisRestApi;
using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.ViewModels.HealthFacility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Services
{
    public class DashboardService
    {
        public TableResponse<ApplicationView> GetApplications(ApplicationFilter filter, string userName, string userId, bool? countsOnly = false)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    List<int?> statusIds = new List<int?>() { 1, 2, 3, 4, 6, 7, 12, 8, 10 };
                    IQueryable<ApplicationView> query = _db.ApplicationViews.Where(x => x.IsActive == true && statusIds.Contains(x.Status_Id) && x.PandSOfficer_Id != 0 && (x.ApplicationSource_Id == 1 || x.ApplicationSource_Id == 2 || x.ApplicationSource_Id == 5) && x.PandSOfficer_Id != null && !x.CNIC.Equals("1111111111111") && !x.Created_By.StartsWith("managerfc")).AsQueryable();
                    if (filter.From == null && filter.To == null)
                    {
                        filter.From = new DateTime(1970, 1, 1);
                        filter.To = DateTime.Now.AddDays(1);
                    }
                    else if (filter.From != null && filter.To == null)
                    {
                        filter.To = DateTime.Now.AddDays(1);
                    }
                    else if (filter.From == null && filter.To == null)
                    {
                        filter.From = new DateTime(1970, 1, 1);
                        filter.To = filter.To.Value.AddDays(1);
                    }
                    else if (filter.From != null && filter.To != null)
                    {
                        filter.To = filter.To.Value.AddDays(1);
                    }
                    if (filter.From != null)
                    {
                        query = query.Where(x => x.Created_Date >= filter.From).AsQueryable();
                    }
                    if (filter.To != null)
                    {
                        query = query.Where(x => x.Created_Date <= filter.To).AsQueryable();
                    }
                    //if (!userName.Equals("dpd") && !userName.Equals("managerfc"))
                    //{
                    //    query = query.Where(x => x.Users_Id.Equals(userId)).AsQueryable();
                    //}
                    if (filter.Source_Id != 0)
                    {
                        query = query.Where(x => x.ApplicationSource_Id == filter.Source_Id).AsQueryable();
                    }
                    if (filter.Status_Id != 0)
                    {
                        if (filter.Status_Id == 22)
                        {
                            query = query.Where(x => x.Status_Id == 1 || x.Status_Id == 10).AsQueryable();
                        }
                        else if (filter.Status_Id == 33)
                        {
                            query = query.Where(x => x.Status_Id == 4 || x.Status_Id == 6 || x.Status_Id == 7).AsQueryable();
                        }
                        else if (filter.Status_Id == 3334)
                        {
                            query = query.Where(x => x.Status_Id == 2 || x.Status_Id == 3 || x.Status_Id == 4 || x.Status_Id == 6 || x.Status_Id == 7).AsQueryable();
                        }
                        else if (filter.Status_Id == 77)
                        {
                            query = query.Where(x => (x.Status_Id == 1 || x.Status_Id == 10) && x.DateDiff <= 7).AsQueryable();
                        }
                        else if (filter.Status_Id == 1515)
                        {
                            query = query.Where(x => (x.Status_Id == 1 || x.Status_Id == 10) && x.DateDiff > 7 && x.DateDiff <= 15).AsQueryable();
                        }
                        else if (filter.Status_Id == 3030)
                        {
                            query = query.Where(x => (x.Status_Id == 1 || x.Status_Id == 10) && x.DateDiff > 15 && x.DateDiff <= 30).AsQueryable();
                        }
                        else if (filter.Status_Id == 3655)
                        {
                            query = query.Where(x => (x.Status_Id == 1 || x.Status_Id == 10) && x.DateDiff > 30).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Status_Id == filter.Status_Id).AsQueryable();
                        }
                    }
                    if (filter.Type_Id != 0)
                    {
                        query = query.Where(x => x.ApplicationType_Id == filter.Type_Id).AsQueryable();
                    }
                    if (!string.IsNullOrEmpty(filter.Program))
                    {
                        query = query.Where(x => x.PandSOfficerProgram.Equals(filter.Program));
                    }
                    if (filter.OfficeId != 0)
                    {
                        query = query.Where(x => x.PandSOfficer_Id == filter.OfficeId);
                    }
                    if (filter.Query != null && filter.Query.Length >= 2)
                    {
                        var isNumber = int.TryParse(filter.Query.Replace(" ", ""), out int tracking);

                        if (new RootService().IsCNIC(filter.Query.Replace(" ", "")))
                        {
                            filter.Query = filter.Query.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filter.Query)).AsQueryable();
                        }
                        else if (isNumber)
                        {
                            var number = Convert.ToInt32(filter.Query);
                            filter.Skip = 0;
                            query = query.Where(x => x.TrackingNumber == number).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.EmployeeName.ToLower().Contains(filter.Query.ToLower())).AsQueryable();
                        }
                    }
                    var count = query.Count();
                    if (countsOnly == false)
                    {
                        var list = query.OrderByDescending(x => x.Created_Date).Skip(filter.Skip).Take(filter.PageSize).ToList();
                        return new TableResponse<ApplicationView>() { Count = count, List = list };
                    }
                    else
                    {
                        return new TableResponse<ApplicationView>() { Count = count };
                    }

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public static List<AS> Getpendency()
        {
            using (var db = new HR_System())
            {
                try
                {

                    List<AS> aslist= new List<AS>();
                    var concernOfficer = db.P_SConcernedOfficers.ToList();
                    var P_SOfficer = db.P_SOfficers.ToList();

                    //Additional Secretary List
                    var ASOfficer = db.P_SOfficers.Where(x => x.Code.ToString().Length == 2 && x.Code != null && x.DesignationName.Contains("Additional Secretary")).Select(x => new AS {
                     Id=x.Id, Name=x.Name, DesignationName= x.DesignationName,
                     Pendency = db.ApplicationMasters.Where(y => y.PandSOfficer_Id == x.Id && y.IsPending == false && (y.Status_Id == 10 || y.Status_Id == 1)).Count() 
                     }).ToList();
                    // Deputy Secretary List
                    var DSOfficer = db.P_SOfficers.Where(x => x.Code.ToString().Length == 3 && x.DesignationName.Contains("Deputy Secretary") && x.Code != null).ToList();
                    // Section List
                    var SectionOfficer = db.P_SOfficers.Where(x => x.Code.ToString().Length == 5 && x.DesignationName.Contains("Section") && x.Code != null).ToList();
                    aslist.AddRange(ASOfficer);
                    foreach (var list in aslist)
                    {
                        foreach (var concern in concernOfficer)
                        {
                            if (list.Id == concern.Officer_Id)
                            {
                                foreach (var Dslist in DSOfficer)
                                {

                                    if (Dslist.Id == concern.ConcernedOfficer_Id)
                                    {
                                        DS ds = new DS();
                                        ds.Id = Dslist.Id;
                                        ds.Name = Dslist.Name;
                                        ds.DesignationName = Dslist.DesignationName;
                                        ds.Pendency = db.ApplicationMasters.Where(x => x.PandSOfficer_Id == Dslist.Id && x.IsPending == false && x.IsActive == true && (x.Status_Id == 10 || x.Status_Id == 1)).Count();
                                        list.DSList.Add(ds);
                                    }
                                    
                                }
                                
                            }
                            
                        }
                        foreach (var item in list.DSList)
                        {
                            foreach (var concern in concernOfficer)
                            {
                                if (item.Id == concern.Officer_Id)
                                {
                                    foreach (var sectonlist in SectionOfficer)
                                    {
                                        if (sectonlist.Id == concern.ConcernedOfficer_Id)
                                        {
                                            section sec = new section();
                                            sec.Id = sectonlist.Id;
                                            sec.Name = sectonlist.Name;
                                            sec.DesignationName = sectonlist.DesignationName;
                                            sec.Pendency = db.ApplicationMasters.Where(x => x.PandSOfficer_Id == sectonlist.Id && x.IsPending == false && x.IsActive == true && (x.Status_Id == 10 || x.Status_Id == 1)).Count();
                                            item.Section.Add(sec);
                                        }
                                        
                                    }

                                }
                            }
                        }
                    }

                    return aslist;
                }
                catch(Exception ex)
                {
                    List<AS> lists;
                    return  lists = new List<AS>();
                }
            }
        }
    }


    public class AS
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DesignationName { get; set; }
        public Nullable<int> Pendency { get; set; }
        public List<DS> DSList { get; set; } = new List<DS>();
    }
   public class DS
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DesignationName { get; set; }
        public Nullable<int> Pendency { get; set; }
        public List<section> Section { get; set; } = new List<section>();
    }
    public class section
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DesignationName { get; set; }
        public Nullable<int> Pendency { get; set; }

    }

}
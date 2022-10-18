using Hrmis.Controllers.HrmisRestApi;
using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using Hrmis.Models.ViewModels.HealthFacility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Services
{
    public class DatabaseService
    {
        //get service list
        public TableResponse<Service> GetServiceList(DatabaseFilter filter, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    IQueryable<Service> query = _db.Services.Where(x => x.IsActive == true).AsQueryable();

                    var count = query.Count();
                    var list = query.OrderBy(x => x.Name).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    return new TableResponse<Service>() { Count = count, List = list };
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        //end service list
        //get HfType list
        //public TableResponse<HfTypeList> GetHfTypeList(DatabaseFilter filter, string userName, string userId)
        //{
        //    using (var _db = new HR_System())
        //    {
        //        try
        //        {
        //            _db.Configuration.ProxyCreationEnabled = false;
        //            var hfTypeList = _db.HfTypeLists.AsQueryable();

        //            var count = hfTypeList.Count();
        //            var list = hfTypeList.OrderBy(x => x.Name).Skip(filter.Skip).Take(filter.PageSize).ToList();
        //            return new TableResponse<HfTypeList>() { Count = count, List = list };

        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }
        //}
        //end get HfType list


        // get HfCategories
        public TableResponse<HFCategory> GetHfCategoryList(DatabaseFilter filter, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var hfCategoryList = _db.HFCategories.AsQueryable();

                    var count = hfCategoryList.Count();
                    var list = hfCategoryList.OrderBy(x => x.Name).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    return new TableResponse<HFCategory>() { Count = count, List = list };
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        //get HfType list
        public TableResponse<HrDesignationView> GetDesignationList(DatabaseFilter filter, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {

                    _db.Configuration.ProxyCreationEnabled = false;
                    var desigList = _db.HrDesignationViews.Where(x => x.IsActive == true).OrderBy(x => x.Name).AsQueryable();

                    var count = desigList.Count();
                    var list = desigList.OrderBy(x => x.Name).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    return new TableResponse<HrDesignationView>() { Count = count, List = list };

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


        public List<dropdown> GetDesignations()
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var desigList = _db.HrDesignations.Where(x => x.IsActive == true).OrderBy(x => x.Name).AsQueryable();
                    List<dropdown> ddl = desigList.Select(x => new dropdown {
                        Id = x.Id,
                        value = x.Name
                    }).ToList();
                    return ddl;
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
        }

        public TableResponse<HrDesignationView> GetDesigSearchList(DatabaseFilter filter, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {

                    _db.Configuration.ProxyCreationEnabled = false;
                    var desigList = _db.HrDesignationViews.Where(x => x.IsActive == true).OrderBy(x => x.Name).AsQueryable();

                    var count = desigList.Count();
                    var list = desigList.OrderBy(x => x.Name).Skip(filter.Skip).Take(filter.PageSize).ToList();
                    return new TableResponse<HrDesignationView>() { Count = count, List = list };

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        //end get HfType list
        private Service bindServiceSaveModel(Service clientService)
        {
            var service = new Service();

            //check if new or edit
            if (clientService.Id != 0) { return null; }

            //required
            if (clientService.Name == null) { return null; }
            else { service.Name = clientService.Name; }

            if (clientService.IsActive == null) { return null; }
            else { service.IsActive = clientService.IsActive; }

            return service;
        }

        //service edit function
        private Service bindServiceEditModel(Service clientService, Service service)
        {
            //check if new or edit
            // if (clientService.Id != 0) { return null; }

            if (clientService.Name == null) { return null; }
            else { service.Name = clientService.Name; }

            if (clientService.IsActive == null) { return null; }
            else { service.IsActive = clientService.IsActive; }

            return service;
        }

        //add service
        public Service addService(Service s, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                        try
                        {
                            if (s.Id == 0)
                            {
                                if (_db.Services.Any(x => x.Name == s.Name))
                                {
                                    // Handle duplicate data error here
                                    return null;

                                }
                                else
                                {
                                    var savableService = bindServiceSaveModel(s);
                                    if (savableService == null) return null;
                                    _db.Services.Add(savableService);
                                    _db.SaveChanges();
                                    return savableService;
                                }
                            }
                            else if (ServiceExists(s.Id))
                            {
                                _db.Configuration.ProxyCreationEnabled = false;
                                var dbs = _db.Services.FirstOrDefault(x => x.Id == s.Id);
                                var editables = bindServiceEditModel(s, dbs);

                                _db.Entry(editables).State = EntityState.Modified;
                                _db.SaveChanges();
                                return editables;

                            }
                        }
                        catch (Exception ex1)
                        {
                            throw;
                        }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //service already exists
        public bool ServiceExists(int Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var service = _db.Services.FirstOrDefault(x => x.Id == Id);
                    if (service == null) return false;
                    else return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //end service already exists
        public class DatabaseFilter : Paginator
        {
            public List<string> Name { get; set; }
        }
        public partial class ServiceList
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string isActive { get; set; }
        }
        // end of start brackets

        //hftype database
        public bool HfTypeExists(int Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var HfType = _db.HFTypes.FirstOrDefault(x => x.Id == Id);
                    if (HfType == null) return false;
                    else return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //bind hftype save model
        private HFType BindHfTypeSaveModel(HFType ClientHfType)
        {
            var hftype = new HFType();
            //check if new or edit
            if (ClientHfType.Id != 0) { return null; }

            //required

            if (ClientHfType.Code == null) { return null; }
            else { hftype.Code = ClientHfType.Code; }

            if (ClientHfType.Name == null) { return null; }
            else { hftype.Name = ClientHfType.Name; }

            if (ClientHfType.HFCat_Id == null) { return null; }
            else { hftype.HFCat_Id = ClientHfType.HFCat_Id; }

            return hftype;
        }

        private HFType bindHfTypeEditModel(HFType ClientHfType, HFType hftype)
        {
            //check if new or edit
            // if (clientService.Id != 0) { return null; }

            if (ClientHfType.Name == null) { return null; }
            else { hftype.Name = ClientHfType.Name; }

            if (ClientHfType.HFCat_Id == null) { return null; }
            else { hftype.HFCat_Id = ClientHfType.HFCat_Id; }

            return hftype;
        }


        public MeritActiveDesignation addmeritActiveDesignation(MeritActiveDesignation mad, string userName, string userId)
        {

            using (var _db = new HR_System())
            {
                try
                {
                    if (mad.Id == 0)
                    {
                        _db.MeritActiveDesignations.Add(mad);
                        _db.SaveChanges();
                    }
                    else
                    {
                        _db.Entry(mad).State = EntityState.Modified;
                        _db.SaveChanges();
                    }


                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            return null;
        }


        //add hftype function
        public HFType addHfType(HFType hft, string userName, string userId)
        {

            using (var _db = new HR_System())
            {
                    try
                    {
                        if (hft.Id == 0)
                        {
                            if (_db.HFTypes.Any(x => x.Name == hft.Name))
                            {
                                // Handle error here
                                return null;
                                //  return Ok("Invalid");
                            }

                            else
                            {
                                var hftype_code = _db.HFTypes.OrderByDescending(x => x.Code).FirstOrDefault();

                                int hftcode = Convert.ToInt32(hftype_code.Code);
                                hftcode = hftcode + 1;
                                string hftc = "0" + hftcode;
                                hft.Code = hftc;

                                var savablehft = BindHfTypeSaveModel(hft);
                                if (savablehft == null) return null;
                                _db.HFTypes.Add(savablehft);
                                _db.SaveChanges();
                                return savablehft;
                            }
                        }

                        else if (HfTypeExists(hft.Id))
                        {
                            _db.Configuration.ProxyCreationEnabled = false;
                            var dbhft = _db.HFTypes.FirstOrDefault(x => x.Id == hft.Id);
                            var editables = bindHfTypeEditModel(hft, dbhft);

                            _db.Entry(editables).State = EntityState.Modified;
                            _db.SaveChanges();
                            return editables;

                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
            }
            return null;
        }


        public List<HFOpenedView> GetOpenHF(string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                        try
                        {
                            var hfOpenedViews = _db.HFOpenedViews.Where(x => x.MeritsActiveDesignationId == 68 && x.IsActive == true).ToList();
                            return hfOpenedViews;
                        }
                        catch (Exception ex1)
                        {
                            throw;
                        }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<AdhocHFOpenedView> GetAdhocOpenHF(string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        var hfOpenedViews = _db.AdhocHFOpenedViews.Where(x=> x.IsActive == true).ToList();
                        return hfOpenedViews;
                    }
                    catch (Exception ex1)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        public bool RemoveOpenHF(long Id, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                            var openHF = _db.HFOpenedPostings.FirstOrDefault(x => x.Id == Id);
                        try
                        {
                            if (openHF != null)
                            {
                                var entity_Lifecycle = _db.Entity_Lifecycle.FirstOrDefault(x => x.Id == openHF.EntityLifecycle_Id);
                                if (entity_Lifecycle != null)
                                {
                                    entity_Lifecycle.IsActive = false;
                                    _db.Entry(entity_Lifecycle).State = EntityState.Modified;
                                    _db.SaveChanges();

                                    Entity_Modified_Log eml = new Entity_Modified_Log();
                                    eml.Modified_By = userId;
                                    eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                                    eml.Entity_Lifecycle_Id = (long)openHF.EntityLifecycle_Id;
                                    eml.Description = "Opened HF Removed by " + userName;
                                    _db.Entity_Modified_Log.Add(eml);
                                    _db.SaveChanges();
                                    return true;
                                }
                            }
                            return false;
                        }
                        catch (Exception ex1)
                        {
                            throw;
                        }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool RemoveAdhocOpenHF(long Id, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var openHF = _db.AdhocHFOpeneds.FirstOrDefault(x => x.Id == Id);
                    if (openHF != null)
                    {
                        openHF.IsActive = false;
                        _db.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex1)
            {
                throw;
            }
        }
        public HFOpenedPosting SaveOpenedHF(HFOpenedPosting hFOpenedPosting, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                        try
                        {
                            if (hFOpenedPosting.Id == 0)
                            {
                                Entity_Lifecycle elc = new Entity_Lifecycle();
                                elc.IsActive = true;
                                elc.Created_By = userName;
                                elc.Created_Date = DateTime.UtcNow.AddHours(5);
                                elc.Users_Id = userId;
                                _db.Entity_Lifecycle.Add(elc);
                                _db.SaveChanges();

                                hFOpenedPosting.EntityLifecycle_Id = elc.Id;


                                _db.HFOpenedPostings.Add(hFOpenedPosting);
                                _db.SaveChanges();

                                return hFOpenedPosting;
                            }
                        }
                        catch (Exception ex1)
                        {
                            throw;
                        }
                    }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }




        public List<ServiceTempView> GetServiceTemp(int profileId, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                        try
                        {
                            var serviceTemps = _db.ServiceTempViews.Where(x => x.Profile_Id == profileId).ToList();
                            return serviceTemps;
                        }
                        catch (Exception ex1)
                        {
                            throw;
                        }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool RemoveServiceTemp(long Id, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                        try
                        {
                            var st = _db.ServiceTemps.Find(Id);
                            if(st != null)
                            {
                                _db.ServiceTemps.Remove(st);
                                _db.SaveChanges();
                                return true;
                            }
                            //var serviceTemp = _db.ServiceTemps.FirstOrDefault(x => x.Id == Id);
                            //if (serviceTemp != null)
                            //{
                            //    serviceTemp.IsActive = false;
                            //    _db.Entry(serviceTemp).State = EntityState.Modified;
                            //    _db.SaveChanges();
                            //    transc.Commit();
                            //    return true;
                            //}
                            return false;
                        }
                        catch (Exception ex1)
                        {
                            throw;
                        }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ServiceTemp SaveServiceTemp(ServiceTemp serviceTemp, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                        try
                        {
                            if (serviceTemp.Id == 0)
                            {
                                serviceTemp.DateTime = DateTime.UtcNow.AddHours(5);
                                serviceTemp.IsActive = true;
                                _db.ServiceTemps.Add(serviceTemp);
                                _db.SaveChanges();

                                return serviceTemp;
                            }
                        }
                        catch (Exception ex1)
                        {
                            throw;
                        }
                }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private HrDesignation bindDesignationEditModel(HrDesignation ClientDesig, HrDesignation desig)
        {
            //check if new or edit
            // if (clientService.Id != 0) { return null; }

            if (ClientDesig.Name == null) { return null; }
            else { desig.Name = ClientDesig.Name; }

            if (ClientDesig.Cadre_Id == 0) { return null; }
            else { desig.Cadre_Id = ClientDesig.Cadre_Id; }

            if (ClientDesig.HrScale_Id == 0) { return null; }
            else { desig.HrScale_Id = ClientDesig.HrScale_Id; }

            if (ClientDesig.IsActive == null) { return null; }
            else { desig.IsActive = ClientDesig.IsActive; }



            return desig;
        }

        private HrDesignation bindDesignationRemoveModel(HrDesignation ClientDesig, HrDesignation desig)
        {
            if (ClientDesig.IsActive == null) { return null; }
            else { desig.IsActive = ClientDesig.IsActive; }

            return desig;
        }

        //bind hftype save model
        private HrDesignation bindDesignationSaveModel(HrDesignation ClientDesig)
        {
            var desig = new HrDesignation();
            //check if new or edit
            if (ClientDesig.Id != 0) { return null; }

            //required

            if (ClientDesig.Code == 0) { return null; }
            else { desig.Code = ClientDesig.Code; }

            if (ClientDesig.Name == null) { return null; }
            else { desig.Name = ClientDesig.Name; }

            if (ClientDesig.Cadre_Id == 0) { return null; }
            else { desig.Cadre_Id = ClientDesig.Cadre_Id; }

            if (ClientDesig.HrScale_Id == 0) { return null; }
            else { desig.HrScale_Id = ClientDesig.HrScale_Id; }

            if (ClientDesig.IsActive == null) { return null; }
            else { desig.IsActive = ClientDesig.IsActive; }



            return desig;
        }

        public HrDesignation addDesig(HrDesignation desig, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                        if (desig.Id == 0)
                        {
                            _db.Configuration.ProxyCreationEnabled = false;
                            var designationCount = _db.HrDesignations.Where(x => x.Name.Equals(desig.Name)).Count();
                            if (designationCount > 0)
                            {
                                _db.Configuration.ProxyCreationEnabled = false;
                                var dsg = _db.HrDesignations.FirstOrDefault(x => x.Name.Equals(desig.Name));
                                dsg.IsActive = true;
                                _db.Entry(dsg).State = EntityState.Modified;
                                _db.SaveChanges();
                                return dsg;
                            }
                            var desig_code = _db.HrDesignations.OrderByDescending(x => x.Code).FirstOrDefault();

                            int dcode = Convert.ToInt32(desig_code.Code);
                            dcode = dcode + 1;
                            desig.Code = dcode;
                            desig.IsActive = true;

                            var savableDesig = bindDesignationSaveModel(desig);
                            if (savableDesig == null) return null;
                            _db.HrDesignations.Add(savableDesig);
                            _db.SaveChanges();
                            return savableDesig;

                        }

                        //else if (DesigExists(desig.Id))
                        else if (desig.Id > 0)
                        {
                            _db.Configuration.ProxyCreationEnabled = false;
                            var dbDesig = _db.HrDesignations.FirstOrDefault(x => x.Id == desig.Id);
                            desig.IsActive = true;
                            var editables = bindDesignationEditModel(desig, dbDesig);

                            _db.Entry(editables).State = EntityState.Modified;
                            _db.SaveChanges();
                            return editables;

                        }

                }
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public HrDesignation RemoveDesig(HrDesignation desig, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                        if (desig.Id > 0)
                        {
                            _db.Configuration.ProxyCreationEnabled = false;
                            desig.IsActive = false;
                            var dbDesig = _db.HrDesignations.FirstOrDefault(x => x.Id == desig.Id);
                            var editables = bindDesignationRemoveModel(desig, dbDesig);

                            _db.Entry(editables).State = EntityState.Modified;
                            _db.SaveChanges();
                            return editables;

                        }

                }
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        //hftype database
        public bool DesigExists(int Id)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var Desig = _db.HrDesignations.FirstOrDefault(x => x.Id == Id);
                    if (Desig == null) return false;
                    else return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public bool DesigExistsByName(string desigName)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var Desig = _db.HrDesignations.FirstOrDefault(x => x.Name.Equals(desigName) && x.IsActive == true);
                    if (Desig == null) return false;
                    else return true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }

    public class dropdown
    {
        public int Id { get; set; }
        public string value { get; set; }
    }
}
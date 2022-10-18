using Hrmis.Models.DbModel;
using Hrmis.Models.ViewModels.Application;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Services
{
    public class SwmoPromotionService
    {
        private UserService _userService;
        public SwmoPromotionService()
        {
            _userService = new UserService();
        }
        public SwmoPromotionViewModel AddSwmoPromtionCandidate(SwmoPromotionViewModel candidate, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    int masterid = 0;
                    _db.Configuration.ProxyCreationEnabled = false;
                    var dbcandidate = _db.SwmoPromotions.FirstOrDefault(x => x.CNIC.Equals(candidate.swmoPromotion.CNIC));
                    try
                    {
                        bool flagnew = false;
                        if (candidate.swmoPromotion == null) return null;
                        if (dbcandidate == null)
                        {
                            dbcandidate = new SwmoPromotion();
                            dbcandidate.IsActive = true;
                            dbcandidate.CreatedBy = userName;
                            dbcandidate.CreatedDate = DateTime.UtcNow.AddHours(5);
                            dbcandidate.UserId = userId;
                            flagnew = true;
                        }
                        dbcandidate.DesignationId = candidate.swmoPromotion.DesignationId;
                        dbcandidate.DistrictCode = candidate.swmoPromotion.DistrictCode;
                        dbcandidate.Rollno = candidate.swmoPromotion.Rollno;
                        dbcandidate.UploadPath = candidate.swmoPromotion.UploadPath;
                        dbcandidate.PermanentSince = candidate.swmoPromotion.PermanentSince;
                        dbcandidate.Name = candidate.swmoPromotion.Name;
                        dbcandidate.HfId = candidate.swmoPromotion.HfId;
                        dbcandidate.CNIC = candidate.swmoPromotion.CNIC;
                        dbcandidate.PhoneNumber = candidate.swmoPromotion.PhoneNumber;
                        if (flagnew)
                        {
                            _db.SwmoPromotions.Add(dbcandidate);
                        }
                        _db.SaveChanges();
                        masterid = dbcandidate.Id;
                        if (candidate.swmoPromotionDetail.Count > 0)
                        {
                            var saveSwmoPromotionDetail = AddSwmoPromotionDetail(candidate.swmoPromotionDetail, userName, userId, masterid);
                            candidate.swmoPromotionDetail = new List<SwmoPromotionDetail>();
                            candidate.swmoPromotionDetail.AddRange(saveSwmoPromotionDetail);
                        }
                        candidate.swmoPromotion.Id = masterid;
                        return candidate;

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

        public List<SwmoPromotionDetail> AddSwmoPromotionDetail(List<SwmoPromotionDetail> candidateDetail, string userName, string userId, int masterid)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var list = new List<SwmoPromotionDetail>();
                    _db.Configuration.ProxyCreationEnabled = false;
                    var dbcandidateDetail = _db.SwmoPromotionDetails.FirstOrDefault(x => x.Id.Equals(masterid));
                    foreach (var item in candidateDetail)
                    {
                        // var dbcandidateDetail = _db.SwmoPromotionDetails.FirstOrDefault(x => x.Id.Equals(item.Id));
                        try
                        {
                            bool flagnew = false;
                            //if (dbcandidateDetail == null) return null;
                            if (dbcandidateDetail == null)
                            {
                                dbcandidateDetail = new SwmoPromotionDetail();
                                dbcandidateDetail.IsActive = true;
                                dbcandidateDetail.CreatedBy = userName;
                                dbcandidateDetail.CreatedDate = DateTime.UtcNow.AddHours(5);
                                dbcandidateDetail.UserId = userId;
                                flagnew = true;
                            }
                            dbcandidateDetail.PreferenceHfId = item.PreferenceHfId;
                            dbcandidateDetail.Swmo_Promo_Id = masterid;
                            if (flagnew)
                            {
                                _db.SwmoPromotionDetails.Add(dbcandidateDetail);
                                list.Add(dbcandidateDetail);
                            }
                            else
                            {
                                var exist = _db.SwmoPromotionDetails.Find(item.PreferenceHfId);
                                if (exist == null)
                                {
                                    _db.SwmoPromotionDetails.Add(dbcandidateDetail);
                                    list.Add(dbcandidateDetail);
                                }
                                //_db.SwmoPromotionDetails.RemoveRange(_db.SwmoPromotionDetails.Where(x => x.Swmo_Promo_Id == masterid));
                                //;
                            }
                            _db.SaveChanges();
                        }
                        catch (Exception ex1)
                        {
                            throw;
                        }
                    }
                    return list;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
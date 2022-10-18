using Hrmis.Models.DbModel;
using Hrmis.Models.ViewModels.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.Services
{
    public class MeritDiplomaService
    {
        private UserService _userService;

        public MeritDiplomaService()
        {
            _userService = new UserService();
        }

        public MeritDiplomaCandidateViewModel AddMeritCandidate(MeritDiplomaCandidateViewModel candidate, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    int masterid = 0;
                    _db.Configuration.ProxyCreationEnabled = false;
                    var dbcandidate = _db.MeritDiplomaCandidates.FirstOrDefault(x => x.Id.Equals(candidate.meritCandidate.Id));
                    try
                    {
                        bool flagnew = false;
                        if (candidate.meritCandidate == null) return null;
                        if (dbcandidate == null)
                        {
                            dbcandidate = new MeritDiplomaCandidate();
                            dbcandidate.IsActive = true;
                            dbcandidate.CreatedBy = userName;
                            dbcandidate.CreatedDate = DateTime.UtcNow.AddHours(5);
                            dbcandidate.UserId = userId;
                            flagnew = true;
                        }
                            dbcandidate.DesignationId = candidate.meritCandidate.DesignationId;
                            dbcandidate.DistrictCode = candidate.meritCandidate.DistrictCode;
                            dbcandidate.ProfileId = candidate.meritCandidate.ProfileId;
                            dbcandidate.Rollno = candidate.meritCandidate.Rollno;
                            dbcandidate.UploadPath = candidate.meritCandidate.UploadPath;
                            dbcandidate.PermanentSince = candidate.meritCandidate.PermanentSince;
                            dbcandidate.Marks = candidate.meritCandidate.Marks;
                            dbcandidate.Name = candidate.meritCandidate.Name;
                            dbcandidate.HfId = candidate.meritCandidate.HfId;
                            dbcandidate.MeritId = candidate.meritCandidate.MeritId;
                            dbcandidate.CNIC = candidate.meritCandidate.CNIC;
                            dbcandidate.PhoneNumber = candidate.meritCandidate.PhoneNumber;
                            if(flagnew)
                             {
                               _db.MeritDiplomaCandidates.Add(dbcandidate);
                             }
                            _db.SaveChanges();
                            masterid = dbcandidate.Id;
                        if (candidate.meritCandidateDetail.Count > 0)
                        {
                            var saveMeritDiplomaDetail = AddMeritCandidateDetail(candidate.meritCandidateDetail, userName, userId,masterid);
                            candidate.meritCandidateDetail = new List<MeritDiplomaCandidateDetail>();
                            candidate.meritCandidateDetail.AddRange(saveMeritDiplomaDetail);
                        }
                        candidate.meritCandidate.Id = masterid;
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
        public List<MeritDiplomaCandidateDetail> AddMeritCandidateDetail(List<MeritDiplomaCandidateDetail> candidateDetail, string userName, string userId,int masterid)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var list = new List<MeritDiplomaCandidateDetail>();
                    _db.Configuration.ProxyCreationEnabled = false;
                    foreach (var item in candidateDetail)
                    {
                        var dbcandidateDetail = _db.MeritDiplomaCandidateDetails.FirstOrDefault(x => x.Id.Equals(item.Id));
                        try
                        {
                            bool flagnew = false;
                            //if (dbcandidateDetail == null) return null;
                            if (dbcandidateDetail == null)
                            {
                                dbcandidateDetail = new MeritDiplomaCandidateDetail();
                                dbcandidateDetail.IsActive = true;
                                dbcandidateDetail.CreatedBy = userName;
                                dbcandidateDetail.CreatedDate = DateTime.UtcNow.AddHours(5);
                                dbcandidateDetail.UserId = userId;
                                flagnew = true;
                            }
                            dbcandidateDetail.PreferenceHfId = item.PreferenceHfId;
                            dbcandidateDetail.Merit_Dip_Cand_Id = masterid;
                            if (flagnew)
                            {
                                _db.MeritDiplomaCandidateDetails.Add(dbcandidateDetail);
                                list.Add(dbcandidateDetail);
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
using Hrmis.Models.DbModel;
using Hrmis.Models.ViewModels.Application;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hrmis.Models.Services
{
    public class PublicService
    {
        public ApplicationFts GetApplication(int Tracking, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    ApplicationFts applicationFts = new ApplicationFts();
                    FilesACRService filesACRService = new FilesACRService();
                    var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                    if (currentOfficer != null)
                    {
                        applicationFts.application = _db.ApplicationViews.FirstOrDefault(x => x.TrackingNumber == Tracking && x.IsActive == true);
                        if (applicationFts.application == null)
                        {
                            return null;
                        }
                        applicationFts.applicationAttachments = _db.AttachmentViews.Where(x => x.Application_Id == applicationFts.application.Id).ToList();
                        return applicationFts;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public ApplicationFts GetApplicationData(int Id, string Type, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    ApplicationFts applicationFts = new ApplicationFts();
                    FilesACRService filesACRService = new FilesACRService();
                    if (Type.Equals("logs"))
                    {
                        applicationFts.applicationLogs = _db.ApplicationLogViews.Where(x => x.Application_Id == Id).OrderBy(x => x.DateTime).ToList();
                        return applicationFts;
                    }
                    else if (Type.Equals("oldlogs"))
                    {
                        applicationFts.applicationForwardLogs = _db.ApplicationForwardLogs.Where(x => x.Application_Id == Id).OrderByDescending(x => x.DateTime).ToList();
                        return applicationFts;
                    }
                    else if (Type.Equals("parliamentarian"))
                    {
                        applicationFts.application = _db.ApplicationViews.FirstOrDefault(x => x.Id == Id && x.IsActive == true);
                        applicationFts.applicationPersonAppeared = _db.ApplicationPersonAppeareds.FirstOrDefault(x => x.Id == applicationFts.application.PersonAppeared_Id);
                        return applicationFts;
                    }
                    else if (Type.Equals("file"))
                    {
                        applicationFts.application = _db.ApplicationViews.FirstOrDefault(x => x.Id == Id && x.IsActive == true);
                        var file = new List<DDS_Files>();
                        if (!string.IsNullOrEmpty(applicationFts.application.FileNumber))
                        {
                            file = filesACRService.GetDDSFilesByFileNumber(applicationFts.application.FileNumber);
                        }
                        if (file.Count == 0)
                        {
                            file = filesACRService.GetDDSFilesByCNIC(applicationFts.application.CNIC);
                        }
                        if (file.Count == 0)
                        {
                            file = filesACRService.GetDDSFilesByName(applicationFts.application.EmployeeName);
                        }
                        if (file.Count > 0)
                        {
                            applicationFts.File = file.FirstOrDefault();
                        }
                        return applicationFts;
                    }
                    else if (Type.Equals("filereqs"))
                    {
                        applicationFts.applicationFileRecositions = _db.ApplicationFileReqViews.Where(x => x.Application_Id == Id && x.IsActive == true).ToList();
                        return applicationFts;
                    }
                    else if (Type.Equals("applicationattachments"))
                    {
                        applicationFts.applicationAttachments = _db.AttachmentViews.Where(x => x.Application_Id == Id).ToList();
                        return applicationFts;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }


        public List<AdhocApplicantQualificationView> GetApplicantQualification(int applicantId, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        var ApplicantQualification = _db.AdhocApplicantQualificationViews.Where(x => x.Applicant_Id == applicantId && x.IsActive == true).OrderByDescending(k => k.DegreeFrom).ToList();
                        return ApplicantQualification;
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
        public AdhocApplicantQualification SaveApplicantQualification(AdhocApplicantQualification ApplicantQualification, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    try
                    {
                        if (ApplicantQualification.Id == 0)
                        {
                            _db.Configuration.ProxyCreationEnabled = false;

                            ApplicantQualification.IsActive = true;
                            ApplicantQualification.CreatedBy = userName;
                            ApplicantQualification.CreatedDate = DateTime.UtcNow.AddHours(5);
                            ApplicantQualification.UserId = userId;
                            _db.AdhocApplicantQualifications.Add(ApplicantQualification);
                            _db.SaveChanges();
                            return ApplicantQualification;
                        }
                        return new AdhocApplicantQualification();
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

        public bool RemoveApplicantQualification(long Id, string userName, string userId)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var ApplicantQualification = _db.AdhocApplicantQualifications.FirstOrDefault(x => x.Id == Id);
                    try
                    {
                        if (ApplicantQualification != null)
                        {
                            ApplicantQualification.IsActive = false;
                            _db.SaveChanges();
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

        public List<MobileTracking> GetApplicationTrack(int Id)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var app = _db.ApplicationViews.FirstOrDefault(x => x.Id == Id);

                    int? trackingNumber = 0;

                    if (app != null)
                    {
                        trackingNumber = app.TrackingNumber;
                    }
                    var logs = _db.ApplicationLogViews.Where(x => x.Application_Id == Id).OrderBy(x => x.DateTime).ToList();

                    List<MobileTracking> tracks = new List<MobileTracking>();
                    if (trackingNumber != null)
                    {
                        if (trackingNumber > 49572)
                        {
                            foreach (var log in logs)
                            {
                                var track = new MobileTracking();
                                track.Id = log.Id;
                                track.DateTime = log.DateTime;
                                track.Status = log.ToStatusName == "No Process Initiated" ? "Under Process" : log.ToStatusName;
                                if (log.Action_Id == 2)
                                {
                                    track.Information = log.StatusByDesignation + " " + log.ActionName + " application to " + log.ToOfficerDesignation;
                                }
                                else if (log.Action_Id == 14)
                                {
                                    track.Information = log.ToOfficerDesignation + " " + log.ActionName + " files from " + log.FromOfficerDesignation;
                                }
                                else if (log.Action_Id == 15)
                                {
                                    track.Information = log.ActionName + " by " + log.StatusByDesignation;
                                }
                                else if (log.Action_Id == 5)
                                {
                                    track.Information = log.ActionName + " by " + log.FileRequestByDesignation;
                                }
                                else if (log.Action_Id == 8)
                                {
                                    track.Information = log.ActionName + " for " + log.FileRequestByDesignation + " at Central Record Room";
                                }
                                else if (log.Action_Id == 6)
                                {
                                    track.Information = log.ActionName + " to " + log.afrLogByDesignation + " by Central Record Room";
                                }
                                else if (log.Action_Id == 7)
                                {
                                    track.Information = log.ActionName + " to Central Record Room by " + log.afrLogByDesignation;
                                }
                                else if (log.Action_Id == 3)
                                {
                                    track.Information = log.FromOfficerDesignation + " " + log.ActionName + " files to " + log.ToOfficerDesignation;
                                }
                                else if (log.Action_Id == 4)
                                {
                                    if (!string.IsNullOrEmpty(log.FromOfficerDesignation) && log.FromOfficerDesignation.Equals("Facilitation Centre (HISDU)"))
                                    {
                                        track.Information = "Facilitation Centre (HISDU) Marked application to " + log.ToOfficerDesignation;
                                    }
                                    else
                                    {
                                        track.Information = log.StatusByDesignation + " " + log.ToStatusName + " application";
                                    }
                                }
                                else if (log.Action_Id == 9)
                                {
                                    track.Information = log.StatusByDesignation + " " + log.ToStatusName + " application";
                                }
                                tracks.Add(track);
                            }
                        }
                        else if (trackingNumber <= 49572)
                        {
                            foreach (var log in logs)
                            {
                                var track = new MobileTracking();

                                track.Id = log.Id;
                                track.DateTime = log.DateTime;
                                track.Status = log.ToStatusName == "No Process Initiated" ? "Recieved" : log.ToStatusName;

                                if (!string.IsNullOrEmpty(log.FromOfficerDesignation))
                                {
                                    track.Information = log.FromOfficerDesignation + " forwarded files to " + log.ToOfficerDesignation;
                                }
                                else if (!string.IsNullOrEmpty(log.StatusByDesignation))
                                {
                                    track.Information = "Status updated by" + log.StatusByDesignation;
                                }
                                tracks.Add(track);
                            }
                        }
                    }
                    return tracks;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

    }
}
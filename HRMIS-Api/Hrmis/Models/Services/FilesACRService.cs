using Hrmis.Models.Common;
using Hrmis.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Hrmis.Models.Services
{
    public class FilesACRService
    {

        public DDS_Files AddFile(DDS_Files ddsFile, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    if (ddsFile.Id == 0)
                    {
                        ddsFile = MapFilesUpdatedToDDSFile(ddsFile);
                        //ddsFile.Date 
                        ddsFile.F_Created_Date = DateTime.UtcNow.AddHours(5);
                        ddsFile.Date = ddsFile.F_Created_Date;
                        ddsFile.F_Created_By = userName;
                        ddsFile.F_Users_Id = userId;
                        ddsFile.F_IsActive = true;

                        //if (ddsFile.EntityLifecycle_Id == null)
                        //{
                        //    Entity_Lifecycle elc = new Entity_Lifecycle();
                        //    elc.Created_Date = DateTime.UtcNow.AddHours(5);
                        //    elc.Created_By = userName + " (added after migration)";
                        //    elc.Users_Id = userId;
                        //    elc.IsActive = true;
                        //    elc.Entity_Id = 296;
                        //    _db.Entity_Lifecycle.Add(elc);
                        //    _db.SaveChanges();
                        //    ddsFile.EntityLifecycle_Id = elc.Id;
                        //}

                        _db.DDS_Files.Add(ddsFile);
                        _db.SaveChanges();
                        ddsFile.RequestId = ddsFile.Id;
                        _db.SaveChanges();
                        return ddsFile;
                    }
                    else
                    {
                        ddsFile = MapFilesUpdatedToDDSFile(ddsFile);
                        if (ddsFile.RequestId == null) { ddsFile.RequestId = ddsFile.Id; }
                        //if (ddsFile.EntityLifecycle_Id == null)
                        //{
                        //    Entity_Lifecycle elc = new Entity_Lifecycle();
                        //    elc.Created_Date = DateTime.UtcNow.AddHours(5);
                        //    elc.Created_By = userName + " (added after migration)";
                        //    elc.Users_Id = userId;
                        //    elc.IsActive = true;
                        //    elc.Entity_Id = 296;
                        //    _db.Entity_Lifecycle.Add(elc);
                        //    _db.SaveChanges();
                        //    ddsFile.EntityLifecycle_Id = elc.Id;
                        //}
                        //Entity_Modified_Log eml = new Entity_Modified_Log();
                        //eml.Modified_By = userId;
                        //eml.Modified_Date = DateTime.UtcNow.AddHours(5);
                        //eml.Entity_Lifecycle_Id = (long)ddsFile.EntityLifecycle_Id;
                        //eml.Description = "DDS File modified by " + userName;
                        //_db.Entity_Modified_Log.Add(eml);
                        //_db.SaveChanges();

                        _db.Entry(ddsFile).State = EntityState.Modified;
                        _db.SaveChanges();
                        return ddsFile;
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return null;
        }

        public FileMoveMasterView GetFileMoveMaster(int MID, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var fileMoveMaster = _db.FileMoveMasterViews.FirstOrDefault(x => x.MID_Number == MID && x.IsActive == true);
                    return fileMoveMaster;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public FileMoveMaster SubmitFileMovement(FileMoveMaster fileMoveMaster, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                using (var transc = _db.Database.BeginTransaction())
                {
                    try
                    {
                        if (fileMoveMaster.Id == 0)
                        {
                            _db.Configuration.ProxyCreationEnabled = false;
                            var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                            if (currentOfficer == null) return null;
                            if (fileMoveMaster.FileMoveDetails.Count == 0) return null;
                            fileMoveMaster.FromOfficer_Id = currentOfficer.Id;
                            fileMoveMaster.DateTime = DateTime.UtcNow.AddHours(5);
                            fileMoveMaster.CreatedBy = userName;
                            fileMoveMaster.User_Id = userId;
                            fileMoveMaster.IsActive = true;
                            fileMoveMaster.FileType_Id = 2;
                            fileMoveMaster.IsRecieved = false;

                            _db.FileMoveMasters.Add(fileMoveMaster);
                            _db.SaveChanges();

                            fileMoveMaster.MID_Number = fileMoveMaster.Id + 1001;
                            _db.Entry(fileMoveMaster).State = EntityState.Modified;
                            _db.SaveChanges();

                            foreach (var fileMoveDetail in fileMoveMaster.FileMoveDetails.ToList())
                            {
                                FileMoveDetail fileMoveDetailForDb = new FileMoveDetail();

                                var fileUpdated = _db.FilesUpdateds.FirstOrDefault(x => fileMoveDetail.FileUpdated_Id == x.Id && x.IsActive == true);
                                if (fileUpdated != null)
                                {
                                    fileMoveDetail.FileUpdated_Id = fileUpdated.Id;
                                }
                                var ddsFile = _db.DDS_Files.FirstOrDefault(x => fileMoveDetail.DDS_Id == x.Id);
                                if (ddsFile != null)
                                {
                                    fileMoveDetail.DDS_Id = ddsFile.Id;
                                }
                                var fileRequest = _db.ApplicationFileRecositions.FirstOrDefault(x => fileMoveDetail.FileRequisition_Id == x.Id && x.IsActive == true);
                                if (fileRequest != null)
                                {
                                    fileMoveDetail.FileRequisition_Id = fileRequest.Id;
                                }
                                fileMoveDetail.Master_Id = fileMoveMaster.Id;
                                fileMoveDetail.IsActive = true;
                                _db.FileMoveDetails.Add(fileMoveDetail);
                                _db.SaveChanges();
                            }
                            transc.Commit();
                            return fileMoveMaster;
                        }
                        else if (fileMoveMaster.Id > 0)
                        {
                            _db.Configuration.ProxyCreationEnabled = false;
                            var currentOfficer = _db.P_SOfficers.FirstOrDefault(x => x.User_Id == userId);
                            if (currentOfficer == null) return null;
                            var fileMoveMasterDb = _db.FileMoveMasters.FirstOrDefault(x => x.Id == fileMoveMaster.Id);
                            fileMoveMasterDb.IsRecieved = true;
                            fileMoveMasterDb.RecievedTime = DateTime.UtcNow.AddHours(5);
                            _db.SaveChanges();

                            foreach (var fileMoveDetail in _db.FileMoveDetails.Where(x => x.Master_Id == fileMoveMasterDb.Id).ToList())
                            {
                                var app = _db.ApplicationMasters.FirstOrDefault(x => x.Id == fileMoveDetail.Application_Id);
                                if (app != null)
                                {
                                    if (app.Status_Id == 11)
                                    {
                                        app.Status_Id = 10;
                                        app.StatusTime = fileMoveMasterDb.RecievedTime;
                                        app.StatusByOfficer_Id = currentOfficer.Id;
                                        app.StatusByOfficerName = currentOfficer.DesignationName;
                                    }

                                    var appLog = new ApplicationLog();
                                    appLog.Application_Id = app.Id;
                                    appLog.Action_Id = 14;

                                    if (app.Status_Id == 11)
                                    {
                                        appLog.FromStatus_Id = 11;
                                        appLog.FromStatus = "Marked";

                                        appLog.ToStatus_Id = 10;
                                        appLog.ToStatus = "No Process Initiated";

                                        appLog.StatusByOfficer_Id = currentOfficer.Id;
                                        appLog.StatusByOfficer = currentOfficer.DesignationName;
                                    }

                                    var fromOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == fileMoveMaster.FromOfficer_Id);
                                    var toOfficer = _db.P_SOfficers.FirstOrDefault(x => x.Id == fileMoveMaster.ToOfficer_Id);

                                    appLog.FromOfficer_Id = fromOfficer.Id;
                                    appLog.FromOfficerName = fromOfficer.DesignationName;
                                    appLog.ToOfficer_Id = toOfficer.Id;
                                    appLog.ToOfficerName = toOfficer.DesignationName;
                                    appLog.IsReceived = true;
                                    appLog.ReceivedTime = fileMoveMasterDb.RecievedTime;
                                    appLog.DateTime = fileMoveMasterDb.RecievedTime;
                                    appLog.CreatedBy = userName;
                                    appLog.User_Id = userId;
                                    appLog.IsActive = true;
                                    _db.ApplicationLogs.Add(appLog);
                                    _db.SaveChanges();

                                    app.IsPending = false;
                                    app.RecieveTime = fileMoveMasterDb.RecievedTime;
                                    app.CurrentLog_Id = appLog.Id;
                                    _db.Entry(app).State = EntityState.Modified;
                                    _db.SaveChanges();

                                    if (app.DDS_Id != null && app.DDS_Id != 0)
                                    {
                                        fileMoveDetail.DDS_Id = app.DDS_Id;
                                    }
                                    if (app.FileRequest_Id != null && app.FileRequest_Id != 0)
                                    {
                                        fileMoveDetail.FileRequisition_Id = app.FileRequest_Id;
                                    }
                                    //_db.FileMoveDetails.Add(fileMoveDetail);
                                    _db.Entry(fileMoveDetail).State = EntityState.Modified;
                                    _db.SaveChanges();
                                }
                            }
                            transc.Commit();
                            return fileMoveMasterDb;
                        }
                        return null;
                    }
                    catch (Exception ex)
                    {
                        transc.Rollback();
                        throw;
                    }
                }
            }
        }
        public TableResponse<FilesUpdated> GetFiles(FilesACRsFilter filters, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var query = _db.FilesUpdateds.Where(x => x.IsActive == true).AsQueryable();

                    if (!string.IsNullOrEmpty(filters.Query))
                    {
                        if (new RootService().IsCNIC(filters.Query))
                        {
                            filters.Query = filters.Query.Replace("-", "");
                            query = query.Where(x => x.CNIC.Equals(filters.Query)).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.FileNumber.ToLower().Contains(filters.Query.ToLower()) || x.Name.ToLower().Contains(filters.Query.ToLower())).AsQueryable();
                        }
                    }
                    var count = query.Count();
                    var list = query.OrderByDescending(x => x.CNIC).Skip(filters.Skip).Take(filters.PageSize).ToList();

                    return new TableResponse<FilesUpdated>() { Count = count, List = list };
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public FilesUpdateView GetFileByCodeBar(string code)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    int barcode = 0;
                    try
                    {
                        barcode = Convert.ToInt32(code);
                    }
                    catch (Exception)
                    {

                    }
                    var file = _db.FilesUpdateViews.FirstOrDefault(x => x.Id == barcode && x.IsActive == true);
                    return file;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<FilesUpdated> GetFilesByCNIC(string cnic)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var files = _db.FilesUpdateds.Where(x => x.CNIC.Equals(cnic) && x.IsActive == true).ToList();
                    return files;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<FilesUpdated> GetFilesByName(string name)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var files = _db.FilesUpdateds.Where(x => x.Name.ToLower().Equals(name.ToLower()) && x.IsActive == true).ToList();
                    return files;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<FilesUpdated> GetFilesByFileNumber(string fileNumber)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var files = _db.FilesUpdateds.Where(x => x.FileNumber.ToLower().Equals(fileNumber.ToLower()) && x.IsActive == true).ToList();
                    if (files.Count == 0)
                    {
                        files = _db.FilesUpdateds.Where(x => (x.FileNumber.ToLower().StartsWith(fileNumber.ToLower()) || x.FileNumber.ToLower().Contains(fileNumber.ToLower())) && x.IsActive == true).OrderByDescending(x => x.FileNumber).Skip(0).Take(60).ToList();
                    }
                    return files;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public TableResponse<DDSView> GetDDSFiles(FilesACRsFilter filters, int fileType, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var query = _db.DDSViews.AsQueryable();

                    if (fileType != 0)
                    {
                        if (fileType == 1)
                        {
                            query = query.Where(x => x.F_FileType_Id == 1).AsQueryable();
                        }
                        else if (fileType == 2)
                        {
                            query = query.Where(x => x.F_FileType_Id == null || x.F_FileType_Id == 2).AsQueryable();
                        }
                        else if (fileType == 3)
                        {
                            query = query.Where(x => x.F_FileType_Id == 3).AsQueryable();
                        }
                    }
                    DateTime date;
                    string[] formats = { "dd/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "d/MM/yyyy", "dd/MM/yy", "dd/M/yy", "d/M/yy", "d/MM/yy",
                             "dd.MM.yyyy", "dd.M.yyyy", "d.M.yyyy", "d.MM.yyyy", "dd.MM.yy", "dd.M.yy", "d.M.yy", "d.MM.yy"
                        };
                    if (!string.IsNullOrEmpty(filters.Query))
                    {
                        if (new RootService().IsCNIC(filters.Query))
                        {
                            filters.Query = filters.Query.Replace("-", "");
                            query = query.Where(x => x.F_CNIC.Equals(filters.Query) || x.FileType.Equals(filters.Query)).AsQueryable();
                        }
                        else if (DateTime.TryParseExact(filters.Query, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                        {
                            query = query.Where(x =>
                            DbFunctions.TruncateTime(x.DateOfBirth) == DbFunctions.TruncateTime(date.Date) ||
                            DbFunctions.TruncateTime(x.F_DateOfBirth) == DbFunctions.TruncateTime(date.Date) ||
                            DbFunctions.TruncateTime(x.DateOfJoining) == DbFunctions.TruncateTime(date.Date) ||
                            DbFunctions.TruncateTime(x.F_DateOfJoining) == DbFunctions.TruncateTime(date.Date)).AsQueryable();
                        }
                        else
                        {
                            query = query.Where(x => x.Subject.ToLower().Contains(filters.Query.ToLower()) || x.DiaryNo.ToLower().Contains(filters.Query.ToLower()) || x.F_FileNumber.ToLower().Contains(filters.Query.ToLower()) || x.F_Name.ToLower().Contains(filters.Query.ToLower()) || x.RequestId.ToString().Equals(filters.Query.ToLower())).AsQueryable();
                        }
                    }
                    var count = query.Count();
                    var list = query.OrderBy(x => Guid.NewGuid()).Skip(filters.Skip).Take(filters.PageSize).ToList();

                    return new TableResponse<DDSView>() { Count = count, List = list };
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public TableResponse<DDSDetailsView> GetDDSDetails(FilesACRsFilter filters, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var dds = _db.DDS_Files.FirstOrDefault(x => x.Id == filters.DDsId);
                    var list = _db.DDSDetailsViews.Where(x => x.DDS_Id == filters.DDsId && x.IsActive == true)
                        .OrderBy(x => x.FromPeriod).ToList();

                    if (dds == null) return null;
                    if (dds.F_DateOfJoining == null) return null;

                    var result = CalculateGapPeriod(dds, list);


                    return new TableResponse<DDSDetailsView>() { List = result };
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private List<DDSDetailsView> CalculateGapPeriod(DDS_Files ddsFile, List<DDSDetailsView> ddsDetails)
        {
            List<DDSDetailsView> list = new List<DDSDetailsView>();
            int startYear = ddsFile.F_DateOfJoining.Value.Year;
            int endYear = DateTime.Now.Year;
            if (ddsFile.F_DateOfBirth.Value.AddYears(60) <= DateTime.Now)
            {
                endYear = ddsFile.F_DateOfBirth.Value.AddYears(60).Year;
            }

            DateTime startDate = ddsFile.F_DateOfJoining.Value, endDate;
            for (int i = startYear; i <= endYear; i++)
            {
                var currentYearDDs = ddsDetails.Where(x => x.FromPeriod.Value.Year == i && x.ToPeriod.Value.Year == i).ToList();

                if (currentYearDDs.Count == 0) // if no record found of this year
                {
                    list.Add(new DDSDetailsView() { FromPeriod = startDate, ToPeriod = new DateTime(i, 12, 31) });
                    startDate = new DateTime(i, 12, 31).AddDays(1);
                    continue;
                }

                var count = 0;
                foreach (var item in currentYearDDs) // if acr records exists of this year
                {
                    count++;
                    if (startDate < item.FromPeriod && item.FromPeriod.Value.Year == i) // if gap exists between From Period and Year Start Date
                    {
                        list.Add(new DDSDetailsView() { FromPeriod = startDate, ToPeriod = item.FromPeriod.Value.AddDays(-1) });
                    }
                    startDate = item.ToPeriod.Value.AddDays(1);
                    endDate = item.ToPeriod.Value.AddDays(1);
                    list.Add(item);
                }
                if (count == currentYearDDs.Count && (startDate < new DateTime(i, 12, 31))) // if gap exists in current Year and last ACR Record Date
                {
                    list.Add(new DDSDetailsView() { FromPeriod = startDate, ToPeriod = new DateTime(i, 12, 31) });
                    startDate = new DateTime(i, 12, 31).AddDays(1);
                }
            }

            return list;
        }

        public List<DDS_Files> GetDDSFilesByName(string name)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var files = _db.DDS_Files.Where(x => x.Subject.ToLower().Equals(name.ToLower()) || x.F_Name.ToLower().Equals(name.ToLower())).ToList();
                    return files;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<DDS_Files> GetDDSFilesByCNIC(string cnic)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var files = _db.DDS_Files.Where(x => x.FileType.Equals(cnic) || x.F_CNIC.Equals(cnic)).ToList();
                    return files;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<DDS_Files> GetDDSFilesByFileNumber(string fileNumber)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var files = _db.DDS_Files.Where(x => x.DiaryNo.ToLower().Equals(fileNumber.ToLower())
                    || x.F_FileNumber.ToLower().Equals(fileNumber.ToLower())
                    || x.DiaryNo.ToLower().Contains(fileNumber.ToLower())
                    || x.F_FileNumber.ToLower().Contains(fileNumber.ToLower())
                    || x.F_CNIC.ToLower().Contains(fileNumber.ToLower())
                    || x.FileType.ToLower().Contains(fileNumber.ToLower())
                    ).ToList();
                    return files;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<LawFile> GetLawFilesByFileNumber(string fileNumber)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var files = _db.LawFiles.Where(x => x.FileNumber.ToLower().Equals(fileNumber.ToLower())
                    || x.FileNumber.ToLower().Contains(fileNumber.ToLower())
                    || x.CaseNumber.ToLower().Contains(fileNumber.ToLower())
                    || x.CourtTitle.ToLower().Contains(fileNumber.ToLower())
                    ).ToList();
                    return files;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public DDSView GetDDSFileByCodeBar(string code)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    int barcode = 0;
                    try
                    {
                        barcode = Convert.ToInt32(code);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                    var ddsFile = _db.DDSViews.FirstOrDefault(x => x.RequestId == barcode);
                    return ddsFile;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public DDSView GetDDSFileById(int Id)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var ddsFile = _db.DDSViews.FirstOrDefault(x => x.Id == Id);
                    return ddsFile;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<HrFile> GetHRFilesByName(string name)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var files = _db.HrFiles.Where(x => x.Room.ToLower().Equals(name.ToLower())).ToList();
                    return files;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<HrFile> GetHRFilesByCNIC(string cnic)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var files = _db.HrFiles.Where(x => x.CNIC.Equals(cnic)).ToList();
                    return files;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<HrFile> GetHRByFileNumber(string fileNumber)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;

                    var files = _db.HrFiles.Where(x => x.FileNo.ToLower().Equals(fileNumber.ToLower())).ToList();
                    return files;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public HrFile HrFile(HrFile hrFile, string userName, string userId)
        {
            using (var db = new HR_System())
            {
                try
                {
                    //if file already exisit
                    HrFile existingFile = db.HrFiles.FirstOrDefault(x => x.FileNo.Equals(hrFile.FileNo));

                    if (existingFile == null)
                    {
                        hrFile.Created_By = userName;
                        hrFile.User_Id = userId;
                        hrFile.Created_Date = DateTime.UtcNow.AddHours(5);
                        hrFile.IsActive = true;
                        db.HrFiles.Add(hrFile);
                        db.SaveChanges();
                    }
                    else
                    {
                        if (hrFile.Id == 0)
                        {
                            hrFile.Rack = existingFile.Rack;
                            hrFile.Row = existingFile.Row;
                        }
                        hrFile.Id = existingFile.Id;


                        existingFile.Rack = hrFile.Rack;
                        existingFile.Room = hrFile.Room;
                        existingFile.Row = hrFile.Row;
                        existingFile.FileNo = hrFile.FileNo;
                        //existingFile.Barcode = hrFile.Barcode;
                        // existingFile.EntityLifeCycleId = hrFile.EntityLifeCycleId;
                        existingFile.CNIC = hrFile.CNIC;
                        existingFile.HrProfileId = hrFile.HrProfileId;
                        // db.Entry(existingFile).State = EntityState.Modified;

                        //db.Entry(hrFile).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    return hrFile;

                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        ////----------------Files upload function------------------- ////

        public string LoadSubDirs(string dir)
        {

            string[] subdirectoryEntries = Directory.GetDirectories(dir);
            string Filedirtory = dir;
            if (subdirectoryEntries.Length > 0)
            {
                foreach (string subdirectory in subdirectoryEntries)
                {

                    LoadSubDirs(subdirectory);

                }
            }

            return Filedirtory;
        }



        public LawFile AddLawFile(LawFileDto file, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    LawFile lawFile = new LawFile();
                    lawFile.Title = file.Title;
                    lawFile.Brief = file.Brief;
                    lawFile.StayStatus = file.StayStatus;
                    lawFile.CourtTitle = file.CourtTitle;
                    lawFile.FileNumber = file.FileNumber;
                    lawFile.CaseNumber = file.CaseNumber;
                    lawFile.Rack = file.Rack;
                    lawFile.Shelf = file.Shelf;
                    lawFile.Remarks = file.Remarks;
                    lawFile.isDisposed = file.isDisposed;
                    if (file.ID == 0)
                    {
                        lawFile.CreatedOn = DateTime.UtcNow.AddHours(5);
                        lawFile.CreatedBy = userId;
                        lawFile.isDeleted = false;

                        _db.LawFiles.Add(lawFile);
                        _db.SaveChanges();

                        foreach (var petitioner in file.petitioners)
                        {
                            LawFilesPetitioner lawFilesPetitioner = new LawFilesPetitioner();
                            lawFilesPetitioner.FilesID = lawFile.ID;
                            lawFilesPetitioner.PetitionersID = petitioner.Id;
                            _db.LawFilesPetitioners.Add(lawFilesPetitioner);
                            _db.SaveChanges();
                        }

                        foreach (var respondent in file.respondents)
                        {
                            LawFilesRepresentative lawFilesRepresentative = new LawFilesRepresentative();
                            lawFilesRepresentative.FilesID = lawFile.ID;
                            lawFilesRepresentative.RepresentativesID = respondent.Id;
                            _db.LawFilesRepresentatives.Add(lawFilesRepresentative);
                            _db.SaveChanges();
                        }
                        foreach (var officer in file.officers)
                        {
                            LawFilesOfficer lawFilesOfficer = new LawFilesOfficer();
                            lawFilesOfficer.FilesID = lawFile.ID;
                            lawFilesOfficer.OfficerID = officer.Id;
                            _db.LawFilesOfficers.Add(lawFilesOfficer);
                            _db.SaveChanges();
                        }
                        return lawFile;
                    }
                    else
                    {
                        var lawFileDb = _db.LawFiles.FirstOrDefault(x => x.ID == file.ID);
                        if (lawFileDb != null)
                        {
                            lawFileDb.Title = file.Title;
                            lawFileDb.CourtTitle = file.CourtTitle;
                            lawFileDb.Brief = file.Brief;
                            lawFileDb.StayStatus = file.StayStatus;
                            lawFileDb.FileNumber = file.FileNumber;
                            lawFileDb.CaseNumber = file.CaseNumber;
                            lawFileDb.Rack = file.Rack;
                            lawFileDb.Shelf = file.Shelf;
                            lawFileDb.Remarks = file.Remarks;
                            lawFileDb.isDisposed = file.isDisposed;
                        }
                        lawFileDb.LastModifiedOn = DateTime.UtcNow.AddHours(5);
                        lawFileDb.LastModifiedBy = userId;
                        lawFileDb.isDeleted = false;
                        _db.Entry(lawFileDb).State = EntityState.Modified;
                        _db.SaveChanges();
                        return lawFileDb;
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public LawFile GetLawFile(int id)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var file = _db.LawFiles.FirstOrDefault(x => x.ID == id);
                    return file;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<LawFilesImage> GetLawFileAttachments(int fileId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var fileAttachments = _db.LawFilesImages.Where(x => x.FilesID == fileId && x.isDeleted == false).ToList();
                    return fileAttachments;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public List<DDS_Attachments> GetFileAttachments(int fileId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var fileAttachments = _db.DDS_Attachments.Where(x => x.DDs_Id == fileId && (x.Saved == null || x.Saved == true)).ToList();
                    return fileAttachments;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<LawPetitioner> GetLawFilePetitioners(int fileId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var petionerIds = _db.LawFilesPetitioners.Where(x => x.FilesID == fileId).Select(k => k.PetitionersID).ToList();
                    var petitioners = _db.LawPetitioners.Where(x => x.IsEnable == true && petionerIds.Contains(x.Id)).ToList();
                    return petitioners;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<P_SOfficers> GetLawFileOfficers(int fileId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var officerIds = _db.LawFilesOfficers.Where(x => x.FilesID == fileId).Select(k => k.OfficerID).ToList();
                    var officers = _db.P_SOfficers.Where(x => officerIds.Contains(x.Id)).ToList();
                    return officers;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public List<LawRepresentative> GetLawFileRespondants(int fileId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var representativesIds = _db.LawFilesRepresentatives.Where(x => x.FilesID == fileId).Select(k => k.RepresentativesID).ToList();
                    var representatives = _db.LawRepresentatives.Where(x => x.IsEnable == true && representativesIds.Contains(x.Id)).ToList();
                    return representatives;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public bool RemoveLawFile(int id, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var lawFile = _db.LawFiles.FirstOrDefault(x => x.ID == id);
                    if (lawFile != null)
                    {
                        lawFile.isDeleted = true;
                        lawFile.DeletedOn = DateTime.UtcNow.AddHours(5);
                        lawFile.DeletedBy = userId;
                        _db.Entry(lawFile).State = EntityState.Modified;
                        _db.SaveChanges();
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public bool DDSSouth(int id, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var dds = _db.DDS_Files.FirstOrDefault(x => x.Id == id);
                    if(dds == null)
                    {
                        return false;
                    }
                    dds.FileType_Id = 3;
                    dds.F_FileType_Id = 3;
                    _db.Entry(dds).State = EntityState.Modified;
                    _db.SaveChanges();

                    DDSSouth ddsSouth = new DDSSouth();
                    ddsSouth.DDS_Id = id;
                    ddsSouth.IsActive = true;
                    ddsSouth.Datetime = DateTime.UtcNow.AddHours(5);
                    ddsSouth.UserId = userId;
                    _db.DDSSouths.Add(ddsSouth);
                    _db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public bool DDSPSHD(int id, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var dds = _db.DDS_Files.FirstOrDefault(x => x.Id == id);
                    if (dds == null)
                    {
                        return false;
                    }
                    dds.FileType_Id = null;
                    dds.F_FileType_Id = null;
                    _db.Entry(dds).State = EntityState.Modified;
                    _db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public bool HideDuplicationFile(int id, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var dds = _db.DDS_Files.FirstOrDefault(x => x.Id == id);
                    if (dds == null)
                    {
                        return false;
                    }
                    dds.FileType_Id = 4;
                    dds.F_FileType_Id = 4;
                    _db.Entry(dds).State = EntityState.Modified;
                    _db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public bool RemoveLawFileAttachments(int id, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var lawFilesImage = _db.LawFilesImages.FirstOrDefault(x => x.ID == id);
                    if (lawFilesImage != null)
                    {
                        lawFilesImage.isDeleted = true;
                        lawFilesImage.DeletedOn = DateTime.UtcNow.AddHours(5);
                        lawFilesImage.DeletedBy = userId;
                        _db.Entry(lawFilesImage).State = EntityState.Modified;
                        _db.SaveChanges();
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public bool RemoveFileAttachments(int id, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var ddsAttachment = _db.DDS_Attachments.FirstOrDefault(x => x.Id == id);
                    if (ddsAttachment != null)
                    {
                        ddsAttachment.Saved = false;
                        _db.Entry(ddsAttachment).State = EntityState.Modified;
                        _db.SaveChanges();
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public TableResponse<LawFile> GetLawFiles(LawFileFilters filters)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    _db.Configuration.ProxyCreationEnabled = false;
                    var query = _db.LawFiles.Where(x => x.isDeleted == false).AsQueryable();
                    var searchTerm = filters.filter.Query;
                    if (string.IsNullOrEmpty(searchTerm))
                    {
                        if (!string.IsNullOrEmpty(filters.file.Title))
                        {
                            query = query.Where(x => x.Title.ToLower().Contains(filters.file.Title.ToLower())).AsQueryable();
                        }
                        if (!string.IsNullOrEmpty(filters.file.CourtTitle))
                        {
                            query = query.Where(x => x.CourtTitle.ToLower().Contains(filters.file.CourtTitle.ToLower())).AsQueryable();
                        }
                        if (!string.IsNullOrEmpty(filters.file.FileNumber))
                        {
                            query = query.Where(x => x.FileNumber.ToLower().Contains(filters.file.FileNumber.ToLower())).AsQueryable();
                        }
                        if (!string.IsNullOrEmpty(filters.file.CaseNumber))
                        {
                            query = query.Where(x => x.CaseNumber.ToLower().Contains(filters.file.CaseNumber.ToLower())).AsQueryable();
                        }
                        if (filters.file.Rack != 0)
                        {
                            query = query.Where(x => x.Rack == filters.file.Rack).AsQueryable();
                        }
                        if (filters.file.Shelf != 0)
                        {
                            query = query.Where(x => x.Shelf == filters.file.Shelf).AsQueryable();
                        }
                        if (!string.IsNullOrEmpty(filters.file.Brief))
                        {
                            query = query.Where(x => x.Brief.ToLower().Contains(filters.file.Brief.ToLower())).AsQueryable();
                        }
                        if (!string.IsNullOrEmpty(filters.file.StayStatus))
                        {
                            query = query.Where(x => x.StayStatus.ToLower().Contains(filters.file.StayStatus.ToLower())).AsQueryable();
                        }
                        if (!string.IsNullOrEmpty(filters.file.Remarks))
                        {
                            query = query.Where(x => x.Remarks.ToLower().Contains(filters.file.Remarks.ToLower())).AsQueryable();
                        }
                    }
                    else
                    {
                        if (searchTerm.Equals("PST"))
                        {
                            query = query.Where(x => x.CaseNumber.ToLower().StartsWith("app")).AsQueryable();
                        }
                        else if (searchTerm.Equals("Hight Court"))
                        {
                            query = query.Where(x => x.CaseNumber.ToLower().StartsWith("wp")
                            || x.CaseNumber.ToLower().StartsWith("w.p")
                            || x.CaseNumber.ToLower().StartsWith("w.p.")
                            || x.CaseNumber.ToLower().StartsWith("wp.")).AsQueryable();
                        }
                        else if (searchTerm.Equals("Hight Court Lahore"))
                        {
                            query = query.Where(x => (x.CaseNumber.ToLower().StartsWith("wp")
                            || x.CaseNumber.ToLower().StartsWith("w.p")
                            || x.CaseNumber.ToLower().StartsWith("w.p.")
                            || x.CaseNumber.ToLower().StartsWith("wp."))
                            && (x.CourtTitle.ToLower().Contains("lahore") || x.CourtTitle.ToLower().Contains("lhr"))
                            && !x.CourtTitle.ToLower().Contains("bahawalpur")
                            && !x.CourtTitle.ToLower().Contains("bwp")
                            && !x.CourtTitle.ToLower().Contains("rawalpindi")
                            && !x.CourtTitle.ToLower().Contains("rwp")
                            && !x.CourtTitle.ToLower().Contains("multan")
                            && !x.CourtTitle.ToLower().Contains("mtn")).AsQueryable();
                        }
                        else if (searchTerm.Equals("Hight Court Multan"))
                        {
                            query = query.Where(x => (x.CaseNumber.ToLower().StartsWith("wp")
                            || x.CaseNumber.ToLower().StartsWith("w.p")
                            || x.CaseNumber.ToLower().StartsWith("w.p.")
                            || x.CaseNumber.ToLower().StartsWith("wp."))
                            && (x.CourtTitle.ToLower().Contains("multan") || x.CourtTitle.ToLower().Contains("mtn"))
                            && !x.CourtTitle.ToLower().Contains("bahawalpur")
                            && !x.CourtTitle.ToLower().Contains("bwp")
                            && !x.CourtTitle.ToLower().Contains("rawalpindi")
                            && !x.CourtTitle.ToLower().Contains("rwp")
                            && !x.CourtTitle.ToLower().Contains("lahore")
                            && !x.CourtTitle.ToLower().Contains("lhr")).AsQueryable();
                        }
                        else if (searchTerm.Equals("Hight Court Bahawalpur"))
                        {
                            query = query.Where(x => (x.CaseNumber.ToLower().StartsWith("wp")
                            || x.CaseNumber.ToLower().StartsWith("w.p")
                            || x.CaseNumber.ToLower().StartsWith("w.p.")
                            || x.CaseNumber.ToLower().StartsWith("wp."))
                            && (x.CourtTitle.ToLower().Contains("bahawalpur") || x.CourtTitle.ToLower().Contains("bwp"))
                            && !x.CourtTitle.ToLower().Contains("multan")
                            && !x.CourtTitle.ToLower().Contains("mtn")
                            && !x.CourtTitle.ToLower().Contains("rawalpindi")
                            && !x.CourtTitle.ToLower().Contains("rwp")
                            && !x.CourtTitle.ToLower().Contains("lahore")
                            && !x.CourtTitle.ToLower().Contains("lhr")).AsQueryable();
                        }
                        else if (searchTerm.Equals("Hight Court Rawalpindi"))
                        {
                            query = query.Where(x => (x.CaseNumber.ToLower().StartsWith("wp")
                            || x.CaseNumber.ToLower().StartsWith("w.p")
                            || x.CaseNumber.ToLower().StartsWith("w.p.")
                            || x.CaseNumber.ToLower().StartsWith("wp."))
                            && (x.CourtTitle.ToLower().Contains("rawalpindi") || x.CourtTitle.ToLower().Contains("rwp"))
                            && !x.CourtTitle.ToLower().Contains("multan")
                            && !x.CourtTitle.ToLower().Contains("mtn")
                            && !x.CourtTitle.ToLower().Contains("bahawalpur")
                            && !x.CourtTitle.ToLower().Contains("bwp")
                            && !x.CourtTitle.ToLower().Contains("lahore")
                            && !x.CourtTitle.ToLower().Contains("lhr")).AsQueryable();
                        }
                        else if (searchTerm.Equals("Supreme Court"))
                        {
                            query = query.Where(x => x.CaseNumber.ToLower().StartsWith("cpla")).AsQueryable();
                        }
                        else if (searchTerm.Equals("Civil Court"))
                        {
                            query = query.Where(x => x.CourtTitle.ToLower().StartsWith("civil court")).AsQueryable();
                        }
                        else if (searchTerm.Equals("Intra Court Appeal"))
                        {
                            query = query.Where(x => x.CaseNumber.ToLower().StartsWith("ica")).AsQueryable();
                        }
                    }

                    var count = query.Count();
                    var list = query.OrderBy(x => x.Title).Skip(filters.filter.Skip).Take(filters.filter.PageSize).ToList();
                    return new TableResponse<LawFile>() { Count = count, List = list };
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public DDS_Files MapFilesUpdatedToDDSFile(DDS_Files dds)
        {

            if (dds.F_FileNumber == null) { return null; }
            dds.DiaryNo = dds.F_FileNumber;

            if (dds.F_Name == null) { return null; }
            dds.Subject = dds.F_Name;

            dds.FileType = dds.F_CNIC;

            dds.FileNIC = dds.F_NIC;

            dds.LegalDocType = "File";
            if (dds.F_DateOfBirth != null)
            {
                if (dds.F_DateOfBirth.Value.Hour == 19)
                {
                    dds.DateOfBirth = dds.F_DateOfBirth.Value.AddHours(5);
                    dds.F_DateOfBirth = dds.F_DateOfBirth.Value.AddHours(5);
                }
                else
                {
                    dds.DateOfBirth = dds.F_DateOfBirth;
                }
            }

            if (dds.F_Designation_Id != 0 && dds.F_Designation_Id == null) { dds.Designation_Id = dds.F_Designation_Id; }

            dds.FileType_Id = dds.F_FileType_Id;
            if (dds.F_DateOfJoining != null)
            {
                if (dds.F_DateOfJoining.Value.Hour == 19)
                {
                    dds.F_DateOfJoining = dds.F_DateOfJoining.Value.AddHours(5);
                }
                else
                {
                    dds.F_DateOfJoining = dds.F_DateOfJoining;
                }
            }
            return dds;

        }
    }
    public class FilesACRsFilter : Paginator
    {
        public string Name { get; set; }
        public string FileNumber { get; set; }
        public string Query { get; set; }
        public int StatusId { get; set; }
        public int MIDNumber { get; set; }
        public int DDsId { get; set; }
        public int BatchNo { get; set; }
        public string Barcode { get; set; }
        public string cnic { get; set; }
        public int type { get; set; }
        public int FromOfficer_Id { get; set; }
        public int ToOfficer_Id { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
    public class LawFileDto
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string CourtTitle { get; set; }
        public string Brief { get; set; }
        public string StayStatus { get; set; }
        public string FileNumber { get; set; }
        public string CaseNumber { get; set; }
        public int Rack { get; set; }
        public int Shelf { get; set; }
        public string Remarks { get; set; }
        public bool isDisposed { get; set; }
        public Nullable<System.DateTime> DisposedOn { get; set; }
        public string DisposedBy { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public bool isDeleted { get; set; }
        public string DeletedBy { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }

        public List<LawPetitioner> petitioners { get; set; }
        public List<LawRepresentative> respondents { get; set; }
        public List<P_SOfficers> officers { get; set; }
    }
    public class LawFileFilters
    {
        public LawFileDto file { get; set; }
        public Filter filter { get; set; }
    }
}
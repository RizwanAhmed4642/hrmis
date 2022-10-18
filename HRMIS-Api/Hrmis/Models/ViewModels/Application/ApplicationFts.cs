using Hrmis.Models.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hrmis.Models.ViewModels.Application
{
    public class ApplicationFts
    {
        public ApplicationView application { get; set; }
        public ApplicationPersonAppeared applicationPersonAppeared { get; set; }
        public List<AttachmentView> applicationAttachments { get; set; }
        public List<ApplicationLogView> applicationLogs { get; set; }
        public List<ApplicationForwardLog> applicationForwardLogs { get; set; }
        public List<MobileTracking> applicationTracks;

        public List<FilesUpdated> filesCnic { get; set; }
        public List<FilesUpdated> filesName { get; set; }
        public List<FilesUpdated> filesFileNumber { get; set; }

        public DDS_Files File { get; set; }
        public List<DDS_Files> DDS_FilesCNIC { get; set; }
        public List<DDS_Files> DDS_FilesName { get; set; }
        public List<DDS_Files> DDS_FilesFileNumber { get; set; }

        public List<HrFile> hrFileFileNumber { get; set; }
        public List<HrFile> hrFileName { get; set; }
        public List<HrFile> hrFileCNIC { get; set; }

        public List<ApplicationFileReqView> applicationFileRecositions { get; set; }
    }
}
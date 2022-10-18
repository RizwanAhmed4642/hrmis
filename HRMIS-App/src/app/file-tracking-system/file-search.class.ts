export class FileSearch {
    public name: string;
    public cnic: string;
    public fileNo: string;
    public rackNo: string;
    public shelfNo: string;
}

export class IssueFile {
    public Id: number;
    public Profile_Id: number;
    public ReqId: number;
    public IssueTo: string;
    public CNIC: string;
    public Designation: string;
    public Contact: number;
    public File_Id: number;

}

export class HrFile {
    public Id: number;
    public HrProfileId: number;
    public CNIC: string;
    public FileNo: string;
    public Barcode: number;
    public DeptName: string;
    public Room: string;
    public Status: number;
    public Rack: string;
    public Row: string;
    public EntityLifeCycleId: number;
    public FileType_Id: number;
}
export class FilesUpdated {
    public Id: number;
    public Name: string;
    public CNIC: string;
    public Designation_Id: number;
    public BPS: number;
    public BatchNo: number;
    public Section_Id: number;
    public FileNumber: string;
    public Rack: string;
    public Shelf: string;
    public DDS_Id: number;
    public NIC: string;
    public InOutStatus: string;
    public FileType_Id: number;
    public DateOfBirth: Date;
    public DateOfJoining: Date;
    public Created_Date: Date;
    public Created_By: string;
    public Users_Id: string;
    public IsActive: boolean;
}

export class DDS_Files {
    public Id: number;
    public DiaryNo: string;
    public Date: Date;
    public Subject: string;
    public Status: number;
    public RequestId: number;
    public Priority: string;
    public DepartmentName: string;
    public FileName: string;
    public FileType: string;
    public FileNIC: string;
    public MovementId: number;
    public LegalDocType: string;
    public LegalDocId: number;
    public Receiver: number;
    public DEOId: number;
    public DateOfBirth: any;
    public Designation_Id: number;
    public FileType_Id: number;
    public DateOfJoining: Date;
    public F_Name: string;
    public F_CNIC: string;
    public F_Designation_Id: number;
    public F_BPS: number;
    public F_BatchNo: number | string;
    public F_Section_Id: number;
    public F_FileNumber: string;
    public F_Rack: string;
    public F_Shelf: string;
    public F_NIC: string;
    public F_InOutStatus: string;
    public F_FileType_Id: number;
    public F_DateOfBirth: any;
    public F_DateOfJoining: any;
    public F_Created_Date: Date;
    public F_Created_By: string;
    public F_Users_Id: string;
    public F_IsActive: boolean;
    public EntityLifecycle_Id: number;
}

export class DDsDetail {
    public Id: number;
    public DDS_Id: number;
    public FromPeriod: Date;
    public ToPeriod: Date;
    public DCPDate: Date;
    public Remarks: string;
    public EntityLifecycle_Id: number;

    public Integrity: string;
    public Assessment: string;
    public AdvRemarks: string;
    public Fitness: string;
    public RORemarks: string;
    public CORemarks: string;
    public ROName: string;
    public RODesignation: string;
    public ROPosting: string;
    public COName: string;
    public CODesignation: string;
    public COPosting: string;



}
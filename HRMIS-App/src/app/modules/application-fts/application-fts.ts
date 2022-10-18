export class ApplicationFts {
}
export class ApplicationMaster {
    constructor(applicationType?: number) {
        this.ApplicationType_Id = applicationType;
        this.DateOfBirth = new Date(2000, 1, 1);
        this.AdhocExpireDate = new Date(2019, 1, 1);
        if (this.ApplicationType_Id == 1) {
            this.FromDate = new Date();
            this.ToDate = new Date();
            this.TotalDays = 1;
        }
        if (this.ApplicationType_Id == 2) {
            this.ToDept_Id = 25;
            this.toDepartmentName = 'Primary & Secondary Healthcare Department';
        }
        this.Department_Id = 25;
        this.DepartmentName = 'Primary & Secondary Healthcare Department';
        this.IsPersonAppeared = true;
        this.ApplicationPersonAppeared = new ApplicationPersonAppeared();
    }
    public Id: number;
    public TrackingNumber: number;
    public ApplicationType_Id: number;
    public ApplicationSource_Id: number;
    public Profile_Id: number;
    public Status_Id: number;
    public ForwardingOfficer_Id: number;
    public PandSOfficer_Id: number;
    public IsPending: boolean;
    public FileRequested: boolean;
    public FileRequest_Id: number;
    public PersonAppeared_Id: number;
    public RawText: string;
    public TokenNumber: number;
    public FromDate: Date;
    public ToDate: Date;
    public TotalDays: number;
    public LeaveType_Id: number;
    public ToOfficer_Id: number;
    public ToOfficerName: string;
    public CurrentScale: number;
    public OrderDate: Date;
    public RetirementType_Id: number;
    public FromHF_Id: number;
    public ToHF_Id: number;
    public FromDept_Id: number;
    public ToDept_Id: number;
    public FromDesignation_Id: number;
    public ToDesignation_Id: number;
    public Remarks: string;
    public EmployeeName: string;
    public FatherName: string;
    public CNIC: string;
    public DateOfBirth: Date;
    public Gender: string;
    public Department_Id: number;
    public Designation_Id: number;
    public HealthFacility_Id: number;
    public HfmisCode: string;
    public MobileNo: string;
    public EMaiL: string;
    public Created_Date: Date;
    public Created_By: string;
    public Users_Id: string;
    public IsActive: boolean;

    public IsPersonAppeared: boolean;
    public ComplaintType: string;
    public AdhocExpireDate: Date;
    public ToHFCode: string;
    public SeniorityNumber: string;
    public ApplicationPersonAppeared: ApplicationPersonAppeared;
    public Purpose_Id: number;

    public designationName: string;
    public fromDesignationName: string;
    public toDesignationName: string;

    public DepartmentName: string;
    public fromDepartmentName: string;
    public toDepartmentName: string;

    public toHealthFacility: string;
    public fromHealthFacility: string;
    public leaveType: string;
    public retirementTypeName: string;

    public DDS_Id: number;
    public FileNumber: string;



    public ForwardingOfficerName: string;
    public barcode: string;

    public DispatchNumber: string;
    public DispactchDated: string;
    public DispatchFrom: string;
    public DispatchSubject: string;


    public reason: string = '';
    public fromScale: number;
    public toScale: number;

    public JoiningGradeBPS: number;
    public CurrentGradeBPS: number;
    public EmpMode_Id: number;
    public EmpStatus_Id: number;
    public EmpModeName: string;
    public EmpStatusName: string;
}
export class ApplicationProfileViewModel {
    public Profile_Id: number;
    public EmployeeName: string;
    public FatherName: string;
    public CNIC: string;
    public DateOfBirth: Date;
    public JoiningDate: Date;
    public Gender: string;
    public MobileNo: string;
    public EMaiL: string;
    public Address: string;
    public Department_Id: number;
    public EmpMode_Id: number;
    public EmpStatus_Id: number;
    public CurrentGradeBPS: number;
    public JoiningGradeBPS: number;
    public SeniorityNo: string;
    public Designation_Id: number;
    public HfmisCode: string;
    public HealthFacility_Id: number;
}
export class ApplicationPersonAppeared {
    constructor() {
    }
    public Id: number;
    public Name: string;
    public CNIC: string;
    public Mobile: string;
    public DistrictCode: string;
    public DistrictName: string;
    public Reference: string;
    public Relation: string;
    public Constituency: string;
}
export class ApplicationLog {
    public Id: number;
    public Application_Id: number;
    public Remarks: string;
    public Action_Id: number;
    public SMS_SentToApplicant: boolean;
    public SMS_SentToOfficer: boolean;
    public StatusByOfficer_Id: number;
    public StatusByOfficer: string;
    public FromStatus: string;
    public ToStatus: string;
    public FromStatus_Id: number;
    public ToStatus_Id: number;
    public FromOfficerName: string;
    public ToOfficerName: string;
    public IsReceived: boolean;
    public ReceivedTime: Date;
    public FromOfficer_Id: number;
    public ToOfficer_Id: number;
    public Purpose_Id: number;
    public Purpose: string;
    public DueDate: any;
    public DateTime: Date;
    public User_Id: string;
    public CreatedBy: string;
    public IsActive: boolean;
}
export class ApplicationLogView {
    public Id: number;
    public Application_Id: number;
    public TrackingNumber: number;
    public Remarks: string;
    public Action_Id: number;
    public ActionName: string;
    public SMS_SentToApplicant: boolean;
    public SMS_SentToOfficer: boolean;
    public FromStatus_Id: number;
    public FromStatusName: string;
    public ToStatus_Id: number;
    public ToStatusName: string;
    public StatusByOfficer_Id: number;
    public StatusByName: string;
    public StatusByDesignation: string;
    public StatusByProgram: string;
    public IsReceived: boolean;
    public ReceivedTime: Date;
    public FromOfficer_Id: number;
    public FromOfficerName: string;
    public FromOfficerDesignation: string;
    public FromOfficerProgram: string;
    public ToOfficer_Id: number;
    public ToOfficerName: string;
    public ToOfficerDesignation: string;
    public ToOfficerProgram: string;
    public FileRequest_Id: number;
    public Purpose_Id: number;
    public Purpose: string;
    public DueDate: any;
    public afrLogDateTime: Date;
    public FileRequestStatusName: string;
    public FileRequestByName: string;
    public FileRequestByDesignation: string;
    public FileRequestByProgram: string;
    public afrLogUser_Id: string;
    public afrLogUserName: string;
    public FileReqLogStatusName: string;
    public afrLogByName: string;
    public afrLogByDesignation: string;
    public afrLogByProgram: string;
    public DateTime: Date;
    public User_Id: string;
    public CreatedBy: string;
    public IsActive: boolean;
}
export class ApplicationDocument {
    public Id: number;
    public Name: string;
    public Original: boolean;
    public Scanned: boolean;
    public AttachementAllow: boolean;
    public Signed: boolean;
    public OrderBy: number;
    public IsActive: boolean;
    public DateTime: Date;
    public User_Id: string;
    public CreatedBy: string;

    public show: boolean;
    public attached: boolean;
}
export class ApplicationAttachment {
    constructor() {
        this.files = [];
    }
    public Id: number;
    public Application_Id: number;
    public Document_Id: number;
    public Base64: string;
    public UploadPath: string;
    public IsBase64: boolean;

    public files: any[];
    public DocName: string;
    public documentName: string;
    public attached: boolean;
}



export class FileMoveMaster {
    public Id: number;
    public MID_Number: number;
    public FromOfficer_Id: number;
    public ToOfficer_Id: number;
    public Dispatcher_Id: number;
    public IsRecieved: boolean
    public RecievedTime: Date
    public DateTime: Date
    public CreatedBy: string;
    public User_Id: string;
    public IsActive: boolean
    public fileMoveDetails: FileMoveDetail[] = [];
}


export class FileMoveDetail {
    public Id: number;
    public Master_Id: number;
    public Application_Id: number;
    public FileUpdated_Id: number;
    public DDS_Id: number;
    public FileRequisition_Id: number;
    public IsActive: boolean;
}






export class ApplicationView {
    public Id: number;
    public ApplicationSource_Id: number;
    public TrackingNumber: number;
    public ApplicationType_Id: number;
    public IsSigned: boolean;
    public SignededAppAttachement_Id: number;
    public CurrentLog_Id: number;
    public RemarksTime: Date;
    public Status_Id: number;
    public StatusByOfficer_Id: number;
    public StatusByOfficerName: string;
    public StatusTime: Date;
    public ForwardingOfficer_Id: number;
    public FromOfficer_Id: number;
    public FromOfficerName: string;
    public ForwardTime: Date;
    public PandSOfficer_Id: number;
    public PandSOfficerName: string;
    public IsPending: boolean;
    public RecieveTime: Date;
    public FileRequested: boolean;
    public FileRequest_Id: number;
    public IsPersonAppeared: boolean;
    public PersonAppeared_Id: number;
    public RawText: string;
    public FromDate: Date;
    public ToDate: Date;
    public TotalDays: number;
    public LeaveType_Id: number;
    public CurrentScale: number;
    public RetirementType_Id: number;
    public FromHF_Id: number;
    public ToHFCode: string;
    public ToHF_Id: number;
    public FromDept_Id: number;
    public ToDept_Id: number;
    public FromDesignation_Id: number;
    public ToDesignation_Id: number;
    public ToScale: number;
    public SeniorityNumber: string;
    public AdhocExpireDate: Date;
    public ComplaintType_id: number;
    public TokenNumber: number;
    public DispatchNumber: string;
    public DispatchDated: Date;
    public DispatchFrom: string;
    public DispatchSubject: string;
    public Remarks: string;
    public EmployeeName: string;
    public FatherName: string;
    public CNIC: string;
    public DateOfBirth: Date;
    public Gender: string;
    public Department_Id: number;
    public Designation_Id: number;
    public HealthFacility_Id: number;
    public HfmisCode: string;
    public FileNumber: string;
    public MobileNo: string;
    public EMaiL: string;
    public Created_Date: Date;
    public Created_By: string;
    public Users_Id: string;
    public IsActive: boolean;
    public Profile_Id: number;
    public ForwardingOfficerName: string;
    public OrderDate: Date;
    public FileRequestTime: Date;
    public FileRequestStatus: string;
    public FileRequestStatus_Id: number;
    public ApplicationSourceName: string;
    public ApplicationTypeName: string;
    public StatusName: string;
    public PersonName: string;
    public PersonReferrence: string;
    public PersonRelation: string;
    public PersonMobile: string;
    public PersonConstituency: string;
    public PersonDistrictCode: string;
    public PersonCNIC: string;
    public PandSOfficerCode: number;
    public ToOfficer_Id: number;
    public ToOfficerName: string;
    public DepartmentName: string;
    public designationName: string;
    public HealthFacilityName: string;
    public fromHealthFacility: string;
    public fromDepartmentName: string;
    public fromDesignationName: string;
    public toHealthFacility: string;
    public toDepartmentName: string;
    public toDesignationName: string;
    public leaveType: string;
}
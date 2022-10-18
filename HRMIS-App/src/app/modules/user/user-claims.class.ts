export class UserClaims {
    public Id: number;
    public UserId: string
    public ClaimType: string
    public ClaimValue: string
}

export class User {
    public Id: string;
    public Email: string;
    public UserName: string;
    public UserDetail: string;
    public CNIC: string;
    public Password: string;
    public ConfirmPassword: string;
    public DivisionID: string;
    public DistrictID: string;
    public TehsilID: string;
    public hfmiscode: string;
    public LevelID: number;
    public DesigCode: number;
    public isActive: boolean;
    public HfmisCodeNew: string;
    public PhoneNumber: string;
    public responsibleuser: string;
    public hashynoty: string;
    public Identity: Object;
    public HfTypeCode: string;
    public Cnic: string;
    public ProfileId: number;
    public roles: string[] = [];
    public isUpdated: boolean;
    public RoleName: string;
}
export class PandSOfficerView {
    public Id: number;
    public Name: string;
    public CNIC: string;
    public FingerPrint_Id: number;
    public Contact: string;
    public DesignationName: string;
    public Program: string;
    public Code: number;
    public Designation_Id: number;
    public User_Id: string;
    public OrderBy: number;
    public HrDesignationName: string;
    public CadreName: string;
    public Scale: number;
    public UserName: string;
    public UserDetail: string;
}
export class PandSOfficerFilters {
    public Query: string;
    public add: boolean;
    public RelatedData: boolean;
    public OfficerId: number;
    public Designation_Id: number;
    public User_Id: string;
    public tableType: number;
    public concernedId: number;
    public metaData: string;
    public fpNumber: number;
    public fprint: FPPrint;
}
export class FPPrint {
    public tableType: number;
    public concernedId: number;
    public metaData: string;
    public fpNumber: number;
}
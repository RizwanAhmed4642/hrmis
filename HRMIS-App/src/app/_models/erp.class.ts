export class User {
    public Id: string;
    public UserName: string;
    public Email: string;
    public HfCode: string;
    public FirstName: string;
    public LastName: string;
    public erpModules: ErpModule[];
}
export class ErpModule {
    public Id: string;
    public Name: string;
    public Value: boolean;
    public Url: string;
    public Icon: string;
    public Badge: string;
    public OrderBy: number;
    public erpComponents: ErpComponent[];
}
export class ErpComponent {
    public Id: string;
    public Value: boolean;
    public Name: string;
    public OrderBy: number;
    public erpFields: ErpField[];
}
export class ErpField {
    public Id: string;
    public Value: boolean;
    public Name: string;
    public Module_Id: string;
}
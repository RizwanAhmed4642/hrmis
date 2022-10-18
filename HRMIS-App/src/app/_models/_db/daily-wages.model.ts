import { AttandanceRoutingModule } from "../../modules/attandance/attandance-routing.module"

export class DailyWagesContractModel {
    id:number
    wagerProfileId: number
    contractStartDate: Date
    contractEndDate :Date
    contractImagePath: ImageData
    contractStatus : string
    createdBy: string
}
export class dailyWagesAccountDetails{
    id: number
    dailyWagerProfileId: number
    bankId: number
    accountTitle: string
    accountNumber: string
}
export class dailyWagesProfile{
    id: number
    name: string
    fatherName: string
    cnic: string
    mobileNumber: string
    designation: string
    Address: string
    divisionCode: string
    DistirctCode: string
    tehsilCode: string
    hfCode : string
    UcCode: string
    HfmisCode: string
    gender: string
    personImage: ImageData
    dateOfBirth: Date
    Category:string
    EmployementMode:string
    dailyWagesAccountDetail: dailyWagesAccountDetails
    DailyWagesContractModel: DailyWagesContractModel
}

export class CoordinatesViewModel{
    lat:string
    lng:string
}
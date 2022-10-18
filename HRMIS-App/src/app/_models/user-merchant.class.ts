declare module namespace {

    export interface PEmail {
        email: string;
        status: string;
        token: string;
    }

    export interface Phone {
        phone_no: string;
        status: string;
        token: string;
    }

    export interface BasicInfo {
        phone: Phone;
        full_name: string;
        own_ref_code: string;
        password: string;
        username: string;
    }

    export interface UserMerchant {
        p_email: PEmail;
        basic_info: BasicInfo;
        _id: string;
        user_agent: string;
        ip: string;
        status: number;
        time: Date;
        ips_info: any[];
        other_emails: any[];
        contacts: any[];
        documents: any[];
        __v: number;
    }

}
import { HealthFacilityFields } from "../../health-facility/health-facility.claims";
import { ProfileFields } from "../../profile/profile.claims";

export interface Module {
    Name: string;
    Value: boolean;
    Id: string;
    Claims: Claim[];
}

export interface Claim {
    Name: string;
    Value: boolean;
    Id: string;
    Fields?: Fields[];
}

export interface Fields {
    Name: string;
    Value: boolean;
    Id: string;
}


export const Modules = [
    {
        Id: '1',
        Name: "Health Facility",
        Value: false,
        Claims: [
            { Name: "Create", Id: "11", Value: false, Fields: HealthFacilityFields },
            { Name: "View", Id: "12", Value: false, Fields: HealthFacilityFields },
            { Name: "Edit", Id: "13", Value: false, Fields: HealthFacilityFields },
            { Name: "Detail", Id: "14", Value: false, Fields: HealthFacilityFields },
            { Name: "Search", Id: "15", Value: false, Fields: HealthFacilityFields },
            { Name: "Download", Id: "16", Value: false, Fields: HealthFacilityFields },
            { Name: "Remove", Id: "17", Value: false },
            { Name: "Block", Id: "18", Value: false },
        ]
    },
    {
        Id: '2',
        Name: "Profile",
        Value: false,
        Claims: [
            { Name: "Create", Id: "21", Value: false, Fields: ProfileFields },
            { Name: "View", Id: "22", Value: false, Fields: ProfileFields },
            { Name: "Edit", Id: "23", Value: false, Fields: ProfileFields },
            { Name: "Detail", Id: "24", Value: false, Fields: ProfileFields },
            { Name: "Search", Id: "25", Value: false, Fields: ProfileFields },
            { Name: "Download", Id: "26", Value: false, Fields: ProfileFields },
            { Name: "Remove", Id: "27", Value: false },
            { Name: "Block", Id: "28", Value: false }
        ]
    },
];
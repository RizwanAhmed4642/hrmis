import { Component, OnInit } from "@angular/core";
import { cstmdummyDbTables } from "../../_models/cstmdummydata";
import { Router } from "@angular/router";

@Component({
  selector: "app-database",
  templateUrl: "./database.component.html",
  styles: [``]
})
export class DatabaseComponent implements OnInit {
  public tablesData: Array<{
    Name: string;
    Id: number;
  }> = this.getTables().slice();
  public tableSelectedValue: any = { Name: "Select Database Table", Id: 0 };
  constructor(private router: Router) { }

  ngOnInit() { }
  getTables() {
    return [
      {
        Id: 1,
        Name: "Health Facility Types"
      },
      {
        Id: 2,
        Name: "Designations"
      },
      {
        Id: 1003,
        Name: "Vacancy Profile Status"
      },
      {
        Id: 3,
        Name: "Services"
      },
      {
        Id: 4,
        Name: "Opened Facilities"
      },
      {
        Id: 5,
        Name: "Merit Active Designation"
      },
      {
        Id: 6,
        Name: "Address Coordinastes"
      }/* ,
      {
        Id: 4,
        Name: "Officers"
      },
      {
        Id: 5,
        Name: "Cadres"
      },
      {
        Id: 6,
        Name: "Order Types"
      },
      {
        Id: 7,
        Name: "Transfer Types"
      },
      {
        Id: 8,
        Name: "Leave Types"
      },
      {
        Id: 23,
        Name: "Leave Status"
      }
      ,
      {
        Id: 9,
        Name: "Application Types"
      }
      ,
      {
        Id: 10,
        Name: "Application Source"
      }
      ,
      {
        Id: 11,
        Name: "Application Status"
      }
      ,
      {
        Id: 12,
        Name: "Courses"
      },
      {
        Id: 13,
        Name: "Divisions"
      }
      ,
      {
        Id: 14,
        Name: "Districts"
      }
      ,
      {
        Id: 15,
        Name: "Tehsils"
      }
      ,
      {
        Id: 16,
        Name: "Domicile"
      }
      ,
      {
        Id: 17,
        Name: "Esr Section Officers"
      }
      ,
      {
        Id: 18,
        Name: "File Type"
      }
      ,
      {
        Id: 19,
        Name: "Grading"
      }
      ,
      {
        Id: 20,
        Name: "Grading Vals"
      }
      ,
      {
        Id: 21,
        Name: "HF Categories"
      }
      ,
      {
        Id: 22,
        Name: "HF Blocks"
      }
      ,
      {
        Id: 23,
        Name: "HF Mode"
      }
      ,
      {
        Id: 24,
        Name: "HF Types"
      }
      ,
      {
        Id: 25,
        Name: "HF Wards"
      }
      ,
      {
        Id: 26,
        Name: "Hr Department"
      }
      ,
      {
        Id: 27,
        Name: "Hr EmpMode"
      }
      ,
      {
        Id: 28,
        Name: "Hr Languages"
      }
      ,
      {
        Id: 29,
        Name: "Hr Post Type"
      }
      ,
      {
        Id: 30,
        Name: "Hr Profile Status"
      }
      ,
      {
        Id: 31,
        Name: "Hr Religion"
      }
      ,
      {
        Id: 32,
        Name: "Hr Scale"
      }
      ,
      {
        Id: 33,
        Name: "Merit Active Designations"
      }
      ,
      {
        Id: 34,
        Name: "Office"
      }
      ,
      {
        Id: 35,
        Name: "Specialization"
      } */


    ];
  }
  dropdownValueChanged({ Id, Name }) {
    if (Id == 2) {
      this.router.navigate(["/database/designations"]);
    }
    if (Id == 3) {
      this.router.navigate(["/database/services"]);
    }
    if (Id == 1003) {
      this.router.navigate(["/database/vp-profile-status"]);
    }
    if (Id == 24) {
      this.router.navigate(["/database/hftype"]);
    }
    if (Id == 21) {
      this.router.navigate(["/database/hfcategories"]);
    }
    if (Id == 4) {
      this.router.navigate(["/database/hfopen"]);
    }
    if (Id == 5) {
      this.router.navigate(["/database/meritactivedesig"]);
    }
    if (Id == 6) {
      this.router.navigate(["/database/cordsaddress"]);
    }
  }
}

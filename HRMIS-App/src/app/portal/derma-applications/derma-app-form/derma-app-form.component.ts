import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { AuthenticationService } from '../../../_services/authentication.service';
import { CookieService } from '../../../_services/cookie.service';
import { RootService } from '../../../_services/root.service';
import { DermaApplicationsService } from '../derma-applications.service';

@Component({
  selector: 'app-derma-app-form',
  templateUrl: './derma-app-form.component.html'
})
export class DermaAppFormComponent implements OnInit {
  public dermaApp: any = {};
  public cnic: string = '';
  public cnicMask: string = "00000-0000000-0";
  public portalClosed: boolean = false;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public designations: any[] = [
    { Id: 361, Name: 'Consultant (ICU)' },
    { Id: 362, Name: 'Consultant Anaesthetist' },
    { Id: 363, Name: 'Consultant Cardiac Aneasthetist' },
    { Id: 364, Name: 'Consultant Cardio Neuclear Medicine' },
    { Id: 365, Name: 'Consultant Cardiologist' },
    { Id: 366, Name: 'Consultant Chest specialist' },
    { Id: 367, Name: 'Consultant Child Specialist' },
    { Id: 368, Name: 'Consultant Dermatologist' },
    { Id: 369, Name: 'Consultant ENT Specialist' },
    { Id: 370, Name: 'Consultant Eye Surgeon' },
    { Id: 371, Name: 'Consultant Gastroenterologist' },
    { Id: 372, Name: 'Consultant Neuro Surgeon' },
    { Id: 373, Name: 'Consultant Gynaecologist' },
    { Id: 374, Name: 'Consultant Ophthalmologist' },
    { Id: 375, Name: 'Consultant Orthopaedic' },
    { Id: 376, Name: 'Consultant Orthopaedics Surgeon' },
    { Id: 377, Name: 'Consultant Paediatric Obstetric and Gynaecologist' },
    { Id: 378, Name: 'Consultant Paediatric Ophthalmologist' },
    { Id: 379, Name: 'Consultant Paediatric Physician' },
    { Id: 380, Name: 'Consultant Paediatric Psychologist' },
    { Id: 381, Name: 'Consultant Paediatrician' },
    { Id: 382, Name: 'Consultant Pathologist' },
    { Id: 383, Name: 'Consultant Physician' },
    { Id: 384, Name: 'Consultant Psychiatrist / Neuro Psychiatrist' },
    { Id: 385, Name: 'Consultant Radiologist' },
    { Id: 386, Name: 'Consultant Rehabilitation' },
    { Id: 387, Name: 'Consultant Surgeon' },
    { Id: 388, Name: 'Consultant TB/Chest Specialist' },
    { Id: 389, Name: 'Consultant Thoracic Surgeon' },
    { Id: 390, Name: 'Consultant Urologist' },
    { Id: 1586, Name: 'Consultant Anaesthetist (Burn Unit)' },
    { Id: 1587, Name: 'Consultant Audiologist' },
    { Id: 1588, Name: 'Consultant Chest Surgeon' },
    { Id: 1589, Name: 'Consultant Child Psychiatrist' },
    { Id: 1590, Name: 'Consultant ENT surgeon' },
    { Id: 1591, Name: 'Consultant Forensic Expert' },
    { Id: 1592, Name: 'Consultant Haemotologist' },
    { Id: 1593, Name: 'Consultant Histopathologist' },
    { Id: 1594, Name: 'Consultant Nephrologist' },
    { Id: 1595, Name: 'Consultant Neuclear Cardiologist' },
    { Id: 1596, Name: 'Consultant Neuro Physiologist' },
    { Id: 1597, Name: 'Consultant Neuro Radiologist' },
    { Id: 1598, Name: 'Consultant Neurologist' },
    { Id: 1599, Name: 'Consultant Paediatric Pathologist' },
    { Id: 1600, Name: 'Consultant Physician Rehabilitation' },
    { Id: 1601, Name: 'Consultant Plastic Surgeon' },
    { Id: 2136, Name: 'Consultant TB/Chest Specialist' },
    { Id: 2149, Name: 'Consultant Psychiatrist' },
    { Id: 802, Name: 'Medical Officer' },
    { Id: 1320, Name: 'Women Medical Officer' },
    { Id: 2404, Name: 'Medical Officer / Women Medical Officer' },
    { Id: 1085, Name: 'Senior Medical Officer' },
    { Id: 1157, Name: 'Senior Women Medical Officer' },
    { Id: 21, Name: 'Additional Principal Medical Officer' },
    { Id: 22, Name: 'Additional Principal Women Medical Officer' },
    { Id: 932, Name: 'Principal Medical Officer' },
    { Id: 936, Name: 'Principal Women Medical Officer' },
  ];
  public Courses: any[] = [
    { Id: 1, Name: 'MS in Dermatology / Dermatosurgery'},
    { Id: 2, Name: 'Master in Public Health'},
    { Id: 3, Name: 'Both'},
  ];
  public savingApplication: boolean = false;
  public filteredDesigs: any[] = [];
  constructor(
    private route: ActivatedRoute,
    private _cookieService: CookieService,
    private router: Router,
    private _rootService: RootService,
    private _dermaAppService: DermaApplicationsService,
    private _authenticationService: AuthenticationService,
  ) { }

  ngOnInit() {
    this.filteredDesigs = this.designations;
  }

  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'designation') {
      this.dermaApp.Designation = value.Name;
    }
    if (filter == 'course') {
      this.dermaApp.Course = value.Name;
    }
  }

  public onSubmit() {
    this.savingApplication = true;
    console.log('dermaApp: ', this.dermaApp);
    this._dermaAppService.saveDermaApplication(this.dermaApp).subscribe((res: any) => {
      if (res) {
        alert('Record Saved Successfully');
        this.resetForm();
      }
    }, err => {
      alert('Record not saved..!!');
      console.log('Saving Error: ', err);
    });
  }

  public resetForm() {
    this.dermaApp = {};
    window.location.reload();
  }

  handleFilter(value) {
    this.designations = this.filteredDesigs.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }
}

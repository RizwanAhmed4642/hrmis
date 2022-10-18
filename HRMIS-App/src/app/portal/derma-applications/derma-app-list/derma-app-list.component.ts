import { Component, OnInit } from '@angular/core';
import { Subject } from 'rxjs/Subject';

@Component({
  selector: 'app-derma-app-list',
  templateUrl: './derma-app-list.component.html'
})
export class DermaAppListComponent implements OnInit {

  public searchTerm: string = '';
  public inputChange: Subject<any>;
  public dermaApp: any = {};
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
  public filteredDesigs: any[] = [];

  constructor() { }

  handleFilter(value) {
    this.designations = this.filteredDesigs.filter(
      (s) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1
    );
  }

  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'designation') {
      this.dermaApp.Designation = value.Name;
    }
  }

  ngOnInit() {
  }

}

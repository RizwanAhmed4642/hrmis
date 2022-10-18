import { Component, OnInit } from '@angular/core';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { RootService } from '../../../_services/root.service';

@Component({
  selector: 'app-adhoc-new',
  templateUrl: './adhoc-new.component.html',
  styles: []
})
export class AdhocNewComponent implements OnInit {
  public dropDowns: DropDownsHR = new DropDownsHR();
  public newAd: any = {};
  public saved: boolean = false;
  public selectedDesignations: any[] = [];
  constructor(private _rootService: RootService) { }

  ngOnInit() {
    this.getDesignations();
  }
  public getDesignations() {
    this._rootService.getDesignations().subscribe((x: any) => {
      this.dropDowns.designations = x;
      this.dropDowns.designationsData = this.dropDowns.designations;
    }, err => {
      console.log(err);

    });
  }
  public dropdownValueChanged(value: any, filter: string) {
    if (filter == 'designation') {
      this.newAd.DesignationId = value.Id;
    }
  }
  
  public handleFilter = (value, filter) => {
    if (filter == 'designation') {
      this.dropDowns.designationsData = this.dropDowns.designations.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
   
  }
}

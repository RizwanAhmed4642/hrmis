import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { DailyWagerService } from '../../../_services/daily-wager.service';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { ProfileCompactView } from '../../profile/profile.class';
@Component({
  selector: 'app-wager-cat-list',
  templateUrl: './wager-cat-list.component.html',
  styleUrls: ['./wager-cat-list.component.scss']
})
export class WagerCatListComponent implements OnInit {
public Category : any
public paramsObject : any
public Type : any
public hfmisCode : any
public value : any
public loading = false;
searchTerm: any
selectedCadres: any
selectedDesignations: any
selectedStatuses: any
retirementInOneYear: any
retirementAlerted: any
designationStr: any
//grid data
public gridView: GridDataResult;
public profiles: ProfileCompactView[] = [];
public profilesAll: any[] = [];
public userViewType: number = 2;
public viewTypes: number[] = [1, 2, 3];
public pageSizes = [50, 100, 200, 500];
  public totalRecords = 0;
  public pageSize = 50;
  public skip = 0;
  constructor(public _DailyWagerService : DailyWagerService,private route: ActivatedRoute) { }

  ngOnInit() {
      debugger
      this.loading = true;
      this.route.queryParamMap
      .subscribe((params) => {
        this.paramsObject = { ...params.keys, ...params };
        //console.log(this.paramsObject);
        this.designationStr = this.paramsObject.params.Category
        this.hfmisCode = this.paramsObject.params.Type
        this.value = this.paramsObject.params.value
        console.log(this.Category);
        console.log(this.Type);
        console.log(this.value)
        this._DailyWagerService.getProfilesForMap(this.skip, this.pageSize, this.hfmisCode, this.searchTerm, this.selectedCadres, this.selectedDesignations, this.selectedStatuses, this.retirementInOneYear, this.retirementAlerted, this.designationStr, this.value).subscribe((res: any) => {
          this.profiles = res.List.List;
          debugger
          this.totalRecords = res.List.Count;
          this.gridView = { data: this.profiles, total: this.totalRecords };
          this.loading = false;
          //this.searching = false;
          
          console.log('aaaaaaaaaaaaaa',this.profiles)
        }, err => {
          console.log(err);
        })
      }
      
    );
  }
    
}

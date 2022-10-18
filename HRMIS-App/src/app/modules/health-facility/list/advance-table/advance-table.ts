import { Component, OnInit, Input, ViewChild, NgZone, EventEmitter, Output } from '@angular/core';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { PageChangeEvent, GridDataResult, GridComponent } from '@progress/kendo-angular-grid';
import { take } from 'rxjs/operators/take';

@Component({
  selector: 'app-hf-advance-table',
  templateUrl: './advance-table.html',
  styles: [`
  

  `]
})
export class AdvanceTableComponent implements OnInit {
  @ViewChild('grid', {static: false}) public grid: GridComponent;
  @Input() public gridView: GridDataResult;
  public columns: any[] = [];


  //loading flag
  @Input() public loading = true;
  //sorting variable
  @Input() public multiple = false;
  @Input() public allowUnsort = true;
  @Input() public sort: SortDescriptor[] = [];
  //Pagination variables
  @Input() public pageSizes = [5, 10, 25, 50];
  @Input() public totalRecords = 0;
  @Input() public pageSize = 10;
  @Input() public skip = 0; 
  @Input() public lock: string = '';

  @Output() sortChangeEmitter = new EventEmitter();
  @Output() pageChangeEmitter = new EventEmitter();


  constructor(private ngZone: NgZone) {
  }

  public ngOnInit(): void {
    this.generateColumns();
  }
  public generateColumns() {
    this.loading = true;
    console.log('iterator');
    this.columns = [];
    var keyNames = Object.keys(this.gridView.data[0]);
    for (let j = 0; j < keyNames.length; j++) {
      this.columns.push({
        field: keyNames[j],
        title: keyNames[j]
      });
    }
    this.loading = false;
    this.fitColumns();
  }
  public onDataStateChange(): void {
    this.fitColumns();
  }

  private fitColumns(): void {
    this.ngZone.onStable.asObservable().pipe(take(1)).subscribe(() => {
      this.grid.autoFitColumns();
    });
  }
  public sortChange(event) {
    this.sortChangeEmitter.emit(event);
  }
  public pageChange(event) {
    this.pageChangeEmitter.emit(event);
  }
}

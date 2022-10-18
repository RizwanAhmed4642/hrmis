import { Component, OnInit } from '@angular/core';
import { OnlineJobsService } from '../online-jobs.service';

@Component({
  selector: 'app-jobs-list',
  templateUrl: './jobs-list.component.html',
  styles: []
})
export class JobsListComponent implements OnInit {
  public jobs: any[] = [];
  constructor(private _onlineJobsService: OnlineJobsService) { }

  ngOnInit() {
    this.getJobs();
  }
  public getJobs() {
    this._onlineJobsService.getJobs().subscribe((res: any) => {
      if (res) {
        this.jobs = res;
      }
    }, err => {
      console.log(err);
    });
  }
}

import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { RootService } from '../../_services/root.service';
import { OnlineFacilitationCenterService } from './online-facilitation-center.service';

@Component({
  encapsulation: ViewEncapsulation.None,
  selector: 'app-online-facilitation-center',
  templateUrl: './online-facilitation-center.component.html',
})
export class OnlineFacilitationCenterComponent implements OnInit {

  items: MenuItem[];
  public typeIds: number[] = [
    1
    , 2
    , 3
    , 4
    , 10
    , 16
    , 31
    , 33
    , 49
  ];

  public dialogClosed: boolean = true;
  public applicationTypes: any[] = [];
  constructor(private _onlineFacilitationService: OnlineFacilitationCenterService,
    private _rootService: RootService,
    private router: Router
  ) { }

  ngOnInit() {
    // this.getApplicationsTypes();
    this.items = [
      {
        label: 'Profile',
        icon: 'pi pi-user',
        routerLink: 'profile',
      },
      {
        label: 'Applications',
        icon: 'pi pi-file',
        routerLink: 'application',
      },
      {
        label: 'Leave Record',
        icon: 'pi pi-folder-open',
        routerLink: 'leave-record'
      },
      {
        label: 'Service History',
        icon: 'pi pi-clock',
        routerLink: 'service-history'
      },

      //   items:[
      //       {
      //           label:'Leave Record',
      //           icon:'pi pi-folder-open',
      //           routerLink: 'leave-record'
      //           // items:[
      //           // {
      //           //     label:'Bookmark',
      //           //     icon:'pi pi-fw pi-bookmark'
      //           // },
      //           // {
      //           //     label:'Video',
      //           //     icon:'pi pi-fw pi-video'
      //           // },

      //           // ]
      //       }
      //   ]

    ];

  }

  public getApplicationsTypes() {
    this.applicationTypes = [];
    this._rootService.getApplicationTypesActive().subscribe((data: any) => {
      if (data) {
        this.dialogClosed = false;
        data.forEach(t => {
          this.typeIds.forEach(id => {
            if(t.Id == id){
              this.applicationTypes.push(t);
            }
          });
        });
        console.log('types: ', this.applicationTypes);
      }
    }, err => {
      console.log(err);
    });
  }

  public openApplicationTypes(link) {
    this.dialogClosed = false;
    if (link != null) {
      this.router.navigate([link]);
      this.dialogClosed = true;
    }

  }

  public closeWindow() {
    this.dialogClosed = true;
  }
  public logout() {
    this._onlineFacilitationService.logout();
  }

}

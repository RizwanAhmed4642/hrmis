import { Component, OnInit } from '@angular/core';
import { navItems } from './../../_nav';
import { AuthenticationService } from '../../_services/authentication.service';
import { Router, NavigationStart, RouteConfigLoadStart, RouteConfigLoadEnd, RoutesRecognized, NavigationEnd, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { RootService } from '../../_services/root.service';
import { LocalService } from '../../_services/local.service';
import { Title } from '@angular/platform-browser';
import { UserNav } from '../../_models/nav.class';
import { NotificationService } from '../../_services/notification.service';
import { PandSOfficerView } from '../../modules/user/user-claims.class';
import { FirebaseHisduService } from '../../_services/firebase-hisdu.service';
import { DialogService } from '../../_services/dialog.service';
import { Observable } from 'rxjs/Observable';
import { OperatorFunction } from 'rxjs/internal/types';
import { defer } from 'rxjs/internal/observable/defer';
import { mergeMap } from 'rxjs/internal/operators/mergeMap';
import { filter } from 'rxjs/internal/operators/filter';

@Component({
  selector: 'app-dashboard',
  templateUrl: './default-layout.component.html'
})
export class DefaultLayoutComponent implements OnInit {

  public navItems = [];
  public sidebarMinimized = true;
  public searchOn: boolean = false;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public routerEventsSubcription: Subscription = null;
  public routesSubcription: Subscription = null;
  public navigating: boolean = false;
  public searching: boolean = false;
  public canApply: boolean = false;
  public loadBarWidth: number = 0;
  private changes: MutationObserver;
  public element: HTMLElement = document.body;
  public user: any;
  public currentOfficer: PandSOfficerView;
  public userLevel: any;
  public currentProfile: any;
  public userLevelName: string = '';
  public searchQuery: string = '';
  public userIP: string = '';
  public elem: any;
  data: Array<any> = [{
    text: 'My Profile'
  }, {
    text: 'Friend Requests'
  }, {
    text: 'Account Settings'
  }, {
    text: 'Support'
  }, {
    text: 'Log Out'
  }];
  public listItems: any[] = [];
  constructor(private _firebaseHisduService: FirebaseHisduService, private _dialogService: DialogService, private _notificationService: NotificationService, private route: ActivatedRoute, private titleService: Title, public _authenticationServie: AuthenticationService, private _rootService: RootService, private _localService: LocalService, private router: Router) {
    this.changes = new MutationObserver((mutations) => {

      this.sidebarMinimized = document.body.classList.contains('sidebar-minimized');
    });
    this.changes.observe(<Element>this.element, {
      attributes: true
    });
  }
  ngOnInit() {
    this.handleRouterEvents();
    this.handleSearchEvents();
    this.handleTitles();
    this.user = this._authenticationServie.getUser();
    if (!this.user) {
      this._notificationService.notify('success', 'Unauthorized User!');
      this._authenticationServie.logout();
    }
    this._rootService.getCurrentOfficer().subscribe((res: PandSOfficerView) => {
      if (res) {
        this.currentOfficer = res;
      }
    });
    this._rootService.getProfileByCNIC(this.user.cnic).subscribe((res: any) => {
      if (res && res != 404) {
        this.currentProfile = res;
        if ((this.currentProfile.Designation_Id == 1320
          || this.currentProfile.Designation_Id == 802
          || this.currentProfile.Designation_Id == 2404
          || this.currentProfile.Designation_Id == 362
          || this.currentProfile.Designation_Id == 365
          || this.currentProfile.Designation_Id == 368
          || this.currentProfile.Designation_Id == 369
          || this.currentProfile.Designation_Id == 373
          || this.currentProfile.Designation_Id == 374
          || this.currentProfile.Designation_Id == 375
          || this.currentProfile.Designation_Id == 381
          || this.currentProfile.Designation_Id == 382
          || this.currentProfile.Designation_Id == 383
          || this.currentProfile.Designation_Id == 384
          || this.currentProfile.Designation_Id == 385
          || this.currentProfile.Designation_Id == 387
          || this.currentProfile.Designation_Id == 390
          || this.currentProfile.Designation_Id == 1594
          || this.currentProfile.Designation_Id == 1598
          || this.currentProfile.Designation_Id == 2136
          || this.currentProfile.Designation_Id == 2313) && this.currentProfile.EmpMode_Id == 13) {
          this.canApply = true;
        }
      }
    });
    this.navItems = this._authenticationServie.getNav();
    if (this.navItems.length == 0) {
      this._notificationService.notify('success', 'Unauthorized User!');
      this._authenticationServie.logout();
    }
    this.userLevel = this._authenticationServie.getUserLevel();
    this._authenticationServie.userLevelNameEmitter.subscribe(x => this.userLevelName = x);
    this._authenticationServie.userIPEmitter.subscribe(x => this.userIP = x);
    this._authenticationServie.setUserDisplay(this._authenticationServie.getUserHfmisCode() === '0');
    /* if (this.user.RoleName == 'Health Facility') {
      this.router.navigate(['/health-facility/' + this.user.HfmisCode]);
    } */
    this.subscribeToFirebase();
    // this.notifyMe('Hello Waseem Bhai');
    this.elem = document.documentElement;
  }
  private subscribeToFirebase() {
    /* this._firebaseHisduService.getApplicationPreview('FDO1').subscribe(data => {
      let obj = data.map(e => {
        return {
          id: e.payload.doc.id,
          ...e.payload.doc.data()
        };
      });
    });
    this._firebaseHisduService.updateAppData({ id: '4tdLisPMH4ahoDIcRkx0', hello: 'h' }); */
  }

  public openFullscreen() {
    if (document.exitFullscreen) {
      document.exitFullscreen();
    }
    if (this.elem.requestFullscreen) {
      this.elem.requestFullscreen();
    } else if (this.elem.mozRequestFullScreen) { /* Firefox */
      this.elem.mozRequestFullScreen();
    } else if (this.elem.webkitRequestFullscreen) { /* Chrome, Safari & Opera */
      this.elem.webkitRequestFullscreen();
    } else if (this.elem.msRequestFullscreen) { /* IE/Edge */
      this.elem.msRequestFullscreen();
    }
  }

  /* Function to close fullscreen mode */
  public closeFullscreen() {
    if (document.exitFullscreen) {
      document.exitFullscreen();
    }
  }

  private notifyMe(text: string) {
    // Let's check if the browser supports notifications
    if (!("Notification" in window)) {
      alert("This browser does not support system notifications");
    }
    // Let's check whether notification permissions have already been granted
    else if (Notification.permission === "granted") {
      // If it's okay let's create a notification
      var img = '../../../assets/img/brand/govlogoUpdated.png';
      var notification = new Notification('HRMIS', { body: text, icon: img });
    }
    // Otherwise, we need to ask the user for permission
    else if (Notification.permission !== 'denied') {
      Notification.requestPermission(function (permission) {
        // If the user accepts, let's create a notification
        if (permission === "granted") {
          var img = '../../../assets/img/brand/govlogoUpdated.png';
          var text = 'HEY! Your task is now overdue.';
          var notification = new Notification('HRMIS', { body: text, icon: img });
        }
      });
    }

    // Finally, if the user has denied notifications and you 
    // want to be respectful there is no need to bother them any more.
  }
  public handleRouterEvents() {
    this.routerEventsSubcription = this.router.events.subscribe((_) => {
      if ((_ instanceof NavigationStart)) {
        this.navigating = true;
        window.scroll(0, 0);
        setTimeout(() => {
          this.loadBarWidth += 10;
        }, 100);
      }
      if ((_ instanceof NavigationEnd)) {
        this.handleTitles();
        this.navigating = false;
      }
    });
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x) => {
        this.search(x);
      });
  }
  public async search(value: string) {
    if (value.length > 2) {
      this.searching = true;
      this.listItems = [];
      try {
        this.searchQuery = value;
        this._rootService.search(value).subscribe((res: any) => {
          if (res) {
            this.listItems = res;
            this.searching = false;
          }
        }, err => {
          console.log(err);
        });
      } catch (error) {
        console.log(error);
      }
    }
  }
  private mergeMapLast<T, R>(next: ((data: T) => Observable<R>)): OperatorFunction<T, R> {
    return (source: Observable<T>) => defer(() => {
      let curr = 0;
      return source.pipe(
        mergeMap((d, i) => {
          curr = i;
          return next(d).pipe(filter(_ => i === curr))
        })
      )
    })
  }
  public searchClicked(itemName: string) {
    let item = this.listItems.find(x => x.Name === itemName);
    if (item) {
      let navigateTo: string[] = [];
      if (item.Type == 1) {
        navigateTo.push('/health-facility/' + item.HFMISCode);
      } else if (item.Type == 2) {
        navigateTo.push('/profile/' + item.CNIC);
      } else if (item.Type == 3) {
        if (this.searchQuery.startsWith('ESR')) {
          navigateTo.push('/order/editor-view/' + item.Id + '/1');
        } else if (this.searchQuery.startsWith('ELR')) {
          navigateTo.push('/order/editor-view/' + item.Id + '/2');
        }
      }
      this.router.navigate(navigateTo);
      this.searchOn = false;
    }

  }
  public handleTitles() {
    if (this.route.children.length > 0) {

      let child = this.route.children[0];
      console.log(child);
      if (child.children.length > 0) {
        child = child.children[0];
        this.routesSubcription = child.data.subscribe(data => {
          this.setTitle(data.title);
        });
      }
      else {
        this.routesSubcription = child.data.subscribe(data => {
          this.setTitle(data.title);
        });
      }

    } else {
      this.routesSubcription = this.route.data.subscribe(data => {
        this.setTitle(data.title);
      });
    }
  }
  public newDialog(type) {
    this._dialogService.openDialog({ type: type });
  }
  loadUserData() {
    /*  switch (this.user.HfmisCode.length) {
         case 1:
           this.userLevelName = 'Punjab';
           break;
         case 3:
           this._mainService.getDivisionsByCode(this.user.HfmisCode).subscribe((data) => {
             this.divisions = data;
             if (data && data.length > 0) {
               this.userLevelName = data[0].Name;
             }
           }, (err) => this.handleError(err));
           this.userLevelName = 'Punjab';
           break;
         case 6:
           this._mainService.getDistrictsByCode(this.user.HfmisCode).subscribe((data) => {
             if (data && data.length > 0) {
               this.userLevelName = data[0].Name;
             }
           }, (err) => this.handleError(err));
           this.userLevelName = 'Punjab';
           break;
         case 9:
           this._mainService.getTehsilsByCode(this.user.HfmisCode).subscribe((data) => {
             if (data && data.length > 0) {
               this.userLevelName = data[0].Name;
             }
           }, (err) => this.handleError(err));
           this.userLevelName = 'Punjab';
           break;
         case 19:
           this.userLevelName = 'Punjab';
           break;
   
         default:
           break;
       } */
  }
  public setTitle(title: string) {
    this.titleService.setTitle(title + ' - HRMIS');
  }
  public getTitle() {
    return this.titleService.getTitle();
  }
  public logout() {
    this._authenticationServie.logout();
  }


}

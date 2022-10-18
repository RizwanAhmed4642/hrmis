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

@Component({
  selector: 'app-a4-layout',
  templateUrl: './a4-layout.component.html'
})
export class A4LayoutComponent implements OnInit {

  public navItems = [];
  public sidebarMinimized = true;
  public searchOn: boolean = false;
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;
  public routerEventsSubcription: Subscription = null;
  public routesSubcription: Subscription = null;
  public navigating: boolean = false;
  public searching: boolean = false;
  public loadBarWidth: number = 0;
  private changes: MutationObserver;
  public element: HTMLElement = document.body;
  public user: any;

  public listItems: any[] = [];
  constructor(private route: ActivatedRoute, private titleService: Title, private _authenticationServie: AuthenticationService, private _rootService: RootService, private _localService: LocalService, private router: Router) {
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
    this.navItems = this._authenticationServie.getNav();
  }
  public handleRouterEvents() {
    this.routerEventsSubcription = this.router.events.subscribe((_) => {
      if ((_ instanceof NavigationStart)) {
        this.navigating = true;
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
  public search(value: string) {
    this.listItems = [];
    if (value.length > 2) {
      this.searching = true;
      this._rootService.search(value).subscribe((data) => {
        this.listItems = data as any[];
      });
    }
  }
  public searchClicked(itemName) {
    let item = this.listItems.find(x => x.Name === itemName);
    let navigateTo: string[] = [];
    if (item.Type == 1) {
      navigateTo.push('/health-facility/' + item.HFMISCode);
    } else if (item.Type == 2) {
      navigateTo.push('/profile/' + item.CNIC);
    } else if (item.Type == 3) {
      navigateTo.push('/order/' + item.Id);
    }
    this.router.navigate(navigateTo);
  }
  public handleTitles() {
    if (this.route.children.length > 0) {
      let child = this.route.children[0];
      this.routesSubcription = child.data.subscribe(data => {
        this.setTitle(data.title);
      });
    } else {
      this.routesSubcription = this.route.data.subscribe(data => {
        this.setTitle(data.title);
      });
    }
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
  public logout() {
    this._authenticationServie.logout();
  }


}

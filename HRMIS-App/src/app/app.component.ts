import { Component, OnInit } from '@angular/core';
import { Router, NavigationEnd, NavigationStart, RouteConfigLoadStart, RouteConfigLoadEnd, RoutesRecognized } from '@angular/router';

@Component({
  selector: 'body',
  template: '<router-outlet></router-outlet>'
})
export class AppComponent implements OnInit {
  constructor(private router: Router) { }
  ngOnInit() {
    this.router.events.subscribe((evt) => 
     {
      if ((evt instanceof NavigationStart)) {
      };
      if ((evt instanceof RouteConfigLoadStart)) {
      }
      if ((evt instanceof RouteConfigLoadEnd)) {
      }
      if ((evt instanceof RoutesRecognized)) {
      }
      if ((evt instanceof NavigationEnd)) {
      };
      if (!(evt instanceof NavigationEnd)) {
        return;
      }
      window.scrollTo(0, 0);
    });
  }
}

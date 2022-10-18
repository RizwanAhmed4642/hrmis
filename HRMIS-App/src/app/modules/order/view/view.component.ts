import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styles: []
})
export class ViewComponent implements OnInit {
  public loaded: boolean = false;

  constructor() { }

  ngOnInit() {
  }

}

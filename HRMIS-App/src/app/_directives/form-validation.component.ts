import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-form-validation',
  template: `
  <div style="margin-top: -6px;" *ngIf="show" class="animated fadeIn" [ngClass]="{'text-danger' : danger}"><small>{{message}}</small></div>
  `,
  styles: []
})
export class FormValidationComponent implements OnInit {
  @Input() show: boolean = false;
  @Input() danger: boolean = true;
  @Input() message: string = '';
  constructor() { }

  ngOnInit() {
  }

}

import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-online-promotion-apply',
  templateUrl: './online-promotion-apply.component.html',
  styles: [`
  .app-body .main {
    margin-bottom: 20px !important;
}
  `]
})
export class OnlinePromotionApplyComponent implements OnInit {
  public registerFlag: boolean = true;
  public current = 1;
  constructor() { }

  public steps = [
    { label: "Personal Info", icon: "user" },
    { label: "Education", icon: "dictionary-add" },
    { label: "Attachments", icon: "attachment", optional: true },
    { label: "Preview", icon: "preview" },
    { label: "Submit", icon: "file-add" },
  ];
  ngOnInit() {
  }

}

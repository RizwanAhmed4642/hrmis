import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RegisterService } from '../register/register.service';
import { AuthenticationService } from '../../_services/authentication.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './change-password.component.html'
})
export class ChangePasswordComponent implements OnInit {
  model: any = {};
  public step: number = 1;
  public nextText: string = 'Verify';
  public loading: boolean = false;
  public value: string = "1234";
  public mask: string = "0-0-0-0";
  constructor(private router: Router, private _registerService: RegisterService,
    private _authenticationService: AuthenticationService) {
    let element = document.getElementsByTagName("body")[0];
    element.classList.remove("sidebar-lg-show");
  }
  ngOnInit() {
  }
  public onSubmit(value: any) {
    console.log(value);
    this._registerService.changePassword({ oldPassword: value.oldPassword, newPassword: value.newPassword }).subscribe(
      response => {
        this._authenticationService.logout();
        this.router.navigate(['/login']);
      },
      err => {
        console.log(err);

      }
    );
  }
}
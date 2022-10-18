import { Component, OnInit } from '@angular/core';
import { DatabaseService } from '../database.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { NotificationService } from '../../../_services/notification.service';
import { Route } from '@angular/compiler/src/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-cords-address',
  templateUrl: './cords-address.component.html',
  styleUrls: ['./cords-address.component.scss']
})
export class CordsAddressComponent implements OnInit {

  public Address: string = "";

  constructor(
    private _databaseService: DatabaseService,
    private _authenticationService: AuthenticationService,
    public _notificationService: NotificationService,
    public router: Router
  ) { }



  ngOnInit() {
  }

  public findCoordinates(){
    this._databaseService.getCords(this.Address).subscribe((res: any)=>{
      if(res){
        alert(res);
      }
    })    
  }
}

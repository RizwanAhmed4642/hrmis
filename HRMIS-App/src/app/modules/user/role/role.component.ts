import { Component, OnInit } from '@angular/core';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { UserService } from '../user.service';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';

@Component({
  selector: 'app-role',
  templateUrl: './role.component.html',
  styles: []
})
export class RoleComponent implements OnInit {
  public roles: string[] = [];
  public newRoleName: string = '';
  public alreadyExistRoleName: string = '';
  public error: string = '';
  public loading: boolean = false;
  constructor(private _userService: UserService) { }

  ngOnInit() {
    this.loadRoles();
  }
  private loadRoles() {
    this.loading = true;
    this._userService.getRoles().subscribe(
      response => this.roles = response as string[]
    );
  }
  public addRole() {
    this.error = '';
    this._userService.registerRole(this.newRoleName).subscribe(
      (response) => {
        console.log(response);
        if (response == true) {
          this.newRoleName = '';
          this.loadRoles();
        } else if (response == false) {
          this.error = 'role name already exist';
          this.alreadyExistRoleName = this.newRoleName;
        }
      }
    );
  }
}

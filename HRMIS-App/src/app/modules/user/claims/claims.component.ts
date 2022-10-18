import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserService } from '../user.service';
import { Module } from './claims.class';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-claims',
  templateUrl: './claims.component.html',
  styleUrls: ['./claims.component.scss']
})
export class ClaimsComponent implements OnInit, OnDestroy {
  public modules: any[] = [];
  public checkAll: boolean = false;
  public checkAllFields: boolean = false;
  private subscription: Subscription;
  public userId: string = '';
  constructor(private _userService: UserService, private route: ActivatedRoute) { }

  ngOnInit() {
    // this.modules = this._userService.getModules();
    this.fetchParams();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id')) {
          this.userId = params['id'];
          this.getModules();
        }
      }
    );
  }
  private getModules() {
    this.modules = [];
    this._userService.getModules().subscribe(
      (res: any) => {
        this.modules = res;
        
        this.initializeProps();
      }
    );
  }
  private initializeProps() {
    this.modules.forEach(erpModule => {
      erpModule.erpComponents.forEach(erpComponent => {
        erpComponent.showFields = false;
        erpComponent.allFields = false;
        this.fieldChecked(erpModule.Id, erpComponent.Id);
      });
    });
  }
  public onSave() {
    let obj = { Id: this.userId, erpModules: this.modules };
    this.modules = [];
    this._userService.saveUserModules(obj).subscribe(
      (res: any) => {
        this.getModules();
      }
    );
  }
  allowAll(event: any) {
    let checkBoxValue = event.target.checked;
    this.modules.forEach(permission => {
      permission.Value = checkBoxValue;
      permission.erpComponents.forEach(subPermission => {
        subPermission.Value = checkBoxValue;
        subPermission.erpFields.forEach((field: any) => {
          field.Value = checkBoxValue;
        });
      });
    });
  }
  allowAllFields(event: any, moduleId: string, componentId: string) {
    let checkBoxValue = event.target.checked;
    this.modules.forEach(permission => {
      if (permission.Id == moduleId) {
        permission.erpComponents.forEach(subPermission => {
          if (subPermission.Id == componentId) {
            subPermission.erpFields.forEach((field: any) => {
              field.Value = checkBoxValue;
            });
          }
        });
      }
    });
  }
  permissionChecked(permissionId: any, event: any) {
    let checkBoxValue = event.target.checked;
    if (checkBoxValue) {
      this.modules.forEach(permission => {
        if (permissionId == permission.Id) {
          if (permission.erpComponents) {
            permission.erpComponents.forEach(subPermission => {
              subPermission.Value = true;
            });
          }
        }
      });
    } else {
      this.modules.forEach(permission => {
        if (permissionId == permission.Id) {
          if (permission.erpComponents) {
            permission.erpComponents.forEach(subPermission => {
              subPermission.Value = false;
            });
          }
        }
      });
    }
  }
  subPermissionChecked(permissionId: string, subPermissionId: string, event: any) {
    let checkBoxValue = event.target.checked;
    if (checkBoxValue) {
      this.modules.forEach(permission => {
        if (permissionId == permission.Id && !permission.Value) {
          permission.Value = true;
        }
      });
    } else if (!checkBoxValue) {
      this.allSubPermissionsUnchecked(permissionId, subPermissionId);
    }
  }
  fieldChecked(permissionId: string, subPermissionId: string) {
    let count = 0;
    this.modules.forEach(permission => {
      if (permissionId == permission.Id) {
        if (permission.erpComponents) {
          permission.erpComponents.forEach(subPermission => {
            if (subPermission.Id == subPermissionId) {
              subPermission.erpFields.forEach((field: any) => {
                if (field.Value) {
                  count++;
                }
              });
              if (count == (subPermission.erpFields.length)) {
                subPermission.allFields = true;
              } else {
                subPermission.allFields = false;
              }
            }
          });
        }
      }
    });
  }
  allSubPermissionsUnchecked(permissionId: string, subPermissionId: string) {
    let count = 0;
    this.modules.forEach(permission => {
      if (permissionId == permission.Id) {
        if (permission.erpComponents) {
          permission.erpComponents.forEach(subPermission => {
            if (subPermission.Id == subPermissionId) {
              subPermission.erpFields.forEach((field: any) => {
                field.Value = false;
              });
            }
            subPermission.allFields = false;
            if (subPermission.Value) {
              count++;
            }
          });
        }
        if (count == 0) {
          permission.Value = false;
        }
      }
    });
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}

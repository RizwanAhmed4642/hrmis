import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserService } from '../../user.service';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { UserClaims } from '../../user-claims.class';

@Component({
  selector: 'app-add-claims',
  templateUrl: './add-claims.component.html',
  styleUrls: ['../claims.component.scss']
})
export class AddClaimsComponent implements OnInit, OnDestroy {
  public loading: boolean = true;
  public user: any;
  public newClaim: UserClaims = new UserClaims();
  public claims: UserClaims[] = [];
  public userId: any = {};
  private subscription: Subscription;
  constructor(private _userService: UserService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.fetchParams();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id')) {
          this.userId = params['id'];
          this.fetchData(this.userId);
        } else {
          this.loading = false;
        }
      }
    );
  }
  private fetchData(userId) {
    this._userService.getUserClaims(userId).subscribe(
      (res: any) => {
        console.log(res);
        this.user = res.user;
        this.claims = res.claims;
        this.loading = false;
      },
      err => {
        console.log(err);
      }
    );
  }
  public addClaim() {
    this.newClaim.UserId = this.userId;
    this._userService.submitUserClaims(this.newClaim).subscribe((response) => {
      console.log(response);

    });
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}

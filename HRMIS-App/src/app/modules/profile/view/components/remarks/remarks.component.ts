import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { ProfileService } from '../../../profile.service';

@Component({
  selector: 'app-profile-remarks',
  templateUrl: './remarks.component.html',
  styles: [`
  .order-thumb-wrap-2 {
    height: 100%;
}
  `]
})
export class RemarksComponent implements OnInit {
  @Input() public profile: any;
  public profileRemarks: any = {};
  public remarks: any[] = [];
  @ViewChild('photoRef', {static: false}) public photoRef: any;
  public photoSrc = '';
  public photoSrces: any[] = [];
  public photoFile: any[] = [];
  public savingRemarks: boolean = false;
  public uploadingFile: boolean = false;
  public uploadingFileError: boolean = false;
  constructor(private _profileService: ProfileService) { }

  ngOnInit() {
    this.getRemarks();
  }
  getRemarks() {
    this._profileService.getProfileRemarks(this.profile.Id).subscribe((res: any) => {
      if (res) {
        this.remarks = res;
      }
    }, err => {
      console.log(err);
    });
  }
  saveRemarks() {
    this.savingRemarks = true;
    this.profileRemarks.Profile_Id = this.profile.Id;
    this._profileService.postProfileRemarks(this.profileRemarks).subscribe((res: any) => {
      if (res && res.Id) {
        if (this.photoFile.length > 0) {
          this.uploadFile(this.profile.Id, res.Id);
        }else {
          this.profileRemarks = {};
          this.getRemarks();
          this.savingRemarks = false;
        }
      }
    }, err => {
      console.log(err);
    });
  }
  
  editRemarks(profileRemarks : any) {
    profileRemarks.saving = true;
    this._profileService.postProfileRemarks(profileRemarks).subscribe((res: any) => {
      if (res && res.Id) {
       /*  if (this.photoFile.length > 0) {
          this.uploadFile(this.profile.Id, res.Id);
        }else {
          this.profileRemarks = {};
          this.getRemarks();
          this.savingRemarks = false;
        } */
        this.getRemarks();
        profileRemarks.saving = false;
        profileRemarks.edit = false;
      }
    }, err => {
      console.log(err);
    });
  }
  removeRemarks(id: number) {
    if (confirm('Confirm remove remarks?')) {
      this._profileService.removeProfileRemarks(id).subscribe((res) => {
        if (res) {
          this.getRemarks();
        }
      }, err => {
        console.log(err);
      });
    }
  }

  public readUrl(event: any, filter: string) {
    if (event.target.files && event.target.files[0]) {
      if (filter == 'pic') {
        this.photoFile = [];
        let inputValue = event.target;
        this.photoFile = inputValue.files;

       /*  var reader = new FileReader();
        reader.onload = ((event: any) => {
          this.photoSrc = event.target.result;
        }).bind(this);
        reader.readAsDataURL(event.target.files[0]); */
      }
    }
  }

  public uploadBtn(filter: string) {
    if (filter == 'pic') {
      this.photoRef.nativeElement.click();
      /*   this.uploadingAcceptanceLetter = true; */
    }
  }
  public uploadFile(profile_Id: number, profileRemarks_Id: number) {
    this.uploadingFile = true;
    this._profileService.uploadProfileAttachement(this.photoFile, profile_Id, profileRemarks_Id).subscribe((x: any) => {
      if (!x.result) {
        this.uploadingFileError = true;
      }
      this.uploadingFile = false;
      this.profileRemarks = {};
      this.photoFile = [];
      this.getRemarks();
      this.savingRemarks = false;
    }, err => {
      this.uploadingFileError = true;
      this.uploadingFile = false;
    });
  }
}

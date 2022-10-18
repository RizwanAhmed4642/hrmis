import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CookieService } from '../../../_services/cookie.service';
import { OnlineAdhocService } from '../online-adhoc.service';

@Component({
  selector: 'app-jobs-list',
  templateUrl: './jobs-list.component.html',
  styles: [`
  .zoom {
  transition: transform 1s; /* Animation */
}

.zoom:hover {
  transform: scale(1.06); /* (150% zoom)*/
}
  `],
   providers: [DatePipe]
})
export class JobsListComponent implements OnInit {
  public designationCategories: any[] = [
    {
      Id: 1, Name: 'Medical Officer',
      Description: 'Any male doctor with MBBS and valid PMDC is eligible for this post',
      Icon: '../../../../assets/img/icons/medical_officer.png'
    },
    {
      Id: 2, Name: 'Women Medical Officer',
      Description: 'Any female doctor with MBBS and valid PMDC is eligible for this post',
      Icon: '../../../../assets/img/icons/woman_doctor.png'
    },
    {
      Id: 3, Name: 'Specialist Doctor',
      Description: 'Any specialist doctor with MBBS and FCPS is eligible for this post. For MCPS, 2 Year experience in relevant speciality is required',
      Icon: '../../../../assets/img/icons/specialist.png'
    },
    {
      Id: 4, Name: 'Charge Nurse',
      Description: 'Anyone with valid PNC number is eligible for this post',
      Icon: '../../../../assets/img/icons/nurse.png'
    },
    {
      Id: 5, Name: 'Dental Surgeon',
      Description: 'Any male doctor with BDMC and valid PMDC is eligible for this post',
      Icon: '../../../../assets/img/icons/dentist.png'
    }
  ];
  public urdu: any = {
    info: `ایڈھاک پر اپلائی کرنے کیلئے مندرجہ ذیل دی گئی کیٹیگریز میں سے کسی ایک کا انتخاب کریں۔`,
    infoeng: `Choose one of following categories for adhoc application`,
    mo: `میل ڈاکٹر ڈسٹرکٹ ہیڈ کوارٹر ہسپتال ، تحصیل ہیڈ کوارٹر ہسپتال، دیہی مرکز صحت پر صوبہ بھر میں خالی کسی بھی میڈیکل آفیسر ، سینئر میڈیکل آفیسر کی آسامی پر اپلائی کر سکتے ہیں اور بنیادی مرکز صحت پر میڈیکل آفیسر اور وومن میڈیکل آفیسر کی آسامی پر اپلائی کر سکتے ہیں۔ صوبہ بھر میں خالی آسامیوں کی معلومات لی جا سکتی ہیں۔`,
    moeng: `Male doctors can apply against the vacant posts of Medical Officer or Senior Medical Officer in DHQs, THQs and RHCs. For BHUs, male doctors can apply against the vacant posts of Medical Officer / Women Medical Officer`,
    wmo: `
  فی میل ڈاکٹر ڈسٹرکٹ ہیڈ کوارٹر ہسپتال ، تحصیل ہیڈ کوارٹر ہسپتال، دیہی مرکز صحت پر صوبہ بھر میں خالی کسی بھی وومن میڈیکل آفیسر ، سینئر وومن میڈیکل آفیسر کی آسامی پر اپلائی کر سکتے ہیں اور بنیادی مرکز صحت پر میڈیکل آفیسر اور وومن میڈیکل آفیسر کی آسامی پر اپلائی کر سکتے ہیں۔ صوبہ بھر میں خالی آسامیوں کی معلومات لی جا سکتی ہیں۔
  `,
    wmoeng: `Female doctors can apply against the vacant posts of Women Medical Officer or Senior Women Medical Officer in DHQs, THQs and RHCs. For BHUs, female doctors can apply against the vacant posts of Medical Officer / Women Medical Officer`,
    sp: `سپیشلسٹ ڈاکٹرڈسٹرکٹ ہیڈ کوارٹر ہسپتال ، تحصیل ہیڈ کوارٹر ہسپتال پر صوبہ بھر میں خالی اپنی متعلقہ سپیشلٹی کی آسامی پر اپلائی کر سکتے ہیں۔ صوبہ بھر میں خالی آسامیوں کی معلومات لی جا سکتی ہیں۔`,
    speng: `Specialist doctor can apply against the vacant posts of Consultant in relevant speciality in DHQs and sTHQ. Yous can also view detail of vacant seats`,
    ds: ` میل ڈاکٹریافی میل ڈاکٹر  ڈسٹرکٹ ہیڈ کوارٹر ہسپتال ، تحصیل ہیڈ کوارٹر ہسپتال پر صوبہ بھر میں خالی کسی بھی ڈینٹل سرجن، سینئر ڈینٹل سرجن کی آسامی پر اپلائی کر سکتے ہیں۔ صوبہ بھر میں خالی آسامیوں کی معلومات لی جا سکتی ہیں۔`,
    dseng: `Male doctors can apply against vacant posts of Dental Surgeon or Senior Dental Surgeon in DHQs, THQs and RHCs. You can also view detail of vacant seats`,
    cn: `  نرس ڈسٹرکٹ ہیڈ کوارٹر ہسپتال ، تحصیل ہیڈ کوارٹر ہسپتال، دیہی مرکز صحت  پر صوبہ بھر میں خالی چارج نرس کی آسامی پر اپلائی کر سکتی ہیں۔ صوبہ بھر میں خالی آسامیوں کی معلومات لی جا سکتی ہیں۔`,
    cneng: `Nurses can apply against vacant posts of Charge Nurse in DHQs, THQs and RHCs. You can also view detail of vacant seats`
  };
  public selectedCategoryId: number = 0;
  public desigId: number = 0;
  public step: number = 1;
  public adhocDesignations: any[] = [];
  public vacancy: any[] = [];
  public hfsOrigional: any[] = [];
  public districts: any[] = [];
  public jobVacancy: any = {};
  public application: any = {};
  public loading: boolean = false;
  public portalClosed: boolean = false;
  public closureDate: any = new Date('12-11-2021');
  constructor(private datePipe: DatePipe, private router: Router, private _cookieService: CookieService, private _onlineAdhocService: OnlineAdhocService) { }

  ngOnInit() {
    this.checkPortalClosure();
  }

  public checkPortalClosure() {
   this.portalClosed = false;
   /*  let currentDate: any = new Date();
    currentDate = this.datePipe.transform(currentDate, 'dd-MM-yyyy');
    this.closureDate = this.datePipe.transform(this.closureDate, 'dd-MM-yyyy');
    console.log('crt date: ', currentDate);
    console.log('cl date: ', this.closureDate);
    if (this.closureDate === currentDate) {
      this.portalClosed = false;
    } */
  }
  public getAdhocVacants() {
    this.jobVacancy = {};
    this._onlineAdhocService.getAdhocVacants('facilities', this.desigId).subscribe((res: any) => {
      if (res) {
        this.jobVacancy = res;

        this.hfsOrigional = this.jobVacancy.hfs;
        this.districts = this.jobVacancy.districts;
        this.districts.forEach(dist => {
          dist.hfs = this.hfsOrigional.filter(x => x.HFMISCode.startsWith(dist.Code))
        });
      }
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }
  public getAdhocs() {
    this.loading = true;
    this.adhocDesignations = [];
    this._onlineAdhocService.getAdhocs(this.selectedCategoryId).subscribe((res: any) => {
      if (res) {
        this.adhocDesignations = res.jobs;
        this.vacancy = res.vps;
        this.setVacancy();
      }
    }, err => {
      console.log(err);
    });
  }
  public setVacancy() {
    this.adhocDesignations.forEach(job => {
      job.vacancy = this.vacancy.filter(x => x.Desg_Id == job.Designation_Id);
      if (job.vacancy) {
        job.posts = 0;
        job.vacancy.forEach(vp => {
          job.posts += vp.Vacant;
        });
        /* job.districts = this.groupArrayOfObjects(job.vacancy, 'DistrictName');
        job.districts.forEach(vpdistrict => {
          vpdistrict.posts += vpdistrict.Vacant;
        }); */
      }
    });
    console.log(this.adhocDesignations);
    this.loading = false;

  }
  public groupArrayOfObjects(list, key) {
    return list.reduce((rv, x) => {
      (rv[x[key]] = rv[x[key]] || []).push(x);
      return rv;
    }, {});
  }
  public applyNow(designationId: number) {
    this._cookieService.deleteAndSetCookie('cnicussradhocdesig', designationId.toString());
    this.router.navigate(['/adhoc/profile']);
  }
  public viewSeats(designationId: number) {
    this.desigId = designationId;
    this.getAdhocVacants();
    this.step = 3;
  }
  public designationSelected(categoryId: number) {
    this.selectedCategoryId = categoryId;
    this.getAdhocs();
    this.step = 2;
  }
}

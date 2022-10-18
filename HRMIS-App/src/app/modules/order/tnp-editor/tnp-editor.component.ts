import { Router } from '@angular/router';
import { Component, Input, OnInit } from '@angular/core';
import { ESR, ESRDetail, LeaveOrder, ESRView } from '../TransferAndPosting/ESR.class';
import { TnPService } from '../transfer-n-posting.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { OrderService } from '../order.service';
import { LeaveOrderView } from '../order';

@Component({
    selector: 'hrmis-tnp-editor',
    templateUrl: './tnp-editor.component.html',
    styles: []
})
export class TnpEditorComponent implements OnInit {
    @Input() public orderResponses: any[] = [];
    @Input() public esr: ESR;
    @Input() public esrView: ESRView;
    @Input() public esrDetail: ESRDetail;
    @Input() public esr2: ESR;
    @Input() public barcodeSrc: string = '';
    @Input() public qrcodeSrc: string = '';
    @Input() public markupCC: string;
    @Input() public leaveOrder: LeaveOrder = new LeaveOrder();
    @Input() public leaveOrderView: LeaveOrderView = new LeaveOrderView();
    public employeeName: string = '';
    public designationName: string = '';
    public facilityName: string = '';
    public scaleBPS: string = '';
    public hfName: string = '';
    public districtName: string = '';
    public tehsilName: string = '';
    public cnic: string = '';
    public reason: string = '';
    public remarks: string = '';
    public fromDate: string = '';
    public toDate: string = '';
    public dateNow;
    public orderText: string = '';
    public watermark: string = '';
    public orderType: string = '';
    public orderLogo: string = '';
    public leaveTypeName: string = '';
    public user: any;
    public isHisduAdmin: boolean = false;
    public isDGH: boolean = false;
    public isPHFMC: boolean = false;
    public showOrder: boolean = false;
    public headerRightText: string = '';
    public DistrictTo: string = '';
    esrView2: any;
    constructor(private _orderService: OrderService, private _userService: AuthenticationService, private _tnpServie: TnPService, private router: Router) {
        this.orderText = ``;
    }

    ngOnInit() {
        this.insertEditorScript();
        this.user = this._userService.getUser();
        if (this._userService.getUserHfmisCode() == '0') {
            if (this.user.RoleName == 'DG Health') {
                this.isDGH = true;
                this.orderLogo = `
                    <img style="display:inline-block;" src="../../../../assets/img/dghslogo.jpg" width="134" alt="Punjab Government Log" />
                `;
                this.watermark = `
                <div style="text-align:center;position:absolute;left:0;width:100%;z-index:-1;font-family: -apple-system, system-ui, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
                font-weight: normal;
                line-height: 1.5;">
                <img alt="HISDU" src="https://hrmis.pshealthpunjab.gov.pk/assets/img/brand/pshdwatermark.jpg" style="display:inline-block; margin-top: 255px; opacity: 0.4;" />
                    </div>
                    `;
            } else if (this.user.RoleName == 'PHFMC Admin' || this.user.RoleName == 'PHFMC') {
                this.isPHFMC = true;
                this.isDGH = false;
                this.orderLogo = `
                    <img style="display:inline-block;" src="https://hrmis.pshealthpunjab.gov.pk/assets/img/brand/PHFMC-logo.png" width="134" alt="Punjab Government Log" />
                `;
                this.watermark = `
                <div style="text-align:center;position:absolute;left:0;width:100%;z-index:-1;font-family: -apple-system, system-ui, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
                font-weight: normal;
                line-height: 1.5;">
                <img alt="HISDU" src="https://hrmis.pshealthpunjab.gov.pk/assets/img/brand/pshdwatermark.jpg" style="display:inline-block; margin-top: 255px; opacity: 0.4;" />
                    </div>
                    `;
            } else {
                this.isHisduAdmin = true;
                this.isDGH = false;
                this.orderLogo = `
                <img style="display:inline-block;" src="https://hrmis.pshealthpunjab.gov.pk/assets/img/brand/govlogoUpdated.png" width="134" alt="Punjab Government Log" />
                `;
                this.watermark = `
                <div style="text-align:center;position:absolute;left:0;width:100%;z-index:-1;font-family: -apple-system, system-ui, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
                font-weight: normal;
                line-height: 1.5;">
                <img alt="HISDU" src="https://hrmis.pshealthpunjab.gov.pk/assets/img/brand/pshdwatermark.jpg" style="display:inline-block; margin-top: 255px; opacity: 0.4;" />
                    </div>
                    `;
            }
        } else {
            this.isHisduAdmin = false;
            this.isDGH = false;
            this.orderLogo = `
            <img style="display:inline-block;" src="https://hrmis.pshealthpunjab.gov.pk/assets/img/brand/govlogoUpdated.png" width="134" alt="Punjab Government Log" />
            `;
            this.watermark = `
            <div style="text-align:center;position:absolute;left:0;width:100%;z-index:-1;font-family: -apple-system, system-ui, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
            font-weight: normal;
            line-height: 1.5;">
                <img alt="HISDU" src="https://hrmis.pshealthpunjab.gov.pk/assets/img/brand/pshdwatermark.jpg" style="display:inline-block; margin-top: 255px; opacity: 0.4;" />
                </div>
                `;
        }
        this.orderResponses = this._tnpServie.orderResponses;
        this.esr = this._tnpServie.esr;
        this.esr2 = this._tnpServie.esr2;
        this.esrView = this._tnpServie.esrView;
        this.esrView2 = this._tnpServie.esrView2;
        this.esrDetail = this._tnpServie.esrDetail;
        this.leaveOrder = this._tnpServie.leaveOrder;
        this.leaveOrderView = this._tnpServie.leaveOrderView;
        this.leaveTypeName = this._tnpServie.leaveTypeName;
        this.employeeName = this._tnpServie.searchedProfile.EmployeeName;
        this.designationName = this._tnpServie.searchedProfile.Designation_Name;
        this.facilityName = this._tnpServie.healthFacilityFromName;
        this.districtName = this._tnpServie.searchedProfile.District;
        this.tehsilName = this._tnpServie.searchedProfile.Tehsil;
        this.scaleBPS = this._tnpServie.searchedProfile.Postaanctionedwithscale;
        this.hfName = this._tnpServie.searchedProfile.HealthFacility;
        this.cnic = this._tnpServie.searchedProfile.CNIC;
        this.markupCC = this._tnpServie.markupCC;
        let reasonId = this._tnpServie.esr.MutualESR_Id;

        this.reason = reasonId == 1 ? 'on poor performance'
            : reasonId == 2 ? 'on administrative grounds'
                : reasonId == 3 ? 'on his/her own request'
                    : reasonId == 4 ? 'public interest' : '';

        let today = new Date();
        let dd: any = today.getDate(), mm: any = this.makeOrderMonth(today.getMonth() + 1), yyyy = today.getFullYear();

        this.dateNow = this.makeOrderDate(dd) + ', ' + mm + ' ' + yyyy;

        this.orderType = this._tnpServie.esrView.TransferTypeID == 7 || this._tnpServie.esrView.TransferTypeID == 9 ? 'N O T I F I C A T I O N' : 'O R D E R';

        if (this._tnpServie.esrView.TransferTypeID == 5) {
            this.orderText = this.getOrderText();
            this.leaveOrder.OrderHTML = this.orderText;
            this._orderService.UpdateLeaveOrderHTML(this.leaveOrder).subscribe((x) => {
                this.showOrder = true;
            });
        } else {
            this.orderText = this.getOrderText();
            this._orderService.UpdateOrderHTML(this.esr.Id, this.orderText).subscribe((x) => {
                this.showOrder = true;
            });
        }
        console.log(this.esrView);
        console.log(this.esrView2);
    }

    insertEditorScript() {
        let script = document.querySelector('script[src="https://cdn.ckeditor.com/4.5.11/full-all/ckeditor.js"]');
        if (!script) {
            var externalScript = document.createElement('script');
            externalScript.setAttribute('src', 'https://cdn.ckeditor.com/4.5.11/full-all/ckeditor.js');
            document.head.appendChild(externalScript);
        }
        this.showOrder = true;
    }
    textChanged(value: any) {
        if (this._tnpServie.esrView.TransferTypeID == 5) {
            this.leaveOrder.OrderHTML = value;
            this._orderService.UpdateLeaveOrderHTML(this.leaveOrder).subscribe((x) => {
            });
        } else {
            this._orderService.UpdateOrderHTML(this.esr.Id, value).subscribe((x) => {
            });
        }
    }
    SaveOrderHtml() {
        if (this._tnpServie.esrView.TransferTypeID == 5) {
            this.leaveOrder.OrderHTML = this.orderText;
            this._orderService.UpdateLeaveOrderHTML(this.leaveOrder).subscribe((x) => {
                //this._orderService.printHTML(this.orderText);
            });
        } else {
            this._orderService.UpdateOrderHTML(this.esr.Id, this.orderText).subscribe((x) => {
                // this._orderService.printHTML(this.orderText);
            });
        }
    };
    /* , on ${this._tnpServie.gender === 0 ? 'his' : 'her'} own request with immediate effect.</span> */
    getGeneralOrderText(): string {
        return `
            <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em>,
                ${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>),
                ${this.esrView.DesigFrom} (BS-${this.esrView.BPSFrom}/Regular), ${this.esrView.HF_From},</strong> is hereby
              transferred and posted at&nbsp;<strong>${this.esrView.HF_TO},</strong> against a vacant
              post&nbsp;of&nbsp;<strong>${this.esrView.DesigTo} (BS-${this.esrView.BPSTo}/Regular)</strong>${this.esrView.COMMENTS}, with immediate effect.
              </span>
            </p>
        `;
    }
    getAtDisposalOrderText(): string {
        return `
        <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em>,
        </strong>The Services of <strong>${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>),
            ${this.esrView.DesigFrom} (BS-${this.esrView.BPSFrom}/Regular), ${this.esrView.HF_From},</strong> are hereby transferred and placed at the
          disposal of <strong>${this.esrView.HF_TO ? this.esrView.HF_TO : this.esrView.Disposalof},</strong> for further posting, with immediate effect.
        </p>
        `;
    }
    getResignationOrderText(): string {
        return `
        <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}(Resignation)</u></em>,</strong>
        The resignation tendered by <strong>${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>), ${this.esrView.DesigFrom} (BS-${this.esrView.BPSFrom})</strong>
        formerly posted at <strong>${this.esrView.HF_From}</strong> is hereby accepted w.e.f, on his/her own request.
        </p>
        `;
    }
    getSuspendOrderText(): string {
        return `

        <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em>,
        </strong>The Services of <strong>${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>),
            ${this.esrView.DesigFrom} (BS-${this.esrView.BPSFrom}), ${this.esrView.HF_From},</strong> are hereby suspended.
        </p>

       
        `;
    }
    getReportBackOrderText(): string {
        return `

        <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em>,
            ${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>),
            ${this.esrView.DesigFrom} (BS-${this.esrView.BPSFrom}), ${this.esrView.HF_From},</strong> is hereby transferred and directed to report back to Primary & Secondary Healthcare Department, with immediate effect.
        </p>`;
    }
    getMutualOrderText(): string {
        return `
        <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em>,
    </p>
            <table  border="1" cellspacing="0" style="width:100%">
                <tbody>
                    <tr>
                        <td style="text-align: center;"><strong>Name/CNIC</strong></td>
                        <td style="text-align: center;"><strong>Designation</strong></td>
                        <td style="text-align: center;"><strong>Current Posting</strong></td>
                        <td style="text-align: center;"><strong>Transferred at</strong></td>
                    </tr>
                    <tr>
                        <td style="text-align: center;"><strong>${this._tnpServie.esrView.EmployeeName} </strong>(<strong>CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>)</td>
                        <td style="text-align: center;"><strong>${this.esrView.DesigFrom} (BS-${this.esrView.BPSFrom})</strong></td>
                        <td style="text-align: center;"><strong>${this.esrView.HF_From}</strong></td>
                        <td style="text-align: center;"><strong>${this.esrView2.HF_From}</strong></td>
                    </tr>
                    <tr>
                        <td style="text-align: center;"><strong>${this.esrView2.EmployeeName} </strong>(<strong>CNIC: </strong>${this.addDashes(this.esrView2.CNIC)}<strong>)</td>
                        <td style="text-align: center;"><strong>${this.esrView2.DesigFrom} (BS-${this.esrView2.BPSFrom})</strong></td>
                        <td style="text-align: center;"><strong>${this.esrView2.HF_From}</strong></td>
                        <td style="text-align: center;"><strong>${this.esrView.HF_From}</strong></td>
                    </tr>
                </tbody>
            </table>

    
                          
                      `.replace(' ,', ',');
    }
    getAwaitingPostingText(): string {
        return `
            <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em>,
                ${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>), ${this.esrView.DesigFrom} (BS-${this.esrView.BPSTo}/Regular/Awaiting Posting), </strong> is hereby posted at&nbsp;<strong>${this.esrView.HF_TO},</strong> against a
              vacant post&nbsp;of&nbsp;<strong>${this.esrView.DesigTo} (BS-${this.esrView.BPSTo}),</strong> with immediate effect${this.esrView.COMMENTS}.

              </span>
            </p>
            `;
    }
    getLeaveOrderText(): string {
        return `
            <p style="width:600px; text-align: justify;">
              <strong>No.<em><u>${this.leaveOrderView.FileNumber}</u></em></strong>,
              Sanction is hereby accorded to the grant of <strong>${this.leaveOrderView.TotalDays ?
                this.leaveOrderView.TotalDays == 1 ? this.leaveOrderView.TotalDays + ' day' :
                    this.leaveOrderView.TotalDays + ' days' : ''} ${this.leaveOrderView.LeaveTypeName}</strong>
              w.e.f
              <u><strong>${this.convertDate(this.leaveOrderView.FromDate)}</strong></u>
              to
              <u><strong>${this.convertDate(this.leaveOrderView.ToDate)}</strong></u> in favour of <strong>
                ${this.leaveOrderView.EmployeeName}</strong> <strong>(CNIC</strong> :
              ${this.addDashes(this.leaveOrderView.CNIC)}<strong>), ${this.leaveOrderView.Designation} (BS-${this.leaveOrderView.Scale}),
                ${this.facilityName}</strong> under the
              Revised Leave Rules, 1981.
              <br>
              The Department has no objection to his proceeding abroad during the said leave

              </span>
            </p>
                ${this.leaveOrderView.LeaveType_Id == 2 || this.leaveOrderView.LeaveType_Id == 10 || this.leaveOrderView.LeaveType_Id == 11 || this.leaveOrderView.LeaveType_Id == 12 ? this.getExPakistanLeaveOrderText() : ''}
        `;
        /* ,
              Tehsil <strong>${this.tehsilName}</strong>, District <strong>${this.districtName}</strong> */
    }
    getExPakistanLeaveOrderText(): string {
        return `
            <p style="width:600px; text-align: justify;">
            Primary & Secondary Healthcare Department, Government of the
                        Punjab has no objection to his/her proceeding abroad during the said leave provided that there
                        will be no financial liability on part of Government of the Punjab.
            </p>
            
        `;
        /* ,
              Tehsil <strong>${this.tehsilName}</strong>, District <strong>${this.districtName}</strong> */
    }
    getNotoficationText(): string {
        return `
        <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em>,
            ${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>),
            ${this.esrView.DesigFrom} (BS-${this.esrView.BPSFrom}), ${this.esrView.HF_From},</strong> is hereby notified
          ${this.esrView.COMMENTS}.

          </span>
        </p>
        `.replace(' ,', ',');
    }
    getAdhocText(): string {
        return `
        <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em>,
            ${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>)</strong>, is hereby appointed
          as <strong>${this.esrView.DesigTo} (BS-${this.esrView.BPSTo})</strong>, on adhoc basis ${this.esrView.AppointmentEffect ?
                'with immediate effect' : '<strong>w.e.f ' + this.convertDate(this.esrView.AppointmentDate)}</strong> at
          <strong>${this.esrView.HF_TO}</strong> for a period of <strong>one year</strong> or till the availability of regular incumbent/selectee of the Punjab
          Public Service Commission, Lahore, whichever is earlier.<br>
          <span class="Apple-tab-span" style="white-space: pre;"> </span>&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; The
          other Terms and Conditions of adhoc appointment are as under:-<br>
          <strong>i.</strong><span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>The
          appointment on adhoc basis shall be subject to revocation at any time by the &nbsp;competent Authority.<br>

          <strong>ii.</strong><span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>Adhoc
          appointment, thus made shall not confer any right on the appointee in the matter of regular appointment to the
          same post nor the service shall count towards seniority in the grade.<br>

          <strong>iii.</strong><span class="Apple-tab-span" style="white-space: pre;"> </span>You shall be governed by
          Civil Servants Act, 1974 and any other such rules and orders, relating to leave, travelling allowances,
          medical attendance, pay etc. as may be issued by Government from time to time.<br>

          <strong>iv.</strong><span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>The
          appointment order is subject to verification of documents/ degrees from the issuing authority and
          <strong>PMDC</strong>&nbsp;registration certificate by the issuing authority. The appointment shall be
          cancelled and all the salaries/financial benefits drawn by the appointee shall be recovered through legal
          process. In case your documents are found to be fake/forged besides legal action shall also be
          initiated.&nbsp;<br>

          <strong>v.</strong><span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>In case
          you wish to resign at any time one month's notice shall be mandatory or in lieu of that one month's pay shall
          have to be deposited.&nbsp;<br>

          <strong>vi.</strong><span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>You
          shall furnish Registration Certificate issued by Pakistan Medical and Dental Council, at the time of joining
          to Controlling Authority.<br>

          <strong>vii.</strong><span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>The appointment is subject to the verification of the character and antecedents by the
          Police Authorities.<br>

          <strong>viii.</strong><span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>The
          offer is valid for fifteen days from the date of its receipt and shall automatically lapse if fail to report
          for duty. In<span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>case you accept
          this offer you may report for the duty to
          <strong>Chief Executive Officer (DHA), ${this.esr.DistrictTo}.</strong><br>
          <span class="Apple-tab-span" style="white-space: pre;"> </span></span>

          </span>
        </p>
        <br>
        `.replace(' ,', ',');
    }
    getConsultantText(): string {
        return `
        <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em></strong>, On
          the recommendations of Consultant Selection Committee,
          <strong>${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>)</strong> is hereby
          appointed as <strong>Consultant</strong> in <strong>Primary & Secondary Healthcare Department</strong> for a
          period of <strong>${this.esrDetail.PeriodDuration}</strong> with effect from
          <strong>${this.esrDetail.StartDate}</strong> against Fee of <strong>Rs. ${this.esrDetail.Salary}/- (Per
            Month)</strong> after observing the procedure, as laid down in <strong>PPRA Rules, 2014</strong>.
          </span>
        </p>
        `.replace(' ,', ',');
    }
    getTerminationText(): string {
        return `
        <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em>,
            ${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>),
            ${this.esrView.DesigFrom} (BS-${this.esrView.BPSFrom}), ${this.esrView.HF_From},</strong> is hereby terminated
          ${this.esrView.COMMENTS}.

          </span>
        </p>
        `.replace(' ,', ',');
    }
    getCorrigendumText(): string {
        return `
        <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em>,
            ${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>),
            ${this.esrView.DesigFrom} (BS-${this.esrView.BPSFrom}), ${this.esrView.HF_From},</strong> is hereby notified
          ${this.esrView.COMMENTS}.

          </span>
        </p>
        `.replace(' ,', ',');
    }
    getCustomOrderText(): string {
        return `<p style="text-align:justify"><strong>No.<em><u>000</u></em>, Employee Name (CNIC:
          </strong>00000-0000000-0<strong>), Designation Name (BS-00), Health Facility Name, City Name,
            District,</strong> is hereby transferred and posted at&nbsp;<strong>To Health Facility Name, City Name,
            District,</strong> against a vacant post&nbsp;of&nbsp;<strong>Vacant Post Name (BS-00)</strong>, on his
          own request with immediate effect.</p>`.replace(' ,', ',');
    }
    getOrderText(): string {
        return `
        <style>
        body {
           
            font-family: Verdana;
            font-size: 12px;
            font-style: normal;
            font-variant: normal;
            font-weight: 400;
            line-height: 18px;
            color: #383e4b;
            background-color: #fff;
        }
        p{
            word-wrap: break-word;
            text-align: justify;
            font-family: Verdana;
            font-size: 12px;
            font-style: normal;
            font-variant: normal;
            font-weight: 400;
            line-height: 18px;
        }
        table { page-break-inside:auto }
   tr    { page-break-inside:avoid; page-break-after:auto }
        </style>
        <div style="padding:10px 50px 0 50px;">
        ${this.watermark}
       
        <table border="0" style="width:100%;border-color:transparent !important;" cellspacing="0">
      <tr>
        <td style="text-align:left;border-color:transparent !important;">
         ${this.orderLogo}
        </td>
        <td style="text-align:right;border-color:transparent !important;">
          <div style="display:inline-block;text-align:center;">
            <strong>
            ${this.isHisduAdmin ? 'GOVERNMENT OF THE PUNJAB<br /> PRIMARY &amp; SECONDARY HEALTHCARE<br /> DEPARTMENT' :
                this.isDGH ? 'DIRECTOR GENERAL HEALTH SERVICES<br/> PUNJAB' :
                    this.isPHFMC ? 'PUNJAB HEALTH FACILITIES MANAGEMENT COMPANY' :
                        'OFFICE OF THE CHIEF EXECUTIVE OFFICER<br /> DISTRICT HEALTH AUTHORITY<br />' + this.esr.DistrictFrom} 
                       
                    </strong>
            <br />
            <span style="display:inline-block;padding-top:10px;">Dated ${this.isHisduAdmin || this.isDGH || this.isPHFMC
                ? 'Lahore' : this.esr.DistrictFrom}, the ${this.dateNow}</span>
            </div>
        </td>
      </tr>
      <tr>
        <td colspan="2" style="border-color:transparent !important;text-align:left;padding:30px 0;">
         <p style="margin-top: 30px;"><strong><u>${this.orderType}</u></strong></p>
        </td>
      </tr>
      <tr>
        <td colspan="2" style="border-color:transparent !important;text-align:left;">
        ${this.getOrderBodyText()}
            </td>
    </tr>
    <tr>
    <td colspan="2" style="text-align:right;padding:20px 0;border-color:transparent !important;">
        <div style="text-align:center; display:inline-block;">
            <strong>
            ${this.isHisduAdmin || this.isPHFMC ? 'SECRETARY<br /> PRIMARY &amp; SECONDARY HEALTHCARE<br /> DEPARTMENT' :
                this.isDGH ? 'Sd/-<br/>DIRECTOR GENERAL HEALTH SERVICES<br/>PUNJAB LAHORE' :
                    /* this.isPHFMC ? 'PUNJAB HEALTH FACILITIES MANAGEMENT COMPANY' : */
                    !this.isDGH && !this.isHisduAdmin && !this.isPHFMC ? 'Sd/-<br/>CHIEF EXECUTIVE OFFICER<br />DISTRICT HEALTH AUTHORITY<br>' + this.esr.DistrictFrom : ''
            }
</strong>
    </div>
    </td>
    </tr>
    <tr>
    <td colspan="2" style="text-align:center;padding:30px 0;border-color:transparent !important;">
        <p></p>
        <img src="${this.barcodeSrc}" alt="IMAGE AREA" style="border-width:0px">
            <br>
            <span><strong>${this._tnpServie.esrView.TransferTypeID == 5 ? 'ELR-' + (this.leaveOrder.Id + 1003) : 'ESR-' + this.esr.Id} </strong></span>
                </td>
                </tr>
                <tr>
                <td colspan="2" style="text-align:left;padding:10px 0;border-color:transparent !important;">
                    <br>
                    <strong><u>Number & Date Even</u></strong>
                        <p>A copy is forwarded for information and necessary action to the: </p>
                            <ol style="font-family: Verdana;font-size: 12px;font-style: normal;font-variant: normal;font-weight: 400;line-height: 18px;">
                                ${this.markupCC}
</>
    </td>
    </tr>
    <tr>
    <td colspan="2" style="font-size:12px;text-align:right;border-color:transparent !important;">
        <div style="text-align:center; display:inline-block;">
            <strong>
            ${this.esr.officerStamp ? this.esr.officerStamp + '<br>' : ''}
            ${this.isHisduAdmin ? 'Section' : ''} (${this.esrView.SectionOfficer.replace('Section ', '').replace('(', '').replace(')', '')})<br>
            ${!this.isDGH && !this.isHisduAdmin && !this.isPHFMC ? 'DISTRICT HEALTH AUTHORITY<br/>' + this.esr.DistrictFrom : ''}
            ${this.isDGH ? 'DIRECTOR GENERAL HEALTH SERVICES<br/>PUNJAB LAHORE' : ''}
</strong>
    </div>
    </td>
    </tr>
                          <tr>
                            <td style="border-color:transparent !important;">
                            <br><br>
                <img src="${this.qrcodeSrc}" alt="IMAGE AREA" style="border-width:0px;width:100px;">
    </td>
    </tr>
    <tr>
    <td colspan="2" style="border-color:transparent !important;text-align: center;">
    <br><br>
    <u>This document is System Generated, There is no need for signature. This document can be verified through QR Code</u>
    </span>            
    </td>

            </tr>
</table>
    </div>
    <div class="divFooter" style="position: fixed;bottom: 0; color: #e3e3e3;margin-left:22%;"> Powered by: <strong>Health Information and
Service Delivery Unit(HISDU) </strong></div>
    </div>
        `.replace(' ,', ',');


        /*  ${this.getSectionOfficerNameHtml()}  */
    }
    getOrderBodyText() {
        if (this.orderResponses && this.orderResponses.length > 0) {
            return this.esrView.TransferTypeID == 1 ? this.getCombineMutualOrderText() :
                this.esrView.TransferTypeID == 2 ? this.getCombineAtDisposalOrderText() :
                    this.esrView.TransferTypeID == 4 ? this.getCombineGeneralOrderText() :
                        this.esrView.TransferTypeID == 3 ? this.getCombineSuspendOrderText() :
                            this.esrView.TransferTypeID == 5 ? this.getCombineLeaveOrderText() :
                                this.esrView.TransferTypeID == 6 ? this.getCombineAwaitingPostingText() :
                                    this.esrView.TransferTypeID == 7 ? this.getCombineNotoficationText() :
                                        this.esrView.TransferTypeID == 8 ? this.getCombineAdhocText() :
                                            this.esrView.TransferTypeID == 9 ? this.getCombineConsultantText() :
                                                this.esrView.TransferTypeID == 10 ? this.getCombineTerminationText() :
                                                    this.esrView.TransferTypeID == 11 ? this.getCombineCorrigendumText() : ''
        } else {
            return this.esrView.TransferTypeID == 1 ? this.getMutualOrderText() :
                this.esrView.TransferTypeID == 2 ? this.getAtDisposalOrderText() :
                    this.esrView.TransferTypeID == 4 ? this.getGeneralOrderText() :
                        this.esrView.TransferTypeID == 3 ? this.getSuspendOrderText() :
                            this.esrView.TransferTypeID == 5 ? this.getLeaveOrderText() :
                                this.esrView.TransferTypeID == 6 ? this.getAwaitingPostingText() :
                                    this.esrView.TransferTypeID == 7 ? this.getNotoficationText() :
                                        this.esrView.TransferTypeID == 8 ? this.getAdhocText() :
                                            this.esrView.TransferTypeID == 9 ? this.getConsultantText() :
                                                this.esrView.TransferTypeID == 10 ? this.getTerminationText() :
                                                    this.esrView.TransferTypeID == 11 ? this.getReportBackOrderText() :
                                                        this.esrView.TransferTypeID == 12 ? this.getResignationOrderText() : ''
        }
    }
    addDashes(f_val) { return f_val.slice(0, 5) + "-" + f_val.slice(5, 12) + "-" + f_val.slice(12); }
    makeOrderDate(day: number) {
        return day == 1 || day == 21 || day == 31 ? day + '<sup>st</sup>'
            : day == 2 || day == 22 ? day + '<sup>nd</sup>'
                : day == 3 || day == 23 ? day + '<sup>rd</sup>'
                    : (day => 4 && day <= 20) || (day => 24 && day <= 30) ? day + '<sup>th</sup>' : '';
    }
    makeOrderMonth(month: number) {
        return month == 1 ? 'January'
            : month == 2 ? 'February'
                : month == 3 ? 'March'
                    : month == 4 ? 'April'
                        : month == 5 ? 'May'
                            : month == 6 ? 'June'
                                : month == 7 ? 'July'
                                    : month == 8 ? 'August'
                                        : month == 9 ? 'September'
                                            : month == 10 ? 'October'
                                                : month == 11 ? 'November'
                                                    : month == 12 ? 'December' : '';
    }
    convertDate(inputFormat) {
        console.log(inputFormat);
        var d = new Date(inputFormat);
        return [this.pad(d.getDate()), this.pad(d.getMonth() + 1), d.getFullYear()].join('.');
    }
    private pad(s) { return (s < 10) ? '0' + s : s; }
    getSectionOfficerNameHtml(): string {
        let name = this.getSectionOfficerName();
        if (name) {
            name = `(${name.toUpperCase()})`;
        }
        return name;
    }
    getSectionOfficerName(): string {
        let name: string = "";
        if (this.esr.SectionOfficer == "Additional Secretary (Admn)") {
            name = "Ammara Khan";
        }
        if (this.esr.SectionOfficer == "Deputy Secretary (Admn)") {
            name = "Sh. Muhammad Tanzeel Ur Rehman";
        }
        if (this.esr.SectionOfficer == "Additional Secretary (Technical)") {
            name = "Dr. Asim Altaf";
        }
        // if(this.esr.SectionOfficer == "Deputy Secretary (General)"){
        //     name = "Zahida Izhar";
        // }

        return name;
    }
    getCombineGeneralOrderText(): string {
        let text = `<p style="width:600px; text-align: justify;" > <strong>No.<em > <u>${this.esrView.EmployeeFileNO} </u></em > </strong>
    </p>`;
        for (let index = 0; index < this.orderResponses.length; index++) {
            const orderResponse = this.orderResponses[index];
            text += `
            <p style="width:600px; text-align: justify;"><strong>${index > 0 ? (index + 1) + '.' : ''} ${orderResponse.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(orderResponse.esrView.CNIC)}<strong>),
            ${orderResponse.esrView.DesigFrom} (BS-${orderResponse.esrView.BPSFrom}/Regular), ${orderResponse.esrView.HF_From},</strong> is hereby
          transferred and posted at&nbsp;<strong>${orderResponse.esrView.HF_TO},</strong> against a vacant
          post&nbsp;of&nbsp;<strong>${orderResponse.esrView.DesigTo} (BS-${orderResponse.esrView.BPSTo}/Regular)</strong>.

        </p>
                    `;
        }
        /*  let text = `
         <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em></strong>
         </p>
         
         <table border="1" style="width:100%;" cellspacing="0">
        <tr>
        <th style="border-color:black !important; text-align:center;padding: 8px !important;">Employee </th>
        <th style="border-color:black !important; text-align:center;padding: 8px !important;"> Posted at </th>
         <th style="border-color:black !important; text-align:center;padding: 8px !important;"> Transferred to </th>
             </tr>`;
         this.orderResponses.forEach(orderResponse => {
             text += `
             
                       <tr>
                         <td style="border-color:black !important; text-align:center;padding: 8px !important;">
                           <p style="text-align:center;margin: 0px !important;">${orderResponse.esrView.EmployeeName ? orderResponse.esrView.EmployeeName : ''}</p>
                           <p style="text-align:center;margin: 0px !important;">${orderResponse.esrView.CNIC ? this.addDashes(orderResponse.esrView.CNIC) : ''}</p>
                         </td>
                         <td style="border-color:black !important; text-align:center;padding: 8px !important;">
                           <p style="text-align:center;margin: 0px !important;">
                             ${orderResponse.esrView.DesigFrom ? orderResponse.esrView.DesigFrom : ''}/BS-${orderResponse.esrView.BPSFrom},
                           </p>
                           <p style="text-align:center;margin: 0px !important;">${orderResponse.esrView.HF_From ? orderResponse.esrView.HF_From : ''}</p>
                         </td>
                         <td style="border-color:black !important; text-align:center;padding: 8px !important;">
                           <p style="text-align:center;margin: 0px !important;">
                             ${orderResponse.esrView.DesigTo ? orderResponse.esrView.DesigTo : ''}/BS-${orderResponse.esrView.BPSTo},
                           </p>
                           <p style="text-align:center;margin: 0px !important;">${orderResponse.esrView.HF_TO ? orderResponse.esrView.HF_TO : ''}</p>
                         </td>
                       </tr>
                     `;
         }); */
        return text;
    }
    getCombineAtDisposalOrderText(): string {
        let text = ''; this.orderResponses.forEach(orderResponse => {
            text += `
        <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em>,
        </strong>The Services of <strong>${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>),
            ${this.esrView.DesigFrom} (BS-${this.esrView.BPSFrom}/Regular), ${this.esrView.HF_From},</strong> are hereby placed at the
          disposal of <strong>${this.esrView.HF_TO},</strong> for further posting, with immediate effect.
        </p>
        ` }); return text;
    }
    getCombineSuspendOrderText(): string {
        let text = ''; this.orderResponses.forEach(orderResponse => {
            text += `

        <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em>,
        </strong>The Services of <strong>${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>),
            ${this.esrView.DesigFrom} (BS-${this.esrView.BPSFrom}), ${this.esrView.HF_From},</strong> are hereby suspended.
        </p>

       
        ` }); return text;
    }
    getCombineMutualOrderText(): string {
        let text = ''; this.orderResponses.forEach(orderResponse => {
            text += `
            
            <table  border="1" cellpadding="1" cellspacing="1" style="width:600px">
                <tbody>
                    <tr>
                        <td style="text-align: center;"><strong>Name/CNIC</strong></td>
                        <td style="text-align: center;"><strong>Designation</strong></td>
                        <td style="text-align: center;"><strong>Current Posting</strong></td>
                        <td style="text-align: center;"><strong>Transferred at</strong></td>
                    </tr>
                    <tr>
                        <td style="text-align: center;"><strong>${this._tnpServie.esrView.EmployeeName} </strong>(<strong>CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>)</td>
                        <td style="text-align: center;"><strong>${this.esrView.DesigFrom} (BS-${this.esrView.BPSFrom})</strong></td>
                        <td style="text-align: center;"><strong>${this.esrView.HF_From}</strong></td>
                        <td style="text-align: center;"><strong>${this.esrView2.HF_From}</strong></td>
                    </tr>
                    <tr>
                        <td style="text-align: center;"><strong>${this.esrView2.EmployeeName} </strong>(<strong>CNIC: </strong>${this.addDashes(this.esrView2.CNIC)}<strong>)</td>
                        <td style="text-align: center;"><strong>${this.esrView2.DesigFrom} (BS-${this.esrView2.BPSFrom})</strong></td>
                        <td style="text-align: center;"><strong>${this.esrView2.HF_From}</strong></td>
                        <td style="text-align: center;"><strong>${this.esrView.HF_From}</strong></td>
                    </tr>
                </tbody>
            </table>

    
                          
                      `.replace(' ,', ',')
        }); return text;
    }
    getCombineAwaitingPostingText(): string {
        let text = `<p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em></strong>
        </p>`;
        for (let index = 0; index < this.orderResponses.length; index++) {
            const orderResponse = this.orderResponses[index];
            text += `
            <p style="width:600px; text-align: justify;"><strong>${index > 0 ? (index + 1) + '.' : ''} ${orderResponse.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(orderResponse.esrView.CNIC)}<strong>), ${this.esrView.DesigFrom} (BS-${orderResponse.esrView.BPSTo}/Regular/Awaiting Posting), </strong> is hereby posted at&nbsp;<strong>${orderResponse.esrView.HF_TO},</strong> against a
              vacant post&nbsp;of&nbsp;<strong>${orderResponse.esrView.DesigTo} (BS-${orderResponse.esrView.BPSTo})</strong>, with immediate effect.

            </p>
            `   ;
        } return text;
    }
    getCombineLeaveOrderText(): string {
        let text = ''; this.orderResponses.forEach(orderResponse => {
            text += `
                <p style="width:600px; text-align: justify;">
                    <strong>No.<em> <u>${this.leaveOrderView.FileNumber} </u></em> </strong>,
            Sanction is hereby accorded to the grant of <strong> ${this.leaveOrderView.TotalDays ?
                    this.leaveOrderView.TotalDays == 1 ? this.leaveOrderView.TotalDays + ' day' :
                        this.leaveOrderView.TotalDays + ' days' : ''
                } ${this.leaveOrderView.LeaveTypeName} </strong>
            leave w.e.f
                <u> <strong>${this.convertDate(this.leaveOrderView.FromDate)} </strong></u>
                    to
                    <u> <strong>${this.convertDate(this.leaveOrderView.ToDate)} </strong></u> in favour of <strong>
                        Mr / Ms.${this.leaveOrderView.EmployeeName} </strong> <strong>(CNIC</strong> :
            ${this.addDashes(this.leaveOrderView.CNIC)} <strong>), ${this.leaveOrderView.Designation} (BS-${this.leaveOrderView.Scale}),
            ${this.facilityName} </strong> under the
            Revised Leave Rules, 1981.
                <br>
                The Department has no objection to his proceeding abroad during the said leave

                    </span>
                    </p>
            ${this.leaveOrderView.LeaveType_Id == 2 || this.leaveOrderView.LeaveType_Id == 10 || this.leaveOrderView.LeaveType_Id == 11 || this.leaveOrderView.LeaveType_Id == 12 ? this.getCombineExPakistanLeaveOrderText() : ''}
            ` }); return text;
        /* ,
              Tehsil <strong>${this.tehsilName}</strong>, District <strong>${this.districtName}</strong> */
    }
    getCombineExPakistanLeaveOrderText(): string {
        let text = ''; this.orderResponses.forEach(orderResponse => {
            text += `
                <p style="width:600px; text-align: justify;">
                    Primary & Secondary Healthcare Department, Government of the
            Punjab has no objection to his / her proceeding abroad during the said leave provided that there
            will be no financial liability on part of Government of the Punjab.
            </p>

                    ` }); return text;
        /* ,
              Tehsil <strong>${this.tehsilName}</strong>, District <strong>${this.districtName}</strong> */
    }
    getCombineNotoficationText(): string {
        let text = ''; this.orderResponses.forEach(orderResponse => {
            text += `
                <p style="width:600px; text-align: justify;"> <strong>No.<em> <u>${this.esrView.EmployeeFileNO} </u></em>,
                    ${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>),
                        (${this.esrView.DesigFrom} / BS - ${this.esrView.BPSFrom}), ${this.esrView.HF_From}, </strong> is hereby notified
            ${this.esrView.COMMENTS}.

            </span>
                </p>
                    `.replace(' ,', ',')
        }); return text;
    }
    getCombineAdhocText(): string {
        let text = `
                <p style="width:600px; text-align: justify;"> <strong>No.<em> <u>${this.esrView.EmployeeFileNO} </u></em>, </strong> The following doctors are hereby appointed as 
            Medical Officer(BS-17) on adhoc basis and posted at place mentioned against their
            names with immediate effect, for a period of <strong>one year</strong> or till the availability of regular
            incumbents/selectees of the Punjab Public Service Commission, whichever is earlier: -</p>

                <table border = "1" style="width:100%;line-height: 1.5;" cellspacing = "0">
                    <tr>
                    <th style="border-color:black !important; text-align:center;padding: 8px !important;"> S.No </th>
                        <th style="border-color:black !important; text-align:center;padding: 8px !important;"> Name of Doctor </th>
                            <th style="border-color:black !important; text-align:center;padding: 8px !important;"> Place of Posting </th>
            ${this.esrView.AppointmentEffect ? '' : '<th style="border-color:black !important; text-align:center;padding: 8px !important;"> w.e.f </th>'}
            </tr>`;
        for (let index = 0; index < this.orderResponses.length; index++) {
            const orderResponse = this.orderResponses[index];
            text += `

            <tr>
            <td style="border-color:black !important; text-align:center;padding: 8px !important;">
              <p style="text-align:center;margin: 0px;">${(index + 1)}</p>
            </td>
            <td style="border-color:black; text-align:center;padding: 8px;">
              <p style="text-align:center;margin: 0px;">${orderResponse.esrView.EmployeeName ? orderResponse.esrView.EmployeeName : ''}, <strong>(CNIC:</strong>${orderResponse.esrView.CNIC ? this.addDashes(orderResponse.esrView.CNIC) : ''}<strong>)</strong></p>
            </td>
            <td style="border-color:black; text-align:center;padding: 8px;">
              <p style="text-align:center;margin: 0px;">
                ${orderResponse.esrView.DesigTo ? orderResponse.esrView.DesigTo : ''}/BS-${orderResponse.esrView.BPSTo},
              </p>
              <p style="text-align:center;margin: 0px;">${orderResponse.esrView.HF_TO ? orderResponse.esrView.HF_TO : ''}</p>
            </td>
        ${this.esrView.AppointmentEffect ? '' : `
        <td style="border-color:black; text-align:center;padding: 8px;">
              <p style="text-align:center;margin: 0px;">
              ${orderResponse.esrView.AppointmentEffect ? '' : '<strong>' + this.convertDate(orderResponse.esrView.AppointmentDate) + '</strong>'}
              </p>
            </td>
        `}
            </tr>`
        }

        text += `</table><p style="width:600px; text-align: justify;">The
          other Terms and Conditions of adhoc appointment are as under:-<br>
          <strong>i.</strong><span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>The
          appointment on adhoc basis shall be subject to revocation at any time by the &nbsp;competent Authority.<br>

          <strong>ii.</strong><span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>Adhoc
          appointment, thus made shall not confer any right on the appointee in the matter of regular appointment to the
          same post nor the service shall count towards seniority in the grade.<br>

          <strong>iii.</strong><span class="Apple-tab-span" style="white-space: pre;"> </span>You shall be governed by
          Civil Servants Act, 1974 and any other such rules and orders, relating to leave, travelling allowances,
          medical attendance, pay etc. as may be issued by Government from time to time.<br>

          <strong>iv.</strong><span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>The
          appointment order is subject to verification of documents/ degrees from the issuing authority and
          <strong>PMDC</strong>&nbsp;registration certificate by the issuing authority. The appointment shall be
          cancelled and all the salaries/financial benefits drawn by the appointee shall be recovered through legal
          process. In case your documents are found to be fake/forged besides legal action shall also be
          initiated.&nbsp;<br>

          <strong>v.</strong><span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>In case
          you wish to resign at any time one month's notice shall be mandatory or in lieu of that one month's pay shall
          have to be deposited.&nbsp;<br>

          <strong>vi.</strong><span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>You
          shall furnish Registration Certificate issued by Pakistan Medical and Dental Council, at the time of joining
          to Controlling Authority.<br>

          <strong>vii.</strong><span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>The appointment is subject to the verification of the character and antecedents by the
          Police Authorities.<br>

          <strong>viii.</strong><span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>The
          offer is valid for fifteen days from the date of its receipt and shall automatically lapse if fail to report
          for duty. In<span class="Apple-tab-span" style="font-weight: bold; white-space: pre;"> </span>case you accept
          this offer you may report for the duty to
          <strong>Chief Executive Officer (DHA), concerned.</strong><br>
          <span class="Apple-tab-span" style="white-space: pre;"> </span></span>
          </span>
        </p>
        `.replace(' ,', ',');
        return text;
    }
    getCombineConsultantText(): string {
        let text = ''; this.orderResponses.forEach(orderResponse => {
            text += `
        <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em></strong>, On
          the recommendations of Consultant Selection Committee,
          <strong>${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>)</strong> is hereby
          appointed as <strong>Consultant</strong> in <strong>Primary & Secondary Healthcare Department</strong> for a
          period of <strong>${this.esrDetail.PeriodDuration}</strong> with effect from
          <strong>${this.esrDetail.StartDate}</strong> against Fee of <strong>Rs. ${this.esrDetail.Salary}/- (Per
            Month)</strong> after observing the procedure, as laid down in <strong>PPRA Rules, 2014</strong>.
          </span>
        </p>
        `.replace(' ,', ',')
        }); return text;
    }
    getCombineTerminationText(): string {
        let text = ''; this.orderResponses.forEach(orderResponse => {
            text += `
        <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em>,
            ${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>),
            ${this.esrView.DesigFrom} (BS-${this.esrView.BPSFrom}), ${this.esrView.HF_From},</strong> is hereby terminated
          ${this.esrView.COMMENTS}.

          </span>
        </p>
        `.replace(' ,', ',')
        }); return text;
    }
    getCombineCorrigendumText(): string {
        let text = ''; this.orderResponses.forEach(orderResponse => {
            text += `
        <p style="width:600px; text-align: justify;"><strong>No.<em><u>${this.esrView.EmployeeFileNO}</u></em>,
            ${this.esrView.EmployeeName} (CNIC: </strong>${this.addDashes(this.esrView.CNIC)}<strong>),
            ${this.esrView.DesigFrom} (BS-${this.esrView.BPSFrom}), ${this.esrView.HF_From},</strong> is hereby notified
          ${this.esrView.COMMENTS}.

          </span>
        </p>
        `.replace(' ,', ',')
        }); return text;
    }
    getCombineCustomOrderText(): string {
        let text = ''; this.orderResponses.forEach(orderResponse => {
            text += `<p style="text-align:justify"><strong>No.<em><u>000</u></em>, Employee Name (CNIC:
          </strong>00000-0000000-0<strong>), Designation Name (BS-00), Health Facility Name, City Name,
            District,</strong> is hereby transferred and posted at&nbsp;<strong>To Health Facility Name, City Name,
            District,</strong> against a vacant post&nbsp;of&nbsp;<strong>(Vacant Post Name/BS-00)</strong>, on his
          own request with immediate effect.</p>`.replace(' ,', ',')
        }); return text;
    }
}

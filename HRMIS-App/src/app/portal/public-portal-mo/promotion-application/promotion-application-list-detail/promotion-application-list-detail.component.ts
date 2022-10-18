import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PublicPortalMOService } from '../../public-portal-mo.service';

@Component({
  selector: 'hrmis-promotion-application-list-detail',
  templateUrl: './promotion-application-list-detail.component.html',
  styles: [``]
})
export class PromotionApplicationListDetailComponent implements OnInit {

  public promotionJobApplication: any = null;
  public showErrorLabel: any = '';
  public serviceStatements: any[] = [];
  public baseUrl = '';
  constructor(
    private route: ActivatedRoute,
    private common: PublicPortalMOService
  ) {

  }

  ngOnInit() {

    this.route.params.subscribe(
      (params: any) => {

        if (params.hasOwnProperty('id')) {
          let id = +params['id'];

          this.common.getPromoApp(id).subscribe((x: any) => {
            console.log(x);

            if (x.Type == "Success") {

              this.promotionJobApplication = x.Data.PromoApp;
              this.serviceStatements = x.Data.ServiceStatements;

            }
            if (x.Type == "Error") {

              this.showErrorLabel = x.Message;

            }
            if (x.Type == "Exception") {

              this.showErrorLabel = x.Message;
              console.log(x.exception);
            }

          });

        }

      }
    );


  }
  /* printReport(elem: any) {
    this._mainService.printHTML(elem.innerHTML);
  } */
  printReport(elem: any) {
    /* let html = document.getElementById('applicationPrint').innerHTML; */
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(elem.innerHTML);
      mywindow.document.write('</body></html>');
      //show upload signed copy input chooser
      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }
}

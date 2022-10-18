import { Component, AfterViewInit, ViewChild, ElementRef, OnInit, ɵCodegenComponentFactoryResolver } from '@angular/core';
import { switchMap } from 'rxjs/operators/switchMap';
import { from } from 'rxjs/observable/from';
import { delay } from 'rxjs/operators/delay';
import { map } from 'rxjs/operators/map';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { DashboardService } from '../dashboard.service';


@Component({
  selector: 'app-vacancy-charts-map',
  templateUrl: './vacancy-charts-map.component.html',
  styleUrls: ['./vacancy-charts-map.component.scss']
})
export class VacancyChartsMapComponent implements OnInit, AfterViewInit {

  constructor(private _rootService: RootService, private _dashboardService: DashboardService, private _authenticationService: AuthenticationService) { }

  @ViewChild('mapContainer', { static: false }) gmap: ElementRef;
  @ViewChild('hfTypeList', { static: false }) hfTypeList;
  @ViewChild("designationsList", { static: false }) designationsList;
  public map: google.maps.Map;
  public showCounter = 35;
  public zoom = 7;
  public red = 0;
  public yellow = 0;
  public green = 0;
  public districtred = 0;
  public districtyellow = 0;
  public districtgreen = 0;
  public lat = 31.1704;
  public lng = 72.7097;
  public vacancyChartData: any = { sanctioned: 0, filled: 0, adhoc: 0, regular: 0, contract: 0, vacant: 0 };
  public vpChartCount: any = {
  };
  public vpChartCounts: any[] = [
    this.vpChartCount.SanctionedInitial,
    this.vpChartCount.SanctionedPromotion,
    this.vpChartCount.AdhocInitial,
    this.vpChartCount.AdhocPromotion,
    this.vpChartCount.RegularInitial,
    this.vpChartCount.RegularPromotion,
    this.vpChartCount.ContractInitial,
    this.vpChartCount.ContractPromotion,
    this.vpChartCount.VacantInitial,
    this.vpChartCount.VacantPromotion
  ];
  public vacancyChart: any = { sanctioned: [], filled: [], adhoc: [], regular: [], contract: [], vacant: [] };
  public vacancyChartDesignations: any[] = [];
  public coordinates = new google.maps.LatLng(this.lat, this.lng);
  public mapOptions: google.maps.MapOptions = {
    center: this.coordinates,
    zoom: 7,
    disableDefaultUI: true,
    styles: [
      /*   {
          "elementType": "labels",
          "stylers": [
            {
              "visibility": "off"
            }
          ]
        }, */
      {
        "featureType": "administrative.land_parcel",
        "stylers": [
          {
            "visibility": "off"
          }
        ]
      },
      {
        "featureType": "administrative.neighborhood",
        "stylers": [
          {
            "visibility": "off"
          }
        ]
      },
      {
        "featureType": "road",
        "stylers": [
          {
            "visibility": "off"
          }
        ]
      }
    ]
  };
  public previous;
  public mapStyles = [
    /*   {
        "elementType": "labels",
        "stylers": [
          {
            "visibility": "off"
          }
        ]
      }, */
    {
      "featureType": "administrative.land_parcel",
      "stylers": [
        {
          "visibility": "off"
        }
      ]
    },
    {
      "featureType": "administrative.neighborhood",
      "stylers": [
        {
          "visibility": "off"
        }
      ]
    },
    {
      "featureType": "road",
      "stylers": [
        {
          "visibility": "off"
        }
      ]
    }
  ]
  public hfCodesMap: any[] = [];
  public surgeries: any[];
  public InforObj: any[] = [];
  public polyLinesObj: any[] = [];
  public chartLabels: any[] = [
    'Santioned Initial',
    'Santioned Promotion',
    'Adhoc Initial',
    'Adhoc Promotion',
    'Regular Initial',
    'Regular Promotion',
    'Contract Initial',
    'Contract Promotion',
    'Vacant Initial',
    'Vacant Promotion'
  ];
  public bounds = new google.maps.LatLngBounds();
  public marker1;
  //dropdowns : need two same arrays for search/filter purpose
  public divisions: any[] = [];
  public divisionsData: any[] = [];
  public districtsRouting: any[] = [];
  public districts: any[] = [];
  public districtsData: any[] = [];
  public tehsils: any[] = [];
  public tehsilsData: any[] = [];
  public hfTypes: any[] = [];
  public hfTypesData: any[] = [];
  public designations: any[] = [];
  public designationsData: any[] = [];
  public selectedDesignations: any[] = [802];
  public hfTypeCodes: any[] = ['011', '012'];
  public gmarkers: any[] = [];
  public profiles: any[] = [];
  public pmarkers: any[] = [];
  public showProfile = false;
  public loadingVpMaster = false;
  public viewAttachedPersons = false;
  public editing = false;
  public intervalSet = false;
  public editFlag = false;
  public show = false;
  public showProfileEmpMode_Id: number = 0;
  public showProfileViewId: number = 0;
  public seriesLabels: any = {
    visible: true, // Note that visible defaults to false
    padding: 3,
    font: "bold 16px Roboto-Bold",
  };
  public valueAxisLabels: any = {
    font: "bold 20px Roboto-Bold"
  };
  public valueAxisLabelsQ: any = {
    font: "bold 14px Roboto-Bold"
  };
  //selected object for drop downs
  public selectedFiltersModel: any = {
    division: { Name: 'Select Division', Code: '0' },
    district: { Name: 'Select District', Code: '0' },
    tehsil: { Name: 'Select Tehsil', Code: '0' }
  };
  public vacancyTech: any = {
    Name: 'Anaesthesia Technologist', Sanctioned: 0, Filled: 0, Vacant: 0, Ahoc: 0, Regular: 0
  }
  public vacancy: any = {
    Name: 'Punjab', Sanctioned: 0, Filled: 0, Vacant: 0, Ahoc: 0, Regular: 0
  }
  public profilesWindow: any = {
    dialogOpened: false,
    data: null,
    profiles: [],
  }
  public hfmisCode: string = '0';
  public dhisCode: string = '1';
  public percents: any = { redStart: 0, redEnd: 10, yellowStart: 11, yellowEnd: 50, greenStart: 51, greenEnd: 100 };
  public dhisFiltetType: string = 'punjab';
  public vpMaps: any[] = [];
  public vpTechs: any[] = [];
  public vpTechsWorking: any[] = [];
  public vpMapsData: any[] = [];
  public attachedPersons: any[] = [];
  public profilesData: any = {};
  public facilityProfilesData: any = {};
  public anaesthesiaTree: any = {};
  public currentUser;
  public loadingMap: boolean = false;
  public pauseShow: boolean = false;
  public showMap: boolean = false;
  public src = 'https://developers.google.com/maps/documentation/javascript/examples/kml/westcampus.kml';
  public geoJsonObject: any;
  public openedWindow: string = '0'; // alternative: array of numbers

  ngOnInit(): void {
    this.currentUser = this._authenticationService.getUser();
    this.hfmisCode = this._authenticationService.getUserHfmisCode();
    this.loadDropdownValues();
    this.getData();
    this.getSurgeries();
    this.getHFCodMap();
    this.getTechnologists();
    this.marker1 = new google.maps.Marker({
      position: this.coordinates,
      map: this.map,
    });
  }
  ngAfterViewInit() {
    // this.mapInitializer();

  }
  round(x: any, n: any) {
    return Math.round(x * Math.pow(10, n)) / Math.pow(10, n)
  }
  public mapInitializer() {
    this.map = new google.maps.Map(this.gmap.nativeElement, this.mapOptions);

    /*    const popable = document.querySelectorAll('.popable');
       let lastClicked;
       popable.forEach(elem => elem.addEventListener('click', togglePopup));
   
       function togglePopup(e) {
         e.preventDefault();
   
         document.getElementById("distP").innerText = e.toElement.alt;
         document.getElementById("asddd").click();
       } */
    /*  var ctaLayer = new google.maps.KmlLayer({
       url: 'https://hrmis.pshealthpunjab.gov.pk/Uploads/Downloads/PunjabDistricts.kml',
       map: this.map
     });
     ctaLayer.addListener('click', function (event) {
      
     }); */

    /* var kmlLayer = new google.maps.KmlLayer();

    var src = 'https://developers.google.com/maps/documentation/javascript/examples/kml/westcampus.kml';
    var kmlLayer = new google.maps.KmlLayer(src, {
      suppressInfoWindows: true,
      preserveViewport: false,
      map: this.map
    }); */

  }
  public getHfs(code) {
    return this.vpMapsData.filter(x => x.Code.substring(0, 6) == code);
  }
  clicked(clickEvent) {
    console.log(clickEvent);

    let dis = this.districts.find(x => x.Name == clickEvent.feature.j.districts);
    if (dis) {
      this.dropdownValueChanged(dis, 'district');
    }
  }
  addMarker() {
    if (this.vpMaps.length < 4) {
      this.map.setZoom(10);
    }
    for (var i = 0; i < this.vpMaps.length; i++) {
      var contentString = `<div id="content">
      <table class="combine-order">
    <tr>
    <th colSpan="4"><img style="height: 120px;width:100%;"
    src="https://hrmis.pshealthpunjab.gov.pk/Uploads/Files/HFPics/${this.vpMaps[i].ImagePath}" alt=""></th>
  </tr>
  <tr>
  <th colSpan="4"><a href="/health-facility/${this.vpMaps[i].Code}" target="_blank">${this.vpMaps[i].Name}</a></th>
</tr>
                      <tr>
                        <th>Designation</th>
                        <th>Sanctioned</th>
                        <th>Filled</th>
                        <th>Vacant</th>
                      </tr>
                      <tr>
                        <td>
                          <p class="m-0">
                          ${this.vpMaps[i].DsgName}
                          </p>
                        </td>
                        <td>
                          <p class="m-0">
                          ${this.vpMaps[i].TotalSanctioned}
                          </p>
                        </td>
                        <td>
                          <p class="m-0">
                          ${this.vpMaps[i].TotalWorking}
                          </p>
                        </td>
                        <td>
                          <p class="m-0">
                          ${this.vpMaps[i].Vacant}
                        </p>
                        </td>
                      </tr>
                    </table>
      </div>`;
      /*  this.vacancy.Name = this.hfmisCode.length < 3 ? 'Punjab' :
         this.hfmisCode.length == 3 ? this.vpMaps[i].DivisionName :
           this.hfmisCode.length == 6 ? this.vpMaps[i].DistrictName :
             this.hfmisCode.length == 9 ? this.vpMaps[i].TehsilName : ''; */
      if (this.vpMaps[i].Latitude > 0) {
        var position = new google.maps.LatLng(this.vpMaps[i].Latitude, this.vpMaps[i].Longitude);
        let numberMarkerImg = {
          url: "https://upload.wikimedia.org/wikipedia/commons/3/3d/Blue_dot.png",
          /*  size: new google.maps.Size(32, 38),
           scaledSize: new google.maps.Size(32, 38), */
          labelOrigin: new google.maps.Point(13, 62)
        };
        let marker: google.maps.Marker;
        if (this.hfmisCode.length > 5) {
          marker = new google.maps.Marker({
            position: position,
            map: this.map,
            icon: numberMarkerImg,/* ,
            animation: google.maps.Animation.DROP */
            label: {
              /* text: this.vpMaps[i].Name.replace('District Head Quarter', 'DHQ').replace('District Headquarter', 'DHQ').replace('Tehsil Head Quarter', 'THQ').replace('Tehsil Headquarter', 'THQ'),
               */text: this.vpMaps[i].TotalWorking + '/' + this.vpMaps[i].TotalSanctioned,
              color: 'black',
              fontSize: '22px',
              fontWeight: 'bold'
            }
          });
        } else {
          marker = new google.maps.Marker({
            position: position,
            map: this.map,
            icon: numberMarkerImg/* ,
            animation: google.maps.Animation.DROP */
          });
        }
        /*   var mapLabel = new google.maps.Lab({
            text: 'Test',
            position: new google.maps.LatLng(34.515233, -100.918565),
            map: map,
            fontSize: 35,
            align: 'right'
        }); */
        //marker.setLabel(this.vpMaps[i].TotalWorking + '');

        //marker.setLabel(this.vpMaps[i].TotalWorking + '/' + this.vpMaps[i].TotalSanctioned);
        this.bounds.extend(position);

        const infowindow = new google.maps.InfoWindow({
          content: contentString,
          maxWidth: 500
        });
        marker.addListener('click', function () {
          infowindow.open(marker.get('map'), marker);
        });
        this.gmarkers.push(marker);
      }
      /* if (this.hfmisCode.length > 5) {
        this.map.fitBounds(this.bounds);
      } */
    }
    /* if (!this.intervalSet) {
      setInterval(() => {
        if (this.showCounter < 36) {
          if (!this.pauseShow)
            this.dropdownValueChanged(this.districts[this.showCounter++], 'district');
        }
      }, 7000);
      this.intervalSet = true;
    } */

    /* this.districts.forEach(element => {
      setTimeout(() => {
        this.dropdownValueChanged(element, 'district');
      }, 5000);
    }); */
  }
  public selectMarker(event: any, window) {
    this.setVpTechnologist();
    let codeMap = this.hfCodesMap.filter(x => x.HrHfId == event.HF_Id);
    this.surgeries = [];
    if (codeMap[0]) {
      this.dhisCode = codeMap[0].DHISHfCode;
      this.dhisFiltetType = 'facility';
      this.getSurgeries();
    }
    this.showMap = true;
    this.openedWindow = event.Code;
    this.pauseShow = true;
    /* if (this.previous) {
      this.previous.close();
    }
    this.previous = window; */


    this.zoom = 9;
    this.facilityProfilesData = {};
    let obj: any = {
      DesignationIds: this.selectedDesignations,
      HFTypeCodes: this.hfTypeCodes,
      HfmisCode: event.Code,
      showProfileViewId: this.showProfileViewId
    };
    this._dashboardService.getAttachedPersons(obj).subscribe((response: any[]) => {
      if (response) {
        //this.facilityProfilesData = response;
        this.anaesthesiaTree = response;
        this.anaesthesiaTree.moWMOAnaesthesiaTree = [];
        this.anaesthesiaTree.moWMO = [];
        this.anaesthesiaTree.MOWMOAnaesthesIsDiploma = 0;
        this.anaesthesiaTree.MOWMOAnaesthesDiplomas = [];
        this.anaesthesiaTree.MOWMOAnaesthesOnTraining = 0;
        this.anaesthesiaTree.MOWMOAnaesthesTrainings = [];
        this.anaesthesiaTree.anaesthesiaTree.forEach(tree => {
          tree.MOTree.forEach(motree => {
            this.anaesthesiaTree.moWMOAnaesthesiaTree.push(motree);
            console.log(motree);
            if (motree && motree.MOWMOAnaesthesia.IsDiploma) {
              this.anaesthesiaTree.MOWMOAnaesthesIsDiploma += 1;
              this.anaesthesiaTree.MOWMOAnaesthesDiplomas.push(motree.MOWMOAnaesthesia);
            }
            if (motree && motree.MOWMOAnaesthesia.OnTraining) {
              this.anaesthesiaTree.MOWMOAnaesthesOnTraining += 1;
              this.anaesthesiaTree.MOWMOAnaesthesTrainings.push(motree.MOWMOAnaesthesia);
            }
            motree.MOWMO.forEach(mowmo => {
              this.anaesthesiaTree.moWMO.push(mowmo);
            });
          });
        });
        console.log(this.anaesthesiaTree);
        //this.set131Team();
      }
    }, err2 => {
    });
  }
  public infoWindowClosed() {
    this.pauseShow = false;

  }


  isInfoWindowOpen(Code) {
    return this.openedWindow == Code; // alternative: check if id is in array
  }
  /* public getMarker(vp: any) {
    return "http://maps.google.com/mapfiles/ms/icons/" + (
      ((vp.TotalWorking / vp.TotalSanctioned) * 100) <= 10 ? "red-dot" :
        ((vp.TotalWorking / vp.TotalSanctioned) * 100) >= 50 ? "green-dot" :
          ((vp.TotalWorking / vp.TotalSanctioned) * 100) > 10 &&
            ((vp.TotalWorking / vp.TotalSanctioned) * 100) < 50 ? "yellow-dot" :
            "yellow-dot") + ".png";
  } */


  public navShow(dir: number) {
    this.pauseShow = true;
    this.showCounter += dir;
    this.dropdownValueChanged(this.districts[this.showCounter], 'district');
  }
  private removeMarkers() {
    for (let i = 0; i < this.gmarkers.length; i++) {
      this.gmarkers[i].setMap(null);
    }
  }
  private getDesignations = () => {
    this._rootService.getAdhocDesignations().subscribe((res: any) => {
      this.designations = res;
      this.designationsData = this.designations.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getDistricts = () => {
    this.districts = [];
    this.districtsData = [];
    this._rootService.getDistrictsLatLong(this.hfmisCode).subscribe((res: any) => {
      this.districts = res;
      this.districtsData = this.districts.slice();
      if (this.districts.length == 1) {
        this.selectedFiltersModel.district = this.districtsData[0];
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getTehsils = () => {
    this.tehsils = [];
    this.tehsilsData = [];
    this._rootService.getTehsils(this.hfmisCode).subscribe((res: any) => {
      this.tehsils = res;
      this.tehsilsData = this.tehsils.slice();
      if (this.tehsils.length == 1) {
        this.selectedFiltersModel.district = this.tehsilsData[0];
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getHFTypes = () => {
    this._rootService.getHFTypes().subscribe((res: any) => {
      this.hfTypes = res;
      this.hfTypes.forEach(element => {
        element.Name = element.Name.replace('District Headquarter Hospital', 'DHQ').replace('Tehsil Headquarter Hospital', 'THQ');
      });
      this.hfTypesData = this.hfTypes.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getHFCodMap = () => {
    this._rootService.getHFCodMap(this.hfmisCode).subscribe((res: any) => {
      this.hfCodesMap = res;
    },
      err => { this.handleError(err); }
    );
  }
  public changeVpChartCount() {
    this.vpChartCount.DesignationId = this.selectedDesignations.length > 0 ? this.selectedDesignations[0] : 0;
    this.vpChartCount.HFMISCode = this.hfmisCode;
    this.vpChartCounts = [
      this.vpChartCount.SanctionedInitial,
      this.vpChartCount.SanctionedPromotion,
      this.vpChartCount.AdhocInitial,
      this.vpChartCount.AdhocPromotion,
      this.vpChartCount.RegularInitial,
      this.vpChartCount.RegularPromotion,
      this.vpChartCount.ContractInitial,
      this.vpChartCount.ContractPromotion,
      this.vpChartCount.VacantInitial,
      this.vpChartCount.VacantPromotion
    ];
    this.saveVPChartCount();
  }
  public getData = () => {
    this.getTechnologists();
    this.loadingMap = true;
    this.loadingVpMaster = true;
    this.profilesData = {};
    this.vacancy = {};
    this.previous = null;
    this.vpMaps = [];
    this.red = 0;
    this.green = 0;
    this.yellow = 0;
    //this.removeMarkers();
    let obj: any = {
      DesignationIds: this.selectedDesignations,
      HFTypeCodes: this.hfTypeCodes,
      HfmisCode: this.hfmisCode,
      showProfileViewId: this.showProfileViewId
    };
    this.getVPChartCount();
    this._dashboardService.getVacancyChartHrmis(obj).subscribe((res: any) => {
      if (res) {
        this.vpMaps = res.vp;
        if (this.vpMapsData.length == 0) {
          this.vpMapsData = this.vpMaps;
        }
        this.vacancy = {
          Name: this.vacancy.Name, Sanctioned: 0, Filled: 0, Vacant: 0, Adhoc: 0, Regular: 0
        };
        this.vacancyChartData = { sanctioned: 0, filled: 0, adhoc: 0, regular: 0, contract: 0, vacant: 0 };
        this.vacancyChart = { sanctioned: [], filled: [], adhoc: [], regular: [], contract: [], vacant: [] };
        this.vacancyChartDesignations = [];
        let tempDesigName = '';
        for (var i = 0; i < this.vpMaps.length; i++) {
          if (tempDesigName != this.vpMaps[i].DsgName) {
            tempDesigName = this.vpMaps[i].DsgName;
            let d = this.vacancyChartDesignations.find(x => x === tempDesigName);
            if (!d) {
              this.vacancyChartDesignations.push(this.vpMaps[i].DsgName);
              let vp = this.vpMaps.filter(x => x.DsgName === tempDesigName);
              this.vacancyChartData = { sanctioned: 0, filled: 0, adhoc: 0, regular: 0, contract: 0, vacant: 0 };
              vp.forEach(v => {
                this.vacancyChartData.sanctioned += v.TotalSanctioned ? v.TotalSanctioned : 0;
                this.vacancyChartData.filled += v.TotalWorking ? v.TotalWorking : 0;
                this.vacancyChartData.adhoc += v.Adhoc ? v.Adhoc : 0;
                this.vacancyChartData.contract += v.Contract ? v.Contract : 0;
                this.vacancyChartData.regular += v.Regular ? v.Regular : 0;
                this.vacancyChartData.vacant += v.Vacant ? v.Vacant : 0;
              });
              this.vacancyChart.sanctioned.push(this.vacancyChartData.sanctioned);
              this.vacancyChart.filled.push(this.vacancyChartData.filled);
              this.vacancyChart.adhoc.push(this.vacancyChartData.adhoc);
              this.vacancyChart.contract.push(this.vacancyChartData.contract);
              this.vacancyChart.regular.push(this.vacancyChartData.regular);
              this.vacancyChart.vacant.push(this.vacancyChartData.vacant);
            }
          }
          this.vacancy.Sanctioned += this.vpMaps[i].TotalSanctioned ? this.vpMaps[i].TotalSanctioned : 0;
          this.vacancy.Filled += this.vpMaps[i].TotalWorking ? this.vpMaps[i].TotalWorking : 0;
          this.vacancy.Vacant += this.vpMaps[i].Vacant ? this.vpMaps[i].Vacant : 0;
          this.vacancy.Adhoc += this.vpMaps[i].Adhoc ? this.vpMaps[i].Adhoc : 0;
          this.vacancy.Regular += this.vpMaps[i].Regular ? this.vpMaps[i].Regular : 0;

          /*  if (this.vpMaps[i].Code.substring(12, 15) == '012') {
             this.vpMaps[i].color = "http://maps.google.com/mapfiles/ms/icons/" + (
               this.vpMaps[i].TotalWorking == 0 ? "red-dot" :
                 this.vpMaps[i].TotalWorking == 1 ? "yellow-dot" : this.vpMaps[i].TotalWorking >= 2 ? "green-dot" :
                   "yellow-dot") + ".png";
           } else if (this.vpMaps[i].Code.substring(12, 15) == '011') {
             this.vpMaps[i].color = "http://maps.google.com/mapfiles/ms/icons/" + (
               this.vpMaps[i].TotalWorking == 0 || this.vpMaps[i].TotalWorking == 1 ? "red-dot" :
                 this.vpMaps[i].TotalWorking == 2 ? "yellow-dot" : this.vpMaps[i].TotalWorking > 2 ? "green-dot" :
                   "yellow-dot") + ".png";
           } else {
             this.vpMaps[i].color = "http://maps.google.com/mapfiles/ms/icons/" + (
               ((this.vpMaps[i].TotalWorking / this.vpMaps[i].TotalSanctioned) * 100) <= 10 ? "red-dot" :
                 ((this.vpMaps[i].TotalWorking / this.vpMaps[i].TotalSanctioned) * 100) >= 50 ? "green-dot" :
                   ((this.vpMaps[i].TotalWorking / this.vpMaps[i].TotalSanctioned) * 100) > 10 &&
                     ((this.vpMaps[i].TotalWorking / this.vpMaps[i].TotalSanctioned) * 100) < 50 ? "yellow-dot" :
                     "yellow-dot") + ".png";
           }
           if (this.vpMaps[i].color == 'http://maps.google.com/mapfiles/ms/icons/red-dot.png') { this.red++; }
           if (this.vpMaps[i].color == 'http://maps.google.com/mapfiles/ms/icons/yellow-dot.png') { this.yellow++; }
           if (this.vpMaps[i].color == 'http://maps.google.com/mapfiles/ms/icons/green-dot.png') { this.green++; }
          */
          if (this.vpMaps[i].Code.substring(12, 15) == '012') {
            this.vpMaps[i].color = "https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|" + (
              this.vpMaps[i].TotalWorking == 0 ? "FF0000" :
                this.vpMaps[i].TotalWorking == 4 ? "008000" :
                  this.vpMaps[i].TotalWorking == 3 ? "7CFC00" :
                    this.vpMaps[i].TotalWorking == 2 ? "FFFF00" :
                      this.vpMaps[i].TotalWorking == 1 ? "FFA500" :
                        "FF0000");
          } else if (this.vpMaps[i].Code.substring(12, 15) == '011') {
            this.vpMaps[i].color = "https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|" + (
              this.vpMaps[i].TotalWorking == 0 ? "FF0000" :
                this.vpMaps[i].TotalWorking == 6 ? "008000" :
                  this.vpMaps[i].TotalWorking == 5 ? "7CFC00" :
                    this.vpMaps[i].TotalWorking == 4 ? "7CFC00" :
                      this.vpMaps[i].TotalWorking == 3 ? "FFFF00" :
                        this.vpMaps[i].TotalWorking == 2 ? "FFA500" :
                          this.vpMaps[i].TotalWorking == 1 ? "FFA500" :
                            "FF0000");
          }
        }
        console.log(this.vacancyChart);
        console.log(this.vacancyChartDesignations);
        //this.addMarker();
        if (this.hfmisCode.length < 6) {
          this.setLayers();
        }
        if (!this.vacancy.Filled) {
          this.vacancy.Filled = 0;
        }
        //this.districts = res.districts;
        /* this.addMarker(); */
        this.loadingMap = false;
        this.loadingVpMaster = false;
      } else {
        this.vacancyChartData = { sanctioned: 0, filled: 0, adhoc: 0, regular: 0, contract: 0, vacant: 0 };
        this.vacancyChart = { sanctioned: [], filled: [], adhoc: [], regular: [], contract: [], vacant: [] };
        this.vacancyChartDesignations = [];
      }
    }, err => {
      this.loadingMap = false;
    });
    /*  this.removeMarkersPersons(); */
    this._dashboardService.getAttachedPersons(obj).subscribe((response: any) => {
      if (response) {
        this.profilesData = response;
        /* this.addMarkerPersons(); */
        this.loadingMap = false;
        this.loadingVpMaster = false;
      }
    }, err2 => {
      this.loadingVpMaster = false;
      this.loadingMap = false;
    });
  }
  public getTechnologists = () => {
    this.vpTechs = [];
    let obj: any = {
      DesignationIds: [2171],
      HFTypeCodes: this.hfTypeCodes,
      HfmisCode: this.hfmisCode,
      showProfileViewId: this.showProfileViewId
    };
    this._dashboardService.getVacancyMapHrmis(obj).subscribe((res: any) => {
      if (res && res.vp) {
        this.vpTechs = res.vp;
        this.vpTechsWorking = this.vpTechs.filter(x => x.TotalWorking > 0);
        this.vacancyTech = {
          Name: this.vacancyTech.Name, Sanctioned: 0, Filled: 0, Vacant: 0, Adhoc: 0, Regular: 0
        };
        for (var i = 0; i < this.vpTechs.length; i++) {
          this.vacancyTech.Sanctioned += this.vpTechs[i].TotalSanctioned ? this.vpTechs[i].TotalSanctioned : 0;
          this.vacancyTech.Filled += this.vpTechs[i].TotalWorking ? this.vpTechs[i].TotalWorking : 0;
          this.vacancyTech.Vacant += this.vpTechs[i].Vacant ? this.vpTechs[i].Vacant : 0;
          this.vacancyTech.Adhoc += this.vpTechs[i].Adhoc ? this.vpTechs[i].Adhoc : 0;
          this.vacancyTech.Regular += this.vpTechs[i].Regular ? this.vpTechs[i].Regular : 0;
        }
        this.setVpTechnologist();
      }
    }, err => {
      this.loadingMap = false;
    });
  }
  public setVpTechnologist() {
    this.vpMaps.forEach(vp => vp.vpTech = this.vpTechs.find(x => x.Code == vp.Code));
  }
  public saveVPChartCount() {
    this.editing = true;
    this._dashboardService.saveVPChartCount(this.vpChartCount).subscribe((response: any) => {
      if (response) {
        this.vpChartCount = response;
        this.editFlag = false;
      }
      this.editing = false;
    }, err => {
      console.log(err);
      this.editing = false;
    });
  }
  public getVPChartCount() {
    let obj: any = {
      DesignationId: this.selectedDesignations.length > 0 ? this.selectedDesignations[0] : 0,
      HfmisCode: this.hfmisCode,
    };
    this._dashboardService.getVPChartCount(obj).subscribe((response: any) => {
      if (response) {
        this.vpChartCount = response;
        this.vpChartCounts = [
          this.vpChartCount.SanctionedInitial,
          this.vpChartCount.SanctionedPromotion,
          this.vpChartCount.AdhocInitial,
          this.vpChartCount.AdhocPromotion,
          this.vpChartCount.RegularInitial,
          this.vpChartCount.RegularPromotion,
          this.vpChartCount.ContractInitial,
          this.vpChartCount.ContractPromotion,
          this.vpChartCount.VacantInitial,
          this.vpChartCount.VacantPromotion
        ];
      } else {
        this.vpChartCount = {};
        this.vpChartCounts = [
          0,
          0,
          0,
          0,
          0,
          0,
          0,
          0,
          0,
          0
        ];
      }
    }, err => {
      console.log(err);

    });
  }
  setLayers() {
    this.districtred = 0;
    this.districtyellow = 0;
    this.districtgreen = 0;
    this._dashboardService.getGeoJson().subscribe((jsonRes: any) => {
      if (jsonRes) {
        jsonRes = JSON.parse(jsonRes);
        jsonRes.features.forEach(element => {
          let dis = this.districts.find(x => x.Name == element.properties.districts);
          if (dis) {
            dis.sanc = 0;
            dis.fill = 0;
            dis.avg = 0;
            dis.score = 0;

            dis.hfs = this.vpMapsData.filter(x => x.Code.substring(0, 6) == dis.Code);
            if (dis.hfs) {
              dis.hfs.forEach(vp => {
                vp.pers = (vp.TotalWorking / vp.TotalSanctioned) * 100;
                if (vp.Code.substring(12, 15) == '012') {
                  vp.avg = vp.pers <= 10 ? 0 : vp.pers >= 51 ? 10 : vp.pers > 10 && vp.pers < 51 ? 5 : 0;
                } else {
                  vp.avg = vp.pers <= 10 ? 0 : vp.pers >= 50 ? 10 : vp.pers > 10 && vp.pers < 50 ? 5 : 0;
                }
                vp.badgeColor = ((vp.TotalWorking / vp.TotalSanctioned) * 100) <= 10 ? "badge-danger" :
                  ((vp.TotalWorking / vp.TotalSanctioned) * 100) >= 50 ? "badge-success" :
                    ((vp.TotalWorking / vp.TotalSanctioned) * 100) > 10 &&
                      ((vp.TotalWorking / vp.TotalSanctioned) * 100) < 50 ? "badge-warning" :
                      "";
                if (vp.Code.substring(12, 15) == '011') {
                  vp.badgeName = this.getDistrictName(vp.Code);

                } else if (vp.Code.substring(12, 15) == '012') {
                  vp.badgeName = this.getTehsilName(vp.Code);
                }
                dis.avg += vp.avg;
                dis.sanc += vp.TotalSanctioned;
                dis.fill += vp.TotalWorking;
              });
            }

            dis.score = (dis.avg / dis.hfs.length);
            dis.color = (
              (dis.score <= 1 ? "red" : dis.score >= 7.5 ? "green" :
                dis.score > 1 && dis.score < 7.5 ? "yellow" :
                  "yellow"));
            if (dis.color == 'red') { this.districtred++; }
            if (dis.color == 'yellow') { this.districtyellow++; }
            if (dis.color == 'green') { this.districtgreen++; }
            element.properties.color = (
              (dis.score <= 1 ? "red" : dis.score >= 7.5 ? "green" :
                dis.score > 1 && dis.score < 7.5 ? "yellow" :
                  "yellow"));
            dis.per = (dis.fill / dis.sanc) * 100;
            /* element.properties.color = (
              (dis.per <= 10 ? "red" : dis.per >= 50 ? "green" :
                dis.per > 10 && dis.per < 50 ? "yellow" :
                  "yellow")); */
            this.districtsRouting.push(dis);
            element.properties.fillOpacity = 0.2;
            element.properties.weight = 1;
            element.properties.strokeColor = 'green';
            element.properties.strokeWeight = 0.4;
          }
        });
        this.lat = 31.1704;
        this.lng = 72.7097;
        this.zoom = 7;
      }
      this.geoJsonObject = jsonRes;
      this.districtsRouting.sort(this.compare);
      this.show = true;
      this.multiDropFilter();

    }, err => {
      console.log(err);
    });
  }
  private compare(a, b) {
    if (a.per > b.per) { return -1; }
    if (a.per < b.per) { return 1; }
    return 0;
  }
  public showProfiles(marker, viewId: number) {
    marker.showProfileViewId = viewId;
    marker.loadingP = true;
    this.pauseShow = true;
    if (marker.showProfileViewId != 0) {
      let obj: any = {
        DesignationIds: this.selectedDesignations,
        HFTypeCodes: this.hfTypeCodes,
        HfmisCode: marker.Code,
        showProfileViewId: marker.showProfileViewId
      };
      this._dashboardService.getAnesthesiaPersons(obj).subscribe((response: any) => {
        if (response) {
          if (marker.showProfileViewId == 1) {
            this.facilityProfilesData.MO = response.MO;
          }
          if (marker.showProfileViewId == 11) {
            this.facilityProfilesData.TrainingMO = response.TrainingMO;
          }
          if (marker.showProfileViewId == 12) {
            this.facilityProfilesData.DiplomaMO = response.DiplomaMO;
          }
          if (marker.showProfileViewId == 2) {
            this.facilityProfilesData.WMO = response.WMO;
          }
          if (marker.showProfileViewId == 21) {
            this.facilityProfilesData.TrainingWMO = response.TrainingWMO;
          }
          if (marker.showProfileViewId == 22) {
            this.facilityProfilesData.DiplomaWMO = response.DiplomaWMO;
          }
        }
        marker.loadingP = false;
      });
    }
  }
  public beginShow() {
    if (!this.intervalSet) {
      setInterval(() => {
        if (this.showCounter >= 0) {
          if (!this.pauseShow)
            this.dropdownValueChanged(this.districtsRouting[this.showCounter--], 'district');
          if (this.showCounter == -1) {
            this.showCounter = 35;
          }
        }
      }, 7000);
      this.intervalSet = true;
    }
  }
  getDistrictName(codeC: string) {
    let d = this.districts.find(x => codeC.startsWith(x.Code));
    return d ? 'DHQ, ' + d.Name : '';
  }
  getTehsilName(codeC: string) {
    let t = this.tehsils.find(x => codeC.startsWith(x.Code));
    return t ? 'THQ, ' + t.Name : '';
  }
  styleFunc(feature) {
    return ({
      clickable: true,
      weight: feature.getProperty('weight'),
      fillColor: feature.getProperty('color'),
      fillOpacity: feature.getProperty('fillOpacity'),
      strokeColor: feature.getProperty('strokeColor'),
      strokeWeight: feature.getProperty('strokeWeight')
    });
  }
  /* private getDistrictsVacancy = () => {
    let obj: any = {
      DesignationIds: this.selectedDesignations,
      HFTypeCodes: this.hfTypeCodes,
      HfmisCode: this.hfmisCode
    };
    this._dashboardService.getVacancyMapHrmis(obj).subscribe(
      res3 => {
        if (res3) {
          this.districts = res3 as any[];
        }
      }, err => {
      }
    );
  } */
  public openWindow(viewId: number) {
    this.showProfileViewId = viewId;
    this.pauseShow = true;
    if (this.showProfileViewId != 0) {
      let obj: any = {
        DesignationIds: this.selectedDesignations,
        HFTypeCodes: this.hfTypeCodes,
        HfmisCode: this.hfmisCode,
        showProfileViewId: this.showProfileViewId
      };
      this._dashboardService.getAnesthesiaPersons(obj).subscribe((response: any) => {
        if (response) {
          if (this.showProfileViewId == 1) {
            this.profilesData.MO = response.MO;
          }
          if (this.showProfileViewId == 11) {
            this.profilesData.TrainingMO = response.TrainingMO;
          }
          if (this.showProfileViewId == 12) {
            this.profilesData.DiplomaMO = response.DiplomaMO;
          }
          if (this.showProfileViewId == 2) {
            this.profilesData.WMO = response.WMO;
          }
          if (this.showProfileViewId == 21) {
            this.profilesData.TrainingWMO = response.TrainingWMO;
          }
          if (this.showProfileViewId == 22) {
            this.profilesData.DiplomaWMO = response.DiplomaWMO;
          }
        }
      });
    }
    this.profilesWindow.dialogOpened = true;
    this.loadingVpMaster = false;
  }
  public getSurgeries() {

    /* this._dashboardService.getSurgeries(this.dhisCode, this.dhisFiltetType).subscribe((response: any) => {
      if (response) {
        if (response != 'null') {
          this.surgeries = JSON.parse(response);
          console.log(this.surgeries);
        } else {
          this.surgeries = [{ga: 0, sa: 0, la: 0, other: 0}];
          console.log(this.surgeries);
        }
      }
    }); */
  }
  public closeWindow() {
    this.showProfileViewId = 0;
    this.showProfile = false;
    this.profilesWindow.dialogOpened = false;
    this.profilesWindow.data = null;
    this.pauseShow = false;
    /*  this.employementModes.forEach(empMode => {
       empMode.totalWorking = undefined;
       empMode.profiles = undefined;
     });
     this.vpMaster = new VpMProfileView(); */
  }
  public showProfileClicked(empModeId: number) {
    this.profiles = [];
    this.showProfileEmpMode_Id = empModeId;
    if (this.showProfileEmpMode_Id == 0) {
      this.profiles = this.profilesWindow.profiles.slice();
    } else {
      this.profiles = this.profilesWindow.profiles.filter(x => x.EmpMode_Id == this.showProfileEmpMode_Id).slice();
    }
    this.showProfile = !this.showProfile;
  }
  public getTeam(profileId: number) {
    if (this.profilesData && this.profilesData.attachedPersons) {
      return this.profilesData.attachedPersons.filter(x => x.ProfileId == profileId);
    }
  }
  public getTeamCount(HfmisCode: string) {
    if (this.profilesData && this.profilesData.attachedPersons) {
      let team = this.profilesData.attachedPersons.filter(x => x.HfmisCode == HfmisCode) as any[];
      return team ? team.length : 0;
    }
  }
  public set131Team() {
    if (this.profilesData && this.profilesData.attachedPersons) {
      for (let index = 0; index < this.facilityProfilesData.ConsultantAnaesthetists.length; index++) {
        const element = this.facilityProfilesData.ConsultantAnaesthetists[index];
        let skip = index * 3;
        let team = this.profilesData.attachedPersons.filter(x => x.HfmisCode == element.HfmisCode) as any[];
        element.Team131 = team.slice(skip, (skip + 3));

        skip = index * 1;
        team = this.profilesData.Technologists.filter(x => x.HfmisCode == element.HfmisCode) as any[];
        element.TeamTechs = team.slice(skip, (skip + 1));
      }

    }
  }

  private loadDropdownValues = () => {
    this.getHFTypes();
    this.getDesignations();
    this.getDistricts();
    this.getTehsils();
    /* this.getDistrictsVacancy(); */
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    //this.showMap = true;
    console.log(value);
    if (value == 'distP' && document.getElementById("distP") && document.getElementById("distP").innerText) {
      this.hfmisCode = document.getElementById("distP").innerText;
      this.selectedFiltersModel.district = this.districts.find(x => x.Code == this.hfmisCode);
    }
    else {
      this.surgeries = [];
      this.dhisCode = '1';
      this.hfmisCode = value.Code;
      this.vacancy.Name = value.Name;
      let codeMap = this.hfCodesMap.filter(x => x.HrHfCode.startsWith(this.hfmisCode));
      console.log(this.hfmisCode);
      console.log(codeMap);
      if (codeMap && codeMap.length > 0) {
        for (let index = 0; index < codeMap.length; index++) {
          const element = codeMap[index];
          if (element && element.DHISHfCode) {
            this.dhisCode = element.DHISHfCode.substring(0, 3);
            console.log(this.dhisCode);
            this.dhisFiltetType = 'district';
            this.getSurgeries();
            break;
          }
        }
      }

      this.selectedFiltersModel.district = this.districts.find(x => x.Code == this.hfmisCode);
      this.lat = value.Latitude;
      this.lng = value.Longitude;
      this.zoom = 9;
      /* this.map.setOptions({
        center: new google.maps.LatLng(value.Latitude, value.Longitude),
        zoom: 9,
        disableDefaultUI: true,
        styles: [
        {
            "featureType": "administrative.land_parcel",
            "stylers": [
              {
                "visibility": "off"
              }
            ]
          },
          {
            "featureType": "administrative.neighborhood",
            "stylers": [
              {
                "visibility": "off"
              }
            ]
          },
          {
            "featureType": "road",
            "stylers": [
              {
                "visibility": "off"
              }
            ]
          }
        ]
      }) */
      if (filter == 'district') {
        this.selectedFiltersModel.tehsil.Code = this.hfmisCode;
      }
    }
    this.getData();
  }
  public handleFilter = (value, filter) => {
    if (filter == 'hftype') {
      this.hfTypesData = this.hfTypes.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'designation') {
      this.designationsData = this.designations.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
  }
  private multiDropFilter = () => {
    if (this.hfTypeList) {
      this.hfTypeList.filterChange.asObservable().pipe(
        switchMap(value => from([this.hfTypes]).pipe(
          delay(300),
          map((data) => data.filter(contains(value)))
        ))
      ).subscribe(x => {
        this.hfTypesData = x;
      });
      this.designationsList.filterChange.asObservable().pipe(
        switchMap(value => from([this.designations]).pipe(
          delay(300),
          map((data) => data.filter(contains(value)))
        ))
      ).subscribe(x => {
        this.designationsData = x;
      });
      const contains = value => (s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1;
    }

  }
  public openInNewTab(link: string) {
    window.open('/' + link, '_blank');
  }
  private handleError(err: any) {
    console.log(err);

  } public dashifyCNIC(cnic: string) {
    if (!cnic) return '';
    return cnic[0] +
      cnic[1] +
      cnic[2] +
      cnic[3] +
      cnic[4] +
      '-' +
      cnic[5] +
      cnic[6] +
      cnic[7] +
      cnic[8] +
      cnic[9] +
      cnic[10] +
      cnic[11] +
      '-' +
      cnic[12];
  }
}


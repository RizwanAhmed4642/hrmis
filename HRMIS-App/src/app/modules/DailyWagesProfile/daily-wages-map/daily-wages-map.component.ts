import { Component, AfterViewInit, ViewChild, ElementRef, OnInit, ɵCodegenComponentFactoryResolver } from '@angular/core';
import { switchMap } from 'rxjs/operators/switchMap';
import { from } from 'rxjs/observable/from';
import { delay } from 'rxjs/operators/delay';
import { map } from 'rxjs/operators/map';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { DashboardService } from '../../dashboard/dashboard.service';
import {CoordinatesViewModel} from '../../../_models/_db/daily-wages.model'
import { Router, ActivatedRoute } from '@angular/router';
import { DailyWagerService } from '../../../_services/daily-wager.service';
@Component({
  selector: 'app-daily-wages-map',
  templateUrl: './daily-wages-map.component.html',
  styleUrls: ['./daily-wages-map.component.scss']
})
export class DailyWagesMapComponent implements OnInit {

  constructor(private _rootService: RootService, private _dashboardService: DashboardService,private route:ActivatedRoute,private router:Router, private _authenticationService: AuthenticationService) { }

  @ViewChild('mapContainer', { static: false }) gmap: ElementRef;
  @ViewChild('hfTypeList', { static: false }) hfTypeList;
  @ViewChild("designationsList", { static: false }) designationsList;
  public map: google.maps.Map;
  public coordinatesViewModelObj : CoordinatesViewModel[];
  public skip = 0;
  public searchTerm: string = '';

  public showCounter = 35;
  public zoom = 7;
  public red = 0;
  public yellow = 0;
  public pageSize = 50;
  public designationStr : "";

  public green = 0;
  public districtred = 0;
  public districtyellow = 0;
  public districtgreen = 0;
  public lat = 31.1704;
  public lng = 72.7097;
  public polygonArray: Array<{ lat: any, lng: any }>;
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
  public surgeries: any[] = [];
  public InforObj: any[] = [];
  public polyLinesObj: any[] = [];
  public bounds = new google.maps.LatLngBounds();
  public marker1;
  //dropdowns : need two same arrays for search/filter purpose
  public divisions: any[] = [];
  public divisionsData: any[] = [];
  public districtsRouting: any[] = [];
  public districts: any[] = [];
  public districtsData: any[] = [];
  public tehsils: any[] = [];
  public personProfiles: any[] = [];
  public tehsilsData: any[] = [];
  public coordinatesData: any[] = [];

  public hfTypes: any[] = [];
  public hfTypesData: any[] = [];
  public designations: any[] = [];
  public designationsData: any[] = [];
  public selectedDesignations: any[] = [21];
  public hfTypeCodes: any[] = ['011'];
  public tehsilCode : any[] = [];
  public gmarkers: any[] = [];
  public profiles: any[] = [];
  public pmarkers: any[] = [];
  public showProfile = false;
  public loadingVpMaster = false;
  public viewAttachedPersons = false;
  public intervalSet = false;
  public show = true;
  public showProfileEmpMode_Id: number = 0;
  public showProfileViewId: number = 0;
  public selectedCadres: any[] = [];

  //selected object for drop downs
  public selectedFiltersModel: any = {
    division: { Name: 'Select Division', Code: '0' },
    district: { Name: 'Select District', Code: '0' },
    tehsil: { Name: 'Select Tehsil', Code: '0' }
  };
  public DesignationList: string[]= [
    'Dengue',
    'Polio',
    'Madadgaar',
    'PMIS',
    'Other'
  ]
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
  public Category = '';
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
  public retirementInOneYear: boolean = false;
  public retirementAlerted: boolean = false;
  public currentUser;
  public loadingMap: boolean = false;
  public pauseShow: boolean = false;
  public showMap: boolean = true;
  public selectedStatuses: any[] = [];
  public selectedEmployementModes: any[] = [];
  public src = 'https://developers.google.com/maps/documentation/javascript/examples/kml/westcampus.kml';
  public geoJsonObject: any;
  public openedWindow: string = '0'; // alternative: array of numbers
 
  polygons = [
    {
      data: [
        {
          polygons: [
            {
                
                "lat": 30.17040212,
                "lng": 73.24498599
            },
            {
                
                "lat": 30.17007299,
                "lng": 73.25623484
            },
            {
               
                "lat": 30.16985261,
                "lng": 73.26374842
            },
            {
               
                "lat": 30.16984515,
                "lng": 73.26400273
            },
            {
               
                "lat": 30.1611953,
                "lng": 73.26345588
            },
            {
                
                "lat": 30.15792372,
                "lng": 73.26334534
            },
            {
                
                "lat": 30.15765905,
                "lng": 73.2632894
            },
            {
                
                "lat": 30.15732203,
                "lng": 73.26321103
            },
            {
                
                "lat": 30.15600032,
                "lng": 73.26312748
            },
            {
                
                "lat": 30.15596666,
                "lng": 73.26309862
            },
            {
                
                "lat": 30.15596647,
                "lng": 73.26309846
            },
            {
                
                "lat": 30.15582002,
                "lng": 73.26297871
            },
            {
                
                "lat": 30.15581963,
                "lng": 73.26297839
            },
            {
                
                "lat": 30.15581954,
                "lng": 73.26297832
            },
            {
                
                "lat": 30.15566813,
                "lng": 73.26286385
            },
            {
                
                "lat": 30.15551244,
                "lng": 73.2627552
            },
            {
               
                "lat": 30.15551238,
                "lng": 73.26275515
            },
            {
               
                "lat": 30.15551225,
                "lng": 73.26275507
            },
            {
                
                "lat": 30.15535273,
                "lng": 73.26265255
            },
          ]
        },
        // {
        //   polygons: [
        //     {
        //       lat: 63.361730895576265,
        //       lng: -147.567801077303
        //     },
        //     {
        //       lat: 63.6606373596412,
        //       lng: -145.985769827303
        //     },
        //     {
        //       lat: 63.97089403964998,
        //       lng: -147.326101858553
        //     },
        //     {
        //       lat: 63.64601011757311,
        //       lng: -147.985281546053
        //     },
        //     {
        //       lat: 63.41586351549344,
        //       lng: -148.172049124178
        //     }
        //   ]
        // },
        // {
        //   polygons: [
        //     {
        //       lat: 63.84526422875888,
        //       lng: -153.060965139803
        //     },
        //     {
        //       lat: 64.03830815102336,
        //       lng: -153.588308889803
        //     },
        //     {
        //       lat: 64.50094419573703,
        //       lng: -153.500418264803
        //     },
        //     {
        //       lat: 64.64246352506252,
        //       lng: -152.050222952303
        //     },
        //     {
        //       lat: 64.20135214728498,
        //       lng: -151.314138967928
        //     },
        //     {
        //       lat: 64.14870724365123,
        //       lng: -152.160086233553
        //     },
        //     {
        //       lat: 64.33966488515952,
        //       lng: -152.544607717928
        //     }
        //   ]
        // },
        // {
        //   polygons: [
        //     {
        //       lat: 65.78452769554828,
        //       lng: -146.622976858553
        //     },
        //     {
        //       lat: 65.91487816470621,
        //       lng: -145.172781546053
        //     },
        //     {
        //       lat: 66.12473526488733,
        //       lng: -145.183767874178
        //     },
        //     {
        //       lat: 66.29756399411468,
        //       lng: -146.117605764803
        //     },
        //     {
        //       lat: 66.32845950832974,
        //       lng: -147.128347952303
        //     },
        //     {
        //       lat: 65.91487816470621,
        //       lng: -147.688650686678
        //     },
        //     {
        //       lat: 65.9820416527134,
        //       lng: -146.853689749178
        //     },
        //     {
        //       lat: 65.64445459904373,
        //       lng: -147.260183889803
        //     }
        //   ]
        // },
        // {
        //   polygons: [
        //     {
        //       lat: 62.88995584557632,
        //       lng: -157.257742483553
        //     },
        //     {
        //       lat: 63.1094153396653,
        //       lng: -157.455496389803
        //     },
        //     {
        //       lat: 63.435522805120655,
        //       lng: -157.191824514803
        //     },
        //     {
        //       lat: 63.346949730527555,
        //       lng: -156.422781546053
        //     },
        //     {
        //       lat: 63.05968297248453,
        //       lng: -156.290945608553
        //     },
        //     {
        //       lat: 63.11935161243623,
        //       lng: -156.862234671053
        //     }
        //   ]
        // },
        // {
        //   polygons: [
        //     {
        //       lat: 65.30241568666297,
        //       lng: -151.522879202303
        //     },
        //     {
        //       lat: 65.48539327007941,
        //       lng: -149.984793264803
        //     },
        //     {
        //       lat: 65.73491119176467,
        //       lng: -150.171560842928
        //     },
        //     {
        //       lat: 65.91039432417611,
        //       lng: -150.819754202303
        //     },
        //     {
        //       lat: 65.5400380950869,
        //       lng: -152.116140921053
        //     },
        //     {
        //       lat: 65.37118184344384,
        //       lng: -152.138113577303
        //     }
        //   ]
        // }
      ]
    }
  ];
  



  ngOnInit(): void {
    this.currentUser = this._authenticationService.getUser();
    this.hfmisCode = this._authenticationService.getUserHfmisCode();
    debugger
    this.loadDropdownValues();
    this.GetDailyWagesMapCount();
    this.getCoordinates(null , null);
    // this.ShowMarker();
    // this.getSurgeries();
    // this.getHFCodMap();
    // this.getTechnologists();
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
    debugger
    console.log(clickEvent);

    let dis = this.districts.find(x => x.Name == clickEvent.feature.i.districts);
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
    debugger
    console.log(this.hfmisCode) 
    //console.log(this.hfmisCode) 
    console.log(event.Category);
    this.router.navigate(['wagercatlist'], { queryParams: { Category: event.Category, Type: this.hfmisCode } });
    
    // debugger
    // console.log(event);

    // this.setVpTechnologist();
    // let codeMap = this.hfCodesMap.filter(x => x.HrHfId == event.HF_Id);
    // this.surgeries = [];
    // if (codeMap[0]) {
    //   this.dhisCode = codeMap[0].DHISHfCode;
    //   this.dhisFiltetType = 'facility';
    //   this.getSurgeries();
    // }
 
    // this.showMap = true;
    // this.openedWindow = event.Code;
    // this.pauseShow = true;
    // /* if (this.previous) {
    //   this.previous.close();
    // }
    // this.previous = window; */


    // this.zoom = 9;
    // this.facilityProfilesData = {};
    // let obj: any = {
    //   DesignationIds: this.selectedDesignations,
    //   HFTypeCodes: this.hfTypeCodes,

    //   HfmisCode: event.Code,
    //   showProfileViewId: this.showProfileViewId
    // };
    //  this._dashboardService.getEmployeePersons(obj).subscribe((response: any) => {
    //   if (response) {
    //     this.personProfiles = response.Persons;
    //     this.loadingMap = false;
    //     this.loadingVpMaster = false;
    //   }
    // }, err2 => {
    //   this.loadingVpMaster = false;
    //   this.loadingMap = false;
    // });
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
    this._rootService.getDesignations().subscribe((res: any) => {
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
        this.selectedFiltersModel.tehsil = this.tehsilsData[0];
      }
    },
      err => { this.handleError(err); }
    );
  }


  
  private getCoordinates = (name : string , Type:string) => {
   this.coordinatesData = [];
    this._rootService.getCoordinates(name,Type).subscribe((res: any) => {
      this.show = false;
      this.polygons[0].data[0].polygons = [];
      this.coordinatesViewModelObj = res.List;
      let arrayTest = res.List.filter(element => {
        delete element.$id;
        return element;
      });
      this.polygons[0].data[0].polygons = arrayTest
      this.drawPolygon(this.coordinatesViewModelObj);
      this.show = true;

    },
      err => { this.handleError(err); }
    );
  }

  private GetDailyWagesMapCount = () => {
    this.coordinatesData = [];
     this._rootService.GetDailyWagesMapCount(this.skip, this.pageSize, this.hfmisCode, this.searchTerm, this.selectedCadres, this.selectedDesignations, this.selectedStatuses, this.retirementInOneYear, this.retirementAlerted, this.designationStr)
      .subscribe((res: any) => {
          debugger
          console.log(res.List.List);
          this.vpMaps = res.List.List;
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
  public getData = () => {
    if (this.selectedDesignations.length == 0) return;
    //this.getTechnologists();
    this.loadingMap = true;
    this.loadingVpMaster = true;
    this.profilesData = {};
    this.personProfiles = [];
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
    this._dashboardService.getVacancyMapHrmis(obj).subscribe((res: any) => {
      if (res) {
        console.log(res);
        this.vpMapsData = [];
        this.vpMaps = res.vp;
        if (this.vpMapsData.length == 0) {
          this.vpMapsData = this.vpMaps;
        }
        this.vacancy = {
          Name: this.vacancy.Name, Sanctioned: 0, Filled: 0, Vacant: 0, Adhoc: 0, Regular: 0
        };
        for (var i = 0; i < this.vpMaps.length; i++) {
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
          /*   if (this.vpMaps[i].Code.substring(12, 15) == '012') {
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
            } */
          this.vpMaps[i].color = "https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|" + (
            (this.vpMaps[i].TotalWorking / this.vpMaps[i].TotalSanctioned) * 100 < 33 ? "FF0000" :
              (this.vpMaps[i].TotalWorking / this.vpMaps[i].TotalSanctioned) * 100 > 33 && (this.vpMaps[i].TotalWorking / this.vpMaps[i].TotalSanctioned) * 100 <= 66 ? "FFFF00" :
                (this.vpMaps[i].TotalWorking / this.vpMaps[i].TotalSanctioned) * 100 > 66 ? "008000" :
                  "FF0000");
        }
        //this.addMarker();
        if (this.hfmisCode.length < 6) {
          this.setLayers();
        }

      
        if (!this.vacancy.Filled) {
          this.vacancy.Filled = 0;
        }
        //this.districts = res.districts;
        /* this.addMarker(); */
        this.loadingVpMaster = false;
      }
    }, err => {
      this.loadingMap = false;
    });
    /*  this.removeMarkersPersons(); */
    
  }
  ShowMarker(){
    debugger
    this.vpMapsData = [];
    // this.vpMaps = res.vp;
    if (this.vpMapsData.length == 0) {
      this.vpMapsData = this.vpMaps;
    }
    this.vacancy = {
      Name: this.vacancy.Name, Sanctioned: 0, Filled: 0, Vacant: 0, Adhoc: 0, Regular: 0
    };
    for (var i = 0; i < this.vpMaps.length; i++) {
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
      /*   if (this.vpMaps[i].Code.substring(12, 15) == '012') {
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
        } */
      this.vpMaps[i].color = "https://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|" + (
        (this.vpMaps[i].TotalWorking / this.vpMaps[i].TotalSanctioned) * 100 < 33 ? "FF0000" :
          (this.vpMaps[i].TotalWorking / this.vpMaps[i].TotalSanctioned) * 100 > 33 && (this.vpMaps[i].TotalWorking / this.vpMaps[i].TotalSanctioned) * 100 <= 66 ? "FFFF00" :
            (this.vpMaps[i].TotalWorking / this.vpMaps[i].TotalSanctioned) * 100 > 66 ? "008000" :
              "FF0000");
    }
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
  setLayers() {
    debugger
    console.log('setLayers');
    this.geoJsonObject = {};
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
            /*  dis.color = (
               (dis.score <= 3.3 ? "red" : dis.score >= 3.4 ? "green" :
                 dis.score > 6.6 && dis.score < 1 ? "yellow" :
                   "yellow")); */
            if (dis.color == 'red') { this.districtred++; }
            if (dis.color == 'blue') { this.districtyellow++; }
            if (dis.color == 'green') { this.districtgreen++; }
            /*   element.properties.color = (
                (dis.score <= 3.3 ? "red" : dis.score >= 3.4 ? "green" :
                  dis.score > 6.6 && dis.score < 1 ? "yellow" :
                    "yellow")); */
            dis.per = (dis.fill / dis.sanc) * 100;

            element.properties.color = (
              (dis.per <= 33 ? "red" : dis.per >= 34 && dis.per <= 66 ? "blue" : "green"));
            this.districtsRouting.push(dis);
            element.properties.fillOpacity = 0.2;
            element.properties.weight = 1;
            element.properties.strokeColor = 'black';
            element.properties.strokeWeight = 0.5;
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
      this.loadingMap = false;

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

    this._dashboardService.getSurgeries(this.dhisCode, this.dhisFiltetType).subscribe((response: any) => {
      if (response) {
        if (response != 'null') {
          this.surgeries = JSON.parse(response);
          console.log(this.surgeries);
        } else {
          this.surgeries = [{ ga: 0, sa: 0, la: 0, other: 0 }];
          console.log(this.surgeries);
        }
      }
    });
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
    // this.getHFTypes();
    this.getDesignations();
    this.getDistricts();
    this.getTehsils();
    /* this.getDistrictsVacancy(); */
  }
  public dropdownValueChanged = (value, filter) => {
    debugger
    if (!value) {
      return;
    }
    // this.showMap = true;
    // console.log(value);

    
    //   this.surgeries = [];
    //   this.dhisCode = '1';
    //   this.hfmisCode = value.Code;
    //   this.vacancy.Name = value.Name;
    //   let codeMap = this.coordinatesData.filter( x => { 
    //     return x.name == value.Name
    //   });
    //   console.log(this.hfmisCode);
    //   console.log(codeMap);
    //   if (codeMap && codeMap.length > 0) {
    //     for (let index = 0; index < codeMap.length; index++) {
    //       const element = codeMap[index];
    //       if (element && element.DHISHfCode) {
    //         this.dhisCode = element.DHISHfCode.substring(0, 3);
    //         console.log(this.dhisCode);
    //         this.dhisFiltetType = 'district';
    //         this.getSurgeries();
    //         break;
    //       }
    //   }

    //   this.selectedFiltersModel.district = this.districts.find(x => x.Code == this.hfmisCode);
    //   this.lat = value.Latitude;
    //   this.lng = value.Longitude;
    //   this.zoom = 9;
    //   /* this.map.setOptions({
    //     center: new google.maps.LatLng(value.Latitude, value.Longitude),
    //     zoom: 9,
    //     disableDefaultUI: true,
    //     styles: [
    //     {
    //         "featureType": "administrative.land_parcel",
    //         "stylers": [
    //           {
    //             "visibility": "off"
    //           }
    //         ]
    //       },
    //       {
    //         "featureType": "administrative.neighborhood",
    //         "stylers": [
    //           {
    //             "visibility": "off"
    //           }
    //         ]
    //       },
    //       {
    //         "featureType": "road",
    //         "stylers": [
    //           {
    //             "visibility": "off"
    //           }
    //         ]
    //       }
    //     ]
    //   }) */
     
    // }

    
    
      if (filter == 'district') {
        this.hfmisCode = value.Code;
        this.getTehsils();
      }
      this.hfmisCode = value.Code;
      this.getCoordinates(value.Name , filter);
      this.GetDailyWagesMapCount();
      this.ShowMarker();
    
    // this.getData();
  }
  public handleFilter = (value, filter) => {
    debugger
    if (filter == 'tehsil') {
      this.tehsilsData = this.tehsilsData.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
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

  onChangedDesignation(event:any){
    debugger
    this.GetDailyWagesMapCount();
  }

  //========================= Daily Wages Working Here ====================================//

  drawPolygon(list:CoordinatesViewModel[]) {
    // this.map = new google.maps.Map(
    //   {
    //     center: this.coordinates,
    //     zoom: 7,
    // }
    // );
    // create empty array
    // we will fill in all the polygons created in this array
    this.map = new google.maps.Map(this.gmap.nativeElement, this.mapOptions);
    let polygonArray = [];
if(list != null){

  this.polygons.filter((data: any) => {
    data.data.filter((res: any) => {
        debugger
        // create new polygon
          let polygonsMap = new google.maps.Polygon({
            map: this.map,
            paths: res.polygons,
            strokeColor: 'blue',
            fillColor: 'blue',
            strokeOpacity: 0.8,
            strokeWeight: 2
          });
          polygonsMap.setMap(this.map);   
          this.vpMaps.forEach(element => {
            for (let index = 0; index < this.vpMaps.length; index++) {
              let marker = new google.maps.Marker({
                position: res.polygons[index],
                title: this.designationStr
              })
              marker.setMap(this.map);
            }
       

});
         
        // add event listener
        google.maps.event.addListener(polygonsMap, 'click', function(event) {
          //all polygons are reset before painting the clicked polygon
          // for (let i in polygonArray) {
          //   polygonArray[i].setOptions({ fillColor: 'blue' });
          //   polygonArray[i].setOptions({ strokeColor: 'blue' });
          // }

          // clicked polygon paint
          polygonsMap.setOptions({ fillColor: '#000' });
          polygonsMap.setOptions({ strokeColor: '#000' });
        });

        // push to polygonArray
        polygonArray.push(polygonsMap);
      });
    });
  }
    // Show polyogon array
    console.log('polygonArray', polygonArray);
  }
  
}

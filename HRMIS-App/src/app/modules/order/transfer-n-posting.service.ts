
import { EventEmitter, Injectable } from '@angular/core';
import { ESR, ESRDetail, EmpProfile, LeaveOrder, ESRView } from './TransferAndPosting/ESR.class';
import { LeaveOrderView } from './order';


@Injectable()
export class TnPService {
  private _employeeName: string;
  private _healthFacilityFromName: string;
  private _healthFacilityToName: string;
  private _designationFromName: string;
  private _designationToName: string;
  public districtFrom: string;
  public tehsilFrom: string;
  private _districtTo: string;
  private _tehsilTo: string;
  public gender: number = 0;
  private _esr: ESR = new ESR();
  private _esrView: ESRView = new ESRView();
  private _esrDetail: ESRDetail = new ESRDetail();

  private _employeeName2: string;
  private _healthFacilityFromName2: string;
  private _healthFacilityToName2: string;
  private _designationFromName2: string;
  private _designationToName2: string;
  public leaveTypeName: string;
  public districtFrom2: string;
  public tehsilFrom2: string;
  private _districtTo2: string;
  private _tehsilTo2: string;
  public gender2: number = 0;
  private _esr2: ESR = new ESR();
  public orderResponses: any[] = [];

  private _searchedProfile: EmpProfile = new EmpProfile();
  private _searchedProfile2: EmpProfile = new EmpProfile();
  public leaveOrder: LeaveOrder = new LeaveOrder();
  public leaveOrderView: LeaveOrderView = new LeaveOrderView();
  esrView2: any;

  private _markupCC: string;
  private _transferType: number;
  constructor() { }


  get searchedProfile() {
    return this._searchedProfile;
  }
  set searchedProfile(searchedProfile: EmpProfile) {
    this._searchedProfile = searchedProfile;
  }
  get searchedProfile2() {
    return this._searchedProfile2;
  }
  set searchedProfile2(searchedProfile2: EmpProfile) {
    this._searchedProfile2 = searchedProfile2;
  }
  get transferType() {
    return this._transferType;
  }
  set transferType(transferType: number) {
    this._transferType = transferType;
  }
  get markupCC() {
    return this._markupCC;
  }
  set markupCC(markupCC: string) {
    this._markupCC = markupCC;
  }
  get esr() {
    return this._esr;
  }
  set esr(esr: ESR) {
    this._esr = esr;
  }
  get esrView() {
    return this._esrView;
  }
  set esrView(esrView: ESRView) {
    this._esrView = esrView;
  }
  get esrDetail() {
    return this._esrDetail;
  }
  set esrDetail(esrDetail: ESRDetail) {
    this._esrDetail = esrDetail;
  }
  get districtTo() {
    return this._districtTo;
  }
  set districtTo(districtTo: string) {
    this._districtTo = districtTo;
  }
  get tehsilTo() {
    return this._tehsilTo;
  }
  set tehsilTo(tehsilTo: string) {
    this._tehsilTo = tehsilTo;
  }

  get employeeName() {
    return this._employeeName;
  }
  set employeeName(employeeName: string) {
    this._employeeName = employeeName;
  }
  get healthFacilityFromName() {
    return this._healthFacilityFromName;
  }
  set healthFacilityFromName(healthFacilityFromName: string) {
    this._healthFacilityFromName = healthFacilityFromName;
  }
  get healthFacilityToName() {
    return this._healthFacilityToName;
  }
  set healthFacilityToName(healthFacilityToName: string) {
    this._healthFacilityToName = healthFacilityToName;
  }
  get designationFromName() {
    return this._designationFromName;
  }
  set designationFromName(designationFromName: string) {
    this._designationFromName = designationFromName;
  }
  get designationToName() {
    return this._designationToName;
  }
  set designationToName(designationToName: string) {
    this._designationToName = designationToName;
  }



  get esr2() {
    return this._esr2;
  }
  set esr2(esr: ESR) {
    this._esr2 = esr;
  }
  get districtTo2() {
    return this._districtTo2;
  }
  set districtTo2(districtTo: string) {
    this._districtTo2 = districtTo;
  }
  get tehsilTo2() {
    return this._tehsilTo2;
  }
  set tehsilTo2(tehsilTo: string) {
    this._tehsilTo2 = tehsilTo;
  }

  get employeeName2() {
    return this._employeeName2;
  }
  set employeeName2(employeeName: string) {
    this._employeeName2 = employeeName;
  }
  get healthFacilityFromName2() {
    return this._healthFacilityFromName2;
  }
  set healthFacilityFromName2(healthFacilityFromName: string) {
    this._healthFacilityFromName2 = healthFacilityFromName;
  }
  get healthFacilityToName2() {
    return this._healthFacilityToName2;
  }
  set healthFacilityToName2(healthFacilityToName: string) {
    this._healthFacilityToName2 = healthFacilityToName;
  }
  get designationFromName2() {
    return this._designationFromName2;
  }
  set designationFromName2(designationFromName: string) {
    this._designationFromName2 = designationFromName;
  }
  get designationToName2() {
    return this._designationToName2;
  }
  set designationToName2(designationToName: string) {
    this._designationToName2 = designationToName;
  }
}
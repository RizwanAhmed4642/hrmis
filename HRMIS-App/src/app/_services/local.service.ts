import { Injectable } from '@angular/core';
import { User } from '../_models/erp.class';

@Injectable({
    providedIn: 'root'
})
export class LocalService {
    constructor() { }
    public set(name: string, item: any) {
        localStorage.setItem(name, JSON.stringify(item));
    }
    public get(name: string) {
        return JSON.parse(localStorage.getItem(name));
    }
}

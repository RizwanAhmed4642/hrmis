import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CookieService } from '../_services/cookie.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    constructor(private _cookieService: CookieService) { }
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // add authorization header with jwt token if available

        if (request.url === 'https://fts.pshealthpunjab.gov.pk/Token') return next.handle(request);
        if (request.url.startsWith('https://fts.pshealthpunjab.gov.pk')) return next.handle(request);
        if (request.url.startsWith('http://125.209.111.70:88')) return next.handle(request);

        let token: any = this._cookieService.getCookie('ussr');
    /*     if (!token) {
            token = this._cookieService.getCookie('ussrpublic');
        } */

        if (token) {
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${token}`
                }
            });
        }
        return next.handle(request);
    }
}
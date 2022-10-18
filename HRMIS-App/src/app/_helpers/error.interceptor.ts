import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthenticationService } from '../_services/authentication.service';
import { NotificationService } from '../_services/notification.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private authenticationService: AuthenticationService, public _notificationService: NotificationService) { }
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            console.log(err);
            /*  if (err.status === 0 && !err.message.includes('https://ipapi.co/json')) {
                 this._notificationService.notify('danger', 'Server Not Responding');
             } */
            if (err.status === 401) {
                // auto logout if 401 response returned from api
                this._notificationService.notify('danger', 'Unauthorized');
                this.authenticationService.logout();
            }
            /*  if (err.status === 400) {
                 this._notificationService.notify('danger', 'Error Reported');
             } */
            if (err.status === 404) {
                this._notificationService.notify('danger', err.message);
            }
            const error = err;
            return throwError(error);
        }))
    }
}
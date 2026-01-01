//import { Injectable } from '@angular/core';
//import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
//import { Observable } from 'rxjs';
//import { tokenKey } from './authservice';

//@Injectable()
//export class TokenInterceptor implements HttpInterceptor {
//  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
//    const jwtToken = localStorage.getItem(tokenKey);
//    if (jwtToken) {
//      const cloned = req.clone({
//        setHeaders: {
//          Authorization: `Bearer ${jwtToken}`
//        }
//      });
//      return next.handle(cloned);
//    }
//    return next.handle(req);
//  }
//}

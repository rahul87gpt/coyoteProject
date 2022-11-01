import { Injectable } from '@angular/core';
import {SocialAuthService, SocialUser} from 'angularx-social-login';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree} from '@angular/router';
import {Observable} from 'rxjs';
import {map, tap} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class GoogleAuthGuardService implements CanActivate {

  constructor(private socialAuthService :SocialAuthService ,private router : Router) { }


  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.socialAuthService.authState.pipe(
      map((socialUser: SocialUser) => !!socialUser),
      tap((isLoggedIn: boolean) => {
        if (!isLoggedIn) {
          this.router.navigate(['login']);
        }
      })
    );
  }


 
}

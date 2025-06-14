import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';

@Injectable({
    providedIn: 'root',
})
export class AuthGuard implements CanActivate {
    constructor(
        private readonly oauthService: OAuthService,
        private readonly router: Router
    ) {}

    canActivate(): boolean {
        if (this.oauthService.hasValidAccessToken()) {
            return true;
        } else {
            this.router.navigate(['/login']);
            return false;
        }
    }
}

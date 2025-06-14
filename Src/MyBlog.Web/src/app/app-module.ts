import {
    NgModule,
    OnInit,
    provideBrowserGlobalErrorListeners,
} from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AuthConfig, OAuthService } from 'angular-oauth2-oidc';
import { App } from './app';
import { AppRoutingModule } from './app-routing-module';
import { CoreModule } from './core/core-module';
import { Login } from './features/login/login';

@NgModule({
    declarations: [App, Login],
    imports: [BrowserModule, AppRoutingModule, CoreModule],
    providers: [provideBrowserGlobalErrorListeners()],
    bootstrap: [App],
})
export class AppModule implements OnInit {
    private readonly authConfig: AuthConfig = {
        issuer: 'http://127.0.0.1:5500/Src/MyBlog.Auth/Templates/callback.html',
        redirectUri: window.location + '/index.html',
        clientId: '73dbadd5-d8c9-46ad-8b04-ba02cf4b06c3',
        responseType: 'code',
        scope: 'read',
        showDebugInformation: true,
    };
    constructor(private readonly oauthService: OAuthService) {}
    ngOnInit(): void {
        this.oauthService.configure(this.authConfig);
        this.oauthService.loadDiscoveryDocumentAndTryLogin();
    }
}

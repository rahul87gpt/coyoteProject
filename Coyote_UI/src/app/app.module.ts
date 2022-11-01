import { BrowserModule } from "@angular/platform-browser"; 
import {
  NgModule,
  CUSTOM_ELEMENTS_SCHEMA,
  NO_ERRORS_SCHEMA,
} from "@angular/core";
import {
  Router,
  NavigationStart,
  NavigationEnd,
  RouterModule,
} from "@angular/router";
import { AppComponent } from "./app.component";
// import { ViewModule } from './modules/section/view.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
// import { RestService } from './modules/section/services/rest.service';
import { FormsModule } from "@angular/forms";
import { HttpModule } from "@angular/http";
// import { CookieService } from 'ngx-cookie-service';
import { AppRoutingModule } from "./../app/app-routing.module";
import { ReactiveFormsModule } from "@angular/forms";
import { LoginComponent } from "./structure/account/login/login.component";
import { ForgotComponent } from "./structure/account/forgot/forgot.component";
import { ResetpassComponent } from "./structure/account/forgot/resetpass.component";
import { ForbiddenComponent } from "./structure/account/404/forbidden.component";
import { NorouteComponent } from "./structure/account/404/noroute.component";
import { FilterPipe } from "./pipes/filter.pipe";
import { ComponentModule } from "./structure/component/component.module";
import { PipesModule } from "./pipes/pipes.module";

import { ApiService } from "../app/service/Api.service";
import { AlertService } from "../app/service/alert.service";
import { NotifierModule, NotifierOptions } from "angular-notifier";
import { LoadingBarHttpClientModule } from "@ngx-loading-bar/http-client";
import { LoadingBarModule } from "@ngx-loading-bar/core";

import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { ConfirmationDialogComponent } from "./confirmation-dialog/confirmation-dialog.component";
import { ConfirmationDialogService } from "./confirmation-dialog/confirmation-dialog.service";
import { EncrDecrService } from "../app/EncrDecr/encr-decr.service";
import { ChangePasswordComponent } from "./structure/account/change-password/change-password.component";

import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { CommonSelectComponent } from "./common-select/common-select.component";
import { CommonSelectService } from "./common-select/common-select.component.service";

import { HttpTokenInterceptor } from "./service/interceptor";
import { AutofocusDirective } from './directive/autofocus.directive';
import { MyCommonModule } from "./my-common/my-common.module";
import { NgIdleKeepaliveModule } from "@ng-idle/keepalive";
import { MomentModule } from "angular2-moment";
import { IdleTimeoutService } from "./service/idle-timeout.service";
import { AutocompleteOffDirective } from './directive/autocomplete-off.directive';
import { GoogleAuthComponent } from './structure/account/google-auth/google-auth.component';

import {GoogleLoginProvider, SocialLoginModule} from 'angularx-social-login';


export let browserRefresh = false;
/// HttpTokenInterceptor
/**
 * Custom angular notifier options
 */
const customNotifierOptions: NotifierOptions = {
  position: {
    horizontal: {
      position: "right",
      distance: 12,
    },
    vertical: {
      position: "top",
      distance: 12,
      gap: 10,
    },
  },
  theme: "material",
  behaviour: {
    autoHide: 5000,
    onClick: "hide",
    onMouseover: "pauseAutoHide",
    showDismissButton: true,
    stacking: 4,
  },
  animations: {
    enabled: true,
    show: {
      preset: "slide",
      speed: 300,
      easing: "ease",
    },
    hide: {
      preset: "fade",
      speed: 300,
      easing: "ease",
      offset: 50,
    },
    shift: {
      speed: 300,
      easing: "ease",
    },
    overlap: 150,
  },
};


@NgModule({
  imports: [
    ReactiveFormsModule,
    BrowserModule,
    FormsModule,
    HttpClientModule,
    ComponentModule,
    AppRoutingModule,
    PipesModule,
    HttpModule,
    RouterModule.forRoot([]),
    NotifierModule.withConfig(customNotifierOptions),
    LoadingBarHttpClientModule,
    LoadingBarModule,
    NgbModule,
    BrowserAnimationsModule,
    NgIdleKeepaliveModule.forRoot(),
    MomentModule,
    SocialLoginModule
  ],
  declarations: [
    AppComponent,
    LoginComponent,
    ForgotComponent,
    ResetpassComponent,
    ForbiddenComponent,
    NorouteComponent,
    ConfirmationDialogComponent,
    ChangePasswordComponent,
    CommonSelectComponent,
    AutofocusDirective,
    AutocompleteOffDirective,
    GoogleAuthComponent
  ],
  providers: [
    ApiService,
    AlertService,
    ConfirmationDialogService,
    EncrDecrService,
    CommonSelectService,
    AutofocusDirective,
    IdleTimeoutService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpTokenInterceptor,
      multi: true,
    },
    {
      provide: 'SocialAuthServiceConfig',
      useValue: {
        autoLogin: true,
        providers: [
          {
            id: GoogleLoginProvider.PROVIDER_ID,
            provider: new GoogleLoginProvider('')
          }
        ]
      }
    },
  ],
  bootstrap: [AppComponent],
  entryComponents: [CommonSelectComponent],
  exports: [CommonSelectComponent,PipesModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
})
export class AppModule {}

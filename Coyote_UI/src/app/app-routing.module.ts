import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { BrowserModule } from "@angular/platform-browser";
import { ReactiveFormsModule } from "@angular/forms";
import { HttpModule } from "@angular/http";
import { AppComponent } from "./app.component";
import { CommonModule } from "@angular/common";

/* CUSTOM CREATED MODULES */
import { LoginComponent } from "./structure/account/login/login.component";
import { ForgotComponent } from "./structure/account/forgot/forgot.component";
import { ResetpassComponent } from "./structure/account/forgot/resetpass.component";
import { ForbiddenComponent } from "./structure/account/404/forbidden.component";
import { NorouteComponent } from "./structure/account/404/noroute.component";
import { ChangePasswordComponent } from "./structure/account/change-password/change-password.component";

const routes: Routes = [
  {
    path: "",
    component: AppComponent,
    children: [
      {
        path: "",
        redirectTo: "/login",
        pathMatch: "full",
      },
      {
        path: "login",
        component: LoginComponent,
      },

      {
        path: "forgot",
        component: ForgotComponent,
      },
      {
        path: "reset-password",
        component: ResetpassComponent,
      },
      {
        path: "forbidden",
        component: ForbiddenComponent,
      },
      {
        path: "change-password",
        component: ChangePasswordComponent,
      },
      {
        path: "**",
        component: NorouteComponent,
      },
    ],
  },
];

@NgModule({
  imports: [CommonModule, RouterModule.forRoot(routes)],
  exports: [RouterModule],
  declarations: [],
})
export class AppRoutingModule {}

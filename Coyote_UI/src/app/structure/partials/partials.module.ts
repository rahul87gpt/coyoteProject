import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { HeaderComponent } from "./header/header.component";
import { FooterComponent } from "./footer/footer.component";
import { SidebarComponent } from "./sidebar/sidebar.component";
import { RouterModule } from "@angular/router";
import { BreadCrumbsComponent } from "./bread-crumbs/bread-crumbs.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

@NgModule({
  declarations: [
    HeaderComponent,
    FooterComponent,
    SidebarComponent,
    BreadCrumbsComponent,
  ],
  imports: [CommonModule, RouterModule, FormsModule,
    ReactiveFormsModule,],
  exports: [
    HeaderComponent,
    FooterComponent,
    SidebarComponent,
    BreadCrumbsComponent,
  ],
})
export class PartialsModule {}

import { Component, OnInit, OnDestroy } from "@angular/core";
import { SharedService } from "src/app/service/shared.service";

@Component({
  selector: "app-bread-crumbs",
  templateUrl: "./bread-crumbs.component.html",
  styleUrls: ["./bread-crumbs.component.scss"],
})
export class BreadCrumbsComponent implements OnInit, OnDestroy {
  breadCrumbs = [];
  subscription: any;
  constructor(private sharedService: SharedService) {
    this.subscription = this.sharedService.BreadCrumbsData.subscribe((res) => {
      this.breadCrumbs = res;
    });
  }

  ngOnInit(): void {}
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
  goToDashboard() {}
}

import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, NavigationStart, NavigationEnd, NavigationCancel, NavigationError } from '@angular/router';
import { LoadingBarService } from '@ngx-loading-bar/core';
import { filter } from 'rxjs/operators';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
import { StocktakedataService } from 'src/app/service/stocktakedata.service';
declare var $: any;
@Component({
  selector: 'app-main-component',
  templateUrl: './main-component.component.html',
  styleUrls: ['./main-component.component.scss']
})
export class MainComponentComponent implements OnInit {

  loginUserData : any = {};
  currentUrl: any;
  previousUrl: any;

  constructor(public apiService: ApiService, private alert: AlertService,
    private route: ActivatedRoute, private router: Router, private loadingBar: LoadingBarService ,private urlService :StocktakedataService) {
      router.events.subscribe(event => {
        if(event instanceof NavigationStart) {
          // console.log("event started")
          this.loadingBar.start();          
        }else if(event instanceof NavigationEnd || NavigationCancel || NavigationError) {
          // console.log("event end")
          this.loadingBar.complete();
          // removed code for back-drop issue
        }
        
        // NavigationEnd
        // NavigationCancel
        // NavigationError
        // RoutesRecognized
      });
     }

  ngOnInit() {
    this.loginUserData = localStorage.getItem("loginUserData");
    this.loginUserData = JSON.parse(this.loginUserData);
    
    if(!this.loginUserData) {
      this.router.navigate(["login"]);   
    }
    // this.getStoreById();
    // fixing back-drop issue
    $(".modal").on("hidden.bs.modal", function(){
      if(!$('.modal').hasClass('show')) {
        $(document.body).removeClass("modal-open");
        $(".modal-backdrop").remove();
      }
    });
    // $(".modal").modal({keyboard: true});
    this.setUrl();
  }

  getStoreById() {
    this.apiService.GET("Store/1").subscribe(storeResponse => {
    }, (error) => { 
      if(error.status=='401') {
        localStorage.removeItem("loginUserData");
        this.alert.notifyErrorMessage("Your session has expired!"); 
        this.router.navigate(["login"]); 
      }

    });
  }

  private setUrl(){
    this.router.events.pipe(
      filter((event) => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      this.previousUrl = this.currentUrl;
      this.currentUrl = event.url;
      this.urlService.setPreviousUrl(this.previousUrl);
    });
  }
}



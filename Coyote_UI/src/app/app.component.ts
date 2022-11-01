import { Component } from '@angular/core';
import { SharedService } from 'src/app/service/shared.service';
import { ApiService } from 'src/app/service/Api.service';
import { Router, ActivatedRoute, NavigationStart } from '@angular/router';

import headerSidebarData from 'src/app/lib/headerSidebarData.json';
import * as jwt_decode from 'jwt-decode';
import { DEFAULT_INTERRUPTSOURCES, Idle } from '@ng-idle/core';
import { Keepalive } from '@ng-idle/keepalive';
import { IdleTimeoutService } from './service/idle-timeout.service';
import { Title } from '@angular/platform-browser';
import { Subscription } from 'rxjs';

export let browserRefresh = false;
declare var $:any;
 
@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrls: ['./app.component.scss']
})
export class AppComponent {
	title = 'Coyote Software';
	differenceInTimeInSecond: number = 900;
	subscription: Subscription;

	idleState = 'Not started.';
    timedOut = false;
    lastPing?: Date = null;
	login_Obj :any;
	constructor(
		public sharedService: SharedService,
		public apiService: ApiService,
		public router: Router,
		private route: ActivatedRoute,
		private idle: Idle, private keepalive: Keepalive, 
		private idleTimeoutService :IdleTimeoutService,
		private titleService: Title
	) {

		this.subscription = router.events.subscribe((event) => {
			if (event instanceof NavigationStart) {
			  browserRefresh = !router.navigated;
			}
		});

		let loginObj = JSON.parse(JSON.stringify(localStorage.getItem("loginUserData")));
		loginObj = JSON.parse(loginObj);
		this.login_Obj = loginObj; 

		if (this.router.url !== '/login' && loginObj && loginObj.tokenExpiration) {

			var decoded = jwt_decode(loginObj.token);
			var todaysDate = new Date();
			var tokenDate = decoded.exp;

			var todaysDateInSeconds = Math.floor(todaysDate.getTime() / 1000);
			var tokenDateInSeconds = Math.floor(tokenDate);

			var timeDifferenceInSecond = todaysDateInSeconds - tokenDateInSeconds;
			// var timeInMinutes = Math.abs(Math.floor(timeDifferenceInSecond / 60));

			if (Math.abs(timeDifferenceInSecond) <= this.differenceInTimeInSecond) {
				this.revokeToken();
			} else {
				setTimeout(() => {
					this.revokeToken();
				}, (Math.abs(timeDifferenceInSecond*1000)));
			}
		}
		this.sharedService.shareHeaderSubject.next(headerSidebarData.pricing);	


		idle.setIdle(300);
		// sets a timeout period of 5 seconds. after 10 seconds of inactivity, the user will be considered timed out.
	   idle.setTimeout(30);
		// sets the default interrupts, in this case, things like clicks, scrolls, touches to the document
	   idle.setInterrupts(DEFAULT_INTERRUPTSOURCES);
   
	   idle.onIdleEnd.subscribe(()=>{
		 this.idleState = 'No longer idle.'
		 this.resetIdleTime();
	   });
   
	   idle.onTimeout.subscribe(()=>{
		 this.idleState = 'Timed out!';
		 this.timedOut = true;
		 this.log_out();
	   });
   
	   idle.onIdleStart.subscribe(()=>{
		 this.idleState = 'You\'ve gone idle!'
		 $("#idleTime_out").modal("show");
	   });

	   idle.onTimeoutWarning.subscribe((countdown) => {
		this.idleState = 'You will time out in ' + countdown + ' seconds!'
	  });
  
	  // sets the ping interval to 15 second
	   keepalive.interval(15);
	   keepalive.onPing.subscribe(() => this.lastPing = new Date());
	   this.CallFunction();

	   if(loginObj && loginObj.userId){
		this.idleTimeoutService.setUserLoggedIn(true); 
	   }
		
	}

	public CallFunction(){
		this.idleTimeoutService.getUserLoggedIn().subscribe(userLoggedIn => {
			if (userLoggedIn) {
			this.idle.watch()
			this.timedOut = false;
			} else {
			this.idle.stop();
			}
		}) ;
	}

	

	revokeToken() {
		let loginObj = JSON.parse(JSON.stringify(localStorage.getItem("loginUserData")));
		loginObj = JSON.parse(loginObj);

		if (loginObj && loginObj.userId) {
			this.apiService
				.LOGIN("Login/RevokeToken", JSON.stringify({
					userId: loginObj.userId,
					refreshToken: loginObj.refreshToken
				})
				)
				.subscribe(loginData => {

					// Set updated value / token
					loginObj.token = loginData.token;
					loginObj.tokenExpiration = loginData.tokenExpiration;
					loginObj.refreshToken = loginData.refreshToken;

					localStorage.setItem("loginUserData", JSON.stringify(loginObj));


					if (this.router.url !== '/login' && loginData && loginData.tokenTimeOut) {
						setTimeout(() => {
							this.revokeToken();
						}, (Math.floor((loginData.tokenTimeOut * 60) - this.differenceInTimeInSecond)*1000));
					}
				})
		}
	}


	private resetIdleTime(){
		this.idle.watch();
		this.timedOut  = false;
	   }
	 
	public stayOnPage() {
	  $("#idleTime_out").modal("hide");
	  this.resetIdleTime();
	}
	 
	public log_out(){
		 $("#idleTime_out").modal("hide");
		 this.idleTimeoutService.setUserLoggedIn(false);
		 localStorage.removeItem("loginUserData");
		 localStorage.removeItem("Header");
		 localStorage.removeItem("moduleName");
		 localStorage.removeItem("masterListId");
		 localStorage.removeItem("masterListText");
		 localStorage.removeItem("masterListCode");
		 this.titleService.setTitle('Coyote Console');
		 localStorage.setItem('RM','false');
		 this.router.navigate(['/login']).then(() => {
          window.location.reload();
        });
		 
		

		

	}

}


import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent implements OnInit {
  loginUserData:any;
  todayDate : Date = new Date();
  constructor( ) { }

  ngOnInit() {
    this.loginUserData = localStorage.getItem("loginUserData");
    this.loginUserData = JSON.parse(this.loginUserData);
    // console.log(' this.loginUserData', this.loginUserData);
  }


}

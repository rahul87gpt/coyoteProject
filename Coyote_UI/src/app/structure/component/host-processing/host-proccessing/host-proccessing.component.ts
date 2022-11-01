import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/service/shared.service';
import moment from 'moment';
declare var $: any;
@Component({
  selector: 'app-host-proccessing',
  templateUrl: './host-proccessing.component.html',
  styleUrls: ['./host-proccessing.component.scss']
})
export class HostProccessingComponent implements OnInit {
  hostProcessingPopupOpen: any;
  currentDate = new Date();
  minDate: Date;
  current_Date: Date;
  weekNumber: any;
  itemSelected: boolean = false;

  constructor(private sharedService: SharedService) {
    this.minDate = new Date();
  }

  ngOnInit(): void {
    this.currentDate = new Date(this.currentDate.getFullYear(), this.minDate.getMonth(), this.minDate.getDate(), this.minDate.getHours(), this.minDate.getMinutes(), this.minDate.getSeconds());
    console.log('this.currentDate', this.currentDate);
    this.weekNumber = moment(this.currentDate).isoWeek()

    console.log('this.weekNo', this.weekNumber);


    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.hostProcessingPopupOpen = popupRes.endpoint;
      if (this.hostProcessingPopupOpen === '/host-processing/host-processing') {
        setTimeout(() => {
          $('#hostProcessing').modal('show');
        }, 10);
      }
    });
  }

  public selectItem(isChecked) {
    if (isChecked) {
      this.itemSelected = true;
    }
    else {
      this.itemSelected = false;
    }
  }

}

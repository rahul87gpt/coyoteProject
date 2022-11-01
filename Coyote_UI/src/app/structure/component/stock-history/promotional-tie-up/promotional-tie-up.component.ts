import { Component, OnInit } from '@angular/core';
declare var $ :any;
@Component({
  selector: 'app-promotional-tie-up',
  templateUrl: './promotional-tie-up.component.html',
  styleUrls: ['./promotional-tie-up.component.scss']
})
export class PromotionalTieUpComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
    $("#promotional-tie-up").modal("show");
  }

}

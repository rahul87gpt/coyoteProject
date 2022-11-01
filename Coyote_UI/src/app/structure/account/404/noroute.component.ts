import { Component, OnInit } from '@angular/core';
import { Router,ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-noroute',
  templateUrl: './noroute.component.html'
})
export class NorouteComponent implements OnInit {



    constructor(public route: ActivatedRoute, public router: Router) {}

    ngOnInit() {
       
    }

}

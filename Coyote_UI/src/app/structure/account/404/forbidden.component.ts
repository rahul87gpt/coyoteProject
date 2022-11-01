import { Component, OnInit } from '@angular/core';
import { Router,ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-forbidden',
  templateUrl: './forbidden.component.html'
})
export class ForbiddenComponent implements OnInit {



    constructor(public route: ActivatedRoute, public router: Router) {
	
    }

    ngOnInit() {
       
    }

}

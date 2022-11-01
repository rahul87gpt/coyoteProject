import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';

@Component({
  selector: 'app-corporate-tree',
  templateUrl: './corporate-tree.component.html',
  styleUrls: ['./corporate-tree.component.scss']
})
export class CorporateTreeComponent implements OnInit {
  corporateData:any;
  storelist:any
  constructor(private apiservice: ApiService) { }

  ngOnInit(): void {
    this.getcorporatetree();
  }
getcorporatetree(){
  this,this.apiservice.GET('StoreGroup/CorporateTree').subscribe(response=>{
console.log(response);
this.corporateData= response.data   ;
setTimeout(()=>{
  var toggler = document.getElementsByClassName("caret");
  var i;

  for (i = 0; i < toggler.length; i++) {
  toggler[i].addEventListener("click", function() {
      this.parentElement.querySelector(".nested")?.classList.toggle("active");
      this.classList.toggle("caret-down");
  });
}
},1000)

  })
}

  
}


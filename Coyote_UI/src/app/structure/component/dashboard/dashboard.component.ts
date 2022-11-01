import { Component, OnInit } from '@angular/core';
import mCache from 'memory-cache';
import { ApiService } from '../../../service/Api.service';
import { AlertService } from 'src/app/service/alert.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  constructor(
    public apiService: ApiService,
    private alert: AlertService) {

  }

  ngOnInit(): void {
    //var newCache = new mCache.Cache();
    let manufacturers: any = JSON.parse(mCache.get("manufacturers"));
    if (!manufacturers) {
      this.getManufacturer();
    }
    let suppliers: any = JSON.parse(mCache.get("suppliers"));
    if (!suppliers) {
      this.getSuppliers();

    }
    let department: any = JSON.parse(mCache.get("department"));
    if (!department) {
      this.departments();
    }

    let commodity: any = JSON.parse(mCache.get("commodity"));
    if (!commodity) {
      this.commodity();
    }

    let store: any = JSON.parse(mCache.get("store"));
    if (!store) {
      this.store();
    }
    let cashier: any = JSON.parse(mCache.get("cashier"));
    if (!cashier) {
      this.cashier();
    }

    let groups: any = JSON.parse(mCache.get("groups"));
    if (!cashier) {
      this.groups();
    }

    // console.log(department,"department");
    // console.log(manufacturers,"manufacturers");
    // console.log(suppliers,"suppliers");
    // console.log(groups,"groups");
    // console.log(cashier,"cashier");
    // console.log(store,"store");
    // console.log(store,"commodity");
    //newCache.debug(true)
  }

  public groups(dataLimit = 22000, skipValue = 0, isFirstTime = false) {
    this.apiService.GET(`MasterListItem/code?Sorting=name&Direction=[asc]&code=GROUP&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
      mCache.put("groups", JSON.stringify(response.data));
    }, (error) => {
      let errorMsg = this.errorHandling(error);
      this.alert.notifyErrorMessage(errorMsg)
    });
  }

  public cashier(dataLimit = 22000, skipValue = 0, isFirstTime = false) {
    this.apiService.GET(`cashier?Sorting=number&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
      mCache.put("cashier", JSON.stringify(response.data));
    }, (error) => {
      let errorMsg = this.errorHandling(error);
      this.alert.notifyErrorMessage(errorMsg)
    });
  }

  public store(dataLimit = 22000, skipValue = 0, isFirstTime = false) {
    this.apiService.GET(`store?Sorting=[desc]&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
      .subscribe(response => {
        mCache.put("store", JSON.stringify(response.data));
      }, (error) => {
        let errorMsg = this.errorHandling(error);
        this.alert.notifyErrorMessage(errorMsg)
      });
  }

  public commodity(dataLimit = 22000, skipValue = 0, isFirstTime = false) {
    this.apiService.GET(`Commodity?Sorting=Desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
      mCache.put("commodity", JSON.stringify(response.data));
    }, (error) => {
      let errorMsg = this.errorHandling(error);
      this.alert.notifyErrorMessage(errorMsg)
    });
  }

  public departments(dataLimit = 22000, skipValue = 0, isFirstTime = false) {
    this.apiService.GET(`department?Sorting=Desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
      mCache.put('department', JSON.stringify(response.data));

    }, (error) => {
      let errorMsg = this.errorHandling(error);
      this.alert.notifyErrorMessage(errorMsg)
    });
  }

  public getManufacturer(dataLimit = 22000, skipValue = 0, isFirstTime = false) {
    var url = `MasterListItem/code?Sorting=name&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}&code=MANUFACTURER&Sorting=name`;

    this.apiService.GET(url).subscribe(response => {
      //console.log(response.data);
      mCache.put('manufacturers', JSON.stringify(response.data));

    }, (error) => {
      this.alert.notifyErrorMessage(error.message);
    });
  }
  //suppliers
  public getSuppliers(dataLimit = 22000, skipValue = 0, isFirstTime = false) {
    this.apiService.GET(`Supplier?Sorting=Desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`)
      .subscribe(response => {
        mCache.put('suppliers', JSON.stringify(response.data));
      }, (error) => {
        let errorMsg = this.errorHandling(error);
        this.alert.notifyErrorMessage(errorMsg)
      });
  }

  public errorHandling(error) {
    let err = error;
    console.log(' -- errorHandling: ', err)

    if (error && error.error && error.error.message)
      err = error.error.message
    else if (error && error.message)
      err = error.message

    return err;
  }
}

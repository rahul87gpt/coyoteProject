import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/service/Api.service';
import { FormArray, FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AlertService } from 'src/app/service/alert.service';
import { SharedService } from 'src/app/service/shared.service';
import { array } from '@amcharts/amcharts4/core';
declare var $: any;
@Component({
  selector: 'app-deactivate-products',
  templateUrl: './deactivate-products.component.html',
  styleUrls: ['./deactivate-products.component.scss']
})
export class DeactivateProductsComponent implements OnInit {
  deactivateProductsForm: FormGroup;
  outletGroup: any;
  departmentsData: any;
  commoditiesData: any;
  departmentData: any;
  array: any = [];
  submitted: boolean = false;
  noSalesDayId: any;
  deactivateProductList: any;
  selectedDeparment: any;
  setDate: any;
  SetDateForNoSales: any;
  changeEvent: any;
  isSearchPopupOpen: any;
  departmentObj = { department: [], commodity: [], department_data: [], commodity_data: [] };
  noSalesDaysArray: any = [{ "id": 60, "name": "60" }, { "id": 90, "name": "90" }, { "id": 120, "name": "120" }, { "id": 150, "name": "150" }, { "id": 180, "name": "180" }, { "id": 365, "name": "365" }];

  reporterObj = {
    sortOrderType: '',
    currentUrl: null,
    check_exitance: {},
    hold_entire_response: {},
    checkbox_checked: {},
    button_text: {},
    select_all_ids: {},
    select_all_id_exitance: {},
    select_all_obj: {},
    open_count: {},
    clear_all: {},
    remove_index_map: {},
    departmentIdsExc: [],
    departmentIdsInc: [],
    commodityIdsExc: [],
    commodityIdsInc: [],

    dropdownField: {

      departmentsExc: 'departmentsExc',
      departmentIdsExc: 'departmentIdsExc',


      departmentsInc: 'departmentsInc',
      departmentIdsInc: 'departmentIdsInc',



      commoditiesExc: 'commoditiesExc',
      commodityIdsExc: 'commodityIdsExc',


      commoditiesInc: 'commoditiesInc',
      commodityIdsInc: 'commodityIdsInc',
    }
  };

  isApiCalled: boolean = false;
  dropdownObj = {

    departmentsExc: [],
    departmentsInc: [],

    commoditiesExc: [],
    commoditiesInc: [],

    self_calling: true,
    count: 0
  };

  selectedValues: any = {

    departmentExc: null,
    commodityExc: null,
    departmenInc: null,
    commodityInc: null,
  }

  searchBtnObj = {
    departmentsExc: {
      text: null,
      fetching: false,
      name: 'departmentsExc',
      searched: ''
    },

    departmentsInc: {
      text: null,
      fetching: false,
      name: 'departmentsInc',
      searched: ''
    },
    commoditiesExc: {
      text: null,
      fetching: false,
      name: 'commoditiesExc',
      searched: ''
    },

    commoditiesInc: {
      text: null,
      fetching: false,
      name: 'commoditiesInc',
      searched: ''
    },

  }

  constructor(private apiService: ApiService, private formBuilder: FormBuilder, private alert: AlertService,
    private sharedService: SharedService) { }

  ngOnInit(): void {
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.changeEvent = !this.changeEvent;



      this.isSearchPopupOpen = popupRes.endpoint;
      if (this.isSearchPopupOpen === '/deactivate-products') {

        if ($.fn.DataTable.isDataTable("#DeactivateList-table")) {
          $("#DeactivateList-table").DataTable().destroy();
        }

        this.deactivateProductList = [];

        setTimeout(() => {
          $("#DeactivateProducts").modal("show");


        }, 1);
      }
    });

    this.deactivateProductsForm = this.formBuilder.group({
      date: ['', Validators.required],
      storeId: ['', Validators.required],
      departmentIdsInc: [''],
      departmentIdsExc: [''],
      commodityIdsInc: [''],
      commodityIdsExc: [''],
      qtyOnHandZero: [true],
      userPassword: ['']
    });

    this.getOutlet();
    this.getCommodities();
    this.getDepartments();
    this.deactivateProductsForm.get('date').setValue(90);
    var dateOffset = (24 * 60 * 60 * 1000) * 90;
    var myDate = new Date();
    myDate.setTime(myDate.getTime() - dateOffset);
    this.SetDateForNoSales = myDate;
  }
  get f() { return this.deactivateProductsForm.controls; }
  private getOutlet() {
    this.apiService.GET('Store?Sorting=[desc]').subscribe(storeRes => {
      this.outletGroup = storeRes.data;
      // console.log('StoreGroup', storeRes);
    },
      error => {
        console.log(error);
      });
  }
  private getDepartments(dataLimit = 1000, skipValue = 0) {

    this.apiService.GET(`Department?Sorting=desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
      this.dropdownObj.departmentsExc = response.data;
      this.dropdownObj.count++;
      this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departmentsExc] = JSON.parse(JSON.stringify(response.data));

      this.dropdownObj.departmentsInc = response.data;
      this.dropdownObj.count++;
      this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.departmentsInc] = JSON.parse(JSON.stringify(response.data));
    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });

    // this.apiService.GET('Department?Sorting=desc').subscribe(departmentRes => {
    //   this.departmentData = departmentRes.data;
    //   for (let index = 0; index < this.departmentData.length; index++) {
    //     const element = this.departmentData[index];
    //     if (element.isDefualt) {
    //       this.deactivateProductsForm.patchValue({ departmentIdsExc: element.id.toString() })
    //     }

    //   }
    // },
    //   error => {
    //     console.log(error);
    //   });
  }
  private getCommodities(dataLimit = 1000, skipValue = 0) {

    this.apiService.GET(`Commodity?Sorting=desc&Direction=[asc]&MaxResultCount=${dataLimit}&SkipCount=${skipValue}`).subscribe(response => {
      this.dropdownObj.commoditiesExc = response.data;
      this.dropdownObj.count++;
      this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.commoditiesExc] = JSON.parse(JSON.stringify(response.data));

      this.dropdownObj.commoditiesInc = response.data;
      this.dropdownObj.count++;
      this.reporterObj.hold_entire_response[this.reporterObj.dropdownField.commoditiesInc] = JSON.parse(JSON.stringify(response.data));

    }, (error) => {
      this.alert.notifyErrorMessage(error?.error?.message);
    });

    // this.apiService.GET('Commodity?Sorting=desc').subscribe(commodityRes => {
    //   this.commoditiesData = commodityRes.data;
    //   console.log('commodityRes', commodityRes);
    // },
    //   error => {
    //     console.log(error);
    //   });
  }
  // public onCheckChange(formKey, type ? , updateDataExist ?,) {
  // 	if (formKey && formKey.toLowerCase() === "department") {
  //     const checkArray= this.deactivateProductsForm.get('departmentIdsExc') as FormArray; 
  //     var indexValue = checkArray.value.indexOf(type);
  // 	  console.log('  var checkArray',  checkArray);
  //     if (!updateDataExist && indexValue !== -1)
  //      return checkArray.removeAt(indexValue);
  //       else if (indexValue === -1) 
  // 		 checkArray.push(new FormControl(type));
  //     } else {
  //         var formKeyObj = {};
  //         formKeyObj[formKey] = type;
  //         this.deactivateProductsForm.patchValue(formKeyObj);
  //   }
  // }
  submitDeactivateProductsForm() {
    this.submitted = true;
    let prodObj = JSON.parse(JSON.stringify(this.deactivateProductsForm.value));
    prodObj.storeId = Number(prodObj.storeId);
    prodObj.qtyOnHandZero = (prodObj.multipleOrdersInAWeek == "true" || prodObj.qtyOnHandZero == true) ? true : false;
    if (this.noSalesDayId > 0) {
      prodObj.date = this.setDate.toISOString().substring(0, 10);

    } else {
      prodObj.date = this.SetDateForNoSales.toISOString().substring(0, 10);

    }

    let departmentExclude = '' + this.reporterObj.departmentIdsExc + '';
    let departmentInclude = '' + this.reporterObj.departmentIdsInc + '';
    let commodityExclude = '' + this.reporterObj.commodityIdsExc + '';
    let commodityInclude = '' + this.reporterObj.commodityIdsInc + '';
    prodObj.departmentIdsExc = departmentExclude;
    prodObj.departmentIdsInc = departmentInclude;
    prodObj.commodityIdsExc = commodityExclude;
    prodObj.commodityIdsInc = commodityInclude;

    if (this.deactivateProductsForm.valid) {
      this.getDeactivateList(prodObj);
    }
  }
  private getDeactivateList(prodObj) {
    if ($.fn.DataTable.isDataTable("#DeactivateList-table")) {
      $("#DeactivateList-table").DataTable().destroy();
    }
    this.apiService
      .POST("OutletProduct/DeactivateList", prodObj)
      .subscribe(
        (deactivateListProductResponse) => {
          console.log('SwitchRoleResponse', deactivateListProductResponse)
          this.deactivateProductList = deactivateListProductResponse?.data;
          if (this.deactivateProductList?.length) {
            this.alert.notifySuccessMessage(deactivateListProductResponse.totalCount + " Products found To Deactivate");
          } else {
            this.alert.notifySuccessMessage("No Products found");
          }
          if (deactivateListProductResponse.totalCount > 0) {
            setTimeout(() => {
              $("#DeactivateList-table").DataTable({
                order: [],
                // scrollY: 300,
                "columnDefs": [{
                  "targets": 'text-center',
                  "orderable": false,
                }]
              });
            }, 500);
          }

        },
        (error) => {
          this.alert.notifyErrorMessage(error.error.message);
        }
      );
  }

  deactivateProduct() {
    let prodObj = JSON.parse(JSON.stringify(this.deactivateProductsForm.value));
    prodObj.storeId = Number(prodObj.storeId);
    prodObj.qtyOnHandZero = (prodObj.multipleOrdersInAWeek == "true" || prodObj.qtyOnHandZero == true) ? true : false;
    if (this.noSalesDayId > 0) {
      prodObj.date = this.setDate.toISOString().substring(0, 10);
    }
    prodObj.date = this.SetDateForNoSales.toISOString().substring(0, 10);
    let departmentExclude = '' + prodObj.departmentIdsExc + '';
    let departmentInclude = '' + prodObj.departmentIdsInc + '';
    let commodityExclude = '' + prodObj.commodityIdsExc + '';
    let commodityInclude = '' + prodObj.commodityIdsInc + '';
    prodObj.departmentIdsExc = departmentExclude;
    prodObj.departmentIdsInc = departmentInclude;
    prodObj.commodityIdsExc = commodityExclude;
    prodObj.commodityIdsInc = commodityInclude;

    this.apiService.POST("OutletProduct/DeactivateProducts", prodObj).subscribe(Response => {
      this.alert.notifySuccessMessage("Products Deactivate successfully");
      this.deactivateProductList = [];
      this.clickedCancel();
    }, (error) => {
      let errorMessage = '';
      if (error.status == 400) {
        errorMessage = error.error.message;
      } else if (error.status == 404) { errorMessage = error.error.message; }
      console.log("Error =  ", error);
      this.alert.notifyErrorMessage(errorMessage);
    });

  }
  changenoSalesDaysArray(event) {
    if (this.noSalesDaysArray && this.noSalesDaysArray.length) {
      this.noSalesDaysArray.map((val) => {
        if (val.id === Number(event.target.value)) {
          this.noSalesDayId = val.id;
          var dateOffset = (24 * 60 * 60 * 1000) * this.noSalesDayId;
          var myDate = new Date();
          myDate.setTime(myDate.getTime() - dateOffset);
          this.setDate = myDate;

          console.log('this.noSalesDayId', this.noSalesDayId);


          return;
        }
      });
    }
  }
  clickedCancel() {
    this.deactivateProductList = [];
    this.submitted = false;
    this.noSalesDayId = 0;
    this.deactivateProductsForm.reset();
    this.deactivateProductsForm.get('date').setValue(90);
    this.deactivateProductsForm.get('qtyOnHandZero').setValue(true);
  }
  outletChange(event) {
    this.changeEvent = event;
  }

  public searchBtnAction(event, modeName: string, actionName?) {
    this.searchBtnObj[modeName].text = event?.term?.trim()?.toUpperCase() || this.searchBtnObj[modeName]?.text?.trim().toUpperCase();

    // console.log(modeName, ' --> ' , this.searchBtnObj[modeName].text, ' ==> ', this.searchBtnObj[modeName].searched)
    // console.log(this.searchBtnObj[modeName].searched.indexOf(this.searchBtnObj[modeName].text))

    if (!this.searchBtnObj[modeName].fetching && !event?.items.length && (this.searchBtnObj[modeName].text.length >= 3)) {

      if (!this.searchBtnObj[modeName].searched.includes(this.searchBtnObj[modeName].text)) {
        this.searchBtnObj[modeName].fetching = true;
        this.searchBtnObj[modeName].searched += `,${this.searchBtnObj[modeName].text}`;

        switch (modeName) {

          case this.reporterObj.dropdownField.commoditiesExc:
            this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'commoditiesExc', this.reporterObj.dropdownField.commoditiesExc)
            break;
          case this.reporterObj.dropdownField.commoditiesInc:
            this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'commoditiesInc', this.reporterObj.dropdownField.commoditiesInc)
            break;
          case this.reporterObj.dropdownField.departmentsExc:
            this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'departmentsExc', this.reporterObj.dropdownField.departmentsExc)
            break;
          case this.reporterObj.dropdownField.departmentsInc:
            this.getApiCallDynamically(null, null, this.searchBtnObj[modeName], 'departmentsInc', this.reporterObj.dropdownField.departmentsInc)
            break;
        }
      }
    }

  }

  private getApiCallDynamically(dataLimit = 1000, skipValue = 0, searchTextObj = null, endpointName = null, pluralName = null, masterListCodeName?) {

    var url = `${endpointName}?MaxResultCount=${dataLimit}&SkipCount=${skipValue}`;



    if (searchTextObj?.text) {
      searchTextObj.text = searchTextObj.text.replace(/ /g, '+').replace(/%27/g, '');
      url = `${endpointName}?GlobalFilter=${searchTextObj.text}`
    }

    this.apiService.GET(url)
      .subscribe((response) => {

        if (searchTextObj?.text) {
          this.alert.notifySuccessMessage(`${response.data.length} record found against "${this.searchBtnObj[searchTextObj.name].text}"`);
          this.searchBtnObj[searchTextObj.name].fetching = false;

          this.dropdownObj[pluralName] = this.dropdownObj[pluralName].concat(response.data);
        } else {
          this.dropdownObj[pluralName] = response.data;
        }
        console.log(this.dropdownObj)
        this.dropdownObj.count++;
        this.reporterObj.hold_entire_response[this.reporterObj.dropdownField[pluralName]] =
          JSON.parse(JSON.stringify(this.dropdownObj[pluralName]));
      },
        (error) => {
          this.alert.notifyErrorMessage((error.error ? error.error.message : error.error));
        }
      );
  }

  //  // WORK WHEN WE CLICK ON CHECKbOX
  public addOrRemoveItem(addOrRemoveObj: any, dropdownName: string, modeName: string, formkeyName?: string, formsIds?: any) {
    modeName = modeName.toLowerCase().replace(' ', '_').replace('-', '_')

    // if (modeName === "clear_all" || (modeName === "de_select_all" && this.salesReportForm.value[formkeyName]?.length)) {
    if (modeName === "clear_all") {
      // this.reporterObj.button_text[dropdownName] = 'Select All';

      // Remove all key-value from indax mapping if 'de-select(button) / clear_all(x button)' performed
      this.reporterObj.remove_index_map[dropdownName] = {};


      // this.deactivateProductsForm.patchValue({
      //   [formkeyName]: []
      // });

      // Make it empty when all removed, it stored value when single - 2 checkbox clicked and use to show on right side section
      this.selectedValues[dropdownName] = null;

    }

    else if (modeName === "add") {
      let idOrNumber = addOrRemoveObj.id || addOrRemoveObj.memB_NUMBER || addOrRemoveObj.name;
      this.reporterObj.remove_index_map[dropdownName][idOrNumber] = idOrNumber;
      let index = formsIds.indexOf(idOrNumber);
      if (index == -1) {
        formsIds.push(idOrNumber);
      }

      // let departmentIdsExc = '' + formsIds + '';

      // this.deactivateProductsForm.patchValue({
      //   [formkeyName]: departmentIdsExc
      // })


      // switch (formkeyName) {
      //   case 'departmentIdsExc':
      //     this.reporterObj.departmentIdsExc.push(idOrNumber);
      //     let departmentIdsExc = '' + this.reporterObj.departmentIdsExc + '';

      //     this.deactivateProductsForm.patchValue({
      //       [formkeyName]: departmentIdsExc
      //     })

      //     break;
      //   case 'departmentIdsInc':
      //     console.log('departmentIdsInc');
      //     break;

      //   case 'commodityIdsExc':
      //     console.log('commodityIdsExc');
      //     break;
      //   case 'commodityIdsInc':
      //     console.log('commodityIdsInc');
      //     break;

      // }

    }
    else if (modeName === "remove") {
      let idOrNumber = addOrRemoveObj.value.id || addOrRemoveObj.value.memB_NUMBER || addOrRemoveObj.value.name;
      delete this.reporterObj.remove_index_map[dropdownName][idOrNumber];

      let index = formsIds.indexOf(idOrNumber);
      if (index == 1) {
        formsIds.splice(index, idOrNumber);
      }


      // let departmentIdsExc = '' + formsIds + '';

      // this.deactivateProductsForm.patchValue({
      //   [formkeyName]: departmentIdsExc
      // })


      // Remove parent selected dropdown if all checkbox is de-select on right side
      if (Object.keys(this.reporterObj.remove_index_map[dropdownName]).length == 0)
        this.selectedValues[dropdownName] = null;

    }
  }


  // WORK WHEN WE CLICK ON fIELD
  public getAndSetFilterData(dropdownName, formkeyName?, shouldBindWithForm = false) {

    if (!this.reporterObj.open_count[dropdownName]) {
      this.reporterObj.open_count[dropdownName] = 0;

      // Service hold data if 'keep_filter' checkbox checked, so no need to initilize with empty if data available
      this.reporterObj.remove_index_map[dropdownName] = this.reporterObj.remove_index_map[dropdownName] || {};

      setTimeout(() => {
        this.reporterObj.open_count[dropdownName] = 1;
      });
    }
  }

  public setDropdownSelection(dropdownName: string, event: any) {
    // Avoid event bubling
    if (event && !event.isTrusted) {

      this.selectedValues[dropdownName] = JSON.parse(JSON.stringify(event));

    }
  }





  // submitDeactivateProductsForm(){
  //   this.submitted=true;
  //   let prodObj = JSON.parse(JSON.stringify(this.deactivateProductsForm.value));
  //   console.log('prodObj',prodObj);
  //   prodObj.storeId=Number(prodObj.storeId);
  //   prodObj.qtyOnHandZero = (  prodObj.multipleOrdersInAWeek == "true" ||  prodObj.qtyOnHandZero == true ) ? true : false;
  //   prodObj.departmentIdsInc=prodObj.departmentIdsInc > 0 ? prodObj.departmentIdsInc :'';
  //   prodObj.departmentIdsExc=prodObj.departmentIdsExc > 0 ? prodObj.departmentIdsExc :'';
  //   prodObj.commodityIdsExc=prodObj.commodityIdsExc > 0 ? prodObj.commodityIdsExc :'';
  //   prodObj.commodityIdsInc=prodObj.commodityIdsInc > 0 ? prodObj.commodityIdsInc :'';
  //   prodObj.date=this.SetDateForNoSales.toISOString().substring(0, 10);
  //   if( this.noSalesDayId>0){
  //    prodObj.date=this.setDate.toISOString().substring(0, 10);
  //   }

  //   let storeId = JSON.parse(JSON.stringify(prodObj.storeId));
  //   let date = JSON.parse(JSON.stringify( prodObj.date));
  //   let qtyOnHandZero = JSON.parse(JSON.stringify(prodObj.qtyOnHandZero));
  //   let departmentIdsExc = JSON.parse(JSON.stringify(prodObj.departmentIdsExc));
  //   let departmentIdsInc = JSON.parse(JSON.stringify(prodObj.departmentIdsInc));
  //   let commodityIdsExc = JSON.parse(JSON.stringify(prodObj.commodityIdsExc));
  //   let commodityIdsInc = JSON.parse(JSON.stringify(prodObj.commodityIdsInc));
  // }

  // ---------------------------- In future use===============================================

  // private getDeactivateList(storeId?,date?,qtyOnHandZero?,departmentIdsExc?,departmentIdsInc?,commodityIdsExc?,commodityIdsInc?){
  //     if ($.fn.DataTable.isDataTable("#DeactivateList-table")) {
  //       $("#DeactivateList-table").DataTable().destroy();
  //     }
  //     this.apiService
  //     .GET(`OutletProduct/DeactivateList?Date=${date}&StoreId=${storeId}&QtyOnHandZero=${qtyOnHandZero}&DepartmentIdsInc=${departmentIdsInc}&DepartmentIdsExc=${departmentIdsExc}&CommodityIdsInc=${commodityIdsInc}&CommodityIdsExc=${commodityIdsExc}`)
  //     .subscribe(
  //       (deactivateListProductResponse) => {
  //         console.log('SwitchRoleResponse',deactivateListProductResponse)
  //         this.deactivateProductList=deactivateListProductResponse.data;
  //         if(this.deactivateProductList?.length){
  //           this.alert.notifySuccessMessage( deactivateListProductResponse.totalCount + " Products found To Deactivate");
  //         }else{
  //           this.alert.notifySuccessMessage("No Products found");
  //         }

  //         setTimeout(() => {
  //           $("#DeactivateList-table").DataTable({
  //             order: [],
  //             scrollY: 300,
  //             "columnDefs": [ {
  //               "targets": 'text-center',
  //               "orderable": false,
  //              } ]
  //           });
  //         }, 500);
  //       },
  //       (error) => {
  //         this.alert.notifyErrorMessage(error.error.message);
  //     }
  //   );

  // }


}

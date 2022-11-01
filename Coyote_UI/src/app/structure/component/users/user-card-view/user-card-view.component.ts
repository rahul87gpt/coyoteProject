import { color, number } from '@amcharts/amcharts4/core';
import { FormatWidth } from '@angular/common';
import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ConfirmationDialogService } from 'src/app/confirmation-dialog/confirmation-dialog.service';
import { AlertService } from 'src/app/service/alert.service';
import { ApiService } from 'src/app/service/Api.service';
import { ExportService } from 'src/app/service/export.service';
import { SharedService } from 'src/app/service/shared.service';
import { StocktakedataService } from 'src/app/service/stocktakedata.service';
declare var $: any;

export interface ExcelJson {
  data: Array<any>;
  header?: Array<string>;
  skipHeader?: boolean;
  origin?: string | number;

}

@Component({
  selector: 'app-user-card-view',
  templateUrl: './user-card-view.component.html',
  styleUrls: ['./user-card-view.component.scss']
})
export class UserCardViewComponent implements OnInit {
  @ViewChild('userTable') userTable: ElementRef;
  userCardForm: FormGroup;
  userList: any;
  path: any;
  message: any;
  totalCount: any;
  endpoint: any;
  tableName = '#userCard-table';
  dataTable: any;
  recordObj = {
    lastSearchExecuted: null
  };
  croppedImage: any = 'assets/images/user-img.png';
  ; constructor(public apiService: ApiService, private alert: AlertService,
    private confirmationDialogService: ConfirmationDialogService, private dataservice: StocktakedataService,
    private formBuilder: FormBuilder, private router: Router, public exportService: ExportService, private sharedService: SharedService) { }


  ngOnInit(): void {
    this.sharedService.sharePopupStatusData.subscribe((popupRes) => {
      this.endpoint = popupRes.endpoint;
      switch (this.endpoint) {
        case '/users/user-card-view':
          if (this.recordObj.lastSearchExecuted) {
            this.getUserList();
          }
          break;
      }
    });
    this.getUserList();

    localStorage.setItem("return_path", "");
    this.dataservice.currentMessage.subscribe(message => this.message = message);
    if (this.message) {
      this.getUserList();
    }
  }
  getUserList() {
    this.recordObj.lastSearchExecuted = null;
    this.apiService.GET('User?IsLogged=true').subscribe(userResponse => {
      this.userList = userResponse.data;
      this.totalCount = userResponse.totalCount;
      this.tableReconstruct();
    }, (error) => {
      let errorMsg = this.errorHandling(error)
      this.alert.notifyErrorMessage(errorMsg);
    });
  }
  addCardView() {
    localStorage.setItem("return_path", "userCardView");
    this.router.navigate(["users/new-user"]);
  }
  updateCardView(userCard_id) {
    localStorage.setItem("return_path", "userCardView");
    this.router.navigate(["users/update-user/" + userCard_id]);
  }

  deleteUserCard(userCard_id) {
    this.confirmationDialogService.confirm('Please confirm..', 'Do you really want to delete... ?')
      .then((confirmed) => {
        if (confirmed) {
          if (userCard_id > 0) {
            this.apiService.DELETE('User/' + userCard_id).subscribe(orderResponse => {
              this.alert.notifySuccessMessage("Deleted Successfully!");
              this.getUserList();
            }, (error) => {
              let errorMsg = this.errorHandling(error)
              this.alert.notifyErrorMessage(errorMsg);
            });
          }
        }
      })
      .catch(() =>
        console.log('User dismissed the dialog (e.g., by using ESC, clicking the cross icon, or clicking outside the dialog)')
      );
  }

  public openuserCardViewSearchFilter(){
		if(true){
			$('#userCardViewSearch').on('shown.bs.modal', function () {
				$('#userCardView_Search_filter').focus();
			  }); 	
		}
	}

  public searchUserCard(searchValue) {
    this.recordObj.lastSearchExecuted = searchValue;
    this.apiService.GET(`User?GlobalFilter=${searchValue.value}`)
      .subscribe(searchResponse => {
        this.userList = searchResponse.data;

        if (searchResponse.data.length > 0) {
          this.alert.notifySuccessMessage(searchResponse.totalCount + " Records found");
          $('#userCardViewSearch').modal('hide');
          // $("#searchForm").trigger("reset");
          this.tableReconstruct();

        } else {
          this.userList = [];
          this.alert.notifyErrorMessage("No record found!");
          $('#userCardViewSearch').modal('hide');
          // $("#searchForm").trigger("reset");

          if ($.fn.DataTable.isDataTable(this.tableName))
            $(this.tableName).DataTable().destroy();
        }
      }, (error) => {
        let errorMsg = this.errorHandling(error)
        this.alert.notifyErrorMessage(errorMsg);
      });
  }

  private tableReconstruct() {
    if ($.fn.DataTable.isDataTable(this.tableName))
      $(this.tableName).DataTable().destroy();

    setTimeout(() => {
      this.dataTable = $(this.tableName).DataTable({
        order: [],
        columnDefs: [{
          orderable: false,
        }],

        dom: 'Blfrtip',
        buttons: [{
          extend: 'excel',
          attr: {
            title: 'export',
            id: 'export-data-table',
          }
        }],
        destroy: true
      });
    }, 10);
  }

  exportUserCard() {
    document.getElementById('export-data-table').click()
    // const edata: Array<any> = [];
    // const udt: any = {
    //   data: [
    //     { A: '', B: '', C: '', D: '', E: 'User Card View', F: '', G: '', H: '', I: '' },
    //     { A: '' },
    //     { A: 'Number', B: 'First Name', C: 'Surname', D: 'Address1', E: 'Address2', F: 'Address3', G: 'PostCode', H: 'Phone', I: 'Mobile' },
    //   ],
    //   skipHeader: true
    // };
    // this.userList.forEach(user => {
    //   udt.data.push({
    //     A: String(user.id),
    //     B: user.firstName,
    //     C: user.lastName,
    //     D: user.address1,
    //     E: user.address2,
    //     F: user.address3,
    //     G: user.postCode,
    //     H: user.phoneNo,
    //     I: user.mobileNo
    //   });
    // });
    // edata.push(udt);
    // this.exportService.exportJsonToExcel(edata, 'User Card View');
  }

  private errorHandling(error) {
    let err = error;
    if (error && error.error && error.error.message)
      err = error.error.message
    else if (error && error.message)
      err = error.message

    return err;
  }

}

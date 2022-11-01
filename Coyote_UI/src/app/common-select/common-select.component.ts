import { Component, OnInit, Input } from "@angular/core";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ApiService } from "../service/Api.service";

declare var $: any;
@Component({
  selector: "app-common-select",
  templateUrl: "./common-select.component.html",
  styleUrls: ["./common-select.component.scss"],
})
export class CommonSelectComponent implements OnInit {
  @Input() title: string;
  @Input() btnOkText: string;
  @Input() btnCancelText: string;
  @Input() label: string;
  @Input() data: any;
  @Input() dataDisplayfields: any;

  listData: any[];
  searchText: string;
  dataLength = 0;
  startval = 100;
  pageSizeValue = [100];
  currentPage = 1;
  pageLimit = 1000;

  constructor(
    private activeModal: NgbActiveModal,
    private apiService: ApiService
  ) {}

  ngOnInit(): void {
    this.getSelectList();
    $(".custom-mega-menu .dropdown-menu").on("click", function (event) {
      event.stopPropagation();
    });
  }

  public decline() {
    this.activeModal.close(false);
  }

  public accept(selectedVal) {
    this.activeModal.close(selectedVal);
  }

  public dismiss() {
    this.activeModal.dismiss(null);
  }
  getData(event) {
    console.log(event);
  }

  searchFun(val) {
    val = val.toLowerCase();
    if (this.data && this.data.length) {
      const newArray = this.data.filter((item) => {
        if (this.dataDisplayfields.length > 1) {
          return (
            item[this.dataDisplayfields[this.dataDisplayfields.length - 1]]
              .toLowerCase()
              .includes(val) ||
            item[this.dataDisplayfields[this.dataDisplayfields.length - 2]]
              .toLowerCase()
              .includes(val)
          );
        } else {
          return item[this.dataDisplayfields].toLowerCase().includes(val);
        }
      });
      this.listData = newArray;
    } else {
      this.listData = this.data;
    }
  }

  pageChangeEvent(event) {
    console.log(event, "===", this.listData.length);
    if (event === this.listData.length) {
      console.log(event);
    }
  }

  private getSelectList() {
    this.apiService.GET(`${this.data.url}`).subscribe((dataRes) => {
      this.listData = dataRes.data;
      this.dataLength = dataRes.totalCount;
    });
  }
}

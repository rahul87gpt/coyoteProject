import { Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";

import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { CommonSelectComponent } from "./common-select.component";

@Injectable()
export class CommonSelectService {
  constructor(private modalService: NgbModal) {}

  public openSelect(
    title: string,
    label: string,
    data: any,
    dataDisplayfields: any,
    windowClass: string,
    btnOkText: string = "Confirm",
    btnCancelText: string = "Cancel",
    dialogSize: "sm" | "lg" | "md" = "md"
  ): Promise<boolean> {
    const modalRef = this.modalService.open(CommonSelectComponent, {
      size: dialogSize,
      backdrop: "static",
      keyboard: false,
    });
    modalRef.componentInstance.title = title;
    modalRef.componentInstance.label = label;
    modalRef.componentInstance.data = data;
    modalRef.componentInstance.dataDisplayfields = dataDisplayfields;
    modalRef.componentInstance.btnOkText = btnOkText;
    modalRef.componentInstance.btnCancelText = btnCancelText;

    return modalRef.result;
  }
}

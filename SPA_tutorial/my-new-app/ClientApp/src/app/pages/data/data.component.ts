import { Component} from '@angular/core';
import { Service } from './data.service';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DxDataGridComponent, DxButtonModule, DxPopupModule, DxFormComponent, } from 'devextreme-angular';
import notify from 'devextreme/ui/notify';

@Component({
  templateUrl: 'data.component.html',
  styleUrls: ['./data.component.scss']
  , providers: [Service]
})

export class DataComponent {
  dataSource: any;
  notificationsPopup = false;

  popupItem = {
    Subject: "", Sender: "", Message: ""
    , Date: "", Status: "U", Id:0
  };

  constructor(private service: Service) {
    this.dataSource = this.service.getWeatherDatasource();
  }

  AddNotification() {
    this.notificationsPopup = true;
  }

  saveNotification() {
      var notiStore = this.service.postNotifications();
      notiStore.store.insert(this.popupItem);

      this.notificationsPopup = false;
  }

  cancelNotification() {
    this.notificationsPopup = false;
  }
}

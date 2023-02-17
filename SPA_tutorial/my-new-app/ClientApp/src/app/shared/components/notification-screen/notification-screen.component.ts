import { Component, NgModule, Input, Output, EventEmitter, ViewChild, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ScreenService, AuthService } from '../../services';
import { UserPanelModule } from '../user-panel/user-panel.component';
import { DxButtonComponent, DxButtonModule } from 'devextreme-angular/ui/button';
import { DxToolbarModule } from 'devextreme-angular/ui/toolbar';
import { DxDropDownButtonModule, DxPopupModule, DxTemplateModule, DxDataGridModule, DxTextAreaModule, DxDataGridComponent  } from 'devextreme-angular';
import { formatMessage, loadMessages, locale } from 'devextreme/localization';
import { Service } from './notification-screen.service';
import { BrowserModule } from '@angular/platform-browser';
import { formatDate } from "@angular/common";

declare var require: any;


@Component({
  selector: 'NotificationScreen',
  templateUrl: 'notification-screen.component.html',
  styleUrls: ['./notification-screen.component.scss'],
  providers: [Service]
})

export class NotificationScreenComponent {
  @ViewChild('notificationsgrid', { static: false }) dataGrid!: DxDataGridComponent;
  @ViewChild('notificationsgrid', { static: false }) button!: DxButtonComponent;
  @Output()
  menuToggle = new EventEmitter<boolean>();

  @Input()
  visible: boolean = false;

  ngOnInit() {

    this.service.hasUnreadMessage()
      .subscribe((res: any) => {
        var msgcount = res.data.length;
        this.hasNewMessage = msgcount > 0;
      });

  }

  @Output()
  public onHide = new EventEmitter<boolean>();

  popup_hiding(e : any) {
    this.onHide.emit(false);
  }

  onRowPrepared(e: any) {
    if (e.rowType === 'data') {
      switch (e.data.Status) {
        case "U":
          e.rowElement.bgColor = "#85B5ED";
          break;
        case "R":
          e.rowElement.bgColor = "#484848";
          break;
      }
    }
  }

  formatMessage = formatMessage;
  hasNewMessage = false;

  selectedNotification = {
      Subject: "",
      Message: "",
      Header: ""
  }

  getUserNotificationsDS: any;

  constructor(public authService: AuthService, private screen: ScreenService, private service: Service) {
    this.getUserNotificationsDS = service.getNotifications();
    this.onRowClick = this.onRowClick.bind(this);
  }

  onRowClick(e: any) {
    if (e.rowType === 'data') {
      this.service.hasUnreadMessage()
        .subscribe((res: any) => {
          var msgcount = res.data.length;
          this.hasNewMessage = msgcount > 0;
        });

      this.selectedNotification.Subject = e.data.Subject;
      this.selectedNotification.Message = e.data.Message;
      if (e.data.Status == "U") {
        this.service.updateNotifications(e.data.Id,"R").subscribe((response) => {

        });
      }

      this.dataGrid.instance.getDataSource().reload();
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if ((changes["visible"] && changes["visible"].currentValue)) {
      this.dataGrid.instance.getDataSource().reload();
    }
  }

  onInitNewRow(e:any) {
    e.data.Status = 'U';
  }
}

@NgModule({
  imports: [
    CommonModule,
    DxButtonModule,
    DxPopupModule,
    UserPanelModule,
    DxToolbarModule,
    DxDropDownButtonModule,
    DxDataGridModule,
    BrowserModule,
    DxTemplateModule,
    DxTextAreaModule,
  ],
  declarations: [NotificationScreenComponent ],
  exports: [NotificationScreenComponent ]
})
export class NotificationScreenModule { }

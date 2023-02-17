import { Component, NgModule, Input, Output, EventEmitter, enableProdMode, QueryList, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ScreenService, AuthService, IUser } from '../../services';
import { UserPanelModule } from '../user-panel/user-panel.component';
import { DxButtonComponent, DxButtonModule } from 'devextreme-angular/ui/button';
import { DxToolbarModule } from 'devextreme-angular/ui/toolbar';
import { DxDropDownButtonModule, DxPopupModule, DxTemplateModule, DxDataGridModule, DxTextAreaModule, DxSelectBoxModule } from 'devextreme-angular';
import { formatMessage, loadMessages, locale } from 'devextreme/localization';
import * as AspNetData from 'devextreme-aspnet-data-nojquery';
import { Service } from './header.service';
import { BrowserModule } from '@angular/platform-browser';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { NotificationScreenModule } from '../../../shared/components/notification-screen/notification-screen.component'

import { Router } from '@angular/router';
@Component({
  selector: 'app-header',
  templateUrl: 'header.component.html',
  styleUrls: ['./header.component.scss'],
  providers: [Service]
})

export class HeaderComponent {
  @Output()
  menuToggle = new EventEmitter<boolean>();

  @Input()
  menuToggleEnabled = false;

  @Input()
  title!: string;

  user: IUser | null = { email: '' };

  profileSettings: any;
  newMessage!: number;
  time: any;
  image!: any;
  AvatarLink!: string;


  //userMenuItems = [{
  //  text: 'Profile',
  //  icon: 'user',
  //  onClick: () => {
  //    this.router.navigate(['/profile']);
  //  }
  //},
  //  {
  //    text: 'Notification',
  //    icon: 'email',
  //    badge: 3,
  //    onClick: () => {
  //      this.notificationsPopup = true;
  //    }
  //  },
  //{
  //  text: 'Logout',
  //  icon: 'runner',
  //  onClick: () => {
  //    this.authService.logOut();
  //  }
  //  }];

  notificationsPopup = false;

  constructor(private authService: AuthService, private service: Service, private router: Router) {
    setInterval(() => {
      this.updateButton();
    }, 30000);

    setInterval(() => {
      this.time = new Date();
    }, 1000);
  }

  updateButton() {
    this.service.hasUnreadMessage()
      .subscribe((res: any) => {

        if (res.data.length > this.newMessage) {
          if (!('Notification' in window)) {
            console.log('Web Notification not supported');
            return;
          }

          Notification.requestPermission(function (permission) {
            var notification = new Notification("ELSW", {
              body: 'Egy új üzenete érkezett', icon: 'http://i.stack.imgur.com/Jzjhz.png?s=48&g=1',
              requireInteraction: true,
              dir: 'auto'
            });
          });
        }

        this.newMessage = res.data.length;

        this.profileSettings = [
          { name: 'Profile', icon: 'user' },
          { name: 'Notifications', icon: 'message', badge: `${this.newMessage}` },
          { name: 'Logout', icon: 'runner' },
        ];
      });
  }

  ngOnInit() {
    this.authService.getUser().then((e) => this.user = e.data);
    this.updateButton();
  }

  ngOnDestroy() {
    clearInterval(this.time);
  }

  toggleMenu = () => {
    this.menuToggle.emit();
  }


  onHiding(e: any) {
    this.service.hasUnreadMessage()
      .subscribe((res: any) => {
        this.newMessage = res.data.length;
        this.profileSettings = [
          { name: 'Profile', icon: 'user' },
          { name: 'Notifications', icon: 'message', badge: `${this.newMessage}` },
          { name: 'Logout', icon: 'runner' },
        ];
      });
    this.notificationsPopup = e;
  }

  onDropdownItemClick(e: any) {
    if (e.itemData.name == 'Logout') {
      this.authService.logOut();
    }
    else if (e.itemData.name == 'Notifications') {
      this.notificationsPopup = true;
    }
    else if (e.itemData.name == 'Profile') {
      this.router.navigate(['/profile']);
    }
  }

  toggle() {

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
    NotificationScreenModule,
    DxSelectBoxModule
  ],
  declarations: [HeaderComponent],
  exports: [HeaderComponent]
})
export class HeaderModule { }

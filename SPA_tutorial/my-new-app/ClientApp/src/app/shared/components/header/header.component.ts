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
  styleUrls: ['./header.component.scss']
})

export class HeaderComponent{
  @Output()
  menuToggle = new EventEmitter<boolean>();

  @Input()
  menuToggleEnabled = false;

  @Input()
  title!: string;

  user: IUser | null = { email: '' };

  userMenuItems = [{
    text: 'Profile',
    icon: 'user',
    onClick: () => {
      this.router.navigate(['/profile']);
    }
  },
    {
      text: 'Notification',
      icon: 'email',
      onClick: () => {
        this.notificationsPopup = true;
      }
    },
  {
    text: 'Logout',
    icon: 'runner',
    onClick: () => {
      this.authService.logOut();
    }
    }];

  notificationsPopup = false;

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit() {
    this.authService.getUser().then((e) => this.user = e.data);
  }

  toggleMenu = () => {
    this.menuToggle.emit();
  }

  onHiding(e: any) {
    this.notificationsPopup = e;
  }
}

@NgModule({
  imports: [
    CommonModule,
    DxButtonModule,
    UserPanelModule,
    DxToolbarModule,
    NotificationScreenModule
  ],
  declarations: [HeaderComponent],
  exports: [HeaderComponent]
})
export class HeaderModule { }

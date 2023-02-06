import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import * as AspNetData from 'devextreme-aspnet-data-nojquery';

const url = '/';

@Injectable()
export class Service {

  constructor(private http: HttpClient) {

  }

  updateNotifications(id: any, status: string) {
    {
      const headers = new HttpHeaders();
      const formData = new FormData();
      formData.append('values', JSON.stringify({
        id: id,
        status: status
      }));
      return this.http.put('/api/Notifications/UpdateNotifications', formData,
        {
          headers
        }
      )
    }
  }

  hasUnreadMessage(): any {
    return this.http.get('/api/Notifications/GetUnread');
  }
  getNotifications() {
    return {
      store: AspNetData.createStore({
        key: 'Id',
        loadUrl: url + 'api/Notifications/GetMessage',
        insertUrl: url + 'api/Notifications/Post',
        updateUrl: url + 'api/Notifications/Put',
        deleteUrl: url + 'api/Notifications/Delete',
        onBeforeSend(method, ajaxOptions) {
          ajaxOptions.xhrFields = { withCredentials: true };
        }
      })
    };
  }

}

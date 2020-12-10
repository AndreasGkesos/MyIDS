import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-signout-callback',
  templateUrl: './signout-callback.component.html',
  styleUrls: ['./signout-callback.component.css']
})
export class SignoutCallbackComponent implements OnInit {

  constructor() { }

  ngOnInit() {
    localStorage.removeItem('access_token');
    localStorage.removeItem('id_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('oidc.user:https://localhost:5001:angularclientCode');

    window.location.href = "/";
  }

}

import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/core/auth.service';

@Component({
  selector: 'app-signin-callback',
  templateUrl: './signin-callback.component.html',
  styleUrls: ['./signin-callback.component.css']
})
export class SigninCallbackComponent implements OnInit {

  constructor(public authService: AuthService) { }

  ngOnInit() {
    this.authService.signinCallback()
      .then(user => {
        this.authService.clear();
        // localStorage.removeItem('oidc.user:https://localhost:5001:angularclientimplicit');
        localStorage.setItem('access_token', user.access_token);
        localStorage.setItem('id_token', user.id_token);
        console.log("user: ", user);
        window.location.href = "/";
      })
  }

}

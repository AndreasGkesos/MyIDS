import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { User } from 'oidc-client';
import { ApiCallService } from './core/api-call.service';
import { AuthService } from './core/auth.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  
  constructor(public authService: AuthService, private fb: FormBuilder, private apicall: ApiCallService) {
  }

  messages: string[] = [];
  get currentUserJson(): string {
    return JSON.stringify(this.currentUser, null, 2);
  }
  currentUser: User;
  
  peopleForm: FormGroup;
  speciesForm: FormGroup;
  starshipForm: FormGroup;

  peopleName: string;
  speciesName: string;
  starshipsName: string;

  errorMessage: string;
  
  ngOnInit(): void {
    this.peopleForm = this.fb.group({
      id: ['', Validators.required]
    });

    this.speciesForm = this.fb.group({
      id: ['', Validators.required]
    });

    this.starshipForm = this.fb.group({
      id: ['', Validators.required]
    });

    // this.authService.getUser().then(user => {
    //   this.currentUser = user;

    //   if (user) {
    //     this.addMessage('User Logged In');
    //   } else {
    //     this.addMessage('User Not Logged In');
    //   }
    // }).catch(err => this.addError(err));

    if (localStorage.getItem('access_token')) {
      this.addMessage('User Logged In');
    } else {
      this.addMessage('User Not Logged In');
    }
  }

  clearMessages() {
    while (this.messages.length) {
      this.messages.pop();
    }
  }
  addMessage(msg: string) {
    this.messages.push(msg);
  }
  addError(msg: string | any) {
    this.messages.push('Error: ' + msg && msg.message);
  }

  public onLogin() {
    this.clearMessages();
    this.authService.login().catch(err => {
      this.addError(err);
    });
  }

  public onRenewToken() {
    this.clearMessages();
    this.authService.renewToken()
      .then(user => {
        this.currentUser = user;
        this.addMessage('Silent Renew Success');
      })
      .catch(err => this.addError(err));
  }

  public onLogout() {
    this.clearMessages();
    this.authService.logout().catch(err => this.addError(err));
  }

  public getStarwarsPeople() {
    const id = this.peopleForm.get('id').value;
    
    this.apicall.getStarwarsPeople(id)
      .subscribe(res => {
        this.peopleName = res.name;
        this.errorMessage = '';
        console.log(res);
      }, err => {
        this.errorMessage = `Error status: ${err.status}`;
        console.log('error', err)
      });
  }

  public getStarwarsSpecies() {
    const id = this.speciesForm.get('id').value;
    
    this.apicall.getStarwarsSpecies(id)
      .subscribe(res => {
        this.speciesName = res.name;
        this.errorMessage = '';
        console.log(res);
      }, err => {
        this.errorMessage = `Error status: ${err.status}`;
        console.log('error', err)
      });
  }

  public getStarwarsStarships() {
    const id = this.starshipForm.get('id').value;
    
    this.apicall.getStarwarsStarships(id)
      .subscribe(res => {
        this.starshipsName = res.name;
        this.errorMessage = '';
        console.log(res);
      }, err => {
        this.errorMessage = `Error status: ${err.status}`;
        console.log('error', err)
      });
  }
}

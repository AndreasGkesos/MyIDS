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
  
  itemForm: FormGroup;
  locationForm: FormGroup;
  pokemonForm: FormGroup;

  itemName: string;
  locationName: string;
  pokemonName: string;

  errorMessage: string;
  
  ngOnInit(): void {
    this.itemForm = this.fb.group({
      id: ['', Validators.required]
    });

    this.locationForm = this.fb.group({
      id: ['', Validators.required]
    });

    this.pokemonForm = this.fb.group({
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

  public getItem() {
    const id = this.itemForm.get('id').value;
    
    this.apicall.getItem(id)
      .subscribe(res => {
        this.itemName = res.name;
        this.errorMessage = '';
        console.log(res);
      }, err => {
        this.errorMessage = `Error status: ${err.status}`;
        console.log('error', err)
      });
  }

  public getLocation() {
    const id = this.locationForm.get('id').value;
    
    this.apicall.getLocation(id)
      .subscribe(res => {
        this.locationName = res.name;
        this.errorMessage = '';
        console.log(res);
      }, err => {
        this.errorMessage = `Error status: ${err.status}`;
        console.log('error', err)
      });
  }

  public getPokemon() {
    const id = this.pokemonForm.get('id').value;
    
    this.apicall.getPokemon(id)
      .subscribe(res => {
        this.pokemonName = res.name;
        this.errorMessage = '';
        console.log(res);
      }, err => {
        this.errorMessage = `Error status: ${err.status}`;
        console.log('error', err)
      });
  }
}

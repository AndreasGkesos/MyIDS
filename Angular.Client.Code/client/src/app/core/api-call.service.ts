import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ApiCallService {

  starwarsBase = 'https://localhost:5201/api';

  constructor(private httpClient: HttpClient) { }

  public getItem(id) {
    return this.httpClient.get(`${this.starwarsBase}/pokemon/items/${id}`)
          .pipe(map((res: any) => res))
  }

  public getLocation(id) {
    return this.httpClient.get(`${this.starwarsBase}/pokemon/location/${id}`)
          .pipe(map((res: any) => res))
  }

  public getPokemon(id) {
    return this.httpClient.get(`${this.starwarsBase}/pokemon/pokemon/${id}`)
          .pipe(map((res: any) => res))
  }
}

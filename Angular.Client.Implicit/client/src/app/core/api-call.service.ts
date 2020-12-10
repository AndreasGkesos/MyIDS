import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ApiCallService {

  starwarsBase = 'https://localhost:5101/api';

  constructor(private httpClient: HttpClient) { }

  public getStarwarsPeople(id) {
    return this.httpClient.get(`${this.starwarsBase}/starwars/people/${id}`)
          .pipe(map((res: any) => res))
  }

  public getStarwarsSpecies(id) {
    return this.httpClient.get(`${this.starwarsBase}/starwars/species/${id}`)
          .pipe(map((res: any) => res))
  }

  public getStarwarsStarships(id) {
    return this.httpClient.get(`${this.starwarsBase}/starwars/starships/${id}`)
          .pipe(map((res: any) => res))
  }
}

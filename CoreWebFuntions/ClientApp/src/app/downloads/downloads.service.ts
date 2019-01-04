import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
//import { map } from 'rxjs/operators';
import {
  DownloadsResource
} from '@app/core/models';
//import { buildQueryParams } from '@app/lib';

@Injectable({
  providedIn: 'root'
})
export class DownloadsService {
  private downloadsEndpoint = '/api/downloads/';

  constructor(@Inject(HttpClient) private http: HttpClient) { }

  getFiles(): Observable<DownloadsResource[]> {
    return this.http.get<DownloadsResource[]>(
      this.downloadsEndpoint + 'list'
    );
  }
}

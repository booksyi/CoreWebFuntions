import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { buildQueryParams } from '@app/lib';

export class InsertScriptQuery {
  tableName: string;
  startRowNumber: number;
  endRowNumber: number;
  containsIdentity: boolean;
}

export class SqlQuery {
  commandText: string;
}

@Injectable({
  providedIn: 'root'
})
export class QueriesService {
  private queriesEndpoint = '/api/queries/';

  constructor(@Inject(HttpClient) private http: HttpClient) { }

  getInsertScript(query: InsertScriptQuery): Observable<{ result: string }> {
    return this.http.get<{ result: string }>(
      this.queriesEndpoint + 'insertScript/' + query.tableName + buildQueryParams(query)
    );
  }

  getSqlSelect(query: SqlQuery): Observable<{ [key: string]: any; }[]> {
    return this.http.get<{ [key: string]: any; }[]>(
      this.queriesEndpoint + 'query' + buildQueryParams(query)
    );
  }
}

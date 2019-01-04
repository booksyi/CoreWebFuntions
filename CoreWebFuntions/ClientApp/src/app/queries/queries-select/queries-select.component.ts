import { QueriesService, SqlQuery } from '@app/queries/queries.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-queries-select',
  templateUrl: './queries-select.component.html',
  //styleUrls: ['./downloads-list.component.scss']
})
export class QueriesSelectComponent implements OnInit {
  constructor(
    private queriesService: QueriesService
  ) { }

  ngOnInit() {
  }

  sqlCommand: SqlQuery = {
    commandText: null
  };

  sqlResult: any[];

  getSqlResult() {
    return this.queriesService
      .getSqlSelect(this.sqlCommand)
      .subscribe(res => { this.sqlResult = res; });
  }
}

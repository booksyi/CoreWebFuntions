import { QueriesService, InsertScriptQuery } from '@app/queries/queries.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-queries-insert-script',
  templateUrl: './queries-insert-script.component.html',
  //styleUrls: ['./downloads-list.component.scss']
})
export class QueriesInsertScriptComponent implements OnInit {
  constructor(
    private queriesService: QueriesService
  ) { }

  ngOnInit() {
  }

  insertScriptQuery: InsertScriptQuery = {
    tableName: null,
    startRowNumber: 1,
    endRowNumber: 5000,
    containsIdentity: false
  };

  insertScript: string;

  getInsertScript() {
    return this.queriesService
      .getInsertScript(this.insertScriptQuery)
      .subscribe(res => { this.insertScript = res.result; });
  }
}

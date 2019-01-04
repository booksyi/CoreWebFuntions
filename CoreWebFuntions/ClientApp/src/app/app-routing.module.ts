import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { DownloadsListComponent } from './downloads/downloads-list/downloads-list.component';
import { QueriesInsertScriptComponent } from './queries/queries-insert-script/queries-insert-script.component';
import { QueriesSelectComponent } from './queries/queries-select/queries-select.component';

const routes: Routes = [
  { path: 'downloads', component: DownloadsListComponent },
  { path: 'queries-insert-script', component: QueriesInsertScriptComponent },
  { path: 'queries-select', component: QueriesSelectComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

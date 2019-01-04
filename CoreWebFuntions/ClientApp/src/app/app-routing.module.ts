import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DownloadsListComponent } from './downloads/downloads-list/downloads-list.component';

const routes: Routes = [
  { path: 'downloads', component: DownloadsListComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

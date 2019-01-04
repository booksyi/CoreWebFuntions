import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DownloadsListComponent } from './downloads/downloads-list/downloads-list.component';
import { QueriesInsertScriptComponent } from './queries/queries-insert-script/queries-insert-script.component';
import { QueriesSelectComponent } from './queries/queries-select/queries-select.component';

@NgModule({
  declarations: [
    AppComponent,
    DownloadsListComponent,
    QueriesInsertScriptComponent,
    QueriesSelectComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

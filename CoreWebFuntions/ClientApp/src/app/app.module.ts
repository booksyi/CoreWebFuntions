import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DownloadsListComponent } from './downloads/downloads-list/downloads-list.component';

@NgModule({
  declarations: [
    AppComponent,
    DownloadsListComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent, DownloadsListComponent]
})
export class AppModule { }

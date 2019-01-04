import { DownloadsResource } from '@app/core/models/downloads.resource';
import { DownloadsService } from '@app/downloads/downloads.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-downloads-list',
  templateUrl: './downloads-list.component.html',
  styleUrls: ['./downloads-list.component.scss']
})
export class DownloadsListComponent implements OnInit {
  constructor(
    private downloadsService: DownloadsService
  ) { }

  ngOnInit() {
    this.getFiles();
  }

  resources: DownloadsResource[];

  getFiles() {
    return this.downloadsService.getFiles().subscribe(res => this.resources = res);
  }
}

import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { VideoStreamComponent } from './components/video-stream.component';

@NgModule({
  declarations: [
    AppComponent,
    VideoStreamComponent
  ],
  imports: [
    BrowserModule, HttpClientModule
  ],
  providers: [],
  bootstrap: [VideoStreamComponent]
})
export class AppModule { }

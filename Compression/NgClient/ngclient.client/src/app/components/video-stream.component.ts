import { Component } from "@angular/core";
import { WebSocketService } from "../services/websocket.service";

@Component({
    selector: "app-video-stream",
    templateUrl: "./video-stream.component.html",
    styleUrls: ["./video-stream.component.scss"]
  })
  export class VideoStreamComponent {
    image$ = this.webSocketService.image$;
  
    constructor(private webSocketService: WebSocketService) {}
  
    startStream() {
      this.webSocketService.startStream();
    }
  
    stopStream() {
      this.webSocketService.stopStream();
    }

    updateCanvas(event: Event) {
        const img = event.target as HTMLImageElement;
        const canvas = document.querySelector("canvas") as HTMLCanvasElement;
        const ctx = canvas.getContext("2d");
        if (ctx) {
          ctx.clearRect(0, 0, canvas.width, canvas.height);
          ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
        }
      }     
  }
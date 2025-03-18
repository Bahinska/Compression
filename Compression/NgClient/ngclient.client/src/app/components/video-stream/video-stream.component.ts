import { Component, OnInit } from "@angular/core";
import { WebSocketService } from "src/app/services/websocket.service";

@Component({
    selector: "app-video-stream",
    templateUrl: "./video-stream.component.html",
    styleUrls: ["./video-stream.component.scss"]
  })
  export class VideoStreamComponent implements OnInit {
    image$ = this.webSocketService.image$;
    receivedImages: string[] = [];
  
    constructor(private webSocketService: WebSocketService) {}

    ngOnInit(): void {
      this.webSocketService.photoSubject.subscribe(blob => {
        this.displayImage(blob);
      });
    }
  
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
    
      private displayImage(blob: Blob) {
        const url = URL.createObjectURL(blob);
        this.receivedImages.push(url);
    
        const img = new Image();
        img.onload = () => {
          URL.revokeObjectURL(url);
        };
        img.src = url;
      }
  }
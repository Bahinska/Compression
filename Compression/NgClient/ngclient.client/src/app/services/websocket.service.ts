import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class WebSocketService {
  private videoSocket!: WebSocket;
  private photoSocket!: WebSocket;
  private imageSubject = new BehaviorSubject<string | null>(null);
  public photoSubject = new Subject<Blob>();
  image$ = this.imageSubject.asObservable();
  serverAddress = "wss://localhost:7246/ws/server";

  constructor(private http: HttpClient) {
    this.startPhotoSocket();
  }

  startPhotoSocket() {
    this.photoSocket = new WebSocket(this.serverAddress);

    this.photoSocket.onmessage = (event) => {
          const blob = new Blob([event.data], { type: 'image/png' });
          this.photoSubject.next(blob);
    };
  }

  async getJwtToken(): Promise<string> {
    const storedToken = localStorage.getItem('jwtToken');
    if (storedToken) return storedToken;
  
    const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
  
    const body = new HttpParams()
      .set('username', 'test')
      .set('password', 'password'); 
  
    try {
      const response: any = await this.http.post('https://localhost:7246/api/account/token', body, { headers }).toPromise();
      localStorage.setItem('jwtToken', response.token);
      return response.token;
    } catch (error) {
      console.error("JWT Token Fetch Error:", error);
      throw error;
    }
  }

  async startStream() {
    this.photoSocket.close();
    const token = await this.getJwtToken();
    const clientAddress = "wss://localhost:4201/ws/client";

    this.videoSocket = new WebSocket(clientAddress);
    this.videoSocket.binaryType = "arraybuffer";
    
    const headers = { 
        'Authorization': `Bearer ${token}`, 
        'Content-Type': 'application/json' 
    };
    const body = JSON.stringify(clientAddress);
    
    const response = await this.http.post(
        'https://localhost:5001/api/control/start', 
        body,
        { headers, responseType: 'text' }
    ).toPromise();
    
    
    this.videoSocket.onmessage = (event) => {
      if (event.data instanceof ArrayBuffer) {
        const imageData = new Uint8Array(event.data);
        const blob = new Blob([imageData], { type: 'image/png' });
        const url = URL.createObjectURL(blob);
        this.imageSubject.next(url);
      }
    };
    
    this.videoSocket.onclose = () => {
      this.imageSubject.next(null);
    };
  }

  async stopStream() {
    if (this.videoSocket) {
      this.videoSocket.close();
    }
    this.startPhotoSocket();
  }
}
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class WebSocketService {
  private socket!: WebSocket;
  private imageSubject = new BehaviorSubject<string | null>(null);
  image$ = this.imageSubject.asObservable();

  constructor(private http: HttpClient) {}

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
    const token = await this.getJwtToken();
    const clientAddress = "wss://localhost:4201/ws/client";

    this.socket = new WebSocket(clientAddress);
    this.socket.binaryType = "arraybuffer";
    
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
    
    
    this.socket.onmessage = (event) => {
      if (event.data instanceof ArrayBuffer) {
        const imageData = new Uint8Array(event.data);
        const blob = new Blob([imageData], { type: 'image/png' });
        const url = URL.createObjectURL(blob);
        this.imageSubject.next(url);
      }
    };
    
    this.socket.onclose = () => {
      this.imageSubject.next(null);
    };
  }

  stopStream() {
    if (this.socket) {
      this.socket.close();
    }
    return this.http.post('https://localhost:4201/api/websocket/stop', {}).toPromise();
  }
}
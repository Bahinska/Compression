import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  exp: number;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  

  constructor(private http: HttpClient) { }

  getJwtTokenFromLocalStorage(): string | null {
    const storedToken = localStorage.getItem('jwtToken');
    return storedToken;
  }

  isJwtTokenExpired(token: string): boolean {
    try {
      const decoded = jwtDecode<JwtPayload>(token);
      const currentTime = Math.floor(Date.now() / 1000);
      return decoded.exp < currentTime;
    } catch (error) {
      console.error("Invalid token:", error);
      localStorage.removeItem('token');
      return true;
    }
  }

  isAuthenticated(): boolean {
    const token = this.getJwtTokenFromLocalStorage();
    if (token) {
      return !this.isJwtTokenExpired(token);
    } else {
      return false;
    }
  }

  async getJwtToken(username: string, password: string): Promise<string> {
    // const storedToken = localStorage.getItem('jwtToken');
    // if (storedToken) return storedToken;
  
    const headers = new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' });
  
    const body = new HttpParams()
      .set('username', username)
      .set('password', password); 
  
    try {
      const response: any = await this.http.post('https://localhost:7246/api/account/token', body, { headers }).toPromise();
      //localStorage.setItem('jwtToken', response.token);
      return response.token;
    } catch (error) {
      console.error("JWT Token Fetch Error:", error);
      throw error;
    }
  }
}

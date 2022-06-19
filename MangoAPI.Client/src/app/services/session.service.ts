import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginCommand } from '../types/requests/LoginCommand';
import { ITokensResponse } from '../types/responses/ITokensResponse';
import { IBaseResponse } from '../types/responses/IBaseResponse';
@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private sessionsRoute = 'api/sessions/';

  constructor(private httpClient: HttpClient) {}

  // POST /api/sessions
  createSession(command: LoginCommand): Observable<ITokensResponse> {
    return this.httpClient.post<ITokensResponse>(
      environment.baseUrl + this.sessionsRoute,
      command,
      { withCredentials: true }
    );
  }

  // POST /api/sessions/{refreshToken}
  refreshSession(refreshToken: string | null): Observable<ITokensResponse> {
    return this.httpClient.post<ITokensResponse>(
      environment.baseUrl + this.sessionsRoute + refreshToken,
      {}
    );
  }

  // DELETE /api/sessions/{refreshToken}
  deleteSession(refreshToken: string | null): Observable<IBaseResponse> {
    return this.httpClient.delete<IBaseResponse>(
      environment.baseUrl + this.sessionsRoute + refreshToken
    );
  }

  // DELETE /api/sessions
  deleteAllSessions(): Observable<IBaseResponse> {
    return this.httpClient.delete<IBaseResponse>(
      environment.baseUrl + this.sessionsRoute
    );
  }
}

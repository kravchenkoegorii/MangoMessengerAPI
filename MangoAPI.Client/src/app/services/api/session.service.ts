import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginCommand } from '../../types/requests/LoginCommand';
import { TokensResponse } from '../../types/responses/TokensResponse';
import { BaseResponse } from '../../types/responses/BaseResponse';
@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private sessionsRoute = 'api/sessions/';

  constructor(private httpClient: HttpClient) {}

  // POST /api/sessions
  createSession(command: LoginCommand): Observable<TokensResponse> {
    return this.httpClient.post<TokensResponse>(environment.baseUrl + this.sessionsRoute, command, {
      withCredentials: true
    });
  }

  // POST /api/sessions/{refreshToken}
  refreshSession(refreshToken: string | null): Observable<TokensResponse> {
    return this.httpClient.post<TokensResponse>(
      environment.baseUrl + this.sessionsRoute + refreshToken,
      {}
    );
  }

  // DELETE /api/sessions/{refreshToken}
  deleteSession(refreshToken: string | null): Observable<BaseResponse> {
    return this.httpClient.delete<BaseResponse>(
      environment.baseUrl + this.sessionsRoute + refreshToken
    );
  }

  // DELETE /api/sessions
  deleteAllSessions(): Observable<BaseResponse> {
    return this.httpClient.delete<BaseResponse>(environment.baseUrl + this.sessionsRoute);
  }
}

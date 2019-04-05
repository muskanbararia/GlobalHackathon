import { Injectable } from '@angular/core';
import { HttpClient,HttpErrorResponse  } from '@angular/common/http';
import { IUser } from '../app/Interface/User';
import { catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) { }
  SignUp(user:IUser): Observable<string>{
    
    return this.http.post<string>('https://localhost:44377/api/washer/signup', user).pipe(catchError(this.errorHandler));
  }
  login(email:string,password:string):Observable<string>{
    var str='?email='+email+'&password='+password
    return this.http.get<string>('https://localhost:44377/api/washer/login'+str).pipe(catchError(this.errorHandler));
  }
  userInfo(userId:string):Observable<IUser>{
    var str='?userId='+userId
    return this.http.get<IUser>('https://localhost:44377/api/washer/UserInfo'+str).pipe(catchError(this.errorHandler));
  }

  errorHandler(error: HttpErrorResponse) {
    console.error(error);
    return throwError(error.message || "Server Error");
  } 
}

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { routing } from './app.routing';
import { HomeNavComponent } from './home-nav/home-nav.component';
import { FooterComponent } from './footer/footer.component';
import { DemoComponent } from './demo/demo.component';
import { RegisterComponent } from './register/register.component';
import {FormsModule} from '@angular/forms';
import { LoginComponent } from './login/login.component';
import { HttpClientModule } from '@angular/common/http';
import { UserDashComponent } from './user-dash/user-dash.component';
import { UserNavComponent } from './user-nav/user-nav.component';
import { EditProfileComponent } from './edit-profile/edit-profile.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    HomeNavComponent,
    FooterComponent,
    DemoComponent,
    RegisterComponent,
    LoginComponent,
    UserDashComponent,
    UserNavComponent,
    EditProfileComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    routing,
    FormsModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

import { RouterModule, Routes } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';
import {HomeComponent } from './home/home.component';
import { DemoComponent } from './demo/demo.component';
import { RegisterComponent } from './register/register.component';
import { UserDashComponent } from './user-dash/user-dash.component';
import { EditProfileComponent } from './edit-profile/edit-profile.component';

const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'how-it-works', component: DemoComponent},
    { path: 'register', component:RegisterComponent},
    { path: 'home', component:UserDashComponent},
    { path: 'editprofile', component:EditProfileComponent}
];

export const routing: ModuleWithProviders = RouterModule.forRoot(routes);
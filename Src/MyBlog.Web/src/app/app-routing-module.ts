import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [{ path: 'users', loadChildren: () => import('./features/users/users-module').then(m => m.UsersModule) }, { path: 'blogs', loadChildren: () => import('./features/blogs/blogs-module').then(m => m.BlogsModule) }];

@NgModule({
    imports: [RouterModule.forRoot(routes), BrowserModule],
    exports: [RouterModule],
})
export class AppRoutingModule {}

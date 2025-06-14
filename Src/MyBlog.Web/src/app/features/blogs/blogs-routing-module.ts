import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Blogs } from './blogs';

const routes: Routes = [{ path: '', component: Blogs }];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class BlogsRoutingModule {}

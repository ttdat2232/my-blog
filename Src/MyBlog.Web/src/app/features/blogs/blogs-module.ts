import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { BlogService } from './blog-service';
import { Blogs } from './blogs';
import { BlogsRoutingModule } from './blogs-routing-module';

@NgModule({
    declarations: [Blogs],
    imports: [CommonModule, BlogsRoutingModule],
    providers: [BlogService],
})
export class BlogsModule {}

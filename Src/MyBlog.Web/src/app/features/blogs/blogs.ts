import { Component, OnInit } from '@angular/core';
import { BlogModel } from './blog-model';
import { BlogService } from './blog-service';

@Component({
    selector: 'app-blogs',
    standalone: false,
    templateUrl: './blogs.html',
    styleUrl: './blogs.scss',
})
export class Blogs implements OnInit {
    blogs: BlogModel[] = [];

    constructor(private readonly blogService: BlogService) {}

    ngOnInit(): void {
        this.getBlogs();
    }

    getBlogs() {
        console.log('fetching blogs...');
        this.blogService.getBlogs().subscribe((data) => {
            if (data) {
                this.blogs = data;
            }
        });
    }
}

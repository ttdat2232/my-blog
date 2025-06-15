import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BlogModel } from './blog-model';

@Injectable({
    providedIn: 'root',
})
export class BlogService {
    constructor(private readonly http: HttpClient) {}

    getBlogs(): Observable<BlogModel[]> {
        return this.http.get<BlogModel[]>(environment.apiUrl);
    }

    getBlog(id: string): Observable<BlogModel> {
        return this.http.get<BlogModel>(`${environment.apiUrl}/${id}`);
    }

    createBlog(blog: BlogModel): Observable<BlogModel> {
        return this.http.post<BlogModel>(environment.apiUrl, blog);
    }

    updateBlog(blog: BlogModel): Observable<BlogModel> {
        return this.http.put<BlogModel>(
            `${environment.apiUrl}/${blog.id}`,
            blog
        );
    }

    deleteBlog(id: string): Observable<BlogModel> {
        return this.http.delete<BlogModel>(`${environment.apiUrl}/${id}`);
    }

    likeBlog(id: string): Observable<BlogModel> {
        return this.http.post<BlogModel>(
            `${environment.apiUrl}/${id}/like`,
            {}
        );
    }

    getComments(blogId: string): Observable<BlogModel[]> {
        return this.http.get<BlogModel[]>(
            `${environment.apiUrl}/${blogId}/comments`
        );
    }

    addComment(blogId: string, comment: BlogModel): Observable<BlogModel> {
        return this.http.post<BlogModel>(
            `${environment.apiUrl}/${blogId}/comments`,
            comment
        );
    }
}

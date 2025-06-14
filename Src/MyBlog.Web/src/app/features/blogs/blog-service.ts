import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BlogModel } from './blog-model';

@Injectable({
    providedIn: 'root',
})
export class BlogService {
    private readonly apiUrl = '/api/blogs';

    constructor(private readonly http: HttpClient) {}

    getBlogs(): Observable<BlogModel[]> {
        return this.http.get<BlogModel[]>(this.apiUrl);
    }

    getBlog(id: string): Observable<BlogModel> {
        return this.http.get<BlogModel>(`${this.apiUrl}/${id}`);
    }

    createBlog(blog: BlogModel): Observable<BlogModel> {
        return this.http.post<BlogModel>(this.apiUrl, blog);
    }

    updateBlog(blog: BlogModel): Observable<BlogModel> {
        return this.http.put<BlogModel>(`${this.apiUrl}/${blog.id}`, blog);
    }

    deleteBlog(id: string): Observable<BlogModel> {
        return this.http.delete<BlogModel>(`${this.apiUrl}/${id}`);
    }

    likeBlog(id: string): Observable<BlogModel> {
        return this.http.post<BlogModel>(`${this.apiUrl}/${id}/like`, {});
    }

    getComments(blogId: string): Observable<BlogModel[]> {
        return this.http.get<BlogModel[]>(`${this.apiUrl}/${blogId}/comments`);
    }

    addComment(blogId: string, comment: BlogModel): Observable<BlogModel> {
        return this.http.post<BlogModel>(
            `${this.apiUrl}/${blogId}/comments`,
            comment
        );
    }
}

import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { WebSocketSubject } from 'rxjs/webSocket';

@Injectable({
    providedIn: 'root',
})
export class NotificationService {
    private readonly socket$: WebSocketSubject<any>;

    constructor() {
        this.socket$ = new WebSocketSubject('ws://websocket-url');
    }
    getNotifications<T>() {
        return this.socket$.asObservable().pipe(map((data) => data as T));
    }

    sendMessage(message: any) {
        this.socket$.next(message);
    }
}

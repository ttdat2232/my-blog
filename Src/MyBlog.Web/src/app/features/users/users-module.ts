import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { UserService } from './user-service';
import { Users } from './users';
import { UsersRoutingModule } from './users-routing-module';

@NgModule({
    declarations: [Users],
    imports: [CommonModule, UsersRoutingModule],
    providers: [UserService],
})
export class UsersModule {}

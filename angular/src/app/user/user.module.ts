import { NgModule } from "@angular/core";
import { CommonModule, NgClass, NgSwitch } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { UserRoutingModule } from "./user.routing.module";
import { SignUpComponent } from "./component/sign-up/sign-up.component";
import { UpdateUserComponent } from "./component/update-user/update-user.component";
import { PersonalAreaComponent } from "./component/personal-area/personal-area.component";

@NgModule({
    declarations: [SignUpComponent, UpdateUserComponent, PersonalAreaComponent],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        NgClass,
        FormsModule,
        NgSwitch,
        UserRoutingModule,

    ]
})
export class UserModule { }
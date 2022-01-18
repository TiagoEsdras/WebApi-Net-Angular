import { Component, Input, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FormValidations } from '../form-validations';

@Component({
  selector: 'app-erro-message',
  templateUrl: './erro-message.component.html',
  styleUrls: ['./erro-message.component.scss']
})
export class ErroMessageComponent implements OnInit {

  @Input() control: FormControl;
  @Input() label: string;

  constructor() { }

  ngOnInit(): void {
  }

  get errorMessage(): any {
    for (const propertyName in this.control.errors) {
      if (this.control.errors.hasOwnProperty(propertyName) && this.control.touched) {
        return FormValidations.getErrorMassage(this.label, propertyName, this.control.errors[propertyName]);
      }
    }
    return null;
  }
}

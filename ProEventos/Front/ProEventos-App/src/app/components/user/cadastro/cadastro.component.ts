import { Component, OnInit } from '@angular/core';
import {
  AbstractControlOptions,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { ValidatorField } from '@app/helpers/ValidatorField';
import { User } from '@app/models/Identity/User';
import { AccountService } from '@app/services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-cadastro',
  templateUrl: './cadastro.component.html',
  styleUrls: ['./cadastro.component.scss'],
})
export class CadastroComponent implements OnInit {
  user = {} as User;
  form: FormGroup;

  get f(): any {
    return this.form.controls;
  }

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.validacoes();
  }

  public validacoes(): void {
    const formOptions: AbstractControlOptions = {
      validators: ValidatorField.MustMatch('password', 'confirmarPassword'),
    };

    this.form = this.fb.group(
      {
        primeiroNome: ['', [Validators.required, Validators.minLength(3)]],
        ultimoNome: ['', [Validators.required, Validators.minLength(3)]],
        email: ['', [Validators.required, Validators.email]],
        userName: ['', Validators.required],
        password: ['', [Validators.required, Validators.minLength(8)]],
        confirmarPassword: ['', Validators.required],
        termo: [null, Validators.pattern('true')],
      },
      formOptions
    );
  }

  public validateCss(campo: FormControl): any {
    return { 'is-invalid': campo.errors && campo.touched };
  }

  public register(): void {
    this.user = { ...this.form.value };
    this.accountService.register(this.user).subscribe(
      () => this.router.navigateByUrl('/user/login'),
      (err: any) => this.toastr.error(err.error)
    );
  }
}

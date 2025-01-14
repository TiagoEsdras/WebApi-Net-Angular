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
import { UserUpdate } from '@app/models/Identity/UserUpdate';
import { AccountService } from '@app/services/account.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.scss'],
})
export class PerfilComponent implements OnInit {
  userUpdate: UserUpdate;
  form: FormGroup;

  constructor(
    public accountService: AccountService,
    private fb: FormBuilder,
    private router: Router,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService
  ) {}

  ngOnInit(): void {
    this.validacoes();
    this.carregarUsuario();
  }

  private carregarUsuario(): void {
    this.spinner.show();
    this.accountService
      .getUser()
      .subscribe(
        (userRetorno: UserUpdate) => {
          this.userUpdate = userRetorno;
          this.form.patchValue(this.userUpdate);
        },
        (error) => {
          console.error(error);
          this.toastr.error('Usuário não Carregado', 'Erro');
          this.router.navigate(['/dashboard']);
        }
      )
      .add(this.spinner.hide());
  }

  get f(): any {
    return this.form.controls;
  }

  private validacoes(): void {
    const formOptions: AbstractControlOptions = {
      validators: ValidatorField.MustMatch('password', 'confirmePassword'),
    };

    this.form = this.fb.group(
      {
        userName: [''],
        imagemURL: [''],
        titulo: ['', Validators.required],
        primeiroNome: ['', Validators.required],
        ultimoNome: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        phoneNumber: ['', [Validators.required]],
        descricao: ['', Validators.required],
        funcao: ['', Validators.required],
        password: ['', [Validators.minLength(8)]],
        confirmePassword: [''],
      },
      formOptions
    );
  }

  public resetForm(event: any): void {
    event.preventDefault();
    this.form.reset();
  }

  public onSubmit(): void {
    this.atualizarUsuario();
  }

  public validateCss(campo: FormControl): any {
    return { 'is-invalid': campo.errors && campo.touched };
  }

  private atualizarUsuario() {
    this.userUpdate = { ...this.form.value };
    this.spinner.show();

    this.accountService
      .updateUser(this.userUpdate)
      .subscribe(
        () => this.toastr.success('Usuário atualizado!', 'Sucesso'),
        (error) => {
          console.error(error);
          this.toastr.error(error.error);
        }
      )
      .add(() => this.spinner.hide());
  }
}

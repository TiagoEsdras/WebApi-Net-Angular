import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.scss']
})
export class PerfilComponent implements OnInit {

  form: FormGroup;

  get f(): any {
    return this.form.controls;
  }

  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.validacoes();
  }

  public validacoes(): void {
    this.form = this.fb.group({
      titulo: ['', Validators.required],
      nome: ['', [Validators.required, Validators.minLength(3)]],
      sobrenome: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      telefone: ['', Validators.required],
      funcao: ['', Validators.required],
      descricao: ['', [Validators.required, Validators.minLength(30), Validators.maxLength(150)]]
    });
  }

  public resetForm(): void {
    this.form.reset();
  }

  public validateCss(campo: FormControl): any {
    return {'is-invalid': campo.errors && campo.touched};
  }
}

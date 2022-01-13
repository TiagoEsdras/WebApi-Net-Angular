import { Component, OnInit } from '@angular/core';
import { Form, FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-evento-detalhe',
  templateUrl: './evento-detalhe.component.html',
  styleUrls: ['./evento-detalhe.component.scss']
})
export class EventoDetalheComponent implements OnInit {

  form: FormGroup = new FormGroup({});
  constructor() { }

  ngOnInit() {
  }

  public validacoes(): void {
    this.form = new FormGroup({
      local: new FormControl(),
      dataEvento: new FormControl(),
      tema: new FormControl(),
      qtdPessoas: new FormControl(),
      imagemURL: new FormControl(),
      telefone: new FormControl(),
      email: new FormControl(),
    });
  }

}

import { Component, OnInit } from '@angular/core';
import { Form, FormControl, FormGroup, Validators } from '@angular/forms';

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
      tema: new FormControl('',
        [Validators.required, Validators.minLength(4), Validators.maxLength(50)]
      ),
      local: new FormControl('', Validators.required),
      dataEvento: new FormControl('', Validators.required),
      qtdPessoas: new FormControl('',
        [Validators.required, Validators.max(120000)]
      ),
      telefone: new FormControl('', Validators.required),
      email: new FormControl('',
        [Validators.required, Validators.email]
      ),
      imagemURL: new FormControl('', Validators.required),
    });
  }

}

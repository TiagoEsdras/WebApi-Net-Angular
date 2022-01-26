import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { EventoService } from '@app/services/evento.service';
import { Evento } from '@app/models/Evento';
import { Lote } from '@app/models/Lote';

@Component({
  selector: 'app-evento-detalhe',
  templateUrl: './evento-detalhe.component.html',
  styleUrls: ['./evento-detalhe.component.scss']
})
export class EventoDetalheComponent implements OnInit {

  evento: Evento;
  form: FormGroup;
  estadoSalvar: string = 'post';

  get lotes(): FormArray {
    return this.form.get('lotes') as FormArray;
  }

  get f(): any {
    return this.form.controls;
  }

  get bsConfig(): any {
    return {
      adaptivePosition: true,
      dateInputFormat: 'DD/MM/YYYY hh:mm a',
      containerClass: 'theme-default',
      showWeekNumbers: false
    };
  }

  constructor(
    private fb: FormBuilder,
    private localeService: BsLocaleService,
    private router: ActivatedRoute,
    private eventoService: EventoService,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService
    ) {
      this.localeService.use('pt-br');
    }

  ngOnInit(): void {
    this.validacoes();
    this.carregarEvento();
  }

  public validacoes(): void {
    this.form = this.fb.group({
      tema: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]],
      local: ['', Validators.required],
      dataEvento: ['', Validators.required],
      qtdPessoas: ['', [Validators.required, Validators.max(120000)]],
      telefone: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      imageURL: ['', Validators.required],
      lotes: this.fb.array([])
    });
  }

  adicionarLote(): void {
    this.lotes.push(this.criarLote({ id: 0 } as Lote));
  }

  criarLote(lote: Lote): FormGroup {
    return this.fb.group({
      id: [lote.id],
      nome: [lote.nome, Validators.required],
      preco: [lote.preco, Validators.required],
      dataInicio: [lote.dataInicio],
      dataFim: [lote.dataFim],
      quantidade: [lote.quantidade, Validators.required]
    });
  }

  public resetForm(): void {
    this.form.reset();
  }

  public validateCss(campo: FormControl): any {
    return {'is-invalid': campo.errors && campo.touched};
  }

  public carregarEvento(): void{
    const eventoIdParam = this.router.snapshot.paramMap.get('id');

    if(eventoIdParam !== null) {
      this.estadoSalvar = "put";
      this.spinner.show();
      this.eventoService.getEventoById(+eventoIdParam).subscribe({
        next: (evento: Evento) => {
          this.evento = {...evento};
          this.form.patchValue(this.evento);
        },
        error: (error: any) => {
          this.toastr.error('Erro ao tentar carregar o evento!', 'Error')
          console.error(error);
        }
      })
      .add(() => this.spinner.hide());
    }
  }

  public salvarAlteracao(): void {
    this.spinner.show();
    if(this.form.valid) {

      this.evento = this.estadoSalvar === 'post' ?  {...this.form.value} : {id: this.evento.id, ...this.form.value}

      this.eventoService[this.estadoSalvar](this.evento).subscribe({
        next: (evento: Evento) => {
          this.toastr.success('Evento salvo com sucesso!', 'Sucesso');
        },
        error: (error: any) => {
          console.error(error);
          this.toastr.error('Erro ao tentar salvar evento!', 'Error');
        }
      })
      .add(() => this.spinner.hide());
    }
  }
}

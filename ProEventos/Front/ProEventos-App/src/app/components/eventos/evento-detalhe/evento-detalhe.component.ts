import { Component, OnInit, TemplateRef } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { EventoService } from '@app/services/evento.service';
import { LoteService } from '@app/services/lote.service';
import { Evento } from '@app/models/Evento';
import { Lote } from '@app/models/Lote';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-evento-detalhe',
  templateUrl: './evento-detalhe.component.html',
  styleUrls: ['./evento-detalhe.component.scss']
})
export class EventoDetalheComponent implements OnInit {

  modalRef: BsModalRef;
  eventoId: number;
  evento: Evento;
  form: FormGroup;
  estadoSalvar: string = 'post';
  loteAtual = {id: 0, nome: '', index: 0};

  get modoEditar(): boolean {
    return this.estadoSalvar === 'put';
  }

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
    private activatedRouter: ActivatedRoute,
    private eventoService: EventoService,
    private loteService: LoteService,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService,
    private router: Router,
    private modalService: BsModalService
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

  public mudarValorData(value: Date, index: number, campo: string): void {
    this.lotes.value[index][campo] = value;
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

  public validateCss(campo: FormControl | AbstractControl): any {
    return {'is-invalid': campo.errors && campo.touched};
  }

  public carregarEvento(): void{
    this.eventoId = +this.activatedRouter.snapshot.paramMap.get('id');

    if(this.eventoId !== null || this.eventoId === 0) {
      this.estadoSalvar = "put";
      this.spinner.show();
      this.eventoService.getEventoById(this.eventoId).subscribe({
        next: (evento: Evento) => {
          this.evento = {...evento};
          this.form.patchValue(this.evento);
          this.evento.lotes.forEach(lote => {
            this.lotes.push(this.criarLote(lote));
          });
        },
        error: (error: any) => {
          this.toastr.error('Erro ao tentar carregar o evento!', 'Error')
          console.error(error);
        }
      })
      .add(() => this.spinner.hide());
    }
  }

  public salvarEvento(): void {
    this.spinner.show();
    if(this.form.valid) {

      this.evento = this.estadoSalvar === 'post' ?  {...this.form.value} : {id: this.evento.id, ...this.form.value}

      this.eventoService[this.estadoSalvar](this.evento).subscribe({
        next: (eventoRetorno: Evento) => {
          this.toastr.success('Evento salvo com sucesso!', 'Sucesso');
          this.router.navigate([`eventos/detalhe/${eventoRetorno.id}`]);
        },
        error: (error: any) => {
          console.error(error);
          this.toastr.error('Erro ao tentar salvar evento!', 'Error');
        }
      })
      .add(() => this.spinner.hide());
    }
  }

  public salvarLotes(): void {
    this.spinner.show();
    if(this.form.controls.lotes.valid) {
      this.loteService.saveLotes(this.eventoId, this.form.value.lotes).subscribe({
        next: () => {
          this.toastr.success('Lotes salvos com sucesso!', 'Sucesso');
        },
        error: (error: any) => {
          console.error(error);
          this.toastr.error('Erro ao tentar salvar lotes!', 'Error');
        }
      })
      .add(() => this.spinner.hide());
    }
  }

  public openModal(template: TemplateRef<any> , index: number): void {
    this.modalRef = this.modalService.show(template, {class: 'modal-sm'});
    this.loteAtual.index = index;
    this.loteAtual.id = this.lotes.value[index].id;
    this.loteAtual.nome = this.lotes.value[index].nome;
  }

  confirm(): void {
    this.modalRef?.hide();
    this.spinner.show();

      this.loteService.deleteLote(this.eventoId, this.loteAtual.id).subscribe({
        next: () => {
          this.toastr.success('Lote deletado com sucesso!', 'Sucesso');
          this.lotes.removeAt(this.loteAtual.index)
        },
        error: (error: any) => {
          console.error(error);
          this.toastr.error('Erro ao tentar deletar lotes', 'Error');
        }
      })
      .add(() => this.spinner.hide());
  }

  decline(): void {
    this.modalRef?.hide();
  }
}

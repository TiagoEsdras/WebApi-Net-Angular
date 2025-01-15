import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { ToastrService } from 'ngx-toastr';
import { NgxSpinnerService } from 'ngx-spinner';

import { LoteService } from './../../../services/lote.service';
import { EventoService } from '@app/services/evento.service';
import { Evento } from '@app/models/Evento';
import { Lote } from '@app/models/Lote';
import { DatePipe } from '@angular/common';
import { environment } from '@environments/environment';

@Component({
  selector: 'app-evento-detalhe',
  templateUrl: './evento-detalhe.component.html',
  styleUrls: ['./evento-detalhe.component.scss'],
  providers: [DatePipe],
})

export class EventoDetalheComponent implements OnInit {

  modalRef: BsModalRef;
  eventoId: number;
  evento = {} as Evento;
  form: FormGroup;
  estadoSalvar: string = 'post';
  loteAtual = {id: 0, nome: '', index: 0};
  imagemUrl = 'assets/img/upload.png';
  file: File;

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
    private modalService: BsModalService,
    private datePipe: DatePipe
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
      imageURL: [''],
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

    if(this.eventoId !== null && this.eventoId !== 0) {
      this.estadoSalvar = "put";
      this.spinner.show();
      this.eventoService.getEventoById(this.eventoId).subscribe({
        next: (evento: Evento) => {
          this.evento = {...evento};
          this.form.patchValue(this.evento);

          if(this.evento.imageURL !== "") {
            this.imagemUrl = environment.apiURL + 'resources/images/' + this.evento.imageURL;
          }

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
    if(this.form.valid) {
      this.spinner.show();
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

  public retornaTituloLote(value: string): string {
    return value === null || value === '' ? 'Nome do lote' : value;
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
          this.toastr.error('Erro ao tentar deletar lotes!', 'Error');
        }
      })
      .add(() => this.spinner.hide());
  }

  decline(): void {
    this.modalRef?.hide();
  }

  public onFileChange(event: any): void {
    const reader = new FileReader();
    reader.onload = (ev: any) => this.imagemUrl = ev.target.result;
    this.file = event.target.files;
    reader.readAsDataURL(this.file[0]);
    this.uploadImage();
  }

  public uploadImage(): void {
    this.spinner.show();
    this.eventoService.postUpload(this.eventoId, this.file).subscribe({
      next: () => {
        this.toastr.success('Imagem foi atualizada com sucesso!', 'Sucesso');
        this.carregarEvento();
      },
      error: (error: any) => {
        console.error(error);
        this.toastr.error('Erro ao tentar fazer upoload da imagem!', 'Error');
      }
    })
    .add(() => this.spinner.hide());
  }
}

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { EventoService } from '@app/services/evento.service';
import { Evento } from '@app/models/Evento';

@Component({
  selector: 'app-evento-detalhe',
  templateUrl: './evento-detalhe.component.html',
  styleUrls: ['./evento-detalhe.component.scss']
})
export class EventoDetalheComponent implements OnInit {

  evento: Evento;
  form: FormGroup;

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
      this.spinner.show();
      this.eventoService.getEventoById(+eventoIdParam).subscribe({
        next: (evento: Evento) => {
          this.evento = {...evento};
          this.form.patchValue(this.evento);
        },
        error: (error: any) => {
          this.spinner.hide();
          this.toastr.error('Erro ao tentar carregar o evento!', 'Error')
          console.error(error);
        },
        complete: () => {
          this.spinner.hide();
        }
      });
    }
  }

  public salvarAlteracao(): void {
    this.spinner.show();
    if(this.form.valid) {
      this.evento = {...this.form.value};
      this.eventoService.postEvento(this.evento).subscribe({
        next: (evento: Evento) => {
          this.toastr.success('Evento salvo com sucesso!', 'Sucesso');
          this.spinner.hide();
        },
        error: (error: any) => {
          console.error(error);
          this.spinner.hide();
          this.toastr.error('Erro ao tentar salvar evento!', 'Error');
        },
        complete: () => {
          this.spinner.hide();
        }
      })
    }

  }
}

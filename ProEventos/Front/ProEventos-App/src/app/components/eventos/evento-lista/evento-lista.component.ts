import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { Evento } from '@app/models/Evento';
import { EventoService } from '@app/services/evento.service';
import { environment } from '@environments/environment';
import { PaginatedResult, Pagination } from '@app/models/Pagination';
import { Subject } from 'rxjs';
import { DatePipe } from '@angular/common';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-evento-lista',
  templateUrl: './evento-lista.component.html',
  styleUrls: ['./evento-lista.component.scss'],
  providers: [DatePipe],
})
export class EventoListaComponent implements OnInit {
  modalRef?: BsModalRef;
  public eventos: Evento[] = [];
  public larguraImg = 150;
  public margemImg = 2;
  public exibirImg = false;
  public eventoId: number;
  public pagination: Pagination;
  public termoBuscaChanged: Subject<string> = new Subject<string>();

  public filtrarEventos(evt: any): void {
    if (this.termoBuscaChanged.observers.length === 0) {
      this.termoBuscaChanged
        .pipe(debounceTime(1000))
        .subscribe((filtrarPor) => {
          this.spinner.show();
          this.eventoService
            .getEventos(
              this.pagination.currentPage,
              this.pagination.itemsPerPage,
              filtrarPor
            )
            .subscribe(
              (paginatedResult: PaginatedResult<Evento[]>) => {
                this.eventos = paginatedResult.result;
                this.pagination = paginatedResult.pagination;
              },
              (error: any) => {
                this.spinner.hide();
                this.toastr.error('Erro ao Carregar os Eventos', 'Erro!');
              }
            )
            .add(() => this.spinner.hide());
        });
    }
    this.termoBuscaChanged.next(evt.value);
  }

  constructor(
    private eventoService: EventoService,
    private modalService: BsModalService,
    private toastr: ToastrService,
    private spinner: NgxSpinnerService,
    private router: Router
  ) {}

  public ngOnInit(): void {
    this.pagination = {
      currentPage: 1,
      itemsPerPage: 3,
      totalItems: 1,
    } as Pagination;

    this.getEventos();
  }

  public exibirEsconderImg(): void {
    this.exibirImg = !this.exibirImg;
  }

  public mostrarImagem(imagemUrl: string): string {
    return imagemUrl !== ''
      ? environment.apiURL + 'resources/images/' + imagemUrl
      : 'assets/img/semImagem.jpeg';
  }

  public getEventos(): void {
    this.spinner.show();
    this.eventoService
      .getEventos(this.pagination.currentPage, this.pagination.itemsPerPage)
      .subscribe({
        next: (page: PaginatedResult<Evento[]>) => {
          this.eventos = page.result;
          this.pagination = page.pagination;
        },
        error: (error: any) => {
          this.toastr.error('Erro ao carregar os eventos', 'Error!');
        },
      })
      .add(() => this.spinner.hide());
  }

  public pageChanged(event): void {
    this.pagination.currentPage = event.page;
    this.getEventos();
  }

  detalheEvento = (eventoId: number) => {
    this.router.navigate([`eventos/detalhe/${eventoId}`]);
  };

  openModal(event: any, template: TemplateRef<any>, eventoId: number): void {
    event.stopPropagation();
    this.eventoId = eventoId;
    this.modalRef = this.modalService.show(template, { class: 'modal-sm' });
  }

  confirm(): void {
    this.modalRef?.hide();
    this.spinner.show();
    this.eventoService
      .deleteEvento(this.eventoId)
      .subscribe({
        next: (result: boolean) => {
          if (result)
            this.toastr.success(
              'O evento foi deletado com sucesso.',
              'Deletado!'
            );
          this.getEventos();
        },
        error: (error: any) => {
          this.toastr.error(
            `Erro ao tentar deletar o evento ${this.eventoId}`,
            'Error!'
          );
          console.error(error);
        },
      })
      .add(() => this.spinner.hide());
  }

  decline(): void {
    this.modalRef?.hide();
  }
}

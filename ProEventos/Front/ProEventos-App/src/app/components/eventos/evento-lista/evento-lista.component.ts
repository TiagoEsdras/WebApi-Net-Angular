import { Component, OnInit, TemplateRef } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { Evento } from '@app/models/Evento';
import { EventoService } from '@app/services/evento.service';
import { environment } from '@environments/environment';
import { PaginatedResult, Pagination } from '@app/models/Pagination';

@Component({
  selector: 'app-evento-lista',
  templateUrl: './evento-lista.component.html',
  styleUrls: ['./evento-lista.component.scss'],
})
export class EventoListaComponent implements OnInit {
  modalRef?: BsModalRef;
  public eventos: Evento[] = [];
  public eventosFiltrados: Evento[] = [];
  public larguraImg = 150;
  public margemImg = 2;
  public exibirImg = false;
  public eventoId: number;
  private filtroListado = '';
  public pagination: Pagination;

  public get filtroLista(): string {
    return this.filtroListado;
  }

  public set filtroLista(value: string) {
    this.filtroListado = value;
    this.eventosFiltrados = this.filtroLista
      ? this.filtrarEventos(this.filtroLista)
      : this.eventos;
  }

  public filtrarEventos(filtrarPor: string): Evento[] {
    filtrarPor = filtrarPor.toLocaleLowerCase();
    return this.eventos.filter(
      (evento: any) =>
        evento.tema.toLocaleLowerCase().indexOf(filtrarPor) !== -1 ||
        evento.local.toLocaleLowerCase().indexOf(filtrarPor) !== -1
    );
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
          this.eventosFiltrados = this.eventos;
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

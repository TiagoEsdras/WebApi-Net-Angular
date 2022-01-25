using System.Threading.Tasks;
using ProEventos.Domain;

namespace ProEventos.Persistence.Contratos
{
    public interface ILotePersist
    {
        Task<Lote[]> GetLotesByEventoId(int eventoId);
        Task<Lote> GetLoteByIdsAsync(int eventoId, int loteId);
    }
}
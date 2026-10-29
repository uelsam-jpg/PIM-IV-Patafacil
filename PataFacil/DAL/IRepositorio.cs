using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PataFacil.DAL
{
    public interface IRepositorio<T> where T : class
    {
        Task<IEnumerable<T>> ObterTodosAsync();
        // Consulta filtrada: o filtro vira WHERE no banco (aproveita os índices em vez de carregar a tabela inteira)
        Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> filtro);
        Task<T?> ObterPorIdAsync(int id);
        Task<int> AdicionarAsync(T entidade);
        Task AtualizarAsync(T entidade);
        Task RemoverAsync(int id);
    }
}

using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IGenericServices<T>  where T: class
    {
        T SelectById(object id);
        ResultAPI Select(string cmd);
        ResultAPI Select(int skip, int take, FiltroAPI filtros);
        Task<ResultAPI> SelectAsync(int skip, int take, FiltroAPI filtros);
        long Insert(T obj);
        long Insert(List<T> obj);
        bool Update(T obj);
        long InsertOrUpdate(T obj);
        bool Delete(T obj);
        void Delete(string cmd);
    }
}

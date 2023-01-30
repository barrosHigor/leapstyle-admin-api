using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class GenericServices<T> : IGenericServices<T> where T : class
    {
        private readonly IGenericServices<T> _IGenericRepository;
        public GenericServices(IGenericServices<T> IGenericRepository) => _IGenericRepository = IGenericRepository;

        public bool Delete(T obj) => _IGenericRepository.Delete(obj);
        public void Delete(string cmd) => _IGenericRepository.Delete(cmd);
        public long Insert(T obj) => _IGenericRepository.Insert(obj);
        public long Insert(List<T> objs) => _IGenericRepository.Insert(objs);
        public long InsertOrUpdate(T obj) => _IGenericRepository.InsertOrUpdate(obj);
        public ResultAPI Select(int skip, int take, FiltroAPI filtros) => _IGenericRepository.Select(skip, take, filtros);
        public async Task<ResultAPI> SelectAsync(int skip, int take, FiltroAPI filtros) => await _IGenericRepository.SelectAsync(skip, take, filtros);
        public ResultAPI Select(string cmd) => _IGenericRepository.Select(cmd);
        public T SelectById(object id) => _IGenericRepository.SelectById(id);
        public bool Update(T obj) => _IGenericRepository.Update(obj);
    }
}

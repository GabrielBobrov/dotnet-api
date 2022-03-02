using System.Collections.Generic;
using System.Threading.Tasks;
using Manager.Domain.Entities;


namespace Manager.Infra.Interface {
    
    public interface IBaseRepository<T> where T : Base
    {
        Task<T> CreateAsync (T obj);
        Task<T> UpdateAsync (T obj);
        Task<T> RemoveAsync (long id);
        Task<T> GetAsync (long id);
        Task<List<T>> GetAsync ();


    }
}
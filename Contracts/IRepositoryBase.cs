using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<IList<T>> FindALL();
        Task<T> FindById(int id);
        Task<bool> Create(T endity);
        Task<bool> isExists(int id);
        Task<bool> Update(T endity);
        Task<bool> Delete(T endity);
        Task<bool> Save();
    }
}

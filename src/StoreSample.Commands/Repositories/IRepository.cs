using StoreSample.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSample.Commands.Repositories
{
    public interface IRepository<T>
        where T : class, IEventSourced
    {
        Task<T> Get(Guid id);

        Task Save(T entity);
    }
}

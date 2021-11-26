using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Interfaces
{
    public interface IRegistrationWaitingSetDao<TEntity> where TEntity : class
    {
        void Create(params TEntity[] obj);

        void Delete(params TEntity[] obj);

        void EraseRegistrationWaitingSetsTable();

        IList<RegistrationWaitingSet> GetAllRegistrationWaitingSets();
    }
}

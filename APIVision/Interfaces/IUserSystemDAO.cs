using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Interfaces
{
    public interface IUserSystemDao<TEntity> where TEntity : class
    {
        void Create(params TEntity[] obj);

        void Delete(params TEntity[] obj);

        IList<UserSystem> GetAllUserSystems();
    }
}

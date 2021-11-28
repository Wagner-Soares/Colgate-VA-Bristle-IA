using System;
using System.Collections.Generic;
using System.Text;
using Database;

namespace APIVision.Interfaces
{
    public interface IModelsDao<TEntity> where TEntity : class
    {
        void Create(params TEntity[] obj);

        IList<Database.Models> GetAllModelss();
    }
}

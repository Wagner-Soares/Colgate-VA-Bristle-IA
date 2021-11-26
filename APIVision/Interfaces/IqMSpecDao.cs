using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Interfaces
{
    public interface IQMSpecDao<TEntity> where TEntity : class
    {
        void Create(params TEntity[] obj);

        IList<QM_Spec> GetAllQM_Specs();
    }
}

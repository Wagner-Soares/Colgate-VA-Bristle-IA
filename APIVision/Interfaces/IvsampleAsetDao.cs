using APIVision.DAO;
using System;
using System.Collections.Generic;
using System.Text;
using Database;

namespace APIVision.Interfaces
{
    public interface IVsampleAsetDao<TEntity> where TEntity : class
    {
        void Create(params TEntity[] obj);

        IList<Vsample_ASet> GetAllVsample_ASets();
    }
}

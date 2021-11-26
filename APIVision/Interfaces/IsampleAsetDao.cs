using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Interfaces
{
    public interface ISampleAsetDao<TEntity> where TEntity : class
    {
        void Create(params TEntity[] obj);

        IList<Sample_ASet> GetAllSample_ASets();
    }
}

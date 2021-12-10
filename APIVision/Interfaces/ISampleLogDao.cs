using Database;
using System.Collections.Generic;

namespace APIVision.Interfaces
{
    public interface ISampleLogDao<TEntity> where TEntity : class
    {
        void Create(params TEntity[] obj);

        IList<Sample_log> GetAllSample_logs();
    }
}

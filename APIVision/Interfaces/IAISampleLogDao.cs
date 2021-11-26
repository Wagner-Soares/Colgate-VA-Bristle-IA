using Database;
using System.Collections.Generic;

namespace APIVision.Interfaces
{
    public interface IAISampleLogDao<TEntity> where TEntity : class
    {
        void Create(params TEntity[] obj);

        IList<AI_Sample_log> GetAllAI_Sample_logs();
    }
}

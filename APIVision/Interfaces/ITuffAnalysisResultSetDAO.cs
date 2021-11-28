using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Interfaces
{
    public interface ITuffAnalysisResultSetDao<TEntity> where TEntity : class
    {
        void Create(params TEntity[] obj);
        

        IList<TuffAnalysisResultSet> GetAllTuffAnalysisResultSets();
    }
}

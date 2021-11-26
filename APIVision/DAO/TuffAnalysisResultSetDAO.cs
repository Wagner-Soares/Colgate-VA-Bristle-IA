using APIVision.Interfaces;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIVision.DAO
{
    public class TuffAnalysisResultSetDao<TEntity> : ITuffAnalysisResultSetDao<TEntity> where TEntity : class
    {
        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        public TuffAnalysisResultSetDao(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            this._colgateSkeltaEntities = colgateSkeltaEntities;
        }

        public void Create(params TEntity[] obj)
        {
            _colgateSkeltaEntities.Set<TEntity>().AddRange(obj);
            _colgateSkeltaEntities.SaveChanges();
        }

        public IList<TuffAnalysisResultSet> GetAllTuffAnalysisResultSets()
        {
            return _colgateSkeltaEntities.TuffAnalysisResultSet.ToList();
        }
    }
}

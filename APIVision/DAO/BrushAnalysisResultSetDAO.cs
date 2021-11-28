using APIVision.Interfaces;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIVision.DAO
{
    public class BrushAnalysisResultSetDao<TEntity> : IBrushAnalysisResultSetDao<TEntity> where TEntity : class
    {
        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        public BrushAnalysisResultSetDao(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            this._colgateSkeltaEntities = colgateSkeltaEntities;
        }

        public void Create(params TEntity[] obj)
        {
            _colgateSkeltaEntities.Set<TEntity>().AddRange(obj);
            _colgateSkeltaEntities.SaveChanges();
        }

        public IList<BrushAnalysisResultSet> GetAllBrushAnalysisResultSets()
        {
            return _colgateSkeltaEntities.BrushAnalysisResultSet.ToList();
        }

    }
}

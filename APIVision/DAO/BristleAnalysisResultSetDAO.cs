using APIVision.Interfaces;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIVision.DAO
{
    public class BristleAnalysisResultSetDao<TEntity> : IBristleAnalysisResultSetDao<TEntity> where TEntity : class
    {
        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        public BristleAnalysisResultSetDao(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            this._colgateSkeltaEntities = colgateSkeltaEntities;
        }

        public void Create(params TEntity[] obj)
        {
            _colgateSkeltaEntities.Set<TEntity>().AddRange(obj);
            _colgateSkeltaEntities.SaveChanges();
        }

        public IList<BristleAnalysisResultSet> GetAllBristleAnalysisResultSets()
        {
            return _colgateSkeltaEntities.BristleAnalysisResultSet.ToList();
        }
    }
}

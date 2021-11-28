using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APIVision.Interfaces;
using APIVision.DataModels;
using Database;

namespace APIVision.DAO
{
    public class AnalyzeSetDao<TEntity> : IanalyzeSetDao<TEntity> where TEntity : class
    {
        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        public AnalyzeSetDao(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            this._colgateSkeltaEntities = colgateSkeltaEntities;
        }

        public void Create(params TEntity[] obj)
        {
            _colgateSkeltaEntities.Set<TEntity>().AddRange(obj);
            _colgateSkeltaEntities.SaveChanges();
        }

        public IList<AnalyzeSet> GetAllAnalyzeSets()
        {
            return _colgateSkeltaEntities.AnalyzeSet.ToList();
        }
    }
}

using APIVision.Interfaces;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIVision.DAO
{
    public class QmSpecDao<TEntity> : IQMSpecDao<TEntity> where TEntity : class
    {
        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        public QmSpecDao(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            this._colgateSkeltaEntities = colgateSkeltaEntities;
        }

        public void Create(params TEntity[] obj)
        {
            _colgateSkeltaEntities.Set<TEntity>().AddRange(obj);
            _colgateSkeltaEntities.SaveChanges();
        }

        public IList<QM_Spec> GetAllQM_Specs()
        {
            return _colgateSkeltaEntities.QM_Spec.ToList();
        }
    }
}

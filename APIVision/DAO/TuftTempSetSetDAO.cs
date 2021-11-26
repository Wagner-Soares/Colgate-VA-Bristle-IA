using APIVision.Interfaces;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIVision.DAO
{
    public class TuftTempSetSetDao<TEntity> : ITuftTempSetSetDao<TEntity> where TEntity : class
    {
        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        public TuftTempSetSetDao(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            this._colgateSkeltaEntities = colgateSkeltaEntities;
        }

        public void Create(params TEntity[] obj)
        {
            _colgateSkeltaEntities.Set<TEntity>().AddRange(obj);
            _colgateSkeltaEntities.SaveChanges();
        }

        public void Delete(params TEntity[] obj)
        {
            _colgateSkeltaEntities.Set<TEntity>().RemoveRange(obj);
            _colgateSkeltaEntities.SaveChanges();
        }

        public void EraseTuftTempSetSetTable()
        {
            _colgateSkeltaEntities.Set<TEntity>().RemoveRange(_colgateSkeltaEntities.Set<TEntity>().ToList());
            _colgateSkeltaEntities.SaveChanges();
        }

        public IList<TuftTempSetSet> GetAllTuftTempSetSets()
        {
            return _colgateSkeltaEntities.TuftTempSetSet.ToList();
        }
    }
}

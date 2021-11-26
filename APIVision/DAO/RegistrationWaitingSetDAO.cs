using APIVision.Interfaces;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIVision.DAO
{
    public class RegistrationWaitingSetDao<TEntity> : IRegistrationWaitingSetDao<TEntity> where TEntity : class
    {
        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        public RegistrationWaitingSetDao(ColgateSkeltaEntities colgateSkeltaEntities)
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

        public void EraseRegistrationWaitingSetsTable()
        {
            _colgateSkeltaEntities.Set<TEntity>().RemoveRange(_colgateSkeltaEntities.Set<TEntity>().ToList());
            _colgateSkeltaEntities.SaveChanges();
        }

        public IList<RegistrationWaitingSet> GetAllRegistrationWaitingSets()
        {
            return _colgateSkeltaEntities.RegistrationWaitingSet.ToList();
        }
    }
}

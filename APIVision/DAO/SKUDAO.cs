using APIVision.Interfaces;
using APIVision.DataModels;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIVision.DAO
{
    public class SkuDao<TEntity> : ISkuDao<TEntity> where TEntity : class
    {
        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        public SkuDao(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            this._colgateSkeltaEntities = colgateSkeltaEntities;
        }


        public IList<SKU> GetAllSKUs()
        {
            return _colgateSkeltaEntities.SKUs.ToList();
        }
    }
}

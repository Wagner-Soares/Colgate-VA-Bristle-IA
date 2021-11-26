using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Interfaces
{
    public interface ISkuDao<TEntity> where TEntity : class
    {
        IList<SKU> GetAllSKUs();

    }
}

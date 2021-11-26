using APIVision.DataModels;
using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Interfaces
{
    public interface IBristleTempSetSetDao<TEntity> where TEntity : class
    {
        void Create(params TEntity[] obj);

        void Delete(params TEntity[] obj);

        void EraseBristleTempSetSetsTable();

        List<BristleTempModel> ListBristleTempModel();

        IList<BristleTempSetSet> GetAllBristleTempSetSets();

    }
}

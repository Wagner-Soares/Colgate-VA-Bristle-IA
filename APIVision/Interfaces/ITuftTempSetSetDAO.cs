using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Interfaces
{
    public interface ITuftTempSetSetDao<TEntity> where TEntity : class
    {
        void Create(params TEntity[] obj);

        void Delete(params TEntity[] obj);

        void EraseTuftTempSetSetTable();

        IList<TuftTempSetSet> GetAllTuftTempSetSets();
    }
}

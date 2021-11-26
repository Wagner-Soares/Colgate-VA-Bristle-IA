using APIVision.DataModels;
using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Interfaces
{
    public interface IImageTempSetSetDao<TEntity> where TEntity : class
    {
        void Create(params TEntity[] obj);

        void Delete(params TEntity[] obj);

        List<ImageTempModel> ListImageTempModel();

        void EraseImageTempSetSetsTable();

        IList<ImageTempSetSet> GetAllImageTempSetSets();
    }
}

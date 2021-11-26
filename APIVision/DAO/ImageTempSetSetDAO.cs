
using APIVision.DataModels;
using APIVision.Interfaces;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIVision.DAO
{
    public class ImageTempSetSetDao<TEntity> : IImageTempSetSetDao<TEntity> where TEntity : class
    {
        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        public ImageTempSetSetDao(ColgateSkeltaEntities colgateSkeltaEntities)
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

        public List<ImageTempModel> ListImageTempModel()
        {
            try
            {
                var query = (from n in _colgateSkeltaEntities.ImageTempSetSet
                                select new ImageTempModel
                                {
                                    Id = n.Id,
                                    Path = n.Path,
                                    TuftSet = n.TuftTempSetSet
                                }).ToList();

                List<ImageTempModel> result = new List<ImageTempModel>(query);

                return result;                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<ImageTempModel>();
            }
        }

        public void EraseImageTempSetSetsTable()
        {
            _colgateSkeltaEntities.Set<TEntity>().RemoveRange(_colgateSkeltaEntities.Set<TEntity>().ToList());
            _colgateSkeltaEntities.SaveChanges();
        }

        public IList<ImageTempSetSet> GetAllImageTempSetSets()
        {
            return _colgateSkeltaEntities.ImageTempSetSet.ToList();
        }
    }
}

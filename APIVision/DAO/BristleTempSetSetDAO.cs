using APIVision.DataModels;
using APIVision.Interfaces;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIVision.DAO
{
    public class BristleTempSetSetDao<TEntity> : IBristleTempSetSetDao<TEntity> where TEntity : class
    {
        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        public BristleTempSetSetDao(ColgateSkeltaEntities colgateSkeltaEntities)
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

        public void EraseBristleTempSetSetsTable()
        {
            _colgateSkeltaEntities.Set<TEntity>().RemoveRange(_colgateSkeltaEntities.Set<TEntity>().ToList());
            _colgateSkeltaEntities.SaveChanges();
        }

        public List<BristleTempModel> ListBristleTempModel()
        {
            try
            {
             
                var query = (from n in _colgateSkeltaEntities.BristleTempSetSet
                                select new BristleTempModel
                                {
                                    Id = n.Id,
                                    Classification = n.Classification,
                                    Name = n.Name,
                                    X = n.X,
                                    Y = n.Y,
                                    Width = n.Width,
                                    Height = n.Height,
                                    TuftSet = n.TuftTempSetSet,
                                    Probability = n.Probability
                                }).ToList();

                List<BristleTempModel> result = new List<BristleTempModel>(query);

                return result;
             
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<BristleTempModel>();
            }
        }

        public IList<BristleTempSetSet> GetAllBristleTempSetSets()
        {
            return _colgateSkeltaEntities.BristleTempSetSet.ToList();
        }
    }
}

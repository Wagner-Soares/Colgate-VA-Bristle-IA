using APIVision.Interfaces;
using APIVision.DataModels;
using Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using APIVision.DAO;

namespace APIVision.Controllers.DataBaseControllers
{
    public class BristleSetController
    {
        private readonly IBristleTempSetSetDao<BristleTempSetSet> _bristleTempSetRepo;
        private readonly ITuftTempSetSetDao<TuftTempSetSet> _tuftTempSetSetRepo;

        public BristleSetController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _bristleTempSetRepo = new BristleTempSetSetDao<BristleTempSetSet>(colgateSkeltaEntities);
            _tuftTempSetSetRepo = new TuftTempSetSetDao<TuftTempSetSet>(colgateSkeltaEntities);
        }

        public void UpdateBristleTempModel
            (List<BristleTempModel> tempInsertBristleTempModel,
             int TuftTempId)
        {            
            try
            {
                #region insert
                if (tempInsertBristleTempModel != null)
                {
                    var TuftTempId_ = (from n in _tuftTempSetSetRepo.GetAllTuftTempSetSets()
                                       where n.Id == TuftTempId
                                        select n).FirstOrDefault();

                    foreach (BristleTempModel insert in tempInsertBristleTempModel)
                    {
                        BristleTempSetSet insertDB = new BristleTempSetSet
                        {
                            Classification = insert.Classification,
                            X = insert.X,
                            Y = insert.Y,
                            Height = insert.Height,
                            Width = insert.Width,
                            Name = insert.Name,
                            TuftTempSetSet = TuftTempId_,
                            Probability = insert.Probability
                        };
                        _bristleTempSetRepo.Create(insertDB);
                    }
                }
                #endregion                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}

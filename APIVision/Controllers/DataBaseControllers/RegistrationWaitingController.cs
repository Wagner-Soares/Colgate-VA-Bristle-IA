using APIVision.DataModels;
using APIVision.Interfaces;
using Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using APIVision.DAO;

namespace APIVision.Controllers.DataBaseControllers
{
    public class RegistrationWaitingController
    {
        private readonly IRegistrationWaitingSetDao<RegistrationWaitingSet> _registrationWaitingSetRepo;
        private readonly IanalyzeSetDao<AnalyzeSet> _analyzeSetRepo;
        private readonly ITuftTempSetSetDao<TuftTempSetSet> _tuftTempSetSetRepo; 
        private readonly IBristleTempSetSetDao<BristleTempSetSet> _bristleTempSetSetRepo;
        private readonly IImageTempSetSetDao<ImageTempSetSet> _imageTempSetSetRepo;

        public RegistrationWaitingController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _registrationWaitingSetRepo = new RegistrationWaitingSetDao<RegistrationWaitingSet>(colgateSkeltaEntities);
            _analyzeSetRepo = new AnalyzeSetDao<AnalyzeSet>(colgateSkeltaEntities);
            _tuftTempSetSetRepo = new TuftTempSetSetDao<TuftTempSetSet>(colgateSkeltaEntities);
            _bristleTempSetSetRepo = new BristleTempSetSetDao<BristleTempSetSet>(colgateSkeltaEntities);
            _imageTempSetSetRepo = new ImageTempSetSetDao<ImageTempSetSet>(colgateSkeltaEntities);
        }

        public void UpdateRegistrationWaitingModel
            (RegistrationWaitingModel tempInsertRegistrationWaitingModel,
             RegistrationWaitingModel tempDeleteRegistrationWaitingModel)
        {
            
            try
            {
                #region insert
                if (tempInsertRegistrationWaitingModel != null)
                {
                    InsertRegistrationWaitingModel(tempInsertRegistrationWaitingModel);
                }
                #endregion

                #region delete
                else if (tempDeleteRegistrationWaitingModel != null)
                {
                    DeleteRegistrationWaitingModel(tempDeleteRegistrationWaitingModel);
                }
                #endregion
                #region delete all
                else
                {
                    _bristleTempSetSetRepo.EraseBristleTempSetSetsTable();
                    _imageTempSetSetRepo.EraseImageTempSetSetsTable();
                    _tuftTempSetSetRepo.EraseTuftTempSetSetTable();
                    _registrationWaitingSetRepo.EraseRegistrationWaitingSetsTable();
                }
                #endregion                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        private void DeleteRegistrationWaitingModel(RegistrationWaitingModel tempDeleteRegistrationWaitingModel)
        {
            RegistrationWaitingSet deleteDB = new RegistrationWaitingSet();
            deleteDB = (from n in _registrationWaitingSetRepo.GetAllRegistrationWaitingSets()
                        where n.Id == tempDeleteRegistrationWaitingModel.Id
                        select n).FirstOrDefault();

            TuftTempSetSet deleteTuftTempSetSet = new TuftTempSetSet();
            deleteTuftTempSetSet = (from n in _tuftTempSetSetRepo.GetAllTuftTempSetSets()
                                    where n.RegistrationWaitingSet.Id == deleteDB.Id
                                    select n).FirstOrDefault();

            while (true)
            {
                BristleTempSetSet deleteBristleTempSetSet;
                deleteBristleTempSetSet = (from n in _bristleTempSetSetRepo.GetAllBristleTempSetSets()
                                           where n.TuftTempSetSet.Id == deleteTuftTempSetSet.Id
                                           select n).FirstOrDefault();

                if (deleteBristleTempSetSet == null)
                {
                    break;
                }
                else
                {
                    _bristleTempSetSetRepo.Delete(deleteBristleTempSetSet);
                }
            }


            while (true)
            {
                ImageTempSetSet deleteImageTempSetSet;
                deleteImageTempSetSet = (from n in _imageTempSetSetRepo.GetAllImageTempSetSets()
                                         where n.TuftTempSetSet.Id == deleteTuftTempSetSet.Id
                                         select n).FirstOrDefault();

                if (deleteImageTempSetSet != null)
                {
                    _imageTempSetSetRepo.Delete(deleteImageTempSetSet);
                }
                else
                {
                    break;
                }
            }

            _tuftTempSetSetRepo.Delete(deleteTuftTempSetSet);
            _registrationWaitingSetRepo.Delete(deleteDB);
        }

        private void InsertRegistrationWaitingModel(RegistrationWaitingModel tempInsertRegistrationWaitingModel)
        {
            var reult = (from r in _analyzeSetRepo.GetAllAnalyzeSets()
                         orderby r.Id descending
                         select r).First();
            RegistrationWaitingSet insertDB = new RegistrationWaitingSet
            {
                AnalyzeSet_id = reult.Id.ToString(),
                DataSet_id = tempInsertRegistrationWaitingModel.DataSet_id,
                Sample_id = tempInsertRegistrationWaitingModel.Sample_id
            };
            _registrationWaitingSetRepo.Create(insertDB);
        }

        public List<RegistrationWaitingModel> ListRegistrationWaitingModel()
        {
            try
            {
                var query = (from n in _registrationWaitingSetRepo.GetAllRegistrationWaitingSets()
                                select new RegistrationWaitingModel
                                {
                                    Id = n.Id,
                                    DataSet_id = n.DataSet_id,
                                    Sample_id = n.Sample_id,
                                    AnalyzeSet_id = n.AnalyzeSet_id,
                                    TuftSet1 = n.TuftTempSetSets
                                }).ToList();

                List<RegistrationWaitingModel> result = new List<RegistrationWaitingModel>(query);

                return result.OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<RegistrationWaitingModel>();
            }
        }
    }
}

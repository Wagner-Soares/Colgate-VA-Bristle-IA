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
    public class TuftTempSetSetController
    {
        private readonly ITuftTempSetSetDao<TuftTempSetSet> _tuftTempSetSet;
        private readonly IRegistrationWaitingSetDao<RegistrationWaitingSet> _registrationWaitingSet;

        public TuftTempSetSetController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _tuftTempSetSet = new TuftTempSetSetDao<TuftTempSetSet>(colgateSkeltaEntities);
            _registrationWaitingSet = new RegistrationWaitingSetDao<RegistrationWaitingSet>(colgateSkeltaEntities);
        }

        public void UpdateTuftTempModel (TuftTempModel tempInsertTuftTempModel)
        {            
            try
            {
                if (tempInsertTuftTempModel != null)
                {
                    var lastId = (from r in _registrationWaitingSet.GetAllRegistrationWaitingSets()
                                    orderby r.Id descending
                                    select r).First();

                    TuftTempSetSet insertDB = new TuftTempSetSet
                    {
                        Position = tempInsertTuftTempModel.Position,
                        RegistrationWaitingSet = lastId
                    };
                    _tuftTempSetSet.Create(insertDB);
                }                    
            }
            catch
            {
                //tryCatch to avoid crash
            }
        }

        public List<TuftTempModel> ListTuftTempModel()
        {
            try
            {
                var query = (from n in _tuftTempSetSet.GetAllTuftTempSetSets()
                             select new TuftTempModel
                                {
                                    Id = n.Id,
                                    Position = n.Position
                                }).ToList();

                List<TuftTempModel> result = new List<TuftTempModel>(query);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<TuftTempModel>();
            }
        }
    }
}

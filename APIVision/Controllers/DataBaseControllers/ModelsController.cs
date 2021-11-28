using APIVision.DAO;
using APIVision.DataModels;
using APIVision.Interfaces;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIVision.Controllers.DataBaseControllers
{
    public class ModelsController
    {
        private readonly IModelsDao<ModelsModel> _modelsModel;

        public ModelsController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _modelsModel = new ModelsDao<ModelsModel>(colgateSkeltaEntities);
        }

        public List<ModelsModel> ListModelsModel()
        {
            try
            {
                var query = (from n in _modelsModel.GetAllModelss()
                                select new ModelsModel
                                {
                                    Id = n.Id,
                                    Status = n.Status,
                                    PerformanceType1 = n.PerformanceType1,
                                    PerformanceType2 = n.PerformanceType2,
                                    PerformanceType3 = n.PerformanceType3,
                                    PerformanceNone = n.PerformanceNone,
                                    PerformanceLocalization = n.PerformanceLocalization
                                }).ToList();

                List<ModelsModel> result = new List<ModelsModel>(query);

                return result.OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<ModelsModel>();
            }
        }
    }
}

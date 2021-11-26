using APIVision.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using APIVision;
using APIVision.DAO;
using APIVision.Interfaces;
using Database;

namespace APIVision.Controllers.DataBaseControllers
{
    public class AnalyzeSetController
    {
        private readonly IanalyzeSetDao<AnalyzeSet> _analyzeSetRepo;
        private readonly ISkuDao<SKU> _skuRepo ;

        public AnalyzeSetController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _analyzeSetRepo = new AnalyzeSetDao<AnalyzeSet>(colgateSkeltaEntities);
            _skuRepo = new SkuDao<SKU>(colgateSkeltaEntities); 
        }

        public int UpdateAnalyzeModel(AnalyzeSetModel tempInsertAnalyzeModel)
        {
            int id_ = 0;
            try
            {
                if (tempInsertAnalyzeModel != null)
                {
                    var lastId = (from r in _skuRepo.GetAllSKUs()

                    where r.sSKU == tempInsertAnalyzeModel.Name
                                    select r).FirstOrDefault();

                    AnalyzeSet insertDB = new AnalyzeSet
                    {
                        Name = tempInsertAnalyzeModel.Name,
                        iSKU_id = lastId.iID,
                        Equipament = tempInsertAnalyzeModel.Equipament,
                        Timestamp = tempInsertAnalyzeModel.Timestamp
                    };
                    _analyzeSetRepo.Create(insertDB);

                    var id = (from r in _analyzeSetRepo.GetAllAnalyzeSets()
                              orderby r.Id descending
                                select r).First();

                    id_ = id.Id;
                }                

                return id_;
            }
            catch
            { return id_; }
        }

        public List<AnalyzeSetModel> ListAnalyzeModel(bool first)
        {
            try
            {
                List<AnalyzeSetModel> result;

                if (!first)
                {
                    var query = (from n in _analyzeSetRepo.GetAllAnalyzeSets()
                                 select new AnalyzeSetModel
                                    {
                                        Id = n.Id,
                                        Name = n.Name,
                                        ISKU_id = n.iSKU_id,
                                        Equipament = n.Equipament,
                                        BristleAnalysisResultSets = n.BristleAnalysisResultSets,
                                        BrushAnalysisResultSets = n.BrushAnalysisResultSets,
                                        TuffAnalysisResultSets = n.TuffAnalysisResultSets,
                                        Timestamp = n.Timestamp
                                    }).ToList();

                    result = new List<AnalyzeSetModel>(query);
                }
                else
                {
                    var query_ = (from r in _analyzeSetRepo.GetAllAnalyzeSets()
                                  orderby r.Id descending
                                    select r).First();

                    AnalyzeSetModel result_ = new AnalyzeSetModel
                    {
                        Id = query_.Id,
                        Name = query_.Name,
                        ISKU_id = query_.iSKU_id,
                        Equipament = query_.Equipament,
                        BristleAnalysisResultSets = query_.BristleAnalysisResultSets,
                        BrushAnalysisResultSets = query_.BrushAnalysisResultSets,
                        TuffAnalysisResultSets = query_.TuffAnalysisResultSets,
                        Timestamp = query_.Timestamp
                    };

                    result = new List<AnalyzeSetModel>
                    {
                        result_
                    };
                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<AnalyzeSetModel>();
            }
        }
    }
}

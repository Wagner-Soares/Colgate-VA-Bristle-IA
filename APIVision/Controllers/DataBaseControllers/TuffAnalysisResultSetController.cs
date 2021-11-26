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
    public class TuffAnalysisResultSetController
    {
        private readonly ITuffAnalysisResultSetDao<TuffAnalysisResultSet> _tuffAnalysisResultSetRepo;
        private readonly IanalyzeSetDao<AnalyzeSet> _analyzeSetRepo;

        public TuffAnalysisResultSetController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _tuffAnalysisResultSetRepo = new TuffAnalysisResultSetDao<TuffAnalysisResultSet>(colgateSkeltaEntities);
            _analyzeSetRepo = new AnalyzeSetDao<AnalyzeSet>(colgateSkeltaEntities);
        }

        public void UpdateTuffAnalysisResultModel
            (TuffAnalysisResultModel tempInsertTuffAnalysisResultModel)
        {
            try
            {
                if (tempInsertTuffAnalysisResultModel != null)
                {
                    var result = (from r in _analyzeSetRepo.GetAllAnalyzeSets()
                                    orderby r.Id descending
                                    select r).First();

                    TuffAnalysisResultSet insertDB = new TuffAnalysisResultSet
                    {
                        Position = tempInsertTuffAnalysisResultModel.Position,
                        TotalBristleFoundManual = tempInsertTuffAnalysisResultModel.TotalBristleFoundManual,
                        TotalBristlesFoundNN = tempInsertTuffAnalysisResultModel.TotalBristlesFoundNN,
                        SelectedManual = tempInsertTuffAnalysisResultModel.SelectedManual,
                        Probability = tempInsertTuffAnalysisResultModel.Probability,
                        AnalyzeSet = result
                    };
                    _tuffAnalysisResultSetRepo.Create(insertDB);
                }                    
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public List<TuffAnalysisResultModel> ListTuffAnalysisResultModel(int analyzedId, BrushAnalysisResultModel brushAnalysisResultModel)
        {
            try
            {
                var query = (from n in _tuffAnalysisResultSetRepo.GetAllTuffAnalysisResultSets()
                             select new TuffAnalysisResultModel
                                {
                                    Id = n.Id,
                                    Position = n.Position,
                                    TotalBristleFoundManual = n.TotalBristleFoundManual,
                                    TotalBristlesFoundNN = n.TotalBristlesFoundNN,
                                    SelectedManual = n.SelectedManual,
                                    AnalyzeSet = n.AnalyzeSet,
                                    Probability = n.Probability
                                }).ToList();

                List<TuffAnalysisResultModel> result = new List<TuffAnalysisResultModel>(query);

                List<TuffAnalysisResultModel> result_ = new List<TuffAnalysisResultModel>();
                foreach (TuffAnalysisResultModel tuffAnalysisResultModel in result)
                {
                    if (analyzedId == -1)
                    {
                        if (tuffAnalysisResultModel.AnalyzeSet.Id == brushAnalysisResultModel.AnalyzeSet.Id)
                        {
                            result_.Add(tuffAnalysisResultModel);
                        }
                    }
                    else
                    {
                        if (tuffAnalysisResultModel.AnalyzeSet.Id == analyzedId)

                        {
                            result_.Add(tuffAnalysisResultModel);
                        }
                    }
                }

                return result_;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<TuffAnalysisResultModel>();
            }
        }
    }
}

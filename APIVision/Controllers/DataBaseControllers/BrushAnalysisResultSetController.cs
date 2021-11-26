using APIVision.DataModels;
using APIVision.Interfaces;
using Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Controls;
using APIVision.DAO;

namespace APIVision.Controllers.DataBaseControllers
{
    public class BrushAnalysisResultSetController
    {
        private readonly IBrushAnalysisResultSetDao<BrushAnalysisResultSet> _brushAnalysisResultSetRepo;
        private readonly IanalyzeSetDao<AnalyzeSet> _analyzeSetRepo;

        public BrushAnalysisResultSetController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _brushAnalysisResultSetRepo = new BrushAnalysisResultSetDao<BrushAnalysisResultSet>(colgateSkeltaEntities);
            _analyzeSetRepo = new AnalyzeSetDao<AnalyzeSet>(colgateSkeltaEntities);
        }

        public void UpdateBrushAnalysisResultModel (BrushAnalysisResultModel tempInsertBrushAnalysisResultModel)
        {
            try
            {
                if (tempInsertBrushAnalysisResultModel != null)
                {

                    BrushAnalysisResultSet insertDB = new BrushAnalysisResultSet();
                    var reult = (from r in _analyzeSetRepo.GetAllAnalyzeSets()
                                    orderby r.Id descending
                                    select r).First();
                    insertDB.AnalysisResult = tempInsertBrushAnalysisResultModel.AnalysisResult;
                    insertDB.TotalBristles = tempInsertBrushAnalysisResultModel.TotalBristles;
                    insertDB.TotalBristlesAnalyzed = tempInsertBrushAnalysisResultModel.TotalBristlesAnalyzed;
                    insertDB.TotalGoodBristles = tempInsertBrushAnalysisResultModel.TotalGoodBristles;
                    insertDB.AnalyzeSet = reult;
                    insertDB.Hybrid = tempInsertBrushAnalysisResultModel.Hybrid;
                    insertDB.Signaling_Id = tempInsertBrushAnalysisResultModel.Signaling_Id;
                    _brushAnalysisResultSetRepo.Create(insertDB);
                }
            }
            catch
            {
                //tryCatch to avoid crash
            }

        }

        public List<BrushAnalysisResultModel> ListBrushAnalysisResultModel(
            int analyzedId,
            string sku,
            string equipment,
            SelectedDatesCollection dates)
        {
            try
            {
                var query = (from n in _brushAnalysisResultSetRepo.GetAllBrushAnalysisResultSets()
                             select new BrushAnalysisResultModel
                                {
                                    Id = n.Id,
                                    AnalysisResult = n.AnalysisResult,
                                    TotalBristles = n.TotalBristles,
                                    TotalBristlesAnalyzed = n.TotalBristlesAnalyzed,
                                    TotalGoodBristles = n.TotalGoodBristles,
                                    AnalyzeSet = n.AnalyzeSet
                                }).ToList();

                List<BrushAnalysisResultModel> result = new List<BrushAnalysisResultModel>(query);


                if (analyzedId != -1)
                {
                    List<BrushAnalysisResultModel> result_ = new List<BrushAnalysisResultModel>();

                    if (analyzedId == -2)
                    {
                        if (sku == "Select SKU" && equipment == "Select Equipment" && dates.Count != 0)
                        {
                            foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                            {
                                foreach (var v in dates)
                                {
                                    if (brushAnalysisResultModel.AnalyzeSet.Timestamp.Date == v.Date)
                                    {
                                        result_.Add(brushAnalysisResultModel);
                                    }
                                }
                            }
                        }
                        else if (sku == "Select SKU" && equipment != "Select Equipment" && dates.Count == 0)
                        {
                            foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                            {
                                if (brushAnalysisResultModel.AnalyzeSet.Equipament == equipment)
                                {
                                    result_.Add(brushAnalysisResultModel);
                                }
                            }
                        }
                        else if (sku == "Select SKU" && equipment != "Select Equipment" && dates.Count != 0)
                        {
                            foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                            {
                                foreach (var v in dates)
                                {
                                    if (brushAnalysisResultModel.AnalyzeSet.Timestamp.Date == v.Date && brushAnalysisResultModel.AnalyzeSet.Equipament == equipment)
                                    {
                                        result_.Add(brushAnalysisResultModel);
                                    }
                                }
                            }
                        }
                        else if (sku != "Select SKU" && equipment == "Select Equipment" && dates.Count == 0)
                        {
                            foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                            {
                                if (brushAnalysisResultModel.AnalyzeSet.Name == sku)
                                {
                                    result_.Add(brushAnalysisResultModel);
                                }
                            }
                        }
                        else if (sku != "Select SKU" && equipment == "Select Equipment" && dates.Count != 0)
                        {
                            foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                            {
                                foreach (var v in dates)
                                {
                                    if (brushAnalysisResultModel.AnalyzeSet.Timestamp.Date == v.Date && brushAnalysisResultModel.AnalyzeSet.Name == sku)
                                    {
                                        result_.Add(brushAnalysisResultModel);
                                    }
                                }
                            }
                        }
                        else if (sku != "Select SKU" && equipment != "Select Equipment" && dates.Count == 0)
                        {
                            foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                            {
                                if (brushAnalysisResultModel.AnalyzeSet.Name == sku && brushAnalysisResultModel.AnalyzeSet.Equipament == equipment)
                                {
                                    result_.Add(brushAnalysisResultModel);
                                }
                            }
                        }
                        else if (sku != "Select SKU" && equipment != "Select Equipment" && dates.Count != 0)
                        {
                            foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                            {
                                foreach (var v in dates)
                                {
                                    if (brushAnalysisResultModel.AnalyzeSet.Timestamp.Date == v.Date && brushAnalysisResultModel.AnalyzeSet.Name == sku && brushAnalysisResultModel.AnalyzeSet.Equipament == equipment)
                                    {
                                        result_.Add(brushAnalysisResultModel);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                        {
                            if (brushAnalysisResultModel.AnalyzeSet.Id == analyzedId)
                            {
                                result_.Add(brushAnalysisResultModel);
                            }
                        }
                    }

                    return result_;
                }
                else
                {
                    return result.OrderByDescending(x => x.Id).ToList();
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<BrushAnalysisResultModel>();
            }
        }
    }
}

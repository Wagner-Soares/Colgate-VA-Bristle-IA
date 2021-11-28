using APIVision.DataModels;
using Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using APIVision;
using APIVision.Interfaces;
using APIVision.DAO;

namespace APIVision.Controllers.DataBaseControllers
{
    public class BristleAnalysisResultSetController
    {
        private readonly IBristleAnalysisResultSetDao<BristleAnalysisResultSet> _bristleAnalysisResultSetRepo;
        private readonly IanalyzeSetDao<AnalyzeSet> _analyzeSetRepo;

        public BristleAnalysisResultSetController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _analyzeSetRepo = new AnalyzeSetDao<AnalyzeSet>(colgateSkeltaEntities);
            _bristleAnalysisResultSetRepo = new BristleAnalysisResultSetDao<BristleAnalysisResultSet>(colgateSkeltaEntities);
        }

        public void UpdateBristleAnalysisResultModel(List<BristleAnalysisResultModel> tempInsertBristleAnalysisResult)
        {
            try
            {
                if (tempInsertBristleAnalysisResult != null)
                {
                    foreach (BristleAnalysisResultModel insert in tempInsertBristleAnalysisResult)
                    {
                        var reult = (from r in _analyzeSetRepo.GetAllAnalyzeSets()
                                     orderby r.Id descending
                                        select r).First();

                        if (insert.AnalyzeSet == null)
                        {
                            BristleAnalysisResultSet insertDB = new BristleAnalysisResultSet
                            {
                                DefectIdentified = insert.DefectIdentified,
                                DefectClassification = insert.DefectClassification,
                                SelectedManual = insert.SelectedManual,
                                X = insert.X,
                                Y = insert.Y,
                                Width = insert.Width,
                                Height = insert.Height,
                                Position = insert.Position,
                                Probability = insert.Probability,
                                AnalyzeSet = reult
                            };
                            _bristleAnalysisResultSetRepo.Create(insertDB);
                        }
                    }
                }
            }
            catch
            {
                //tryCatch to avoid crash
            }

        }

        public List<BristleAnalysisResultModel> ListBristleAnalysisResultModel(int analyzedId, BrushAnalysisResultModel brushAnalysisResultModel)
        {
            try
            {
                var query = (from n in _bristleAnalysisResultSetRepo.GetAllBristleAnalysisResultSets()
                             select new BristleAnalysisResultModel
                                {
                                    Id = n.Id,
                                    DefectClassification = n.DefectClassification,
                                    DefectIdentified = n.DefectIdentified,
                                    SelectedManual = n.SelectedManual,
                                    X = n.X,
                                    Y = n.Y,
                                    Width = n.Width,
                                    Height = n.Height,
                                    AnalyzeSet = n.AnalyzeSet,
                                    Position = n.Position,
                                    Probability = n.Probability,
                                }).ToList();

                List<BristleAnalysisResultModel> result = new List<BristleAnalysisResultModel>(query);

                List<BristleAnalysisResultModel> result_ = new List<BristleAnalysisResultModel>();
                foreach (BristleAnalysisResultModel bristleAnalysisResultModel in result)
                {
                    if (brushAnalysisResultModel != null)
                    {
                        if (bristleAnalysisResultModel.AnalyzeSet.Id == analyzedId || bristleAnalysisResultModel.AnalyzeSet.Id == brushAnalysisResultModel.AnalyzeSet.Id)
                        {
                            result_.Add(bristleAnalysisResultModel);
                        }
                    }
                    else
                    {
                        if (bristleAnalysisResultModel.AnalyzeSet.Id == analyzedId)
                        {
                            result_.Add(bristleAnalysisResultModel);
                        }
                    }
                }

                return result_;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<BristleAnalysisResultModel>();
            }
        }

    }
}

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
    public class DataSetSetController
    {
        private readonly IDatasetSetDao<DatasetSet> _dataSetSetRepo;
        private readonly IanalyzeSetDao<AnalyzeSet> _analyzeSetRepo;
        private readonly ISampleAsetDao<Sample_ASet> _sample_AsetRepo;
        private readonly ITuftTempSetSetDao<TuftTempSetSet> _tuftTempSetSet;
        private readonly ITuftSetDao<TuftSet> _tuftSet;
        private readonly IBristleTempSetSetDao<BristleTempSetSet> _bristleTempSetSet;
        private readonly IBristleSetDao<BristleSet> _bristleSet;
        private readonly IImageTempSetSetDao<ImageTempSetSet> _imageTempSetSet;
        private readonly IImageSetDao<ImageSet> _imageSet;

        public DataSetSetController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _dataSetSetRepo = new DatasetSetDao<DatasetSet>(colgateSkeltaEntities); 
            _analyzeSetRepo = new AnalyzeSetDao<AnalyzeSet>(colgateSkeltaEntities);
            _sample_AsetRepo = new SampleAsetDao<Sample_ASet>(colgateSkeltaEntities);
            _tuftTempSetSet = new TuftTempSetSetDao<TuftTempSetSet>(colgateSkeltaEntities);
            _tuftSet = new TuftSetDao<TuftSet>(colgateSkeltaEntities);
            _bristleTempSetSet = new BristleTempSetSetDao<BristleTempSetSet>(colgateSkeltaEntities);
            _bristleSet = new BristleSetDao<BristleSet>(colgateSkeltaEntities);
            _imageTempSetSet = new ImageTempSetSetDao<ImageTempSetSet>(colgateSkeltaEntities);
            _imageSet = new ImageSetDao<ImageSet>(colgateSkeltaEntities);
        }

        public void UpdateDatasetModel(DatasetModel tempInsertDatasetModel, List<RegistrationWaitingModel> tempRegistrationWaitingModel)
        {
            try
            {
                if (tempInsertDatasetModel == null)
                {
                    return;
                }

                DatasetSet insertDB = new DatasetSet
                {
                    Name = tempInsertDatasetModel.Name,
                    Historic = tempInsertDatasetModel.Historic
                };
                _dataSetSetRepo.Create(insertDB);

                var datasetSetId = (from r in _dataSetSetRepo.GetAllDatasetSets()
                                    orderby r.Id descending
                                    select r).First();

                foreach (var tempRegistrationWaitingModel_ in tempRegistrationWaitingModel)
                {
                    int id = Convert.ToInt32(tempRegistrationWaitingModel_.AnalyzeSet_id);

                    AnalyzeSet analyzeDB = (from n in _analyzeSetRepo.GetAllAnalyzeSets()
                                            where n.Id == id
                                            select n).FirstOrDefault();

                    Sample_ASet insetSample_ASet = new Sample_ASet
                    {
                        Name = analyzeDB.Name,
                        SKU_Id = analyzeDB.iSKU_id,
                        DatasetSet = datasetSetId
                    };
                    _sample_AsetRepo.Create(insetSample_ASet);

                    var sample_ASet_ = (from r in _sample_AsetRepo.GetAllSample_ASets()
                                        orderby r.Id descending
                                        select r).First();

                    TuftTempSetSet tuftTempSetSet;
                    tuftTempSetSet = (from n in _tuftTempSetSet.GetAllTuftTempSetSets()
                                      where n.RegistrationWaitingSet.Id == tempRegistrationWaitingModel_.Id
                                      select n).FirstOrDefault();

                    TuftSet insertTuftSet = new TuftSet
                    {
                        Position = tuftTempSetSet.Position,
                        Sample_ASet = sample_ASet_
                    };
                    _tuftSet.Create(insertTuftSet);

                    var tuftSet_ = (from r in _tuftSet.GetAllTuftSets()
                                    orderby r.Id descending
                                    select r).First();

                    foreach (var bristleTemp_ in _bristleTempSetSet.ListBristleTempModel()
                                                                    .Where(bristleTemp_ => bristleTemp_.TuftSet.Id == tuftTempSetSet.Id))
                    {
                        BristleSet insertBristleSet = new BristleSet
                        {
                            Classification = bristleTemp_.Classification,
                            Name = bristleTemp_.Name,
                            X = bristleTemp_.X,
                            Y = bristleTemp_.Y,
                            Height = bristleTemp_.Height,
                            Width = bristleTemp_.Width,
                            TuftSet = tuftSet_,
                            Probability = bristleTemp_.Probability
                        };
                        _bristleSet.Create(insertBristleSet);
                    }

                    foreach (var imageTemp_ in _imageTempSetSet.ListImageTempModel()
                                                                .Where(imageTemp_ => imageTemp_.TuftSet.Id == tuftTempSetSet.Id))
                    {
                        //remove nota
                        string[] pathWithoutNote = imageTemp_.Path.Split('@');
                        ImageSet insertImageSet = new ImageSet
                        {
                            Path = pathWithoutNote[0],
                            TuftSet = tuftSet_
                        };
                        _imageSet.Create(insertImageSet);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public List<DatasetModel> ListDatasetModel()
        {
            try
            {
                var query = (from n in _dataSetSetRepo.GetAllDatasetSets()
                             select new DatasetModel
                                {
                                    Id = n.Id,
                                    Name = n.Name,
                                    Historic = n.Historic,
                                    Sample_ASet = n.Sample_ASet
                                }).ToList();

                List<DatasetModel> result = new List<DatasetModel>(query);

                return result.OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<DatasetModel>();
            }
        }

    }
}

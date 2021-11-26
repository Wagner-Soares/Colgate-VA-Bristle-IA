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
    public class ValidationDatasetController
    {
        private readonly IValidationDatasetDao<ValidationDataset> _validationDatasetRepo;
        private readonly IanalyzeSetDao<AnalyzeSet> _analyzeSetRepo;
        private readonly IVsampleAsetDao<Vsample_ASet> _vsample_ASetRepo;
        private readonly ITuftTempSetSetDao<TuftTempSetSet> _tuftTempSetSetRepo;
        private readonly IVtuftSetDao<VtuftSet> _vtuftSetRepo;
        private readonly IBristleTempSetSetDao<BristleTempSetSet> _bristleTempSetSetRepo;
        private readonly IImageTempSetSetDao<ImageTempSetSet> _imageTempSetSetRepo;
        private readonly IVBristleSetDao<VbristleSet> _vbristleSetRepo;
        private readonly IVimageSetDao<VimageSet> _vimageSetRepo;

        public ValidationDatasetController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _validationDatasetRepo = new ValidationDatasetDao<ValidationDataset>(colgateSkeltaEntities);
            _analyzeSetRepo = new AnalyzeSetDao<AnalyzeSet>(colgateSkeltaEntities);
            _vsample_ASetRepo = new VsampleAsetDao<Vsample_ASet>(colgateSkeltaEntities);
            _tuftTempSetSetRepo = new TuftTempSetSetDao<TuftTempSetSet>(colgateSkeltaEntities);
            _vtuftSetRepo = new VtuftSetDao<VtuftSet>(colgateSkeltaEntities);
            _bristleTempSetSetRepo = new BristleTempSetSetDao<BristleTempSetSet>(colgateSkeltaEntities);
            _imageTempSetSetRepo = new ImageTempSetSetDao<ImageTempSetSet>(colgateSkeltaEntities);
            _vbristleSetRepo = new VbristleSetDao<VbristleSet>(colgateSkeltaEntities);
            _vimageSetRepo = new VimageSetDao<VimageSet>(colgateSkeltaEntities);
        }

        public List<ValidationDatasetModel> ListValidationDatasetModel()
        {
            try
            {
                var query = (from n in _validationDatasetRepo.GetAllValidationDatasets()
                                select new ValidationDatasetModel
                                {
                                    Id = n.Id,
                                    Name = n.Name,
                                    Historic = n.Historic,
                                    Vsample_ASet = n.Vsample_ASet
                                }).ToList();

                List<ValidationDatasetModel> result = new List<ValidationDatasetModel>(query);

                return result.OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<ValidationDatasetModel>();
            }
        }

        public void UpdateValidationDatasetModel
            (DatasetModel tempInsertValidationDatasetModel,
             List<RegistrationWaitingModel> tempRegistrationWaitingModel)
        {
            try
            {
                if (tempInsertValidationDatasetModel == null)
                {
                    return;                    
                }

                ValidationDataset insertDB = new ValidationDataset
                {
                    Name = tempInsertValidationDatasetModel.Name,
                    Historic = tempInsertValidationDatasetModel.Historic
                };
                _validationDatasetRepo.Create(insertDB);

                var datasetSetId = (from r in _validationDatasetRepo.GetAllValidationDatasets()
                                    orderby r.Id descending
                                    select r).First();

                foreach (var tempRegistrationWaitingModel_ in tempRegistrationWaitingModel)
                {
                    int id = Convert.ToInt32(tempRegistrationWaitingModel_.AnalyzeSet_id);

                    AnalyzeSet analyzeDB = (from n in _analyzeSetRepo.GetAllAnalyzeSets()
                                            where n.Id == id
                                            select n).FirstOrDefault();

                    Vsample_ASet insetVsample_ASet = new Vsample_ASet
                    {
                        Name = analyzeDB.Name,
                        SKU_Id = analyzeDB.iSKU_id,
                        ValidationDataset = datasetSetId
                    };
                    _vsample_ASetRepo.Create(insetVsample_ASet);

                    var vsample_ASet_ = (from r in _vsample_ASetRepo.GetAllVsample_ASets()
                                         orderby r.Id descending
                                         select r).First();

                    TuftTempSetSet tuftTempSetSet;
                    tuftTempSetSet = (from n in _tuftTempSetSetRepo.GetAllTuftTempSetSets()
                                      where n.RegistrationWaitingSet.Id == tempRegistrationWaitingModel_.Id
                                      select n).FirstOrDefault();

                    VtuftSet insertVtuftSet = new VtuftSet
                    {
                        Position = tuftTempSetSet.Position,
                        Vsample_ASet = vsample_ASet_
                    };
                    _vtuftSetRepo.Create(insertVtuftSet);

                    var vtuftSet_ = (from r in _vtuftSetRepo.GetAllVtuftSets()
                                     orderby r.Id descending
                                     select r).First();
                    foreach (var bristleTemp_ in _bristleTempSetSetRepo.ListBristleTempModel()
                                                                        .Where(bristleTemp_ => bristleTemp_.TuftSet.Id == tuftTempSetSet.Id))
                    {
                        VbristleSet insertVbristleSet = new VbristleSet
                        {
                            Classification = bristleTemp_.Classification,
                            Name = bristleTemp_.Name,
                            X = bristleTemp_.X,
                            Y = bristleTemp_.Y,
                            Height = bristleTemp_.Height,
                            Width = bristleTemp_.Width,
                            VtuftSet = vtuftSet_,
                            Probability = bristleTemp_.Probability
                        };
                        _vbristleSetRepo.Create(insertVbristleSet);
                    }

                    foreach (var imageTemp_ in _imageTempSetSetRepo.ListImageTempModel()
                                                                    .Where(imageTemp_ => imageTemp_.TuftSet.Id == tuftTempSetSet.Id))
                    {
                        //remove nota
                        string[] pathWithoutNote = imageTemp_.Path.Split('@');
                        VimageSet insertVimageSet = new VimageSet
                        {
                            Path = pathWithoutNote[0],
                            VtuftSet = vtuftSet_
                        };
                        _vimageSetRepo.Create(insertVimageSet);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

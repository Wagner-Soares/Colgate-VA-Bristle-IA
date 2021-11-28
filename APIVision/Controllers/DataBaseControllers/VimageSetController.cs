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
    public class VimageSetController
    {
        private readonly IVimageSetDao<VimageSet> _vimageSetRepo;
        private readonly IVtuftSetDao<VtuftSet> _vTuftSetRepo;

        public VimageSetController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _vimageSetRepo = new VimageSetDao<VimageSet>(colgateSkeltaEntities);
            _vTuftSetRepo = new VtuftSetDao<VtuftSet>(colgateSkeltaEntities);
        }

        public List<string> ListVimageSetModel(ValidationDatasetModel validationDataset)
        {
            try
            {
                List<VtuftSet> resultTufts = new List<VtuftSet>();
                List<VimageSet> resulImages = new List<VimageSet>();

                foreach (var item in validationDataset.Vsample_ASet)
                {
                    var query = (from n in _vTuftSetRepo.GetAllVtuftSets()
                                 where n.Vsample_ASet.Id == item.Id
                                    select n).FirstOrDefault();

                    resultTufts.Add(query);
                }

                foreach (var item in resultTufts)
                {
                    var query = (from n in _vimageSetRepo.GetAllVimageSets()
                                 where n.VtuftSet.Id == item.Id
                                    select n).FirstOrDefault();

                    resulImages.Add(query);
                }

                List<string> result = new List<string>();

                foreach (var item in resulImages)
                {
                    result.Add(item.Path);
                }

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<string>();
            }
        }
    }
}

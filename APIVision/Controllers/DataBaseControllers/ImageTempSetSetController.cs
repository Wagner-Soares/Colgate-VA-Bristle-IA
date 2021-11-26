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
    public class ImageTempSetSetController
    {
        private readonly IImageTempSetSetDao<ImageTempSetSet> _imageTempSetSetRepo;
        private readonly ITuftTempSetSetDao<TuftTempSetSet> _tuftTempSetSet;

        public ImageTempSetSetController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _imageTempSetSetRepo = new ImageTempSetSetDao<ImageTempSetSet>(colgateSkeltaEntities);
            _tuftTempSetSet = new TuftTempSetSetDao<TuftTempSetSet>(colgateSkeltaEntities);
        }

        public void UpdateImageTempModel (ImageTempModel tempInsertImageTempModel)
        {            
            try
            {
                if (tempInsertImageTempModel != null)
                {
                    var lastId = (from r in _tuftTempSetSet.GetAllTuftTempSetSets()
                                  orderby r.Id descending
                                    select r).First();

                    ImageTempSetSet insertDB = new ImageTempSetSet
                    {
                        Path = tempInsertImageTempModel.Path,
                        TuftTempSetSet = tempInsertImageTempModel.TuftSet
                    };
                    insertDB.TuftTempSetSet = lastId;
                    _imageTempSetSetRepo.Create(insertDB);
                }
            }
            catch
            {
                //tryCatch to avoid crash
            }
        }

        public List<ImageTempModel> ListImageTempModelByTuft(int tuft)
        {
            try
            {
                var query = (from n in _imageTempSetSetRepo.GetAllImageTempSetSets()
                             select new ImageTempModel
                             {
                                 Id = n.Id,
                                 Path = n.Path,
                                 TuftSet = n.TuftTempSetSet
                             }).ToList();

                List<ImageTempModel> result = new List<ImageTempModel>(query);
                List<ImageTempModel> result_ = new List<ImageTempModel>();
                foreach (ImageTempModel imageTempModel in result)
                {
                    if (imageTempModel.TuftSet.Id == tuft)
                    {
                        result_.Add(imageTempModel);
                    }
                }
                return result_;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<ImageTempModel>();
            }
        }
    }
}

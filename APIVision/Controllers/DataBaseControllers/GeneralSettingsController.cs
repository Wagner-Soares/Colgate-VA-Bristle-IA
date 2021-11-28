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
    public class GeneralSettingsController
    {
        private readonly IGeneralSettingsDao<GeneralSettings> _generalSettingsRepo;

        public GeneralSettingsController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _generalSettingsRepo = new GeneralSettingsDao<GeneralSettings>(colgateSkeltaEntities);
        }

        public List<GeneralSettingsModel> ListGeneralSettingsModel()
        {
            try
            {
                var query = (from n in _generalSettingsRepo.GetAllGeneralSettingss()
                                select new GeneralSettingsModel
                                {
                                    Id = n.Id,
                                    Prefix = n.Prefix
                                }).ToList();

                List<GeneralSettingsModel> result = new List<GeneralSettingsModel>(query);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<GeneralSettingsModel>();
            }
        }

        public void UpdateGeneralSettingsModel
            (GeneralSettingsModel tempInsertGeneralSettingsModel)
        {
            try
            {
                #region insert
                if (tempInsertGeneralSettingsModel != null)
                {
                    GeneralSettings insertDB = new GeneralSettings
                    {
                        Prefix = tempInsertGeneralSettingsModel.Prefix
                    };
                    _generalSettingsRepo.Create(insertDB);
                }
                #endregion
            }
            catch
            {
                //tryCatch to avoid crash
            }
        }
    }
}

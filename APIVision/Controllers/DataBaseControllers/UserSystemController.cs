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
    public class UserSystemController
    {
        private readonly IUserSystemDao<UserSystem> _userSystem;

        public UserSystemController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _userSystem = new UserSystemDao<UserSystem>(colgateSkeltaEntities);
        }

        public void UpdateUserSystemModel (UserSystemModel tempInsertUserSystemModel,UserSystemModel tempDeleteUserSystemModel)
        {            
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    #region insert
                    if (tempInsertUserSystemModel != null)
                    {
                        UserSystem insertDB = new UserSystem
                        {
                            Name = tempInsertUserSystemModel.Name,
                            Salt = tempInsertUserSystemModel.Salt,
                            Key = tempInsertUserSystemModel.Key,
                            Type = tempInsertUserSystemModel.Type
                        };
                        _userSystem.Create(insertDB);
                    }
                    #endregion                   

                    #region delete
                    if (tempDeleteUserSystemModel != null)
                    {
                        UserSystem deleteDB;
                        deleteDB = (from n in _userSystem.GetAllUserSystems()
                                    where n.Id == tempDeleteUserSystemModel.Id
                                    select n).FirstOrDefault();

                        _userSystem.Delete(deleteDB);
                    }
                    #endregion
                }
            }
            catch
            {
                //tryCatch to avoid crash
            }
        }

        public List<UserSystemModel> ListUserSystemModel()
        {
            try
            {
                var query = (from n in _userSystem.GetAllUserSystems()
                             select new UserSystemModel
                                {
                                    Id = n.Id,
                                    Name = n.Name,
                                    Salt = n.Salt,
                                    Key = n.Key,
                                    Type = n.Type
                                }).ToList();

                List<UserSystemModel> result = new List<UserSystemModel>(query);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<UserSystemModel>();
            }
        }
    }
}

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
    public class EquipmentController
    {
        private readonly IEquipmentDao<Equipment> _equipmentRepo;

        public EquipmentController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _equipmentRepo = new EquipmentDao<Equipment>(colgateSkeltaEntities);
        }

        public List<EquipmentModel> ListEquipmentModel(string area)
        {
            try
            {
                var query = (from n in _equipmentRepo.GetAllEquipments()
                                select new EquipmentModel
                                {
                                    IID = n.iID,
                                    IEquipment_id = n.iEquipment_id,
                                    IArea_id = n.iArea_id,
                                    SDescription = n.sDescription,
                                    SCreated_by = n.sCreated_by,
                                    DtCreated_at = n.dtCreated_at
                                }).ToList();

                List<EquipmentModel> result = new List<EquipmentModel>(query);
                List<EquipmentModel> resultFilter = new List<EquipmentModel>();

                if (area != "*")
                {
                    foreach (var item in result)
                    {
                        if (item.IArea_id == area)
                        {
                            resultFilter.Add(item);
                        }
                    }

                    return resultFilter;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<EquipmentModel>();
            }
        }
    }
}

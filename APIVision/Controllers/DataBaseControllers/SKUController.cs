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
    public class SkuController
    {
        private readonly ISkuDao<SKU> _skuRepo;

        public SkuController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _skuRepo = new SkuDao<SKU>(colgateSkeltaEntities);
        }

        public List<SkuModel> ListSKUsModel()
        {
            try
            {
                var query = (from n in _skuRepo.GetAllSKUs()
                                select new SkuModel
                                {
                                    IID = n.iID,
                                    SSKU = n.sSKU,
                                    IArea_id = n.iArea_id,
                                    SDescription = n.sDescription,
                                    DtCreated_at = n.dtCreated_at,
                                    SCreated_by = n.sCreated_by
                                }).ToList();

                List<SkuModel> result = new List<SkuModel>(query);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<SkuModel>();
            }
        }
    }
}
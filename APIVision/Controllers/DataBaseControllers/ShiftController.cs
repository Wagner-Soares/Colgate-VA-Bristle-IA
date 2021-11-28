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
    public class ShiftController
    {
        private readonly IShiftDao<Shift> _shiftRepo;

        public ShiftController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _shiftRepo = new ShiftDao<Shift>(colgateSkeltaEntities);
        }

        public List<ShiftsModel> ListShiftsModel()
        {
            try
            {
                var query = (from n in _shiftRepo.GetAllShifts()
                                select new ShiftsModel
                                {
                                    Id = n.Shift_id,
                                    Shift_start = n.Shift_start,
                                    Shift_end = n.Shift_end
                                }).ToList();

                List<ShiftsModel> result = new List<ShiftsModel>(query);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<ShiftsModel>();
            }
        }
    }
}

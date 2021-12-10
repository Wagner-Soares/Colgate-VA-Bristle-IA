using System;
using System.Collections.Generic;
using System.Text;
using APIVision.DAO;
using APIVision.Interfaces;
using APIVision.DataModels;
using Database;

namespace APIVision.Controllers.DataBaseControllers
{
    public class SampleLogController
    {
        private readonly ISampleLogDao<Sample_log> _SampleLogRepo;

        public SampleLogController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _SampleLogRepo = new SampleLogDao<Sample_log>(colgateSkeltaEntities);
        }

        public void UpdateSample_log(SampleLogModel tempInsertSample_log)
        {
            try
            {
                if (tempInsertSample_log != null)
                {
                    Sample_log insertDB = new Sample_log
                    {
                        iStatus_id = tempInsertSample_log.IStatus_id,
                        bActive = tempInsertSample_log.BActive,
                        iShift = tempInsertSample_log.IShift,
                        iTest_id = tempInsertSample_log.ITest_id,
                        sEquipament = tempInsertSample_log.SEquipament,
                        sArea = tempInsertSample_log.SArea,
                        sBatchLote = tempInsertSample_log.SBatchLote,
                        dtSample = tempInsertSample_log.DtSample.HasValue
                                 ? tempInsertSample_log.DtSample.Value.AddHours(-3)
                                 : (DateTime?)null,
                        fResult = tempInsertSample_log.FResult,
                        sOperator = tempInsertSample_log.SOperator,
                        dtPublished_at = tempInsertSample_log.DtPublished_at.AddHours(-3),
                        sComments = "Teste salvo automaticamente",
                        sCreated_by = tempInsertSample_log.SCreated_by,
                        dtCreated_at = tempInsertSample_log.DtCreated_at.AddHours(-3)
                    };


                    _SampleLogRepo.Create(insertDB);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex.Message);
            }
        }
    }
}

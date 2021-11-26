using System;
using System.Collections.Generic;
using System.Text;
using APIVision.DAO;
using APIVision.Interfaces;
using APIVision.DataModels;
using Database;

namespace APIVision.Controllers.DataBaseControllers
{
    public class AiSampleLogController
    {
        private readonly IAISampleLogDao<AI_Sample_log> _aiSampleLogRepo;

        public AiSampleLogController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _aiSampleLogRepo = new AiSampleLogDao<AI_Sample_log>(colgateSkeltaEntities);
        }

        public void UpdateAI_Sample_log(AiSampleLogModel tempInsertSample_log)
        {
            try
            {
                if (tempInsertSample_log != null)
                {
                    AI_Sample_log insertDB = new AI_Sample_log
                    {
                        iStatus_id = tempInsertSample_log.IStatus_id,
                        bActive = tempInsertSample_log.BActive,
                        iShift = tempInsertSample_log.IShift,
                        iTest_id = tempInsertSample_log.ITest_id,
                        sEquipament = tempInsertSample_log.SEquipament,
                        sArea = tempInsertSample_log.SArea,
                        sBatchLote = tempInsertSample_log.SBatchLote,
                        dtSample = tempInsertSample_log.DtSample,
                        fResult = tempInsertSample_log.FResult,
                        sOperator = tempInsertSample_log.SOperator,
                        dtPublished_at = tempInsertSample_log.DtPublished_at,
                        sComments = tempInsertSample_log.SComments,
                        sCreated_by = tempInsertSample_log.SCreated_by,
                        dtCreated_at = tempInsertSample_log.DtCreated_at
                    };
                    _aiSampleLogRepo.Create(insertDB);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex.Message);
            }
        }
    }
}

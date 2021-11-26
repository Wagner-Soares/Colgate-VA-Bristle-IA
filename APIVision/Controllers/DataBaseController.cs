using APIVision.Models;
using Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using APIVision;

namespace APIVision.Controllers
{
    /// <summary>
    /// Class integration with database  
    /// </summary>
    public class DataBaseController
    {
        #region update

        #region Results

        /*
         * Sample_logModel        
         */
        public void updateAI_Sample_log
            (AI_Sample_logModel tempInsertSample_log,
             AI_Sample_logModel tempModifySample_log,
             AI_Sample_logModel tempDeleteSample_log)
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    #region insert
                    if (tempInsertSample_log != null)
                    {
                        AI_Sample_log insertDB = new AI_Sample_log();
                        insertDB.iStatus_id = tempInsertSample_log.iStatus_id;
                        insertDB.bActive = tempInsertSample_log.bActive;
                        insertDB.iShift = tempInsertSample_log.iShift;
                        insertDB.iTest_id = tempInsertSample_log.iTest_id;
                        insertDB.sEquipament = tempInsertSample_log.sEquipament;
                        insertDB.sArea = tempInsertSample_log.sArea;
                        insertDB.sBatchLote = tempInsertSample_log.sBatchLote;
                        insertDB.dtSample = tempInsertSample_log.dtSample;
                        insertDB.fResult = tempInsertSample_log.fResult;
                        insertDB.sOperator = tempInsertSample_log.sOperator;
                        insertDB.dtPublished_at = tempInsertSample_log.dtPublished_at;
                        insertDB.sComments = tempInsertSample_log.sComments;
                        insertDB.sCreated_by = tempInsertSample_log.sCreated_by;
                        insertDB.sCreated_by = tempInsertSample_log.sCreated_by;
                        insertDB.dtCreated_at = tempInsertSample_log.dtCreated_at;
                        ef.AI_Sample_log.Add(insertDB);
                        ef.SaveChanges();
                    }
                    #endregion

                    #region modify
                    if (tempModifySample_log != null)
                    {
                        AI_Sample_log modifyDB = (from n in ef.AI_Sample_log
                                                  where n.Id == tempModifySample_log.Id
                                               select n).FirstOrDefault();

                        modifyDB.iStatus_id = tempInsertSample_log.iStatus_id;
                        modifyDB.bActive = tempInsertSample_log.bActive;
                        modifyDB.iShift = tempInsertSample_log.iShift;
                        modifyDB.iTest_id = tempInsertSample_log.iTest_id;
                        modifyDB.sEquipament = tempInsertSample_log.sEquipament;
                        modifyDB.sArea = tempInsertSample_log.sArea;
                        modifyDB.sBatchLote = tempInsertSample_log.sBatchLote;
                        modifyDB.dtSample = tempInsertSample_log.dtSample;
                        modifyDB.fResult = tempInsertSample_log.fResult;
                        modifyDB.sOperator = tempInsertSample_log.sOperator;
                        modifyDB.dtPublished_at = tempInsertSample_log.dtPublished_at;
                        modifyDB.sComments = tempInsertSample_log.sComments;
                        modifyDB.sCreated_by = tempInsertSample_log.sCreated_by;
                        modifyDB.sCreated_by = tempInsertSample_log.sCreated_by;
                        modifyDB.dtCreated_at = tempInsertSample_log.dtCreated_at;
                        ef.AI_Sample_log.Add(modifyDB);
                        ef.SaveChanges();
                    }
                    #endregion

                    #region delete
                    if (tempDeleteSample_log != null)
                    {
                        AI_Sample_log deleteDB = new AI_Sample_log();
                        deleteDB = (from n in ef.AI_Sample_log
                                    where n.Id == tempDeleteSample_log.Id
                                    select n).FirstOrDefault();

                        ef.AI_Sample_log.Remove(deleteDB);
                        ef.SaveChanges();
                    }
                    #endregion                  
                }        
            }
            catch (Exception ex)
            { 
                Console.WriteLine("{0}", ex.Message); 
            }
        }

        /*
        * AnalyzeModel        
        */
        public int updateAnalyzeModel
            (AnalyzeModel tempInsertAnalyzeModel,
             AnalyzeModel tempModifyAnalyzeModel,
             AnalyzeModel tempDeleteAnalyzeModel)
        {
            int id_ = 0;
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    #region insert
                    if (tempInsertAnalyzeModel != null)
                    {
                        var lastId = (from r in ef.SKUs
                                      where r.sSKU == tempInsertAnalyzeModel.Name
                                        select r).FirstOrDefault();

                        AnalyzeSet insertDB = new AnalyzeSet();
                        insertDB.Name = tempInsertAnalyzeModel.Name; 
                        insertDB.iSKU_id = lastId.iID;
                        insertDB.Equipament = tempInsertAnalyzeModel.Equipament;
                        insertDB.Timestamp = tempInsertAnalyzeModel.Timestamp;
                        ef.AnalyzeSet.Add(insertDB);
                        ef.SaveChanges();

                        var id = (from r in ef.AnalyzeSet
                                  orderby r.Id descending
                                  select r).First();

                        id_ = id.Id;
                    }
                    #endregion

                    #region modify
                    if (tempModifyAnalyzeModel != null)
                    {
                        AnalyzeSet modifyDB = (from n in ef.AnalyzeSet
                                               where n.Id == tempModifyAnalyzeModel.Id
                                                select n).FirstOrDefault();

                        modifyDB.Name = tempModifyAnalyzeModel.Name;
                        modifyDB.iSKU_id = modifyDB.iSKU_id;
                        modifyDB.Timestamp = tempModifyAnalyzeModel.Timestamp;
                        ef.AnalyzeSet.Add(modifyDB);
                        ef.SaveChanges();
                    }
                    #endregion

                    #region delete
                    if (tempDeleteAnalyzeModel != null)
                    {
                        AnalyzeSet deleteDB = new AnalyzeSet();
                        deleteDB = (from n in ef.AnalyzeSet
                                    where n.Id == tempDeleteAnalyzeModel.Id
                                    select n).FirstOrDefault();

                        ef.AnalyzeSet.Remove(deleteDB);
                        ef.SaveChanges();
                    }
                    #endregion                  
                }

                return id_;
            }
            catch
            { return id_; }
        }

        /*
        * TuffAnalysisResult        
        */
        public void updateTuffAnalysisResultModel
            (TuffAnalysisResultModel tempInsertTuffAnalysisResultModel,
             TuffAnalysisResultModel tempModifyTuffAnalysisResultModel,
             TuffAnalysisResultModel tempDeleteTuffAnalysisResultModel)
        {
            {
                try
                {
                    using (var ef = new ColgateSkeltaEntities())
                    {
                        #region insert
                        if (tempInsertTuffAnalysisResultModel != null)
                        {
                            var result = (from r in ef.AnalyzeSet
                                          orderby r.Id descending
                                             select r).First();
  
                            TuffAnalysisResultSet insertDB = new TuffAnalysisResultSet();
                            insertDB.Position = tempInsertTuffAnalysisResultModel.Position;
                            insertDB.TotalBristleFoundManual = tempInsertTuffAnalysisResultModel.TotalBristleFoundManual;
                            insertDB.TotalBristlesFoundNN = tempInsertTuffAnalysisResultModel.TotalBristlesFoundNN;
                            insertDB.SelectedManual = tempInsertTuffAnalysisResultModel.SelectedManual;
                            insertDB.Probability = tempInsertTuffAnalysisResultModel.Probability;
                            insertDB.AnalyzeSet = result;
                            ef.TuffAnalysisResultSet.Add(insertDB);
                            ef.SaveChanges();
                        }
                        #endregion

                        #region modify
                        if (tempModifyTuffAnalysisResultModel != null)
                        {
                            TuffAnalysisResultSet modifyDB = (from n in ef.TuffAnalysisResultSet
                                                              where n.Id == tempModifyTuffAnalysisResultModel.Id
                                                                select n).FirstOrDefault();

                            modifyDB.Position = tempModifyTuffAnalysisResultModel.Position;
                            modifyDB.TotalBristleFoundManual = tempModifyTuffAnalysisResultModel.TotalBristleFoundManual;
                            modifyDB.TotalBristlesFoundNN = tempModifyTuffAnalysisResultModel.TotalBristlesFoundNN;
                            modifyDB.SelectedManual = tempModifyTuffAnalysisResultModel.SelectedManual;


                        }
                        #endregion

                        #region delete
                        if (tempDeleteTuffAnalysisResultModel != null)
                        {
                            TuffAnalysisResultSet deleteDB = new TuffAnalysisResultSet();
                            deleteDB = (from n in ef.TuffAnalysisResultSet
                                        where n.Id == tempDeleteTuffAnalysisResultModel.Id
                                        select n).FirstOrDefault();

                            ef.TuffAnalysisResultSet.Remove(deleteDB);
                            ef.SaveChanges();
                        }
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /*
        * BrushAnalysisResult         
        */
        public void updateBrushAnalysisResultModel
            (BrushAnalysisResultModel tempInsertBrushAnalysisResultModel,
             BrushAnalysisResultModel tempModifyBrushAnalysisResultModel,
             BrushAnalysisResultModel tempDeleteBrushAnalysisResultModel)
        {
            {
                try
                {
                    using (var ef = new ColgateSkeltaEntities())
                    {
                        #region insert
                        if (tempInsertBrushAnalysisResultModel != null)
                        {

                            BrushAnalysisResultSet insertDB = new BrushAnalysisResultSet();
                            var reult = (from r in ef.AnalyzeSet
                                         orderby r.Id descending
                                         select r).First();
                            insertDB.AnalysisResult = tempInsertBrushAnalysisResultModel.AnalysisResult;
                            insertDB.TotalBristles = tempInsertBrushAnalysisResultModel.TotalBristles;
                            insertDB.TotalBristlesAnalyzed = tempInsertBrushAnalysisResultModel.TotalBristlesAnalyzed;
                            insertDB.TotalGoodBristles = tempInsertBrushAnalysisResultModel.TotalGoodBristles;
                            insertDB.AnalyzeSet = reult; 
                            insertDB.Hybrid = tempInsertBrushAnalysisResultModel.Hybrid;
                            insertDB.Signaling_Id = tempInsertBrushAnalysisResultModel.Signaling_Id;
                            ef.BrushAnalysisResultSet.Add(insertDB);
                            ef.SaveChanges();
                        }
                        #endregion

                        #region modify
                        if (tempModifyBrushAnalysisResultModel != null)
                        {
                            BrushAnalysisResultSet modifyDB = (from n in ef.BrushAnalysisResultSet
                                                               where n.Id == tempModifyBrushAnalysisResultModel.Id
                                                                select n).FirstOrDefault();

                            modifyDB.AnalysisResult = tempModifyBrushAnalysisResultModel.AnalysisResult;
                            //modifyDB.SignalingSet = SignalingModel_;
                            //modifyDB.SKUSet = SKUModel_;
                            modifyDB.TotalBristles = tempModifyBrushAnalysisResultModel.TotalBristles;
                            modifyDB.TotalBristlesAnalyzed = tempModifyBrushAnalysisResultModel.TotalBristlesAnalyzed;
                            modifyDB.TotalGoodBristles = tempModifyBrushAnalysisResultModel.TotalGoodBristles;
                            //modifyDB.Timestamp = modify.Timestamp;
                            //modifyDB.Tuff = modify.Tuff;
                        }
                        #endregion

                        #region delete
                        if (tempDeleteBrushAnalysisResultModel != null)
                        {
                            BrushAnalysisResultSet deleteDB = new BrushAnalysisResultSet();
                            deleteDB = (from n in ef.BrushAnalysisResultSet
                                        where n.Id == tempDeleteBrushAnalysisResultModel.Id
                                        select n).FirstOrDefault();

                            ef.BrushAnalysisResultSet.Remove(deleteDB);
                            ef.SaveChanges();
                        }
                        #endregion                       
                    }
                }
                catch
                { }
            }
        }

        /// <summary>
        /// BristleAnalysisResult
        /// </summary>
        /// <param name="tempInsertBristleAnalysisResult"></param>
        /// <param name="tempModifyBristleAnalysisResult"></param>
        /// <param name="tempDeleteBristleAnalysisResult"></param>
        public void updateBristleAnalysisResultModel
            (List<BristleAnalysisResultModel> tempInsertBristleAnalysisResult, 
             List<BristleAnalysisResultModel> tempModifyBristleAnalysisResult, 
             List<BristleAnalysisResultModel> tempDeleteBristleAnalysisResult)
        {
            {
                try
                {
                    using (var ef = new ColgateSkeltaEntities())
                    {
                        #region insert
                        if (tempInsertBristleAnalysisResult != null)
                        {
                            foreach (BristleAnalysisResultModel insert in tempInsertBristleAnalysisResult)
                            {
                                var reult = (from r in ef.AnalyzeSet
                                             orderby r.Id descending
                                             select r).First();

                                if(insert.AnalyzeSet == null)
                                {
                                    BristleAnalysisResultSet insertDB = new BristleAnalysisResultSet();
                                    insertDB.DefectIdentified = insert.DefectIdentified;
                                    insertDB.DefectClassification = insert.DefectClassification;
                                    insertDB.SelectedManual = insert.SelectedManual;
                                    insertDB.X = insert.X;
                                    insertDB.Y = insert.Y;
                                    insertDB.Width = insert.Width;
                                    insertDB.Height = insert.Height;
                                    insertDB.Position = insert.Position;
                                    insertDB.Probability = insert.Probability;
                                    insertDB.AnalyzeSet = reult;
                                    ef.BristleAnalysisResultSet.Add(insertDB);
                                    ef.SaveChanges();
                                }                            
                            }
                        }
                        #endregion

                        #region modify
                        if (tempModifyBristleAnalysisResult != null)
                        {
                            foreach (BristleAnalysisResultModel modify in tempModifyBristleAnalysisResult)
                            {
    
                                BristleAnalysisResultSet modifyDB = (from n in ef.BristleAnalysisResultSet
                                                                     where n.Id == modify.Id
                                                                           select n).FirstOrDefault();

                                modifyDB.DefectIdentified = modify.DefectIdentified;
                                modifyDB.DefectClassification = modify.DefectClassification;
                                modifyDB.SelectedManual = modify.SelectedManual;
                                modifyDB.X = modify.X;
                                modifyDB.Y = modify.Y;
                                modifyDB.Width = modify.Width;
                                modifyDB.Height = modify.Height;
                            }
                        }
                        #endregion

                        #region delete
                        if (tempDeleteBristleAnalysisResult != null)
                        {
                            foreach (BristleAnalysisResultModel delete in tempDeleteBristleAnalysisResult)
                            {
                                BristleAnalysisResultSet deleteDB = new BristleAnalysisResultSet();
                                deleteDB = (from n in ef.BristleAnalysisResultSet
                                            where n.Id == delete.Id
                                            select n).FirstOrDefault();

                                ef.BristleAnalysisResultSet.Remove(deleteDB);
                                ef.SaveChanges();
                            }
                        }
                        #endregion

                       
                    }
                }
                catch
                { }
            }
        }
        #endregion Results

        #region RegistrationWaitingModel
        /*
        * RegistrationWaitingModel [OK]        
        */
        public void updateRegistrationWaitingModel
            (RegistrationWaitingModel tempInsertRegistrationWaitingModel,
             RegistrationWaitingModel tempModifyRegistrationWaitingModel,
             RegistrationWaitingModel tempDeleteRegistrationWaitingModel)
        {
            {
                try
                {
                    using (var ef = new ColgateSkeltaEntities())
                    {
                        #region insert
                        if (tempInsertRegistrationWaitingModel != null)
                        {
                            var reult = (from r in ef.AnalyzeSet
                                         orderby r.Id descending
                                         select r).First();
                            RegistrationWaitingSet insertDB = new RegistrationWaitingSet();
                            insertDB.AnalyzeSet_id = reult.Id.ToString();
                            insertDB.DataSet_id = tempInsertRegistrationWaitingModel.DataSet_id;
                            insertDB.Sample_id = tempInsertRegistrationWaitingModel.Sample_id;
                            ef.RegistrationWaitingSet.Add(insertDB);
                            ef.SaveChanges();
                        }
                        #endregion

                        #region modify
                        else if (tempModifyRegistrationWaitingModel != null)
                        {
                            RegistrationWaitingSet modifyDB = (from n in ef.RegistrationWaitingSet
                                                               where n.Id == tempModifyRegistrationWaitingModel.Id
                                                   select n).FirstOrDefault();
                            modifyDB.AnalyzeSet_id = tempModifyRegistrationWaitingModel.AnalyzeSet_id;
                            modifyDB.DataSet_id = tempModifyRegistrationWaitingModel.DataSet_id;
                            modifyDB.Sample_id = tempModifyRegistrationWaitingModel.Sample_id;
                            ef.RegistrationWaitingSet.Add(modifyDB);
                            ef.SaveChanges();
                        }
                        #endregion

                        #region delete
                        else if (tempDeleteRegistrationWaitingModel != null)
                        {
                            RegistrationWaitingSet deleteDB = new RegistrationWaitingSet();
                            deleteDB = (from n in ef.RegistrationWaitingSet
                                        where n.Id == tempDeleteRegistrationWaitingModel.Id
                                        select n).FirstOrDefault();

                            TuftTempSetSet deleteTuftTempSetSet = new TuftTempSetSet();
                            deleteTuftTempSetSet = (from n in ef.TuftTempSetSet
                                                    where n.RegistrationWaitingSet.Id == deleteDB.Id
                                        select n).FirstOrDefault();

                            while(true)
                            {
                                BristleTempSetSet deleteBristleTempSetSet = new BristleTempSetSet();
                                deleteBristleTempSetSet = (from n in ef.BristleTempSetSet
                                                           where n.TuftTempSetSet.Id == deleteTuftTempSetSet.Id
                                                           select n).FirstOrDefault();

                                if (deleteBristleTempSetSet != null)
                                {
                                    ef.BristleTempSetSet.Remove(deleteBristleTempSetSet);
                                    ef.SaveChanges();
                                }
                                else
                                {
                                    break;
                                }
                            }
                          

                            while(true)
                            {
                                ImageTempSetSet deleteImageTempSetSet = new ImageTempSetSet();
                                deleteImageTempSetSet = (from n in ef.ImageTempSetSet
                                                         where n.TuftTempSetSet.Id == deleteTuftTempSetSet.Id
                                                         select n).FirstOrDefault();

                                if(deleteImageTempSetSet != null)
                                {
                                    ef.ImageTempSetSet.Remove(deleteImageTempSetSet);
                                    ef.SaveChanges();
                                }
                                else
                                {
                                    break;
                                }
                            }
                           
                            ef.TuftTempSetSet.Remove(deleteTuftTempSetSet);
                            ef.SaveChanges();
                            ef.RegistrationWaitingSet.Remove(deleteDB);
                            ef.SaveChanges();
                        }
                        #endregion
                        #region delete all
                        else
                        {
                            ef.BristleTempSetSet.RemoveRange(ef.BristleTempSetSet);
                            ef.SaveChanges();
                            ef.ImageTempSetSet.RemoveRange(ef.ImageTempSetSet);
                            ef.SaveChanges();
                            ef.TuftTempSetSet.RemoveRange(ef.TuftTempSetSet);
                            ef.SaveChanges();
                            ef.RegistrationWaitingSet.RemoveRange(ef.RegistrationWaitingSet);
                            ef.SaveChanges();                    
                        }
                        #endregion
                    }                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);                   
                }
            }
        }

        /*
    * UserSystemModel       
    */
        public void updateUserSystemModel
           (UserSystemModel tempInsertUserSystemModel,
            UserSystemModel tempModifyUserSystemModel,
            UserSystemModel tempDeleteUserSystemModel)
        {
            {
                try
                {
                    using (var ef = new ColgateSkeltaEntities())
                    {
                        #region insert
                        if (tempInsertUserSystemModel != null)
                        {
                            UserSystem insertDB = new UserSystem();
                            insertDB.Name = tempInsertUserSystemModel.Name;
                            insertDB.Salt = tempInsertUserSystemModel.Salt;
                            insertDB.Key = tempInsertUserSystemModel.Key;
                            insertDB.Type = tempInsertUserSystemModel.Type;
                            ef.UserSystems.Add(insertDB);
                            ef.SaveChanges();
                        }
                        #endregion

                        #region modify
                        if (tempModifyUserSystemModel != null)
                        {
                            UserSystem modifyDB = (from n in ef.UserSystems
                                                   where n.Id == tempModifyUserSystemModel.Id
                                                       select n).FirstOrDefault();
                            modifyDB.Name = tempInsertUserSystemModel.Name;
                            modifyDB.Salt = tempInsertUserSystemModel.Salt;
                            modifyDB.Key = tempModifyUserSystemModel.Key;
                            modifyDB.Type = tempInsertUserSystemModel.Type;
                            ef.UserSystems.Add(modifyDB);
                            ef.SaveChanges();
                        }
                        #endregion

                        #region delete
                        if (tempDeleteUserSystemModel != null)
                        {
                            UserSystem deleteDB = new UserSystem();
                            deleteDB = (from n in ef.UserSystems
                                        where n.Id == tempDeleteUserSystemModel.Id
                                        select n).FirstOrDefault();

                            ef.UserSystems.Remove(deleteDB);
                            ef.SaveChanges();
                        }
                        #endregion
                    }
                }
                catch
                { }
            }
        }

        /*
        * TuftTempModel       
        */
        public void updateTuftTempModel
           (TuftTempModel tempInsertTuftTempModel,
            TuftTempModel tempModifyTuftTempModel,
            TuftTempModel tempDeleteTuftTempModel)
        {
            {
                try
                {
                    using (var ef = new ColgateSkeltaEntities())
                    {
                        #region insert
                        if (tempInsertTuftTempModel != null)
                        {
                            var lastId = (from r in ef.RegistrationWaitingSet
                                          orderby r.Id descending
                                          select r).First();

                            TuftTempSetSet insertDB = new TuftTempSetSet();
                            insertDB.Position = tempInsertTuftTempModel.Position;
                            insertDB.RegistrationWaitingSet = lastId;
                            ef.TuftTempSetSet.Add(insertDB);
                            ef.SaveChanges();
                        }
                        #endregion

                        #region modify
                        if (tempModifyTuftTempModel != null)
                        {
                            TuftTempSetSet modifyDB = (from n in ef.TuftTempSetSet
                                                       where n.Id == tempModifyTuftTempModel.Id
                                                   select n).FirstOrDefault();
                            modifyDB.Position = tempInsertTuftTempModel.Position;
                            modifyDB.RegistrationWaitingSet = tempInsertTuftTempModel.RegistrationWaiting;
                            ef.TuftTempSetSet.Add(modifyDB);
                            ef.SaveChanges();
                        }
                        #endregion

                        #region delete
                        if (tempDeleteTuftTempModel != null)
                        {
                            TuftTempSetSet deleteDB = new TuftTempSetSet();
                            deleteDB = (from n in ef.TuftTempSetSet
                                        where n.Id == tempDeleteTuftTempModel.Id
                                        select n).FirstOrDefault();

                            ef.TuftTempSetSet.Remove(deleteDB);
                            ef.SaveChanges();
                        }
                       #endregion                      
                    }
                }
                catch
                { }
            }
        }

        /*
       * BristleTempModel        
       */
        public void updateBristleTempModel
            (List<BristleTempModel> tempInsertBristleTempModel,
             List<BristleTempModel> tempModifyBristleTempModel,
             List<BristleTempModel> tempDeleteBristleTempModel, int TuftTempId)
        {
            {
                try
                {
                    using (var ef = new ColgateSkeltaEntities())
                    {
                        #region insert
                        if (tempInsertBristleTempModel != null)
                        {
                            //var lastId = (from r in ef.TuftTempSetSet
                            //              orderby r.Id descending
                            //              select r).First();

                            var TuftTempId_ = (from n in ef.TuftTempSetSet
                                               where n.Id == TuftTempId
                                            select n).FirstOrDefault();

                            foreach (BristleTempModel insert in tempInsertBristleTempModel)
                            {
                                BristleTempSetSet insertDB = new BristleTempSetSet();
                                insertDB.Classification = insert.Classification;
                                insertDB.X = insert.X;
                                insertDB.Y = insert.Y;
                                insertDB.Height = insert.Height;
                                insertDB.Width = insert.Width;
                                insertDB.Name = insert.Name;
                                insertDB.TuftTempSetSet = TuftTempId_;
                                insertDB.Probability = insert.Probability;
                                ef.BristleTempSetSet.Add(insertDB);
                                ef.SaveChanges();
                            }
                        }
                        #endregion

                        #region modify
                        if (tempModifyBristleTempModel != null)
                        {
                            foreach (BristleTempModel modify in tempModifyBristleTempModel)
                            {
                                BristleTempSetSet modifyDB = (from n in ef.BristleTempSetSet
                                                              where n.Id == modify.Id
                                                           select n).FirstOrDefault();

                                modifyDB.Classification = modify.Classification;
                                modifyDB.X = modify.X;
                                modifyDB.Y = modify.Y;
                                modifyDB.Height = modify.Height;
                                modifyDB.Width = modify.Width;
                                modifyDB.Name = modify.Name;                          
                                ef.BristleTempSetSet.Add(modifyDB);
                                ef.SaveChanges();
                            }
                        }
                        #endregion

                        #region delete
                        if (tempDeleteBristleTempModel != null)
                        {
                            foreach (BristleTempModel delete in tempDeleteBristleTempModel)
                            {
                                BristleTempSetSet deleteDB = new BristleTempSetSet();
                                deleteDB = (from n in ef.BristleTempSetSet
                                            where n.Id == delete.Id
                                            select n).FirstOrDefault();

                                ef.BristleTempSetSet.Remove(deleteDB);
                                ef.SaveChanges();
                            }
                        }
                        #endregion                     
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /*
      * ImageTempModel        
      */
        public void updateImageTempModel
            (ImageTempModel tempInsertImageTempModel,
             ImageTempModel tempModifyImageTempModel,
             ImageTempModel tempDeleteImageTempModel)
        {
            {
                try
                {
                    using (var ef = new ColgateSkeltaEntities())
                    {
                        #region insert
                        if (tempInsertImageTempModel != null)
                        {
                            var lastId = (from r in ef.TuftTempSetSet
                                          orderby r.Id descending
                                          select r).First();

                            ImageTempSetSet insertDB = new ImageTempSetSet();
                            insertDB.Path = tempInsertImageTempModel.Path;
                            insertDB.TuftTempSetSet = tempInsertImageTempModel.TuftSet;
                            insertDB.TuftTempSetSet = lastId;
                            ef.ImageTempSetSet.Add(insertDB);
                            ef.SaveChanges();
                        }
                        #endregion

                        #region modify
                        if (tempModifyImageTempModel != null)
                        {
                            ImageTempSetSet modifyDB = (from n in ef.ImageTempSetSet
                                                        where n.Id == tempModifyImageTempModel.Id
                                                        select n).FirstOrDefault();

                            modifyDB.Path = tempInsertImageTempModel.Path;
                            modifyDB.TuftTempSetSet = tempInsertImageTempModel.TuftSet;
                            ef.ImageTempSetSet.Add(modifyDB);
                            ef.SaveChanges();
                        }
                        #endregion

                        #region delete
                        if (tempDeleteImageTempModel != null)
                        {
                            ImageTempSetSet deleteDB = new ImageTempSetSet();
                            deleteDB = (from n in ef.ImageTempSetSet
                                        where n.Id == tempDeleteImageTempModel.Id
                                        select n).FirstOrDefault();

                            ef.ImageTempSetSet.Remove(deleteDB);
                            ef.SaveChanges();
                        }
                        #endregion           
                    }
                }
                catch
                { }
            }
        }

        #endregion RegistrationWaitingModel

        #region RegistrationModel
        /*
         * Bristle         
         */
        public void updateBristleModel
            (List<BristleModel> tempInsertBristleModel,
             List<BristleModel> tempModifyBristleModel,
             List<BristleModel> tempDeleteBristleModel)
        {
            {
                try
                {
                    using (var ef = new ColgateSkeltaEntities())
                    {
                        #region insert
                        if (tempInsertBristleModel != null)
                        {
                            foreach (BristleModel insert in tempInsertBristleModel)
                            {
                                BristleSet insertDB = new BristleSet();

                                var DatasetModel_ = ef.DatasetSet.First(x => x.Id == insert.Dataset.Id);

                                insertDB.Name = insert.Name;
                                insertDB.X = insert.X;
                                insertDB.Y = insert.Y;
                                insertDB.Width = insert.Width;
                                insertDB.Height = insert.Height;
                                insertDB.Probability = insert.Probability;                                                         
                                ef.BristleSet.Add(insertDB);
                                ef.SaveChanges();
                            }
                        }
                        #endregion

                        #region modify
                        if (tempModifyBristleModel != null)
                        {
                            foreach (BristleModel modify in tempModifyBristleModel)
                            {
                                BristleSet modifyDB = (from n in ef.BristleSet
                                                       where n.Id == modify.Id
                                                       select n).FirstOrDefault();

                                var DatasetModel_ = ef.DatasetSet.First(x => x.Id == modify.Dataset.Id);

                                modifyDB.Name = modify.Name;
                                modifyDB.X = modify.X;
                                modifyDB.Y = modify.Y;
                                modifyDB.Width = modify.Width;
                                modifyDB.Height = modify.Height;
                                // modifyDB.Dataset = DatasetModel_;
                                //insertDB.Dataset.Id = insert.Dataset.Id; //verificar posteriormente 
                                //modifyDB.DefectClassification = modify.Classification;
                                ef.SaveChanges();
                            }
                        }
                        #endregion

                        #region delete
                        if (tempDeleteBristleModel != null)
                        {
                            foreach (BristleModel delete in tempDeleteBristleModel)
                            {
                                BristleSet deleteDB = new BristleSet();
                                deleteDB = (from n in ef.BristleSet
                                            where n.Id == delete.Id
                                            select n).FirstOrDefault();

                                ef.BristleSet.Remove(deleteDB);
                                ef.SaveChanges();
                            }
                        }
                        #endregion                
                    }
                }
                catch
                { }
            }
        }

        /*
        * Dataset         
        */
        public void updateDatasetModel
            (DatasetModel tempInsertDatasetModel,
             DatasetModel tempModifyDatasetModel,
             DatasetModel tempDeleteDatasetModel,
             List<RegistrationWaitingModel> tempRegistrationWaitingModel)
        {       
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    #region insert
                    if (tempInsertDatasetModel != null)
                    {
                        DatasetSet insertDB = new DatasetSet();
                        insertDB.Name = tempInsertDatasetModel.Name;
                        insertDB.Historic = tempInsertDatasetModel.Historic;
                        ef.DatasetSet.Add(insertDB);
                        ef.SaveChanges();

                        var datasetSetId = (from r in ef.DatasetSet
                                            orderby r.Id descending
                                            select r).First();

                        foreach (var tempRegistrationWaitingModel_ in tempRegistrationWaitingModel)
                        {
                            int id = Convert.ToInt32(tempRegistrationWaitingModel_.AnalyzeSet_id);

                            AnalyzeSet analyzeDB = (from n in ef.AnalyzeSet
                                                    where n.Id == id
                                                    select n).FirstOrDefault();

                            Sample_ASet insetSample_ASet = new Sample_ASet();
                            insetSample_ASet.Name = analyzeDB.Name;
                            insetSample_ASet.SKU_Id = analyzeDB.iSKU_id;
                            insetSample_ASet.DatasetSet = datasetSetId;
                            ef.Sample_ASet.Add(insetSample_ASet);
                            ef.SaveChanges();

                            var sample_ASet_ = (from r in ef.Sample_ASet
                                                orderby r.Id descending
                                                select r).First();

                            TuftTempSetSet tuftTempSetSet = new TuftTempSetSet();
                            tuftTempSetSet = (from n in ef.TuftTempSetSet
                                              where n.RegistrationWaitingSet.Id == tempRegistrationWaitingModel_.Id
                                                select n).FirstOrDefault();

                            TuftSet insertTuftSet = new TuftSet();
                            insertTuftSet.Position = tuftTempSetSet.Position;
                            insertTuftSet.Sample_ASet = sample_ASet_;
                            ef.TuftSet.Add(insertTuftSet);
                            ef.SaveChanges();

                            var tuftSet_ = (from r in ef.TuftSet
                                            orderby r.Id descending
                                            select r).First();

                            foreach (BristleTempModel bristleTemp_ in listBristleTempModel())
                            {
                                if(bristleTemp_.TuftSet.Id == tuftTempSetSet.Id)
                                {                                
                                    BristleSet insertBristleSet = new BristleSet();
                                    insertBristleSet.Classification = bristleTemp_.Classification;
                                    insertBristleSet.Name = bristleTemp_.Name;
                                    insertBristleSet.X = bristleTemp_.X;
                                    insertBristleSet.Y = bristleTemp_.Y;
                                    insertBristleSet.Height = bristleTemp_.Height;
                                    insertBristleSet.Width = bristleTemp_.Width;
                                    insertBristleSet.TuftSet = tuftSet_;
                                    insertBristleSet.Probability = bristleTemp_.Probability;
                                    ef.BristleSet.Add(insertBristleSet);
                                    ef.SaveChanges();
                                }
                            }

                            foreach (ImageTempModel imageTemp_ in listImageTempModel())
                            {
                                if (imageTemp_.TuftSet.Id == tuftTempSetSet.Id)
                                {
                                    //remove nota
                                    string[] pathWithoutNote = imageTemp_.Path.Split('@');

                                    ImageSet insertImageSet = new ImageSet();
                                    insertImageSet.Path = pathWithoutNote[0];
                                    insertImageSet.TuftSet = tuftSet_;
                                    ef.ImageSet.Add(insertImageSet);
                                    ef.SaveChanges();
                                }
                            }
                        }
                        #endregion

                        #region modify
                        if (tempModifyDatasetModel != null)
                        {
                            //var BristleModel_ = ef.BristleSet.First(x => x.Id == insert.Bristle.Id);

                            DatasetSet modifyDB = (from n in ef.DatasetSet
                                                   where n.Id == tempModifyDatasetModel.Id
                                                    select n).FirstOrDefault();
                            modifyDB.Name = tempModifyDatasetModel.Name;
                            //modifyDB.Bristle = BristleModel_;
                            //modifyDB.SKU = SKUModel_;
                            //modifyDB.GeneralSystemSettings = GeneralSystemSettingsModel_;
                            ef.SaveChanges();
                        }
                        #endregion

                        #region delete
                        if (tempDeleteDatasetModel != null)
                        {
                            DatasetSet deleteDB = new DatasetSet();
                            deleteDB = (from n in ef.DatasetSet
                                        where n.Id == tempModifyDatasetModel.Id
                                        select n).FirstOrDefault();

                            ef.DatasetSet.Remove(deleteDB);
                            ef.SaveChanges();
                        }
                        #endregion
                    }
                }
            }
                catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /*
         * Validation Dataset         
         */
        public void updateValidationDatasetModel
            (DatasetModel tempInsertValidationDatasetModel,
             DatasetModel tempModifyValidationDatasetModel,
             DatasetModel tempDeleteValidationDatasetModel,
             List<RegistrationWaitingModel> tempRegistrationWaitingModel)
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    #region insert
                    if (tempInsertValidationDatasetModel != null)
                    {
                        ValidationDataset insertDB = new ValidationDataset();
                        insertDB.Name = tempInsertValidationDatasetModel.Name;
                        insertDB.Historic = tempInsertValidationDatasetModel.Historic;
                        ef.ValidationDatasetSet.Add(insertDB);
                        ef.SaveChanges();

                        var datasetSetId = (from r in ef.ValidationDatasetSet
                                            orderby r.Id descending
                                            select r).First();

                        foreach (var tempRegistrationWaitingModel_ in tempRegistrationWaitingModel)
                        {
                            int id = Convert.ToInt32(tempRegistrationWaitingModel_.AnalyzeSet_id);

                            AnalyzeSet analyzeDB = (from n in ef.AnalyzeSet
                                                    where n.Id == id
                                                    select n).FirstOrDefault();

                            Vsample_ASet insetVsample_ASet = new Vsample_ASet();
                            insetVsample_ASet.Name = analyzeDB.Name;
                            insetVsample_ASet.SKU_Id = analyzeDB.iSKU_id;
                            insetVsample_ASet.ValidationDataset = datasetSetId;
                            ef.Vsample_ASet.Add(insetVsample_ASet);
                            ef.SaveChanges();

                            var vsample_ASet_ = (from r in ef.Vsample_ASet
                                                 orderby r.Id descending
                                                 select r).First();

                            TuftTempSetSet tuftTempSetSet = new TuftTempSetSet();
                            tuftTempSetSet = (from n in ef.TuftTempSetSet
                                              where n.RegistrationWaitingSet.Id == tempRegistrationWaitingModel_.Id
                                              select n).FirstOrDefault();

                            VtuftSet insertVtuftSet = new VtuftSet();
                            insertVtuftSet.Position = tuftTempSetSet.Position;
                            insertVtuftSet.Vsample_ASet = vsample_ASet_;
                            ef.VtuftSets.Add(insertVtuftSet);
                            ef.SaveChanges();

                            var vtuftSet_ = (from r in ef.VtuftSets
                                             orderby r.Id descending
                                             select r).First();

                            foreach (BristleTempModel bristleTemp_ in listBristleTempModel())
                            {        
                                if (bristleTemp_.TuftSet.Id == tuftTempSetSet.Id)
                                {
                                    VbristleSet insertVbristleSet = new VbristleSet();
                                    insertVbristleSet.Classification = bristleTemp_.Classification;
                                    insertVbristleSet.Name = bristleTemp_.Name;
                                    insertVbristleSet.X = bristleTemp_.X;
                                    insertVbristleSet.Y = bristleTemp_.Y;
                                    insertVbristleSet.Height = bristleTemp_.Height;
                                    insertVbristleSet.Width = bristleTemp_.Width;
                                    insertVbristleSet.VtuftSet = vtuftSet_;
                                    insertVbristleSet.Probability = bristleTemp_.Probability;
                                    ef.VbristleSets.Add(insertVbristleSet);
                                    ef.SaveChanges();
                                }
                            }

                            foreach (ImageTempModel imageTemp_ in listImageTempModel())
                            {
                                if (imageTemp_.TuftSet.Id == tuftTempSetSet.Id)
                                {
                                    //remove nota
                                    string[] pathWithoutNote = imageTemp_.Path.Split('@');        
                                    
                                    VimageSet insertVimageSet = new VimageSet();
                                    insertVimageSet.Path = pathWithoutNote[0];
                                    insertVimageSet.VtuftSet = vtuftSet_;
                                    ef.VimageSets.Add(insertVimageSet);
                                    ef.SaveChanges();
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion RegistrationModel

        #region Others
        /*
        * GeneralLocalSettings        
        */
        public void updateGeneralSettingsModel
            (GeneralSettingsModel tempInsertGeneralSettingsModel,
             GeneralSettingsModel tempModifyGeneralSettingsModel,
             GeneralSettingsModel tempDeleteGeneralSettingsModel)
        {
            {
                try
                {
                    using (var ef = new ColgateSkeltaEntities())
                    {
                        #region insert
                        if (tempInsertGeneralSettingsModel != null)
                        {
                            GeneralSettings insertDB = new GeneralSettings();
                            insertDB.Prefix = tempInsertGeneralSettingsModel.Prefix;                
                            ef.GeneralSettings.Add(insertDB);
                            ef.SaveChanges();
                        }
                        #endregion

                        #region modify
                        if (tempModifyGeneralSettingsModel != null)
                        {
                            GeneralSettings modifyDB = (from n in ef.GeneralSettings
                                                        where n.Id == tempModifyGeneralSettingsModel.Id
                                                        select n).FirstOrDefault();

                            modifyDB.Prefix = tempModifyGeneralSettingsModel.Prefix;                         
                            //ef.GeneralSettings. Add(modifyDB);
                            ef.SaveChanges();
                        }
                        #endregion

                        #region delete
                        if (tempDeleteGeneralSettingsModel != null)
                        {
                            GeneralSettings deleteDB = new GeneralSettings();
                            deleteDB = (from n in ef.GeneralSettings
                                        where n.Id == tempDeleteGeneralSettingsModel.Id
                                        select n).FirstOrDefault();

                            ef.GeneralSettings.Remove(deleteDB);
                            ef.SaveChanges();
                        }
                        #endregion           
                    }
                }
                catch
                { }
            }
        }
        #endregion Others

        #endregion update

        #region List

        /*
        * Sample_logModel
        */
        public List<AI_Sample_logModel> listSample_logModel()
        {
            try
            {
                using (var ef_ = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef_.Sample_log
                                 select new AI_Sample_logModel
                                 {
                                     Id = n.iID,
                                     iStatus_id = n.iStatus_id,
                                     bActive = n.bActive,
                                     iShift = n.iShift,
                                     iTest_id = n.iTest_id,
                                     sEquipament = n.sEquipament,
                                     sArea = n.sArea,
                                     sBatchLote = n.sBatchLote,
                                     dtSample = n.dtSample,
                                     fResult = n.fResult,
                                     sOperator = n.sOperator,
                                     dtPublished_at =n.dtPublished_at,
                                     sComments = n.sComments,
                                     sCreated_by = n.sCreated_by,
                                     dtCreated_at = n.dtCreated_at
                                 }).ToList();

                    List<AI_Sample_logModel> result = new List<AI_Sample_logModel>(query);

                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
        * QM_StatusModel
        */
        public List<QM_StatusModel> listQM_StatusModel()
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.QM_Status
                                 select new QM_StatusModel
                                 {
                                    Id = n.iID,
                                    iStatus_id = n.iStatus_id,
                                    sDescription = n.sDescription,
                                    sCreated_by = n.sCreated_by,
                                    dtCreated_at = n.dtCreated_at
                                 }).ToList();

                    List<QM_StatusModel> result = new List<QM_StatusModel>(query);

                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
         * QM_SpecModel
        */
        public List<QM_SpecModel> listQM_SpecModel()
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.QM_Spec
                                 select new QM_SpecModel
                                 {                                     
                                     iID = n.iID,
                                     iTest_id = n.iTest_id,
                                     fTarget = n.fTarget
                                 }).ToList();

                    List<QM_SpecModel> result = new List<QM_SpecModel>(query);

                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
        * QM_StatusModel
        */
        public List<ShiftsModel> listShiftsModel()
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.Shifts
                                 select new ShiftsModel
                                 {
                                     Id = n.Shift_id,
                                     Shift_start  = n.Shift_start,
                                     Shift_end = n.Shift_end
                                 }).ToList();

                    List<ShiftsModel> result = new List<ShiftsModel>(query);

                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
         * AreaModel
         */
        public List<AreaModel> listAreaModel()
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.Areas
                                 select new AreaModel
                                 {
                                     Id = n.iID,
                                     iArea_id = n.iArea_id,
                                     sDescription = n.sDescription,
                                     dtCreated_at = n.dtCreated_at,
                                     sCreated_by = n.sCreated_by
                                 }).ToList();

                    List<AreaModel> result = new List<AreaModel>(query);

                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
        * QM_StatusModel
        */
        public List<TestModel> listTestModel(int SKU)
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.Tests
                                 select new TestModel
                                 {
                                     Id = n.iTest_id,
                                     iSKU = n.iSKU,
                                     sDescription = n.sDescription,
                                     dtCreated_at = n.dtCreated_at,
                                     sCreated_by = n.sCreated_by
                                 }).ToList();

                    List<TestModel> result = new List<TestModel>(query);
                    List<TestModel> resultFilter = new List<TestModel>();

                    if(SKU != -1)
                    {
                        foreach (var item in result)
                        {
                            if (item.iSKU == SKU)
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
         * GeneralSettingsModel
         */
        public List<GeneralSettingsModel> listGeneralSettingsModel()
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.GeneralSettings
                                 select new GeneralSettingsModel
                                 {
                                     Id = n.Id,
                                     Prefix = n.Prefix                         
                                 }).ToList();

                    List<GeneralSettingsModel> result = new List<GeneralSettingsModel>(query);

                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
        * ImageTempModel
        */
        public List<ImageTempModel> listImageTempModel()
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.ImageTempSetSet
                                 select new ImageTempModel
                                 {
                                     Id = n.Id,
                                     Path = n.Path,
                                     TuftSet = n.TuftTempSetSet
                                 }).ToList();

                    List<ImageTempModel> result = new List<ImageTempModel>(query);

                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
         * ImageTempModel
         */
        public List<ImageTempModel> listImageTempModel(int tuft)
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.ImageTempSetSet
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
         * VimageSetModel
         */
        public List<string> listVimageSetModel(ValidationDatasetModel validationDataset)
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    List<VtuftSet> resultTufts = new List<VtuftSet>();
                    List<VimageSet> resulImages = new List<VimageSet>();

                    foreach (var item in validationDataset.Vsample_ASet)
                    {
                        var query = (from n in ef.VtuftSets
                                     where n.Vsample_ASet.Id == item.Id
                                       select n).FirstOrDefault();

                        resultTufts.Add(query);
                    }

                    foreach (var item in resultTufts)
                    {
                        var query = (from n in ef.VimageSets
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
            * BristleTempModel
            */
        public List<BristleTempModel> listBristleTempModel()
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.BristleTempSetSet
                                 select new BristleTempModel
                                 {
                                     Id = n.Id,
                                     Classification = n.Classification,
                                     Name  = n.Name,                                  
                                     X = n.X,
                                     Y = n.Y,
                                     Width = n.Width,
                                     Height = n.Height,
                                     TuftSet = n.TuftTempSetSet,    
                                     Probability = n.Probability
                                 }).ToList();

                    List<BristleTempModel> result = new List<BristleTempModel>(query);

                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
         * BristleTempModel
         */
        public List<BristleTempModel> listBristleTempModel(int tuft)
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.BristleTempSetSet
                                 select new BristleTempModel
                                 {
                                     Id = n.Id,
                                     Classification = n.Classification,
                                     Name = n.Name,
                                     X = n.X,
                                     Y = n.Y,
                                     Width = n.Width,
                                     Height = n.Height,
                                     TuftSet = n.TuftTempSetSet,
                                     Probability = n.Probability
                                 }).ToList();

                    List<BristleTempModel> result = new List<BristleTempModel>(query);
                    List<BristleTempModel> result_ = new List<BristleTempModel>();
                    foreach (BristleTempModel bristleTempModel in result)
                    {
                        if(bristleTempModel.TuftSet.Id == tuft)
                        {
                            result_.Add(bristleTempModel);
                        }
                    }

                    return result_;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        /*
         * BristleAnalysisResult
         */
        public List<BristleAnalysisResultModel> listBristleAnalysisResultModel(int analyzedId, BrushAnalysisResultModel brushAnalysisResultModel)
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.BristleAnalysisResultSet
                                 select new BristleAnalysisResultModel
                                 {
                                     Id = n.Id,
                                     DefectClassification = n.DefectClassification,
                                     DefectIdentified= n.DefectIdentified,                              
                                     SelectedManual = n.SelectedManual,
                                     X = n.X,
                                     Y = n.Y,
                                     Width = n.Width,
                                     Height = n.Height,
                                     AnalyzeSet = n.AnalyzeSet,
                                     Position = n.Position,
                                     Probability = n.Probability,                                    
                                 }).ToList();

                    List<BristleAnalysisResultModel> result = new List<BristleAnalysisResultModel>(query);

                    List<BristleAnalysisResultModel> result_ = new List<BristleAnalysisResultModel>();
                    foreach (BristleAnalysisResultModel bristleAnalysisResultModel in result)
                    {
                        if(brushAnalysisResultModel != null)
                        {
                            if (bristleAnalysisResultModel.AnalyzeSet.Id == analyzedId || bristleAnalysisResultModel.AnalyzeSet.Id == brushAnalysisResultModel.AnalyzeSet.Id)
                            {
                                result_.Add(bristleAnalysisResultModel);
                            }
                        }
                        else
                        {
                            if (bristleAnalysisResultModel.AnalyzeSet.Id == analyzedId)
                            {
                                result_.Add(bristleAnalysisResultModel);
                            }
                        }
                    }

                    return result_;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
         * Bristle
         */
        public List<BristleModel> listBristleModel()
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.BristleSet
                                 select new BristleModel
                                 {
                                     Id = n.Id,
                                     Classification = n.Classification,
                                     Name = n.Name,
                                     X = n.X,
                                     Y = n.Y,
                                     Width = n.Width,
                                     Height = n.Height,                                     
                                    // DatasetId = n.Dataset.Id,                                                                  
                                 }).ToList();

                    List<BristleModel> result = new List<BristleModel>(query);

                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
         * BrushAnalysisResul
         */
        public List<BrushAnalysisResultModel> listBrushAnalysisResultModel(
            int analyzedId, 
            BrushAnalysisResultModel brushAnalysisResultModel_, 
            string sku, 
            string equipment,
            SelectedDatesCollection dates)
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.BrushAnalysisResultSet
                                 select new BrushAnalysisResultModel
                                 {
                                     Id = n.Id,
                                     AnalysisResult = n.AnalysisResult,                            
                                     TotalBristles = n.TotalBristles,
                                     TotalBristlesAnalyzed = n.TotalBristlesAnalyzed,
                                     TotalGoodBristles = n.TotalGoodBristles,    
                                     AnalyzeSet = n.AnalyzeSet
                                 }).ToList();

                    List<BrushAnalysisResultModel> result = new List<BrushAnalysisResultModel>(query);


                    if(analyzedId != -1)
                    {
                        List<BrushAnalysisResultModel> result_ = new List<BrushAnalysisResultModel>();

                        if(analyzedId == -2)
                        {
                            if (sku == "Select SKU" && equipment == "Select Equipment" && dates.Count != 0)
                            {
                                foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                                {
                                    foreach(var v in dates)
                                    {
                                        if (brushAnalysisResultModel.AnalyzeSet.Timestamp.Date == v.Date)
                                        {
                                            result_.Add(brushAnalysisResultModel);
                                        }
                                    }                                
                                }
                            }
                            else if (sku == "Select SKU" && equipment != "Select Equipment" && dates.Count == 0)
                            {
                                foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                                {
                                    if (brushAnalysisResultModel.AnalyzeSet.Equipament == equipment)
                                    {
                                        result_.Add(brushAnalysisResultModel);
                                    }
                                }
                            }
                            else if (sku == "Select SKU" && equipment != "Select Equipment" && dates.Count != 0)
                            {
                                foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                                {
                                    foreach (var v in dates)
                                    {
                                        if (brushAnalysisResultModel.AnalyzeSet.Timestamp.Date == v.Date)
                                        {
                                            if(brushAnalysisResultModel.AnalyzeSet.Equipament == equipment)
                                            {
                                                result_.Add(brushAnalysisResultModel);
                                            }                                            
                                        }
                                    }
                                }
                            }
                            else if (sku != "Select SKU" && equipment == "Select Equipment" && dates.Count == 0)
                            {
                                foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                                {
                                    if (brushAnalysisResultModel.AnalyzeSet.Name == sku)
                                    {
                                        result_.Add(brushAnalysisResultModel);
                                    }
                                }
                            }
                            else if (sku != "Select SKU" && equipment == "Select Equipment" && dates.Count != 0)
                            {
                                foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                                {
                                    foreach (var v in dates)
                                    {
                                        if (brushAnalysisResultModel.AnalyzeSet.Timestamp.Date == v.Date)
                                        {
                                            if (brushAnalysisResultModel.AnalyzeSet.Name == sku)
                                            {
                                                result_.Add(brushAnalysisResultModel);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (sku != "Select SKU" && equipment != "Select Equipment" && dates.Count == 0)
                            {
                                foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                                {
                                    if (brushAnalysisResultModel.AnalyzeSet.Name == sku && brushAnalysisResultModel.AnalyzeSet.Equipament == equipment)
                                    {
                                        result_.Add(brushAnalysisResultModel);
                                    }
                                }
                            }
                            else if (sku != "Select SKU" && equipment != "Select Equipment" && dates.Count != 0)
                            {
                                foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                                {
                                    foreach (var v in dates)
                                    {
                                        if (brushAnalysisResultModel.AnalyzeSet.Timestamp.Date == v.Date)
                                        {
                                            if (brushAnalysisResultModel.AnalyzeSet.Name == sku && brushAnalysisResultModel.AnalyzeSet.Equipament == equipment)
                                            {
                                                result_.Add(brushAnalysisResultModel);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (BrushAnalysisResultModel brushAnalysisResultModel in result)
                            {
                                //if (brushAnalysisResultModel.AnalyzeSet.Id == analyzedId || brushAnalysisResultModel.AnalyzeSet.Id == brushAnalysisResultModel_.AnalyzeSet.Id)
                                if (brushAnalysisResultModel.AnalyzeSet.Id == analyzedId)
                                {
                                    result_.Add(brushAnalysisResultModel);
                                }
                            }
                        }                     

                        return result_;
                    }
                    else
                    {
                        return result.OrderByDescending(x => x.Id).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
         * Dataset
         */
        public List<DatasetModel> listDatasetModel()
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.DatasetSet
                                 select new DatasetModel
                                 {
                                     Id = n.Id,
                                     Name = n.Name,
                                     Historic = n.Historic,
                                     Sample_ASet = n.Sample_ASet                                     
                                 }).ToList();

                    List<DatasetModel> result = new List<DatasetModel>(query);

                    return result.OrderByDescending(x => x.Id).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
       * AnalyzeModel
       */
        public List<AnalyzeModel> listAnalyzeModel(bool first)
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    List<AnalyzeModel> result;

                    if (!first)
                    {
                        var query = (from n in ef.AnalyzeSet
                                     select new AnalyzeModel
                                     {
                                         Id = n.Id,
                                         Name = n.Name,
                                         iSKU_id = n.iSKU_id,
                                         Equipament = n.Equipament,
                                         BristleAnalysisResultSets = n.BristleAnalysisResultSets,
                                         BrushAnalysisResultSets = n.BrushAnalysisResultSets,
                                         TuffAnalysisResultSets = n.TuffAnalysisResultSets,
                                         Timestamp = n.Timestamp
                                     }).ToList();

                        result = new List<AnalyzeModel>(query);
                    }
                    else
                    {
                        var query_ = (from r in ef.AnalyzeSet
                                      orderby r.Id descending
                                      select r).First();

                        AnalyzeModel result_ = new AnalyzeModel();
                        result_.Id = query_.Id;
                        result_.Name = query_.Name;
                        result_.iSKU_id = query_.iSKU_id;
                        result_.Equipament = query_.Equipament;
                        result_.BristleAnalysisResultSets = query_.BristleAnalysisResultSets;
                        result_.BrushAnalysisResultSets = query_.BrushAnalysisResultSets;
                        result_.TuffAnalysisResultSets = query_.TuffAnalysisResultSets;
                        result_.Timestamp = query_.Timestamp;

                        result = new List<AnalyzeModel>();
                        result.Add(result_);
                    }     
                    
                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
          * Validation Dataset
          */
        public List<ValidationDatasetModel> listValidationDatasetModel()
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.ValidationDatasetSet
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
          * Models
          */
        public List<ModelsModel> listModelsModel()
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.ModelsSet
                                 select new ModelsModel
                                 {
                                     Id = n.Id,
                                     Status = n.Status,
                                     PerformanceType1 = n.PerformanceType1,
                                     PerformanceType2 = n.PerformanceType2,
                                     PerformanceType3 = n.PerformanceType3,
                                     PerformanceNone = n.PerformanceNone,   
                                     PerformanceLocalization = n.PerformanceLocalization
                                 }).ToList();

                    List<ModelsModel> result = new List<ModelsModel>(query);

                    return result.OrderByDescending(x => x.Id).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<RegistrationWaitingModel> listRegistrationWaitingModel()
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.RegistrationWaitingSet
                                 select new RegistrationWaitingModel
                                 {
                                     Id = n.Id,
                                     DataSet_id = n.DataSet_id,
                                     Sample_id = n.Sample_id,
                                     AnalyzeSet_id = n.AnalyzeSet_id,
                                     TuftSet1 = n.TuftTempSetSets
                                 }).ToList();

                    List<RegistrationWaitingModel> result = new List<RegistrationWaitingModel>(query);

                    return result.OrderByDescending(x => x.Id).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
         * Equipment
         */
        public List<EquipmentModel> listEquipmentModel(string area)
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.Equipments
                                 select new EquipmentModel
                                 {
                                     iID = n.iID,
                                     iEquipment_id = n.iEquipment_id,
                                     iArea_id = n.iArea_id,
                                     sDescription = n.sDescription,
                                     sCreated_by = n.sCreated_by,
                                     dtCreated_at = n.dtCreated_at
                                 }).ToList();

                    List<EquipmentModel> result = new List<EquipmentModel>(query);
                    List<EquipmentModel> resultFilter = new List<EquipmentModel>();

                    if(area != "*")
                    {
                        foreach (var item in result)
                        {
                            if (item.iArea_id == area)
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
         * TuffAnalysisResult
         */
        public List<TuffAnalysisResultModel> listTuffAnalysisResultModel(int analyzedId, BrushAnalysisResultModel brushAnalysisResultModel)
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.TuffAnalysisResultSet
                                 select new TuffAnalysisResultModel
                                 {
                                     Id = n.Id,
                                     Position = n.Position,
                                     TotalBristleFoundManual = n.TotalBristleFoundManual,
                                     TotalBristlesFoundNN = n.TotalBristlesFoundNN,
                                     SelectedManual = n.SelectedManual,
                                     AnalyzeSet = n.AnalyzeSet,
                                     Probability = n.Probability
                                 }).ToList();

                    List<TuffAnalysisResultModel> result = new List<TuffAnalysisResultModel>(query);

                    List<TuffAnalysisResultModel> result_ = new List<TuffAnalysisResultModel>();
                    foreach (TuffAnalysisResultModel tuffAnalysisResultModel in result)
                    {                        
                        if(analyzedId == -1)
                        {
                            if (tuffAnalysisResultModel.AnalyzeSet.Id == brushAnalysisResultModel.AnalyzeSet.Id)
                            {
                                result_.Add(tuffAnalysisResultModel);
                            }
                        }
                        else
                        {
                            if (tuffAnalysisResultModel.AnalyzeSet.Id == analyzedId)

                            {
                                result_.Add(tuffAnalysisResultModel);
                            }
                        }                     
                    }

                    return result_;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
       * TuftTempModel
       */
        public List<TuftTempModel> listTuftTempModel()
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.TuftTempSetSet
                                 select new TuftTempModel
                                 {
                                     Id = n.Id,
                                     Position = n.Position
                                 }).ToList();

                    List<TuftTempModel> result = new List<TuftTempModel>(query);

                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /*
         * UserSystem
         */
        public List<UserSystemModel> listUserSystemModel()
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.UserSystems
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        /*
         * Skelta
         * SKU 
         */
        public List<SKU1Model> listSKUsModel()
        {
            try
            {
                using (var ef = new ColgateSkeltaEntities())
                {
                    var query = (from n in ef.SKUs
                                 select new SKU1Model
                                 {
                                     iID = n.iID,
                                     sSKU = n.sSKU,
                                     iArea_id = n.iArea_id,                                    
                                     sDescription = n.sDescription,
                                     dtCreated_at = n.dtCreated_at,
                                     sCreated_by = n.sCreated_by
                                 }).ToList();

                    List<SKU1Model> result = new List<SKU1Model>(query);

                    return result;
                }
            }
            catch (Exception e)
            {                
                Console.WriteLine(e.Message);
                return null;
            }
        }
        #endregion List
    }
}

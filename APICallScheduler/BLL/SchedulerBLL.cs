using APICallScheduler.Common;
using APICallScheduler.DAL;
using APICallScheduler.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;



namespace APICallScheduler.BLL
{
    public class SchedulerBLL
    {
        BaseBLL baseBLL = new BaseBLL();
        #region Tata AIA TeleMER staus push
        SchedulerDAL DAL = new SchedulerDAL();
        public bool funPushTeleMERCaseStatus()
        {
            DataSet ds = DAL.funPushTeleMERCaseStatus(52);
            DataTable dt = ds.Tables[0];
            BaseBLL BBLL = new BaseBLL();
            bool Flag = false;
            try
            {



                List<HAServiceTeleMERStatus> objList = new List<HAServiceTeleMERStatus>();
                foreach (DataRow item in dt.Rows)
                {
                    HAServiceTeleMERStatus objTeleModel = new HAServiceTeleMERStatus();
                    //string endPoint = @"http://202.177.146.141:9082/MedicalExamBatch/TeleMerControl/TeleMerReq/Qx5IW3xeLSHDqNoH74hxCQ==/Dh4cJiSe5yRgAAO2mDMZWw==/tatatpaservice12";

                    string endPoint = @"https://emtrfdatafeed.tataaia.com/MedicalExamBatch/TeleMerControl/TeleMerReq/TBHpLHGmxfkAFCzgWzT7Tw==/MwVNbXXg7CEPUaUIsozkhQ==/tatatpaservice12";
                    TeleMERStatsServicceModel objModel = new TeleMERStatsServicceModel();
                    TeleMERStatsServicceResponseModel objResponse = new TeleMERStatsServicceResponseModel();
                    objModel.TELEMerPolicyno = Convert.ToString(item["PolicyRefNo"]); //"C123456789";
                    objModel.MedicalStdNonStdFlag = Convert.ToString(item["MedicalStandardFlag"]); //"Standard";
                    string data = TATAAiaEncrypt(JsonConvert.SerializeObject(objModel), "tatatpaservice12");
                    var json = string.Empty;
                    var client = new WebService.RestClient("text", endPoint, APICallScheduler.Common.HttpVerb.POST, data);
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            json = client.MakeRequest();
                            break;
                        }
                        catch (Exception ex)
                        {

                            BBLL.uspSaveErrorLog("TeleMErJSON", Convert.ToString(json), "", "Y", "TELEMERScheduler");
                        }
                    }

                    objResponse = JsonConvert.DeserializeObject<TeleMERStatsServicceResponseModel>(json);

                    objTeleModel.TeleProposerId = Convert.ToInt64(item["TeleProposerId"]);
                    objTeleModel.Status = Convert.ToString(item["MedicalStandardFlag"]);
                    objTeleModel.Response = Convert.ToString(objResponse.Status);
                    objTeleModel.SentFlag = Convert.ToString((objResponse.Status).ToLower() == "success" ? "Y" : "N");
                    objTeleModel.CreatedBy = Convert.ToString("System");
                    objTeleModel.Reason = Convert.ToString(objResponse.Reason);
                    objTeleModel.URL = endPoint;
                    objTeleModel.RawResponse = Convert.ToString(json.ToString());
                    objTeleModel.Request = data;
                    objList.Add(objTeleModel);

                }

                DataTable dtSave = new DataTable();
                if (objList.Count > 0)
                {
                    dtSave = BaseBLL.ConvertToDataTable(objList);
                    DAL.SaveHAServiceTeMERStatus(dtSave);
                }
            }
            catch (Exception ex)
            {

                BBLL.uspSaveErrorLog("TeleMErService", ex.Message, ex.ToString(), "Y", "TELEMERScheduler");
            }

            return Flag;
        }

        public string TATAAiaEncrypt(string text, string keyValue)
        {
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            AesManaged tdes = new AesManaged();
            tdes.Key = UTF8.GetBytes(keyValue);
            tdes.IV = UTF8.GetBytes(keyValue);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypt = tdes.CreateEncryptor();
            byte[] plain = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] cipher = crypt.TransformFinalBlock(plain, 0, plain.Length);
            String encryptedText = Convert.ToBase64String(cipher);
            //pk3o5xoVfV09q1fOIsy3wQ==]
            return encryptedText;
        }

        #endregion


        public string TATAAiaEncryptCBC(string text, string keyValue)
        {
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
            AesManaged tdes = new AesManaged();
            tdes.Key = UTF8.GetBytes(keyValue);
            tdes.IV = UTF8.GetBytes(keyValue);
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypt = tdes.CreateEncryptor();
            byte[] plain = UTF8.GetBytes(text);
            byte[] cipher = crypt.TransformFinalBlock(plain, 0, plain.Length);
            String encryptedText = Convert.ToBase64String(cipher);
            encryptedText = HttpUtility.UrlEncode(encryptedText, UTF8);
            //pk3o5xoVfV09q1fOIsy3wQ==]
            return encryptedText;
        }

        public string TATAAiaDecryptCBC(string text, string keyValue)
        {
            String decryptedText = string.Empty;
            try
            {


                System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
                string decryptedData = HttpUtility.UrlDecode(text, UTF8);
                // String decrypted = Convert.ToBase64String(decryptedData);

                AesManaged tdes = new AesManaged();
                tdes.Key = UTF8.GetBytes(keyValue);
                tdes.IV = UTF8.GetBytes(keyValue);
                tdes.Mode = CipherMode.CBC;
                tdes.Padding = PaddingMode.PKCS7;
                ICryptoTransform crypt = tdes.CreateDecryptor();
                byte[] plain = Convert.FromBase64String(decryptedData);
                byte[] cipher = crypt.TransformFinalBlock(plain, 0, plain.Length);
                decryptedText = UTF8.GetString(cipher);

                //pk3o5xoVfV09q1fOIsy3wQ==]
            }
            catch (Exception)
            {

                decryptedText = text;
            }
            return decryptedText;
        }

        public DataTable GetRHIData(decimal? AppointmentId, decimal? AppointmentLogId, decimal? Id, decimal? ReferenceId, string ReferenceType, string WebServiceType, string WebServiceRequest, string WebServiceResponse, string Flag)
        {
            try
            {
                return DAL.GetRHIData(AppointmentId, AppointmentLogId, Id, ReferenceId, ReferenceType, WebServiceType, WebServiceRequest, WebServiceResponse, Flag);
            }
            catch
            {
                throw;
            }
        }

        public List<TalicTeleStatusUpdate> TalicTeleStatusUpdate()
        {
            try
            {
                List<TalicTeleStatusUpdate> List = new List<TalicTeleStatusUpdate>();
                DataTable dt = DAL.GetTalicTeleStatusAPIDetails("STATUSUPDATE");
                List = dt.AsEnumerable().ToList().
                     ConvertAll(x => new TalicTeleStatusUpdate
                     {
                         TeleStatusLogId = x.Field<Int64>("TeleStatusLogId"),
                         TeleProposerId = x.Field<Int64>("TeleProposerId"),
                         applicationId = x.Field<string>("applicationId"),
                         name = x.Field<string>("name"),
                         caseIntimationDate = x.Field<string>("caseIntimationDate"),
                         tpaRefNum = x.Field<string>("tpaRefNum"),
                         firstcallDate = x.Field<string>("firstcallDate"),
                         callStatus = x.Field<string>("callStatus"),
                         finalcallDate = x.Field<string>("finalcallDate"),
                         finalStatus = x.Field<string>("finalStatus"),
                         policyDisposition = x.Field<string>("policyDisposition"),
                         mobileNumDialed = x.Field<string>("mobileNumDialed")

                     });
                return List;
            }
            catch
            {
                throw;
            }
        }

        public DataSet DownloadVideoRecording(long TeleProposerId, string Flag)
        {
            try
            {

                DataSet ds = DAL.DownloadVideoRecording(TeleProposerId, Flag);

                return ds;
            }
            catch
            {
                throw;
            }
        }

        public void SaveRecordingPath(List<VideoDownloadModel> List)
        {
            DAL.SaveVideoDownLoadStatus(List);
        }

        public void SaveComposition(long TeleProposerId, string CompositionId,long VideoLinkId)
        {
            DAL.SaveComposition(TeleProposerId,  CompositionId, VideoLinkId);
        }

        public List<BajajStatusUpdate> BajajStatusUpdateNew()
        {
            try
            {
                List<BajajStatusUpdate> List = new List<BajajStatusUpdate>();
                DataTable dt = DAL.GetBajajApiDetailsNew("STATUSUPDATE");
                List = dt.AsEnumerable().ToList().
                     ConvertAll(x => new BajajStatusUpdate
                     {
                         AppointmentLogId = x.Field<Int64>("AppointmentLogId"),
                         AppointmentId = x.Field<Int64>("AppointmentId"),
                         pScrutiny_no = x.Field<string>("DocketNo"),
                         pAppointmentCode = x.Field<string>("PolicyRefNo"),
                         pCurrentStatus = x.Field<string>("AppointmentStatus"),
                         pMedicalStatus = x.Field<string>("MedicalStatus"),
                         pRemark = x.Field<string>("Remark"),
                         pDateOfRemarks = x.Field<string>("StatusUpdatedDate"),
                         pTimeOfRemarks = x.Field<string>("StatusUpdatedTime"),
                         pApptDate = x.Field<string>("AppointmentDate"),
                         pApptTime = x.Field<string>("AppointmentTime"),
                         pDcName = x.Field<string>("ProviderName"),
                         pDcAddress = x.Field<string>("ProviderAddress"),
                         pDcLocation = x.Field<string>("ProviderLocation"),
                         pDcCity = x.Field<string>("ProviderCity"),
                         pDcState = x.Field<string>("ProviderState"),
                         pDcPincode = x.Field<string>("ProviderPin"),
                         pNablYN = x.Field<string>("ProviderNABLStatus"),
                         pDcContactNo = x.Field<string>("ProviderContactNo"),
                         pDcEmail = x.Field<string>("ProviderEmailId"),
                         pHomeVisit = x.Field<string>("HomeVisit"),
                         pFirstCallDate = x.Field<string>("FirstCallDate"),
                         pFirstCallTime = x.Field<string>("FirstCallTime"),
                         pFinalCallDate = x.Field<string>("LastCallDate"),
                         pFinalCallTime = x.Field<string>("LastCallTime"),
                         pCallbackRequestedByClient = x.Field<string>("CallbackDate"),
                         pReportUploadDate = x.Field<string>("ReportUploadDate"),
                         pFollowUpDate = x.Field<string>("FollowUpDate"),
                         pSecretKey = x.Field<string>("SecretKey"),
                         pTpaName = x.Field<string>("TPA"),
                         pCaseReferredPortal = x.Field<string>("CaseReferredPortal"),
                         pAdditionalTest = x.Field<string>("AdditionalTest"),
                         UserName = x.Field<string>("UserName"),
                         Password = x.Field<string>("Password")
                     });
                return List;
            }
            catch
            {
                throw;
            }
        }

        #region Adde by Wamik HDFCERGO statusUpdate 
        public List<HDFCERGOStatusUpdate> HDFCERGOStatusUpdateNew()
        {
            try
            {
                List<HDFCERGOStatusUpdate> List = new List<HDFCERGOStatusUpdate>();
                DataTable dt = DAL.GetHDFCERGOApiDetailsNew("STATUSUPDATE");

                List = (from DataRow dr in dt.Rows
                        select new HDFCERGOStatusUpdate()
                        {

                            AppointmentLogId = Convert.ToInt32(dr["AppointmentLogId"]),
                            AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                            AppointmentStatusId = Convert.ToInt32(dr["AppointmentStatusId"]),
                            AppointmentCode = dr["PolicyRefNo"].ToString(),
                            CallStatusName = dr["AppointmentStatus"].ToString(),
                            CallRemarks = dr["AppointmentStatus"].ToString(),
                            //DCRemarks = x.Field<string>("Remark"),
                            CallDatetime = dr["StatusUpdatedDate"].ToString(),
                            StatusUpdatedTime = dr["StatusUpdatedTime"].ToString(),
                            AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                            AppointmentTime = Convert.ToDateTime(dr["AppointmentTime"]),
                            CallerUsername = dr["ProviderName"].ToString(),
                            //DCName = x.Field<string>("ProviderName"),
                            // DCAddress2 =x.Field<string>("ProviderAddress"),
                            // DCAddress3=x.Field<string>("ProviderAddress"),

                             InsuredDetailId=Convert.ToInt32(dr["ClientRefId"]),
                              
                            DCAddress1 = dr["ProviderAddress"].ToString(),
                            DCCity = dr["ProviderCity"].ToString(),
                            DCState = dr["ProviderState"].ToString(),
                            DCPincode = dr["ProviderPin"].ToString(),
                            DCContactNumber = dr["ProviderContactNo"].ToString(),
                            DCEmailId = dr["ProviderEmailId"].ToString(),
                            DCUniqueCode = dr["SecretKey"].ToString()

                        }).ToList();

                return List;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
        public List<HDFCERGOStatusUpdate> HDFCERGOStatusUpdateTELEMER()
        {
            try
            {
                List<HDFCERGOStatusUpdate> List = new List<HDFCERGOStatusUpdate>();
                DataTable dt = DAL.GetHDFCERGOStatusUpdateTELEMER("STATUSUPDATE");

                List = (from DataRow dr in dt.Rows
                        select new HDFCERGOStatusUpdate()
                        {

                            AppointmentLogId = Convert.ToInt32(dr["AppointmentLogId"]),
                            AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                           // AppointmentStatusId = Convert.ToInt32(dr["AppointmentStatusId"]),
                            AppointmentCode = dr["PolicyRefNo"].ToString(),
                            CallStatusName = dr["AppointmentStatus"].ToString(),
                            CallRemarks = dr["AppointmentStatus"].ToString(),
                            DCRemarks = dr["Remark"].ToString(),
                            CallDatetime = dr["StatusUpdatedDate"].ToString(),
                            StatusUpdatedTime = dr["StatusUpdatedTime"].ToString(),
                          //  AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                           // AppointmentTime = Convert.ToDateTime(dr["AppointmentTime"]),
                            CallerUsername = dr["name"].ToString(),
                            DCName = dr["name"].ToString(),
                            // DCAddress2 =x.Field<string>("ProviderAddress"),
                            // DCAddress3=x.Field<string>("ProviderAddress"),
                             InsuredDetailId= Convert.ToInt32(dr["ProposerReferenceId"]),
                            
                            DCAddress1 = dr["ProviderAddress"].ToString(),
                            DCCity = dr["ProviderCity"].ToString(),
                            DCState = dr["ProviderState"].ToString(),
                            DCPincode = dr["ProviderPin"].ToString(),
                            DCContactNumber = dr["mobileNumDialed"].ToString(),
                            DCEmailId = dr["ProviderEmailId"].ToString(),
                            DCUniqueCode = dr["SecretKey"].ToString()

                        }).ToList();

                return List;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<BajajReportUpload> BajajReportUploadNew()
        {
            try
            {
                List<BajajReportUpload> List = new List<BajajReportUpload>();
                DataTable dt = DAL.GetBajajApiDetailsNew("REPORTUPLOAD");
                string PrePath = baseBLL.GetSystemCodePath("FILESAVEPATH", "PURGEPATH");
                string TranPath = baseBLL.GetSystemCodePath("FILESAVEPATH", "TELECALLRECORDING");
                List = dt.AsEnumerable().ToList().
                     ConvertAll(x => new BajajReportUpload
                     {
                         ReportAppointmentId = x.Field<Int64>("ReportAppointmentId"),
                         AppointmentId = x.Field<Int64>("AppointmentId"),
                         pScrutinyNo = x.Field<string>("DocketNo"),
                         pAppointmentCode = x.Field<string>("PolicyRefNo"),
                         pFileName = ((x.Field<string>("ReportFlag") == "REPORT") ? PrePath : TranPath) + x.Field<string>("FilePath"),
                         pTpaName = x.Field<string>("TPA"),
                         ReportType = (x.Field<string>("ReportFlag")),
                         ProposerName = (x.Field<string>("ProposerName")),
                         UserName = x.Field<string>("UserName"),
                         Password = x.Field<string>("Password")
                     });
                return List;
            }
            catch
            {
                throw;
            }
        }

        public List<BajajStatusUpdate> BajajTELEStatusUpdateNew()
        {
            try
            {
                List<BajajStatusUpdate> List = new List<BajajStatusUpdate>();
                DataTable dt = DAL.GetBajajTELEApiDetailsNew("STATUSUPDATE");
                List = dt.AsEnumerable().ToList().
                     ConvertAll(x => new BajajStatusUpdate
                     {
                         AppointmentLogId = x.Field<Int64>("AppointmentLogId"),
                         AppointmentId = x.Field<Int64>("AppointmentId"),
                         pScrutiny_no = x.Field<string>("DocketNo"),
                         pAppointmentCode = x.Field<string>("PolicyRefNo"),
                         pCurrentStatus = x.Field<string>("AppointmentStatus"),
                         pMedicalStatus = x.Field<string>("MedicalStatus"),
                         pRemark = x.Field<string>("Remark"),
                         pDateOfRemarks = x.Field<string>("StatusUpdatedDate"),
                         pTimeOfRemarks = x.Field<string>("StatusUpdatedTime"),
                         pApptDate = x.Field<string>("AppointmentDate"),
                         pApptTime = x.Field<string>("AppointmentTime"),
                         pDcName = x.Field<string>("ProviderName"),
                         pDcAddress = x.Field<string>("ProviderAddress"),
                         pDcLocation = x.Field<string>("ProviderLocation"),
                         pDcCity = x.Field<string>("ProviderCity"),
                         pDcState = x.Field<string>("ProviderState"),
                         pDcPincode = x.Field<string>("ProviderPin"),
                         pNablYN = x.Field<string>("ProviderNABLStatus"),
                         pDcContactNo = x.Field<string>("ProviderContactNo"),
                         pDcEmail = x.Field<string>("ProviderEmailId"),
                         pHomeVisit = x.Field<string>("HomeVisit"),
                         pFirstCallDate = x.Field<string>("FirstCallDate"),
                         pFirstCallTime = x.Field<string>("FirstCallTime"),
                         pFinalCallDate = x.Field<string>("LastCallDate"),
                         pFinalCallTime = x.Field<string>("LastCallTime"),
                         pCallbackRequestedByClient = x.Field<string>("CallbackDate"),
                         pReportUploadDate = x.Field<string>("ReportUploadDate"),
                         pFollowUpDate = x.Field<string>("FollowUpDate"),
                         pSecretKey = x.Field<string>("SecretKey"),
                         pTpaName = x.Field<string>("TPA"),
                         pCaseReferredPortal = x.Field<string>("CaseReferredPortal"),
                         pAdditionalTest = x.Field<string>("AdditionalTest"),
                         UserName = x.Field<string>("UserName"),
                         Password = x.Field<string>("Password")
                     });
                return List;
            }
            catch
            {
                throw;
            }
        }

        public List<BajajReportUpload> BajajTELEReportUploadNew()
        {
            try
            {
                List<BajajReportUpload> List = new List<BajajReportUpload>();
                DataTable dt = DAL.GetBajajTELEApiDetailsNew("REPORTUPLOAD");
                string PrePath = baseBLL.GetSystemCodePath("FILESAVEPATH", "PURGEPATH");
                string TranPath = baseBLL.GetSystemCodePath("FILESAVEPATH", "TELECALLRECORDING");
                List = dt.AsEnumerable().ToList().
                     ConvertAll(x => new BajajReportUpload
                     {
                         ReportAppointmentId = x.Field<Int64>("ReportAppointmentId"),
                         AppointmentId = x.Field<Int64>("AppointmentId"),
                         pScrutinyNo = x.Field<string>("DocketNo"),
                         pAppointmentCode = x.Field<string>("PolicyRefNo"),
                         pFileName = ((x.Field<string>("ReportFlag") == "REPORT") ? PrePath : TranPath) + x.Field<string>("FilePath"),
                         pTpaName = x.Field<string>("TPA"),
                         ReportType = (x.Field<string>("ReportFlag")),
                         ProposerName = (x.Field<string>("ProposerName")),
                         UserName = x.Field<string>("UserName"),
                         Password = x.Field<string>("Password")
                     });
                return List;
            }
            catch
            {
                throw;
            }
        }

        #region Added By Dheeraj For LIC Report Upload
        public List<LICReportUpload> LICReportUpload()
        {
            try
            {
                List<LICReportUpload> List = new List<LICReportUpload>();
                DataTable dt = DAL.GetLICApiDetails("REPORTUPLOAD");
                string PrePath = baseBLL.GetSystemCodePath("FILESAVEPATH", "PURGEPATH");
                string TranPath = baseBLL.GetSystemCodePath("FILESAVEPATH", "TELECALLRECORDING");
                List = dt.AsEnumerable().ToList().
                     ConvertAll(x => new LICReportUpload
                     {

                         AppointmentId = x.Field<Int64>("AppointmentId"),
                         ReportAppointmentId = x.Field<Int64>("ReportAppointmentId"),
                         appid = x.Field<string>("appid"),
                         dataclassName = x.Field<string>("dataclassName"),
                         docname = x.Field<string>("docname"),
                         AcknowledgmentNumber = x.Field<string>("AcknowledgmentNumber"),
                         MSPName = x.Field<string>("MSPName"),
                         MSPRegNumber = x.Field<string>("MSPRegNumber"),
                         PrimaryProposalNumber = x.Field<string>("PrimaryProposalNumber"),
                         BOCode = x.Field<string>("BOCode"),
                         DOCode = x.Field<string>("DOCode"),
                         ZOCode = x.Field<string>("ZOCode"),
                         FinYear = x.Field<string>("FinYear"),
                         LAName = x.Field<string>("LAName"),
                         DOB = x.Field<string>("DOB"),
                         pFileName = PrePath + x.Field<string>("FilePath"),
                         DocumentType = x.Field<string>("DocumentType"),
                         DocumentName = x.Field<string>("DocumentName"),
                         DateOfExamination = x.Field<string>("DateOfExamination"),
                         Status = x.Field<string>("Status")

                     });
                return List;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        public DataTable GetTranscriptData(decimal? TeleProposerId, decimal? ClientId, decimal? QueId, string Flag, string FromDate = null, string ToDate = null)
        {
            try
            {
                return DAL.GetTranscriptData(TeleProposerId, ClientId, QueId, Flag, FromDate, ToDate);
            }
            catch
            {
                throw;
            }
        }

        public List<BajajStatusUpdate> BajajStatusUpdateTeleMer()
        {
            try
            {
                List<BajajStatusUpdate> List = new List<BajajStatusUpdate>();
                DataTable dt = DAL.GetBajajApiDetailsTeleMer("STATUSUPDATE");
                List = dt.AsEnumerable().ToList().
                     ConvertAll(x => new BajajStatusUpdate
                     {
                         AppointmentLogId = x.Field<Int64>("AppointmentLogId"),
                         AppointmentId = x.Field<Int64>("AppointmentId"),
                         pScrutiny_no = x.Field<string>("DocketNo"),
                         pAppointmentCode = x.Field<string>("PolicyRefNo"),
                         pCurrentStatus = x.Field<string>("AppointmentStatus"),
                         pRemark = x.Field<string>("Remark"),
                         pDateOfRemarks = x.Field<string>("StatusUpdatedDate"),
                         pTimeOfRemarks = x.Field<string>("StatusUpdatedTime"),
                         pApptDate = x.Field<string>("AppointmentDate"),
                         pApptTime = x.Field<string>("AppointmentTime"),
                         pDcName = x.Field<string>("ProviderName"),
                         pDcAddress = x.Field<string>("ProviderAddress"),
                         pDcLocation = x.Field<string>("ProviderLocation"),
                         pDcCity = x.Field<string>("ProviderCity"),
                         pDcState = x.Field<string>("ProviderState"),
                         pFollowUpDate = x.Field<string>("FollowUpDate"),
                         pHomeVisit = x.Field<string>("HomeVisit"),
                         pSecretKey = x.Field<string>("SecretKey"),
                         pTpaName = x.Field<string>("TPA")
                     });
                return List;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public List<BajajReportUpload> BajajReportUploadTeleMer()
        {
            try
            {
                List<BajajReportUpload> List = new List<BajajReportUpload>();
                DataTable dt = DAL.GetBajajApiDetailsTeleMer("REPORTUPLOAD");
                string PrePath = baseBLL.GetSystemCodePath("FILESAVEPATH", "PURGEPATH");
                string TranPath = baseBLL.GetSystemCodePath("FILESAVEPATH", "TELECALLRECORDING");
                List = dt.AsEnumerable().ToList().
                     ConvertAll(x => new BajajReportUpload
                     {
                         ReportAppointmentId = x.Field<Int64>("ReportAppointmentId"),
                         AppointmentId = x.Field<Int64>("AppointmentId"),
                         pScrutinyNo = x.Field<string>("DocketNo"),
                         pAppointmentCode = x.Field<string>("PolicyRefNo"),
                         pFileName = ((x.Field<string>("ReportFlag") == "REPORT") ? PrePath : TranPath) + x.Field<string>("FilePath"),
                         pTpaName = x.Field<string>("TPA"),
                         ReportType = (x.Field<string>("ReportFlag"))
                     });
                return List;
            }
            catch
            {
                throw;
            }
        }

        public DataSet DownloadVideoRecordingPPHC(long AppointmentId, string Flag)
        {
            try
            {

                DataSet ds = DAL.DownloadVideoRecordingPPHC(AppointmentId, Flag);

                return ds;
            }
            catch
            {
                throw;
            }
        }


        public void SaveRecordingPathPPHC(List<VideoDownloadModel> List)
        {
            DAL.SaveVideoDownLoadStatusPPHC(List);
        }


        public void SaveCompositionPPHC(long AppointmentId, string CompositionId, long VideoLinkId)
        {
            DAL.SaveCompositionPPHC(AppointmentId, CompositionId, VideoLinkId);
        }

        public List<BajajLifeBillingAPI> BajajLifeBillingAPI()
        {
            try
            {
                List<BajajLifeBillingAPI> List = new List<BajajLifeBillingAPI>();
                DataTable dt = DAL.BajajBillingAPI();
                List = dt.AsEnumerable().ToList().
                     ConvertAll(x => new BajajLifeBillingAPI
                     {

                         AppointmentId = x.Field<Int64>("AppointmentId"),
                         applicationNo = x.Field<string>("applicationNo"),
                         finalApplicationNo = x.Field<string>("finalApplicationNo"),
                         clientName = x.Field<string>("clientName"),
                         intimationDate = x.Field<string>("intimationDate"),
                         firstCallToClientDate = x.Field<string>("firstCallToClientDate"),
                         clientCity = x.Field<string>("clientCity"),
                         testCodesBalic = x.Field<string>("testCodesBalic"),
                         testCodesTpa = x.Field<string>("testCodesTpa"),
                         dcCode = x.Field<string>("dcCode"),
                         dcName = x.Field<string>("dcName"),
                         dcCity = x.Field<string>("dcCity"),
                         appointmentDate = x.Field<string>("appointmentDate"),
                         reportUploadDate = x.Field<string>("reportUploadDate"),
                         homeVisit = x.Field<string>("homeVisit"),
                         areaMetroRural = x.Field<string>("areaMetroRural"),
                         medicalFees = x.Field<string>("medicalFees"),
                         homeVisitCharges = x.Field<string>("homeVisitCharges"),
                         serviceFees = x.Field<string>("serviceFees"),
                         digitizationCharges = x.Field<string>("digitizationCharges"),
                         interpretationCharges = x.Field<string>("interpretationCharges"),
                         storageCharges = x.Field<string>("storageCharges"),
                         totalMedicalFeeWithoutGst = x.Field<string>("totalMedicalFeeWithoutGst"),
                         gstCharges = x.Field<string>("gstCharges"),
                         totalMedicalFeeWithGst = x.Field<string>("totalMedicalFeeWithGst"),
                         tatFromMedicalDoneToUploadDate = x.Field<string>("tatFromMedicalDoneToUploadDate"),
                         tatFromIntimationToMedicalDone = x.Field<string>("tatFromIntimationToMedicalDone"),
                         tatFromIntimationToReportUpload = x.Field<string>("tatFromIntimationToReportUpload"),
                         reportType = x.Field<string>("reportType"),
                         billingMonth = x.Field<string>("billingMonth"),
                         merType = x.Field<string>("merType"),
                         doctorName = x.Field<string>("doctorName"),
                         doctorCode = x.Field<string>("doctorCode"),
                         doctorQualification = x.Field<string>("doctorQualification"),
                         tpaName = x.Field<string>("tpaName"),
                         channelName = x.Field<string>("channelName"),
                         mainChannel = x.Field<string>("mainChannel"),
                         policyStatus = x.Field<string>("policyStatus"),
                         rejectionReason = x.Field<string>("rejectionReason"),
                         rejectionFlag = x.Field<string>("rejectionFlag"),
                         rejectionRemark = x.Field<string>("rejectionRemark"),
                         paymentType = x.Field<string>("paymentType"),
                         totalPremiumAmt = x.Field<string>("totalPremiumAmt"),
                         fianlPayoutAmtDedOfMedicalChages = x.Field<string>("fianlPayoutAmtDedOfMedicalChages"),
                         policyRef = x.Field<string>("policyRef"),
                         premiumReceiptNo = x.Field<string>("premiumReceiptNo"),
                         premiumReceiptDate = x.Field<string>("premiumReceiptDate"),
                         channel = x.Field<string>("channel"),
                         changeDescription = x.Field<string>("changeDescription"),
                         productId = x.Field<string>("productId"),
                         productName = x.Field<string>("productName"),
                         adjDate = x.Field<string>("adjDate"),
                         verticalCode = x.Field<string>("verticalCode")

                     });
                return List;
            }
            catch
            {
                throw;
            }
        }

        public List<BALICMISAPI> BajajMISAPI()
        {
            try
            {
                List<BALICMISAPI> List = new List<BALICMISAPI>();
                DataTable dt = DAL.BajajMISAPI();
                List = dt.AsEnumerable().ToList().
                     ConvertAll(x => new BALICMISAPI
                     {

                         AppointmentId = x.Field<Int64>("AppointmentId"),
                         applicationNo = x.Field<string>("applicationNo"),
                         clientName = x.Field<string>("clientName"),
                         clientCity = x.Field<string>("clientCity"),
                         clientAddress = x.Field<string>("clientAddress"),
                         testName = x.Field<string>("testName"),
                         comments = x.Field<string>("comments"),
                         tpaName = x.Field<string>("tpaName"),
                         medicalStatus = x.Field<string>("medicalStatus"),
                         doctorName = x.Field<string>("doctorName"),
                         callAttemptsCount = x.Field<string>("callAttemptsCount"),
                         dataReceivedByTpa = x.Field<string>("dataReceivedByTpa"),
                         updatedDate = x.Field<string>("updatedDate"),
                         medicalDoneDate = x.Field<string>("medicalDoneDate"),
                         reportUploadDate = x.Field<string>("reportUploadDate"),
                         appointmentDate = x.Field<string>("appointmentDate"),
                         visitType = x.Field<string>("visitType"),
                         dcName = x.Field<string>("dcName"),
                         dcCity = x.Field<string>("dcCity"),
                         dcId = x.Field<string>("dcId"),
                         firstCallDate = x.Field<string>("firstCallDate"),
                         lastCallDate = x.Field<string>("lastCallDate"),
                         lastCallRemark = x.Field<string>("lastCallRemark"),
                         lastCallComment = x.Field<string>("lastCallComment"),
                         medicalType = x.Field<string>("medicalType"),
                         pendingReason = x.Field<string>("pendingReason")
                         

                     });
                return List;
            }
            catch
            {
                throw;
            }
        }


        #region Bagic

        public MERFormModel GetBAGICMERAppointments(MERFormModel model, int ClientId, int? Minutes)
        {
            try
            {
                return model = DAL.GetBAGICMERAppointments(model, ClientId, Minutes);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void updateBAGICDigitalDataResponse(long AppointmentId, string Result)
        {
            try
            {
                DAL.updateBAGICDigitalDataResponse(AppointmentId, Result);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void updateHDFCERGODigitalDataResponse(long AppointmentId, string Result)
        {
            try
            {
                DAL.updateHDFCERGODigitalDataResponse(AppointmentId, Result);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void updateBAGICDigitalDataResponseTELE(long AppointmentId, string Result)
        {
            try
            {
                DAL.updateBAGICDigitalDataResponseTELE(AppointmentId, Result);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void updateHDFCERGODigitalDataResponseTELE(long AppointmentId, string Result)
        {
            try
            {
                DAL.updateHDFCERGODigitalDataResponseTELE(AppointmentId, Result);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void SaveServiceRequest(string ClientCode, string UserName, string Password, string ServiceName, string ReferenceId, string ReferenceType, string RequestType, string Request, string ResponseType, string Response, string CreatedBy, string BusinessCorelationId = null)
        {
            try
            {
                DAL.SaveServiceRequest(ClientCode, UserName, Password, ServiceName, ReferenceId, ReferenceType, RequestType, Request, ResponseType, Response, CreatedBy, BusinessCorelationId);
            }
            catch
            {
            }
        }

        #endregion

        #region Aditya BAGIC TELE MER FORM
        public MERFormModel GetBAGICTELEMERAppointments(MERFormModel model, int ClientId, int? Minutes)
        {
            try
            {
                return model = DAL.GetBAGICTELEMERAppointments(model, ClientId, Minutes);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        public MERFormModel GetHDFCERGODigitalMERAppointments(MERFormModel model, int ClientId, int? Minutes)
        {
            try
            {
                return model = DAL.GetHDFCERGODigitalMERAppointments(model, ClientId, Minutes);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public MERFormModel GetHDFCERGOTELEMERAppointments(MERFormModel model, int ClientId, int? Minutes)
        {
            try
            {
                return model = DAL.GetHDFCERGOTELEMERAppointments(model, ClientId, Minutes);
            }
            catch (Exception)
            {

                throw;
            }
        }

        
    }


}
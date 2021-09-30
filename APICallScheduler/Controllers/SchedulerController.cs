using APICallScheduler.BLL;
using APICallScheduler.Common;
using APICallScheduler.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
//using Dapper;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using iTextSharp.text.pdf;
using System.Configuration;
using Twilio;
using Twilio.Rest.Video.V1;
using static Twilio.Rest.Video.V1.CompositionResource;
using System.Web.Script.Serialization;


public enum HttpVerb
{
    GET,
    POST,
    PUT,
    DELETE
}

namespace APICallScheduler.Controllers
{
    public class SchedulerController : Controller
    {
        // GET: Scheduler
        SchedulerBLL SBLL = new SchedulerBLL();
        BaseBLL BBLL = new BaseBLL();
        public ActionResult Index()
        {
            return View();
        }

        #region Tata AIA TeleMER staus push

        public void funPushTeleMERCaseStatus()
        {
            SBLL.funPushTeleMERCaseStatus();


        }

        #endregion


        #region Talic Tele Status push
        public string TeleTALICStatusUpdate()
        {
            try
            {
                // string endPoint = @"https://appsuat.tataaia.com/TPAAPI/services/TeleVmer/update";//Dev Link
                string endPoint = @"https://apps.tataaia.com/TPAAPI/services/TeleVmer/update";//Live Link
                List<TalicTeleStatusUpdate> List = SBLL.TalicTeleStatusUpdate();

                TALICTeleStatsServicceResponseModel objResponse = new TALICTeleStatsServicceResponseModel();
                foreach (var model in List)
                {
                    try
                    {
                       
                           reqInfoM objReqInfo = new reqInfoM();
                        List<TalicTeleAPIStatusUpdate> lstDataModel = new List<TalicTeleAPIStatusUpdate>();
                        TalicTeleAPIStatusUpdate dataModel = new TalicTeleAPIStatusUpdate();
                        dataModel.applicationId = model.applicationId;
                        dataModel.name = model.name;
                        dataModel.caseIntimationDate = model.caseIntimationDate;
                        dataModel.tpaRefNum = model.tpaRefNum;
                        dataModel.firstcallDate = model.firstcallDate;
                        dataModel.callStatus = model.callStatus;
                        dataModel.finalcallDate = model.finalcallDate;
                        dataModel.finalStatus = model.finalStatus;
                        dataModel.policyDisposition = model.policyDisposition;
                        dataModel.mobileNumDialed = model.mobileNumDialed;
                        lstDataModel.Add(dataModel);
                        objReqInfo.reqInfo = lstDataModel;
                        string Request = JsonConvert.SerializeObject(objReqInfo);
                        string data = SBLL.TATAAiaEncryptCBC(Request, "TPATELEVMERHAAPI");
                        List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
                        headers.Add(new KeyValuePair<string, string>("appKey", "HA"));
                        var client = new WebService.RestClient("text/plain", endPoint, APICallScheduler.Common.HttpVerb.POST, data, headers);
                        var json = "";
                        for (int i = 0; i < 5; i++)
                        {
                            try
                            {


                                json = client.MakeRequest();
                                json = SBLL.TATAAiaDecryptCBC(json, "TPATELEVMERHAAPI");
                                objResponse = JsonConvert.DeserializeObject<TALICTeleStatsServicceResponseModel>(json);
                            }
                            catch (Exception ex)
                            {

                                SBLL.GetRHIData(model.TeleProposerId, model.TeleStatusLogId, null, model.TeleStatusLogId, "TELESTATUSID", "TELESTATUSUPDATE", ex.ToString(), ex.Message, "TELEINSERTALL");
                            }
                            if (!String.IsNullOrWhiteSpace(objResponse.status))
                            {
                                if (objResponse.status == "SUCCESS")
                                {
                                    break;
                                }

                                SBLL.GetRHIData(model.TeleProposerId, model.TeleStatusLogId, null, model.TeleStatusLogId, "TELESTATUSID", "TELESTATUSUPDATE", Request, json, "TELEINSERTALL");
                            }


                        }




                        SBLL.GetRHIData(model.TeleProposerId, model.TeleStatusLogId, null, model.TeleStatusLogId, "TELESTATUSID", "TELESTATUSUPDATE", Request, json, "TELEINSERTALL");
                    }
                    catch (Exception ex)
                    {
                        SBLL.GetRHIData(model.TeleProposerId, model.TeleStatusLogId, null, model.TeleStatusLogId, "TELESTATUSID", "TELESTATUSUPDATE", ex.ToString(), ex.Message, "TELEINSERTALL");
                    }
                }
            }
            catch (Exception ex)
            {
                BBLL.uspSaveErrorLog("TalicTeleStatusUpdate", ex.Message, ex.ToString(), "Y", "TELEMERScheduler");
            }
            return null;
        }
        #endregion

        #region Video Download
        public void DownloadVideoRecording()
        {

            long TeleProposerId = 0, ReferenceId = 0;
            string CompositionId, FilePath, FileExtension;

            List<VideoDownloadModel> objVList = new List<VideoDownloadModel>();
            try
            {
                DataSet ds = SBLL.DownloadVideoRecording(0, "");
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TeleProposerId = Convert.ToInt64(dr["TeleProposerId"]);
                        ds = SBLL.DownloadVideoRecording(TeleProposerId, "SINGLE");
                        if (ds.Tables.Count > 0)
                        {
                            foreach (DataRow dr1 in ds.Tables[0].Rows)
                            {
                                VideoDownloadModel objModel = new VideoDownloadModel();
                                try
                                {
                                    CompositionId = Convert.ToString(dr1["CompositionId"]);
                                    FilePath = Convert.ToString(dr1["FilePath"]);
                                    ReferenceId = Convert.ToInt64(dr1["referenceId"]);

                                    objModel.FileType = "VIDEO";
                                    objModel.FileSavePath = Convert.ToString(dr1["DBPath"]); ;
                                    objModel.TeleProposerId = TeleProposerId;
                                    objModel.ReferenceId = ReferenceId;

                                    if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                                    {
                                        Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                                    }

                                    string apiKeySid = "SK79ca89b646645a4a8da4a9de6b23f108";
                                    string apiKeySecret = "lsJjvVLu1WBToQkbdrHDwvY0dGn8mqA2";

                                    try
                                    {
                                        string uri = @"https://video.twilio.com/v1/Compositions/" + CompositionId + "/Media?Ttl=3600";
                                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                        var request = (HttpWebRequest)WebRequest.Create(uri);
                                        request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(apiKeySid + ":" + apiKeySecret)));
                                        request.AllowAutoRedirect = false;




                                        string responseBody = new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();
                                        var mediaLocation = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody)["redirect_to"];
                                        FileExtension = Path.GetExtension(mediaLocation);
                                        Console.WriteLine(mediaLocation);
                                        new WebClient().DownloadFile(mediaLocation, FilePath);
                                        objVList.Add(objModel);
                                    }
                                    catch (Exception ex)
                                    {


                                    }

                                }
                                catch (Exception ex)
                                {


                                }

                            }

                        }
                    }
                }
                SBLL.SaveRecordingPath(objVList);
            }
            catch (Exception ex)
            {

                BBLL.uspSaveErrorLog("TeleVideoDownSch", ex.Message, ex.ToString(), "Y", "TeleVideoDownSch");
            }
        }
        #endregion


        #region Video Download
        public void PushCompositionByRoomId()
        {

            long TeleProposerId = 0, VideoLinkId = 0, i = 0;
            string CompositionId, RoomId;


            try
            {
                DataSet ds = SBLL.DownloadVideoRecording(0, "ROOMLIST1");
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TeleProposerId = Convert.ToInt64(dr["TeleProposerId"]);
                        VideoLinkId = Convert.ToInt64(dr["VideoLinkId"]);
                        RoomId = Convert.ToString(dr["RoomId"]);
                        try
                        {


                            CompositionId = GetCompositionId(RoomId);
                            if (!string.IsNullOrEmpty(CompositionId))
                            {
                                i++;
                                SBLL.SaveComposition(TeleProposerId, CompositionId, VideoLinkId);
                            }
                            if (i >= 2)
                            {
                                break;
                            }

                        }
                        catch (Exception ex)
                        {
                            BBLL.uspSaveErrorLog("TeleVideoDownSch", ex.Message, ex.ToString(), "Y", "TeleVideoDownSch");

                        }
                    }
                }

            }
            catch (Exception ex)
            {

                BBLL.uspSaveErrorLog("TeleVideoDownSch", ex.Message, ex.ToString(), "Y", "TeleVideoDownSch");
            }
        }
        public string GetCompositionId(string RoomId)
        {
            // Find your API Key SID and Secret at twilio.com/console
            const string apiKeySid = "SK575b2769753968a7ac4c9db88edea256";
            const string apiKeySecret = "5KWR4TLI7CYddf1WVaTxESWX4KTtww4H";
            string cid = String.Empty;
            TwilioClient.Init(apiKeySid, apiKeySecret);
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };


                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var layout = new
                {
                    grid = new
                    {
                        video_sources = new string[] { "*" }
                    }
                };

                var composition = CompositionResource.Create(
                  roomSid: RoomId,
                  audioSources: new List<string> { "*" },
                  videoLayout: layout,
                  statusCallback: new Uri("http://my.server.org/callbacks"),
                  format: FormatEnum.Mp4
                );


                cid = composition.Sid;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            return cid;
        }
        #endregion

        public void BajajStatusUpdateNew()
        {
            try
            {



                // string endPoint = @"https://api.bagicsit2.bajajallianz.com/ppc/health/updateTpaStatusWs";
                //string endPoint = @"http://webservicesdev.bajajallianz.com/PrePolicyTPA/updateTpaStatusWs";//Dev Link
                //string endPoint = @"http://webservices.bajajallianz.com/PrePolicyTPA/updateTpaStatusWs";//Live Link
                // string endPoint = @"https://api.bagicuat.bajajallianz.com/ppc/health/updateTpaStatusWs";
                string endPoint = @"https://webapi.bajajallianz.com/ppc/health/updateTpaStatusWs";
                List<BajajStatusUpdate> List = SBLL.BajajStatusUpdateNew();
                foreach (var model in List)
                {
                    try
                    {
                        BajajStatusUpdateApiModel dataModel = new BajajStatusUpdateApiModel();
                        dataModel.pScrutiny_no = model.pScrutiny_no;
                        dataModel.pAppointmentCode = model.pAppointmentCode;
                        dataModel.pCurrentStatus = model.pCurrentStatus;
                        dataModel.pMedicalStatus = model.pMedicalStatus;
                        dataModel.pTpaRemark = dataModel.pRemark = model.pRemark;
                        dataModel.pDateOfRemarks = model.pDateOfRemarks;
                        dataModel.pTimeOfRemarks = model.pTimeOfRemarks;
                        dataModel.pApptDate = model.pApptDate;
                        dataModel.pApptTime = model.pApptTime;
                        dataModel.pDcName = model.pDcName;
                        dataModel.pDcAddress = model.pDcAddress;
                        dataModel.pDcLocation = model.pDcLocation;
                        dataModel.pDcCity = model.pDcCity;
                        dataModel.pDcState = model.pDcState;
                        dataModel.pDcPincode = model.pDcPincode;
                        dataModel.pNablYN = model.pNablYN;
                        dataModel.pDcContactNo = model.pDcContactNo;
                        dataModel.pDcEmail = model.pDcEmail;
                        dataModel.pHomeVisit = model.pHomeVisit;
                        dataModel.pFirstCallDate = model.pFirstCallDate;
                        dataModel.pFirstCallTime = model.pFirstCallTime;
                        dataModel.pFinalCallDate = model.pFinalCallDate;
                        dataModel.pFinalCallTime = model.pFinalCallTime;
                        dataModel.pCallbackRequestedByClient = model.pCallbackRequestedByClient;
                        dataModel.pReportUploadDate = model.pReportUploadDate;
                        dataModel.pSecretKey = model.pSecretKey;
                        dataModel.pFollowUpDate = model.pFollowUpDate;
                        dataModel.pUserName = dataModel.pTpaName = model.pTpaName;
                        dataModel.pCaseReferredPortal = model.pCaseReferredPortal;
                        dataModel.pAdditionalTest = model.pAdditionalTest;
                        string BusinessCorelationId = model.pAppointmentCode + DateTime.Now.ToString("dd-MM-yyyy") + Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());

                        List<KeyValuePair<string, string>> header = new List<KeyValuePair<string, string>>();
                        header.Add(new KeyValuePair<string, string>("username", model.UserName));
                        header.Add(new KeyValuePair<string, string>("password", model.Password));
                        header.Add(new KeyValuePair<string, string>("BusinessCorelationId", BusinessCorelationId)); //"650dc225-cd24-46a9-b417-1812ceaf7c19"));


                        string data = JsonConvert.SerializeObject(dataModel);
                        var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data, header);
                        var json = "";
                        for (int i = 0; i < 5; i++)
                        {
                            try
                            {
                                json = client.MakeRequest();
                                data = data.Replace("'", "\\'");
                                data = "BusinessCorelationId:" + BusinessCorelationId + "  request:" + data;
                                json = json.Replace("'", "\\'");
                                SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "APPOINTMENTLOGID", "STATUSUPDATE", data, json, "INSERTALL");
                                break;
                            }
                            catch (Exception ex)
                            {

                                SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "APPOINTMENTLOGID", "STATUSUPDATE", data, json, "INSERTALL");
                            }
                        }


                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }

        public void BajajReportUploadNew()

        {
            //string endPoint = @"https://api.bagicsit2.bajajallianz.com/ppc/health/updateTpaFile";
            //string endPoint = @"http://webservicesdev.bajajallianz.com/PrePolicyTPA/updateTpaFile";//Dev Link
            //string endPoint = @"https://api.bagicuat.bajajallianz.com/ppc/health/updateTpaFile";
            string endPoint = @"https://webapi.bajajallianz.com/ppc/health/updateTpaFile";
        
            //string endPoint = @"";//Live Link
            List<BajajReportUpload> List = SBLL.BajajReportUploadNew();
            foreach (var model in List)
            {
                try
                {
                    BajajReportUploadApiModel dataModel = new BajajReportUploadApiModel();
                    dataModel.pAppointmentCode = RemoveSpecialCharacterBAGIC(model.pAppointmentCode);
                    dataModel.pScrutinyNo = RemoveSpecialCharacterBAGIC(model.pScrutinyNo);
                    dataModel.pTpaName = RemoveSpecialCharacterBAGIC(model.pTpaName);
                    //model.pFileName = "D://BAGIC0020449.pdf";
                    if (System.IO.File.Exists(model.pFileName))
                    {
                        dataModel.pFileName = model.ProposerName + model.pAppointmentCode + System.IO.Path.GetExtension(model.pFileName);
                        //System.IO.Path.GetFileName(model.pFileName);
                        dataModel.imageBytes = Convert.ToBase64String(System.IO.File.ReadAllBytes(model.pFileName));
                    }
                    string BusinessCorelationId = model.pAppointmentCode + DateTime.Now.ToString("dd-MM-yyyy") + Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());

                    List<KeyValuePair<string, string>> header = new List<KeyValuePair<string, string>>();
                    header.Add(new KeyValuePair<string, string>("username", model.UserName));
                    header.Add(new KeyValuePair<string, string>("password", model.Password));
                    header.Add(new KeyValuePair<string, string>("BusinessCorelationId", BusinessCorelationId)); //"650dc225-cd24-46a9-b417-1812ceaf7c19"));


                    string data = JsonConvert.SerializeObject(dataModel);
                    var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data, header);


                    var json = "";
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            json = client.MakeRequest();
                            data = data.Replace("'", "\\'");
                            data = "BusinessCorelationId:" + BusinessCorelationId + "  request:" + data;
                            json = json.Replace("'", "\\'");
                            SBLL.GetRHIData(model.AppointmentId, null, null, model.ReportAppointmentId, "REPORTAPPOINTMENTID", "REPORTUPLOAD", data, json, "INSERTALL");
                            break;
                        }
                        catch (Exception ex)
                        {
                            SBLL.GetRHIData(model.AppointmentId, null, null, model.ReportAppointmentId, "REPORTAPPOINTMENTID", "REPORTUPLOAD", data, json, "INSERTALL");
                            // SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "APPOINTMENTLOGID", "STATUSUPDATE", data, json, "INSERTALL");
                        }
                    }
                }
                catch
                {
                }
            }
        }


        #region Added By Dheeraj For LIC Report Upload 21-12-2020
        public void LICReportUpload()
        {
            //string endPoint = @"http://webservicesdev.bajajallianz.com/PrePolicyTPA/updateTpaFile";//Dev Link
            string endPoint = @"https://edmsupload.licindia.in/AddDocumentRest/uploaddocument/";//Live Link
            List<LICReportUpload> List = SBLL.LICReportUpload();
            foreach (var model in List)
            {
                try
                {
                    DataClassName getdata = new DataClassName();
                    getdata.AcknowledgmentNumber = model.AcknowledgmentNumber;
                    getdata.MSPName = model.MSPName;
                    getdata.MSPRegNumber = model.MSPRegNumber;
                    getdata.PrimaryProposalNumber = model.PrimaryProposalNumber;
                    getdata.BOCode = model.BOCode;
                    getdata.DOCode = model.DOCode;
                    getdata.ZOCode = model.ZOCode;
                    getdata.FinYear = model.FinYear;
                    getdata.LAName = model.LAName;
                    getdata.DOB = model.DOB;
                    getdata.DocumentType = model.DocumentType;
                    getdata.DocumentName = model.DocumentName;
                    getdata.DateOfExamination = model.DateOfExamination;
                    getdata.Status = model.Status;

                    LICReportUploadApiModel dataModel = new LICReportUploadApiModel();
                    dataModel.dataclassName = model.dataclassName;
                    dataModel.appid = model.appid;

                    if (System.IO.File.Exists(model.pFileName))
                    {
                        dataModel.docname = model.docname; // System.IO.Path.GetFileName(model.pFileName);
                        dataModel.imagevalue = Convert.ToBase64String(System.IO.File.ReadAllBytes(model.pFileName));

                    }

                    dataModel.dataclassProperties = getdata;
                    string data = JsonConvert.SerializeObject(dataModel);
                    var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data);
                    var json = client.MakeRequest();
                    data = data.Replace("'", "\\'");
                    json = json.Replace("'", "\\'");

                    SBLL.GetRHIData(model.AppointmentId, null, null, model.ReportAppointmentId, "REPORTAPPOINTMENTID", "REPORTUPLOAD", data, json, "INSERTALL");
                }
                catch
                {
                }
            }
        }

        #endregion

        public void BajajTELEStatusUpdateNew()
        {
            try
            {



                // string endPoint = @"https://api.bagicsit2.bajajallianz.com/ppc/health/updateTpaStatusWs";
                //string endPoint = @"http://webservicesdev.bajajallianz.com/PrePolicyTPA/updateTpaStatusWs";//Dev Link
                //string endPoint = @"http://webservices.bajajallianz.com/PrePolicyTPA/updateTpaStatusWs";//Live Link
                // string endPoint = @"https://api.bagicuat.bajajallianz.com/ppc/health/updateTpaStatusWs";
                string endPoint = @"https://webapi.bajajallianz.com/ppc/health/updateTpaStatusWs";
                List<BajajStatusUpdate> List = SBLL.BajajTELEStatusUpdateNew();
                foreach (var model in List)
                {
                    try
                    {
                        BajajStatusUpdateApiModel dataModel = new BajajStatusUpdateApiModel();
                        dataModel.pScrutiny_no = model.pScrutiny_no;
                        dataModel.pAppointmentCode = model.pAppointmentCode;
                        dataModel.pCurrentStatus = model.pCurrentStatus;
                        dataModel.pMedicalStatus = model.pMedicalStatus;
                        dataModel.pTpaRemark = dataModel.pRemark = model.pRemark;
                        dataModel.pDateOfRemarks = model.pDateOfRemarks;
                        dataModel.pTimeOfRemarks = model.pTimeOfRemarks;
                        dataModel.pApptDate = model.pApptDate;
                        dataModel.pApptTime = model.pApptTime;
                        dataModel.pDcName = model.pDcName;
                        dataModel.pDcAddress = model.pDcAddress;
                        dataModel.pDcLocation = model.pDcLocation;
                        dataModel.pDcCity = model.pDcCity;
                        dataModel.pDcState = model.pDcState;
                        dataModel.pDcPincode = model.pDcPincode;
                        dataModel.pNablYN = model.pNablYN;
                        dataModel.pDcContactNo = model.pDcContactNo;
                        dataModel.pDcEmail = model.pDcEmail;
                        dataModel.pHomeVisit = model.pHomeVisit;
                        dataModel.pFirstCallDate = model.pFirstCallDate;
                        dataModel.pFirstCallTime = model.pFirstCallTime;
                        dataModel.pFinalCallDate = model.pFinalCallDate;
                        dataModel.pFinalCallTime = model.pFinalCallTime;
                        dataModel.pCallbackRequestedByClient = model.pCallbackRequestedByClient;
                        dataModel.pReportUploadDate = model.pReportUploadDate;
                        dataModel.pSecretKey = model.pSecretKey;
                        dataModel.pFollowUpDate = model.pFollowUpDate;
                        dataModel.pUserName = dataModel.pTpaName = model.pTpaName;
                        dataModel.pCaseReferredPortal = model.pCaseReferredPortal;
                        dataModel.pAdditionalTest = model.pAdditionalTest;
                        string BusinessCorelationId = model.pAppointmentCode + DateTime.Now.ToString("dd-MM-yyyy") + Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());

                        List<KeyValuePair<string, string>> header = new List<KeyValuePair<string, string>>();
                        header.Add(new KeyValuePair<string, string>("username", model.UserName));
                        header.Add(new KeyValuePair<string, string>("password", model.Password));
                        header.Add(new KeyValuePair<string, string>("BusinessCorelationId", BusinessCorelationId)); //"650dc225-cd24-46a9-b417-1812ceaf7c19"));


                        string data = JsonConvert.SerializeObject(dataModel);
                        var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data, header);
                        var json = "";
                        for (int i = 0; i < 5; i++)
                        {
                            try
                            {
                                json = client.MakeRequest();
                                data = data.Replace("'", "\\'");
                                json = json.Replace("'", "\\'");
                                SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "TELESTATUSLOGID", "TELESTATUSUPDATE", data, json, "TELEINSERTALL");
                                break;
                            }
                            catch (Exception ex)
                            {

                                SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "TELESTATUSLOGID", "TELESTATUSUPDATE", data, json, "TELEINSERTALL");
                            }
                        }


                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }

        public void BajajTELEReportUploadNew()

        {
            //string endPoint = @"https://api.bagicsit2.bajajallianz.com/ppc/health/updateTpaFile";
            //string endPoint = @"http://webservicesdev.bajajallianz.com/PrePolicyTPA/updateTpaFile";//Dev Link
            //string endPoint = @"https://api.bagicuat.bajajallianz.com/ppc/health/updateTpaFile";
            string endPoint = @"https://webapi.bajajallianz.com/ppc/health/updateTpaFile";
            //string endPoint = @"";//Live Link
            List<BajajReportUpload> List = SBLL.BajajTELEReportUploadNew();
            foreach (var model in List)
            {
                try
                {
                    BajajReportUploadApiModel dataModel = new BajajReportUploadApiModel();
                    dataModel.pAppointmentCode = RemoveSpecialCharacterBAGIC(model.pAppointmentCode);
                    dataModel.pScrutinyNo = RemoveSpecialCharacterBAGIC(model.pScrutinyNo);
                    dataModel.pTpaName = RemoveSpecialCharacterBAGIC(model.pTpaName);
                    //model.pFileName = "D://BAGIC0020449.pdf";
                    model.pFileName = GenerateBAGICTranscriptForAPI(model.AppointmentId);


                    if (System.IO.File.Exists(model.pFileName))
                    {
                        dataModel.pFileName = model.ProposerName + model.pAppointmentCode + System.IO.Path.GetExtension(model.pFileName);
                        //System.IO.Path.GetFileName(model.pFileName);
                        dataModel.imageBytes = Convert.ToBase64String(System.IO.File.ReadAllBytes(model.pFileName));
                    }
                    string BusinessCorelationId = model.pAppointmentCode + DateTime.Now.ToString("dd-MM-yyyy") + Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());

                    List<KeyValuePair<string, string>> header = new List<KeyValuePair<string, string>>();
                    header.Add(new KeyValuePair<string, string>("username", model.UserName));
                    header.Add(new KeyValuePair<string, string>("password", model.Password));
                    header.Add(new KeyValuePair<string, string>("BusinessCorelationId", BusinessCorelationId)); //"650dc225-cd24-46a9-b417-1812ceaf7c19"));


                    string data = JsonConvert.SerializeObject(dataModel);
                    var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data, header);


                    var json = "";
                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            json = client.MakeRequest();
                            data = data.Replace("'", "\\'");
                            json = json.Replace("'", "\\'");
                            SBLL.GetRHIData(model.AppointmentId, null, null, model.ReportAppointmentId, "TELESTATUSLOGID", "TELETRANSCRIPTUPLOAD", data, json, "TELEINSERTALL");
                            break;
                        }
                        catch (Exception ex)
                        {
                            SBLL.GetRHIData(model.AppointmentId, null, null, model.ReportAppointmentId, "TELESTATUSLOGID", "TELETRANSCRIPTUPLOAD", data, json, "TELEINSERTALL");
                            // SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "APPOINTMENTLOGID", "STATUSUPDATE", data, json, "INSERTALL");
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public string RemoveSpecialCharacterBAGIC(string value)
        {
            string retValue = string.Empty;
            try
            {
                retValue = Regex.Replace(value, "[^0-9a-zA-Z.~@|,:-]+", " ");

            }
            catch (Exception)
            {
                retValue = value;


            }
            return retValue;
        }
        public string RemoveSpecialCharacterHDFCERGO(string value)
        {
            string retValue = string.Empty;
            try
            {
                retValue = Regex.Replace(value, "[^0-9a-zA-Z.~@|,:-]+", " ");

            }
            catch (Exception)
            {
                retValue = value;


            }
            return retValue;
        }
        public string GenerateBAGICTranscriptForAPI(long TeleProposerId)
        {
            string[] filePaths = Directory.GetFiles(BBLL.GetFilePath(@"Files\Transcripts\BAGICTranscript\\PDF\\"));
            //foreach (string filePath in filePaths)
            //System.IO.File.Delete(filePath);

            string retPath = string.Empty;
            try
            {
                Byte[] bytes;
                DataTable TeleData = SBLL.GetTranscriptData(TeleProposerId, null, null, "GETTELEDATA");
                var html = @"<html>
                            <head></head>
                            <body><div style='text-align: right;'>
                            <table border='0' cellpadding='1' cellspacing='1' style='width:100%;'>
                            <tr><td style='text-align: -webkit-left;'></td>
                            <td style='text-align:right;'><img alt='' src='http://live.healthassure.in/Images/logo3.png' style='padding-top: 18px;width: 250px;' /></td></tr>
                            </table>
                            </div>
                            <div style='padding:1%;text-align:left;'><h5><u><b>Personal Information</b></u></h5></div>
                            <div style='padding:1%;'>
                            <table border='1' cellpadding='1' cellspacing='1'>
                            <tr><th>Full Name of the Applicant:</th><th>" + TeleData.Rows[0]["InsuredName"] + "</th></tr>" +
                            "<tr><th>Application No.</th><th>" + TeleData.Rows[0]["PolicyRefNo"] + "</th></tr>" +
                            "<tr><th>Telephone/Mobile no.</th><th>" + TeleData.Rows[0]["MobileNo"] + "</th></tr>" +
                            "<tr><th>Gender:</th><th>" + TeleData.Rows[0]["Gender"] + "</th></tr>" +
                            "<tr><th>Date of Birth:</th><th>" + TeleData.Rows[0]["DOB"] + "</th></tr>" +
                            "<tr><th>Height (cm):</th><th>" + TeleData.Rows[0]["Height"] + "</th></tr>" +
                            "<tr><th>Weight (kg):</th><th>" + TeleData.Rows[0]["Weight"] + "</th></tr>" +
                            "<tr><th>BMI:</th><th>" + TeleData.Rows[0]["BMI"] + "</th></tr>" +
                            "<tr><th>Qualification:</th><th>" + TeleData.Rows[0]["Qualification"] + "</th></tr>" +
                            "<tr><th>Occupation:</th><th>" + TeleData.Rows[0]["Occupation"] + "</th></tr>" +
                            "<tr><th>Calling Date:</th><th>" + TeleData.Rows[0]["ModifiedDateTime"] + "</th></tr>" +
                            @"</table>
                            </div>
                            <div style='padding:1%;text-align:left;'><h5><u><b>Medical History</b></u></h5></div>
                            <div style='padding:1%;'>
                            <table border='1' cellpadding='1' cellspacing='1'  style='font-size:12px;width:100%;'>
                            <tr><th style='text-align: center;'></th><th style='text-align: center;'></th><th style='text-align: center;'></th><th style='text-align: center;'></th></th><th style='text-align: center;'></th></th><th style='text-align: center;'></th></tr>
                            <tr><th style='text-align: center;width: 35px;'><b>Sr No.</b></th><th colspan='2' style='text-align: center;'><b>Question</b></th><th style='text-align: center;width: 50px;'><b>Yes/No</b></th><th colspan='2' style='text-align: center;'><b>Details</b></th></tr>";
                DataTable MainQueData = SBLL.GetTranscriptData(TeleProposerId, 55, null, "TELEMAINQUEDATA");
                int i = 1;
                foreach (DataRow MainQue in MainQueData.Rows)
                {
                    html += "<tr><td style='text-align: center;'>" + i + "</td><td colspan='2'>" + MainQue["Question"] + "</td><td style='text-align: center;'>" + MainQue["Answer"] + "</td><td colspan='2'>";
                    if (Convert.ToString(MainQue["Answer"]) == "Yes" && Convert.ToString(MainQue["HasSubQue"]) == "Y")
                    {
                        DataTable SubQueData = SBLL.GetTranscriptData(TeleProposerId, null, decimal.Parse(MainQue["QueId"].ToString()), "TELESUBQUEDATA");
                        int j = 1;
                        foreach (DataRow SubQue in SubQueData.Rows)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(SubQue["Answer"])))
                            {
                                html += "<div>" + j + ". " + SubQue["Question"] + "</div>";
                                html += "<div>-  " + SubQue["Answer"] + "</div>";
                                j++;
                            }
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(MainQue["Remark"])))
                            html += "<div>Remark - " + MainQue["Remark"] + "</div>";
                        html += "</td></tr>";
                    }
                    else if (Convert.ToString(MainQue["Answer"]) == "Yes" && Convert.ToString(MainQue["HasSubQue"]) == "N")
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(MainQue["Remark"])))
                            html += "<div>Remark - " + MainQue["Remark"] + "</div>";
                        html += "</td></tr>";
                    }
                    else if (Convert.ToString(MainQue["Answer"]) == "No" && !string.IsNullOrEmpty(Convert.ToString(MainQue["Remark"])))
                    {
                        html += "<div>Remark - " + MainQue["Remark"] + "</div>";
                        html += "</td></tr>";
                    }
                    else
                    {
                        html += "</td></tr>";
                    }
                    i++;
                }

                html += "</table></div>";

                html += @"<div style='padding:1%;'>
                                        <table border='1' cellpadding='1' cellspacing='1'  style='font-size:12px;width:30%;'>
                                        <tr>
                                        <td style='text-align: center;'><b>Tele-MER Done By:</b></td>
                                        <td style='text-align: center;'><b>" + TeleData.Rows[0]["CreatedBy"] + @"</b></td>
                                        </tr>
                                        <tr>
                                        <td style='text-align: center;'><b>Doctor's Registration No:</b></td>
                                        <td style='text-align: center;'><b>" + TeleData.Rows[0]["RegistrationNo"] + @"</b></td>
                                        </tr>";


                html += @"</table></div></body></html>";
                using (var ms = new MemoryStream())
                {
                    try
                    {
                        using (var doc = new iTextSharp.text.Document())
                        {
                            using (var writer = PdfWriter.GetInstance(doc, ms))
                            {
                                doc.Open();
                                using (var htmlWorker = new iTextSharp.text.html.simpleparser.HTMLWorker(doc))
                                {
                                    using (var sr = new StringReader(html))
                                    {
                                        htmlWorker.Parse(sr);
                                    }
                                }
                                doc.Close();
                            }
                        }
                        bytes = ms.ToArray();
                        string DestinationPath = BBLL.GetFilePath(@"Files\Transcripts\BAGICTranscript\\PDF\\");
                        string FileName = TeleData.Rows[0]["ProposalNo"] + ".pdf";
                        var FinalPath = Path.Combine(DestinationPath, FileName);
                        System.IO.File.WriteAllBytes(FinalPath, bytes);
                        retPath = FinalPath;
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
            return retPath;

        }

        public void BajajStatusUpdateTeleMer()
        {
            try
            {
                // string endPoint = @"http://webservicesdev.bajajallianz.com/PrePolicyTPA/updateTpaStatusWs";//Dev Link
                string endPoint = @"http://webservices.bajajallianz.com/PrePolicyTPA/updateTpaStatusWs";//Live Link
                List<BajajStatusUpdate> List = SBLL.BajajStatusUpdateTeleMer();
                foreach (var model in List)
                {
                    try
                    {
                        BajajStatusUpdateApiModelTeleMER dataModel = new BajajStatusUpdateApiModelTeleMER();
                        dataModel.pScrutiny_no = RemoveSpecialCharacterBAGIC(model.pScrutiny_no);
                        dataModel.pAppointmentCode = RemoveSpecialCharacterBAGIC(model.pAppointmentCode);
                        dataModel.pCurrentStatus = RemoveSpecialCharacterBAGIC(model.pCurrentStatus);
                        dataModel.pRemark = RemoveSpecialCharacterBAGIC(model.pRemark);
                        dataModel.pDateOfRemarks = RemoveSpecialCharacterBAGIC(model.pDateOfRemarks);
                        dataModel.pTimeOfRemarks = RemoveSpecialCharacterBAGIC(model.pTimeOfRemarks);
                        dataModel.pApptDate = RemoveSpecialCharacterBAGIC(model.pApptDate);
                        dataModel.pApptTime = RemoveSpecialCharacterBAGIC(model.pApptTime);
                        dataModel.pDcName = RemoveSpecialCharacterBAGIC(model.pDcName);
                        dataModel.pDcAddress = RemoveSpecialCharacterBAGIC(model.pDcAddress);
                        dataModel.pDcLocation = RemoveSpecialCharacterBAGIC(model.pDcLocation);
                        dataModel.pDcCity = RemoveSpecialCharacterBAGIC(model.pDcCity);
                        dataModel.pDcState = RemoveSpecialCharacterBAGIC(model.pDcState);
                        dataModel.pFollowUpDate = RemoveSpecialCharacterBAGIC(model.pFollowUpDate);
                        dataModel.pHomeVisit = RemoveSpecialCharacterBAGIC(model.pHomeVisit);
                        dataModel.pSecretKey = RemoveSpecialCharacterBAGIC(model.pSecretKey);
                        dataModel.pTpaName = RemoveSpecialCharacterBAGIC(model.pTpaName);
                        string data = JsonConvert.SerializeObject(dataModel);
                        var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data);
                        var json = string.Empty;
                        for (int i = 0; i < 5; i++)
                        {
                            try
                            {


                                json = client.MakeRequest();
                                data = data.Replace("'", "\\'");
                                json = json.Replace("'", "\\'");
                                SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "TELESTATUSLOGID", "TELESTATUSUPDATE", data, json, "TELEINSERTALL");


                                break;
                            }
                            catch (Exception ex)
                            {

                                SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "TELESTATUSLOGID", "TELESTATUSUPDATE", data, json, "TELEINSERTALL");

                            }
                        }
                        // BLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "TELESTATUSLOGID", "TELESTATUSUPDATE", data, json, "INSERTALL");
                    }
                    catch (Exception ex)
                    {
                        SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "TELESTATUSLOGID", "TELESTATUSUPDATE", ex.Message, ex.Message, "TELEINSERTALL");

                    }
                }
            }
            catch
            {
            }
        }

        public void BajajReportUploadTeleMer()
        {
            // string endPoint = @"http://webservicesdev.bajajallianz.com/PrePolicyTPA/updateTpaFile";//Dev Link
            string endPoint = @"http://webservices.bajajallianz.com/PrePolicyTPA/updateTpaFile";//Live Link
            List<BajajReportUpload> List = SBLL.BajajReportUploadTeleMer();
            foreach (var model in List)
            {
                try
                {

                    BajajReportUploadApiModelTeleER dataModel = new BajajReportUploadApiModelTeleER();
                    dataModel.pAppointmentCode = RemoveSpecialCharacterBAGIC(model.pAppointmentCode);
                    dataModel.pScrutinyNo = RemoveSpecialCharacterBAGIC(model.pScrutinyNo);
                    dataModel.pTpaName = RemoveSpecialCharacterBAGIC(model.pTpaName);
                    model.pFileName = GenerateBAGICTranscriptForAPI(model.AppointmentId);
                    if (System.IO.File.Exists(model.pFileName))
                    {
                        dataModel.pFileName = System.IO.Path.GetFileName(model.pFileName);
                        dataModel.imageBytes = Convert.ToBase64String(System.IO.File.ReadAllBytes(model.pFileName));
                    }
                    string data = JsonConvert.SerializeObject(dataModel);
                    var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data);
                    var json = client.MakeRequest();
                    data = data.Replace("'", "\\'");
                    json = json.Replace("'", "\\'");
                    string Operation = model.ReportType == "REPORT" ? "INSERTALL" : "TELEINSERTALL";
                    string type = model.ReportType == "REPORT" ? "REPORTUPLOAD" : "TRANSCRIPTUPLOAD";
                    SBLL.GetRHIData(model.AppointmentId, null, null, model.ReportAppointmentId, "TELETRANSCRIPTUPLOAD", type, data, json, Operation);
                }
                catch
                {
                }
            }
        }


        #region Video Download PPHC aditya
        public void DownloadVideoRecordingPPHC()
        {

            long AppointmentId = 0, ReferenceId = 0;
            string CompositionId, FilePath, FileExtension;

            List<VideoDownloadModel> objVList = new List<VideoDownloadModel>();
            try
            {
                DataSet ds = SBLL.DownloadVideoRecordingPPHC(0, "");
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        AppointmentId = Convert.ToInt64(dr["AppointmentId"]);
                        ds = SBLL.DownloadVideoRecordingPPHC(AppointmentId, "SINGLE");
                        if (ds.Tables.Count > 0)
                        {
                            foreach (DataRow dr1 in ds.Tables[0].Rows)
                            {
                                VideoDownloadModel objModel = new VideoDownloadModel();

                                CompositionId = Convert.ToString(dr1["CompositionId"]);
                                FilePath = Convert.ToString(dr1["FilePath"]);
                                ReferenceId = Convert.ToInt64(dr1["referenceId"]);

                                objModel.FileType = "VIDEO";
                                objModel.FileSavePath = Convert.ToString(dr1["DBPath"]); ;
                                objModel.TeleProposerId = AppointmentId;
                                objModel.ReferenceId = ReferenceId;

                                if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                                }
                                try
                                {
                                    string apiKeySid = "SK79ca89b646645a4a8da4a9de6b23f108";
                                    string apiKeySecret = "lsJjvVLu1WBToQkbdrHDwvY0dGn8mqA2";


                                    string uri = @"https://video.twilio.com/v1/Compositions/" + CompositionId + "/Media?Ttl=3600";
                                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                    var request = (HttpWebRequest)WebRequest.Create(uri);
                                    request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(apiKeySid + ":" + apiKeySecret)));
                                    request.AllowAutoRedirect = false;

                                    try
                                    {


                                        string responseBody = new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();
                                        var mediaLocation = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody)["redirect_to"];
                                        FileExtension = Path.GetExtension(mediaLocation);
                                        Console.WriteLine(mediaLocation);
                                        new WebClient().DownloadFile(mediaLocation, FilePath);
                                        objVList.Add(objModel);
                                    }
                                    catch (Exception ex)
                                    {
                                       

                                    }

                                }
                                catch (Exception ex)
                                {
                                   

                                }

                            }

                        }
                    }
                }
                SBLL.SaveRecordingPathPPHC(objVList);
            }
            catch (Exception ex)
            {

                BBLL.uspSaveErrorLog("PPHCVideoDownSch", ex.Message, ex.ToString(), "Y", "PPHCVideoDownSch");
            }
        }
        #endregion


        #region Video Download
        public void PushCompositionByRoomIdPPHC()
        {

            long AppointmentId = 0, VideoLinkId = 0, i = 0;
            string CompositionId, RoomId;


            try
            {
                DataSet ds = SBLL.DownloadVideoRecordingPPHC(0, "ROOMLIST");
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        AppointmentId = Convert.ToInt64(dr["AppointmentId"]);
                        VideoLinkId = Convert.ToInt64(dr["VideoLinkId"]);
                        RoomId = Convert.ToString(dr["RoomId"]);
                        try
                        {


                            CompositionId = GetCompositionId(RoomId);
                            if (!string.IsNullOrEmpty(CompositionId))
                            {
                                i++;
                                SBLL.SaveCompositionPPHC(AppointmentId, CompositionId, VideoLinkId);
                            }
                            if (i >= 2)
                            {
                                break;
                            }

                        }
                        catch (Exception ex)
                        {
                            BBLL.uspSaveErrorLog("PPHCVideoDownSch", ex.Message, ex.ToString(), "Y", "PPHCVideoDownSch");

                        }
                    }
                }

            }
            catch (Exception ex)
            {

                BBLL.uspSaveErrorLog("PPHCVideoDownSch", ex.Message, ex.ToString(), "Y", "PPHCVideoDownSch");
            }
        }
        //public void PushCompositionByRoomIdPPHC()
        //{

        //    long AppointmentId = 0, VideoLinkId = 0;
        //    string CompositionId, RoomId;


        //    try
        //    {
        //        DataSet ds = SBLL.DownloadVideoRecordingPPHC(0, "ROOMLIST");
        //        if (ds.Tables.Count > 0)
        //        {
        //            foreach (DataRow dr in ds.Tables[0].Rows)
        //            {
        //                AppointmentId = Convert.ToInt64(dr["AppointmentId"]);
        //                VideoLinkId = Convert.ToInt64(dr["VideoLinkId"]);
        //                RoomId = Convert.ToString(dr["RoomId"]);
        //                CompositionId = GetCompositionId(RoomId);
        //                SBLL.SaveCompositionPPHC(AppointmentId, CompositionId, VideoLinkId);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        BBLL.uspSaveErrorLog("PPHCVideoDownSch", ex.Message, ex.ToString(), "Y", "PPHCVideoDownSch");
        //    }
        //}
        #endregion

        public void BajajLifeBillingAPI()
        {
            try
            {
           
                  //string endPoint = @"https://balicuat.bajajallianz.com/MednetWS/api/autoNbRefundMedicalCase";//Dev Link
                string endPoint = @"https://webportal.bajajallianz.com/MednetWS/api/autoNbRefundMedicalCase";//Live Link
                
                List<BajajLifeBillingAPI> List = SBLL.BajajLifeBillingAPI();

                foreach (var model in List)
                {
                    try
                    {
                        BajajLifeBillingAPIModel dataModel = new BajajLifeBillingAPIModel();
                        List<BajajLifeBillingAPIModel> BajajLifeBillingAPIModelList = new List<BajajLifeBillingAPIModel>();
                        BajajLifeBillingAPIModelList FinalList = new BajajLifeBillingAPIModelList();
                        dataModel.applicationNo = model.applicationNo;
                        dataModel.finalApplicationNo = model.finalApplicationNo;
                        dataModel.clientName = model.clientName;
                        dataModel.intimationDate = model.intimationDate;
                        dataModel.firstCallToClientDate = model.firstCallToClientDate;
                        dataModel.clientCity = model.clientCity;
                        dataModel.testCodesBalic = model.testCodesBalic;
                        dataModel.testCodesTpa = model.testCodesTpa;
                        dataModel.dcCode = model.dcCode;
                        dataModel.dcName = model.dcName;
                        dataModel.dcCity = model.dcCity;
                        dataModel.appointmentDate = model.appointmentDate;
                        dataModel.reportUploadDate = model.reportUploadDate;
                        dataModel.homeVisit = model.homeVisit;
                        dataModel.areaMetroRural = model.areaMetroRural;
                        dataModel.medicalFees = model.medicalFees;
                        dataModel.homeVisitCharges = model.homeVisitCharges;
                        dataModel.serviceFees = model.serviceFees;
                        dataModel.digitizationCharges = model.digitizationCharges;
                        dataModel.interpretationCharges = model.interpretationCharges;
                        dataModel.storageCharges = model.storageCharges;
                        dataModel.totalMedicalFeeWithoutGst = model.totalMedicalFeeWithoutGst;
                        dataModel.gstCharges = model.gstCharges;
                        dataModel.totalMedicalFeeWithGst = model.totalMedicalFeeWithGst;
                        dataModel.tatFromMedicalDoneToUploadDate = model.tatFromMedicalDoneToUploadDate;
                        dataModel.tatFromIntimationToMedicalDone = model.tatFromIntimationToMedicalDone;
                        dataModel.tatFromIntimationToReportUpload = model.tatFromIntimationToReportUpload;
                        dataModel.reportType = model.reportType;
                        dataModel.billingMonth = model.billingMonth;
                        dataModel.merType = model.merType;
                        dataModel.doctorName = model.doctorName;
                        dataModel.doctorCode = model.doctorCode;
                        dataModel.doctorQualification = model.doctorQualification;
                        dataModel.tpaName = model.tpaName;
                        dataModel.channelName = model.channelName;
                        dataModel.mainChannel = model.mainChannel;
                        dataModel.policyStatus = model.policyStatus;
                        dataModel.rejectionReason = model.rejectionReason;
                        dataModel.rejectionFlag = model.rejectionFlag;
                        dataModel.rejectionRemark = model.rejectionRemark;
                        dataModel.paymentType = model.paymentType;
                        dataModel.totalPremiumAmt = model.totalPremiumAmt;
                        dataModel.fianlPayoutAmtDedOfMedicalChages = model.fianlPayoutAmtDedOfMedicalChages;
                        dataModel.policyRef = model.policyRef;
                        dataModel.premiumReceiptNo = model.premiumReceiptNo;
                        dataModel.premiumReceiptDate = model.premiumReceiptDate;
                        dataModel.channel = model.channel;
                        dataModel.changeDescription = model.changeDescription;
                        dataModel.productId = model.productId;
                        dataModel.productName = model.productName;
                        dataModel.adjDate = model.adjDate;
                        dataModel.verticalCode = model.verticalCode;
                        BajajLifeBillingAPIModelList.Add(dataModel);

                        FinalList.autoNbRefundMedicalList = BajajLifeBillingAPIModelList;
                        string data = JsonConvert.SerializeObject(FinalList);
                        var json = string.Empty;
                        var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data);
                        for (int i = 0; i < 5; i++)
                        {
                            try
                            {


                                json = client.MakeRequest();
                                data = data.Replace("'", "\\'");
                                json = json.Replace("'", "\\'");
                                SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentId, "AppointmentId", "BILLINGUPDATE", data, json, "INSERTALL");


                                break;
                            }
                            catch (Exception ex)
                            {

                                SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentId, "AppointmentId", "BILLINGUPDATE", data, json, "INSERTALL");

                            }
                        }

                        //SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentId, "AppointmentId", "BILLINGUPDATE", data, json, "INSERTALL");
                    }
                    catch
                    {
                    }
                }
                //FinalList.autoNbRefundMedicalList = BajajLifeBillingAPIModelList;
                //string data = JsonConvert.SerializeObject(FinalList);
                //var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data);
                //var json = client.MakeRequest();
                //data = data.Replace("'", "\\'");
                //json = json.Replace("'", "\\'");
                //BLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "APPOINTMENTLOGID", "STATUSUPDATE", data, json, "INSERTALL");
            }
            catch (Exception ex)
            {

            }
        }


        //Development Of BALICMISAPI -- Aditya - 30 apr 2021

        public void BajajMISAPI()
        {
            try
            {
                 string endPoint = @"https://balicuat.bajajallianz.com/MednetWS/api/saveMISData"; //Dev Link
                //string endPoint = @"https://webportal.bajajallianz.com/MednetWS/api/saveMISData"; //Live Link
                List<BALICMISAPI> List = SBLL.BajajMISAPI();
               // List<BALICMISAPIMODEL> BALICMISAPIMODELList = new List<BALICMISAPIMODEL>();
                BALICMISAPIMODELList FinalList = new BALICMISAPIMODELList();
                foreach (var model in List)
                {
                    try
                    {
                        BALICMISAPIMODEL dataModel = new BALICMISAPIMODEL();
                       
                        dataModel.applicationNo = model.applicationNo;
                        dataModel.clientName = model.clientName;
                        dataModel.clientCity = model.clientCity;
                        dataModel.clientAddress = model.clientAddress;
                        dataModel.testName = model.testName;
                        dataModel.comments = model.comments;
                        dataModel.tpaName = model.tpaName;
                        dataModel.medicalStatus = model.medicalStatus;
                        dataModel.doctorName = model.doctorName;
                        dataModel.callAttemptsCount = model.callAttemptsCount;
                        dataModel.dataReceivedByTpa = model.dataReceivedByTpa;
                        dataModel.updatedDate = model.updatedDate;
                        dataModel.medicalDoneDate = model.medicalDoneDate;
                        dataModel.reportUploadDate = model.reportUploadDate;
                        dataModel.appointmentDate = model.appointmentDate;
                        dataModel.visitType = model.visitType;
                        dataModel.dcName = model.dcName;
                        dataModel.dcCity = model.dcCity;
                        dataModel.dcId = model.dcId;
                        dataModel.firstCallDate = model.firstCallDate;
                        dataModel.lastCallDate = model.lastCallDate;
                        dataModel.lastCallRemark = model.lastCallRemark;
                        dataModel.lastCallComment = model.lastCallComment;
                        dataModel.medicalType = model.medicalType;
                        dataModel.pendingReason = model.pendingReason;

                        FinalList.misDataList.Add(dataModel);

                         }
                    catch
                    {
                    }
                }

               // FinalList.misDataList = BALICMISAPIMODELList;
                string data = JsonConvert.SerializeObject(FinalList);
                var json = string.Empty;
                var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data);
                int No = 1;
            ADI:
                try
                {
                   
                    json = client.MakeRequest();
                    data = data.Replace("'", "\\'");
                    json = json.Replace("'", "\\'");
                    SBLL.GetRHIData(null, null, null, null, "AppointmentId", "BALICMISAPIList", data, json, "INSERTALL");

                }
                catch(Exception ex)
                {
                    No++;
                    if(No<=3)
                    {
                        goto ADI;
                    }

                }
                //FinalList.autoNbRefundMedicalList = BajajLifeBillingAPIModelList;
                //string data = JsonConvert.SerializeObject(FinalList);
                //var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data);
                //var json = client.MakeRequest();
                //data = data.Replace("'", "\\'");
                //json = json.Replace("'", "\\'");
                //BLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "APPOINTMENTLOGID", "STATUSUPDATE", data, json, "INSERTALL");
            }
            catch (Exception ex)
            {

            }
        }


        //added by aditya on 19 july 2021 (Production)

        public void BAGICMERForm()
        {
            string data = string.Empty;
            string json = string.Empty;
            //AppointmentQueryBLL AptBLL = new AppointmentQueryBLL();
            BaseBLL baseBLL = new BaseBLL();
            string AppointmentId = "";
            string BusinessCorelationId = "";
            try
            {
                int ClientId = 55;
                int Minutes = 1000;

                // string endPoint = @"https://api.bagicuat.bajajallianz.com/ppc/health/saveDigitalDataPPC"; //@"https://api.bagicsit2.bajajallianz.com/ppc/health/saveDigitalDataPPC";
                string endPoint = @"https://webapi.bajajallianz.com/ppc/health/saveDigitalDataPPC";
                PPMCBAGICDigitalInformation PPMCBAGICDigitalInformation = new PPMCBAGICDigitalInformation();

                MERFormModel model = new MERFormModel();

                model = SBLL.GetBAGICMERAppointments(model, ClientId, Minutes);


                foreach (var apptdetails in model.appointmentlst)
                {

                    AppointmentId = apptdetails.AppointmentId.ToString();

                    PPMCBAGICDigitalInformation.testDetails = new List<TestDetail>();
                    PPMCBAGICDigitalInformation.appointmentId = apptdetails.MemberId;//AppointmentId; //"3250000111";

                    PPMCBAGICDigitalInformation.membercode = null;//apptdetails.MemberId;
                    PPMCBAGICDigitalInformation.appointmentDate = Convert.ToString(apptdetails.AppointmentDateTime.Date.ToString("dd-MM-yyyy"));
                    PPMCBAGICDigitalInformation.dcaddress = apptdetails.dcaddress;
                    PPMCBAGICDigitalInformation.destinationSystem = apptdetails.destinationSystem;

                    PPMCBAGICDigitalInformation.membername = apptdetails.membername; //model.feedbackMERQuestionAnswerlst.Where(m => m.MainQueClientCode == "Name" && m.AppointmentId == apptdetails.AppointmentId).Select(m => m.Text).SingleOrDefault(); //apptdetails.Name;
                    PPMCBAGICDigitalInformation.gender = apptdetails.Gender;// model.feedbackMERQuestionAnswerlst.Where(m => m.MainQueClientCode == "Gender" && m.AppointmentId == apptdetails.AppointmentId).Select(m => m.FeedbackOptions).SingleOrDefault();// apptdetails.Gender;
                    PPMCBAGICDigitalInformation.isHNI = null;
                    PPMCBAGICDigitalInformation.placeOfService = apptdetails.placeOfService; // model.feedbackMERQuestionAnswerlst.Where(m => m.MainQueClientCode == "Place" && m.AppointmentId == apptdetails.AppointmentId).Select(m => m.Text).SingleOrDefault();// apptdetails.Place;
                    PPMCBAGICDigitalInformation.vendorCode = apptdetails.vendorCode;
                    PPMCBAGICDigitalInformation.testtype = apptdetails.testtype;
                    PPMCBAGICDigitalInformation.proposalNumber = apptdetails.DocketNo;
                    PPMCBAGICDigitalInformation.sourceSystem = apptdetails.sourceSystem;
                    List<TestDetail> LstTestDetails = new List<TestDetail>();
                    List<CtmtReportDetail> LstctmtReportDetails = new List<CtmtReportDetail>();

                    List<Testresult> Lsttestresults = new List<Testresult>();
                    TestDetail SingleTestDetails = new TestDetail();
                    foreach (var item in model.AppointmentTestDetails.Where(m => m.AppointmentId == apptdetails.AppointmentId))
                    {

                        SingleTestDetails = new TestDetail();
                        SingleTestDetails.remarks = item.remarks;
                        SingleTestDetails.status = item.status;
                        SingleTestDetails.testcategory = item.testcategory;
                        SingleTestDetails.testId = item.testId;
                        #region TestValues
                        foreach (var testResult in model.AppointmentTestvalueslst.Where(m => m.Appointmentid == apptdetails.AppointmentId && m.referenceId == item.referenceId))
                        {
                            Testresult testResultModel = new Testresult();
                            testResultModel.documentUrl = testResult.documentUrl;
                            testResultModel.externalId = testResult.externalId;
                            testResultModel.min_range = testResult.MinValue;
                            testResultModel.max_range = testResult.MaxValue;
                            testResultModel.unitOfMeasurement = testResult.Unit;
                            testResultModel.remarks = testResult.Remark;
                            testResultModel.ranges = testResult.ranges;
                            testResultModel.recommendation = testResult.recommendation;
                            testResultModel.loincCode = testResult.loincCode;
                            testResultModel.limit = testResult.limit;
                            testResultModel.testtype = testResult.testtype;
                            testResultModel.testcomponent = testResult.testcomponent;
                            testResultModel.resultValue = testResult.resultValue;
                            testResultModel.testPerformedDate = testResult.testPerformedDate;
                            testResultModel.testResultNotes = testResult.testResultNotes;
                            testResultModel.testResultNotes = testResult.testResultNotes;
                            Lsttestresults.Add(testResultModel);
                        }

                        #endregion

                        #region ctmtReportDetails
                        foreach (var ReportResult in model.AptCtmtReportDetaillst.Where(m => m.AppointmentId == apptdetails.AppointmentId && m.referenceId == item.referenceId))
                        {
                            CtmtReportDetail ReportResultModel = new CtmtReportDetail();
                            ReportResultModel.grade = ReportResult.grade;
                            ReportResultModel.phase = ReportResult.phase;
                            ReportResultModel.remarks = ReportResult.remarks;
                            ReportResultModel.speed = ReportResult.speed;
                            ReportResultModel.stage = ReportResult.stage;
                            ReportResultModel.testName = ReportResult.testName;
                            ReportResultModel.time = ReportResult.time;
                            ReportResultModel.totalTime = ReportResult.totalTime;
                            ReportResultModel.workload = ReportResult.workload;

                            LstctmtReportDetails.Add(ReportResultModel);
                        }

                        #endregion


                        List<ParentLevelQuestionCode> ParentlevelQuestioncodelst = new List<ParentLevelQuestionCode>();
                        List<MedicalExaminationDetail> LstmedicalExaminationDetails = new List<MedicalExaminationDetail>();
                        List<FeedbackQuestionsMER> ParentQuestionlst = new List<FeedbackQuestionsMER>();
                        #region CreateMER
                        foreach (var QueId in model.feedbackMERQuestionAnswerlst.Where(m => m.AppointmentId == apptdetails.AppointmentId))
                        {
                            MedicalExaminationDetail SinglemedicalExaminationDetails = new MedicalExaminationDetail();


                            SinglemedicalExaminationDetails.answer = QueId.Answer;
                            SinglemedicalExaminationDetails.details = QueId.Details;
                            SinglemedicalExaminationDetails.questionCode = QueId.MainQueClientCode;
                            SinglemedicalExaminationDetails.examinationType = QueId.ExaminationType;
                            SinglemedicalExaminationDetails.question = QueId.Question;
                            SinglemedicalExaminationDetails.questionSet = QueId.QuestionSet;
                            LstmedicalExaminationDetails.Add(SinglemedicalExaminationDetails);

                        }

                        #endregion
                        SingleTestDetails.ctmtReportDetails = LstctmtReportDetails;
                        SingleTestDetails.medicalExaminationDetails = LstmedicalExaminationDetails;
                        SingleTestDetails.testresults = Lsttestresults;
                        LstTestDetails.Add(SingleTestDetails);
                    }

                    PPMCBAGICDigitalInformation.testDetails = LstTestDetails;

                    data = JsonConvert.SerializeObject(PPMCBAGICDigitalInformation);
                    BusinessCorelationId = apptdetails.PolicyRefNo + DateTime.Now.ToString("dd-MM-yyyy") + Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());
                    List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();

                    headers.Add(new KeyValuePair<string, string>("username", "support.it@healthassure.in"));
                    headers.Add(new KeyValuePair<string, string>("password", "HADec$2020"));
                    headers.Add(new KeyValuePair<string, string>("BusinessCorelationId", BusinessCorelationId));
                    var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data, headers);

                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            json = client.MakeRequest();
                            break;
                        }
                        catch (Exception Ex)
                        {

                        }
                    }
                    data = data.Replace("'", "\\'");
                    json = json.Replace("'", "\\'");
                    BAGICDigitalDataResponse BAGICresponse = new BAGICDigitalDataResponse();
                    string responseValue = Convert.ToString(json);
                    responseValue = responseValue.Replace(@"[", "").Replace(@"]", "");
                    if (json == "Unable to connect to the remote server")
                    {
                        SBLL.updateBAGICDigitalDataResponse(apptdetails.AppointmentId, responseValue);
                    }
                    else
                    {
                        BAGICresponse = new JavaScriptSerializer().Deserialize<BAGICDigitalDataResponse>(responseValue);
                        if (BAGICresponse.Result == "Pass")
                        {
                            // BAGICStatusUpdate(apptdetails.AppointmentId, "STATUSUPDATE_MER");
                        }
                        SBLL.updateBAGICDigitalDataResponse(apptdetails.AppointmentId, BAGICresponse.responseMessage);

                    }

                    SBLL.SaveServiceRequest("BAGIC", "", "", "BAGICMERForm", apptdetails.AppointmentId.ToString(), "AppointmentId", "JSON", data, "JSON", json, "SERVICE", BusinessCorelationId);
                    SBLL.GetRHIData(apptdetails.AppointmentId, null, null, apptdetails.AppointmentId, "APPOINTMENTID", "DIGITALAPI", data, json, "INSERTALL");

                }

            }
            catch (Exception ex)
            {
                SBLL.SaveServiceRequest("BAGIC", "", "", "BAGICMERForm", AppointmentId, "AppointmentId", "JSON", data, "JSON", json, "SERVICE", BusinessCorelationId);

                json = ex.Message.Replace("'", "\\'");
                ex.GetBaseException();
                baseBLL.uspSaveErrorLog("BAGICMERForm", ex.Message, ex.ToString(), "Y", "Service");
            }

        }


        public void BAGICTELEMERForm()
        {
            string data = string.Empty;
            string json = string.Empty;
            //AppointmentQueryBLL AptBLL = new AppointmentQueryBLL();
            BaseBLL baseBLL = new BaseBLL();
            string AppointmentId = "";
            //string TeleProposerId = "";
            string BusinessCorelationId = "";
            try
            {
                int ClientId = 55;
                int Minutes = 1000;

                // string endPoint = @"https://api.bagicuat.bajajallianz.com/ppc/health/saveDigitalDataPPC"; //@"https://api.bagicsit2.bajajallianz.com/ppc/health/saveDigitalDataPPC";
                string endPoint = @"https://webapi.bajajallianz.com/ppc/health/saveDigitalDataPPC";
                PPMCBAGICDigitalInformation PPMCBAGICDigitalInformation = new PPMCBAGICDigitalInformation();

                MERFormModel model = new MERFormModel();

                model = SBLL.GetBAGICTELEMERAppointments(model, ClientId, Minutes);


                foreach (var apptdetails in model.appointmentlst)
                {

                    AppointmentId = apptdetails.AppointmentId.ToString();

                    PPMCBAGICDigitalInformation.testDetails = new List<TestDetail>();
                    PPMCBAGICDigitalInformation.appointmentId = apptdetails.MemberId;//AppointmentId; //"3250000111";

                   // PPMCBAGICDigitalInformation.membercode = null;//apptdetails.MemberId;
                   // PPMCBAGICDigitalInformation.appointmentDate = Convert.ToString(apptdetails.AppointmentDateTime.Date.ToString("dd-MM-yyyy"));
                   // PPMCBAGICDigitalInformation.dcaddress = apptdetails.dcaddress;
                    PPMCBAGICDigitalInformation.destinationSystem = apptdetails.destinationSystem;

                    PPMCBAGICDigitalInformation.membername = apptdetails.membername; //model.feedbackMERQuestionAnswerlst.Where(m => m.MainQueClientCode == "Name" && m.AppointmentId == apptdetails.AppointmentId).Select(m => m.Text).SingleOrDefault(); //apptdetails.Name;
                    PPMCBAGICDigitalInformation.gender = apptdetails.Gender;// model.feedbackMERQuestionAnswerlst.Where(m => m.MainQueClientCode == "Gender" && m.AppointmentId == apptdetails.AppointmentId).Select(m => m.FeedbackOptions).SingleOrDefault();// apptdetails.Gender;
                    //PPMCBAGICDigitalInformation.isHNI = null;
                    PPMCBAGICDigitalInformation.placeOfService = apptdetails.placeOfService; // model.feedbackMERQuestionAnswerlst.Where(m => m.MainQueClientCode == "Place" && m.AppointmentId == apptdetails.AppointmentId).Select(m => m.Text).SingleOrDefault();// apptdetails.Place;
                    //PPMCBAGICDigitalInformation.vendorCode = apptdetails.vendorCode;
                    PPMCBAGICDigitalInformation.testtype = apptdetails.testtype;
                    PPMCBAGICDigitalInformation.proposalNumber = apptdetails.DocketNo;
                    PPMCBAGICDigitalInformation.sourceSystem = apptdetails.sourceSystem;
                    List<TestDetail> LstTestDetails = new List<TestDetail>();
                    //List<CtmtReportDetail> LstctmtReportDetails = new List<CtmtReportDetail>();

                    //List<Testresult> Lsttestresults = new List<Testresult>();
                    TestDetail SingleTestDetails = new TestDetail();
                    foreach (var item in model.AppointmentTestDetails.Where(m => m.AppointmentId == apptdetails.AppointmentId))
                    {

                        SingleTestDetails = new TestDetail();
                        SingleTestDetails.remarks = item.remarks;
                        SingleTestDetails.status = item.status;
                        SingleTestDetails.testcategory = item.testcategory;
                        SingleTestDetails.testId = item.testId;


                        List<ParentLevelQuestionCode> ParentlevelQuestioncodelst = new List<ParentLevelQuestionCode>();
                        List<MedicalExaminationDetail> LstmedicalExaminationDetails = new List<MedicalExaminationDetail>();
                        List<FeedbackQuestionsMER> ParentQuestionlst = new List<FeedbackQuestionsMER>();
                        #region CreateMER
                        foreach (var QueId in model.feedbackMERQuestionAnswerlst.Where(m => m.AppointmentId == apptdetails.AppointmentId))
                        {
                            MedicalExaminationDetail SinglemedicalExaminationDetails = new MedicalExaminationDetail();


                            SinglemedicalExaminationDetails.answer = QueId.Answer;
                            SinglemedicalExaminationDetails.details = QueId.Details;
                            SinglemedicalExaminationDetails.questionCode = QueId.MainQueClientCode;
                            SinglemedicalExaminationDetails.examinationType = QueId.ExaminationType;
                            SinglemedicalExaminationDetails.question = QueId.Question;
                            SinglemedicalExaminationDetails.questionSet = QueId.QuestionSet;
                            LstmedicalExaminationDetails.Add(SinglemedicalExaminationDetails);

                        }

                        #endregion
                        ///SingleTestDetails.ctmtReportDetails = LstctmtReportDetails;
                        SingleTestDetails.medicalExaminationDetails = LstmedicalExaminationDetails;
                        //SingleTestDetails.testresults = Lsttestresults;
                        LstTestDetails.Add(SingleTestDetails);
                    }

                    PPMCBAGICDigitalInformation.testDetails = LstTestDetails;

                    data = JsonConvert.SerializeObject(PPMCBAGICDigitalInformation);
                    BusinessCorelationId = apptdetails.PolicyRefNo + DateTime.Now.ToString("dd-MM-yyyy") + Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());
                    List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();

                    headers.Add(new KeyValuePair<string, string>("username", "support.it@healthassure.in"));
                    headers.Add(new KeyValuePair<string, string>("password", "HADec$2020"));
                    headers.Add(new KeyValuePair<string, string>("BusinessCorelationId", BusinessCorelationId));
                    var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data, headers);

                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            json = client.MakeRequest();
                            break;
                        }
                        catch (Exception Ex)
                        {

                        }
                    }
                    data = data.Replace("'", "\\'");
                    json = json.Replace("'", "\\'");
                    BAGICDigitalDataResponse BAGICresponse = new BAGICDigitalDataResponse();
                    string responseValue = Convert.ToString(json);
                    responseValue = responseValue.Replace(@"[", "").Replace(@"]", "");
                    //apptdetails.AppointmentId = apptdetails.TeleProposerId;

                    if (json == "Unable to connect to the remote server")
                    {
                        SBLL.updateBAGICDigitalDataResponse(apptdetails.AppointmentId, responseValue);
                    }
                    else
                    {
                        BAGICresponse = new JavaScriptSerializer().Deserialize<BAGICDigitalDataResponse>(responseValue);
                        if (BAGICresponse.Result == "Pass")
                        {
                            // BAGICStatusUpdate(apptdetails.AppointmentId, "STATUSUPDATE_MER");
                        }
                        SBLL.updateBAGICDigitalDataResponseTELE(apptdetails.AppointmentId, BAGICresponse.responseMessage);

                    }

                    SBLL.SaveServiceRequest("BAGIC", "", "", "BAGICTELEMERForm", apptdetails.AppointmentId.ToString(), "TeleProposerId", "JSON", data, "JSON", json, "SERVICE", BusinessCorelationId);
                    SBLL.GetRHIData(apptdetails.AppointmentId, null, null, apptdetails.AppointmentId, "TeleProposerId", "DIGITALAPI", data, json, "INSERTALL");

                }

            }
            catch (Exception ex)
            {
                SBLL.SaveServiceRequest("BAGIC", "", "", "BAGICTELEMERForm", AppointmentId, "AppointmentId", "JSON", data, "JSON", json, "SERVICE", BusinessCorelationId);

                json = ex.Message.Replace("'", "\\'");
                ex.GetBaseException();
                baseBLL.uspSaveErrorLog("BAGICTELEMERForm", ex.Message, ex.ToString(), "Y", "Service");
            }

        }

        #region added by Wamik HDFCERGODigitalMerForm
        public void HDFCERGODigitalMerForm()
        {
            string data = string.Empty;
            string json = string.Empty;
            //AppointmentQueryBLL AptBLL = new AppointmentQueryBLL();
            BaseBLL baseBLL = new BaseBLL();
            string AppointmentId = "";
            string BusinessCorelationId = "";
            try
            {
                int ClientId = 9;
                int Minutes = 1000;


                string endPoint = @"https://he-ppc-v2-api.synovergetech.com/MerAnswer/DigitalMerInsert";
                
                HDFCERGODigitalInformation _HDFCERGODigitalInformation = new HDFCERGODigitalInformation();

                MERFormModel model = new MERFormModel();

                model = SBLL.GetHDFCERGODigitalMERAppointments(model, ClientId, Minutes);


                foreach (var apptdetails in model.Filelst)
                {

                    // AppointmentId = apptdetails.appointmentId.ToString();


                    _HDFCERGODigitalInformation.InsuredDetailId = apptdetails.ProposerReferenceId;//AppointmentId; //"3250000111";
                   // _HDFCERGODigitalInformation.PolicyRefNo = apptdetails.PolicyRefNo;
                    apptdetails.FileName = "D://AMHI00106790.pdf";
                    if (System.IO.File.Exists(apptdetails.FileName))
                    {
                        _HDFCERGODigitalInformation.FileName = System.IO.Path.GetFileName(apptdetails.FileName);
                        _HDFCERGODigitalInformation.FileContent = Convert.ToBase64String(System.IO.File.ReadAllBytes(apptdetails.FileName));
                    }


                    List<DiagnosisDetails> listdiagnosisDetails = new List<DiagnosisDetails>();
                    List<FeedbackQuestionsMER> FeedbackQuestionsMERlst = new List<FeedbackQuestionsMER>();
                    List<MerDtAnswerResponses> _MerDtAnswerResponses = new List<MerDtAnswerResponses>();
                    foreach (var item in model.feedbackMERQuestionAnswerlst.Where(m => m.AppointmentId == apptdetails.AppointmentId))
                    {

                        //List<MerDtAnswerResponses> _MerDtAnswerResponses = new List<MerDtAnswerResponses>();
                        MerDtAnswerResponses merDtAnswerResponses = new MerDtAnswerResponses();

                        merDtAnswerResponses.QuestionRefCode = Convert.ToString(item.QueId);
                        merDtAnswerResponses.QuestionTitle = item.Question;
                        merDtAnswerResponses.ResponseRemarks = item.Details;
                        merDtAnswerResponses.ResponseStatus = item.Answer;
                        _MerDtAnswerResponses.Add(merDtAnswerResponses);

                        #region CreateMER
                    }
                    MerDtAnswerResponses merdtAnswerResponses = new MerDtAnswerResponses();
                    // List<DiagnosisDetails> listdiagnosisDetails = new List<DiagnosisDetails>();
                    if (merdtAnswerResponses.ResponseStatus == "Y")
                    {
                        foreach (var QueId in model.feedbackMERQuestionAnswerlst.Where(m => m.AppointmentId == apptdetails.AppointmentId))
                        {
                            DiagnosisDetails _diagnosisDetails = new DiagnosisDetails();

                            _diagnosisDetails.QuestionRefCode = Convert.ToString(QueId.QueId);
                            _diagnosisDetails.QuestionTitle = QueId.Question;
                            _diagnosisDetails.DiagnosisName = QueId.Details;
                            _diagnosisDetails.DiagnosisDate = Convert.ToString(QueId.UpdatedDateTime.Date.ToString("dd-MM-yyyy"));
                            _diagnosisDetails.Consultationdate = Convert.ToString(QueId.CreatedDateTime.Date.ToString("dd-MM-yyyy"));
                            _diagnosisDetails.LineofTreatment = QueId.FeedbackType;
                            _diagnosisDetails.TreatmentDetails = QueId.ExaminationType;



                            listdiagnosisDetails.Add(_diagnosisDetails);



                        }
                    }



                    #endregion

                    _HDFCERGODigitalInformation.MerDtAnswerResponses = _MerDtAnswerResponses;
                    _HDFCERGODigitalInformation.DiagnosisDetails = listdiagnosisDetails;

                    data = JsonConvert.SerializeObject(_HDFCERGODigitalInformation);
                    BusinessCorelationId = apptdetails.PolicyRefNo + DateTime.Now.ToString("dd-MM-yyyy") + Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());




                    List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
                    headers.Add(new KeyValuePair<string, string>("userName", "mangrokr"));
                    headers.Add(new KeyValuePair<string, string>("userPassword", "Synoverge@123"));
                    headers.Add(new KeyValuePair<string, string>("BusinessCorelationId", BusinessCorelationId));


                    headers.Add(new KeyValuePair<string, string>("Authorization", "bearer " + GetAuthorizationToken()));

                    var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data, headers);

                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            json = client.MakeRequest();
                            break;
                        }
                        catch (Exception Ex)
                        {

                        }
                    }
                    data = data.Replace("'", "\\'");
                    json = json.Replace("'", "\\'");
                    HDFCERGODigitalDataResponse HDFCERGOResponse = new HDFCERGODigitalDataResponse();
                    string responseValue = Convert.ToString(json);
                    responseValue = responseValue.Replace(@"[", "").Replace(@"]", "");
                    if (json == "Unable to connect to the remote server")
                    {
                        SBLL.updateHDFCERGODigitalDataResponse(apptdetails.AppointmentId, responseValue);
                    }
                    else
                    {
                        // HDFCERGOResponse = new JavaScriptSerializer().Deserialize<HDFCERGODigitalDataResponse>(responseValue);
                        if (HDFCERGOResponse.Result == "Pass")
                        {
                            // BAGICStatusUpdate(apptdetails.AppointmentId, "STATUSUPDATE_MER");
                        }
                        SBLL.updateHDFCERGODigitalDataResponse(apptdetails.AppointmentId, HDFCERGOResponse.Message);

                    }

                    SBLL.SaveServiceRequest("HDFC ERGO Health Insurance", "", "", "HDFCERGODigitalMerForm", apptdetails.AppointmentId.ToString(), "AppointmentId", "JSON", data, "JSON", json, "SERVICE", BusinessCorelationId);
                    SBLL.GetRHIData(apptdetails.AppointmentId, null, null, apptdetails.AppointmentId, "APPOINTMENTID", "DIGITALAPI", data, json, "INSERTALL");

                }

            }
            catch (Exception ex)
            {
                SBLL.SaveServiceRequest("HDFC ERGO Health Insurance", "", "", "HDFCERGODigitalMerForm", AppointmentId, "AppointmentId", "JSON", data, "JSON", json, "SERVICE", BusinessCorelationId);

                json = ex.Message.Replace("'", "\\'");
                ex.GetBaseException();
                baseBLL.uspSaveErrorLog("HDFCERGODigitalMerForm", ex.Message, ex.ToString(), "Y", "Service");
            }

        } 
        #endregion

        #region Added by Wamik HDFCERGOTELEMerForm
        public void HDFCERGOTELEMerForm( )
        {
            string data = string.Empty;
            string json = string.Empty;
            BaseBLL baseBLL = new BaseBLL();
            string TeleProposerId = "";
            string BusinessCorelationId = "";
            try
            {
                int ClientId = 9;
                int Minutes = 1000;


                string endPoint = @"https://he-ppc-v2-api.synovergetech.com/MerAnswer/TeleMerInsert";
                HDFCERGODigitalInformation _HDFCERGODigitalInformation = new HDFCERGODigitalInformation();

                MERFormModel model = new MERFormModel();

                model = SBLL.GetHDFCERGOTELEMERAppointments(model, ClientId, Minutes);


                foreach (var apptdetails in model.Filelst)
                {
                    _HDFCERGODigitalInformation.InsuredDetailId = apptdetails.ProposerReferenceId;
                    string tempFilePath = BBLL.GetSystemCodePath("FILESAVEPATH", "TELEPHOTOSAVE");
                    var sourcePath = System.IO.Path.Combine(tempFilePath, apptdetails.FileSavePath);
                    // apptdetails.FileName = "D://AMHI00106790.pdf";                                            
                    if (System.IO.File.Exists(sourcePath))
                    {
                        _HDFCERGODigitalInformation.FileName = apptdetails.file_name; 
                        _HDFCERGODigitalInformation.FileContent = Convert.ToBase64String(System.IO.File.ReadAllBytes(sourcePath));
                    }
                    List<DiagnosisDetails> listdiagnosisDetails = new List<DiagnosisDetails>();
                    List<FeedbackQuestionsMER> FeedbackQuestionsMERlst = new List<FeedbackQuestionsMER>();
                    List<MerDtAnswerResponses> _MerDtAnswerResponses = new List<MerDtAnswerResponses>();
                    foreach (var item in model.feedbackMERQuestionAnswerlst.Where(m => m.TeleProposerId == apptdetails.TeleProposerId))
                    {

                       
                        MerDtAnswerResponses merDtAnswerResponses = new MerDtAnswerResponses();

                        merDtAnswerResponses.QuestionRefCode = Convert.ToString(item.MainQueClientCode);
                        merDtAnswerResponses.QuestionTitle = item.QuestionsName;
                        merDtAnswerResponses.ResponseRemarks = item.Answer;
                        merDtAnswerResponses.ResponseStatus = item.HasSubQue;
                        if (merDtAnswerResponses.ResponseStatus == "Y")
                        {

                            if (item.MainQueId != 0)
                            {
                                merDtAnswerResponses.QuestionRefCode = Convert.ToString(item.MainQueClientCode);
                                merDtAnswerResponses.QuestionTitle = item.QuestionsName;
                                merDtAnswerResponses.ResponseRemarks = item.Answer;
                                merDtAnswerResponses.ResponseStatus = item.HasSubQue;

                            }
                        }
                        if(item.MainQueClientCode != null)
                        {
                            _MerDtAnswerResponses.Add(merDtAnswerResponses);
                        }
                    }
                    
                    MerDtAnswerResponses merdtAnswerResponses = new MerDtAnswerResponses();
                    
                    _HDFCERGODigitalInformation.MerDtAnswerResponses = _MerDtAnswerResponses;
                    _HDFCERGODigitalInformation.DiagnosisDetails = listdiagnosisDetails;

                    data = JsonConvert.SerializeObject(_HDFCERGODigitalInformation);

                    BusinessCorelationId = apptdetails.PolicyRefNo + DateTime.Now.ToString("dd-MM-yyyy") + Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());
                    List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
                    headers.Add(new KeyValuePair<string, string>("userName", "mangrokr"));
                    headers.Add(new KeyValuePair<string, string>("userPassword", "Synoverge@123"));
                    headers.Add(new KeyValuePair<string, string>("BusinessCorelationId", BusinessCorelationId));

                    headers.Add(new KeyValuePair<string, string>("Authorization", "Bearer " + GetAuthorizationToken()));
                    var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data, headers);

                    for (int i = 0; i < 5; i++)
                    {
                        try
                        {
                            json = client.MakeRequest();
                            break;
                        }
                        catch (Exception Ex)
                        {

                        }
                    }
                    data = data.Replace("'", "\\'");
                    json = json.Replace("'", "\\'");
                    HDFCERGODigitalDataResponse HDFCERGOResponse = new HDFCERGODigitalDataResponse();
                    string responseValue = Convert.ToString(json);
                    responseValue = responseValue.Replace(@"[", "").Replace(@"]", "");
                    if (json == "Unable to connect to the remote server")
                    {
                        SBLL.updateHDFCERGODigitalDataResponse(apptdetails.TeleProposerId, responseValue);
                    }
                    else
                    {
                        // HDFCERGOResponse = new JavaScriptSerializer().Deserialize<HDFCERGODigitalDataResponse>(responseValue);
                        if (HDFCERGOResponse.Result == "Pass")
                        {
                            // BAGICStatusUpdate(apptdetails.AppointmentId, "STATUSUPDATE_MER");
                        }
                        SBLL.updateHDFCERGODigitalDataResponse(apptdetails.TeleProposerId, HDFCERGOResponse.Message);

                    }

                    SBLL.SaveServiceRequest("HDFC ERGO Health Insurance", "", "", "HDFCERGOTELEMerForm", apptdetails.TeleProposerId.ToString(), "TeleProposerId", "JSON", data, "JSON", json, "SERVICE", BusinessCorelationId);
                    SBLL.GetRHIData(apptdetails.TeleProposerId, null, null, apptdetails.TeleProposerId, "TeleProposerId", "DIGITALAPI", data, json, "TELEINSERTALL");

                }


            }
            catch (Exception ex)
             {
                SBLL.SaveServiceRequest("HDFC ERGO Health Insurance", "", "", "HDFCERGOTELEMerForm", TeleProposerId, "TeleProposerId", "JSON", data, "JSON", json, "SERVICE", BusinessCorelationId);

                json = ex.Message.Replace("'", "\\'");
                ex.GetBaseException();
                baseBLL.uspSaveErrorLog("HDFCERGOTELEMerForm", ex.Message, ex.ToString(), "Y", "Service");
            }

        } 
        #endregion

        #region added by Wamik Getauthorization token from HDFCERGO client
        public string GetAuthorizationToken()
        {
            string json = string.Empty;
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();

            AuthorizationDataModel _Authdatamodel = new AuthorizationDataModel();
            _Authdatamodel.userName = ConfigurationManager.AppSettings["UserNameHDFCERGO"].ToString(); //"mangrokr"; 
            _Authdatamodel.userPassword = ConfigurationManager.AppSettings["PasswordHDFCERGO"].ToString(); //"Synoverge@123";
            string authdata = string.Empty;
            authdata = JsonConvert.SerializeObject(_Authdatamodel);

            List<KeyValuePair<string, string>> headersauth = new List<KeyValuePair<string, string>>();
            string auth = @"https://he-ppc-v2-api.synovergetech.com/Security/AuthenticateUser";

            headersauth.Add(new KeyValuePair<string, string>("userName", "_Authdatamodel.userName"));
            headersauth.Add(new KeyValuePair<string, string>("userPassword", "_Authdatamodel.userPassword"));

            var clientAuth = new WebService.RestClient("application/json", auth, APICallScheduler.Common.HttpVerb.POST, authdata, headersauth);

            json = clientAuth.MakeRequest();


            authdata = authdata.Replace("'", "\\'");

            JavaScriptSerializer js = new JavaScriptSerializer();
            List<ExtraData> extraData = new List<ExtraData>();
            ExtraData extraData1 = new ExtraData();
            List<Role> roles = new List<Role>();
            List<PermissionActionPermission> permissionActions = new List<PermissionActionPermission>();
            List<UserInfo> userInfos = new List<UserInfo>();

            AUthorizationModelResponse aUthorizationModelResponse = new AUthorizationModelResponse();
            aUthorizationModelResponse = new JavaScriptSerializer().Deserialize<AUthorizationModelResponse>(json);


            string SecureToken = aUthorizationModelResponse.ExtraData.SecureToken;

            return SecureToken;
        }

        #endregion

        #region adde by Wamik for hdfcergo updatecalllog 
        public void HDFCERGOStatusUpdateCallLogForm()

        {
            try
            {
                string endPoint = @"https://he-ppc-v2-api.synovergetech.com/CallLog/CallLogDetailInsert";
                List<HDFCERGOStatusUpdate> List = SBLL.HDFCERGOStatusUpdateNew();
                foreach (var model in List)
                {
                    try
                    {
                        HDFCERGOStatusUpdateApiModel dataModel = new HDFCERGOStatusUpdateApiModel();
                        dataModel.InsuredDetailId = model.InsuredDetailId;
                        // dataModel.CallDatetime = model.CallDatetime;
                        dataModel.CallerUsername = model.CallerUsername;
                        dataModel.CallRemarks = model.CallRemarks;
                        dataModel.CallStatusName = model.CallStatusName;
                        dataModel.DCAddress1 = model.DCAddress1;
                        dataModel.CallDatetime = model.CallDatetime;
                        //dataModel.DCAddress2 = model.DCAddress2;
                        //dataModel.DCAddress3 = model.DCAddress3;
                        dataModel.DCCity = model.DCCity;
                        dataModel.DCContactNumber = model.DCContactNumber;
                        dataModel.DCEmailId = model.DCEmailId;
                        dataModel.DCName = model.DCName;
                        dataModel.DCPincode = model.DCPincode;
                        dataModel.DCRemarks = model.DCRemarks;
                        dataModel.DCState = model.DCState;
                        dataModel.DCUniqueCode = model.DCUniqueCode;

                        string BusinessCorelationId = model.AppointmentCode + DateTime.Now.ToString("dd-MM-yyyy") + Convert.ToString(DateTimeOffset.Now.ToUnixTimeMilliseconds());

                        List<KeyValuePair<string, string>> header = new List<KeyValuePair<string, string>>();
                        header.Add(new KeyValuePair<string, string>("userName", "mangrokr"));
                        header.Add(new KeyValuePair<string, string>("userPassword", "Synoverge@123"));
                        header.Add(new KeyValuePair<string, string>("BusinessCorelationId", BusinessCorelationId)); //"650dc225-cd24-46a9-b417-1812ceaf7c19"));
                        header.Add(new KeyValuePair<string, string>("Authorization", "Bearer " + GetAuthorizationToken()));


                        string data = JsonConvert.SerializeObject(dataModel);
                        var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data, header);
                        var json = "";
                        for (int i = 0; i < 5; i++)
                        {
                            try
                            {
                                json = client.MakeRequest();
                                data = data.Replace("'", "\\'");
                                data = "BusinessCorelationId:" + BusinessCorelationId + "  request:" + data;
                                json = json.Replace("'", "\\'");
                                SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "APPOINTMENTLOGID", "STATUSUPDATE", data, json, "INSERTALL");
                                break;
                            }
                            catch (Exception ex)
                            {

                                SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "APPOINTMENTLOGID", "STATUSUPDATE", data, json, "INSERTALL");
                            }
                        }


                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }
        #endregion

        #region added by wamik for hdfergo updatecalllogtelemer 
        public void HDFCERGOStatusUpdateCallLogTeleMerForm()
        
        
        {
            try
            {
                string endPoint = @"https://he-ppc-v2-api.synovergetech.com/CallLog/CallLogDetailInsert";
                List<HDFCERGOStatusUpdate> List = SBLL.HDFCERGOStatusUpdateTELEMER();
                foreach (var model in List)
                {
                    try
                    {
                        HDFCERGOStatusUpdateApiModel dataModel = new HDFCERGOStatusUpdateApiModel();
                        dataModel.InsuredDetailId = model.InsuredDetailId;
                        // dataModel.CallDatetime = model.CallDatetime;
                        dataModel.CallerUsername = RemoveSpecialCharacterHDFCERGO(model.CallerUsername);
                        dataModel.CallRemarks = RemoveSpecialCharacterHDFCERGO(model.CallRemarks);
                        dataModel.CallStatusName = RemoveSpecialCharacterHDFCERGO(model.CallStatusName);
                        dataModel.DCAddress1 = RemoveSpecialCharacterHDFCERGO(model.DCAddress1);
                        dataModel.CallDatetime = RemoveSpecialCharacterHDFCERGO(model.CallDatetime);
                        dataModel.DCAddress2 = RemoveSpecialCharacterHDFCERGO(model.DCAddress1);
                        dataModel.DCAddress3 = RemoveSpecialCharacterHDFCERGO(model.DCAddress1);
                        dataModel.DCCity = RemoveSpecialCharacterHDFCERGO(model.DCCity);
                        dataModel.DCContactNumber = RemoveSpecialCharacterHDFCERGO(model.DCContactNumber);
                        dataModel.DCEmailId = RemoveSpecialCharacterHDFCERGO(model.DCEmailId);
                        dataModel.DCName = RemoveSpecialCharacterHDFCERGO(model.DCName);
                        dataModel.DCPincode = RemoveSpecialCharacterHDFCERGO(model.DCPincode);
                        dataModel.DCRemarks = RemoveSpecialCharacterHDFCERGO(model.DCRemarks);
                        dataModel.DCState = RemoveSpecialCharacterHDFCERGO(model.DCState);
                        dataModel.DCUniqueCode = RemoveSpecialCharacterHDFCERGO(model.DCUniqueCode);

                        string data = JsonConvert.SerializeObject(dataModel);
                        List<KeyValuePair<string, string>> header = new List<KeyValuePair<string, string>>();
                        header.Add(new KeyValuePair<string, string>("userName", "mangrokr"));
                        header.Add(new KeyValuePair<string, string>("userPassword", "Synoverge@123"));
                        //header.Add(new KeyValuePair<string, string>("BusinessCorelationId", BusinessCorelationId)); //"650dc225-cd24-46a9-b417-1812ceaf7c19"));
                        header.Add(new KeyValuePair<string, string>("Authorization", "Bearer " + GetAuthorizationToken()));

                        var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data, header);
                        var json = string.Empty;
                        for (int i = 0; i < 5; i++)
                        {
                            try
                            {


                                json = client.MakeRequest();
                                data = data.Replace("'", "\\'");
                                json = json.Replace("'", "\\'");
                                SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "TELESTATUSLOGID", "TELESTATUSUPDATE", data, json, "TELEINSERTALL");


                                break;
                            }
                            catch (Exception ex)
                            {

                                SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "TELESTATUSLOGID", "TELESTATUSUPDATE", data, json, "TELEINSERTALL");

                            }
                        }
                        // BLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "TELESTATUSLOGID", "TELESTATUSUPDATE", data, json, "INSERTALL");
                    }
                    catch (Exception ex)
                    {
                        SBLL.GetRHIData(model.AppointmentId, null, null, model.AppointmentLogId, "TELESTATUSLOGID", "TELESTATUSUPDATE", ex.Message, ex.Message, "TELEINSERTALL");

                    }
                }
            }
            catch
            {
            }
        }

        #endregion

        #region Added by Mayuri for Medical & Vendor Tele Video Integration

        public string GetTokenForAuth()
        {

            var json = string.Empty;
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();

            AuthDataModel _Authdatamodel = new AuthDataModel();
            _Authdatamodel.grantType = "client_credentials";
            _Authdatamodel.clientInfo = "MW9MSGR6WnEwbXByTjZ1Z0xlUW5GMWJla01VYTpnWjdmR3BLOURHMlRmV2E3Vlcycl95M1JRRmdh";
            string authdata = string.Empty;
            authdata = JsonConvert.SerializeObject(_Authdatamodel);

            List<KeyValuePair<string, string>> headersauth = new List<KeyValuePair<string, string>>();
            string authEndPoint = @"https://kliapiuat.mykotaklife.com/custom/1.0.0/auth";
            headersauth.Add(new KeyValuePair<string, string>("grantType", _Authdatamodel.grantType));
            headersauth.Add(new KeyValuePair<string, string>("clientInfo", _Authdatamodel.clientInfo));

            var clientAuth = new WebService.RestClient("application/json", authEndPoint, APICallScheduler.Common.HttpVerb.POST, authdata, headersauth);
            
            json = clientAuth.MakeRequest();
            authdata = authdata.Replace("'", "\\'");

            JavaScriptSerializer js = new JavaScriptSerializer();
            AUthorizationModelResponse aUthorizationModelResponse = new AUthorizationModelResponse();
            aUthorizationModelResponse = new JavaScriptSerializer().Deserialize<AUthorizationModelResponse>(json);


            string SecureToken = aUthorizationModelResponse.access_token;

            return SecureToken;
        }

        public void GetTPARecordStatus()
        {
            BaseBLL baseBLL = new BaseBLL();
            try
            {
                string endPoint = @"https://kliapiuat.mykotaklife.com/gettpastatus/v1.0.0/GetTPAStatus";
             
                List<TPAInformation> List = SBLL.GetTELEAppointmentDetails();

                foreach (var model in List)
                {
                    TPAInformationModel dataModel = new TPAInformationModel();
                    
                    dataModel.MAHSRequestId = model.MAHSRequestId;
                    dataModel.TestCategory = model.TestCategory;
                    dataModel.TPAStatus = model.TPAStatus;
                   
                    List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
                    headers.Add(new KeyValuePair<string, string>("grantType", "client_credentials"));
                    headers.Add(new KeyValuePair<string, string>("clientInfo", "MW9MSGR6WnEwbXByTjZ1Z0xlUW5GMWJla01VYTpnWjdmR3BLOURHMlRmV2E3Vlcycl95M1JRRmdh"));
                    headers.Add(new KeyValuePair<string, string>("Authorization", "Bearer " + GetTokenForAuth()));

                    string data = JsonConvert.SerializeObject(dataModel);
                    var json = "";
                    var client = new WebService.RestClient("application/json", endPoint, APICallScheduler.Common.HttpVerb.POST, data, headers);
                    for (int i = 0; i < 5; i++)
                    
                    {
                        try
                        {
                            json = client.MakeRequest();
                            break;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    data = data.Replace("'", "\\'");
                    json = json.Replace("'", "\\'");
                    SBLL.SaveServiceRequest("TELE Health Insurance", "", "", "GetTPAStatus", model.MAHSRequestId.ToString(), "AppointmentId", "JSON", data, "JSON", json, "SERVICE", null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}

   
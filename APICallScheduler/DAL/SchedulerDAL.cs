using APICallScheduler.Common;
using APICallScheduler.Models;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace APICallScheduler.DAL
{
    public class SchedulerDAL
    {
        BaseBLL objCommon = new BaseBLL();
        public DataSet funPushTeleMERCaseStatus(long ClientId)
        {
            BaseBLL objCommon = new BaseBLL();
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter dap;
                SqlConnection MyConn = objCommon.GetConnection();
                SqlCommand cmd = new SqlCommand("USPGETTELEMERSTANDARDNONSTANDARDFLAG", MyConn);
                cmd.Parameters.AddWithValue("@ClientId", 52);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(ds);
            }
            catch (Exception ce)
            {
                throw ce;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return ds;
        }

        public string SaveHAServiceTeMERStatus(DataTable dt)
        {
            SqlCommand cmd;

            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("USPINSERTINTOHASERVICETELEMERSTATUS", MyConn);
                cmd.Parameters.AddWithValue("@TypeStatus", dt);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                return (string)cmd.ExecuteScalar();
            }
            catch
            {
                throw;
            }
        }


        public DataTable GetTalicTeleStatusAPIDetails(string Type)
        {
            SqlCommand cmd;
            DataTable dt = new DataTable();
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("uspGetTalicTeleApiDetails", MyConn);
                cmd.Parameters.Add("@Type", SqlDbType.VarChar).Value = Type;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return dt;
        }

        public DataTable GetRHIData(decimal? AppointmentId, decimal? AppointmentLogId, decimal? Id, decimal? ReferenceId, string ReferenceType, string WebServiceType, string WebServiceRequest, string WebServiceResponse, string Flag)
        {
            SqlCommand cmd;
            DataTable dt = new DataTable();
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("USPGETSETRHIDATA", MyConn);
                cmd.Parameters.Add("@AppointmentId", SqlDbType.BigInt).Value = AppointmentId;
                cmd.Parameters.Add("@AppointmentLogId", SqlDbType.BigInt).Value = AppointmentLogId;
                cmd.Parameters.Add("@Id", SqlDbType.BigInt).Value = Id;
                cmd.Parameters.Add("@ReferenceId", SqlDbType.BigInt).Value = ReferenceId;
                cmd.Parameters.Add("@ReferenceType", SqlDbType.VarChar).Value = ReferenceType;
                cmd.Parameters.Add("@WebServiceType", SqlDbType.VarChar).Value = WebServiceType;
                cmd.Parameters.Add("@WebServiceRequest", SqlDbType.VarChar).Value = WebServiceRequest;
                cmd.Parameters.Add("@WebServiceResponse", SqlDbType.VarChar).Value = WebServiceResponse;
                cmd.Parameters.Add("@Flag", SqlDbType.VarChar).Value = Flag;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return dt;
        }

        public DataSet DownloadVideoRecording(long TeleProposerId, string Flag)
        {
            SqlCommand cmd;
            DataSet ds = new DataSet();
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("uspGetDataToSaveVideoRecording", MyConn);
                cmd.Parameters.Add("@TeleProposerId", SqlDbType.BigInt).Value = TeleProposerId;
                cmd.Parameters.Add("@Flag", SqlDbType.VarChar).Value = Flag;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(ds);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return ds;
        }

        public DataTable SaveVideoDownLoadStatus(List<VideoDownloadModel> List)
        {

            SqlCommand cmd;
            DataTable dt = new DataTable();
            string SerializeString = JsonConvert.SerializeObject(List);
            dt = BaseBLL.ToDataTable(List);
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("uspUpdateRecordingSaveStatus", MyConn);
                cmd.Parameters.Add("@RecList", SqlDbType.Structured).Value = dt;

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return dt;
        }

        public DataTable SaveComposition(long TeleProposerId, string CompositionId,long VideoLinkId)
        {

            SqlCommand cmd;
            DataTable dt = new DataTable();
            
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("uspInsertCompositionId", MyConn);
                cmd.Parameters.Add("@CompositionId", SqlDbType.VarChar).Value = CompositionId;
                cmd.Parameters.Add("@ProposerId", SqlDbType.BigInt).Value = TeleProposerId;
                cmd.Parameters.Add("@VideoLinkId", SqlDbType.BigInt).Value = VideoLinkId;
                cmd.Parameters.Add("Flag", SqlDbType.VarChar).Value = "C";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return dt;
        }

        public DataTable GetBajajApiDetailsNew(string Type)
        {
            SqlCommand cmd;
            DataTable dt = new DataTable();
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("USPGETBAJAJAPIDETAILSNEW", MyConn);
                cmd.Parameters.Add("@Type", SqlDbType.VarChar).Value = Type;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return dt;
        }
        public DataTable GetHDFCERGOApiDetailsNew(string Type)
        {
            SqlCommand cmd;
            DataTable dt = new DataTable();
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("USPGETHDFCERGOAPIDETAILSNEW", MyConn);
                cmd.Parameters.Add("@Type", SqlDbType.VarChar).Value = Type;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return dt;
        }

        public DataTable GetBajajTELEApiDetailsNew(string Type)
        {
            SqlCommand cmd;
            DataTable dt = new DataTable();
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("uspGetBajajApiDetailsTeleMERNew", MyConn);
                cmd.Parameters.Add("@Type", SqlDbType.VarChar).Value = Type;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return dt;
        }



        #region Added By Dheeraj For LIC Report Upload
        public DataTable GetLICApiDetails(string Type)
        {
            SqlCommand cmd;
            DataTable dt = new DataTable();
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("USPGETLICAPIDETAILS", MyConn);
                cmd.Parameters.Add("@Type", SqlDbType.VarChar).Value = Type;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return dt;
        }

        #endregion

        public string GetSystemCodePath(string CodeType, string Code)
        {
            string Result = "";
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                SqlCommand cmd = new SqlCommand("USPGETCONTACTUSDETAIL", MyConn);
                cmd.Parameters.AddWithValue("@CodeType", CodeType);
                cmd.Parameters.AddWithValue("@Code", Code);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                Result = (string)cmd.ExecuteScalar();
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return Result;
        }

        public DataTable GetTranscriptData(decimal? TeleProposerId, decimal? ClientId, decimal? QueId, string Flag, string FromDate = null, string ToDate = null)
        {
            SqlCommand cmd;
            DataTable dt = new DataTable();
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("uspGenerateTranscript", MyConn);
                cmd.Parameters.Add("@TeleProposerId", SqlDbType.BigInt).Value = TeleProposerId;
                cmd.Parameters.Add("@ClientId", SqlDbType.BigInt).Value = ClientId;
                cmd.Parameters.Add("@QueId", SqlDbType.BigInt).Value = QueId;
                cmd.Parameters.Add("@Flag", SqlDbType.VarChar).Value = Flag;
                cmd.Parameters.Add("@FromDate", SqlDbType.VarChar).Value = FromDate;
                cmd.Parameters.Add("@ToDate", SqlDbType.VarChar).Value = ToDate;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return dt;
        }

        public DataTable GetBajajApiDetailsTeleMer(string Type)
        {
            SqlCommand cmd;
            DataTable dt = new DataTable();
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("uspGetBajajApiDetails_TeleMER", MyConn);
                cmd.Parameters.Add("@Type", SqlDbType.VarChar).Value = Type;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return dt;
        }
        public DataTable GetHDFCERGOStatusUpdateTELEMER(string Type)
        {
            SqlCommand cmd;
            DataTable dt = new DataTable();
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("USPGETHDFCERGOAPIDETAILSTeleMER", MyConn);
                cmd.Parameters.Add("@Type", SqlDbType.VarChar).Value = Type;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return dt;
        }
        public DataSet DownloadVideoRecordingPPHC(long AppointmentId, string Flag)
        {
            SqlCommand cmd;
            DataSet ds = new DataSet();
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("uspGetDataToSaveVideoRecordingPPHC", MyConn);
                cmd.Parameters.Add("@AppointmentId", SqlDbType.BigInt).Value = AppointmentId;
                cmd.Parameters.Add("@Flag", SqlDbType.VarChar).Value = Flag;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(ds);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return ds;
        }

        public DataTable SaveVideoDownLoadStatusPPHC(List<VideoDownloadModel> List)
        {

            SqlCommand cmd;
            DataTable dt = new DataTable();
            string SerializeString = JsonConvert.SerializeObject(List);
            dt = BaseBLL.ToDataTable(List);
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("uspUpdateRecordingSaveStatusPPHC", MyConn);
                cmd.Parameters.Add("@RecList", SqlDbType.Structured).Value = dt;

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return dt;
        }


        public DataTable SaveCompositionPPHC(long AppointmentId, string CompositionId, long VideoLinkId)
        {

            SqlCommand cmd;
            DataTable dt = new DataTable();

            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("uspInsertCompositionIdPPHC", MyConn);
                cmd.Parameters.Add("@CompositionId", SqlDbType.VarChar).Value = CompositionId;
                cmd.Parameters.Add("@ProposerId", SqlDbType.BigInt).Value = AppointmentId;
                cmd.Parameters.Add("@VideoLinkId", SqlDbType.BigInt).Value = VideoLinkId;
                cmd.Parameters.Add("Flag", SqlDbType.VarChar).Value = "C";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return dt;
        }

        public DataTable BajajBillingAPI()
        {
            SqlCommand cmd;
            DataTable dt = new DataTable();
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("upsBALICAutoRefundMedicalList", MyConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return dt;
        }

        public DataTable BajajMISAPI()
        {
            SqlCommand cmd;
            DataTable dt = new DataTable();
            SqlDataAdapter dap;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("upsBALICMISAPI", MyConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                dap = new SqlDataAdapter(cmd);
                dap.Fill(dt);
            }
            catch
            {
                throw;
            }
            finally
            {
                objCommon.CloseConnection();
            }
            return dt;
        }

        #region bagic

        public MERFormModel GetBAGICMERAppointments(MERFormModel model, int ClientId, int? Minutes)
        {
            try
            {
                var parameters = new DynamicParameters();
                SqlConnection MyConn = objCommon.GetConnection();

                parameters.Add("@ClientId", ClientId);
                parameters.Add("@Minutes", Minutes);
                var data = MyConn.QueryMultiple("UspGetBAGICMERAppointments", parameters, commandType: CommandType.StoredProcedure);
                model.appointmentlst = data.Read<AppointmentDetails>().OrderBy(m => m.AppointmentId).ToList();
                model.feedbackMERQuestionAnswerlst = data.Read<FeedbackMERQuestionAnswer>().OrderBy(m => m.QueId).ToList();
                model.AppointmentTestvalueslst = data.Read<AppointmentTestvalues>().ToList();
                model.AptCtmtReportDetaillst = data.Read<AptCtmtReportDetail>().ToList();
                model.AppointmentTestDetails = data.Read<ApttTestDetails>().ToList();
                return model;
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
                var parameters = new DynamicParameters();
                SqlConnection MyConn = objCommon.GetConnection();

                parameters.Add("@AppointmentId", AppointmentId);
                parameters.Add("@Result", Result);
                var data = MyConn.QueryMultiple("uspupdateBAGICDigitalDataResponse", parameters, commandType: CommandType.StoredProcedure);

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
                var parameters = new DynamicParameters();
                SqlConnection MyConn = objCommon.GetConnection();

                parameters.Add("@AppointmentId", AppointmentId);
                parameters.Add("@Result", Result);
                var data = MyConn.QueryMultiple("uspupdateHDFCERGODigitalDataResponse", parameters, commandType: CommandType.StoredProcedure);

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
                var parameters = new DynamicParameters();
                SqlConnection MyConn = objCommon.GetConnection();

                parameters.Add("@AppointmentId", AppointmentId);
                parameters.Add("@Result", Result);
                var data = MyConn.QueryMultiple("uspupdateBAGICDigitalDataResponseTELE", parameters, commandType: CommandType.StoredProcedure);

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
                var parameters = new DynamicParameters();
                SqlConnection MyConn = objCommon.GetConnection();

                parameters.Add("@AppointmentId", AppointmentId);
                parameters.Add("@Result", Result);
                var data = MyConn.QueryMultiple("uspupdateHDFCERGODigitalDataResponseTELE", parameters, commandType: CommandType.StoredProcedure);

            }
            catch (Exception)
            {

                throw;
            }
        }


        public void SaveServiceRequest(string ClientCode, string UserName, string Password, string ServiceName, string ReferenceId, string ReferenceType, string RequestType, string Request, string ResponseType, string Response, string CreatedBy, string BusinessCorelationId)
        {
            SqlCommand cmd;
            try
            {
                SqlConnection MyConn = objCommon.GetConnection();
                cmd = new SqlCommand("uspSaveServiceRequestResponse", MyConn);
                cmd.Parameters.Add("@ClientCode", SqlDbType.VarChar).Value = ClientCode;
                cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = UserName;
                cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = Password;
                cmd.Parameters.Add("@ServiceName", SqlDbType.VarChar).Value = ServiceName;
                cmd.Parameters.Add("@ReferenceId", SqlDbType.VarChar).Value = ReferenceId;
                cmd.Parameters.Add("@ReferenceType", SqlDbType.VarChar).Value = ReferenceType;
                cmd.Parameters.Add("@RequestType", SqlDbType.VarChar).Value = RequestType;
                cmd.Parameters.Add("@Request", SqlDbType.VarChar).Value = Request;
                cmd.Parameters.Add("@ResponseType", SqlDbType.VarChar).Value = ResponseType;
                cmd.Parameters.Add("@Response", SqlDbType.VarChar).Value = Response;
                cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar).Value = CreatedBy;
                cmd.Parameters.Add("@BusinessCorelationId", SqlDbType.VarChar).Value = BusinessCorelationId;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 50000;
                cmd.ExecuteNonQuery();
            }
            catch
            {
            }
            finally
            {
                objCommon.CloseConnection();
            }
        }
        #endregion

        #region Aditya BAGIC TELE MER
        public MERFormModel GetBAGICTELEMERAppointments(MERFormModel model, int ClientId, int? Minutes)
        {
            try
            {
                var parameters = new DynamicParameters();
                SqlConnection MyConn = objCommon.GetConnection();

                parameters.Add("@ClientId", ClientId);
                parameters.Add("@Minutes", Minutes);
                var data = MyConn.QueryMultiple("UspGetBAGICTELEMERAppointments", parameters, commandType: CommandType.StoredProcedure);
                model.appointmentlst = data.Read<AppointmentDetails>().OrderBy(m => m.TeleProposerId).ToList();
                model.feedbackMERQuestionAnswerlst = data.Read<FeedbackMERQuestionAnswer>().OrderBy(m => m.QueId).ToList();
                //model.AppointmentTestvalueslst = data.Read<AppointmentTestvalues>().ToList();
                //model.AptCtmtReportDetaillst = data.Read<AptCtmtReportDetail>().ToList();
                model.AppointmentTestDetails = data.Read<ApttTestDetails>().ToList();
                return model;
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
                var parameters = new DynamicParameters();
                SqlConnection MyConn = objCommon.GetConnection();

                parameters.Add("@ClientId", ClientId);
                parameters.Add("@Minutes", Minutes);
                var data = MyConn.QueryMultiple("UspGetHDFCERGODigitalMERAppointments", parameters, commandType: CommandType.StoredProcedure);
                model.Filelst = data.Read<Filelsts>().OrderBy(m => m.AppointmentId).ToList();
                model.appointmentlst = data.Read<AppointmentDetails>().OrderBy(m => m.AppointmentId).ToList();
                model.feedbackMERQuestionAnswerlst = data.Read<FeedbackMERQuestionAnswer>().OrderBy(m => m.QueId).ToList();
                model.FeedbackQuestionsMERlst=data.Read<FeedbackQuestionsMER>().OrderBy(m => m.QueId).ToList();
                
                //model.AppointmentTestvalueslst = data.Read<AppointmentTestvalues>().ToList();
                //model.AptCtmtReportDetaillst = data.Read<AptCtmtReportDetail>().ToList();
                model.AppointmentTestDetails = data.Read<ApttTestDetails>().ToList();
                
                return model;
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
                var parameters = new DynamicParameters();
                SqlConnection MyConn = objCommon.GetConnection();

                parameters.Add("@ClientId", ClientId);
                parameters.Add("@Minutes", Minutes);
                var data = MyConn.QueryMultiple("UspGetHDFCERGOTELEMERAppointments", parameters, commandType: CommandType.StoredProcedure);
                model.Filelst = data.Read<Filelsts>().OrderBy(m => m.AppointmentId).ToList();
                model.appointmentlst = data.Read<AppointmentDetails>().OrderBy(m => m.TeleProposerId).ToList();
                model.feedbackMERQuestionAnswerlst = data.Read<FeedbackMERQuestionAnswer>().OrderBy(m => m.QueId).ToList();
                model.FeedbackQuestionsMERlst = data.Read<FeedbackQuestionsMER>().OrderBy(m => m.QueId).ToList();

                //model.AppointmentTestvalueslst = data.Read<AppointmentTestvalues>().ToList();
                //model.AptCtmtReportDetaillst = data.Read<AptCtmtReportDetail>().ToList();
                model.AppointmentTestDetails = data.Read<ApttTestDetails>().ToList();

                return model;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APICallScheduler.Models
{
    public class SchedulerModel
    {

    }
    public class HAServiceTeleMERStatus
    {
        public long TeleProposerId { get; set; }
        public string Status { get; set; }
        public string Response { get; set; }
        public string SentFlag { get; set; }
        public string CreatedBy { get; set; }
        public string Reason { get; set; }
        public string RawResponse { get; set; }
        public string Request { get; set; }
        public string URL { get; set; }
    }
    public class TeleMERStatsServicceModel
    {
        public string TELEMerPolicyno { get; set; }
        public string MedicalStdNonStdFlag { get; set; }

    }

    public class TeleMERStatsServicceResponseModel
    {
        public string Status { get; set; }
        public string PolicyNumber { get; set; }
        public string Reason { get; set; }
    }

    #region Talic Tele Status Push
    public class TalicTeleStatusUpdate
    {
        public string applicationId { get; set; }
        public string name { get; set; }
        public string tpaRefNum { get; set; }
        public string caseIntimationDate { get; set; }
        public string firstcallDate { get; set; }
        public string callStatus { get; set; }
        public string finalcallDate { get; set; }
        public string finalStatus { get; set; }
        public string policyDisposition { get; set; }
        public string mobileNumDialed { get; set; }
        public long TeleProposerId { get; set; }
        public long TeleStatusLogId { get; set; }
    }
    public class TalicTeleAPIStatusUpdate
    {
        public string applicationId { get; set; }
        public string name { get; set; }
        public string caseIntimationDate { get; set; }
        public string tpaRefNum { get; set; }
        public string firstcallDate { get; set; }
        public string callStatus { get; set; }
        public string finalcallDate { get; set; }
        public string finalStatus { get; set; }
        public string policyDisposition { get; set; }
        public string mobileNumDialed { get; set; }

    }

    public class reqInfoM
    {
        public List<TalicTeleAPIStatusUpdate> reqInfo { get; set; }
    }
    public class reqInfoFinal
    {
        public reqInfoM reqInfo { get; set; }
    }

    public class TALICTeleStatsServicceResponseModel
    {
        public string status { get; set; }
        public string errorMessage { get; set; }
        public List<subTalicResponse> resInfo { get; set; }
    }
    public class subTalicResponse
    {
        public string applicationId { get; set; }
        public string tpaRefNum { get; set; }
        public string status { get; set; }
        public string errorMessage { get; set; }
    }
    #endregion

    public class VideoDownloadModel
    {
        public Int64 TeleProposerId { get; set; }
        public Int64 ReferenceId { get; set; }
        public string FileSavePath { get; set; }
        public string FileType { get; set; }
        
    }

    public class BajajStatusUpdate
    {
        public Int64 AppointmentLogId { get; set; }
        public Int64 AppointmentId { get; set; }
        public string pScrutiny_no { get; set; }
        public string pAppointmentCode { get; set; }
        public string pCurrentStatus { get; set; }
        public string pRemark { get; set; }
        public string pDateOfRemarks { get; set; }
        public string pTimeOfRemarks { get; set; }
        public string pApptDate { get; set; }
        public string pApptTime { get; set; }
        public string pDcName { get; set; }
        public string pDcAddress { get; set; }
        public string pDcLocation { get; set; }
        public string pDcCity { get; set; }
        public string pDcState { get; set; }
        public string pFollowUpDate { get; set; }
        public string pHomeVisit { get; set; }
        public string pSecretKey { get; set; }
        public string pTpaName { get; set; }
        public string pFinalCallDate { get; set; }
        public string pFinalCallTime { get; set; }
        public string pFirstCallDate { get; set; }
        public string pFirstCallTime { get; set; }
        public string pCallbackRequestedByClient { get; set; }
        public string pMedicalStatus { get; set; }
        public string pDcPincode { get; set; }
        public string pDcContactNo { get; set; }
        public string pNablYN { get; set; }
        public string pDcEmail { get; set; }
        public string pReportUploadDate { get; set; }
        public string pCaseReferredPortal { get; set; }
        public string pAdditionalTest { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class HDFCERGOStatusUpdate
    {

        public Int32 AppointmentLogId { get; set; }
        public Int32 AppointmentId { get; set; }
         public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string StatusUpdatedTime { get; set; }
        public string AppointmentCode { get; set; }
        public Int32 AppointmentStatusId { get; set; }
        public Int32 InsuredDetailId { get; set; }
        public string CallDatetime { get; set; }
        public string CallStatusName { get; set; }
        public string CallRemarks { get; set; }
        public string CallerUsername { get; set; }
        public string DCUniqueCode { get; set; }
        public string DCName { get; set; }
        public string DCAddress1 { get; set; }
        //public string DCAddress2 { get; set; }
        //public string DCAddress3 { get; set; }
        public string DCCity { get; set; }
        public string DCState { get; set; }
        public string DCPincode { get; set; }
        public string DCEmailId { get; set; }
        public string DCContactNumber { get; set; }
        public string DCRemarks { get; set; }
        //public string UserName { get; set; }
        //public string Password { get; set; }
    }
    public class BajajReportUpload
    {
        public Int64 ReportAppointmentId { get; set; }
        public Int64 AppointmentId { get; set; }
        public string pAppointmentCode { get; set; }
        public string pScrutinyNo { get; set; }
        public string pTpaName { get; set; }
        public string pFileName { get; set; }
        public string imageBytes { get; set; }
        public string ReportType { get; set; }
        public string ProposerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class BajajStatusUpdateApiModel
    {
        public string pScrutiny_no { get; set; }
        public string pAppointmentCode { get; set; }
        public string pCurrentStatus { get; set; }
        public string pRemark { get; set; }
        public string pDateOfRemarks { get; set; }
        public string pTimeOfRemarks { get; set; }
        public string pApptDate { get; set; }
        public string pApptTime { get; set; }
        public string pDcName { get; set; }
        public string pDcAddress { get; set; }
        public string pDcLocation { get; set; }
        public string pDcCity { get; set; }
        public string pDcState { get; set; }
        public string pFollowUpDate { get; set; }
        public string pHomeVisit { get; set; }
        public string pSecretKey { get; set; }
        public string pTpaName { get; set; }
        public string pUserName { get; set; }
        public string pFinalCallDate { get; set; }
        public string pFinalCallTime { get; set; }
        public string pFirstCallDate { get; set; }
        public string pFirstCallTime { get; set; }
        public string pCallbackRequestedByClient { get; set; }
        public string pTpaRemark { get; set; }
        public string pMedicalStatus { get; set; }
        public string pDcPincode { get; set; }
        public string pDcContactNo { get; set; }
        public string pNablYN { get; set; }
        public string pDcEmail { get; set; }
        public string pReportUploadDate { get; set; }
        public string pCaseReferredPortal { get; set; }
        public string pAdditionalTest { get; set; }
    }

    public class HDFCERGOStatusUpdateApiModel
    {
        public Int32 InsuredDetailId { get; set; }
        public string CallDatetime { get; set; }
        //public string AppointmentCode { get; set; }
        public string CallStatusName { get; set; }
        public string CallRemarks { get; set; }
        public string CallerUsername { get; set; }
        public string DCUniqueCode { get; set; }
        public string DCName { get; set; }
        public string DCAddress1 { get; set; }
        public string DCAddress2 { get; set; }
        public string DCAddress3 { get; set; }
        public string DCCity { get; set; }
        public string DCState { get; set; }
        public string DCPincode { get; set; }
        public string DCEmailId { get; set; }
        public string DCContactNumber { get; set; }
        public string DCRemarks { get; set; }
    }
    public class BajajReportUploadApiModel
    {
        public string pAppointmentCode { get; set; }
        public string pScrutinyNo { get; set; }
        public string pTpaName { get; set; }
        public string pFileName { get; set; }
        public string imageBytes { get; set; }
    }

    public class BajajStatusUpdateApiModelTeleMER
    {
        public string pScrutiny_no { get; set; }
        public string pAppointmentCode { get; set; }
        public string pCurrentStatus { get; set; }
        public string pRemark { get; set; }
        public string pDateOfRemarks { get; set; }
        public string pTimeOfRemarks { get; set; }
        public string pApptDate { get; set; }
        public string pApptTime { get; set; }
        public string pDcName { get; set; }
        public string pDcAddress { get; set; }
        public string pDcLocation { get; set; }
        public string pDcCity { get; set; }
        public string pDcState { get; set; }
        public string pFollowUpDate { get; set; }
        public string pHomeVisit { get; set; }
        public string pSecretKey { get; set; }
        public string pTpaName { get; set; }
    }

    public class BajajReportUploadApiModelTeleER
    {
        public string pAppointmentCode { get; set; }
        public string pScrutinyNo { get; set; }
        public string pTpaName { get; set; }
        public string pFileName { get; set; }
        public string imageBytes { get; set; }
    }

    #region  Added by dheeraj for LIC
    public class LICReportUpload
    {
        public Int64 ReportAppointmentId { get; set; }
        public Int64 AppointmentId { get; set; }
        public string pFileName { get; set; }
        public string dataclassName { get; set; }
        public string imagevalue { get; set; }
        public string docname { get; set; }
        public string appid { get; set; }
        public string AcknowledgmentNumber { get; set; }
        public string dataclassProperties { get; set; }
        public string MSPName { get; set; }
        public string MSPRegNumber { get; set; }
        public string PrimaryProposalNumber { get; set; }
        public string BOCode { get; set; }
        public string DOCode { get; set; }
        public string ZOCode { get; set; }
        public string FinYear { get; set; }
        public string LAName { get; set; }
        public string DOB { get; set; }
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string DateOfExamination { get; set; }
        public string Status { get; set; }

    }



    public class LICReportUploadApiModel
    {
        public string dataclassName { get; set; }

        public string imagevalue { get; set; }
        public string docname { get; set; }
        public string appid { get; set; }
        public DataClassName dataclassProperties { get; set; }
    }

    public class DataClassName
    {
        public string AcknowledgmentNumber { get; set; }
        public string MSPName { get; set; }
        public string MSPRegNumber { get; set; }
        public string PrimaryProposalNumber { get; set; }
        public string BOCode { get; set; }
        public string DOCode { get; set; }
        public string ZOCode { get; set; }
        public string FinYear { get; set; }
        public string LAName { get; set; }
        public string DOB { get; set; }
        public string DocumentType { get; set; }
        public string DocumentName { get; set; }
        public string DateOfExamination { get; set; }
        public string Status { get; set; }




    }

    #endregion


    public class BajajLifeBillingAPI
    {

        public Int64 AppointmentId { get; set; }
        public string applicationNo { get; set; }
        public string finalApplicationNo { get; set; }
        public string clientName { get; set; }
        public string intimationDate { get; set; }
        public string firstCallToClientDate { get; set; }
        public string clientCity { get; set; }
        public string testCodesBalic { get; set; }
        public string testCodesTpa { get; set; }
        public string dcCode { get; set; }
        public string dcName { get; set; }
        public string dcCity { get; set; }
        public string appointmentDate { get; set; }
        public string reportUploadDate { get; set; }
        public string homeVisit { get; set; }
        public string areaMetroRural { get; set; }
        public string medicalFees { get; set; }
        public string homeVisitCharges { get; set; }
        public string serviceFees { get; set; }
        public string digitizationCharges { get; set; }
        public string interpretationCharges { get; set; }
        public string storageCharges { get; set; }
        public string totalMedicalFeeWithoutGst { get; set; }
        public string gstCharges { get; set; }
        public string totalMedicalFeeWithGst { get; set; }
        public string tatFromMedicalDoneToUploadDate { get; set; }
        public string tatFromIntimationToMedicalDone { get; set; }
        public string tatFromIntimationToReportUpload { get; set; }
        public string reportType { get; set; }
        public string billingMonth { get; set; }
        public string merType { get; set; }
        public string doctorName { get; set; }
        public string doctorCode { get; set; }
        public string doctorQualification { get; set; }
        public string tpaName { get; set; }
        public string channelName { get; set; }
        public string mainChannel { get; set; }
        public string policyStatus { get; set; }
        public string rejectionReason { get; set; }
        public string rejectionFlag { get; set; }
        public string rejectionRemark { get; set; }
        public string paymentType { get; set; }
        public string totalPremiumAmt { get; set; }
        public string fianlPayoutAmtDedOfMedicalChages { get; set; }
        public string policyRef { get; set; }
        public string premiumReceiptNo { get; set; }
        public string premiumReceiptDate { get; set; }
        public string channel { get; set; }
        public string changeDescription { get; set; }
        public string productId { get; set; }
        public string productName { get; set; }
        public string adjDate { get; set; }
        public string verticalCode { get; set; }


    }

    public class BajajLifeBillingAPIModel
    {
        public string applicationNo { get; set; }
        public string finalApplicationNo { get; set; }
        public string clientName { get; set; }
        public string intimationDate { get; set; }
        public string firstCallToClientDate { get; set; }
        public string clientCity { get; set; }
        public string testCodesBalic { get; set; }
        public string testCodesTpa { get; set; }
        public string dcCode { get; set; }
        public string dcName { get; set; }
        public string dcCity { get; set; }
        public string appointmentDate { get; set; }
        public string reportUploadDate { get; set; }
        public string homeVisit { get; set; }
        public string areaMetroRural { get; set; }
        public string medicalFees { get; set; }
        public string homeVisitCharges { get; set; }
        public string serviceFees { get; set; }
        public string digitizationCharges { get; set; }
        public string interpretationCharges { get; set; }
        public string storageCharges { get; set; }
        public string totalMedicalFeeWithoutGst { get; set; }
        public string gstCharges { get; set; }
        public string totalMedicalFeeWithGst { get; set; }
        public string tatFromMedicalDoneToUploadDate { get; set; }
        public string tatFromIntimationToMedicalDone { get; set; }
        public string tatFromIntimationToReportUpload { get; set; }
        public string reportType { get; set; }
        public string billingMonth { get; set; }
        public string merType { get; set; }
        public string doctorName { get; set; }
        public string doctorCode { get; set; }
        public string doctorQualification { get; set; }
        public string tpaName { get; set; }
        public string channelName { get; set; }
        public string mainChannel { get; set; }
        public string policyStatus { get; set; }
        public string rejectionReason { get; set; }
        public string rejectionFlag { get; set; }
        public string rejectionRemark { get; set; }
        public string paymentType { get; set; }
        public string totalPremiumAmt { get; set; }
        public string fianlPayoutAmtDedOfMedicalChages { get; set; }
        public string policyRef { get; set; }
        public string premiumReceiptNo { get; set; }
        public string premiumReceiptDate { get; set; }
        public string channel { get; set; }
        public string changeDescription { get; set; }
        public string productId { get; set; }
        public string productName { get; set; }
        public string adjDate { get; set; }
        public string verticalCode { get; set; }


    }

    public class BajajLifeBillingAPIModelList
    {
        public List<BajajLifeBillingAPIModel> autoNbRefundMedicalList = new List<BajajLifeBillingAPIModel>();
    }

    public class BALICMISAPI {
        public Int64 AppointmentId { get; set; }
        public string applicationNo { get; set; }
        public string clientName { get; set; }
        public string clientCity   { get; set; } 
        public string clientAddress  { get; set; } 
        public string testName  { get; set; } 
        public string comments  { get; set; } 
        public string tpaName  { get; set; } 
        public string medicalStatus   { get; set; } 
        public string doctorName   { get; set; } 
        public string callAttemptsCount   { get; set; } 
        public string dataReceivedByTpa  { get; set; } 
        public string updatedDate  { get; set; } 
        public string medicalDoneDate  { get; set; } 
        public string reportUploadDate  { get; set; } 
        public string appointmentDate  { get; set; } 
        public string visitType  { get; set; } 
        public string dcName  { get; set; } 
        public string dcCity  { get; set; } 
        public string dcId  { get; set; } 
        public string firstCallDate  { get; set; } 
        public string lastCallDate  { get; set; } 
        public string lastCallRemark  { get; set; } 
        public string lastCallComment  { get; set; } 
        public string medicalType  { get; set; } 
        public string pendingReason { get; set; }
    }

    public class BALICMISAPIMODEL
    {
        public string applicationNo { get; set; }
        public string clientName { get; set; }
        public string clientCity { get; set; }
        public string clientAddress { get; set; }
        public string testName { get; set; }
        public string comments { get; set; }
        public string tpaName { get; set; }
        public string medicalStatus { get; set; }
        public string doctorName { get; set; }
        public string callAttemptsCount { get; set; }
        public string dataReceivedByTpa { get; set; }
        public string updatedDate { get; set; }
        public string medicalDoneDate { get; set; }
        public string reportUploadDate { get; set; }
        public string appointmentDate { get; set; }
        public string visitType { get; set; }
        public string dcName { get; set; }
        public string dcCity { get; set; }
        public string dcId { get; set; }
        public string firstCallDate { get; set; }
        public string lastCallDate { get; set; }
        public string lastCallRemark { get; set; }
        public string lastCallComment { get; set; }
        public string medicalType { get; set; }
        public string pendingReason { get; set; }


    }

    public class BALICMISAPIMODELList
    {
        public List<BALICMISAPIMODEL> misDataList = new List<BALICMISAPIMODEL>();
    }

    public class MERFormModel
    {
        //public List<Filelststele> Filelststele = new List<Filelststele>();
        public List<Filelsts> Filelst = new List<Filelsts>();
        public List<AppointmentDetails> appointmentlst = new List<AppointmentDetails>();
        public List<FeedbackMER> feedBackMerlst = new List<FeedbackMER>();
        public List<FeedbackQuestionsMER> FeedbackQuestionsMERlst = new List<FeedbackQuestionsMER>();
        public List<FeedbackOptionsMER> feedBackOptionMerlst = new List<FeedbackOptionsMER>();
        public List<FeedbackQueOptMappingMER> queOptMappingMerlst = new List<FeedbackQueOptMappingMER>();
        public List<FeedbackMERQuestionAnswer> feedbackMERQuestionAnswerlst = new List<FeedbackMERQuestionAnswer>();
        public List<AppointmentTestvalues> AppointmentTestvalueslst = new List<AppointmentTestvalues>();
        public List<AptCtmtReportDetail> AptCtmtReportDetaillst = new List<AptCtmtReportDetail>();
        public List<ApttTestDetails> AppointmentTestDetails = new List<ApttTestDetails>();
    }

    public class Filelsts
    {
        public long AppointmentId { get; set; }
        public long TeleProposerId { get; set; }
        public string FileContent { get; set; }
        public string FileName { get; set; }
        public string file_name { get; set; }
        public string FileSavePath { get; set; }
        public int ProposerReferenceId { get; set; }
        public string FilePath { get; set; }
        public string PolicyRefNo { get; set; }

    }
    public class Filelststele
    {
      
        public string FileContent { get; set; }
        public string FileName { get; set; }
     
        public int ProposerReferenceId { get; set; }
        public string FilePath { get; set; }
  

    }

    public class ApttTestDetails
    {
        public long AppointmentId { get; set; }
        public string remarks { get; set; }
        public long QueId { get; set; }
        public string question { get; set; }

        public string Answer { get; set; }
        public string status { get; set; }
        public string testId { get; set; }
        public string testName { get; set; }
        public string testcategory { get; set; }
        public long referenceId { get; set; }

        public long TeleProposerId { get; set; }
        
    }

    public class AptCtmtReportDetail
    {
        public long AppointmentId { get; set; }
        public string grade { get; set; }
        public string phase { get; set; }
        public string remarks { get; set; }
        public string speed { get; set; }
        public string stage { get; set; }
        public string testName { get; set; }
        public string time { get; set; }
        public string totalTime { get; set; }
        public string workload { get; set; }
        public long referenceId { get; set; }
    }


    public class FeedbackMER
    {
        public long FeedbackMERId { get; set; }
        public long AppointmentId { get; set; }
        public long QueId { get; set; }
        public int OptionId { get; set; }
        public string Text { get; set; }
        public string FeedbackType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string DropdownValue { get; set; }
    }

    public class FeedbackQuestionsMER
    {
        public long QueId { get; set; }
        public int ClientId { get; set; }
        public string Question { get; set; }
        public string ConditionalName { get; set; }
        public string HasSubQue { get; set; }
        public long MainQueId { get; set; }
        public string PlanType { get; set; }
        public string Type { get; set; }
        public string TerminationQue { get; set; }
        public int QueOrder { get; set; }
        public string IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string IsMandatory { get; set; }
        public string SubQuestionOpensWhenAnswerIs { get; set; }
        public string ClientCode { get; set; }

    }

    public class FeedbackOptionsMER
    {
        public int FeedbackOptionsMERId { get; set; }
        public string FeedbackOptions { get; set; }
        public string DataType { get; set; }
        public string IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
        public int ClientId { get; set; }
        public string ClientCode { get; set; }
        public string ChildClientCode { get; set; }
    }


    public class FeedbackQueOptMappingMER
    {
        public int FeedbackQueOptMappingMERId { get; set; }
        public int QueId { get; set; }
        public int ClientId { get; set; }
        public int FeedbackOptionsMERId { get; set; }
        public int TypeId { get; set; }
        public string IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }



    public class AppointmentDetails
    {
        public long AppointmentId { get; set; }
        public int ClientId { get; set; }
        public int ProposerId { get; set; }

        public string FilePath { get; set; }
        public string PolicyRefNo { get; set; }
        public string CaseNo { get; set; }
        public string DocketNo { get; set; }
        public int AppointmentStatusId { get; set; }
        public string AppointmentType { get; set; }
        public string HomeVisitAllowed { get; set; }
        public string ProposerName { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string DoctorName { get; set; }
        public string AttendingDoctor { get; set; }
        public string RegistrationNo { get; set; }
        public string Place { get; set; }
        public string InternalNotes { get; set; }
        public string Date { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public string HomeVisit { get; set; }

        public string ApplicationNumber { get; set; }
        public string InsuredId { get; set; }
        public string MemberId { get; set; }
        public string PolicyNo { get; set; }

      public int ProposerReferenceId { get; set; }
        public string FileContent { get; set; }
        public string FileName { get; set; }

        #region for bagic
        public string dcaddress { get; set; }
        public string destinationSystem { get; set; }
        public string membername { get; set; }
        public string sourceSystem { get; set; }
        public string testtype { get; set; }
        public string vendorCode { get; set; }
        public string placeOfService { get; set; }

        public long TeleProposerId { get; set; } //added by aditya for TELE MER FORM

        #endregion

        #region For Call Log 
        public string CallDatetime { get; set; }
        public string CallStatusName { get; set; }
        public string CallRemarks { get; set; }
        public string CallerUsername { get; set; }
        public string DCUniqueCode { get; set; }
        public string DCName { get; set; }//
        public string DCAddress1 { get; set; }
        public string DCAddress2 { get; set; }
        public string DCAddress3 { get; set; }
        public string DCCity { get; set; }
        public string DCState { get; set; }
        public int DCPincode { get; set; }
        public string DCEmailId { get; set; }
        public string DCContactNumber { get; set; }
        public string DCRemarks { get; set; } 
        #endregion


    }

    public class FeedbackMERQuestionAnswer
    {
        public long FeedbackMERId { get; set; }
        public long AppointmentId { get; set; }
        public long QuestionId { get; set; }
        public long QueId { get; set; }
        public string Question { get; set; }
        public string QuestionsName { get; set; }
        public string HasSubQue { get; set; }
       // public strig Questions Name {get; set;}
        public int FeedbackMEROptionId { get; set; }
        public string MainQueClientCode { get; set; }
        public string Text { get; set; }
        public string status { get; set; }
        public string FeedbackOptions { get; set; }
        public string OptionClientCode { get; set; }
        public string FeedbackType { get; set; }
        public string DropdownValues { get; set; }
        public long MainQueId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string DropdownOptionClientCode { get; set; }
        public long OptionId { get; set; }
        public string QuestionSet { get; set; }
        public string ExaminationType { get; set; }
        public string Details { get; set; }
        public string Answer { get; set; }
        public long referenceId { get; set; }

        public long TeleProposerId { get; set; } //added by aditya for TELE MER FORM
    }

    public class AppointmentTestvalues
    {
        public long Appointmentid { get; set; }
        public string TestName { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string MaxValue { get; set; }
        public string MinValue { get; set; }
        public string Remark { get; set; }

        #region for bagic

        public string documentUrl { get; set; }
        public string externalId { get; set; }
        public string limit { get; set; }
        public string loincCode { get; set; }
        public string ranges { get; set; }
        public string recommendation { get; set; }
        public string resultValue { get; set; }
        public string testPerformedDate { get; set; }
        public string testResultNotes { get; set; }
        public string testcomponent { get; set; }
        public string testtype { get; set; }
        public long referenceId { get; set; }
        #endregion
    }



    public class CtmtReportDetail
    {
        public string grade { get; set; }
        public string phase { get; set; }
        public string remarks { get; set; }
        public string speed { get; set; }
        public string stage { get; set; }
        public string testName { get; set; }
        public string time { get; set; }
        public string totalTime { get; set; }
        public string workload { get; set; }
        //public long referenceId { get; set; }
    }

    public class MedicalExaminationDetail
    {
        public string answer { get; set; }
        public string details { get; set; }
        public string examinationType { get; set; }
        public string question { get; set; }
        public string questionCode { get; set; }
        public string questionSet { get; set; }
    }
    public class DiagnosisDetails
    {
        public string QuestionRefCode { get; set; }
        public string QuestionTitle { get; set; }
        public string DiagnosisName { get; set; }
        public string DiagnosisDate { get; set; }
        public string Consultationdate { get; set; }
        public string LineofTreatment { get; set; }

        public string TreatmentDetails { get; set; }
    }

    public class Testresult
    {
        public string documentUrl { get; set; }
        public string externalId { get; set; }
        public string limit { get; set; }
        public string loincCode { get; set; }
        public string max_range { get; set; }
        public string min_range { get; set; }
        public string ranges { get; set; }
        public string recommendation { get; set; }
        public string remarks { get; set; }
        public string resultValue { get; set; }
        public string testPerformedDate { get; set; }
        public string testResultNotes { get; set; }
        public string testcomponent { get; set; }
        public string testtype { get; set; }
        public string unitOfMeasurement { get; set; }
    }

    public class TestDetail
    {
        public List<CtmtReportDetail> ctmtReportDetails { get; set; }
        public List<MedicalExaminationDetail> medicalExaminationDetails { get; set; }
        public string remarks { get; set; }
        public string status { get; set; }
        public string testId { get; set; }
        public string testcategory { get; set; }
        public List<Testresult> testresults { get; set; }
    }

    public class MerDtAnswerResponses
    {
        
        
        public string QuestionRefCode { get; set; }
        public string QuestionTitle { get; set; }
        public string ResponseStatus { get; set; }
        public string ResponseRemarks { get; set; }
       
    }

    public class PPMCBAGICDigitalInformation
    {
        public string appointmentDate { get; set; }
        public string appointmentId { get; set; }
        public string dcaddress { get; set; } //
        public string destinationSystem { get; set; } //
        public string gender { get; set; }
        public string isHNI { get; set; }
        public string membercode { get; set; }
        public string membername { get; set; }
        public string placeOfService { get; set; } //
        public string proposalNumber { get; set; }
        public string sourceSystem { get; set; } //
        public string testtype { get; set; } //
        public string vendorCode { get; set; } //
        public List<TestDetail> testDetails { get; set; }

        public string TeleProposerId { get; set; } //Added by Aditya FOR TELE MER FORM BAGIC         

    }

    public class HDFCERGODigitalInformation
    {
        public int InsuredDetailId { get; set; }
        public string FileContent { get; set; }
        public string FileName { get; set; }

        public List<MerDtAnswerResponses> MerDtAnswerResponses { get; set; }
        public List<DiagnosisDetails> DiagnosisDetails { get; set; }


        //public string TeleProposerId { get; set; } //Added by Aditya FOR TELE MER FORM BAGIC         

    }
    public class AuthorizationDataModel
    {
        public string userName { get; set; }
        public string userPassword { get; set; }        

    }

 
    public class UserInfo
    {
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public object UserStatus { get; set; }
        public int FailedLogons { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime ActiveTo { get; set; }
        public int ApplicantId { get; set; }
        public string DefaultRoleCode { get; set; }
        public int LoginHistoryId { get; set; }
        public DateTime PreviousLogonDate { get; set; }
        public int Id { get; set; }
        public string InternalKey { get; set; }
        public object ConcurrencyKey { get; set; }
        public int IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public object UpdateDate { get; set; }
        public int CreateUserId { get; set; }
        public object UpdateUserId { get; set; }
    }

    public class Role
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class PermissionActionPermission
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ExtraData
    {
        public UserInfo UserInfo { get; set; }
        public List<Role> Roles { get; set; }
        public List<PermissionActionPermission> PermissionActionPermissions { get; set; }
        public string SecureToken { get; set; }
        public object RefreshToken { get; set; }
    }

    public class AUthorizationModelResponse
    {
        public int StatusCode { get; set; }
        public int Status { get; set; }
        public List<object> Errors { get; set; }
        public object Message { get; set; }
        public object RedirectUrlOk { get; set; }
        public object RedirectUrlNext { get; set; }
        public ExtraData ExtraData { get; set; }
        public int ResponseType { get; set; }
        public object Name { get; set; }
        public object MessageCode { get; set; }
      
    }


    public class BAGICDigitalDataResponse
    {
        List<Errors> Errors = new List<Errors>();
        public string Result { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public Messages messages { get; set; }
    }

    public class HDFCERGODigitalDataResponse
    {
    
    
        public string StatusCode { get; set; }
        public string Status { get; set; }
   
       
        List<Errors> Errors = new List<Errors>();
        public string Message { get; set; }
        public string RedirectUrlOk { get; set; }
        public string RedirectUrlNext { get; set; }
        
        
        public string ExtraData { get; set; }
        public string ResponseType { get; set; }
        public string Name { get; set; }
        //public string responseMessage { get; set; }
        public string MessageCode { get; set; }
        public string Result { get; set; }

        // public Messages messages { get; set; }
    }
    public class Messages
    {
        public string messageCode { get; set; }
        public string messageDescription { get; set; }
        public string ErrorType { get; set;}
        public string ErrorCode { get; set; }
    }

    public class HDFCERGOCALLLOGInformation
    {
        public int InsuredDetailId { get; set; }
        public string CallDatetime { get; set; }
        public string CallStatusName { get; set; }
        public string CallRemarks { get; set; }
        public string CallerUsername { get; set; }
        public string DCUniqueCode { get; set; }
        public string DCName { get; set; }
        public string DCAddress1 { get; set; }
        public string DCAddress2 { get; set; }
        public string DCAddress3 { get; set; }
        public string DCCity { get; set; }
        public string DCState { get; set; }
        public int DCPincode { get; set; }
        public string DCEmailId { get; set; }
        public string DCContactNumber { get; set; }
        public string DCRemarks { get; set; }
    }
    public class ParentLevelQuestionCode
    {
        public string QuestionCode { get; set; }
        public long QueId { get; set; }

        public List<AnswersMaster> Answers = new List<AnswersMaster>();
        public List<ChildLevel1QuestionCode> ChildLevel1QueCode = new List<ChildLevel1QuestionCode>();
    }

    public class ChildLevel1QuestionCode
    {
        public string QuestionCode { get; set; }
        public string ClientCode { get; set; }
        public long QueId { get; set; }

        public List<AnswersMaster> Answers = new List<AnswersMaster>();

        public List<ChildLevel2QuestionCode> ChildLevel2QueCode = new List<ChildLevel2QuestionCode>();
    }

    public class ChildLevel2QuestionCode
    {
        public string QuestionCode { get; set; }
        public string ClientCode { get; set; }
        public long QueId { get; set; }

        public List<AnswersMaster> Answers = new List<AnswersMaster>();

        public List<ChildLevel3QuestionCode> ChildLevel2QueCode = new List<ChildLevel3QuestionCode>();
    }

    public class ChildLevel3QuestionCode
    {
        public string QuestionCode { get; set; }
        public string ClientCode { get; set; }
        public long QueId { get; set; }

        public List<AnswersMaster> Answers = new List<AnswersMaster>();
    }

    public class QuestionCodeMapping
    {
        public List<ParentLevelQuestionCode> lst = new List<ParentLevelQuestionCode>();
    }

    public class AnswersMaster
    {
        public string AnswerCode { get; set; }
        public string Remark { get; set; }
        public long AnswerId { get; set; }
    }


    public class Errors
    {
        
      //  public string Code { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorType { get; set; }
        public string Message { get; set; }
    }

    public class TPAInformation { 
        public int MAHSRequestId { get; set; }
        public string TestCategory { get; set; }
        public string TPAStatus { get; set; }
    }


    public class TPAInformationModel
    {
        public int MAHSRequestId { get; set; }
        public string TestCategory { get; set; }
        public string TPAStatus { get; set; }
    }

    public class AuthDataModel {
        public string grantType { get; set; } 
        public string clientInfo { get; set; } 
    }
    public class AUthorizationResponseModel {

        public object clainsString { get; set; }
        public string access_token { get; set; }
        public object refresh_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}
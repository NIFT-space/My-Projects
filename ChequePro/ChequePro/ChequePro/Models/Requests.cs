namespace ChequePro.Models
{
    public class Requests
    {
    }

    public class User_Login
    {
        public string H_AuthToken { get; set; }
        public string H_AuthSecretKey { get; set; }
        public int BankID { get; set; }
        public int BranchID { get; set; }
        public long AccountID { get; set; }
    }

    public class AuthDetails
    {
        public int AuthID { get; set; }
        public int IsFirstLogin { get; set; }
        public DateTime AuthTokenExpireOn { get; set; }
    }

    public class ChequeSummary
    {
        public long hostid { get; set; }
        public string ChequeNo { get; set; }
        public string AccountNo { get; set; }
        public string Amount { get; set; }
        public string Processdate { get; set; }
        public string receiverbank { get; set; }
        public string receiverbranch { get; set; }
        public string senderbank { get; set; }
        public string senderbranch { get; set; }
        public string reasonid { get; set; }
        public string reason { get; set; }
        public string ReceiverBankBranchName { get; set; }
    }

    public class ChequeDetails
    {
        public long hostid { get; set; }
        public string ChequeNo { get; set; }
        public string AccountNo { get; set; }
        public string SequenceNo { get; set; }
        public string Amount { get; set; }
        public string Processdate { get; set; }
        public string cycleno { get; set; }
        public string CityID { get; set; }
        public string receiverbank { get; set; }
        public string receiverbranch { get; set; }
        public string reasonid { get; set; }
        public string reason { get; set; }
        public string isDeffer { get; set; }
        public string isauth { get; set; }
        public string Undersize_Image { get; set; }
        public string FoldedDocCorners { get; set; }
        public string FoldedDocEdges { get; set; }
        public string Framing_Error { get; set; }
        public string DocSkew { get; set; }
        public string Oversize_Image { get; set; }
        public string Piggy_Back { get; set; }
        public string Image_Too_Light { get; set; }
        public string Image_Too_Dark { get; set; }
        public string Horizontal_Streaks { get; set; }
        public string BelowMinImgsize { get; set; }
        public string AboveMaxImgsize { get; set; }
        public string Spot_Noise { get; set; }
        public string FrontRearMismatch { get; set; }
        public string Carbon_Strip { get; set; }
        public string Out_of_Focus { get; set; }
        public string IQATag { get; set; }
        public string barcodeMatch { get; set; }
        public string UVStr { get; set; }
        public string Duplicate { get; set; }
        public string MICR_Present { get; set; }
        public string Average_Amount { get; set; }
        public string STD_Non_STD { get; set; }
        public string Water_Mark { get; set; }
        public string isFraud { get; set; }
        public byte[] FrontImage { get; set; }
        public byte[] BackImage { get; set; }
        public byte[] UVImage { get; set; }
        public string ReceiverBankName { get; set; }
        public string ReceiverBranchName { get; set; }
        public string TransCode { get; set; }
        public string UVPercent { get; set; }
        public string UVTemplateID { get; set; }
    }
}

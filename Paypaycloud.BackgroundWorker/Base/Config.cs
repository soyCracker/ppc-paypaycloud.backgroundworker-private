using Amazon;

namespace Paypaycloud.BackgroundWorker.Base
{
    public class Config
    {
        public static string Ver = "BackgroundWorker-1.0.1";

        public static bool IsReleaseMode = true;

        //S3
        public static string S3BucketNamePro = "paypaycloudpayment-pro";
        public static string S3BucketNameUat = "paypaycloudpayment-uat";
        public static RegionEndpoint S3BucketRegion = RegionEndpoint.USEast1;
        public static string AwsAccessKey = "AKIAJL7SBSJ42XPAQNSA";
        public static string AwsSecAccessKey = "rpob33vgcjdT7ATwrH7rY3icLhXde3DCWIA1ToFT";

    }
}

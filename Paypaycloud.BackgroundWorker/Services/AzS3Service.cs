using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Paypaycloud.BackgroundWorker.Base;
using Paypaycloud.BackgroundWorker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Paypaycloud.BackgroundWorker.Services
{
    public class AzS3Service
    {
        private string s3BucketName = Config.S3BucketNamePro;
        private IAmazonS3 s3Client;
        private ILogger logger;

        public AzS3Service(ILogger logger)
        {
            this.logger = logger;
            InitAzS3();
        }

        private void InitAzS3()
        {
            if (!Config.IsReleaseMode)
            {
                s3BucketName = Config.S3BucketNameUat;
            }
            s3Client = new AmazonS3Client(Config.AwsAccessKey, Config.AwsSecAccessKey, Config.S3BucketRegion);
        }

        public async Task CreateCertificateAsync(List<CertificateTwins> twins)
        {
            try
            {
                logger.LogDebug("AzS3Service CreateCertificateAsync Start\n");
                foreach (CertificateTwins twin in twins)
                {
                    CertificateTwins twinRe = AddProtectedServers(twin);
                    await S3UploadFile(twinRe.Updater);
                    await S3UploadFile(twinRe.App);
                }
                logger.LogDebug("AzS3Service CreateCertificateAsync End\n");
            }
            catch(Exception e)
            {
                logger.LogError("AzS3Service CreateCertificateAsync:"+ e);
            }
        }

        private string GetS3FileName(string clientName)
        {
            string[] words = clientName.Split('_');
            return "IPCSecurityFiles/IPC_" + words[1] + "/" + clientName + ".json";
        }

        private async Task S3UploadFile(CertificateClient certificate)
        {
            try
            {                
                TransferUtility fileTransferUtility = new TransferUtility(s3Client);
                using (MemoryStream ms = new MemoryStream())
                {
                    string jsonStr = JsonConvert.SerializeObject(certificate);
                    logger.LogDebug("AzS3Service S3UploadFile jsonStr:" + jsonStr);
                    StreamWriter outputFile = new StreamWriter(ms);
                    outputFile.WriteLine(jsonStr);
                    outputFile.Flush();
                    await fileTransferUtility.UploadAsync(ms, s3BucketName, GetS3FileName(certificate.ClientName));
                }
            }
            catch (AmazonS3Exception e)
            {
                logger.LogError("AzS3Service S3UploadFile AmazonS3Exception:" + e);
            }
            catch (Exception e)
            {
                logger.LogError("AzS3Service S3UploadFile Exception:" + e);
            }
        }

        public void TestCount()
        {
            for (int i = 0; i < 10; i++)
            {
                logger.LogDebug("TestCount *************** " + i);
                Thread.Sleep(10);
            }
        }

        private void TestCreateFile()
        {
            CertificateClient certificate = new CertificateClient();
            certificate.ClientId = "6365964288277830760001";
            certificate.ClientName = "IPC_8400503587204034044E";
            certificate.ClientKey = "9C83211C510C350A49BDB8695936C6B5";
            certificate.ClientIV = "5B87FBD852AE85E7";
            string lines = JsonConvert.SerializeObject(certificate);

            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            logger.LogDebug("TestCreateFile *************** " + docPath);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "WriteLines.txt")))
            {
                outputFile.WriteLine(lines);
            }
        }

        private CertificateTwins AddProtectedServers(CertificateTwins twin)
        {
            twin.Updater.ProtectedServers.Clear();
            twin.Updater.ProtectedServers.Add(GetMaintenanceServer());
            twin.App.ProtectedServers.Clear();
            twin.App.ProtectedServers.Add(GetMaintenanceServer());
            twin.App.ProtectedServers.Add(GetTransServer());
            return twin;
        }

        private CertificateProtectedServers GetMaintenanceServer()
        {
            CertificateProtectedServers maintenance = new CertificateProtectedServers();
            maintenance.ServerId = "6365653900499473450001";
            maintenance.ServerName = "uatcommmaintenance";
            if(Config.IsReleaseMode)
            {
                maintenance.ServerURL = "http://commmaintenance.paypaycloud.com/";
            }
            else
            {
                maintenance.ServerURL = "http://uatcommmaintenance.paypaycloud.com/";
            }
            return maintenance;
        }

        private CertificateProtectedServers GetTransServer()
        {
            CertificateProtectedServers trans = new CertificateProtectedServers();
            trans.ServerId = "6365602916349767730001";
            trans.ServerName = "uatcommtrans";
            if(Config.IsReleaseMode)
            {
                trans.ServerURL = "http://commtrans.paypaycloud.com/";
            }
            else
            {
                trans.ServerURL = "http://uatcommtrans.paypaycloud.com/";
            }
            return trans;
        }
    }
}

using Newtonsoft.Json;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFTPBulkUpdateService
{
    public class SFTPFileTransferUtility
    {
        public SFTPFileTransferUtility() {}

        public void MoveSftpBulkData()
        {
            List<BulkUpdateModel> model = GetBulkUpdateData().Result;
            TargetFolderLocation targetFolderLocation = null;
            LogUtility.WriteToFile("DataCount:" + model.Count);
            if (model != null && model.Count > 0)
            {
                try
                {
                    LogUtility.WriteToFile("Inside :" + JsonConvert.SerializeObject(model[0]));
                    targetFolderLocation = GetTargetFolderLocation(model[0]).Result;
                    if (targetFolderLocation != null && !string.IsNullOrEmpty(targetFolderLocation.targetFolderLocation))
                    {
                       TransferFileFromSFTP(targetFolderLocation.targetFolderLocation, model[0].filePath);
                       BulkUpdateResponseModel bulkUpdateModel = UpdateBulkUpdateData(model[0]).Result;
                       LogUtility.WriteToFile("SFTP Bulk Data Update:" + model[0].ToString());
                    }
                }catch (Exception)
                {
                    if (targetFolderLocation != null && targetFolderLocation.ridAttachment != 0)
                    {
                        DeleteResponseModel delModel = DeleteAttachment(targetFolderLocation.ridAttachment).Result;
                        LogUtility.WriteToFile("SFTP Bulk Attachment Deleted:" + targetFolderLocation.ridAttachment + targetFolderLocation.targetFolderLocation);
                    }
                    throw;
                }
            }
        }

        public void TransferFileFromSFTP(string localPath, string remotePath)
        {
            LogUtility.WriteToFile("TransferFileFromSFTP:" + localPath);
            string host = ConfigurationManager.AppSettings["Host"]; 
            string username = ConfigurationManager.AppSettings["Username"];
            string password = ConfigurationManager.AppSettings["Password"];
            string localFilePath = localPath;
            //ConfigurationManager.AppSettings["LocalFilePath"];
            string remoteFilePath = remotePath;
            //"/X0232-ERL-EGS-CL-XXXXX.pdf";


            using (SftpClient client = new SftpClient(host, 22, username, password))
            {
                try
                {
                    client.Connect();

                    LogUtility.WriteToFile("SFTP Bulk Connection and Remote File Path:" + remoteFilePath);                    

                    using (Stream fileStream = File.Create(localFilePath))
                    {
                        Console.WriteLine(fileStream.ToString());
                        client.DownloadFile(remoteFilePath, fileStream);
                        //client.Delete(remoteFilePath);
                    }

                    client.Disconnect();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        
        public async Task<List<BulkUpdateModel>> GetBulkUpdateData()
        {
            LogUtility.WriteToFile("Inside GetBulkUpdateData");
            ResponseModel respModel = new ResponseModel();
            
                using (var client = new HttpClient())
            {
                string ApiKey = ConfigurationManager.AppSettings["ApiKey"];
                string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
                string GetPendingBulkDataUrl = client.BaseAddress + string.Format("BulkData/GetPendingBulkData?Count={0}", 1);
                LogUtility.WriteToFile("Url is " + GetPendingBulkDataUrl); 
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                client.DefaultRequestHeaders.TryAddWithoutValidation("ApiKey", ApiKey);
                //GET Method  
                HttpResponseMessage response = await client.GetAsync(GetPendingBulkDataUrl);
                if (response.IsSuccessStatusCode)
                {
                    var cont = await response.Content.ReadAsStringAsync();
                    LogUtility.WriteToFile(cont);
                    try
                    {
                        respModel = JsonConvert.DeserializeObject<ResponseModel>(cont);
                    }
                    catch (Exception ex)
                    {
                        LogUtility.WriteToFile("Failed to deserialize:" + ex.Message + ex.StackTrace);
                        throw;
                    }
                    
                    //LogUtility.WriteToFile(respModel.statusCode);
                    //LogUtility.WriteToFile(respModel.message);
                    //LogUtility.WriteToFile("DataCount:" + respModel.data.Count);
                }
               
            }
            return respModel.data;
        }

        public async Task<TargetFolderLocation> GetTargetFolderLocation(BulkUpdateModel model)
        {
            TargetFolderResponseModel respModel = new TargetFolderResponseModel();
       
            using (var client = new HttpClient())
            {
                string ApiKey = ConfigurationManager.AppSettings["ApiKey"];
                string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
                string GetPendingBulkDataUrl = client.BaseAddress + string.Format("Attachment/MoveBulkData?CRN={0}&category={1}&SaveFilename={2}", model.crn, model.type, model.fileName); ;
                LogUtility.WriteToFile(GetPendingBulkDataUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("ApiKey", ApiKey);
                //client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("Bearer {0}", Service1.IsUserLoginToken));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                //GET Method  
                HttpResponseMessage response = await client.GetAsync(GetPendingBulkDataUrl);
                if (response.IsSuccessStatusCode)
                {
                    var cont = await response.Content.ReadAsStringAsync();
                    LogUtility.WriteToFile(cont);
                    respModel = JsonConvert.DeserializeObject<TargetFolderResponseModel>(cont);
                }

            }
            return respModel.data;
        }

        public async Task<DeleteResponseModel> DeleteAttachment(int attachmentID)
        {
            DeleteResponseModel respModel = new DeleteResponseModel();

            using (var client = new HttpClient())
            {
                string ApiKey = ConfigurationManager.AppSettings["ApiKey"];
                string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
                string GetDeleteAttachmentUrl = client.BaseAddress + string.Format("Attachment/DeleteForBulk?AttachmentID={0}", attachmentID); ;
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.TryAddWithoutValidation("ApiKey", ApiKey);
                
           
                //Delete Method  
                HttpResponseMessage response = await client.DeleteAsync(GetDeleteAttachmentUrl);
                if (response.IsSuccessStatusCode)
                {
                    var cont = await response.Content.ReadAsStringAsync();
                    respModel = JsonConvert.DeserializeObject<DeleteResponseModel>(cont);
                }
            }
            return respModel;
        }

        public async Task<BulkUpdateResponseModel> UpdateBulkUpdateData(BulkUpdateModel model)
        {
            BulkUpdateResponseModel respModel = new BulkUpdateResponseModel();

            using (var client = new HttpClient())
            {
                string ApiKey = ConfigurationManager.AppSettings["ApiKey"];
                string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
                string UpdateBulkDataUrl = client.BaseAddress + string.Format("BulkData/Update");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("ApiKey", ApiKey);
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

                //HTTP POST
                model.isProcessed = "Y";
                string strPayload = JsonConvert.SerializeObject(model);
                LogUtility.WriteToFile("OCR Process Request PayLoad:" + strPayload);
                HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(UpdateBulkDataUrl, c).Result;
                LogUtility.WriteToFile("OCR Process Response:" + response);
                if (response.IsSuccessStatusCode)
                {
                    var cont = await response.Content.ReadAsStringAsync();
                    respModel = JsonConvert.DeserializeObject<BulkUpdateResponseModel>(cont);
                }
            }
            return respModel;
        }

        //public async Task<string> GetAccessToken()
        //{
        //    UserResponse respModel = new UserResponse();

        //    using (var client = new HttpClient())
        //    {
        //        string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        //        client.BaseAddress = new Uri(baseUrl);
        //        string UpdateBulkDataUrl = client.BaseAddress + string.Format("Usermaster/AuthenticateUser");
        //        client.DefaultRequestHeaders.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        //        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

        //        //HTTP POST
        //        User user = new User();
        //        user.username = ConfigurationManager.AppSettings["LoginUser"];
        //        user.password = ConfigurationManager.AppSettings["LoginUserPwd"];
        //        string strPayload = JsonConvert.SerializeObject(user);
        //        LogUtility.WriteToFile("Get Access Token PayLoad:" + strPayload);
        //        HttpContent c = new StringContent(strPayload, Encoding.UTF8, "application/json");
        //        HttpResponseMessage response = client.PostAsync(UpdateBulkDataUrl, c).Result;
        //        LogUtility.WriteToFile("Get Access Token Response:" + response);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var cont = await response.Content.ReadAsStringAsync();
        //            respModel = JsonConvert.DeserializeObject<UserResponse>(cont);
        //        }
        //    }
        //    return respModel.data[0].token;
        //}
    }
}


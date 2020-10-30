using System;
using System.Collections.Generic;

namespace SFTPBulkUpdateService
{

    public class BulkUpdateModel
    {
        public int ridBulkData { get; set; }
        public string crn { get; set; }
        public string type { get; set; }
        public string fileName { get; set; }
        public string filePath { get; set; }
        public string isProcessed { get; set; }
        public int ridUsermaster { get; set; }
        public DateTime addedOn { get; set; }
        public string uploadBatch { get; set; }
        public string isactive { get; set; }
    }

    public class ResponseModel
    {
        public List<BulkUpdateModel> data { get; set; }
        public string message { get; set; }
        public string statusCode { get; set; }
    }

    public class TargetFolderLocation
    {
        public int ridAttachment { get; set; }
        public string targetFolderLocation { get; set; }
    }

    public class TargetFolderResponseModel
    {
        public TargetFolderLocation data { get; set; }
        public string message { get; set; }
        public string statusCode { get; set; }
    }

    public class DeleteResponseModel
    {
        public int data { get; set; }
        public string message { get; set; }
        public string statusCode { get; set; }

    }

    public class BulkUpdateResponseModel
    {
        public int data { get; set; }
        public string message { get; set; }
        public string statusCode { get; set; }

    }

    public class User
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class LoginUser
    {
        public int ridRoles { get; set; }
        public string rolename { get; set; }
        public int ridEntityType { get; set; }
        public string entityTypeName { get; set; }
        public int ridEntityList { get; set; }
        public string entityCode { get; set; }
        public string entityName { get; set; }
        public int ridUsermaster { get; set; }
        public string isaccountenabled { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public object middlename { get; set; }
        public string email { get; set; }
        public string isaduser { get; set; }
        public string username { get; set; }
        public string appointedas { get; set; }
        public int ridLanguage { get; set; }
        public string languagename { get; set; }
        public string isactive { get; set; }
        public object addedby { get; set; }
        public object addedon { get; set; }
        public object updatedby { get; set; }
        public object updatedon { get; set; }
        public object profileImage { get; set; }
        public object rootEntityId { get; set; }
        public string token { get; set; }
    }

    public class UserResponse
    {
        public List<LoginUser> data { get; set; }
        public string message { get; set; }
        public string statusCode { get; set; }
    }
}



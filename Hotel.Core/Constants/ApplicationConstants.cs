﻿namespace Hotel.Core.Constants
{
    public class ApplicationConstants
    {
        public const string KEYID_EXISTED = "KeyId {0} đã tồn tại.";
        public const string KeyId = "KeyId";
        public const string DUPLICATE = "Symtem_id is duplicated";
        public const int BREAK_TIME = 1;
        public const int WORKINGSTEP = 1;
    }

    public class ResponseCodeConstants
    {
        public const string NOT_FOUND = "Not found!";
        public const string SUCCESS = "Success!";
        public const string FAILED = "Failed!";
        public const string EXISTED = "Existed!";
        public const string DUPLICATE = "Duplicate!";
        public const string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";
        public const string INVALID_INPUT = "Invalid input!";
        public const string UNAUTHORIZED = "Unauthorized!";
        public const string FORBIDDEN = "Forbidden!";
        public const string BADREQUEST = "Bad request!";
        public const string INVALID_TOKEN = "Invalid token!";
    }
    public static class FirebaseConstants
    {
        public const string ArticleFolder = "Article";
        public const string FinalFileFolder = "FinalFile";
        public const string PostFolder = "Post";
        public const string ProductFileFolder = "ProductFile";
        public const string RegistrationFormsFolder = "RegistrationForms";
        public const string ReportFileFolder = "ReportFile";
        public static readonly List<string> AllFolders = new List<string> { ArticleFolder, FinalFileFolder, ProductFileFolder, ReportFileFolder, RegistrationFormsFolder, PostFolder };
    }
}

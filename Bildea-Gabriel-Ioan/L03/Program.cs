using System;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis;
using System.Threading;
using System.Net;
using Newtonsoft.Json.Linq;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace L03
{
    class Program
    {
        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "DATC Bildea Gabriel Ioan";
        static UserCredential credential;
        static DriveService service;

        static string fileName = "test.txt";
        static string folderId = "17VNgWaK-KrkwiF7GeIO6CspOYkv9c1hu"; //TestFOLDER folder in drive

        static void Main(string[] args)
        {
            Initialize();

            GetAllFiles();
            UploadFile().GetAwaiter().GetResult();
        }

        static void Initialize()
        {
            using (var stream =  new FileStream("client_id.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    Environment.UserName,
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        static void GetAllFiles()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/drive/v3/files?q='root'%20in%20parents"); 
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + credential.Token.AccessToken);
            
            using(var response = request.GetResponse())
            {
                using(Stream data  = response.GetResponseStream())
                using(var reader = new StreamReader(data))
                {
                    string text = reader.ReadToEnd();
                    var myData = JObject.Parse(text);
                    foreach(var file in myData["files"])
                    {
                        if(file["mimeType"].ToString() != "application/vnd.google-apps.folder")
                        {
                            Console.WriteLine("File name: " + file["name"]);
                        }
                    }
                }
            }
        }

        public static async Task<Google.Apis.Drive.v3.Data.File> UploadFile()
        {
            var name = ($"{DateTime.UtcNow.ToString()}.txt");
            var mimeType = "text/plain";

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = name,
                MimeType = mimeType,
                Parents = new[] { folderId }
            };
            
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            FilesResource.CreateMediaUpload request = service.Files.Create(fileMetadata, stream, mimeType);
            request.Fields = "id, name, parents, createdTime, modifiedTime, mimeType, thumbnailLink";
            await request.UploadAsync();
   
            return request.ResponseBody;
        }
    }
}

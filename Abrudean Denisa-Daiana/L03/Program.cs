using System;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Google.Apis.Util.Store;

namespace L03
{
    class Program
    {
        private static DriveService _service;
        private static string _token;

        static void Main(string[] args)
        {
           Initialize();
        }

        static void Initialize()

        {   

            string[] scopes=new string[]{
                DriveService.Scope.Drive,
                DriveService.Scope.DriveFile
            };


            var clientId="916453973888-3d9i5vbc5afcti2l42reous8nk51i31b.apps.googleusercontent.com";
            var clientSecret="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";


              var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                scopes,
                Environment.UserName,
                CancellationToken.None,

                new FileDataStore("Daimto.GoogleDrive.Auth.Store")
            ).Result;
            _service = new DriveService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });

            _token = credential.Token.AccessToken;
            Console.Write("Token: " + credential.Token.AccessToken);
            GetMyFiles();


        }

          static void GetMyFiles()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/drive/v3/files?q='root'%20in%20parents");
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + _token);

            using (var response = request.GetResponse())
            {
                using (Stream data = response.GetResponseStream())
                using (var reader = new StreamReader(data))
                {
                    string text = reader.ReadToEnd();
                    var myData = JObject.Parse(text);
                    foreach (var file in myData["files"])
                    {
                        if (file["mimeType"].ToString() != "application/vnd.google-apps.folder")
                        {
                            Console.WriteLine("File name: " + file["name"]);
                        }
                    }
                }
            }
        }

        static void UploadFile(){
            var body=new Google.Apis.Drive.v3.Data.File();
            body.Name="DATC-2021.txt";
            body.MimeType="text/plain";


            var byteArray=File.ReadAllBytes("DATC-2021.txt");
            System.IO.MemoryStream stream= new System.IO.MemoryStream(byteArray);

            var request=_service.Files.Create(body, stream, "text/plain");
            request.Upload();
            Console.WriteLine(request.ResponseBody);

        }
    }
}

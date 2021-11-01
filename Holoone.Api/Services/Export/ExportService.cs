﻿using Flurl.Http;
using Flurl.Http.Configuration;
using Holoone.Api.Helpers.Constants;
using Holoone.Api.Helpers.Extensions;
using Holoone.Api.Models;
using Holoone.Api.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Services
{
    public class Folder
    {
        public string Name { get; set; }
        public IList<OFile> ListofFiles { get; set; }
    }


    public class OFile
    {

        public string Name { get; set; }
        public string ext { get; set; }
        public string path { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ExportService : IExportService
    {
        private readonly IFlurlClient _flurlClient;

        public ExportService(IFlurlClientFactory flurlClientFac)
        {
            _flurlClient = flurlClientFac.Get(RequestConstants.BaseUrl);
        }

        // version 1 (Not working)
        public async Task<IFlurlResponse> ExportModelAsync(UserLogin user, MediaItem mediaItem, string filePath)
        {
            _flurlClient.BaseUrl = RequestConstants.BaseUrl;

            //using (FileStream fs = File.Open(filePath, FileMode.Open))
            //{
            string jsonFormData = JsonConvert.SerializeObject(mediaItem);
            string jsonProcessingParams = JsonConvert.SerializeObject(mediaItem.RequestProcessingParams);

            //return await _flurlClient.Request("media/add/file/")
            //       .WithBasicAuth("baki.test@holo-one.com", "g6hN!(3#")
            //       // .WithHeader("Content-Type", "application/x-www-form-urlencoded")
            //       .PostMultipartAsync(mp => mp
            //                                .AddFile("file", fs, "Baki_file")
            //                                .AddJson("formdata", new { json })
            //       )
            //       .ReceiveJson<IFlurlResponse>();

            try
            {

                string postData = string.Empty;
                Encoding encoding = Encoding.UTF8;
                string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
                string contentType = "multipart/form-data; boundary=" + formDataBoundary;
                byte[] formData;

                foreach (var itm in mediaItem.FormData)
                {
                    postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                    formDataBoundary,
                    itm.Key,
                    itm.Value);

                   //  formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));

                }

                var resp = await _flurlClient.Request("media/add/file/")
                    .WithBasicAuth("baki.test@holo-one.com", "g6hN!(3#")
                    .PostMultipartAsync(mp => mp
                        // .AddJson("mode", "formdata")                // individual string
                        // .AddJson("formdata", jsonFormData)
                        // .AddJson("processing_params", jsonProcessingParams)
                        .AddJson("formdata", postData)
                        .AddStringParts("file_extension", "png")
                        .AddFile("file", filePath)                    // local file path
                                                                      // .AddStringParts(new { a = 1, b = 2 })         // multiple strings
                                                                      //.AddFile("file2", stream, "foo.txt")        // file stream
                                                                      // .AddJson("formdata", new { foo = "x" })         // json
                                                                      // .AddUrlEncoded("urlEnc", new { bar = "y" }) // URL-encoded                      
                                                                      // .Add(content)
                        );

                return resp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // }
        }

        // version 2
        public async Task<string> ExportModelFormCompositionAsync(UserLogin user, NameValueCollection values, NameValueCollection files, ProcessingParams processingParams)
        {
            string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                           .GetBytes(user.Username + ":" + user.Password));

            string url = RequestConstants.BaseUrl + "media/add/file/";

            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            // The first boundary
            byte[] boundaryBytes = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            // The last boundary
            byte[] trailer = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            // The first time it itereates, we need to make sure it doesn't put too many new paragraphs down or it completely messes up poor webbrick
            byte[] boundaryBytesF = System.Text.Encoding.ASCII.GetBytes("--" + boundary + "\r\n");

            // Create the request and set parameters
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Headers.Add("Authorization", "Basic " + encoded);

            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;

            // Get request stream
            Stream requestStream = request.GetRequestStream();

            foreach (string key in values.Keys)
            {
                // Write item to stream
                byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}", key, values[key]));
                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                requestStream.Write(formItemBytes, 0, formItemBytes.Length);
            }

            // add procesing parameters
            var processingArgs = processingParams.GetProperties();
            var items = processingArgs.SelectMany(x=> x.AllKeys.SelectMany(x.GetValues, (k, v) => new { key = k, value = v }));

            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.key) || string.IsNullOrEmpty(item.value))
                    break;

                // Write item to stream
                byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}", item.key, item.value));
                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                requestStream.Write(formItemBytes, 0, formItemBytes.Length);
            }

            if (files != null)
            {
                foreach (string key in files.Keys)
                {
                    if (File.Exists(key))
                    {
                        string fileName = System.IO.Path.GetFileName(key);
                        int bytesRead = 0;
                        byte[] buffer = new byte[2048];
                        byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(
                            string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n", "file", fileName));
                        requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                        requestStream.Write(formItemBytes, 0, formItemBytes.Length);

                        using (FileStream fileStream = new FileStream(key, FileMode.Open, FileAccess.Read))
                        {
                            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                // Write file content to stream, byte by byte
                                requestStream.Write(buffer, 0, bytesRead);
                            }

                            fileStream.Close();
                        }
                    }
                }
            }

            // Write trailer and close stream
            requestStream.Write(trailer, 0, trailer.Length);
            requestStream.Close();

            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                return await reader.ReadToEndAsync();
            };
        }
    }
}
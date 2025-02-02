﻿using Flurl.Http;
using Flurl.Http.Configuration;
using Hanssens.Net;
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        ILoginService _loginService;
        private readonly ILocalStorage _localeStorage;

        public ExportService(IFlurlClientFactory flurlClientFac, ILoginService loginService, ILocalStorage localeStorage)
        {
            _loginService = loginService;
            _localeStorage = localeStorage;

            _flurlClient = flurlClientFac.Get(RequestConstants.BaseUrl);
            // _flurlClient.Configure(settings => settings.BeforeCallAsync = EnsureTokenAsync);
            // FlurlHttp
        }

        public async Task<ExportService> EnsureTokenAsync(UserLogin userLogin) //FlurlCall arg)
        {
            // try refresh token
            await _loginService.RefreshLoginToken(userLogin);
            return this;
        }

        public async Task<IList<MediaFile>> GetCompanyMediaFolderContent(UserLogin user, int folderId = 0)
        {
            _flurlClient.BaseUrl = Utility.GetBaseUrl(user.LoginType.Type, user.LoginType.Region);

            IDictionary<string, string> queryParams = null;
            //queryParams = new Dictionary<string, string> { { "type", "folder" }, { "folder_pk", folderId.ToString() } };

            if (folderId != 0)
                queryParams = new Dictionary<string, string> { { "folder_pk", folderId.ToString() } };

            IFlurlResponse response;

            if (user.LoginType.Type == "LCP")
                response = await _flurlClient.Request("media/")
                                    .WithHeader("Bearer", user.Token)
                                    .WithHeader("Content-Type", "application/json")
                                    .SetQueryParams(queryParams)
                                    .GetAsync();
            else
                response = await _flurlClient.Request("media/")
                                //.WithBasicAuth(user.Username, user.Password)
                                .WithHeader("Authorization", "Token " + user.Token)
                                .WithHeader("Content-Type", "application/json")
                                .SetQueryParams(queryParams)
                                .GetAsync();

            return response.ResponseMessage.IsSuccessStatusCode
                    ? await response.GetJsonAsync<IList<MediaFile>>()
                    : null;
        }

        public async Task<IList<MediaFile>> GetCompany3DModels(UserLogin user)
        {
            _flurlClient.BaseUrl = Utility.GetBaseUrl(user.LoginType.Type, user.LoginType.Region);

            IFlurlResponse response;

            if (user.LoginType.Type == "LCP")
                response = await _flurlClient.Request("media/")
                                    .SetQueryParam("type", "generic_3d_model") //"one_to_one_overlay_model"
                                    .WithHeader("Bearer", user.Token)
                                    .WithHeader("Content-Type", "application/json")
                                    .GetAsync();
            else
                response = await _flurlClient.Request("media/")
                                    .SetQueryParam("type", "generic_3d_model")
                                    // .SetQueryParam("type", "one_to_one_overlay_model")
                                    .WithHeader("Authorization", "Token " + user.Token)
                                    .WithHeader("Content-Type", "application/json")
                                    .GetAsync();

            return response.ResponseMessage.IsSuccessStatusCode
                    ? await response.GetJsonAsync<IList<MediaFile>>()
                    : null;
        }

        public async Task<IEnumerable<BIM3DLayer>> Get3DModelById(UserLogin user, int mediaFileId)
        {
            _flurlClient.BaseUrl = Utility.GetBaseUrl(user.LoginType.Type, user.LoginType.Region);

            IFlurlResponse response;

            if (user.LoginType.Type == "LCP")
                response = await _flurlClient.Request($"media/3dmodel/{mediaFileId}/")
                                    .WithHeader("Bearer", user.Token)
                                    .WithHeader("Content-Type", "application/json")
                                    .GetAsync();
            else
                response = await _flurlClient.Request($"media/3dmodel/{mediaFileId}/")
                                    .WithHeader("Authorization", "Token " + user.Token)
                                    .WithHeader("Content-Type", "application/json")
                                    .GetAsync();

            return response.ResponseMessage.IsSuccessStatusCode
                    ? await response.GetJsonAsync<IEnumerable<BIM3DLayer>>()
                    : null;
        }

        /// <summary>
        /// Export by file path
        /// </summary>
        /// <param name="user"></param>
        /// <param name="values"></param>
        /// <param name="files"></param>
        /// <param name="processingParams"></param>
        /// <returns></returns>
        public async Task<string> ExportDefaultModelAndNewBIMAsync(UserLogin user, NameValueCollection values, NameValueCollection files, ProcessingParams processingParams, string urlPath, string exportType)
        {
            string encodedCredentials = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                           .GetBytes(user.Username + ":" + user.Password));

            string url = Utility.GetBaseUrl(user.LoginType.Type, user.LoginType.Region) + urlPath;

            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            // The first boundary
            byte[] boundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            // The last boundary
            byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            // The first time it itereates, we need to make sure it doesn't put too many new paragraphs down or it completely messes up poor webbrick
            byte[] boundaryBytesF = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");

            // Create the request and set parameters
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            //request.Headers.Add("Authorization", "Basic " + encodedCredentials);
            if (user.LoginType.Type == "LCP")
                request.Headers.Add("Bearer", user.Token);
            else
                request.Headers.Add("Authorization", "Token " + user.Token);

            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;

            // Get request stream
            Stream requestStream = request.GetRequestStream();

            foreach (string key in values.Keys)
            {
                // Write item to stream
                byte[] formItemBytes = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}", key, values[key]));
                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                requestStream.Write(formItemBytes, 0, formItemBytes.Length);
            }

            // Processing Parameters already included in the above statement as JSON object

            //// add procesing parameters
            //if (processingParams != null)
            //{
            //    var processingArgs = processingParams.GetProperties();
            //    var items = processingArgs.SelectMany(x => x.AllKeys.SelectMany(x.GetValues, (k, v) => new { key = k, value = v }));

            //    foreach (var item in items)
            //    {
            //        if (string.IsNullOrEmpty(item.key) || string.IsNullOrEmpty(item.value))
            //            break;

            //        // Write item to stream
            //        byte[] formItemBytes = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}", item.key, item.value));
            //        requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            //        requestStream.Write(formItemBytes, 0, formItemBytes.Length);
            //    }
            //}

            if (files != null)
            {
                foreach (string key in files.Keys)
                {
                    if (File.Exists(key))
                    {
                        string fileName = Path.GetFileName(key);
                        int bytesRead = 0;
                        byte[] buffer = new byte[2048];
                        byte[] formItemBytes = new byte[2048];

                        formItemBytes = Encoding.UTF8.GetBytes(
                            string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n", exportType, fileName));

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

        /// <summary>
        /// Export by file path
        /// </summary>
        /// <param name="user"></param>
        /// <param name="values"></param>
        /// <param name="files"></param>
        /// <param name="processingParams"></param>
        /// <returns></returns>
        public async Task<string> ExportExistingBIMAsync(UserLogin user, int mediaId, Dictionary<string, dynamic> payload, NameValueCollection files)
        {
            string encodedCredentials = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                                           .GetBytes(user.Username + ":" + user.Password));

            string url = Utility.GetBaseUrl(user.LoginType.Type, user.LoginType.Region) + $"media/bim/{mediaId}/update/";

            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            // The first boundary
            byte[] boundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            // The last boundary
            byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            // The first time it itereates, we need to make sure it doesn't put too many new paragraphs down or it completely messes up poor webbrick
            byte[] boundaryBytesF = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");

            // Create the request and set parameters
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;

            if (user.LoginType.Type == "LCP")
                request.Headers.Add("Bearer", user.Token);
            else
                request.Headers.Add("Authorization", "Token " + user.Token);

            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;

            // Get request stream
            Stream requestStream = request.GetRequestStream();

            foreach (string key in payload.Keys)
            {

                byte[] formItemBytes = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}", key, payload[key]));
                requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                requestStream.Write(formItemBytes, 0, formItemBytes.Length);
            }

            if (files != null)
            {
                foreach (string key in files.Keys)
                {
                    if (File.Exists(key))
                    {
                        int bytesRead = 0;
                        byte[] buffer = new byte[2048];
                        byte[] formItemBytes = new byte[2048];
                        formItemBytes = Encoding.UTF8.GetBytes(
                            string.Format("Content-Disposition: form-data; name=\"layers[]\"; filename=\"{0}\"\r\nContent-Type: application/octet-stream\r\n\r\n", files[key]));

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
using Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BaseService
    {
        public T ConvertStringToObject<T>(string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public string EnsureCorrectAPI(string api)
        {
            api = api.Replace("////api/", "/api/");
            api = api.Replace("///api/", "/api/");
            api = api.Replace("//api/", "/api/");
            api = api.Replace("omniacloud.netapi/", "omniacloud.net/api/");//omniacloud.net/api/
            return api;
        }

        public string ConvertObjectToString(object value)
        {
            try
            {
                return JsonConvert.SerializeObject(value);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            return "";
        }

        public T CreatePostRequest<T>(string url, object value)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string jsonStr = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                    streamWriter.Write(jsonStr);
                    streamWriter.Flush();
                    streamWriter.Close();

                    HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string resultStr = streamReader.ReadToEnd();
                        return JsonConvert.DeserializeObject<T>(resultStr);
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public T CreatePostRequestWithHeaders<T>(string url, object value, List<HeaderData> headers)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8"; 
                foreach (var header in headers)
                {
                    httpWebRequest.Headers.Add(header.Key, header.Value);
                }
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var s = value.GetType();
                    
                    string jsonStr = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                    if(s.FullName == "System.String")
                    {
                        jsonStr=value.ToString();
                    }
                    streamWriter.Write(jsonStr);
                    streamWriter.Flush();
                    streamWriter.Close();

                    HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string resultStr = streamReader.ReadToEnd();
                        return JsonConvert.DeserializeObject<T>(resultStr);
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public T CreateHttpPostRequestWithHeaders<T>(string url, string value, List<HeaderData> headers)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "*/*";
                foreach (var header in headers)
                {
                    httpWebRequest.Headers.Add(header.Key, header.Value);
                }
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string jsonStr = value;

                    streamWriter.Write(jsonStr);
                    streamWriter.Flush();
                    streamWriter.Close();

                    HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        string resultStr = streamReader.ReadToEnd();
                        return JsonConvert.DeserializeObject<T>(resultStr);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private readonly Encoding encoding = Encoding.UTF8;
        public HttpWebResponse MultipartFormPost(string postUrl, string userAgent, Dictionary<string, object> postParameters,List<HeaderData> headers)
        {
            string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

            return PostForm(postUrl, userAgent, contentType, formData, headers);
        }
        private HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData, List<HeaderData> headers)
        {
            //HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);

            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            // Set up the request properties.  
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            //request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // You could add authentication here as well if needed:  
            // request.PreAuthenticate = true;  
            // request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;  

            //Add header if needed  
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            } 

            // Send the form data to the request.  
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }

        private byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {

                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter) // to check if parameter if of file type   
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream  
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string.  
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline  
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]  
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }

        public T UploadFileWithHeaders<T>(string url, VersionData versionData, List<HeaderData> headers)
        {
            try
            {
                string requestURL = url; 
                byte[] bytes = versionData.Binary; // You need to do this download if your file is on any other server otherwise you can convert that file directly to bytes  
                Dictionary<string, object> postParameters = new Dictionary<string, object>();
                // Add your parameters here  
                //postParameters.Add("name", "File");
                //postParameters.Add("filename", Path.GetFileName(versionData.FileRef));
                postParameters.Add(Path.GetFileName(versionData.FileRef), new FileParameter(bytes));
                string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36";
                HttpWebResponse webResponse = MultipartFormPost(requestURL, userAgent, postParameters, headers);
                // Process response  
                //StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                using (var streamReader = new StreamReader(webResponse.GetResponseStream()))
                {
                    string resultStr = streamReader.ReadToEnd();
                    webResponse.Close();
                    return JsonConvert.DeserializeObject<T>(resultStr);
                }
            }
            catch(Exception ex) { throw ex; } 
        }  
        public T CreateGetRequest<T>(string url)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                HttpWebResponse tmpHttpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(tmpHttpResponse.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<T>(result);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public T CreateGetRequestWithHeader<T>(string url, List<HeaderData> headers)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                foreach (var header in headers)
                {
                    httpWebRequest.Headers.Add(header.Key, header.Value);
                }
                HttpWebResponse tmpHttpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(tmpHttpResponse.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<T>(result);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string CreatePostRequestWebPart(string url, object value)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string jsonStr = JsonConvert.SerializeObject(value);
                    streamWriter.Write(jsonStr);
                    streamWriter.Flush();
                    streamWriter.Close();

                    HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T CreateGetRequestOmniaPage<T>(string url)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";


                HttpWebResponse tmpHttpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(tmpHttpResponse.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();
                    //result = CommonUtils.ReplacePropertiesNameWithUpperCase(result);
                    return JsonConvert.DeserializeObject<T>(result);
                }
            }
            catch
            {
                throw;
            }
        }

        public string CreatePutRequestOmniaPage(string url, object value)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "PUT";
                httpWebRequest.Accept = "application/json; charset=utf-8";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    //string loginjson = new JavaScriptSerializer().Serialize(pageInfo);
                    string jsonStr = JsonConvert.SerializeObject(value);
                    //Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                    streamWriter.Write(jsonStr);
                    streamWriter.Flush();
                    streamWriter.Close();

                    HttpWebResponse tempHttpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(tempHttpResponse.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public string CreatePostRequestOmniaPage(string url, object value)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string jsonStr = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                    streamWriter.Write(jsonStr);
                    streamWriter.Flush();
                    streamWriter.Close();

                    HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public string CreatePostRequestComment(string url, object value)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Accept = "application/json; charset=utf-8";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string jsonStr = JsonConvert.SerializeObject(value);
                    streamWriter.Write(jsonStr);
                    streamWriter.Flush();
                    streamWriter.Close();

                    HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public string CreateDeleteRequestOmniaPage(string url)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "DELETE";
                httpWebRequest.Accept = "application/json; charset=utf-8";

                HttpWebResponse tmpHttpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(tmpHttpResponse.GetResponseStream()))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch
            {
                throw;
            }
        }

        public T CreatePutRequest<T>(string url, object value)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "PUT";
                httpWebRequest.Accept = "application/json; charset=utf-8";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string loginjson = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                    streamWriter.Write(loginjson);
                    streamWriter.Flush();
                    streamWriter.Close();

                    HttpWebResponse tempHttpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(tempHttpResponse.GetResponseStream()))
                    {
                        string result = streamReader.ReadToEnd();
                        return JsonConvert.DeserializeObject<T>(result);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public T CreateDeleteRequest<T>(string url)
        {
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "DELETE";
                httpWebRequest.Accept = "application/json; charset=utf-8";

                HttpWebResponse tmpHttpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(tmpHttpResponse.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();
                    return JsonConvert.DeserializeObject<T>(result);
                }
            }
            catch
            {
                throw;
            }
        }

        //public Root GetDocumentHistory(string docId, string webURL)
        //{
        //    try
        //    {
        //        string api = EnsureCorrectAPI(SiteInfo.OmniaODMAPI + "/api/documenthistory/getdocumenthistory?omniaapp=false&odmDocId={0}&webUrl={1}");
        //        api = string.Format(api, docId, webURL);
        //        Root ret = CreateGetRequestWithHeader<Root>(api, SiteInfo.NetworkHeaders);
        //        return ret;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }


    public class FileParameter
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public FileParameter(byte[] file) : this(file, null) { }
        public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
        public FileParameter(byte[] file, string filename, string contenttype)
        {
            File = file;
            FileName = filename;
            ContentType = contenttype;
        }
    }
}

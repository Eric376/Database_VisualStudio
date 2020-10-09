using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarkLogic.Controllers
{
    public class MarkLogicController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<string> Get()
        {
            using (var client = new HttpClient())
            {
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential("admin", "admin"),
                };
                var httpClient = new HttpClient(httpClientHandler);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = await httpClient.GetStringAsync("http://192.168.56.200:8000/LATEST/documents?uri=/films/tt0027977.json");
                return result;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upload( List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    Stream s = formFile.OpenReadStream();
                    HttpContent content = new StreamContent(s);
                    using (var client = new HttpClient())
                    {
                        var httpClientHandler = new HttpClientHandler()
                        {
                            Credentials = new NetworkCredential("admin", "admin"),
                        };
                        var httpClient = new HttpClient(httpClientHandler);
                        httpClient.DefaultRequestHeaders.Accept.Clear();

                        string fileType = formFile.ContentType;
                        string fileName = Path.GetFileName(formFile.FileName);

                        //Debug.WriteLine("~~~~~~~~~~~~~~~~LOOK~~~~~~~~~~~~");

                        if (fileType != "application/json" || fileType != "application/xml" ||
                            fileType != "text/plain" || fileType != "image/jpeg" || fileType != "image/png")
                        {
                            fileType = "application/octet-stream";
                        }

                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(fileType));
                        string uri = "http://192.168.56.200:8000/LATEST/documents?uri=/example/" + fileName;
                        await httpClient.PutAsync(uri, content);
                    }
                }
            }
            return Ok(new { count = files.Count, size});
            // NuGet Newtonsoft.Json library for object conversion
            // string json = JsonConvert.SerializeObject(account, Formatting.Indented);
            // Account account_back = JsonConvert.DeserializeObject<Account>(json);
        }


        public IActionResult Browse()
        {
            return View();
        }
        public async Task<string> Browsing()
        {

            using (var client = new HttpClient())
            {
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential("admin", "admin"),
                };
                var httpClient = new HttpClient(httpClientHandler);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                // httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/jpeg"));
                var result = await httpClient.GetStringAsync("http://192.168.56.200:8000/LATEST/search?pageLength=2000"); //search?pageLength=2000
                return result;
            }
        }

        public IActionResult Delete()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Deleting(string id)
        {
            using (var client = new HttpClient())
            {
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential("admin", "admin"),
                };
                var httpClient = new HttpClient(httpClientHandler);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                
                if (!String.IsNullOrEmpty(id))
                {
                    string uri = "http://192.168.56.200:8000/LATEST/documents?uri=" + id;
                    await httpClient.DeleteAsync(uri);
                }
            }
                
            return Redirect("/MarkLogic/Delete");
        }


        public IActionResult Download()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Downloading(string id)
        {
            using (var client = new HttpClient())
            {
                var httpClientHandler = new HttpClientHandler()
                {
                    Credentials = new NetworkCredential("admin", "admin"),
                };
                var httpClient = new HttpClient(httpClientHandler);
                httpClient.DefaultRequestHeaders.Accept.Clear();

                if (!String.IsNullOrEmpty(id))
                {
                    string uri = "http://192.168.56.200:8000/LATEST/documents?uri=" + id;
                    await httpClient.GetStringAsync(uri);
                }
            }

            return Redirect("/MarkLogic/Download");
        }
    }
}
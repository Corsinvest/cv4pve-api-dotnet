/*
 * SPDX-FileCopyrightText: Copyright Corsinvest Srl
 * SPDX-License-Identifier: GPL-3.0-only
 */

using Corsinvest.ProxmoxVE.Api;
using Corsinvest.ProxmoxVE.Api.Console.Helpers;
using Microsoft.Extensions.Logging;

System.Console.WriteLine("pippo");


var app = ConsoleHelper.CreateApp("Test", "Automatic snapshot VM/CT with retention");
var loggerFactory = ConsoleHelper.CreateLoggerFactory<Program>(app.GetLogLevelFromDebug());
 await app.ExecuteAppAsync(args, loggerFactory.CreateLogger(typeof(Program)));


 
// _ = new Commands(app, loggerFactory);
// return await app.ExecuteAppAsync(args, loggerFactory.CreateLogger(typeof(Program)));

// var client = new PveClient("192.168.0.2");
// if (await client.LoginAsync("root", Environment.GetEnvironmentVariable("PvePassword")))
// {
//     Console.WriteLine("pippo");

//     var aa  = await client.Nodes["cc01"].Qemu[1006].Agent.Exec.Exec(["powershell", "-command", "echo", "test"]);

//     Console.WriteLine(aa.ReasonPhrase);

//     //var mpfdContent = new MultipartFormDataContent()
//     // {
//     //     { new StringContent("iso"),"content"},
//     //     { new ByteArrayContent(File.ReadAllBytes(fileName)), "filename",  Path.GetFileName(fileName)}
//     // };

//     // var httpClient = client.GetHttpClient();
//     // httpClient.DefaultRequestHeaders.Add("User-Agent", "pveClient/8.0.1");

//     //httpClient.DefaultRequestHeaders.ExpectContinue = true;
//     //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
//     // var message = await httpClient.PostAsync($"{client.GetApiUrl()}/nodes/pve8/storage/local/upload", mpfdContent);
// }

// // var client = new PveClient("10.92.90.101");
// // if (await client.Login("root", Environment.GetEnvironmentVariable("pve_password")))
// // {

// //     var isoContent = new ByteArrayContent(Encoding.ASCII.GetBytes("content=iso"));
// //     isoContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
// //     var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(fileName));
// //     //fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
// //     fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

// //     var mpfdContent = new MultipartFormDataContent()
// //     {
// //         { isoContent},
// //         { fileContent, "filename"} //, Path.GetFileName(fileName)
// //     };

// //     //536871237
// //     //536871269

// //     var handler = new HttpClientHandler()
// //     {
// //         CookieContainer = new CookieContainer(),
// //         ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
// //     };

// //     var httpClient = new HttpClient(handler);
// //     if (client.CSRFPreventionToken != null)
// //     {
// //         handler.CookieContainer.Add(new Cookie("PVEAuthCookie", HttpUtility.UrlEncode(client.PVEAuthCookie), "/", client.Host)
// //         {
// //             Secure = true
// //         });
// //     }
// //     httpClient.DefaultRequestHeaders.Add("User-Agent", "pveClient/8.0.1");
// //     //httpClient.DefaultRequestHeaders.ExpectContinue = true;
// //     httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));

// //     var message = await httpClient.PostAsync($"{client.GetApiUrl()}/nodes/cv-pve01/storage/local/upload", mpfdContent);
// //     message.EnsureSuccessStatusCode();

// //     var result = await client.UploadFileToStorage("cv-pve01",
// //                                                     "local",
// //                                                     "iso",
// //                                                     "gparted-live-1.5.0-6-amd64.iso",
// //                                                     @"C:\Users\Daniele\Downloads\gparted-live-1.5.0-6-amd64.iso");

// //     Console.Out.WriteLine(result.Response);
// // }
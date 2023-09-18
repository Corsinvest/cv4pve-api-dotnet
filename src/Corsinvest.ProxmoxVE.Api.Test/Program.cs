using Corsinvest.ProxmoxVE.Api;

var fileName = @"C:\Users\Daniele\Downloads\gparted-live-1.5.0-6-amd64.iso";

var client = new PveClient("10.92.90.70");
if (await client.Login("root", Environment.GetEnvironmentVariable("pve_password")))
{
    var mpfdContent = new MultipartFormDataContent()
    {
        { new StringContent("iso"),"content"},
        { new ByteArrayContent(File.ReadAllBytes(fileName)), "filename",  Path.GetFileName(fileName)}
    };

    var httpClient = client.GetHttpClient();
    httpClient.DefaultRequestHeaders.Add("User-Agent", "pveClient/8.0.1");

    //httpClient.DefaultRequestHeaders.ExpectContinue = true;
    //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));
    var message = await httpClient.PostAsync($"{client.GetApiUrl()}/nodes/pve8/storage/local/upload", mpfdContent);
}

// var client = new PveClient("10.92.90.101");
// if (await client.Login("root", Environment.GetEnvironmentVariable("pve_password")))
// {

//     var isoContent = new ByteArrayContent(Encoding.ASCII.GetBytes("content=iso"));
//     isoContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
//     var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(fileName));
//     //fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
//     fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

//     var mpfdContent = new MultipartFormDataContent()
//     {
//         { isoContent},
//         { fileContent, "filename"} //, Path.GetFileName(fileName)
//     };

//     //536871237
//     //536871269

//     var handler = new HttpClientHandler()
//     {
//         CookieContainer = new CookieContainer(),
//         ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
//     };

//     var httpClient = new HttpClient(handler);
//     if (client.CSRFPreventionToken != null)
//     {
//         handler.CookieContainer.Add(new Cookie("PVEAuthCookie", HttpUtility.UrlEncode(client.PVEAuthCookie), "/", client.Host)
//         {
//             Secure = true
//         });
//     }
//     httpClient.DefaultRequestHeaders.Add("User-Agent", "pveClient/8.0.1");
//     //httpClient.DefaultRequestHeaders.ExpectContinue = true;
//     httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));


//     var message = await httpClient.PostAsync($"{client.GetApiUrl()}/nodes/cv-pve01/storage/local/upload", mpfdContent);
//     message.EnsureSuccessStatusCode();


//     var result = await client.UploadFileToStorage("cv-pve01",
//                                                     "local",
//                                                     "iso",
//                                                     "gparted-live-1.5.0-6-amd64.iso",
//                                                     @"C:\Users\Daniele\Downloads\gparted-live-1.5.0-6-amd64.iso");

//     Console.Out.WriteLine(result.Response);
// }
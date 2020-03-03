using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Services;

namespace SignatureValue
{
    /// <summary>
    /// Сводное описание для POSTServiceDeskAPI
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class POSTServiceDeskAPI : System.Web.Services.WebService
    {

        [WebMethod]
        public string postServiceDeskAPI()
        {
            string textResult = "";
            Stream receiveStream = HttpContext.Current.Request.InputStream;
            receiveStream.Position = 0;
            using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
            {
                // Load into XML document
                textResult = readStream.ReadToEnd();
            }
            RestClient restClient = new RestClient("http://servicedesk.gradient.ru/api");

            restClient.Proxy = new System.Net.WebProxy("proxy-dc.gradient.ru", 3128);
            restClient.Proxy.Credentials = new System.Net.NetworkCredential("Webservice", "web123");
            restClient.Authenticator = new HttpBasicAuthenticator(@"Webservice", "web123");
            restClient.AddDefaultHeader("Content-Type", "application/xml");
            restClient.AddDefaultHeader("messageType", "application/xml");
            var request = new RestRequest(textResult, Method.POST);
            request.AddHeader("Content-Type", "application/xml");
            request.RequestFormat = DataFormat.Xml;
            request.AddHeader("Accept", "application/xml");


            return restClient.Execute(request).Content;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Services;

namespace SignatureValue
{
    /// <summary>
    /// Сводное описание для getNFCReport
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class getNFCReport : System.Web.Services.WebService
    {

        [System.Web.Services.WebMethod(Description = "Метод получает отчет, сохранятет его в файл, и возвращает путь к файлу")]
        public void GetNFCReport(string userId, string password, string _idReport)
        {
            this.Context.Response.ContentType = "text/plain charset=utf-8";
            Browser browser = new Browser();
            string doc = browser.POSTLogin("https://e.factoring.ru/api/ext/session", userId, password);
            string auth = doc.Replace("\"", "");
            string fileName = string.Format(@"\\zskpk02\ExchangeDocs\NFC\reports\{0}_{1}.xls", _idReport, DateTime.Now.ToString("yyyy-MM-ddTHH_mm_ss"));
            string test = browser.GetFileNFC(string.Format("https://e.factoring.ru/api/ext/reports/settings/{0}/result?details=false", _idReport), auth, fileName);
            doc = browser.POSTNFC("https://e.factoring.ru/api/ext/session/current", "", auth);
            if (test == "")
            {

                Context.Response.Write(fileName);
            }
            else
            {
                Context.Response.Clear();
                Context.Response.ContentType = "text/html; charset=utf8";
                byte[] data = Encoding.UTF8.GetBytes(test);
                Context.Response.AddHeader("content-length", data.Length.ToString());
                Context.Response.Flush();

                Context.Response.Write(test);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Security.Cryptography.Pkcs;

namespace SignatureValue
{
    /// <summary>
    /// Summary description for GetSignatureValue
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    //[System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    public class GetSignatureValue : System.Web.Services.WebService
    {
        /// <summary>
        /// Метод возвращает SignatureValue после подписания xml
        /// </summary>
        /// <param name="_xmlText">Текст xml который нужно подписать</param>
        /// <param name="_thumbprint">Код отпечатка</param>
        /// <returns>SignatureValue закодированная в base64</returns>
        [System.Web.Services.WebMethod(Description = "Метод возвращает SignatureValue после подписания xml")]
        public string getSignatureValue(string _xmlText, string _thumbprint)
        {
            X509Certificate2 myCert = null;
            _xmlText = _xmlText.Replace(" ", "+");
            byte[] data = System.Convert.FromBase64String(_xmlText);
            string base64Decoded = System.Text.UTF8Encoding.UTF8.GetString(data);

            //string base64Decoded = System.Text.Encoding.UTF8.GetString(data);
            byte[] sign = Sign(_thumbprint, System.Text.UTF8Encoding.UTF8.GetBytes(base64Decoded), true);

            string ret = Convert.ToBase64String(sign);

            return ret;
        }

        public byte[] Sign(string certificateThumbprint, byte[] data, bool detached)
        {
            var store = new X509Store(StoreLocation.CurrentUser);

            store.Open(OpenFlags.ReadOnly);

            var certificates = store.Certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, false);
            if (certificates.Count == 0) throw new Exception("Сертификат с отпечатком " + certificateThumbprint + " не найден в хранилище " + store.Location + ". Пользователь " + Environment.UserName);

            var contentInfo = new ContentInfo(data);
            var signedCms = new SignedCms(contentInfo, detached);
            var cmsSigner = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, certificates[0]);

            cmsSigner.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.Now));

            signedCms.ComputeSignature(cmsSigner, true);

            return signedCms.Encode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Services;

namespace SignatureValue
{
    /// <summary>
    /// Сводное описание для GetSignatureValue12511
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    //[System.Web.Script.Services.ScriptService]
    // [System.ComponentModel.ToolboxItem(false)]

    public class GetSignatureValue12511 : System.Web.Services.WebService
    {

        [System.Web.Services.WebMethod(Description = "Метод возвращает SignatureValue кодировка 1251 подпись неосоединенная")]
        public void getSignatureValue1251(string _text, string _thumbprint)
        {
            _text = _text.Replace(" ", "+");
            byte[] data = System.Convert.FromBase64String(_text);
            string base64Decoded = System.Text.Encoding.Default.GetString(data);

            //string base64Decoded = System.Text.Encoding.UTF8.GetString(data);
            byte[] sign = Sign(_thumbprint, System.Text.Encoding.Default.GetBytes(base64Decoded), false);

            string ret = Convert.ToBase64String(sign);

            Context.Response.Write(ret);
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

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Services;
using CAdESCOM;

namespace SignatureValue
{
    /// <summary>
    /// Сводное описание для SignBES
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class SignBES : System.Web.Services.WebService
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        [System.Web.Services.WebMethod(Description = "Подписание сообщения ЭЦП")]
        public string signBES(string _text, string _thumbprint, bool attached)
        {
            var sSignedMessage = "";
            string base64String = Base64Encode(_text);
            try
            {
               
                CPSigner oSigner = new CPSigner();
                oSigner.Certificate = GetCertificateByThumbprint(_thumbprint);
                oSigner.TSAAddress = "http://qs.cryptopro.ru/tsp/tsp.srf";

                CadesSignedDataClass test = new CadesSignedDataClass();
                var oSignedData = new CadesSignedData();
                //{
                oSignedData.ContentEncoding = CADESCOM_CONTENT_ENCODING_TYPE.CADESCOM_STRING_TO_UCS2LE;
                oSignedData.Content = System.Text.UTF8Encoding.UTF8.GetBytes(_text);
                //};

                try
                {
                    var sign = oSignedData.SignCades(oSigner, CADESCOM_CADES_TYPE.CADESCOM_CADES_BES, attached, CAPICOM.CAPICOM_ENCODING_TYPE.CAPICOM_ENCODE_BINARY);
                    sSignedMessage = System.Convert.ToBase64String(sign);


                }
                catch (Exception e)
                {
                    sSignedMessage = e.Message + " " + e.StackTrace + " " + base64String;
                }
            }
            catch (Exception e)
            {
                sSignedMessage = e.Message + " " + e.StackTrace + " " + base64String;
            }

            return sSignedMessage;
        }

       


        CAPICOM.ICertificate2 GetCertificateByThumbprint(string certThumbprint)
        {
            CAPICOM.ICertificate2 ret = null;
            string test = "";
            try
            {

                CAPICOM.Store oStore = new CAPICOM.Store();


                oStore.Open(CAPICOM.CAPICOM_STORE_LOCATION.CAPICOM_CURRENT_USER_STORE, OpenMode: CAPICOM.CAPICOM_STORE_OPEN_MODE.CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);

                foreach (CAPICOM.ICertificate2 certif in oStore.Certificates)
                {
                    test += certif.Thumbprint + "  ";
                    if (certif.Thumbprint == certThumbprint.ToUpper())
                    {
                        ret = certif;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                new Exception(e.Message + " " + e.StackTrace);
            }
            if (ret == null)
            {
                throw new Exception("Пустой сертификат " + test);
            }
            return ret;
        }
    }
}

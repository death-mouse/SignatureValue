using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SignatureValue
{
    public class Browser
    {
        public Browser()
        {
            Cookies = new CookieContainer();
            Randomize();
        }


        private CookieContainer Cookies;

        public CookieContainer geiveCookie()
        {
            return Cookies;
        }
        public string GetFileNFC(string url, string auth, string _fileName)
        {
            HttpWebRequest hwrq = CreateRequest(url);
            /*if (Environment.UserDomainName == "GRADIENT")
                hwrq.Credentials = CredentialCache.DefaultNetworkCredentials;
            else*/
            {
                Decryptor decryptor = new Decryptor("ckuJ1YQrX7ysO/WVHkowGA==");
                hwrq.Credentials = new NetworkCredential("d.astahov", decryptor.DescryptStr, "GRADIENT");
            }
            WebProxy proxy = new System.Net.WebProxy("proxy-dc.gradient.ru", 3128);
            proxy.Credentials = hwrq.Credentials;
            hwrq.Proxy = proxy;
            WebHeaderCollection webHeaderCollection = new System.Net.WebHeaderCollection();
            webHeaderCollection.Add(System.Net.HttpRequestHeader.Authorization, string.Format("token {0}", auth));
            hwrq.Headers.Add(webHeaderCollection);

            hwrq.ContentType = "application/octet-stream";
            HttpWebResponse hwrs;
            try
            {
                hwrs = (HttpWebResponse)hwrq.GetResponse();
                
                    Stream stream = hwrs.GetResponseStream();
                    Byte[] buffer = new System.Byte[16 * 1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    FileStream fileStream = System.IO.File.Create(_fileName);
                    while (bytesRead > 0)
                    {
                        int int32 = bytesRead;
                        fileStream.Write(buffer, 0, int32);
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                    }
                    stream.Close();
                    stream.Dispose();
                    fileStream.Close();
                    fileStream.Dispose();
                
            }
            catch(WebException e)
            {
                HttpWebResponse hwrsE = (HttpWebResponse)e.Response;
                using (StreamReader sr = new StreamReader(hwrsE.GetResponseStream(), Encoding.UTF8))
                {
                    
                        return sr.ReadToEnd().Trim();
                }
            }

            return "";
        }
        public string GETFile(string url, Encoding _encoding, string ResultFileName, Boolean needHeaders = false)
        {
            try
            {
                HttpWebRequest hwrq = CreateRequest(url);
                if (Environment.UserDomainName == "GRADIENT")
                    hwrq.Credentials = CredentialCache.DefaultNetworkCredentials;
                else
                {
                    Decryptor decryptor = new Decryptor("ckuJ1YQrX7ysO/WVHkowGA==");
                    hwrq.Credentials = new NetworkCredential("d.astahov", decryptor.DescryptStr, "GRADIENT");
                }
                hwrq.CookieContainer = Cookies;
                hwrq.AllowAutoRedirect = false;
                using (HttpWebResponse hwrs = (HttpWebResponse)hwrq.GetResponse())
                {
                    Cookies.Add(hwrs.Cookies);
                    string[] value = hwrs.Headers.GetValues(3);
                    foreach (string line in value)
                    {

                        string[] split = line.Split(new char[] { '\'' });
                        string fileName = split[split.Length - 1];
                        fileName = System.Web.HttpUtility.UrlDecode(fileName);
                        ResultFileName = string.Format(@"{0}\{1}", @"temp", fileName);
                    }
                    Stream str = hwrs.GetResponseStream();
                    byte[] inBuf = new byte[100000];
                    int bytesReadTotal = 0;
                    FileStream fstr = new FileStream(ResultFileName, FileMode.Create, FileAccess.Write);
                    while (true)
                    {
                        int n = str.Read(inBuf, 0, 100000);
                        if ((n == 0) || (n == -1))
                        {
                            break;
                        }
                        fstr.Write(inBuf, 0, n);
                        bytesReadTotal += n;
                    }
                    str.Close();
                    fstr.Close();
                    return ResultFileName;
                }
            }
            catch (Exception e)
            {
                return "";
            }
        }
        public string GET(string url, Encoding _encoding, Boolean needHeaders = false)
        {
            try
            {
                HttpWebRequest hwrq = CreateRequest(url);
                /*if (Environment.UserDomainName == "GRADIENT")
                    hwrq.Credentials = CredentialCache.DefaultNetworkCredentials;
                else*/
                {
                    Decryptor decryptor = new Decryptor("ckuJ1YQrX7ysO/WVHkowGA==");
                    hwrq.Credentials = new NetworkCredential("d.astahov", decryptor.DescryptStr, "GRADIENT");
                }
                WebProxy proxy = new System.Net.WebProxy("proxy-dc.gradient.ru", 3128);
                proxy.Credentials = hwrq.Credentials;
                hwrq.Proxy = proxy;
                hwrq.CookieContainer = Cookies;
                hwrq.AllowAutoRedirect = false;
                using (HttpWebResponse hwrs = (HttpWebResponse)hwrq.GetResponse())
                {
                    Cookies.Add(hwrs.Cookies);
                    using (StreamReader sr = new StreamReader(hwrs.GetResponseStream(), _encoding))
                    {
                        if (needHeaders)
                            return hwrs.Headers.ToString() + sr.ReadToEnd().Trim();
                        else
                            return sr.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string POST(string url, string query)
        {
            HttpWebRequest hwrq = CreateRequest(url);
            hwrq.CookieContainer = Cookies;
            hwrq.Method = "POST";
            Decryptor decryptor = new Decryptor("ckuJ1YQrX7ysO/WVHkowGA==");
            hwrq.Credentials = new NetworkCredential("d.astahov", decryptor.DescryptStr, "GRADIENT");
            hwrq.ContentType = "application/x-www-form-urlencoded";
            hwrq.AutomaticDecompression = DecompressionMethods.GZip;
            WebProxy proxy = new System.Net.WebProxy("proxy-dc.gradient.ru", 3128);
            proxy.Credentials = hwrq.Credentials;
            hwrq.Proxy = proxy;


            byte[] data = Encoding.UTF8.GetBytes(query);
            hwrq.ContentLength = data.Length;
            hwrq.GetRequestStream().Write(data, 0, data.Length);
            using (HttpWebResponse hwrs = (HttpWebResponse)hwrq.GetResponse())
            {
                Cookies.Add(hwrs.Cookies);
                using (StreamReader sr = new StreamReader(hwrs.GetResponseStream(), Encoding.Default))
                {
                    return sr.ReadToEnd().Trim();
                }
            }
        }
        public string POSTLogin(string url, string userName, string password)
        {
            HttpWebRequest hwrq = CreateRequest(url);
            hwrq.CookieContainer = Cookies;
            hwrq.Method = "POST";
            Decryptor decryptor = new Decryptor("ckuJ1YQrX7ysO/WVHkowGA==");
            hwrq.Credentials = new NetworkCredential("d.astahov", decryptor.DescryptStr, "GRADIENT");
            hwrq.ContentType = "application/json";
            
            WebProxy proxy = new System.Net.WebProxy("proxy-dc.gradient.ru", 3128);
            proxy.Credentials = hwrq.Credentials;
            hwrq.Proxy = proxy;
            string query = "{";
            query += string.Format(" \"userName\":\"{0}\", \"password\":\"{1}\" ", userName, password);
            query +="}";

            byte[] data = Encoding.UTF8.GetBytes(query);
            hwrq.ContentLength = data.Length;
            hwrq.GetRequestStream().Write(data, 0, data.Length);
            using (HttpWebResponse hwrs = (HttpWebResponse)hwrq.GetResponse())
            {
                Cookies.Add(hwrs.Cookies);
                using (StreamReader sr = new StreamReader(hwrs.GetResponseStream(), Encoding.Default))
                {
                    return sr.ReadToEnd().Trim();
                }
            }
        }
        public string POSTNFC(string url, string query, string auth)
        {
            HttpWebRequest hwrq = CreateRequest(url);
            hwrq.CookieContainer = Cookies;
            hwrq.Method = "DELETE";
            hwrq.ContentType = "application/x-www-form-urlencoded";
            WebHeaderCollection webHeaderCollection = new System.Net.WebHeaderCollection();
            Decryptor decryptor = new Decryptor("ckuJ1YQrX7ysO/WVHkowGA==");
            hwrq.Credentials = new NetworkCredential("d.astahov", decryptor.DescryptStr, "GRADIENT");
            WebProxy proxy = new System.Net.WebProxy("proxy-dc.gradient.ru", 3128);
            proxy.Credentials = hwrq.Credentials;
            hwrq.Proxy = proxy;
            webHeaderCollection.Add(System.Net.HttpRequestHeader.Authorization, string.Format("token {0}", auth));
            hwrq.Headers.Add(webHeaderCollection);
            byte[] data = Encoding.UTF8.GetBytes(query);
            hwrq.ContentLength = data.Length;
            hwrq.GetRequestStream().Write(data, 0, data.Length);
            using (HttpWebResponse hwrs = (HttpWebResponse)hwrq.GetResponse())
            {
                Cookies.Add(hwrs.Cookies);
                using (StreamReader sr = new StreamReader(hwrs.GetResponseStream(), Encoding.Default))
                {
                    return hwrs.Headers.ToString() + sr.ReadToEnd().Trim();
                }
            }

        }


        public Cookie GetCookie(string url, string name)
        {
            foreach (Cookie c in Cookies.GetCookies(new Uri(url)))
            {
                if (c.Name == name)
                    return c;
            }
            return null;
        }


        string UserAgent;
        string Accept;
        string AcceptLang;
        DecompressionMethods DMethod;


        private void Randomize()
        {
            string[] useragents = {
                                  /*"Mozilla/5.0 (Windows; U; Windows NT 5.1; ru; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13",
                                  "Mozilla/5.0 (Macintosh; U; PPC Max OS X Mach-O; en-US; rv:1.8.0.7) Gecko/200609211 Camino/1.0.3"*/
                                  " Opera/9.80 (X11; Linux x86_64; U; ru) Presto/2.2.15 Version/10.10"
                              };
            string[] acceptlang = {
                               "en-us;q=0.5,en;q=0.3",
                               "ru-ru,ru; q=0.3",
                               "q=0.8,en-us; q=0.3",
                               "q=0.5,en"
                           };
            string[] accepts = {
                              "application/json"
                          };
            DecompressionMethods[] dmethods = {
                                     DecompressionMethods.Deflate,
                                     DecompressionMethods.GZip,
                                     DecompressionMethods.None,
                                     (DecompressionMethods.Deflate | DecompressionMethods.GZip),
                                     (DecompressionMethods.Deflate | DecompressionMethods.None),
                                     (DecompressionMethods.GZip | DecompressionMethods.None)
                                 };
            AcceptLang = acceptlang[new Random().Next(acceptlang.Length)];
            UserAgent = useragents[new Random().Next(useragents.Length)];
            Accept = accepts[new Random().Next(accepts.Length)];
            DMethod = dmethods[new Random().Next(dmethods.Length)];
        }


        private HttpWebRequest CreateRequest(string url)
        {
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);
            Request.UserAgent = UserAgent;
            Request.Accept = Accept;
            Request.Headers.Add("Accept-Language", AcceptLang);
            Request.AutomaticDecompression = DMethod;
            return Request;
        }
    }
    class Decryptor
    {
        string descryPass;
        string descryString;

        public Decryptor()
        {
            descryPass = descryString;
        }
        public Decryptor(string encrStr)
        {
            descryPass = CreSap();
            descryString = Decrypt(encrStr);
        }
        public string DescryptStr
        {
            get { return descryString; }
            set { descryString = Decrypt(value); }

        }

        double SimAlgEncry(int n)
        {
            if (n == 0)
            {
                return 0;
            }
            else
            {
                if (n == 1)
                {
                    return 1;
                }
                else
                {
                    return SimAlgEncry(n - 1) + SimAlgEncry(n - 2);
                }
            }
        }
        string CreSap()
        {
            return Math.Pow(SimAlgEncry(10), 3).ToString();
        }
        /// <summary>
        /// Decrypt string
        /// </summary>
        /// <param name="crypt_str"></param>
        /// <returns></returns>

        string Decrypt(string crypt_str)
        {
            // Получаем массив байт
            byte[] crypt_data = Convert.FromBase64String(crypt_str);

            // Алгоритм 
            SymmetricAlgorithm sa_out = Rijndael.Create();
            // Объект для преобразования данных
            ICryptoTransform ct_out = sa_out.CreateDecryptor(
                (new PasswordDeriveBytes(descryPass, null)).GetBytes(16),
                new byte[16]);
            // Поток
            MemoryStream ms_out = new MemoryStream(crypt_data);
            // Расшифровываем поток
            CryptoStream cs_out = new CryptoStream(ms_out, ct_out, CryptoStreamMode.Read);
            // Создаем строку
            StreamReader sr_out = new StreamReader(cs_out);
            string source_out = sr_out.ReadToEnd();
            return source_out;
        }
    }
    class Encryptor
    {
        string encryPass;
        string encryString;



        public Encryptor()
        {
            encryPass = CreSap();
        }

        public Encryptor(string encrStr)
        {
            encryPass = CreSap();
            encryString = Encrypt(encrStr);
        }
        public string EncryptStr
        {
            get { return encryString; }
            set { encryString = Encrypt(value); }

        }

        double SimAlgEncry(int n)
        {
            if (n == 0)
            {
                return 0;
            }
            else
            {
                if (n == 1)
                {
                    return 1;
                }
                else
                {
                    return SimAlgEncry(n - 1) + SimAlgEncry(n - 2);
                }
            }
        }
        string CreSap()
        {
            return Math.Pow(SimAlgEncry(10), 3).ToString();
        }
        /// <summary>
        /// Encrypt string
        /// </summary>
        /// <param name="source_str"></param>
        /// <returns></returns>
        string Encrypt(string source_str)
        {
            // Получаем из строки набор байт, которые будем шифровать
            byte[] source_data = Encoding.UTF8.GetBytes(source_str);
            // Алгоритм 
            SymmetricAlgorithm sa_in = Rijndael.Create();
            // Объект для преобразования данных
            ICryptoTransform ct_in = sa_in.CreateEncryptor(
                (new PasswordDeriveBytes(encryPass, null)).GetBytes(16), new byte[16]);
            // Поток
            MemoryStream ms_in = new MemoryStream();
            // Шифровальщик потока
            CryptoStream cs_in = new CryptoStream(ms_in, ct_in, CryptoStreamMode.Write);
            // Записываем шифрованные данные в поток
            cs_in.Write(source_data, 0, source_data.Length);
            cs_in.FlushFinalBlock();
            // Создаем строку
            return Convert.ToBase64String(ms_in.ToArray());
            // Выводим зашифрованную строку

        }
        /// <summary>
        /// Decrypt string
        /// </summary>
        /// <param name="crypt_str"></param>
        /// <returns></returns>
        string Decrypt(string crypt_str)
        {
            // Получаем массив байт
            byte[] crypt_data = Convert.FromBase64String(crypt_str);

            // Алгоритм 
            SymmetricAlgorithm sa_out = Rijndael.Create();
            // Объект для преобразования данных
            ICryptoTransform ct_out = sa_out.CreateDecryptor(
                (new PasswordDeriveBytes(encryPass, null)).GetBytes(16),
                new byte[16]);
            // Поток
            MemoryStream ms_out = new MemoryStream(crypt_data);
            // Расшифровываем поток
            CryptoStream cs_out = new CryptoStream(ms_out, ct_out, CryptoStreamMode.Read);
            // Создаем строку
            StreamReader sr_out = new StreamReader(cs_out);
            string source_out = sr_out.ReadToEnd();
            return source_out;
        }
    }
}
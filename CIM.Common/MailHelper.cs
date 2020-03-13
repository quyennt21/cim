using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace CIM.Common
{
    public static class MailHelper
    {
        static string key = "daoquyenlinhhinhcong";
        public static void SendMail(string fromEmailAddress, string fromEmailPassword, string host, string port,
            bool enabledSSL, string toEmailAddress, string subject, string fromEmailDisplayName, string content)
        {
            try
            {
                string password=MailHelper.Decrypt(fromEmailPassword);
                MailMessage message = new MailMessage(new MailAddress(fromEmailAddress, fromEmailDisplayName),
                    new MailAddress(toEmailAddress));
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = content;
                var client = new SmtpClient();
                client.Credentials = new NetworkCredential(fromEmailAddress, password);
                client.Host = host;
                client.EnableSsl = enabledSSL;
                client.Port = !string.IsNullOrEmpty(port) ? Convert.ToInt32(port) : 0;
                client.Send(message);
            }
            catch (Exception ex)
            {
            }
        }

        public static String createContent(string path, string emailForder, string emailTemplate, object[] param)
        {
            String pathFileTemplate = String.Format("{0}{1}{2}.html", path, emailForder, emailTemplate);
            String template = File.ReadAllText(pathFileTemplate);

            for (int i = 0; i < param.Length; i++)
            {
                String position = "{" + i + "}";
                template = template.Replace(position, param[i] == null ? "" : param[i].ToString());
            }
            return template;
        }
        
        public static string Encrypt(string toEncrypt)
        {
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string toDecrypt)
        {
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
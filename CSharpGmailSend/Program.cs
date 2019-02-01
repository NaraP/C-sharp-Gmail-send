using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CSharpGmailSend
{
    class Program
    {
        static void Main(string[] args)
        {
             CSharpGmailsSend();
        }

        private static void CSharpGmailsSend()
        {
            string FullBody = string.Empty;

            string attachments = @"D:\\GmailSendCSharp\\GMailSend\\Narasimhulu Résume.doc";

           // String path = @"E:\\Narasimha_Data\\GMailSend\\EmailIds.txt";

            //string GetEmailIds = File.ReadAllText(path);

            List<string> EmailIdsList = new List<string>();

            EmailIdsList= GetGmailIds();

            //string[] ArrayEmailIds = GetEmailIds.Split(','); ;

            using (StreamReader sr = new StreamReader(@"D:\\GmailSendCSharp\\GMailSend\\Body.txt"))
            {
                FullBody = sr.ReadToEnd();
            }

            string Subject = "H1B Opportunity on Microsoft.net";

            foreach (var emailid in EmailIdsList)
            {
                SendEMail(emailid, "pnara527@gmail.com", Subject, FullBody.ToString(), attachments);
            }
        }

        public static List<string> GetGmailIds()
        {
            List<string> lstEmailids = new List<string>();

            var connection = new NpgsqlConnection("Host=localhost;Username=postgres;Port=5432;Password=admin;Database=TestDB");
            connection.Open();

            // connection.ConnectionString = connection.ConnectionString;
            //connection.Open();
            NpgsqlCommand cmd = new NpgsqlCommand("Select * from public.h1bemailids", connection);
            cmd.Connection = connection;
            cmd.CommandText = "Select * from public.h1bemailids";
            cmd.CommandType = CommandType.Text;
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            NpgsqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                string EmailIds = dr["EmailIDs"].ToString();
                lstEmailids.Add(EmailIds);
            }
            //DataTable dt = new DataTable();
            // da.Fill(dt);
            cmd.Dispose();
            connection.Close();
            return lstEmailids;
        }

        public static bool SendEMail(string to, string from, string subject, string body, string attachments)
        {
            try
            {
                MailMessage mail = new MailMessage();

                //set the addresses
                mail.From = new MailAddress("pnara527@gmail.com");
                mail.To.Add(to);

                //set the content
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                if (attachments.Length > 0)
                {
                    if (System.IO.File.Exists(attachments.Trim()))
                        mail.Attachments.Add(new Attachment(attachments.Trim()));
                }

                //Authenticate we set the username and password properites on the SmtpClient
                //SmtpClient client = new SmtpClient("smtp.cmyfleet.com");
                //SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp        

                //NetworkCredential credential = new NetworkCredential("pnara527@gmail.com", "narapriya");
                //client.UseDefaultCredentials = false;
                //client.Credentials = credential;
                //client.EnableSsl = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                NetworkCredential nc = new NetworkCredential("pnara527@gmail.com", "narapriya");
                smtp.Credentials = nc;

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
            }
            return true;
        }
    }
}

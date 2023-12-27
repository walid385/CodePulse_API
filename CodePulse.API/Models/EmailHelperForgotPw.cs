using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.Mail;

namespace Identity.Models
{
    public class EmailHelperForgotPw
    {
        public bool SendEmail(string userEmail, string confirmationLink)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress("wboulhazayez@gmail.com");
                mailMessage.To.Add(new MailAddress(userEmail));

                mailMessage.Subject = "Reset your Password";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = $"<a href='{confirmationLink}'>Click here to reset your password</a>";


                using (SmtpClient client = new SmtpClient())
                {
                    client.Credentials = new System.Net.NetworkCredential("wboulhazayez@gmail.com", "mxmn sinf plvx mwhg");
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.EnableSsl = true;

                    try
                    {
                        client.Send(mailMessage);
                        return true;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

    }
}
using System.Net.Mail;

namespace CodePulse.API.Models
{
    public class EmailHelperContactUs
    {
        public bool SendEmail(string Name, string Message, string Email)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(Email);
                mailMessage.To.Add(new MailAddress("webps.info@gmail.com"));

                mailMessage.Subject = $"From {Name}: {Email}";
                mailMessage.IsBodyHtml = true;

                // Injecting the HTML template
                mailMessage.Body = Message;

                using (SmtpClient client = new SmtpClient())
                {
                    client.Credentials = new System.Net.NetworkCredential("webps.info@gmail.com", "uvpm ioua bhkz jrjh");
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

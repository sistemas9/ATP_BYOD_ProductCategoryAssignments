using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ATP_BYOD_ProductCategoryAssignments
{
  public class SendEmail
  {
    private SmtpClient client = new SmtpClient();
    //private NetworkCredential Credential = new NetworkCredential("notificacioninterna@avanceytec.com.mx", "avanceytec");
    private MailMessage mail = new MailMessage();

    public async Task<Boolean> SendEmailAsync(String subject, String body, String[] to)
    {
      client.Host = "smtp.office365.com";
      client.Port = 587;
      client.DeliveryMethod = SmtpDeliveryMethod.Network;
      client.UseDefaultCredentials = false;
      client.Credentials = new NetworkCredential("notificacioninterna@avanceytec.com.mx", "8hn?X#~J?rvOI.+#=:E1S1}2C3xi6h)K");
      client.EnableSsl = true;
      client.DeliveryMethod = SmtpDeliveryMethod.Network;

      MailAddress fromATP = new MailAddress("notificacioninterna@avanceytec.com.mx", "Notificaciones Avance");
      mail.From = fromATP;
      foreach (String correo in to)
      {
        if (IsValidEmail(correo))
        {
          mail.To.Add(new MailAddress(correo, correo, Encoding.UTF8));
        }
        else
        {
          return false;
        }
      }
      mail.Body = body;
      mail.IsBodyHtml = true;
      mail.Subject = subject;

      try
      {
        client.Send(mail);
        mail.Attachments.Clear();
        mail.To.Clear();
        return true;
      }catch(Exception e)
      {
        Console.WriteLine(e.Message);
        return false;
      }
    }

    public Boolean IsValidEmail(string email)
    {
      if (email.Trim().EndsWith("."))
      {
        return false; // suggested by @TK-421
      }
      if (email.Contains(".@"))
      {
        return false; // suggested by jack(@Luis Armendariz)
      }
      try
      {
        var addr = new System.Net.Mail.MailAddress(email, email, Encoding.UTF8);
        Boolean isValid = addr.Host.Contains(".");
        if (isValid)
        {
          return addr.Address == email;
        }
        else
        {
          return false;
        }
      }
      catch
      {
        return false;
      }
    }
  }
}

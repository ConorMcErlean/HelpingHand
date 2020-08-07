using System.Collections.Generic;
using TagTool.Data.Models;
using RestSharp;

namespace TagTool.Data.Services
{
    public interface IEmailService
    {
        IRestResponse SendMessage (IList<User> MailList, Report report);
    }
}
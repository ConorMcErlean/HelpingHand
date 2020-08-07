using Microsoft.AspNetCore.Mvc;

namespace TagTool.Web.Controllers
{
    public enum AlertType { success, danger, warning, info }

    public class BaseController : Controller
    {
        public void Alert(string message, AlertType type = AlertType.info, string boldMessage = null)
        {
            TempData["Alert.Message"] = message;
            TempData["Alert.Type"] = type.ToString();
            TempData["Alert.BoldMessage"] = boldMessage;
        }

    }// BaseController
}
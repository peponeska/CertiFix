using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertiFix
{
    internal class ErrorHandling
    {
        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject);
        }

        public static void HandleException(Exception ex)
        {
            //Handle your Exception here
            var popupNotifier = Helper.PopupFactory();

            popupNotifier.ShowOptionsButton = false;
            popupNotifier.TitleText = "Exception occured!";
            popupNotifier.ContentText = ex.ToString();
            popupNotifier.Popup();
        }
    }
}

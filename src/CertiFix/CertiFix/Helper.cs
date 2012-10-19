using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using NotificationWindow;

namespace CertiFix
{
    internal static class Helper
    {
        /// <summary>
        /// Creates a new popup window. OnClick it hides. Close and grip are disabled. Options are enabled.
        /// Padding is set to 10 for title and picture. Content has no padding.
        /// </summary>
        /// <returns></returns>
        public static PopupNotifier PopupFactory()
        {
            var popup = new PopupNotifier();

            popup.Click += (o, e) => { ((PopupNotifier)o).Hide(); };
            popup.ShowCloseButton = false;
            popup.ShowOptionsButton = true;
            popup.ShowGrip = false;
            popup.Image = Properties.Resources.fire;
            popup.ImagePadding = new Padding(10);
            popup.TitlePadding = new Padding(10);
            popup.ContentPadding = new Padding(0);

            return popup;
        }


        /// <summary>
        /// Find last certificate in collection (based on NotAfter property)
        /// </summary>
        /// <param name="col">Collection to look in</param>
        /// <returns></returns>
        public static X509Certificate2 FindLast(this X509Certificate2Collection col)
        {
            X509Certificate2 last = null;

            if (col != null)
            {
                foreach (var adCert in col)
                {
                    if (last == null || last.NotAfter < adCert.NotAfter)
                        last = adCert;
                }
            }
            return last;
        }
    }
}

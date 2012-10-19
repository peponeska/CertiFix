using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows.Forms;
using NotificationWindow;

namespace CertiFix
{
    static class Program
    {
        internal static readonly int DaysToExpire = Properties.Settings.Default.DaysBeforeExpiring;
        internal static IntPtr handle = IntPtr.Zero;


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Error handling
            AppDomain.CurrentDomain.UnhandledException += ErrorHandling.CurrentDomain_UnhandledException;
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ErrorHandling.Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Initialize();

            Application.Run();
        }

        static void Initialize()
        {
            Form f = new Form();
            f.Icon = Properties.Resources.notify;
            f.ShowIcon = true;
            handle = f.Handle;

            var timer = new Timer();
            timer.Interval = Properties.Settings.Default.CertificateCheckInterval;
            timer.Tick += (o, e) => { CheckEncryptionCerts(); CheckExpiringCertificates(); };

            var notifyIcon1 = new System.Windows.Forms.NotifyIcon();
            notifyIcon1.Icon = Properties.Resources.notify;
            notifyIcon1.Text = "notifyIcon1";
            notifyIcon1.Visible = true;

            var cm = new System.Windows.Forms.ContextMenuStrip(new System.ComponentModel.Container());
            cm.Items.AddRange(
                new System.Windows.Forms.ToolStripItem[] { 
                    new System.Windows.Forms.ToolStripMenuItem("Check expiring certificates", null, (o, e) => CheckExpiringCertificates()),
                    new System.Windows.Forms.ToolStripMenuItem("Exit", Properties.Resources.exit, (o, e) => Application.Exit())
                });

            notifyIcon1.ContextMenuStrip = cm;

            timer.Enabled = true; // Enable it
            timer.Start();

            CheckEncryptionCerts();
            CheckExpiringCertificates();
        }

        static void CheckExpiringCertificates()
        {
            var certUtils = new CertificateUtils { DaysBeforeEnd = DaysToExpire };
            X509Certificate2Collection expiringCerts = certUtils.Check();
            if (expiringCerts.Count == 0)
                return;

            var popupNotifier = Helper.PopupFactory();

            ContextMenuStrip ContextMenu = new System.Windows.Forms.ContextMenuStrip(new System.ComponentModel.Container());
            ContextMenu.Items.AddRange(
                new System.Windows.Forms.ToolStripItem[] { 
                    new System.Windows.Forms.ToolStripMenuItem("Details", Properties.Resources.details, 
                        (o, e)=> X509Certificate2UI.SelectFromCollection(expiringCerts, "Expiring certificates", "Selecting certificate will do nothing.", X509SelectionFlag.MultiSelection, handle)) });

            popupNotifier.OptionsMenu = ContextMenu;

            popupNotifier.TitleText = "Some certificates are going to expire!";
            popupNotifier.ContentText = String.Format("{0} certificates are going to expire in less than {1} days", expiringCerts.Count, DaysToExpire);
            popupNotifier.Popup();
        }

        static void CheckEncryptionCerts()
        {
            var certUtils = new CertificateUtils();

            var myself = UserPrincipal.Current;
            DateTime notAfterDefault = new DateTime(1900, 1, 1);
            if (myself.Certificates.Count > 0)
            {
                var lastADCert = myself.Certificates.FindLast();
                notAfterDefault = lastADCert.NotAfter;
            }
            var localEncryptionCerts = certUtils.GetEncryptionCerts(notAfterDefault);

            if (localEncryptionCerts.Count == 0)
                return;

            var popupNotifier = Helper.PopupFactory();

            ContextMenuStrip ContextMenu = new System.Windows.Forms.ContextMenuStrip(new System.ComponentModel.Container());
            ContextMenu.Items.AddRange(
                new System.Windows.Forms.ToolStripItem[] { 
                    new System.Windows.Forms.ToolStripMenuItem("Details", Properties.Resources.details, 
                        (o, e)=> {
                            var certs = X509Certificate2UI.SelectFromCollection(localEncryptionCerts, "Certificates mising in AD", "Select encryption certificate(s) to add to AD.", X509SelectionFlag.MultiSelection, handle);
                            
                            if (certs.Count > 0)
                            {
                                myself.Certificates.AddRange(localEncryptionCerts);
                                myself.Save();
                            }
                        }) });

            popupNotifier.OptionsMenu = ContextMenu;

            popupNotifier.TitleText = "Some certificates are not in AD!";
            popupNotifier.ContentText = String.Format("{0} certificates are not in AD. People may not be able to send you encrypted mail.", localEncryptionCerts.Count);
            popupNotifier.Popup();
        }

        

        
    }
}

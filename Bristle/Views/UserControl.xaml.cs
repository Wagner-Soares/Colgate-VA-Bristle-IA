using APIVision;
using APIVision.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Bristle.Log;

namespace Bristle.Views
{
    /// <summary>
    /// Lógica interna para UserControl.xaml
    /// </summary>
    public partial class UserControl : Window
    {
        private BusinessSystem businessSystem = new BusinessSystem();
        private bool select = false;
        private Log.Log log = new Log.Log();

        public UserControl ()
        {
            try
            {
                Process prC = Process.GetCurrentProcess();
                foreach (Process pr2 in Process.GetProcessesByName("Bristle"))
                {
                    if (!pr2.HasExited && (pr2.Id != prC.Id))
                    {
                        pr2.Kill();
                    }
                }
            }
            catch(Exception ex)
            {
                //User does not have enough permission
                log.LogMessage("Error while trying to get or kill Bristles processes: " + ex.Message);
            }

            InitializeComponent();            

            #region Add User
            //Views.User user = new Views.User(true, businessSystem);
            //user.ShowDialog();
            #endregion

            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            try
            {
                if((businessSystem.userSystemModels = businessSystem.dataBaseController.listUserSystemModel()) == null)
                {
                    MessageBox.Show("Without connection to the database!");
                }
                //businessSystem.generalSystemSettingsModel.Threshold = 95;
                password.PasswordChar = '*';
            }
            catch
            {
                log.LogMessage("Error while trying to connect with SQL Database, please verify your network connection");
            }

            try
            {
                txtDomain.Text = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            }
            catch
            {
                log.LogMessage("Error while trying to get domain info, please verify your network connection");
            }

            this.Closed += new EventHandler(UserControl_Closed);
            this.Loaded += UserControl_Loaded;
        }

        private void UserControl_Loaded(object sender, EventArgs e)
        {
            try
            {
                txtDomain.Text = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;

                businessSystem.generalController.MountBat("DelSocketAI.bat", @"del /F ""C:\ProgramData\Colgate Vision\AI\nn_controller.pid""");
                businessSystem.generalController.MountBat("StopSocketAI.bat", @"C:\Vision\vision-venv\Scripts\python.exe -m nn_command.socket_controller stop");
                businessSystem.generalController.MountBat("StartSocketAI.bat", @"C:\Vision\vision-venv\Scripts\python.exe -m nn_command.socket_controller start");
                businessSystem.generalController.MountVbs("DelSocketAI");
                businessSystem.generalController.MountVbs("StopSocketAI");
                businessSystem.generalController.MountVbs("StartSocketAI");
            }
            catch(Exception ex)
            {
                log.LogMessage("Error while trying to creare Bats and VBs to control Vision-Venv sockets: " + ex.Message);
            }
        }     
        private void ButtonValidateUser_Click(object sender, RoutedEventArgs e)
        {
            ValidateUser();
        }

        private void ValidateUser()
        {            

            if (!domainValidateUser())
            {
                bool ValidatePassword = false;

                foreach (var v in businessSystem.userSystemModels)
                {
                    if (user.Text == v.Name)
                    {
                        businessSystem.userSystemCurrent.Name = v.Name;
                        businessSystem.userSystemCurrent.Type = v.Type;
                        ValidatePassword = businessSystem.userControlController.ValidatePassword(password.Password, v.Salt, v.Key);
                    }
                }

                if (ValidatePassword)
                {
                    this.Hide();
                    businessSystem.bristleInitController.init(businessSystem);
                    Views.MainWindow mainWindow = new Views.MainWindow(true, businessSystem);
                    mainWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Access Denied!");
                }
            }
            else
            {
                log.LogMessage("Successfully logged using domain user: " + user.Text.Substring(user.Text.IndexOf(@"\")+1) + " Domain: " + (user.Text
                                                                                                                                            .Contains(@"\") ? user.Text
                                                                                                                                                            .Substring(0,user.Text
                                                                                                                                                                         .IndexOf(@"\")) : "mf" ));
                this.Hide();
                businessSystem.bristleInitController.init(businessSystem);
                Views.MainWindow mainWindow = new Views.MainWindow(true, businessSystem);
                mainWindow.ShowDialog();
            }
        }

        public string[] GetGroupNames(string domainName, string userName)
        {
            List<string> result = new List<string>();

            using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, domainName))
            {
                try
                {
                    var userPrincipal = UserPrincipal.FindByIdentity(principalContext, userName);

                    if (userPrincipal is null)
                    {
                        throw new Exception("User " + userName + " cannot be found in domain: " + domainName);
                    }
                    else
                    {
                        using (PrincipalSearchResult<Principal> src = userPrincipal.GetGroups())
                        {
                            src.ToList().ForEach(sr => result.Add(sr.SamAccountName));
                        }
                    }
                }
                catch(Exception ex)
                {
                    log.LogMessage("Error while trying to get user groups from domain " + domainName + ". The program got the error: " + ex.Message);
                    result.Add("DefaultGroup");
                }
            }

            return result.ToArray();
        }

        public bool domainValidateUser()
        {
            try
            {
                string domain;

                if (user.Text.Contains(@"\"))
                {
                    domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;

                    //use case nl1 domain

                    if (user.Text.Substring(0, user.Text.IndexOf(@"\")) == "nl1")
                    {
                        domain = domain
                            .Replace(domain.Substring(0, domain.IndexOf('.')),
                                "nl.am");
                    }
                    else
                    {
                        domain = domain
                            .Replace(domain.Substring(0, domain.IndexOf('.')),
                                user.Text.Substring(0, user.Text.IndexOf(@"\")));
                    }
                }
                else
                {
                    domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                }

                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain))
                {
                    // validate the credentials
                    string userName;

                    if (user.Text.Contains(@"\"))
                    {
                        userName = user.Text.Replace(user.Text.Substring(0, user.Text.LastIndexOf(@"\") + 1), string.Empty);
                    }
                    else
                    {
                        userName = user.Text;
                    }
                    
                    bool isValid = pc.ValidateCredentials(userName, password.Password, ContextOptions.Negotiate);

                    if (isValid)
                    {
                        businessSystem.userSystemCurrent.Name = user.Text;
                        businessSystem.userSystemCurrent.Type = "";
                        businessSystem.networkUserModel.NetworkUser = user.Text;
                        businessSystem.networkUserModel.NetworkUserGroup = GetGroupNames(domain, user.Text);
                        return true;
                    }
                    else
                    {
                        return false;
                    }                                                  
                }
            }
            catch(Exception ex)
            {
                log.LogMessage("Error while validating user in domain:");
                return false;
            }
        }


        private void ButtonWindowClose_Click(object sender, RoutedEventArgs e)
        {
            shutdownSystem();
        }     

        private void user_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (user.Text.Contains(@"\"))
            {
                string domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;

                if (user.Text.Substring(0, user.Text.IndexOf(@"\")) == "nl1")
                {
                    domain = domain
                        .Replace(domain.Substring(0, domain.IndexOf('.')),
                            "nl.am");
                }
                else
                {
                    domain = domain
                        .Replace(domain.Substring(0, domain.IndexOf('.')),
                            user.Text.Substring(0, user.Text.IndexOf(@"\")));
                }

                txtDomain.Text = domain;
            }

            
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();            
        }

        private void KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                ValidateUser();
            }
            else if(e.Key == Key.Tab)
            {
                if (!select)
                {
                    password.Focus();
                    select = true;
                }
                else
                {
                    user.Focus();
                    select = false;
                }
            }
        }

        private void UserControl_Closed(object sender, EventArgs e)
        {
            shutdownSystem();
        }

        private void shutdownSystem()
        {
            businessSystem.generalController.CMD("./StopSocketAI.vbs");

            try
            {
                if (File.Exists(@"C:\ProgramData\Colgate Vision\AI\nn_controller.pid"))
                {
                    businessSystem.generalController.CMD("./DelSocketAI.vbs");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            warning.Visibility = Visibility.Visible;

            try
            {
                ProductionObject.dinoLiteSDK.Preview = false;
                ProductionObject.dinoLiteSDK.Connected = false;
                ProductionObject.dinoLiteSDK.PreviewScale = false;
            }
            catch
            {

            }

            Process prC = Process.GetCurrentProcess();
            foreach (Process pr2 in Process.GetProcessesByName("Bristle"))
            {
                if (!pr2.HasExited && (pr2.Id != prC.Id))
                {
                    pr2.Kill();
                }
            }
            Application.Current.Shutdown();
            System.Environment.Exit(1);
        }           
    }
}

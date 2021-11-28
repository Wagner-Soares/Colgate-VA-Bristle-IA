using APIVision;
using APIVision.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using APIVision.Controllers.DataBaseControllers;
using Database;
using Bristle.UseCases;
using Bristle.utils;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;

namespace Bristle.Views
{
    /// <summary>
    /// Lógica interna para UserControl.xaml
    /// </summary>
    public partial class UserControl : Window
    {
        private readonly BusinessSystem businessSystem = new BusinessSystem();
        private readonly System.Windows.Forms.Timer timerToLogIn = new System.Windows.Forms.Timer();

        private bool select = false;

        private readonly UserSystemController _userSystemController;

        private ColgateSkeltaEntities _colgateSkeltaEntities = new ColgateSkeltaEntities();

        public UserControl()
        {            
            _userSystemController = new UserSystemController(_colgateSkeltaEntities);

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
                Log.CustomLog.LogMessage("Error while trying to get or kill Bristles processes: " + ex.Message);
            }

            InitializeComponent();            

            try
            {
                if((businessSystem.UserSystemModels = _userSystemController.ListUserSystemModel()) == null)
                {
                    MessageBox.Show("Without connection to the database!");
                }
                password.PasswordChar = '*';
            }
            catch
            {
                Log.CustomLog.LogMessage("Error while trying to connect with SQL Database, please verify your network connection");
            }

            try
            {
                txtDomain.Text = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            }
            catch
            {
                Log.CustomLog.LogMessage("Error while trying to get domain info, please verify your network connection");
            }

            this.Closed += new EventHandler(UserControl_Closed);
            this.Loaded += UserControl_Loaded;
        }

        private void UserControl_Loaded(object sender, EventArgs e)
        {
            txtDomain.Text = UserControlUseCases.GetDomainName();

            timerToLogIn.Tick += new EventHandler(LogInUser);
        }

        private void ButtonValidateUser_Click(object sender, RoutedEventArgs e)
        {
            ButtonValidateUser.Background = Brushes.Gray;
            ButtonValidateUser.IsEnabled = false;

            timerToLogIn.Interval = 500;
            timerToLogIn.Start();
        }

        private void ValidateUser()
        {            

            if (!DomainValidateUser())
            {
                bool ValidatePassword = false;

                foreach (var v in businessSystem.UserSystemModels)
                {
                    if (user.Text == v.Name)
                    {
                        businessSystem.UserSystemCurrent.Name = v.Name;
                        businessSystem.UserSystemCurrent.Type = v.Type;
                        ValidatePassword = UserControlUseCases.ValidatePassword(password.Password, v.Salt, v.Key);
                    }
                }

                if (ValidatePassword)
                {
                    this.Hide();
                    Views.MainWindow mainWindow = new Views.MainWindow(true, businessSystem, _colgateSkeltaEntities);
                    mainWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Access Denied!");
                }
            }
            else
            {
                Log.CustomLog.LogMessage("Successfully logged using domain user: " + user.Text.Substring(user.Text.IndexOf(@"\")+1) + " Domain: " + (user.Text
                                                                                                                                            .Contains(@"\") ? user.Text
                                                                                                                                                            .Substring(0,user.Text
                                                                                                                                                                         .IndexOf(@"\")) : "mf" ));

                Views.MainWindow mainWindow = new Views.MainWindow(true, businessSystem, _colgateSkeltaEntities);
                mainWindow.ShowDialog();
                this.Hide();
                
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
                        Exception exception = new Exception("User " + userName + " cannot be found in domain: " + domainName);
                        throw exception;
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
                    Log.CustomLog.LogMessage("Error while trying to get user groups from domain " + domainName + ". The program got the error: " + ex.Message);
                    result.Add("DefaultGroup");
                }
            }

            return result.ToArray();
        }

        public bool DomainValidateUser()
        {
            try
            {
                bool isValid;
                string userName;
                string domain = UserControlUseCases.GetDomainName();

                if (user.Text.Contains(@"\"))
                {
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
                                
                if (user.Text.Contains(@"\"))
                {
                    userName = user.Text.Replace(user.Text.Substring(0, user.Text.LastIndexOf(@"\") + 1), string.Empty);
                }
                else
                {
                    userName = user.Text;
                }

                isValid = UserControlUseCases.ValidateUserInDomain(domain,userName, password.Password);

                if (isValid)
                {
                    businessSystem.UserSystemCurrent.Name = user.Text;
                    businessSystem.UserSystemCurrent.Type = "";
                    businessSystem.NetworkUserModel.NetworkUser = user.Text;
                    businessSystem.NetworkUserModel.NetworkUserGroup = GetGroupNames(domain, user.Text);
                }

                return isValid;
            }
            catch
            {
                Log.CustomLog.LogMessage("Error while validating user in domain:");
                return false;
            }
        }

        private void ButtonWindowClose_Click(object sender, RoutedEventArgs e)
        {
            ShutdownSystem();
        }     

        private void User_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (user.Text.Contains(@"\"))
            {
                string domain = UserControlUseCases.GetDomainName();

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

        private new void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();            
        }

        private new void KeyUp(object sender, KeyEventArgs e)
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
            ShutdownSystem();
        }

        private void ShutdownSystem()
        {
            DataHandlerUseCases.CMD("./StopSocketAI.vbs");

            try
            {
                DataHandlerUseCases.CMD("./DelSocketAI.vbs");
            }
            catch (Exception e)
            {
                Log.CustomLog.LogMessage("Error while deleting socket AI: " + e.Message);
            }

            warning.Visibility = Visibility.Visible;

            try
            {
                CameraObject.DinoLiteSDK.Preview = false;
                CameraObject.DinoLiteSDK.Connected = false;
                CameraObject.DinoLiteSDK.PreviewScale = false;
            }
            catch(Exception e)
            {
                Log.CustomLog.LogMessage("Error while stopping camera services: " + e.Message);
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

        private void LogInUser(object myObject, EventArgs eventArgs)
        {
            timerToLogIn.Stop();

            user.Text = string.Empty;
            password.Clear();

            ButtonValidateUser.Background = Brushes.Red;
            ButtonValidateUser.IsEnabled = true;

            businessSystem.NetworkUserModel.NetworkUserGroup = new string[] { "BRVA_TB_ WonderwareAdminsU" };
            businessSystem.UserSystemCurrent.Name = "Iago";

            _colgateSkeltaEntities = new ColgateSkeltaEntities();
            Views.MainWindow mainWindow = new Views.MainWindow(true, businessSystem, _colgateSkeltaEntities);
            mainWindow.ShowDialog();

            this.Hide();
        }
    }
}

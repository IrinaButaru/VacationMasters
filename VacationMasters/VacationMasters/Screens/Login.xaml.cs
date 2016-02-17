using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using VacationMasters.Essentials;
using VacationMasters.UserManagement;
using VacationMasters.Wrappers;


namespace VacationMasters.Screens
{
    public sealed partial class Login : UserControl, INotifyPropertyChanged
    {
        private IUserManager _userManager;
        private DbWrapper _dbWrapper;
        private bool _lePackage;
        private bool _canChangePassword;
        public static string userName = "";
        public Login()
        {
            this.DataContext = this;
            _dbWrapper = new DbWrapper();
            _userManager = new UserManager(_dbWrapper);
            InitializeComponent();
        }
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            IsOperationInProgress = true;
            string userName = this.txtBoxUsrName.Text;
            string password = this.pwdBox.Password;

            if (UserManager.CurrentUser != null)
            {
                string message = "You are already logged.";
                var messageDialog = new MessageDialog(message);
                messageDialog.ShowAsync();
                return;
            }
            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                int canLogin = _userManager.CanLogin(userName, password);
                if (canLogin == 0)
                {
                    var frame = (Frame) Window.Current.Content;
                    var page = (MainPage) frame.Content;
                    page.IsLogged = true;
                    page.IsNotLogged = false;
                    page.Hellou = "Hello " + userName;

                    string userType = _userManager.GetUser(userName).Type;
                    if (userType == "Admin")
                    {
                        page.IsAdmin = true;
                        page.IsNormalUser = true;
                    }
                    else if (userType == "Agent")
                    {
                        page.IsNormalUser = true;
                        page.IsAgent = true;
                    }
                    else
                    {
                        page.IsNormalUser = true;
                    }
                   
                }
                else
                if (canLogin == 1)
                {
                    String message = "You have been baned!";
                    var messageDialog = new MessageDialog(message);
                    messageDialog.ShowAsync();
                }
                else
                    if(canLogin == 2)
                {
                    String message = "Incorrect username or password!";
                    var messageDialog = new MessageDialog(message);
                    messageDialog.ShowAsync();
                }
                    else 
                        if (canLogin == 3)
                    {
                        String message = "All fields are mandatory!";
                        var messageDialog = new MessageDialog(message);
                        messageDialog.ShowAsync();   
                    }
                        else if (canLogin == 4)
                        {
                            string message = "Incorrect credentials!";
                            var messageDialog = new MessageDialog(message);
                            messageDialog.ShowAsync();
                        }
                        else
                        {
                            string message = "An error occured.Please try again later.";
                            var messageDialog = new MessageDialog(message);
                            messageDialog.ShowAsync();
                        }
                _userManager.Login(userName, password);
               
                var frame1 = (Frame)Window.Current.Content;
                var page1 = (MainPage)frame1.Content;
                page1.UpdatePackages();
                VisualStateManager.GoToState(page1, "PackagesV", true);
            });
            IsOperationInProgress = false;

            this.txtBoxUsrName.Text = String.Empty;
            this.pwdBox.Password = String.Empty;
           
        }

        private void ForgotPwd_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtBoxUsrName.Text == String.Empty || !_userManager.CheckIfUserExists(this.txtBoxUsrName.Text))
            {
                var messageDialog = new MessageDialog("Please retype your credentials and try again.");
                messageDialog.ShowAsync();
                return;
            }

            userName = this.txtBoxUsrName.Text;
            var frame = (Frame)Window.Current.Content;
            var page = (MainPage)frame.Content;
            VisualStateManager.GoToState(page, "ChangePasswordV", true);
            this.txtBoxUsrName.Text = String.Empty;
            this.pwdBox.Password = String.Empty;

        }

        public bool CanChangePassword
        {
            get
            {
                return _canChangePassword;
            }
            set
            {
                _canChangePassword = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isOperationInProgress ;
        public bool IsOperationInProgress
        {
            get { return _isOperationInProgress; }
            set
            {
                if (value != _isOperationInProgress)
                {
                    _isOperationInProgress = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            var frame = (Frame)Window.Current.Content;
            var page = (MainPage)frame.Content;
            VisualStateManager.GoToState(page, "RegisterControl", true);

            this.txtBoxUsrName.Text = String.Empty;
            this.pwdBox.Password = String.Empty;
        }

    }
}

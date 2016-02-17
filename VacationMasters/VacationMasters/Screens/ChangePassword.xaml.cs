using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using VacationMasters.UserManagement;
using VacationMasters.Wrappers;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace VacationMasters.Screens
{
    public sealed partial class ChangePassword : UserControl, INotifyPropertyChanged
    {

        private bool _isQuestionAnswered;
        private IUserManager _userManager;
        private bool _isQuestionHidden;

        #region Public Methods
        public ChangePassword()
        {
            this.DataContext = this;
            var dbWrapper = new DbWrapper();
            _userManager = new UserManager(dbWrapper);
            this.InitializeComponent();
            IsQuestionHidden = true;
        }

        public bool IsQuestionAnswered
        {
            get { return _isQuestionAnswered; }
            set
            {
                if (_isQuestionAnswered != value)
                {
                    _isQuestionAnswered = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsQuestionHidden
        {
            get { return _isQuestionHidden; }
            set
            {
                if (_isQuestionHidden != value)
                {
                    _isQuestionHidden = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

       
        #endregion

        #region Private Methods
        private void Validate(object sender, RoutedEventArgs e)             
        {
            string givenQuestion = AnswerBox.Password.ToString();
            string name = Screens.Login.userName;
            IsQuestionAnswered = _userManager.CheckAnswer(name, givenQuestion);
            IsQuestionHidden = !IsQuestionAnswered;

            if (IsQuestionHidden)
            {
                string message = "Wrong Answer!";
                var messageDialog = new MessageDialog(message);
                messageDialog.ShowAsync();
            }

            AnswerBox.Password = String.Empty;
        }

        private void ChangePwdBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!_userManager.CheckIfPasswordMach(pwdBox.Password.ToString(), confirmPwdBox.Password.ToString()))
            {
                String message = "Password don't match.";
                var messageDialog = new MessageDialog(message);
                messageDialog.ShowAsync();
                return;
            }

            if (_userManager.ChangePassword(Screens.Login.userName, pwdBox.Password.ToString()) == false)
            {
                String message = "You already have this password.";
                var messageDialog = new MessageDialog(message);
                messageDialog.ShowAsync();
            }

            var frame = (Frame)Window.Current.Content;
            var page = (MainPage)frame.Content; 
            VisualStateManager.GoToState(page, "LoginControl", true);
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}

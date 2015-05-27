using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using VacationMasters.Essentials;
using VacationMasters.UserManagement;
using VacationMasters.Wrappers;
using System.Threading.Tasks;

namespace VacationMasters.Screens
{
    public sealed partial class Register : UserControl, INotifyPropertyChanged
    {
        public UserManager UserManager { get; set; }
        public DbWrapper DbWrapper { get; set; }
        public GroupManager GroupManager { get; set; }
        private bool _isOperationInProgress;
        public Register()
        {
            this.DataContext = this;
            InitializeComponent();
            FillPreferencesAndGroups();
        }
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
        private async void FillPreferencesAndGroups()
        {
            IsOperationInProgress = true;
            DbWrapper = new DbWrapper();
            UserManager = new UserManager(DbWrapper);
            GroupManager = new GroupManager(DbWrapper);
            var preferences = await Task.Run(() => DbWrapper.GetAllPreferences());
            var groups = await Task.Run(() => GroupManager.GetAllGroups());
            CountriesGridView.ItemsSource = preferences.Where(c => c.Category == "Country").Select(d => d.Name).ToArray();
            TypesGridView.ItemsSource = preferences.Where(c => c.Category == "Type").Select(d => d.Name).ToArray();
            GroupsGridView.ItemsSource = groups.Select(c=>c.Trim()).ToArray();
            IsOperationInProgress = false;
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            
            if (VerifyRegisterFields() == false) return;
            var countriesPreferences = CountriesGridView.SelectedItems.Select(c => c.ToString());
            var typesPreferences = TypesGridView.SelectedItems.Select(c => c.ToString());
            var preferences = countriesPreferences.Concat(typesPreferences).ToList();
            var groups = GroupsGridView.SelectedItems.Select(c => c.ToString()).ToList();
            var user = new User(txtBoxUsrName.Text, txtBoxFrsName.Text, txtBoxLstName.Text, txtBoxEmail.Text, txtBoxPhone.Text, false, "User", null);
            UserManager.AddUser(user, pwdBox.Password, preferences, groups);
            EraseJunk();
        }

        private bool VerifyRegisterFields()
        {
            var completedFields = true;
            if (ChangeRequiredTextBlock(usrRequired, txtBoxUsrName) == false) completedFields = false;
            if (ChangeRequiredTextBlock(frsRequired, txtBoxFrsName) == false) completedFields = false;
            if (ChangeRequiredTextBlock(lstRequired, txtBoxLstName) == false) completedFields = false;
            if (ChangeRequiredTextBlock(emailRequired, txtBoxEmail) == false) completedFields = false;
            if (ChangeRequiredTextBlock(phoneRequired, txtBoxPhone) == false) completedFields = false;
            if (ChangeRequiredTextBlock(pwdRequired, pwdBox) == false) completedFields = false;
            if (ChangeRequiredTextBlock(confRequired, confirmPwdBox) == false) completedFields = false;
            if (string.IsNullOrEmpty(txtBoxUsrName.Text) == false && UserManager.CheckIfUserExists(txtBoxUsrName.Text))
            {
                usrRequired.FontSize = 11;
                usrRequired.VerticalAlignment = VerticalAlignment.Center;
                usrRequired.Text = "The username is already taken";
                completedFields = false;
            }
            if (string.IsNullOrEmpty(txtBoxEmail.Text) == false && IsValidEmail(txtBoxEmail.Text) == false)
            {
                emailRequired.FontSize = 11;
                emailRequired.VerticalAlignment = VerticalAlignment.Center;
                emailRequired.Text = "The e-mail is not valid";
                completedFields = false;
            }
            if (string.IsNullOrEmpty(txtBoxEmail.Text) == false && IsValidEmail(txtBoxEmail.Text) && UserManager.CheckIfEmailExists(txtBoxEmail.Text))
            {
                emailRequired.FontSize = 11;
                emailRequired.VerticalAlignment = VerticalAlignment.Center;
                emailRequired.Text = "The e-mail is already used";
                completedFields = false;
            }
            if (string.IsNullOrEmpty(txtBoxPhone.Text) == false && IsValidPhoneNumber(txtBoxPhone.Text) == false)
            {
                phoneRequired.FontSize = 11;
                phoneRequired.VerticalAlignment = VerticalAlignment.Center;
                phoneRequired.Text = "The phone is not valid";
                completedFields = false;
            }
            if (pwdBox.Password != confirmPwdBox.Password)
            {
                errorTxt.Text = "Password and Confirm Password do not match";
                completedFields = false;
            }
            return completedFields;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private bool ChangeRequiredTextBlock(TextBlock textblock, TextBox textbox)
        {
            if (string.IsNullOrEmpty(textbox.Text))
            {
                textblock.FontSize = 11;
                textblock.VerticalAlignment = VerticalAlignment.Center;
                textblock.Text = "This field is required!";
                return false;
            }
            return true;
        }
        private bool ChangeRequiredTextBlock(TextBlock textblock, PasswordBox passwordbox)
        {
            if (string.IsNullOrEmpty(passwordbox.Password))
            {
                textblock.FontSize = 11;
                textblock.VerticalAlignment = VerticalAlignment.Center;
                textblock.Text = "This field is required!";
                return false;
            }
            return true;
        }
        public bool IsValidEmail(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
              + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
              + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }
        public bool IsValidPhoneNumber(string phoneNumber)
        {
            string pattern = @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(phoneNumber);
        }

        private void EraseJunk()
        {
            txtBoxUsrName.Text = txtBoxFrsName.Text = txtBoxLstName.Text = txtBoxEmail.Text = txtBoxPhone.Text = String.Empty;
            pwdBox.Password = confirmPwdBox.Password = string.Empty;
        }

    }
}

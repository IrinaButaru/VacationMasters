using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using VacationMasters.Essentials;
using VacationMasters.UserManagement;
using VacationMasters.Wrappers;
using System.Threading.Tasks;

namespace VacationMasters.Screens
{
    public sealed partial class UserPage : UserControl, INotifyPropertyChanged
    {
        private DbWrapper _dbWrapper;
        private UserManager _userManager;
        private GroupManager _groupManager;

        private bool _isOperationInProgress;

        private DispatcherTimer dispatcherTimer;
        public UserPage()
        {
            this.DataContext = this;
            InitializeComponent();
            _dbWrapper = new DbWrapper();
            _userManager = new UserManager(_dbWrapper);
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            dispatcherTimer.Start();
           
        }

        private void dispatcherTimer_Tick(object sender, object e)
        {
            if (UserManager.CurrentUser == null)
                return;

            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { Initialize(); });
            dispatcherTimer.Stop();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
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

        private async void FillPreferencesCountry()
        {
            IsOperationInProgress = true;
            _dbWrapper = new DbWrapper();
            _userManager = new UserManager(_dbWrapper);
            var preferences = await Task.Run(() => _dbWrapper.GetAllPreferences());
            // var preferencesUseit Task.Run(() =>  _userManager.GetPreferencesCountryUser(UserName));
            List<string> preferencesUser = _userManager.GetPreferencesCountryUser(UserManager.CurrentUser.UserName);
            CountriesGridView.ItemsSource = preferences.Where(c => c.Category == "Country").Select(d => d.Name).ToArray();
            foreach (string i in preferencesUser)
            {
                if (CountriesGridView.Items != null)
                    foreach (var j in CountriesGridView.Items)
                    {
                        if (j.ToString().Equals(i))
                        {
                            CountriesGridView.SelectedItems.Add(j);
                        }
                    }
            }

            IsOperationInProgress = false;
        }



        private async void FillPreferencesType()
        {
            IsOperationInProgress = true;
            _dbWrapper = new DbWrapper();
            _userManager = new UserManager(_dbWrapper);
            var preferences = await Task.Run(() => _dbWrapper.GetAllPreferences());
            List<string> preferencesUser = _userManager.GetPreferencesTypeUser(UserManager.CurrentUser.UserName);
            TypesGridView.ItemsSource = preferences.Where(c => c.Category == "Type").Select(d => d.Name).ToArray();
            foreach (string i in preferencesUser)
            {
                if (TypesGridView.Items != null)
                    foreach (var j in TypesGridView.Items)
                    {
                        if (j.ToString().Equals(i))
                        {
                            TypesGridView.SelectedItems.Add(j);
                        }
                    }
            }

            IsOperationInProgress = false;
        }

        private async void FillGroups()
        {
            IsOperationInProgress = true;
            _groupManager = new GroupManager(_dbWrapper);
            var groups = await Task.Run(() => _groupManager.GetAllGroups());
            List<string> groupsUser = _groupManager.GetUserGroup(UserManager.CurrentUser.UserName);
            GroupsGridView.ItemsSource = groups.Select(c => c.Trim()).ToArray();
            foreach (string i in groupsUser)
            {
                if (GroupsGridView.Items != null)
                    foreach (var j in GroupsGridView.Items)
                    {
                        if (j.ToString().Equals(i))
                        {
                            GroupsGridView.SelectedItems.Add(j);
                        }
                    }
            }
            IsOperationInProgress = false;
        }




        private void Initialize()
        {

            FillPreferencesCountry();
            FillPreferencesType();
            FillGroups();
            FillRadioButton();

            FillText();
            FillPassword();
            FillConfirmPassword();

        }

        public static string UserName { set; get; }

        public void FillText()
        {
            text_box_email.Text = _userManager.GetMail(UserManager.CurrentUser.UserName);
        }
        public void FillPassword()
        {
            password_box.Password = "Password";
        }
        public void FillConfirmPassword()
        {
            confirm_password_box.Password = "Password";
        }

        private void OrderHistory(object sender, RoutedEventArgs e)//OrderHistory
        {
            var frame = (Frame)Window.Current.Content;
            var page = (MainPage)frame.Content;
            VisualStateManager.GoToState(page, "CancelOrderControl", true);
        }
        private void Browse(object sender, RoutedEventArgs e)
        {

        }
        private void LogOut(object sender, RoutedEventArgs e)//LogOut
        {
            //this.Frame.Navigate(typeof(MainPage), null);
        }

        //public bool Checked { get; set; }

        private void FillRadioButton()
        {
            var n = _userManager.GetNewsletter(UserName);

            if (n == 1)
            {
                radio_button.IsChecked = true;
            }
            else
            {
                radio_button.IsChecked = false;
            }
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            bool var;

            if (radio_button.IsChecked == true)

                var = true;
            else
                var = false;

            var countriesPreferences = CountriesGridView.SelectedItems.Select(c => c.ToString());
            var typesPreferences = TypesGridView.SelectedItems.Select(c => c.ToString());
            var preferences = countriesPreferences.Concat(typesPreferences).ToList();
            var groups = GroupsGridView.SelectedItems.Select(c => c.ToString()).ToList();
            _userManager.UpdateUser(
                UserManager.CurrentUser.UserName,
                var,
                text_box_email.Text,
                password_box.Password,
                confirm_password_box.Password,
                preferences,
                groups
                );

        }
    }
}

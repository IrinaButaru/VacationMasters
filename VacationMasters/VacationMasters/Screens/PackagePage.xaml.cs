using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using VacationMasters.Essentials;
using VacationMasters.PackageManagement;
using VacationMasters.UserManagement;
using VacationMasters.Wrappers;

// The User Control item template is documented at http://go.micosoft.com/fwlink/?LinkId=234236

namespace VacationMasters.Screens
{
    public sealed partial class PackagePage : UserControl, INotifyPropertyChanged
    {

        private readonly PackageManager _packageManager;
        private int _orderId;

        public PackagePage()
        {
            InitializeComponent();
            var dbWrapper = new DbWrapper();
            _packageManager = new PackageManager(dbWrapper);
        }
        private bool _isRatingEnabled;
        public bool IsRatingEnabled
        {
            get { return _isRatingEnabled; }
            set
            {
                if (value != _isRatingEnabled)
                {
                    _isRatingEnabled = value;
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
        private void ReserveOrCancel_OnClick(object sender, RoutedEventArgs e)
        {
            var package = MainPage.SelectedPackage;
            if (UserManager.CurrentUser == null)
            {
                var messageDialog = new MessageDialog("Please login in order to reserve");
                messageDialog.ShowAsync();
                return;
            }
            if (_orderId == 0)
            {
                _packageManager.ReservePackage(UserManager.CurrentUser.UserName, DateTime.Now, package.Price, package.ID);
                this.UpdatePackage();
                var messageDialog = new MessageDialog("The reservation has been made");
                messageDialog.ShowAsync();
            }
            else
            {
                _packageManager.CancelReservation(package.ID, _orderId);
                this.UpdatePackage();
                var messageDialog = new MessageDialog("The order has been canceled");
                messageDialog.ShowAsync();
            }
        }

        public void UpdatePackage()
        {
            var package = MainPage.SelectedPackage;
            var tab = "  ";
            imagePackage.Source = package.Photo;
            NameTextBlock.Text = tab + package.Name;
            TypeTextBlock.Text = tab + package.Type;
            IncludedTextBlock.Text = tab + package.Included;
            PriceTextBlock.Text = tab + package.Price;
            TransportTextBlock.Text = tab + package.Transport;
            DateTextBlock.Text = tab + package.BeginDate + " - " + package.EndDate;
            Rating.Value = package.Rating;
            if (UserManager.CurrentUser != null)
            {
                _orderId = _packageManager.CheckIfUserHasOrderedThePackage(package.ID, UserManager.CurrentUser.UserName);
                var hasVoted = _packageManager.CheckIfUserDidVote(package.ID, UserManager.CurrentUser.UserName);
                if (_orderId != 0) ReserveOrCancel.Content = "Cancel";
                else ReserveOrCancel.Content = "Reserve";
                if (_orderId != 0 && hasVoted == false)
                { IsRatingEnabled = true; }
                else IsRatingEnabled = false;
            }
        }

        private void Rating_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (UserManager.CurrentUser != null)
                _packageManager.UpdateRating(MainPage.SelectedPackage.ID, Rating.Value, _orderId);
        }
    }
}
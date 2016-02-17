using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using VacationMasters.UserManagement;
using VacationMasters.Wrappers;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace VacationMasters.Screens
{
    public sealed partial class CancelOrder : UserControl, INotifyPropertyChanged
    {
        private bool _isOperationInProgress;
        private DispatcherTimer dispatcherTimer;
        private UserManager _userManager;

        private DbWrapper _dbWrapper;
        public CancelOrder()
        {
            this.DataContext = this;
            this.InitializeComponent();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            dispatcherTimer.Start();
           
        }

        private void dispatcherTimer_Tick(object sender, object e)
        {
            if (UserManager.CurrentUser == null)
                return;
            FillOrders();
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

        private async void FillOrders()
        {
            IsOperationInProgress = true;
            _dbWrapper = new DbWrapper();
            _userManager = new UserManager(_dbWrapper);
            List<string> orderUser = _userManager.GetOrders(UserManager.CurrentUser.UserName);
            OrdersGridView.ItemsSource = orderUser.Select(c => c.Trim()).ToArray();
            IsOperationInProgress = false;
        }
    }
}

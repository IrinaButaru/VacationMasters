using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using VacationMasters.Wrappers;
using VacationMasters.UserManagement;
using VacationMasters.Essentials;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using VacationMasters.PackageManagement;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace VacationMasters.Screens
{
    public sealed partial class PackagesDisplay : UserControl, INotifyPropertyChanged
    {
        private bool _isOperationInProgress;
        private string _title = String.Empty;
        private string _price = String.Empty;

        private DispatcherTimer dispatcherTimer;
        private byte[] _photo;

        private IUserManager _userManager;
        private ObservableCollection<Package> _list;
        public UserManager UserManager { get; set; }
        public PackageManager PackManager { get; set; }
        public DbWrapper DbWrapper { get; set; }
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

        public PackagesDisplay()
        {
            this.DataContext = this;

            this.InitializeComponent();

            DbWrapper = new DbWrapper();

            Initialize();
        }

 

        public async void Initialize()
        {
            IsOperationInProgress = true;

            DbWrapper = new DbWrapper();
            UserManager = new UserManager(DbWrapper);
       
            PackManager = new PackageManager(DbWrapper);

            if (UserManager.CurrentUser == null) 
                List = new ObservableCollection<Package>(DbWrapper.getRandomPackages());
            else
                List = new ObservableCollection<Package>(PackManager.GetPackagesByRecommendation());
           
            IsOperationInProgress = false;
        }

        public ObservableCollection<Package> List
        {
            get { return _list; }
            set
            {
                _list = value;
                NotifyPropertyChanged();
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

        private void ItemGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var frame = (Frame)Window.Current.Content;
            var page = (MainPage)frame.Content;
            page.UpdateSelectedPackage((Package)e.ClickedItem);
       
            VisualStateManager.GoToState(page, "PackagePage", true);
        }
    }
}


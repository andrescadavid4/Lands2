

namespace Lands.ViewsModels
{
    using GalaSoft.MvvmLight.Command;
    using System;
    using System.ComponentModel;
    using System.Windows.Input;
    using Xamarin.Forms;
    using Views;
    using Services;
    using Helpers;

    public class LoginViewModel : BaseViewModel
    {
        #region Services
        private ApiService apiServices;
        #endregion

        #region Attributes
        private string email;
        private string password;
        private bool isRunning;
        private bool isEnabled;
        #endregion

        #region Properties
        public string Email
        {
            get { return this.email; }
            set { SetValue(ref this.email, value); }
        }

        public string Password
        {
            get { return this.password; }
            set { SetValue(ref this.password, value); }
        }

        public bool IsRunning
        {
            get { return this.isRunning; }
            set { SetValue(ref this.isRunning, value); }
        }

        public bool IsRemembered
        {
            get;
            set;
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { SetValue(ref this.isEnabled, value); }
        }
        #endregion

        #region Contructors
        public LoginViewModel()
        {
            this.IsRemembered = true;
            this.IsEnabled = true;
            this.apiServices = new ApiService();
        }
        #endregion

        #region Commands
        public ICommand LoginCommand
        {
            get
            {
                return new RelayCommand(Login);
            }
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(this.Email))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.EmailValidation,
                    Languages.Accept
                    );
                return;
            }

            if (string.IsNullOrEmpty(this.Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordValidation,
                    Languages.Accept
                    );
                return;
            }
            this.IsRunning = true;
            this.isEnabled = false;

            //Validar conexión a internet
            var connection = await apiServices.CheckConnection();

            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.isEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    connection.Message,
                    Languages.Accept
                    );
                return;
            }
            // Consumo de Token para el Login
            var token = await this.apiServices.GetToken(
                "https://landsapimaya.azurewebsites.net",
                this.Email,
                this.Password
                );

            if (token == null)
            {
                this.IsRunning = false;
                this.isEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    "Something was wrong, please try later",
                    Languages.Accept
                    );
                return;
            }

            if (string.IsNullOrEmpty(token.AccessToken))
            {
                this.IsRunning = false;
                this.isEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    token.ErrorDescription,
                    Languages.Accept
                    );
                this.Password = string.Empty;
                return;
            }
            var mainviewmodel = MainViewModel.GetInstance();
            mainviewmodel.Token = token;
            mainviewmodel.Lands = new LandsViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new LandsPage());

            this.IsRunning = false;
            this.isEnabled = true;

            this.Email = string.Empty;
            this.Password = string.Empty;
        }
        #endregion
    }
}

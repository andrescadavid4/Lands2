namespace Lands.ViewsModels
{
    using Models;
    using System;
    using System.Collections.ObjectModel;
    using Services;
    using Xamarin.Forms;
    using System.Collections.Generic;

    class LandsViewModel : BaseViewModel
    {
        #region Services
        private ApiService apiServices;
        #endregion

        #region Attributes
        private ObservableCollection<Land> lands;
        #endregion

        #region Propoerties
        // Se vuelve Observable conlection para poderla mostrar en la ListView
        public ObservableCollection<Land> Lands
        {
            get { return this.lands; }
            set { SetValue(ref this.lands, value); }
        }
        #endregion

        #region Constructors
        public LandsViewModel()
        {
            this.apiServices = new ApiService();
            this.LoadLands();
        }
        #endregion

        #region Methods
        private async void LoadLands()
        {
            var response = await this.apiServices.GetList<Land>(
                "http://restcountries.eu",
                "/rest",
                "/v2/all"
                );

            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Accept"
                    );
                return;
            }

            var list = (List<Land>)response.Result;
            this.Lands = new ObservableCollection<Land>(list);
        } 
        #endregion
    }
}

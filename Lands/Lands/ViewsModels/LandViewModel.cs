namespace Lands.ViewsModels
{
    using Models;

    public class LandViewModel
    {
        #region properties
        public Land Land
        {
            get;
            set;
        }
        #endregion

        #region Constructors
        public LandViewModel(Land land)
        {
            this.Land = land;
        } 
        #endregion
    }
}

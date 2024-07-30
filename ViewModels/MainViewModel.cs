namespace PropsGen.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        public ConnectionViewModel ConnectionViewModel { get; } = new ConnectionViewModel();
        public PropsViewModel PropsViewModel { get; } = new PropsViewModel();
    }
}

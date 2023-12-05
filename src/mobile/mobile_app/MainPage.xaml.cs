namespace mobile_app;

public partial class MainPage : ContentPage
{
    private MainViewModel _viewModel;

    public MainPage()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        BindingContext = _viewModel;
        _viewModel.UpdateDate("Program started");

        // Use 'await' to wait for the completion of the asynchronous method
         _ = MakeRequestAsync();
    }

    // Define an asynchronous method to make the request
    private async Task MakeRequestAsync()
    {
        await _viewModel.MakeRequest();
    }

}


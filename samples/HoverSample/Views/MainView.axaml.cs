using Avalonia.Controls;
using HoverSample.ViewModels;
using System;

namespace HoverSample.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is MainViewModel viewModel)
        {
            MapControlName.Map = viewModel.Map;
        }
    }
}

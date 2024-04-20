using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CountdownV2.Controls;
using CountdownV2.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace CountdownV2.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();
        ViewModel = new MainViewModel();
        CountdownTextBox = this.FindControl<TextBox>("CountdownTextBox");
        this.WhenActivated(ManageBindings);
        AttachedToVisualTree += ManageVisualTree;
    }

    private void ManageBindings(CompositeDisposable disposables)
    {
        this.OneWayBind(ViewModel, vm => vm.LogText, v => v.CountdownTextBox.Text)
            .DisposeWith(disposables);
    }

    private void ManageVisualTree(object? sender, VisualTreeAttachmentEventArgs args)
    {
        if (sender is not Control control) return;
        control.PropertyChanged += (o, e) =>
        {
            if (e.Property != BoundsProperty) return;
            var newBounds = (Rect)e.NewValue;
            CountdownTextBox.FontSize = newBounds.Width / 20;
        };
    }

    private void OnConfettiButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        var buttonCenter = new Point(button.Bounds.Center.X, button.Bounds.Center.Y);
        var canvas = this.FindControl<ConfettiCanvas>("DrawingControl");
        canvas.StartAnimation(buttonCenter);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
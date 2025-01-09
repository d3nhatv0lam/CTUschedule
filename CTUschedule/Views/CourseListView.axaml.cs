using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace CTUschedule.Views;

public partial class CourseListView : UserControl
{
    public CourseListView()
    {
        InitializeComponent();
       
    }

    private void _Unloaded(object? sender, RoutedEventArgs e)
    {
        this.DataContext = null;
        this.Unloaded -= _Unloaded;
    }
}
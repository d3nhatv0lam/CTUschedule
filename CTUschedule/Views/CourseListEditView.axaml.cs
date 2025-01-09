using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DialogHostAvalonia;
using System.Diagnostics;
using Avalonia.Interactivity;

namespace CTUschedule.Views;

public partial class CourseListEditView : UserControl
{
    public CourseListEditView()
    {
        InitializeComponent();
        //this.Unloaded += _Unloaded;
    }

    private void _Unloaded(object? sender, RoutedEventArgs e)
    {
        this.DataContext = null;
        this.Unloaded -= _Unloaded;
    }
}
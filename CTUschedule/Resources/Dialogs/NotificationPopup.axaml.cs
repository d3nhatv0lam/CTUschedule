using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Diagnostics;
using Material;

using System.Windows;
using System.Threading;
using System.Threading.Tasks;

namespace CTUschedule;

public partial class NotificationPopup : Window
{
    // milisec 5200
    private int WaitTime = 5100;

    public NotificationPopup()
    {
        InitializeComponent();
        //SetPopupPosition();
        //SetStatusNotification("AlertOctagonOutline", "#971c38", "#FF0000", "Lỗi", "Có gì đó không hợp lệ");
        //AlertOctagonOutline
    }

    public NotificationPopup(int Posindex, string symbolKind, string symbolColorHex, string LineColorHex, string Title, string Message)
    {
        InitializeComponent();
        SetPopupPosition(Posindex);

        SetStatusNotification( symbolKind, symbolColorHex, LineColorHex, Title, Message);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        WaitAndClose();
    }

    private async void WaitAndClose()
    {
        await Task.Delay(WaitTime);
        this.DataContext = null;
        Close();
    }

    private void SetStatusNotification(string symbolKind, string symbolColorHex,string LineColorHex , string Title, string Message)
    {
        icon.Text = symbolKind;
        icon.Foreground = new SolidColorBrush(Color.Parse(symbolColorHex));
        line.Background = new SolidColorBrush(Color.Parse(LineColorHex));
        title.Text = Title;
        description.Text = Message;
    }

    private void SetPopupPosition(int Posindex)
    {
        var primaryScreen = this.Screens.Primary;

        // Lấy độ rộng và chiều cao của màn hình
        int DesktopWidth = primaryScreen.WorkingArea.Width;
        int DesktopHeight = primaryScreen.WorkingArea.Height;

        this.Position = new PixelPoint((DesktopWidth - (int)this.Width - 20), (DesktopHeight - (int)this.Height*(Posindex+1) - Posindex*10 - 20));
    }
}
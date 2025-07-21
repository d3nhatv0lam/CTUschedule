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
using Material.Icons;

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

    public NotificationPopup(int Posindex, MaterialIconKind symbolKind, string symbolColorHex, string LineColorHex, string Title, string Message)
    {
        InitializeComponent();
        this.DataContext = this;
        SetStatusNotification(symbolKind, symbolColorHex, LineColorHex, Title, Message);
        this.Opened += (s, e) =>
        {
            SetPopupPosition(Posindex);
        };
        
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

    private void Close_Click(object sender, RoutedEventArgs e) 
    {
        this.DataContext = null;
        Close();
    }

    private void SetStatusNotification(MaterialIconKind symbolKind, string symbolColorHex,string LineColorHex , string Title, string Message)
    {
       
        PopupIcon.Kind = symbolKind;
        PopupIcon.Foreground = new SolidColorBrush(Color.Parse(symbolColorHex));
        line.Background = new SolidColorBrush(Color.Parse(LineColorHex));
        title.Text = Title;
        description.Text = Message;
    }

    private void SetPopupPosition(int posIndex, int marginDip = 20, int spacingDip = 10)
    {
        //var primaryScreen = this.Screens.Primary;

        //// Lấy độ rộng và chiều cao của màn hình
        //int DesktopWidth = primaryScreen.WorkingArea.Width;
        //int DesktopHeight = primaryScreen.WorkingArea.Height;

        //int x = (DesktopWidth - (int)this.Width - marginDip);
        //int y = (DesktopHeight - (int)this.Height * (posIndex + 1) - posIndex * spacingDip - marginDip);

        //this.Position = new PixelPoint(x , y);

        var primaryScreen = this.Screens.Primary;
        double scale = primaryScreen.Scaling; // hệ số scale

        int DesktopWidth = primaryScreen.WorkingArea.Width;
        int DesktopHeight = primaryScreen.WorkingArea.Height;

        // Chuyển kích thước cửa sổ sang pixel
        int winWidth = (int)(this.Width * scale);
        int winHeight = (int)(this.Height * scale);

        // Margin, spacing cũng đổi sang pixel
        int marginPx = (int)(marginDip * scale);
        int spacingPx = (int)(spacingDip * scale);

        int x = DesktopWidth - winWidth - marginPx;
        int y = DesktopHeight - winHeight * (posIndex + 1) - posIndex * spacingPx - marginPx;

        this.Position = new PixelPoint(x, y);
    }
}
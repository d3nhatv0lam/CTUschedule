using NETWORKLIST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;
using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

using System.Runtime.InteropServices;
using System.Net.NetworkInformation;


namespace CTUschedule.Utilities
{
    // Class auto detect Internet status
    public class CheckerInternetHelper : INetworkListManagerEvents, IDisposable
    {
        private INetworkListManager networkListManager;
        private IConnectionPoint connectionPoint;
        private int cookie;

        [DllImport("ole32.dll")]
        private static extern int CoCreateInstance(
        [In] ref Guid rclsid,
        [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkOuter,
        [In] uint dwClsContext,
        [In] ref Guid riid,
        [Out, MarshalAs(UnmanagedType.Interface)] out INetworkListManager ppv);

        private static Guid CLSID_NetworkListManager = new Guid("DCB00C01-570F-4A9B-8D69-199FDBA5723B");
        private static Guid IID_INetworkListManager = new Guid("DCB00000-570F-4A9B-8D69-199FDBA5723B");
        // global Internet tracker
        public static bool _isHasInternet;
        public bool IsHasInternet
        {
            get => _isHasInternet;
            set
            {

                _isHasInternet = value;
                OnInternetChanged();
            }
        }
        public event EventHandler _internetChanged;

        public event EventHandler InternetChanged
        {
            add
            {
                _internetChanged += value;
            }
            remove 
            {
                _internetChanged -= value;
            }
        }

        public void OnInternetChanged()
        {
            if (_internetChanged != null)
            {
                _internetChanged(this, new EventArgs());
            }
        }



        public CheckerInternetHelper()
        {
            networkListManager = new NetworkListManager();
            IConnectionPointContainer container = (IConnectionPointContainer)networkListManager;
            Guid guid = typeof(INetworkListManagerEvents).GUID;
            container.FindConnectionPoint(ref guid, out connectionPoint);
            connectionPoint.Advise(this, out cookie);
            //First check
            IsHasInternet = IsConnectedToInternet();
        }



        // libary auto detect when internet changed?
        public void ConnectivityChanged(NLM_CONNECTIVITY newConnectivity)
        {
            // is disconnect
            if (newConnectivity == NLM_CONNECTIVITY.NLM_CONNECTIVITY_DISCONNECTED ||
               // dont have ipv4
               ((int)newConnectivity & (int)NLM_CONNECTIVITY.NLM_CONNECTIVITY_IPV4_INTERNET) == 0 &&
                 // dont have ipv6
                 ((int)newConnectivity & (int)NLM_CONNECTIVITY.NLM_CONNECTIVITY_IPV6_INTERNET) == 0)
            {
                // Internet is down
                //Debug.WriteLine("pay internet");
                IsHasInternet = false;
            }
            else
            {
                // Internet is running
                //Debug.WriteLine("internet co lai r");
                IsHasInternet = true;
            }
        }


        public static bool IsConnectedToInternet()
        {
            INetworkListManager networkListManager = null;
            try
            {
                CoCreateInstance(ref CLSID_NetworkListManager, null, 1, ref IID_INetworkListManager, out networkListManager);
                return networkListManager.IsConnectedToInternet;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (networkListManager != null)
                {
                    Marshal.ReleaseComObject(networkListManager);
                }
            }
        }

        public void Dispose()
        {
            // Unadvise from connection point
            if (connectionPoint != null && cookie != 0)
            {
                connectionPoint.Unadvise(cookie);
                cookie = 0;
            }

            // Release COM objects
            if (connectionPoint != null)
            {
                Marshal.ReleaseComObject(connectionPoint);
                connectionPoint = null;
            }

            if (networkListManager != null)
            {
                Marshal.ReleaseComObject(networkListManager);
                networkListManager = null;
            }
        }
    }
}

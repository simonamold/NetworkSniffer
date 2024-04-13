using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpPcap;
using SharpPcap.LibPcap;

namespace NetworkSniffer
{
    public partial class NetworkSnifferFrm : Form
    {
        private bool BackgroundThreadStop;

        private object QueueLock = new object();

        private List<RawCapture> PacketQueue = new List<RawCapture>();

        private DateTime LastStatisticsOutput;

        private TimeSpan LastStatisticsInterval = new TimeSpan(0, 0, 2);

        private Thread backgroundThread;

        private CaptureDeviceList devices;

        private ILiveDevice device;

        public NetworkSnifferFrm()
        {
            InitializeComponent();
        }

        private void NetworkSnifferFrm_Load(object sender, EventArgs e)
        {
            devices = CaptureDeviceList.Instance;

            foreach (var dev in devices)
            {
                var str = String.Format("{0} {1}", dev.Name, dev.Description);
                interfaceListCmb.Items.Add(str);
            }

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            int selecteDevice = interfaceListCmb.SelectedIndex;
            device = devices[selecteDevice];
            device.OnPacketArrival +=
               new PacketArrivalEventHandler(device_OnPacketArrival);

            int readTimeoutMilliseconds = 1000;
            device.Open(mode: DeviceModes.Promiscuous 
                | DeviceModes.DataTransferUdp 
                | DeviceModes.NoCaptureLocal,
                read_timeout: readTimeoutMilliseconds);

            device.StartCapture();

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            device.StopCapture();
        }

        private static void device_OnPacketArrival(object sender, PacketCapture e)
        {
            var time = e.Header.Timeval.Date;
            var len = e.Data.Length;
            var rawPacket = e.GetPacket();
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
                time.Hour, time.Minute, time.Second, time.Millisecond, len);
            Console.WriteLine(rawPacket.ToString());
            
        }
    }
}

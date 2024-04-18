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
using NetworkSniffer.UI;
using NetworkSniffer.Utils;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;

namespace NetworkSniffer
{
    public partial class NetworkSnifferFrm : Form
    {
        private List<RawCapture> PacketQueue = new List<RawCapture>();
      
        private CaptureDeviceList devices;
       
        private ILiveDevice device;

        private Queue<PacketWrapper> packetStrings;

        //private Queue<Packet> packetStrings;

        private int packetCount;
        
        private BindingSource bs;
        
        private ICaptureStatistics captureStatistics;
        
        private DateTime LastStatisticsOutput;
        
        private TimeSpan LastStatisticsInterval = new TimeSpan(0, 0, 2);

        private bool statisticsUiNeedsUpdate = false;

        private bool stopThread;
        
        private object QueueLock = new object();

        private Thread backgroundThread;

        private static CaptureFileWriterDevice captureFileWriter;

        private SaveFileDialog fileDialog;

        private List<RawCapture> packets = new List<RawCapture>();

        private PacketDetailForm detailForm;
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
            StartCapture(selecteDevice);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (device != null)
            {
                device.StopCapture();   
                stopThread = true;
                backgroundThread.Join();
            }
        }

        private void device_OnPacketArrival(object sender, PacketCapture e)
        {
            var Now = DateTime.Now;
            var interval = Now - LastStatisticsOutput;
            if (interval > LastStatisticsInterval)
            {
                Console.WriteLine("device_OnPacketArrival: " + e.Device.Statistics);
                captureStatistics = e.Device.Statistics;
                statisticsUiNeedsUpdate = true;
                LastStatisticsOutput = Now;
            }

            lock(QueueLock)
            {
                PacketQueue.Add(e.GetPacket());
            }
        }

        private void StartCapture(int index)
        {
            packetCount = 0;
            
            packetStrings = new Queue<PacketWrapper>();

            //packetStrings = new Queue<Packet>();
            bs = new BindingSource();
            dataGridViewPackets.DataSource = bs;
            LastStatisticsOutput = DateTime.Now;

            stopThread = false;
            backgroundThread = new Thread(BackgroundThread);
            backgroundThread.Start();

            try
            {
                device = devices[index];
                device.OnPacketArrival +=
                   new PacketArrivalEventHandler(device_OnPacketArrival);

                int readTimeoutMilliseconds = 1000;
                device.Open(mode: DeviceModes.Promiscuous
                    | DeviceModes.DataTransferUdp
                    | DeviceModes.NoCaptureLocal,
                    read_timeout: readTimeoutMilliseconds);

                captureStatistics = device.Statistics;

                device.StartCapture();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Legeti interfata de ascultare");
            }
        }

        private void BackgroundThread()
        {
            while(!stopThread)
            {
                bool shouldSleep = true;

                lock(QueueLock)
                {
                    if (PacketQueue.Count != 0)
                    {
                        shouldSleep = false;
                    }
                }

                if(shouldSleep)
                {
                    Thread.Sleep(250);
                }
                else
                {
                    List<RawCapture> ourQueue;
                    lock (QueueLock)
                    {
                        ourQueue = PacketQueue;
                        PacketQueue = new List<RawCapture>();
                    }

                    foreach (var packet in ourQueue)
                    {
                        packets.Add(packet);
                        var packetWrapper = new PacketWrapper(packetCount, packet);
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                             /*packetStrings.Enqueue(Packet.
                                               ParsePacket(packetWrapper.p.LinkLayerType,
                                               packetWrapper.p.Data));*/
                            packetStrings.Enqueue(packetWrapper);
                        }
                        ));

                        packetCount++;

                        var time = packet.Timeval.Date;
                        var len = packet.Data.Length;
                        Console.WriteLine("BackgroundThread: {0}:{1}:{2},{3} Len={4}",
                            time.Hour, time.Minute, time.Second, time.Millisecond, len);
                    }

                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        bs.DataSource = packetStrings.Reverse();
                    }
                    ));

                    if (statisticsUiNeedsUpdate)
                    {
                        UpdateCaptureStatistics();
                        statisticsUiNeedsUpdate = false;
                    }
                }
            }
        }

        private void UpdateCaptureStatistics()
        {
            toolStripStatusLabel1.Text = string.Format("Received packets: {0}, " +
                "Dropped packets: {1}," +
                " Interface dropped packets: {2}",
                captureStatistics.ReceivedPackets,
                captureStatistics.DroppedPackets,
                captureStatistics.InterfaceDroppedPackets);
        }

        private void dataGridViewPackets_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            fileDialog = new SaveFileDialog();
            fileDialog.ShowDialog();
            fileDialog.InitialDirectory = @"D:\";
            fileDialog.RestoreDirectory = true;
            fileDialog.Title = "Choose Location";
            fileDialog.DefaultExt = "pcap";
            fileDialog.CheckFileExists = true;
            string name = fileDialog.FileName;

            if (!name.Equals("")) {
                // string name = "capture.pcap";
                captureFileWriter = new CaptureFileWriterDevice(name);
                captureFileWriter.Open(device);

                foreach (var rawPacket in packets)
                {
                    captureFileWriter.Write(rawPacket);
                }

                var file = new CaptureFileReaderDevice(name);
                if (file != null)
                {
                    MessageBox.Show("Salvare cu success!");
                    file.Close();
                }
                else
                {
                    MessageBox.Show("Salvare fisier esuata!");
                    file.Close();
                }

                captureFileWriter.Close();
                if (device != null)
                {
                    device.Close();
                    device = null;
                } 
            }
        }

        private void dataGridViewPackets_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dataGridViewPackets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var packetWrapper = (PacketWrapper)dataGridViewPackets.Rows[dataGridViewPackets.SelectedCells[0].RowIndex].DataBoundItem;
            var packet = Packet.ParsePacket(packetWrapper.p.LinkLayerType, packetWrapper.p.Data);


            detailForm = new PacketDetailForm(packet);
            detailForm.Show();
            if (detailForm.DialogResult == DialogResult.Cancel)
            {
                detailForm.Hide();
                detailForm = null;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            packets.Clear();
            packetStrings.Clear();
            bs.DataSource = null;
            toolStripStatusLabel1.Text = string.Empty;
        }
    }
}

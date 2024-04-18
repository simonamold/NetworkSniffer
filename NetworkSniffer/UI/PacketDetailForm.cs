using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PacketDotNet;

namespace NetworkSniffer.UI
{
    public partial class PacketDetailForm : Form
    {
        private Packet packet;
        public PacketDetailForm(Packet packet)
        {
            this.packet = packet;
            InitializeComponent();
        }

        private void PacketDetailForm_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = packet.ToString(StringOutputType.VerboseColored);
        }
    }
}

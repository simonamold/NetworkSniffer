namespace NetworkSniffer
{
    partial class NetworkSnifferFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.interfaceListCmb = new System.Windows.Forms.ComboBox();
            this.lstReceivedPackets = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(13, 13);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(119, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Choose interface:";
            // 
            // interfaceListCmb
            // 
            this.interfaceListCmb.FormattingEnabled = true;
            this.interfaceListCmb.Location = new System.Drawing.Point(16, 86);
            this.interfaceListCmb.Name = "interfaceListCmb";
            this.interfaceListCmb.Size = new System.Drawing.Size(786, 24);
            this.interfaceListCmb.TabIndex = 3;
            // 
            // lstReceivedPackets
            // 
            this.lstReceivedPackets.HideSelection = false;
            this.lstReceivedPackets.Location = new System.Drawing.Point(16, 116);
            this.lstReceivedPackets.Name = "lstReceivedPackets";
            this.lstReceivedPackets.Size = new System.Drawing.Size(121, 97);
            this.lstReceivedPackets.TabIndex = 4;
            this.lstReceivedPackets.UseCompatibleStateImageBehavior = false;
            // 
            // NetworkSnifferFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lstReceivedPackets);
            this.Controls.Add(this.interfaceListCmb);
            this.Controls.Add(iths.label1);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Name = "NetworkSnifferFrm";
            this.Text = "Network Sniffer";
            this.Load += new System.EventHandler(this.NetworkSnifferFrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox interfaceListCmb;
        private System.Windows.Forms.ListView lstReceivedPackets;
    }
}


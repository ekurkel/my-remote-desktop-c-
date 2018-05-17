namespace Remote_Admin
{
    partial class MainWindow
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
            this.clientBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.bConnect = new System.Windows.Forms.Button();
            this.serverBox = new System.Windows.Forms.GroupBox();
            this.bServStart = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.clientBox.SuspendLayout();
            this.serverBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // clientBox
            // 
            this.clientBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.clientBox.Controls.Add(this.label1);
            this.clientBox.Controls.Add(this.label2);
            this.clientBox.Controls.Add(this.textBoxIP);
            this.clientBox.Controls.Add(this.bConnect);
            this.clientBox.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.clientBox.Location = new System.Drawing.Point(3, 67);
            this.clientBox.Name = "clientBox";
            this.clientBox.Size = new System.Drawing.Size(168, 141);
            this.clientBox.TabIndex = 0;
            this.clientBox.TabStop = false;
            this.clientBox.Text = "Client";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(30, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = " (or computer name)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(30, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Enter IP-address";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(9, 41);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(153, 21);
            this.textBoxIP.TabIndex = 2;
            // 
            // bConnect
            // 
            this.bConnect.Location = new System.Drawing.Point(6, 85);
            this.bConnect.Name = "bConnect";
            this.bConnect.Size = new System.Drawing.Size(156, 46);
            this.bConnect.TabIndex = 0;
            this.bConnect.Text = "Connect";
            this.bConnect.UseVisualStyleBackColor = true;
            this.bConnect.Click += new System.EventHandler(this.bConnect_Click);
            // 
            // serverBox
            // 
            this.serverBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serverBox.Controls.Add(this.bServStart);
            this.serverBox.Controls.Add(this.label6);
            this.serverBox.Controls.Add(this.label5);
            this.serverBox.Controls.Add(this.label4);
            this.serverBox.Controls.Add(this.label3);
            this.serverBox.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.serverBox.Location = new System.Drawing.Point(177, 67);
            this.serverBox.Name = "serverBox";
            this.serverBox.Size = new System.Drawing.Size(213, 141);
            this.serverBox.TabIndex = 1;
            this.serverBox.TabStop = false;
            this.serverBox.Text = "Server";
            // 
            // bServStart
            // 
            this.bServStart.Location = new System.Drawing.Point(57, 85);
            this.bServStart.Name = "bServStart";
            this.bServStart.Size = new System.Drawing.Size(95, 46);
            this.bServStart.TabIndex = 5;
            this.bServStart.Text = "Server Start";
            this.bServStart.UseVisualStyleBackColor = true;
            this.bServStart.Click += new System.EventHandler(this.bServStart_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(82, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 17);
            this.label6.TabIndex = 4;
            this.label6.Text = "label6";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(124, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 17);
            this.label5.TabIndex = 3;
            this.label5.Text = "aasscacw";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(6, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "Ip-address: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 17);
            this.label3.TabIndex = 1;
            this.label3.Text = "Computer name:";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 208);
            this.Controls.Add(this.serverBox);
            this.Controls.Add(this.clientBox);
            this.Name = "MainWindow";
            this.Text = "Remote Admin";
            this.clientBox.ResumeLayout(false);
            this.clientBox.PerformLayout();
            this.serverBox.ResumeLayout(false);
            this.serverBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox clientBox;
        private System.Windows.Forms.GroupBox serverBox;
        private System.Windows.Forms.Button bConnect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bServStart;
    }
}


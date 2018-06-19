namespace Remote_Admin
{
    partial class ServerForm
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.listViewClients = new System.Windows.Forms.ListView();
            this.ClientID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ComputerName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.UserName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.IP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.remoteDesctopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.materialLabel1 = new MaterialSkin.Controls.MaterialLabel();
            this.materialLabel2 = new MaterialSkin.Controls.MaterialLabel();
            this.Label6 = new MaterialSkin.Controls.MaterialLabel();
            this.Label7 = new MaterialSkin.Controls.MaterialLabel();
            this.SendToAllButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.RunAtAllButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.CloseAllConnectionsButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.materialDivider1 = new MaterialSkin.Controls.MaterialDivider();
            this.TaskManagerButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.CommandLineButton = new MaterialSkin.Controls.MaterialRaisedButton();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(7, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "Waiting for connections....";
            // 
            // listViewClients
            // 
            this.listViewClients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ClientID,
            this.ComputerName,
            this.UserName,
            this.IP});
            this.listViewClients.ContextMenuStrip = this.contextMenuStrip1;
            this.listViewClients.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listViewClients.FullRowSelect = true;
            this.listViewClients.GridLines = true;
            this.listViewClients.Location = new System.Drawing.Point(237, 67);
            this.listViewClients.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listViewClients.MultiSelect = false;
            this.listViewClients.Name = "listViewClients";
            this.listViewClients.Size = new System.Drawing.Size(387, 388);
            this.listViewClients.TabIndex = 12;
            this.listViewClients.UseCompatibleStateImageBehavior = false;
            this.listViewClients.View = System.Windows.Forms.View.Details;
            // 
            // ClientID
            // 
            this.ClientID.Text = "Client ID";
            this.ClientID.Width = 70;
            // 
            // ComputerName
            // 
            this.ComputerName.Text = "ComputerName";
            this.ComputerName.Width = 110;
            // 
            // UserName
            // 
            this.UserName.Text = "User";
            this.UserName.Width = 100;
            // 
            // IP
            // 
            this.IP.Text = "IP ";
            this.IP.Width = 100;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.remoteDesctopToolStripMenuItem,
            this.sendFileToolStripMenuItem,
            this.runFileToolStripMenuItem,
            this.disconnectToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(162, 92);
            // 
            // remoteDesctopToolStripMenuItem
            // 
            this.remoteDesctopToolStripMenuItem.Name = "remoteDesctopToolStripMenuItem";
            this.remoteDesctopToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.remoteDesctopToolStripMenuItem.Text = "Remote Desktop";
            this.remoteDesctopToolStripMenuItem.Click += new System.EventHandler(this.remoteDesctopToolStripMenuItem_Click);
            // 
            // sendFileToolStripMenuItem
            // 
            this.sendFileToolStripMenuItem.Name = "sendFileToolStripMenuItem";
            this.sendFileToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.sendFileToolStripMenuItem.Text = "Send file";
            this.sendFileToolStripMenuItem.Click += new System.EventHandler(this.sendFileToolStripMenuItem_Click);
            // 
            // runFileToolStripMenuItem
            // 
            this.runFileToolStripMenuItem.Name = "runFileToolStripMenuItem";
            this.runFileToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.runFileToolStripMenuItem.Text = "Run file";
            this.runFileToolStripMenuItem.Click += new System.EventHandler(this.runFileToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // materialLabel1
            // 
            this.materialLabel1.AutoSize = true;
            this.materialLabel1.Depth = 0;
            this.materialLabel1.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel1.Location = new System.Drawing.Point(7, 103);
            this.materialLabel1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel1.Name = "materialLabel1";
            this.materialLabel1.Size = new System.Drawing.Size(86, 19);
            this.materialLabel1.TabIndex = 20;
            this.materialLabel1.Text = "Ip-address: ";
            // 
            // materialLabel2
            // 
            this.materialLabel2.AutoSize = true;
            this.materialLabel2.Depth = 0;
            this.materialLabel2.Font = new System.Drawing.Font("Roboto", 11F);
            this.materialLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialLabel2.Location = new System.Drawing.Point(7, 131);
            this.materialLabel2.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialLabel2.Name = "materialLabel2";
            this.materialLabel2.Size = new System.Drawing.Size(120, 19);
            this.materialLabel2.TabIndex = 21;
            this.materialLabel2.Text = "Computer name:";
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Depth = 0;
            this.Label6.Font = new System.Drawing.Font("Roboto", 11F);
            this.Label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Label6.Location = new System.Drawing.Point(89, 103);
            this.Label6.MouseState = MaterialSkin.MouseState.HOVER;
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(53, 19);
            this.Label6.TabIndex = 22;
            this.Label6.Text = "Label6";
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Depth = 0;
            this.Label7.Font = new System.Drawing.Font("Roboto", 11F);
            this.Label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Label7.Location = new System.Drawing.Point(123, 131);
            this.Label7.MouseState = MaterialSkin.MouseState.HOVER;
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(108, 19);
            this.Label7.TabIndex = 23;
            this.Label7.Text = "materialLabel4";
            // 
            // SendToAllButton
            // 
            this.SendToAllButton.Depth = 0;
            this.SendToAllButton.Location = new System.Drawing.Point(12, 170);
            this.SendToAllButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.SendToAllButton.Name = "SendToAllButton";
            this.SendToAllButton.Primary = true;
            this.SendToAllButton.Size = new System.Drawing.Size(102, 37);
            this.SendToAllButton.TabIndex = 26;
            this.SendToAllButton.Text = "Send to all ";
            this.SendToAllButton.UseVisualStyleBackColor = true;
            this.SendToAllButton.Click += new System.EventHandler(this.SendToAllButton_Click);
            // 
            // RunAtAllButton
            // 
            this.RunAtAllButton.Depth = 0;
            this.RunAtAllButton.Location = new System.Drawing.Point(12, 213);
            this.RunAtAllButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.RunAtAllButton.Name = "RunAtAllButton";
            this.RunAtAllButton.Primary = true;
            this.RunAtAllButton.Size = new System.Drawing.Size(102, 37);
            this.RunAtAllButton.TabIndex = 27;
            this.RunAtAllButton.Text = "Run at all";
            this.RunAtAllButton.UseVisualStyleBackColor = true;
            this.RunAtAllButton.Click += new System.EventHandler(this.RunAtAllButton_Click);
            // 
            // CloseAllConnectionsButton
            // 
            this.CloseAllConnectionsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CloseAllConnectionsButton.Depth = 0;
            this.CloseAllConnectionsButton.Location = new System.Drawing.Point(12, 407);
            this.CloseAllConnectionsButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.CloseAllConnectionsButton.Name = "CloseAllConnectionsButton";
            this.CloseAllConnectionsButton.Primary = true;
            this.CloseAllConnectionsButton.Size = new System.Drawing.Size(115, 39);
            this.CloseAllConnectionsButton.TabIndex = 28;
            this.CloseAllConnectionsButton.Text = "Close all connections";
            this.CloseAllConnectionsButton.UseVisualStyleBackColor = true;
            this.CloseAllConnectionsButton.Click += new System.EventHandler(this.closeAllConnectionsButton_Click);
            // 
            // materialDivider1
            // 
            this.materialDivider1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.materialDivider1.Depth = 0;
            this.materialDivider1.Location = new System.Drawing.Point(7, 163);
            this.materialDivider1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialDivider1.Name = "materialDivider1";
            this.materialDivider1.Size = new System.Drawing.Size(224, 1);
            this.materialDivider1.TabIndex = 29;
            this.materialDivider1.Text = "materialDivider1";
            // 
            // TaskManagerButton
            // 
            this.TaskManagerButton.Depth = 0;
            this.TaskManagerButton.Location = new System.Drawing.Point(120, 213);
            this.TaskManagerButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.TaskManagerButton.Name = "TaskManagerButton";
            this.TaskManagerButton.Primary = true;
            this.TaskManagerButton.Size = new System.Drawing.Size(103, 37);
            this.TaskManagerButton.TabIndex = 30;
            this.TaskManagerButton.Text = "Task manager";
            this.TaskManagerButton.UseVisualStyleBackColor = true;
            this.TaskManagerButton.Click += new System.EventHandler(this.TaskManagerButton_Click);
            // 
            // CommandLineButton
            // 
            this.CommandLineButton.Depth = 0;
            this.CommandLineButton.Location = new System.Drawing.Point(120, 170);
            this.CommandLineButton.MouseState = MaterialSkin.MouseState.HOVER;
            this.CommandLineButton.Name = "CommandLineButton";
            this.CommandLineButton.Primary = true;
            this.CommandLineButton.Size = new System.Drawing.Size(102, 37);
            this.CommandLineButton.TabIndex = 31;
            this.CommandLineButton.Text = "Command line";
            this.CommandLineButton.UseVisualStyleBackColor = true;
            this.CommandLineButton.Click += new System.EventHandler(this.CommandLineButton_Click);
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 458);
            this.Controls.Add(this.CommandLineButton);
            this.Controls.Add(this.TaskManagerButton);
            this.Controls.Add(this.materialDivider1);
            this.Controls.Add(this.CloseAllConnectionsButton);
            this.Controls.Add(this.RunAtAllButton);
            this.Controls.Add(this.SendToAllButton);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.materialLabel2);
            this.Controls.Add(this.materialLabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewClients);
            this.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(627, 390);
            this.Name = "ServerForm";
            this.Sizable = false;
            this.Text = "Remote Admin | Server";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listViewClients;
        private System.Windows.Forms.ColumnHeader ClientID;
        private System.Windows.Forms.ColumnHeader UserName;
        private System.Windows.Forms.ColumnHeader ComputerName;
        private MaterialSkin.Controls.MaterialLabel materialLabel1;
        private MaterialSkin.Controls.MaterialLabel materialLabel2;
        private MaterialSkin.Controls.MaterialLabel Label6;
        private MaterialSkin.Controls.MaterialLabel Label7;
        private MaterialSkin.Controls.MaterialRaisedButton SendToAllButton;
        private MaterialSkin.Controls.MaterialRaisedButton RunAtAllButton;
        private MaterialSkin.Controls.MaterialRaisedButton CloseAllConnectionsButton;
        private MaterialSkin.Controls.MaterialDivider materialDivider1;
        private System.Windows.Forms.ColumnHeader IP;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem remoteDesctopToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem runFileToolStripMenuItem;
        private MaterialSkin.Controls.MaterialRaisedButton TaskManagerButton;
        private MaterialSkin.Controls.MaterialRaisedButton CommandLineButton;
    }
}
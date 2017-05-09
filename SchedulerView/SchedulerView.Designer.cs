namespace Scheduler
{
    partial class SchedulerView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.LeftContainer = new System.Windows.Forms.SplitContainer();
            this.TaskNameContainer = new System.Windows.Forms.Panel();
            this.TaskNamePanel = new System.Windows.Forms.PictureBox();
            this.ResourceNameContainer = new System.Windows.Forms.Panel();
            this.ResourceNamePanel = new System.Windows.Forms.PictureBox();
            this.RightContainer = new System.Windows.Forms.SplitContainer();
            this.TaskContainer = new System.Windows.Forms.Panel();
            this.TaskPanel = new System.Windows.Forms.PictureBox();
            this.ResourceContainer = new System.Windows.Forms.Panel();
            this.ResourcePanel = new System.Windows.Forms.PictureBox();
            this.TimePanel = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.LeftContainer)).BeginInit();
            this.LeftContainer.Panel1.SuspendLayout();
            this.LeftContainer.Panel2.SuspendLayout();
            this.LeftContainer.SuspendLayout();
            this.TaskNameContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TaskNamePanel)).BeginInit();
            this.ResourceNameContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResourceNamePanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RightContainer)).BeginInit();
            this.RightContainer.Panel1.SuspendLayout();
            this.RightContainer.Panel2.SuspendLayout();
            this.RightContainer.SuspendLayout();
            this.TaskContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TaskPanel)).BeginInit();
            this.ResourceContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResourcePanel)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // LeftContainer
            // 
            this.LeftContainer.Dock = System.Windows.Forms.DockStyle.Left;
            this.LeftContainer.IsSplitterFixed = true;
            this.LeftContainer.Location = new System.Drawing.Point(0, 0);
            this.LeftContainer.Name = "LeftContainer";
            this.LeftContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // LeftContainer.Panel1
            // 
            this.LeftContainer.Panel1.Controls.Add(this.TaskNameContainer);
            // 
            // LeftContainer.Panel2
            // 
            this.LeftContainer.Panel2.Controls.Add(this.ResourceNameContainer);
            this.LeftContainer.Size = new System.Drawing.Size(150, 228);
            this.LeftContainer.SplitterDistance = 44;
            this.LeftContainer.SplitterWidth = 6;
            this.LeftContainer.TabIndex = 0;
            // 
            // TaskNameContainer
            // 
            this.TaskNameContainer.BackColor = System.Drawing.Color.White;
            this.TaskNameContainer.Controls.Add(this.TaskNamePanel);
            this.TaskNameContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TaskNameContainer.Location = new System.Drawing.Point(0, 0);
            this.TaskNameContainer.Name = "TaskNameContainer";
            this.TaskNameContainer.Size = new System.Drawing.Size(150, 44);
            this.TaskNameContainer.TabIndex = 0;
            // 
            // TaskNamePanel
            // 
            this.TaskNamePanel.BackColor = System.Drawing.Color.White;
            this.TaskNamePanel.Location = new System.Drawing.Point(0, 0);
            this.TaskNamePanel.Name = "TaskNamePanel";
            this.TaskNamePanel.Size = new System.Drawing.Size(100, 50);
            this.TaskNamePanel.TabIndex = 0;
            this.TaskNamePanel.TabStop = false;
            // 
            // ResourceNameContainer
            // 
            this.ResourceNameContainer.BackColor = System.Drawing.Color.White;
            this.ResourceNameContainer.Controls.Add(this.ResourceNamePanel);
            this.ResourceNameContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResourceNameContainer.Location = new System.Drawing.Point(0, 0);
            this.ResourceNameContainer.Name = "ResourceNameContainer";
            this.ResourceNameContainer.Size = new System.Drawing.Size(150, 178);
            this.ResourceNameContainer.TabIndex = 0;
            // 
            // ResourceNamePanel
            // 
            this.ResourceNamePanel.BackColor = System.Drawing.Color.White;
            this.ResourceNamePanel.Location = new System.Drawing.Point(0, 0);
            this.ResourceNamePanel.Name = "ResourceNamePanel";
            this.ResourceNamePanel.Size = new System.Drawing.Size(100, 50);
            this.ResourceNamePanel.TabIndex = 0;
            this.ResourceNamePanel.TabStop = false;
            // 
            // RightContainer
            // 
            this.RightContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RightContainer.Location = new System.Drawing.Point(150, 0);
            this.RightContainer.Name = "RightContainer";
            this.RightContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // RightContainer.Panel1
            // 
            this.RightContainer.Panel1.Controls.Add(this.TaskContainer);
            // 
            // RightContainer.Panel2
            // 
            this.RightContainer.Panel2.Controls.Add(this.ResourceContainer);
            this.RightContainer.Size = new System.Drawing.Size(331, 228);
            this.RightContainer.SplitterDistance = 97;
            this.RightContainer.SplitterWidth = 6;
            this.RightContainer.TabIndex = 1;
            // 
            // TaskContainer
            // 
            this.TaskContainer.AutoScroll = true;
            this.TaskContainer.BackColor = System.Drawing.Color.White;
            this.TaskContainer.Controls.Add(this.TaskPanel);
            this.TaskContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TaskContainer.Location = new System.Drawing.Point(0, 0);
            this.TaskContainer.Name = "TaskContainer";
            this.TaskContainer.Size = new System.Drawing.Size(331, 97);
            this.TaskContainer.TabIndex = 0;
            // 
            // TaskPanel
            // 
            this.TaskPanel.BackColor = System.Drawing.Color.White;
            this.TaskPanel.Location = new System.Drawing.Point(0, 0);
            this.TaskPanel.Name = "TaskPanel";
            this.TaskPanel.Size = new System.Drawing.Size(100, 50);
            this.TaskPanel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.TaskPanel.TabIndex = 0;
            this.TaskPanel.TabStop = false;
            // 
            // ResourceContainer
            // 
            this.ResourceContainer.AutoScroll = true;
            this.ResourceContainer.BackColor = System.Drawing.Color.White;
            this.ResourceContainer.Controls.Add(this.ResourcePanel);
            this.ResourceContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ResourceContainer.Location = new System.Drawing.Point(0, 0);
            this.ResourceContainer.Name = "ResourceContainer";
            this.ResourceContainer.Size = new System.Drawing.Size(331, 125);
            this.ResourceContainer.TabIndex = 0;
            // 
            // ResourcePanel
            // 
            this.ResourcePanel.BackColor = System.Drawing.Color.White;
            this.ResourcePanel.Location = new System.Drawing.Point(0, 0);
            this.ResourcePanel.Name = "ResourcePanel";
            this.ResourcePanel.Size = new System.Drawing.Size(100, 50);
            this.ResourcePanel.TabIndex = 0;
            this.ResourcePanel.TabStop = false;
            // 
            // TimePanel
            // 
            this.TimePanel.BackColor = System.Drawing.Color.LightGray;
            this.TimePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TimePanel.Location = new System.Drawing.Point(0, 0);
            this.TimePanel.Name = "TimePanel";
            this.TimePanel.Size = new System.Drawing.Size(481, 30);
            this.TimePanel.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.RightContainer);
            this.panel2.Controls.Add(this.LeftContainer);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 30);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(481, 228);
            this.panel2.TabIndex = 2;
            this.panel2.SizeChanged += new System.EventHandler(this.OnPanelSizeChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // SchedulerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.TimePanel);
            this.Name = "SchedulerView";
            this.Size = new System.Drawing.Size(481, 258);
            this.LeftContainer.Panel1.ResumeLayout(false);
            this.LeftContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LeftContainer)).EndInit();
            this.LeftContainer.ResumeLayout(false);
            this.TaskNameContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TaskNamePanel)).EndInit();
            this.ResourceNameContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ResourceNamePanel)).EndInit();
            this.RightContainer.Panel1.ResumeLayout(false);
            this.RightContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RightContainer)).EndInit();
            this.RightContainer.ResumeLayout(false);
            this.TaskContainer.ResumeLayout(false);
            this.TaskContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TaskPanel)).EndInit();
            this.ResourceContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ResourcePanel)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer LeftContainer;
        private System.Windows.Forms.SplitContainer RightContainer;
        private System.Windows.Forms.Panel TaskContainer;
        private System.Windows.Forms.PictureBox TaskPanel;
        private System.Windows.Forms.Panel ResourceContainer;
        private System.Windows.Forms.PictureBox ResourcePanel;
        private System.Windows.Forms.Panel TaskNameContainer;
        private System.Windows.Forms.PictureBox TaskNamePanel;
        private System.Windows.Forms.Panel ResourceNameContainer;
        private System.Windows.Forms.PictureBox ResourceNamePanel;
        private System.Windows.Forms.Panel TimePanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Timer timer1;
    }
}

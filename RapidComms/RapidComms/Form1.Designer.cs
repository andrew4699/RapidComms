namespace RapidComms
{
    partial class Form1
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
            ChangeClipboardChain(this.Handle, nextClipboardViewer);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.sendButton = new System.Windows.Forms.Button();
            this.clipboardContents = new System.Windows.Forms.Label();
            this.clipboardType = new System.Windows.Forms.Label();
            this.statusIndicator = new System.Windows.Forms.Panel();
            this.clipboardImage = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clipboardImage)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.panel1.Controls.Add(this.clipboardType);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(-3, -3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(383, 25);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "RapidComms";
            // 
            // sendButton
            // 
            this.sendButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.sendButton.FlatAppearance.BorderSize = 0;
            this.sendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sendButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sendButton.ForeColor = System.Drawing.Color.White;
            this.sendButton.Location = new System.Drawing.Point(-3, 76);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(383, 21);
            this.sendButton.TabIndex = 1;
            this.sendButton.TabStop = false;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = false;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // clipboardContents
            // 
            this.clipboardContents.AutoSize = true;
            this.clipboardContents.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clipboardContents.ForeColor = System.Drawing.Color.White;
            this.clipboardContents.Location = new System.Drawing.Point(1, 25);
            this.clipboardContents.MaximumSize = new System.Drawing.Size(350, 50);
            this.clipboardContents.Name = "clipboardContents";
            this.clipboardContents.Size = new System.Drawing.Size(57, 17);
            this.clipboardContents.TabIndex = 2;
            this.clipboardContents.Text = "contents";
            // 
            // clipboardType
            // 
            this.clipboardType.AutoSize = true;
            this.clipboardType.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clipboardType.ForeColor = System.Drawing.Color.White;
            this.clipboardType.Location = new System.Drawing.Point(247, 4);
            this.clipboardType.Name = "clipboardType";
            this.clipboardType.Size = new System.Drawing.Size(78, 17);
            this.clipboardType.TabIndex = 3;
            this.clipboardType.Text = "UNKNOWN";
            // 
            // statusIndicator
            // 
            this.statusIndicator.BackColor = System.Drawing.Color.Silver;
            this.statusIndicator.Location = new System.Drawing.Point(364, 0);
            this.statusIndicator.Name = "statusIndicator";
            this.statusIndicator.Size = new System.Drawing.Size(17, 97);
            this.statusIndicator.TabIndex = 3;
            // 
            // clipboardImage
            // 
            this.clipboardImage.Location = new System.Drawing.Point(127, 25);
            this.clipboardImage.Name = "clipboardImage";
            this.clipboardImage.Size = new System.Drawing.Size(106, 45);
            this.clipboardImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.clipboardImage.TabIndex = 4;
            this.clipboardImage.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(378, 97);
            this.Controls.Add(this.clipboardImage);
            this.Controls.Add(this.statusIndicator);
            this.Controls.Add(this.clipboardContents);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "RapidComms";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clipboardImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Label clipboardType;
        private System.Windows.Forms.Label clipboardContents;
        private System.Windows.Forms.Panel statusIndicator;
        private System.Windows.Forms.PictureBox clipboardImage;
    }
}


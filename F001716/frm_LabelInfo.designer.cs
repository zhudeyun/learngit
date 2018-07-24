namespace F001716
{
    partial class frm_LabelInfo
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
            this.txt_KitLabel = new System.Windows.Forms.TextBox();
            this.lbl_BagLabel = new System.Windows.Forms.Label();
            this.btn_Abort = new System.Windows.Forms.Button();
            this.btn_Reset = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.txt_MACLabel = new System.Windows.Forms.TextBox();
            this.Productlabel1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txt_KitLabel
            // 
            this.txt_KitLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_KitLabel.Location = new System.Drawing.Point(16, 50);
            this.txt_KitLabel.Name = "txt_KitLabel";
            this.txt_KitLabel.Size = new System.Drawing.Size(283, 29);
            this.txt_KitLabel.TabIndex = 0;
            this.txt_KitLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_KitLabel.TextChanged += new System.EventHandler(this.txt_KitLabel_TextChanged);
            this.txt_KitLabel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_ScanEntrys_KeyDown);
            // 
            // lbl_BagLabel
            // 
            this.lbl_BagLabel.AutoSize = true;
            this.lbl_BagLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_BagLabel.Location = new System.Drawing.Point(47, 23);
            this.lbl_BagLabel.Name = "lbl_BagLabel";
            this.lbl_BagLabel.Size = new System.Drawing.Size(203, 24);
            this.lbl_BagLabel.TabIndex = 19;
            this.lbl_BagLabel.Text = "Enter Package Label";
            // 
            // btn_Abort
            // 
            this.btn_Abort.Location = new System.Drawing.Point(222, 150);
            this.btn_Abort.Name = "btn_Abort";
            this.btn_Abort.Size = new System.Drawing.Size(77, 37);
            this.btn_Abort.TabIndex = 18;
            this.btn_Abort.Text = "Abort";
            this.btn_Abort.UseVisualStyleBackColor = true;
            this.btn_Abort.Click += new System.EventHandler(this.btn_Abort_Click);
            // 
            // btn_Reset
            // 
            this.btn_Reset.Location = new System.Drawing.Point(115, 150);
            this.btn_Reset.Name = "btn_Reset";
            this.btn_Reset.Size = new System.Drawing.Size(77, 37);
            this.btn_Reset.TabIndex = 17;
            this.btn_Reset.Text = "Reset";
            this.btn_Reset.UseVisualStyleBackColor = true;
            this.btn_Reset.Click += new System.EventHandler(this.btn_Reset_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(16, 150);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(77, 37);
            this.btn_OK.TabIndex = 16;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // txt_MACLabel
            // 
            this.txt_MACLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_MACLabel.Location = new System.Drawing.Point(16, 109);
            this.txt_MACLabel.Name = "txt_MACLabel";
            this.txt_MACLabel.Size = new System.Drawing.Size(283, 29);
            this.txt_MACLabel.TabIndex = 20;
            this.txt_MACLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt_MACLabel.Visible = false;
            this.txt_MACLabel.TextChanged += new System.EventHandler(this.txt_MACLabel_TextChanged);
            this.txt_MACLabel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_ScanEntrys_KeyDown);
            // 
            // Productlabel1
            // 
            this.Productlabel1.AutoSize = true;
            this.Productlabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Productlabel1.Location = new System.Drawing.Point(47, 82);
            this.Productlabel1.Name = "Productlabel1";
            this.Productlabel1.Size = new System.Drawing.Size(195, 24);
            this.Productlabel1.TabIndex = 21;
            this.Productlabel1.Text = "Enter Product Label";
            this.Productlabel1.Visible = false;
            // 
            // frm_LabelInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 211);
            this.ControlBox = false;
            this.Controls.Add(this.txt_MACLabel);
            this.Controls.Add(this.Productlabel1);
            this.Controls.Add(this.txt_KitLabel);
            this.Controls.Add(this.lbl_BagLabel);
            this.Controls.Add(this.btn_Abort);
            this.Controls.Add(this.btn_Reset);
            this.Controls.Add(this.btn_OK);
            this.Name = "frm_LabelInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Label Info";
            this.Load += new System.EventHandler(this.frm_LabelInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox txt_KitLabel;
        internal System.Windows.Forms.Label lbl_BagLabel;
        internal System.Windows.Forms.Button btn_Abort;
        internal System.Windows.Forms.Button btn_Reset;
        internal System.Windows.Forms.Button btn_OK;
        internal System.Windows.Forms.TextBox txt_MACLabel;
        internal System.Windows.Forms.Label Productlabel1;
    }
}
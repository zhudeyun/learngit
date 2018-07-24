namespace F001716
{
    partial class frm_SoftwareInfo
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
            this.Tex_QAD_Line = new System.Windows.Forms.TextBox();
            this.lbl_QAD_Line = new System.Windows.Forms.Label();
            this.btn_Abort = new System.Windows.Forms.Button();
            this.btn_Reset = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.txt_Description = new System.Windows.Forms.TextBox();
            this.lbl_Description = new System.Windows.Forms.Label();
            this.txt_FlashImg = new System.Windows.Forms.TextBox();
            this.lbl_FlashImg = new System.Windows.Forms.Label();
            this.txt_BomRev = new System.Windows.Forms.TextBox();
            this.lbl_BomRev = new System.Windows.Forms.Label();
            this.txt_Item = new System.Windows.Forms.TextBox();
            this.lbl_ItemNumber = new System.Windows.Forms.Label();
            this.lbl_Model = new System.Windows.Forms.Label();
            this.txt_Model = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Tex_QAD_Line
            // 
            this.Tex_QAD_Line.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Tex_QAD_Line.Location = new System.Drawing.Point(37, 379);
            this.Tex_QAD_Line.MaxLength = 14;
            this.Tex_QAD_Line.Name = "Tex_QAD_Line";
            this.Tex_QAD_Line.Size = new System.Drawing.Size(238, 24);
            this.Tex_QAD_Line.TabIndex = 54;
            this.Tex_QAD_Line.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_ScanEntrys_KeyDown);
            // 
            // lbl_QAD_Line
            // 
            this.lbl_QAD_Line.AutoSize = true;
            this.lbl_QAD_Line.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_QAD_Line.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_QAD_Line.Location = new System.Drawing.Point(33, 357);
            this.lbl_QAD_Line.Name = "lbl_QAD_Line";
            this.lbl_QAD_Line.Size = new System.Drawing.Size(81, 19);
            this.lbl_QAD_Line.TabIndex = 53;
            this.lbl_QAD_Line.Text = "QAD Line";
            this.lbl_QAD_Line.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btn_Abort
            // 
            this.btn_Abort.Location = new System.Drawing.Point(310, 355);
            this.btn_Abort.Name = "btn_Abort";
            this.btn_Abort.Size = new System.Drawing.Size(77, 37);
            this.btn_Abort.TabIndex = 49;
            this.btn_Abort.Text = "Abort";
            this.btn_Abort.UseVisualStyleBackColor = true;
            this.btn_Abort.Click += new System.EventHandler(this.btn_Abort_Click);
            // 
            // btn_Reset
            // 
            this.btn_Reset.Location = new System.Drawing.Point(310, 312);
            this.btn_Reset.Name = "btn_Reset";
            this.btn_Reset.Size = new System.Drawing.Size(77, 37);
            this.btn_Reset.TabIndex = 48;
            this.btn_Reset.Text = "Reset";
            this.btn_Reset.UseVisualStyleBackColor = true;
            this.btn_Reset.Click += new System.EventHandler(this.btn_Reset_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(310, 269);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(77, 37);
            this.btn_OK.TabIndex = 47;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // txt_Description
            // 
            this.txt_Description.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Description.Location = new System.Drawing.Point(37, 160);
            this.txt_Description.Name = "txt_Description";
            this.txt_Description.Size = new System.Drawing.Size(238, 24);
            this.txt_Description.TabIndex = 46;
            this.txt_Description.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_ScanEntrys_KeyDown);
            // 
            // lbl_Description
            // 
            this.lbl_Description.AutoSize = true;
            this.lbl_Description.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_Description.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Description.Location = new System.Drawing.Point(33, 138);
            this.lbl_Description.Name = "lbl_Description";
            this.lbl_Description.Size = new System.Drawing.Size(97, 19);
            this.lbl_Description.TabIndex = 45;
            this.lbl_Description.Text = "Description";
            this.lbl_Description.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txt_FlashImg
            // 
            this.txt_FlashImg.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_FlashImg.Location = new System.Drawing.Point(37, 224);
            this.txt_FlashImg.Name = "txt_FlashImg";
            this.txt_FlashImg.Size = new System.Drawing.Size(238, 24);
            this.txt_FlashImg.TabIndex = 43;
            this.txt_FlashImg.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_ScanEntrys_KeyDown);
            // 
            // lbl_FlashImg
            // 
            this.lbl_FlashImg.AutoSize = true;
            this.lbl_FlashImg.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_FlashImg.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_FlashImg.Location = new System.Drawing.Point(29, 200);
            this.lbl_FlashImg.Name = "lbl_FlashImg";
            this.lbl_FlashImg.Size = new System.Drawing.Size(114, 19);
            this.lbl_FlashImg.TabIndex = 42;
            this.lbl_FlashImg.Text = "SUF Software";
            this.lbl_FlashImg.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txt_BomRev
            // 
            this.txt_BomRev.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_BomRev.Location = new System.Drawing.Point(37, 107);
            this.txt_BomRev.Name = "txt_BomRev";
            this.txt_BomRev.Size = new System.Drawing.Size(238, 24);
            this.txt_BomRev.TabIndex = 39;
            this.txt_BomRev.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_ScanEntrys_KeyDown);
            // 
            // lbl_BomRev
            // 
            this.lbl_BomRev.AutoSize = true;
            this.lbl_BomRev.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_BomRev.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_BomRev.Location = new System.Drawing.Point(33, 85);
            this.lbl_BomRev.Name = "lbl_BomRev";
            this.lbl_BomRev.Size = new System.Drawing.Size(80, 19);
            this.lbl_BomRev.TabIndex = 38;
            this.lbl_BomRev.Text = "BOM Rev";
            this.lbl_BomRev.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txt_Item
            // 
            this.txt_Item.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Item.Location = new System.Drawing.Point(37, 44);
            this.txt_Item.Name = "txt_Item";
            this.txt_Item.Size = new System.Drawing.Size(238, 24);
            this.txt_Item.TabIndex = 36;
            this.txt_Item.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_ScanEntrys_KeyDown);
            // 
            // lbl_ItemNumber
            // 
            this.lbl_ItemNumber.AutoSize = true;
            this.lbl_ItemNumber.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_ItemNumber.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ItemNumber.Location = new System.Drawing.Point(37, 20);
            this.lbl_ItemNumber.Name = "lbl_ItemNumber";
            this.lbl_ItemNumber.Size = new System.Drawing.Size(106, 19);
            this.lbl_ItemNumber.TabIndex = 37;
            this.lbl_ItemNumber.Text = "Item Number";
            this.lbl_ItemNumber.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lbl_Model
            // 
            this.lbl_Model.AutoSize = true;
            this.lbl_Model.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_Model.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_Model.Location = new System.Drawing.Point(37, 262);
            this.lbl_Model.Name = "lbl_Model";
            this.lbl_Model.Size = new System.Drawing.Size(55, 19);
            this.lbl_Model.TabIndex = 37;
            this.lbl_Model.Text = "Model";
            this.lbl_Model.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txt_Model
            // 
            this.txt_Model.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_Model.Location = new System.Drawing.Point(37, 286);
            this.txt_Model.Name = "txt_Model";
            this.txt_Model.Size = new System.Drawing.Size(238, 24);
            this.txt_Model.TabIndex = 36;
            this.txt_Model.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_ScanEntrys_KeyDown);
            // 
            // frm_SoftwareInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 424);
            this.ControlBox = false;
            this.Controls.Add(this.Tex_QAD_Line);
            this.Controls.Add(this.lbl_QAD_Line);
            this.Controls.Add(this.btn_Abort);
            this.Controls.Add(this.btn_Reset);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.txt_Description);
            this.Controls.Add(this.lbl_Description);
            this.Controls.Add(this.txt_FlashImg);
            this.Controls.Add(this.lbl_FlashImg);
            this.Controls.Add(this.txt_BomRev);
            this.Controls.Add(this.lbl_BomRev);
            this.Controls.Add(this.txt_Model);
            this.Controls.Add(this.lbl_Model);
            this.Controls.Add(this.txt_Item);
            this.Controls.Add(this.lbl_ItemNumber);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frm_SoftwareInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Label Info";
            this.Load += new System.EventHandler(this.frm_SoftwareInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox Tex_QAD_Line;
        internal System.Windows.Forms.Label lbl_QAD_Line;
        internal System.Windows.Forms.Button btn_Abort;
        internal System.Windows.Forms.Button btn_Reset;
        internal System.Windows.Forms.Button btn_OK;
        internal System.Windows.Forms.TextBox txt_Description;
        internal System.Windows.Forms.Label lbl_Description;
        internal System.Windows.Forms.TextBox txt_FlashImg;
        internal System.Windows.Forms.Label lbl_FlashImg;
        internal System.Windows.Forms.TextBox txt_BomRev;
        internal System.Windows.Forms.Label lbl_BomRev;
        internal System.Windows.Forms.TextBox txt_Item;
        internal System.Windows.Forms.Label lbl_ItemNumber;
        internal System.Windows.Forms.Label lbl_Model;
        internal System.Windows.Forms.TextBox txt_Model;
    }
}
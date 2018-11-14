namespace FenceKiller
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
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.ItemID = new System.Windows.Forms.TextBox();
			this.TraderYesNo = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// richTextBox1
			// 
			this.richTextBox1.Location = new System.Drawing.Point(12, 38);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(525, 304);
			this.richTextBox1.TabIndex = 0;
			this.richTextBox1.Text = "";
			this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
			// 
			// ItemID
			// 
			this.ItemID.Location = new System.Drawing.Point(12, 11);
			this.ItemID.Name = "ItemID";
			this.ItemID.Size = new System.Drawing.Size(167, 20);
			this.ItemID.TabIndex = 1;
			// 
			// TraderYesNo
			// 
			this.TraderYesNo.Location = new System.Drawing.Point(370, 11);
			this.TraderYesNo.Name = "TraderYesNo";
			this.TraderYesNo.PasswordChar = '+';
			this.TraderYesNo.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.TraderYesNo.Size = new System.Drawing.Size(167, 20);
			this.TraderYesNo.TabIndex = 2;
			this.TraderYesNo.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(191, 12);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(167, 20);
			this.button1.TabIndex = 3;
			this.button1.Text = "Buy that shit?";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(549, 354);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.TraderYesNo);
			this.Controls.Add(this.ItemID);
			this.Controls.Add(this.richTextBox1);
			this.Name = "Form1";
			this.Text = "Rest in peace, EFT";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.TextBox ItemID;
		private System.Windows.Forms.TextBox TraderYesNo;
		private System.Windows.Forms.Button button1;
	}
}


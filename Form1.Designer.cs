namespace Strooptest
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.Beendenbtn = new System.Windows.Forms.Button();
            this.tbCode2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Beendenbtn
            // 
            this.Beendenbtn.Location = new System.Drawing.Point(716, 505);
            this.Beendenbtn.Name = "Beendenbtn";
            this.Beendenbtn.Size = new System.Drawing.Size(207, 44);
            this.Beendenbtn.TabIndex = 0;
            this.Beendenbtn.Text = "Beenden";
            this.Beendenbtn.UseVisualStyleBackColor = true;
            this.Beendenbtn.Click += new System.EventHandler(this.Beendenbtn_Click);
            // 
            // tbCode2
            // 
            this.tbCode2.Location = new System.Drawing.Point(566, 190);
            this.tbCode2.Name = "tbCode2";
            this.tbCode2.Size = new System.Drawing.Size(100, 26);
            this.tbCode2.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 561);
            this.Controls.Add(this.tbCode2);
            this.Controls.Add(this.Beendenbtn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Beendenbtn;
        private System.Windows.Forms.TextBox tbCode2;
    }
}


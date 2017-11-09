namespace Proyecto2_compi2_2sem_2017.Editor
{
    partial class tipo_archivo
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
            this.btn_olc = new System.Windows.Forms.Button();
            this.btn_tree = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_olc
            // 
            this.btn_olc.Location = new System.Drawing.Point(12, 43);
            this.btn_olc.Name = "btn_olc";
            this.btn_olc.Size = new System.Drawing.Size(133, 62);
            this.btn_olc.TabIndex = 0;
            this.btn_olc.Text = "OLC";
            this.btn_olc.UseVisualStyleBackColor = true;
            this.btn_olc.Click += new System.EventHandler(this.btn_olc_Click);
            // 
            // btn_tree
            // 
            this.btn_tree.Location = new System.Drawing.Point(161, 43);
            this.btn_tree.Name = "btn_tree";
            this.btn_tree.Size = new System.Drawing.Size(129, 62);
            this.btn_tree.TabIndex = 1;
            this.btn_tree.Text = "TREE";
            this.btn_tree.UseVisualStyleBackColor = true;
            this.btn_tree.Click += new System.EventHandler(this.btn_tree_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "SELECCIONE EL TIPO DE ARCHIVO";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // tipo_archivo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 117);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_tree);
            this.Controls.Add(this.btn_olc);
            this.Name = "tipo_archivo";
            this.Text = "tipo_archivo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_olc;
        private System.Windows.Forms.Button btn_tree;
        private System.Windows.Forms.Label label1;
    }
}
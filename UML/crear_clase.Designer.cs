namespace Proyecto2_compi2_2sem_2017.UML
{
    partial class crear_clase
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
            this.txt_nombre_var = new System.Windows.Forms.TextBox();
            this.cbm_tipo_clase = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmb_visibilidad_clase = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tab_metodo = new System.Windows.Forms.TabPage();
            this.tab_var = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cmb_visiblidad_variable = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmb_tipo_variable = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.combo_variables = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txt_nombre_clase = new System.Windows.Forms.TextBox();
            this.clase = new System.Windows.Forms.Label();
            this.combo_metodos = new System.Windows.Forms.ComboBox();
            this.cmb_tipo_met = new System.Windows.Forms.ComboBox();
            this.cmb_visi_met = new System.Windows.Forms.ComboBox();
            this.cmb_parametros = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txt_nombre_metodo = new System.Windows.Forms.TextBox();
            this.txt_nom_param = new System.Windows.Forms.TextBox();
            this.txt_tipo_param = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.btn_crear_metodo = new System.Windows.Forms.Button();
            this.btn_crear_parametro = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tab_metodo.SuspendLayout();
            this.tab_var.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_nombre_var
            // 
            this.txt_nombre_var.Location = new System.Drawing.Point(154, 84);
            this.txt_nombre_var.Name = "txt_nombre_var";
            this.txt_nombre_var.Size = new System.Drawing.Size(127, 20);
            this.txt_nombre_var.TabIndex = 0;
            // 
            // cbm_tipo_clase
            // 
            this.cbm_tipo_clase.FormattingEnabled = true;
            this.cbm_tipo_clase.Items.AddRange(new object[] {
            "Tree",
            "OLC"});
            this.cbm_tipo_clase.Location = new System.Drawing.Point(96, 57);
            this.cbm_tipo_clase.Name = "cbm_tipo_clase";
            this.cbm_tipo_clase.Size = new System.Drawing.Size(127, 21);
            this.cbm_tipo_clase.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Tipo";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Visibilidad";
            // 
            // cmb_visibilidad_clase
            // 
            this.cmb_visibilidad_clase.FormattingEnabled = true;
            this.cmb_visibilidad_clase.Items.AddRange(new object[] {
            "Publico",
            "Privado",
            "Protegido"});
            this.cmb_visibilidad_clase.Location = new System.Drawing.Point(96, 86);
            this.cmb_visibilidad_clase.Name = "cmb_visibilidad_clase";
            this.cmb_visibilidad_clase.Size = new System.Drawing.Size(127, 21);
            this.cmb_visibilidad_clase.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(17, 126);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(206, 34);
            this.button1.TabIndex = 6;
            this.button1.Text = "CREAR";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tab_metodo);
            this.tabControl1.Controls.Add(this.tab_var);
            this.tabControl1.Location = new System.Drawing.Point(263, 31);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(370, 320);
            this.tabControl1.TabIndex = 7;
            // 
            // tab_metodo
            // 
            this.tab_metodo.Controls.Add(this.panel1);
            this.tab_metodo.Location = new System.Drawing.Point(4, 22);
            this.tab_metodo.Name = "tab_metodo";
            this.tab_metodo.Padding = new System.Windows.Forms.Padding(3);
            this.tab_metodo.Size = new System.Drawing.Size(362, 294);
            this.tab_metodo.TabIndex = 0;
            this.tab_metodo.Text = "Metodos";
            this.tab_metodo.UseVisualStyleBackColor = true;
            // 
            // tab_var
            // 
            this.tab_var.Controls.Add(this.panel2);
            this.tab_var.Location = new System.Drawing.Point(4, 22);
            this.tab_var.Name = "tab_var";
            this.tab_var.Padding = new System.Windows.Forms.Padding(3);
            this.tab_var.Size = new System.Drawing.Size(362, 294);
            this.tab_var.TabIndex = 1;
            this.tab_var.Text = "Variables";
            this.tab_var.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btn_crear_parametro);
            this.panel1.Controls.Add(this.btn_crear_metodo);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.txt_tipo_param);
            this.panel1.Controls.Add(this.txt_nom_param);
            this.panel1.Controls.Add(this.txt_nombre_metodo);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.cmb_parametros);
            this.panel1.Controls.Add(this.cmb_visi_met);
            this.panel1.Controls.Add(this.cmb_tipo_met);
            this.panel1.Controls.Add(this.combo_metodos);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(362, 294);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.combo_variables);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.cmb_visiblidad_variable);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.cmb_tipo_variable);
            this.panel2.Controls.Add(this.txt_nombre_var);
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(359, 291);
            this.panel2.TabIndex = 0;
            // 
            // cmb_visiblidad_variable
            // 
            this.cmb_visiblidad_variable.FormattingEnabled = true;
            this.cmb_visiblidad_variable.Items.AddRange(new object[] {
            "Publico",
            "Privado",
            "Protegido"});
            this.cmb_visiblidad_variable.Location = new System.Drawing.Point(154, 142);
            this.cmb_visiblidad_variable.Name = "cmb_visiblidad_variable";
            this.cmb_visiblidad_variable.Size = new System.Drawing.Size(127, 21);
            this.cmb_visiblidad_variable.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(63, 145);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Visibilidad";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(63, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(28, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Tipo";
            // 
            // cmb_tipo_variable
            // 
            this.cmb_tipo_variable.FormattingEnabled = true;
            this.cmb_tipo_variable.Items.AddRange(new object[] {
            "entero",
            "decimal",
            "cadena",
            "caracter",
            "booleano"});
            this.cmb_tipo_variable.Location = new System.Drawing.Point(154, 113);
            this.cmb_tipo_variable.Name = "cmb_tipo_variable";
            this.cmb_tipo_variable.Size = new System.Drawing.Size(127, 21);
            this.cmb_tipo_variable.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Nombre Variable";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(66, 183);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(215, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "CREAR VARIABLE";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // combo_variables
            // 
            this.combo_variables.FormattingEnabled = true;
            this.combo_variables.Location = new System.Drawing.Point(134, 30);
            this.combo_variables.Name = "combo_variables";
            this.combo_variables.Size = new System.Drawing.Size(158, 21);
            this.combo_variables.TabIndex = 12;
            this.combo_variables.SelectedIndexChanged += new System.EventHandler(this.combo_variables_SelectedIndexChanged);
            this.combo_variables.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.combo_variables_MouseDoubleClick);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.WhiteSmoke;
            this.label6.Location = new System.Drawing.Point(65, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Variables:";
            // 
            // txt_nombre_clase
            // 
            this.txt_nombre_clase.Location = new System.Drawing.Point(96, 31);
            this.txt_nombre_clase.Name = "txt_nombre_clase";
            this.txt_nombre_clase.Size = new System.Drawing.Size(127, 20);
            this.txt_nombre_clase.TabIndex = 11;
            // 
            // clase
            // 
            this.clase.AutoSize = true;
            this.clase.Location = new System.Drawing.Point(14, 34);
            this.clase.Name = "clase";
            this.clase.Size = new System.Drawing.Size(36, 13);
            this.clase.TabIndex = 12;
            this.clase.Text = "Clase:";
            // 
            // combo_metodos
            // 
            this.combo_metodos.FormattingEnabled = true;
            this.combo_metodos.Location = new System.Drawing.Point(97, 9);
            this.combo_metodos.Name = "combo_metodos";
            this.combo_metodos.Size = new System.Drawing.Size(210, 21);
            this.combo_metodos.TabIndex = 0;
            this.combo_metodos.SelectedIndexChanged += new System.EventHandler(this.combo_metodos_SelectedIndexChanged);
            // 
            // cmb_tipo_met
            // 
            this.cmb_tipo_met.FormattingEnabled = true;
            this.cmb_tipo_met.Items.AddRange(new object[] {
            "vacio",
            "cadena",
            "caracter",
            "decimal",
            "entero",
            "booleano"});
            this.cmb_tipo_met.Location = new System.Drawing.Point(146, 73);
            this.cmb_tipo_met.Name = "cmb_tipo_met";
            this.cmb_tipo_met.Size = new System.Drawing.Size(161, 21);
            this.cmb_tipo_met.TabIndex = 1;
            // 
            // cmb_visi_met
            // 
            this.cmb_visi_met.FormattingEnabled = true;
            this.cmb_visi_met.Items.AddRange(new object[] {
            "privado",
            "publico",
            "protegido"});
            this.cmb_visi_met.Location = new System.Drawing.Point(146, 100);
            this.cmb_visi_met.Name = "cmb_visi_met";
            this.cmb_visi_met.Size = new System.Drawing.Size(161, 21);
            this.cmb_visi_met.TabIndex = 2;
            // 
            // cmb_parametros
            // 
            this.cmb_parametros.FormattingEnabled = true;
            this.cmb_parametros.Location = new System.Drawing.Point(101, 169);
            this.cmb_parametros.Name = "cmb_parametros";
            this.cmb_parametros.Size = new System.Drawing.Size(206, 21);
            this.cmb_parametros.TabIndex = 3;
            this.cmb_parametros.SelectedIndexChanged += new System.EventHandler(this.cmb_parametros_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(29, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Metodos:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(29, 55);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Nombre";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(28, 76);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Tipo";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(29, 97);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 13);
            this.label10.TabIndex = 7;
            this.label10.Text = "Visibilidad";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(28, 177);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 13);
            this.label11.TabIndex = 8;
            this.label11.Text = "Parametros:";
            // 
            // txt_nombre_metodo
            // 
            this.txt_nombre_metodo.Location = new System.Drawing.Point(146, 47);
            this.txt_nombre_metodo.Name = "txt_nombre_metodo";
            this.txt_nombre_metodo.Size = new System.Drawing.Size(161, 20);
            this.txt_nombre_metodo.TabIndex = 9;
            // 
            // txt_nom_param
            // 
            this.txt_nom_param.Location = new System.Drawing.Point(146, 197);
            this.txt_nom_param.Name = "txt_nom_param";
            this.txt_nom_param.Size = new System.Drawing.Size(161, 20);
            this.txt_nom_param.TabIndex = 10;
            // 
            // txt_tipo_param
            // 
            this.txt_tipo_param.Location = new System.Drawing.Point(146, 224);
            this.txt_tipo_param.Name = "txt_tipo_param";
            this.txt_tipo_param.Size = new System.Drawing.Size(161, 20);
            this.txt_tipo_param.TabIndex = 11;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(32, 203);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(44, 13);
            this.label12.TabIndex = 12;
            this.label12.Text = "Nombre";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(36, 227);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(24, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "tipo";
            // 
            // btn_crear_metodo
            // 
            this.btn_crear_metodo.Location = new System.Drawing.Point(146, 128);
            this.btn_crear_metodo.Name = "btn_crear_metodo";
            this.btn_crear_metodo.Size = new System.Drawing.Size(161, 23);
            this.btn_crear_metodo.TabIndex = 14;
            this.btn_crear_metodo.Text = "crear_metodo";
            this.btn_crear_metodo.UseVisualStyleBackColor = true;
            this.btn_crear_metodo.Click += new System.EventHandler(this.btn_crear_metodo_Click);
            // 
            // btn_crear_parametro
            // 
            this.btn_crear_parametro.Location = new System.Drawing.Point(146, 250);
            this.btn_crear_parametro.Name = "btn_crear_parametro";
            this.btn_crear_parametro.Size = new System.Drawing.Size(161, 23);
            this.btn_crear_parametro.TabIndex = 15;
            this.btn_crear_parametro.Text = "crear_parametro";
            this.btn_crear_parametro.UseVisualStyleBackColor = true;
            this.btn_crear_parametro.Click += new System.EventHandler(this.btn_crear_parametro_Click);
            // 
            // crear_clase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(645, 383);
            this.Controls.Add(this.clase);
            this.Controls.Add(this.txt_nombre_clase);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmb_visibilidad_clase);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbm_tipo_clase);
            this.Name = "crear_clase";
            this.Text = "crear_clase";
            this.tabControl1.ResumeLayout(false);
            this.tab_metodo.ResumeLayout(false);
            this.tab_var.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_nombre_var;
        private System.Windows.Forms.ComboBox cbm_tipo_clase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmb_visibilidad_clase;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tab_metodo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage tab_var;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cmb_visiblidad_variable;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmb_tipo_variable;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox combo_variables;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_nombre_clase;
        private System.Windows.Forms.Label clase;
        private System.Windows.Forms.Button btn_crear_parametro;
        private System.Windows.Forms.Button btn_crear_metodo;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txt_tipo_param;
        private System.Windows.Forms.TextBox txt_nom_param;
        private System.Windows.Forms.TextBox txt_nombre_metodo;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmb_parametros;
        private System.Windows.Forms.ComboBox cmb_visi_met;
        private System.Windows.Forms.ComboBox cmb_tipo_met;
        private System.Windows.Forms.ComboBox combo_metodos;
    }
}
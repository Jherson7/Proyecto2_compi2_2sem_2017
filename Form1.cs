﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Proyecto2_compi2_2sem_2017.Editor;
using Proyecto2_compi2_2sem_2017.Compilador;
using Proyecto2_compi2_2sem_2017.Control3D;
using Proyecto2_compi2_2sem_2017.Ejecucion3D;
using System.IO;
using Proyecto2_compi2_2sem_2017.UML;
using Proyecto2_compi2_2sem_2017.Editor;

namespace Proyecto2_compi2_2sem_2017
{
    public partial class Form1 : Form
    {

        public static  int cont_tree=0;
        public static int cont_olc= 0;
        private LinkedList<string> rutas_proyectos;


        public Form1()
        {
            InitializeComponent();

            this.rutas_proyectos = new LinkedList<string>();
            this.rutas_proyectos.AddFirst("C:\\Users\\Jherson Sazo\\Documents\\COMPI2\\Archivos Proyecto2 2sem 2017");

            //configurar_salida3d();
            
        }

        private void configurar_salida3d()
        {
            control_salida.TabPages.Clear();
            for (int i =0; i< 5; i++)
            {
                pagina pag = new pagina(3);
                control_salida.TabPages.Add(pag);
                switch (i)
                {
                    case 0:
                        pag.Text = "CONSOLA";
                            break;
                    case 1:
                        pag.Text = "CONSOLA";
                        break;
                    case 2:
                        pag.Text = "CONSOLA";
                        break;
                    case 3:
                        pag.Text = "CONSOLA";
                        break;
                    case 4:
                        pag.Text = "CONSOLA";
                        break;
                }
            }
            
            
        }

        private void btnCompilar_Click(object sender, EventArgs e)
        {

            if (tabControl1.SelectedIndex > 0) {
                Control3d.iniciar_controlador();
                pagina nuevo = (pagina)tabControl1.TabPages[tabControl1.SelectedIndex];
                string contenido = nuevo.contenido.Text;
                Interprete inter = new Interprete();

                if (nuevo.tipo.Equals("olc"))
                    inter.analizar(contenido);
                else
                    inter.analizar_TREE(contenido);

                RichTextBox aux = (RichTextBox)control_salida.TabPages[1].Controls[0];
                aux.Text = Convert.ToString(inter.errores);
                foreach (errores err in Control3d.getErrores())
                {
                    aux.AppendText(err.tipo + "  |  " + err.descripcion + "  |  " + err.linea + "  |  " + err.columna + "\n");
                }
                ironyFCTB1.SetParser(new Gramatica3d());
                ironyFCTB1.Text = Control3d.retornarC3D().ToString();
                
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            string tipo =tipo_archivo.getType();
            

            pagina pag = new pagina(tipo);
            tabControl1.TabPages.Add(pag);
            tabControl1.SelectTab(pag);

        }

        private void btnEjecutar3d_Click(object sender, EventArgs e)
        {
            Interprete3d inter = new Interprete3d();
            string cont = ironyFCTB1.Text;
            
            inter.analizar(cont);

            RichTextBox salida = (RichTextBox)control_salida.TabPages[0].Controls[0];
            salida.Text = inter.salida.ToString();
            RichTextBox aux = (RichTextBox)control_salida.TabPages[1].Controls[0];
            foreach (errores err in Control3d.getErrores())
            {
                aux.AppendText(err.tipo + "  |  " + err.descripcion + "  |  " + err.linea + "  |  " + err.columna + "\n");
            }

        }

        private void btnAbrir_Click(object sender, EventArgs e)
        {
            OpenFileDialog abrir = new OpenFileDialog();
            abrir.Title = "Abrir archivos de texto";
            abrir.Filter = "Tipo de Archivo (*.olc, *.tree) | *.olc; *.tree";
            abrir.ShowDialog();

            if (abrir.FileName.Length == 0)
                return;

            System.IO.StreamReader sr = new System.IO.StreamReader(abrir.FileName, System.Text.Encoding.UTF8);
            String contenido = sr.ReadToEnd();
            Console.WriteLine(contenido);
            sr.Close();

            string tipo = "olc";

            if (abrir.FileName.EndsWith("tree"))
                tipo = "tree";
            int pos = abrir.FileName.LastIndexOf("\\")+1;
            string nombre = abrir.FileName.Substring(abrir.FileName.LastIndexOf("\\", abrir.FileName.Length)).Replace("\\","");

            foreach (TabPage item in tabControl1.TabPages)
            {
                if (item.Text.Equals(nombre)) {
                    tabControl1.SelectTab(item);
                    return;
                }
            }

            pagina pag = new pagina(tipo, contenido, nombre,abrir.FileName);
            
            tabControl1.TabPages.Add(pag);
            tabControl1.SelectTab(pag);

            int ini = abrir.FileName.LastIndexOf("\\");
            string ruta = abrir.FileName.Substring(0, ini+1);

            Control3d.set_ruta(ruta);

            tree_view.Nodes.Add(new TreeNode(nombre));
            tree_view.Refresh();
        }
        //"C:\\Users\\Jherson Sazo\\Documents\\COMPI2\\Archivos Proyecto2 2sem 2017\\archivo1.olc"

        private int fibonacci(int n)
        {
            if (n <= 1)
                return 1;
            else
                return fibonacci(n - 1) + fibonacci(n - 2);
        }

        private void btnCrear_carpeta_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string ruta = folderBrowserDialog1.SelectedPath;

                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                foreach (var item in rutas_proyectos)
                    if (item.Equals(ruta))
                        return;
                rutas_proyectos.AddLast(ruta);
                cargar_tree_view();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex > 0)
            {
                pagina nuevo = (pagina)tabControl1.TabPages[tabControl1.SelectedIndex];
                string contenido = nuevo.contenido.Text;

                if (nuevo.ruta.Equals(""))
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    if(nuevo.tipo.Equals("olc"))
                        saveFileDialog.Filter = "Archivos OLC++,TREE (*.olc)|*.olc";
                    else
                        saveFileDialog.Filter = "Archivos OLC++,TREE (*.tree)|*.tree";
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string ruta_file = saveFileDialog.FileName;
                        string nombre = ruta_file.Substring(ruta_file.LastIndexOf("\\", ruta_file.Length)).Replace("\\", "");
                        nuevo.ruta = ruta_file;
                        nuevo.Text = nombre;
                    }
                    else
                        return;
                }

                System.IO.File.WriteAllText(@""+nuevo.ruta, contenido);
                cargar_tree_view();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cargar_tree_view();

        }

        private void cargar_tree_view()
        {
            tree_view.Nodes.Clear();
            foreach (string a in rutas_proyectos)
            {
                DirectoryInfo dir = new DirectoryInfo(a);

                tree_view.Nodes.Add(retornar_arbol(dir));

            }
        }

        private TreeNode retornar_arbol(DirectoryInfo dir)
        {
            TreeNode arbol = new TreeNode(dir.FullName);

            foreach (var item in dir.GetDirectories())
            {
                arbol.Nodes.Add(retornar_arbol(item));
            }

            foreach(var item in dir.GetFiles())
            {
                if(item.Name.EndsWith("olc")|| item.Name.EndsWith("tree"))
                    arbol.Nodes.Add(new TreeNode(item.Name));
            }

            return arbol;
        }

        private void tree_view_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void tree_view_DoubleClick(object sender, EventArgs e)
        {
            TreeNode aux = tree_view.SelectedNode;
            if (aux != null)
            {
                string ruta = aux.FullPath;
                if (ruta.EndsWith("olc") || ruta.EndsWith("tree"))
                {
                    ruta = ruta.Replace("\\\\", "\\");

                    
                    string nombre = ruta.Substring(ruta.LastIndexOf("\\", ruta.Length)).Replace("\\", "");

                    foreach (TabPage item in tabControl1.TabPages)
                    {
                        if (item.Text.Equals(nombre))
                        {
                            tabControl1.SelectTab(item);
                            return;
                        }
                    }


                    System.IO.StreamReader sr = new System.IO.StreamReader(ruta, System.Text.Encoding.UTF8);
                    String contenido = sr.ReadToEnd();
                    Console.WriteLine(contenido);
                    sr.Close();

                    string tipo = "olc";
                    if (ruta.EndsWith("tree"))
                        tipo = "tree";

                    pagina pag = new pagina(tipo, contenido, nombre, ruta);

                    tabControl1.TabPages.Add(pag);
                    tabControl1.SelectTab(pag);
                }
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex > 0)
            {
                tabControl1.TabPages.RemoveAt(tabControl1.SelectedIndex);
                tabControl1.SelectTab(tabControl1.TabPages.Count - 1);
            }
        }

        private void aToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uml_principal um = new uml_principal();
            um.ShowDialog();
        }

        private void realizarOptimizacionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
            string contenido = control_salida.TabPages[3].Controls[0].Text;
            Optimizador3D nuevo = new Optimizador3D();
            nuevo.analizar(contenido);
            string optimizado = nuevo.salida.ToString();
            RichTextBox salida = (RichTextBox)control_salida.TabPages[4].Controls[0];
            salida.Text = optimizado;
            string secuencia = nuevo.reporte_optimizacion.ToString();
            salida = (RichTextBox)control_salida.TabPages[2].Controls[0];
            salida.Text = secuencia;

        }

        private void cOMPILARTREEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            login LOG = new login();
            LOG.ShowDialog();
        }

        private void arbolOlcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //VAMO A INSERTAR UNA NUEVA CLASE
            if (master.get_usuario() != null)
            {
                if (tabControl1.SelectedIndex > 0)
                {
                    string nombre = "";
                    string ruta_file = "";
                    pagina nuevo = (pagina)tabControl1.TabPages[tabControl1.SelectedIndex];
                    string contenido = nuevo.contenido.Text;

                    if (nuevo.ruta.Equals(""))
                    {
                        SaveFileDialog saveFileDialog = new SaveFileDialog();
                        if (nuevo.tipo.Equals("olc"))
                            saveFileDialog.Filter = "Archivos OLC++,TREE (*.olc)|*.olc";
                        else
                            saveFileDialog.Filter = "Archivos OLC++,TREE (*.tree)|*.tree";
                        saveFileDialog.RestoreDirectory = true;

                        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            ruta_file = saveFileDialog.FileName;
                            nombre = ruta_file.Substring(ruta_file.LastIndexOf("\\", ruta_file.Length)).Replace("\\", "");
                            nuevo.ruta = ruta_file;
                            nuevo.Text = nombre;
                        }
                        else
                        {
                            MessageBox.Show("Debe guardar primero la clase");
                            return;
                        }

                    }
                    else
                    {
                        int pos = nuevo.ruta.LastIndexOf('\\')+1;
                        int fin = nuevo.ruta.Length ;
                        nombre = nuevo.ruta.Substring(pos, fin - pos);

                    }
                        
                    string des = Descripcion.get_des();
                    string fecha = DateTime.Today.Year + "-" + DateTime.Today.Month + "-" + DateTime.Today.Day;
                    master.getControlador().insertar_clase(nombre, master.get_usuario().id, nuevo.ruta, des, fecha,contenido);
                    System.IO.File.WriteAllText(@"" + nuevo.ruta, contenido);

                }  
            }

        }

        private void cERRARSESIONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (master.get_usuario() != null)
            {
                master.set_usuario(null);
            }
            else
                MessageBox.Show("No se ha iniciado sesion");
        }

        private void tablaSimbolosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graficador g = new Graficador();
            g.abrirArbol(@"C:\compiladores\tablaSym.html");
        }
    }
}

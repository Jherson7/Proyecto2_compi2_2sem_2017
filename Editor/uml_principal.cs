using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Proyecto2_compi2_2sem_2017.UML;
using System.Threading;
using Proyecto2_compi2_2sem_2017.Compilador;

using Irony.Parsing;

namespace Proyecto2_compi2_2sem_2017.Editor
{
    public partial class uml_principal : Form
    {

        public static LinkedList<clase_uml> lista_clases;
        public static LinkedList<relacion> lista_relaciones;

        public LinkedList<clase_uml> clases_a_traducir;

        public static int contador =0;
        
        public uml_principal()
        {
            InitializeComponent();
            lista_clases = new LinkedList<clase_uml>();
            lista_relaciones = new LinkedList<relacion>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            crear_clase clase = new crear_clase();
            clase.ShowDialog();
            Thread.Sleep(1000);
            this.pictureBox1.Image = new System.Drawing.Bitmap(@"C:\compiladores\imagenes_uml\diagrama.png");
            //this.pictureBox1.Image = new System.Drawing.Bitmap(@"C:\compiladores\imagenes_uml\diagrama.png");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //agregacion 1 
            crear_relacion rel = new crear_relacion(1,lista_relaciones,lista_clases);
            rel.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //composicion 2
            crear_relacion rel = new crear_relacion(2, lista_relaciones, lista_clases);
            rel.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //dependencia 3
            crear_relacion rel = new crear_relacion(3, lista_relaciones, lista_clases);
            rel.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //herencia 4
            crear_relacion rel = new crear_relacion(4, lista_relaciones, lista_clases);
            rel.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //asociasion 5

            //solo se crean objetos de la clase referenciada
            crear_relacion rel = new crear_relacion(5, lista_relaciones, lista_clases);
            rel.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if(cmb_clase.SelectedItem==null)
            {
                MessageBox.Show("Seleccione tipo de salida");
                return;
            }
            string clase = cmb_clase.SelectedItem.ToString();
            if (clase.Equals("Tree"))
                traducir_clase_tree();
            else
                traducir_clase_olc();
        }

        private void traducir_clase_olc()
        {
            LinkedList<clase_uml> lista = uml_principal.lista_clases;
            StringBuilder clases = new StringBuilder();
            
            foreach(clase_uml clase in lista)
            {
                StringBuilder cad = new StringBuilder();

                //escribimos el nombre de la clase
                cad.Append("publico clase " + clase.nombre + " ");
                //buscar de que clase hereda
                string hereda = clase_heredada(clase.nombre);
                if (hereda != "")
                    cad.Append("hereda_de " + hereda);
                cad.Append(" {\n");
                //retonar las clases de que tienen que ver con variables

                foreach (variable_uml var in clase.variables)
                    cad.Append("\t\t"+var.visiblidad + " " + var.tipo + " " + var.nombre + "; \n");

                foreach (string a in get_asociaciones(clase.nombre))
                    cad.Append("\t\t"+"publico " + a + " objeto_" + a + "; \n");

                LinkedList<string> agregaciones = new LinkedList<string>();
                LinkedList<string> composicion  = new LinkedList<string>();
                LinkedList<string> dependencias = new LinkedList<string>();
                //todas las variables que tiene que ver con metodos
                llenar_agregaciones(clase.nombre,agregaciones);
                llenar_composiciones(clase.nombre,composicion);
                llenar_dependencias(clase.nombre, dependencias);

                foreach(var item in agregaciones)//lleno las variables de las agregaciones
                    cad.Append("\t\tpublico " + item + " objeto_" + item + "; \n");

                foreach (var item in composicion)//lleno las variables de las composiciones
                    cad.Append("\t\tpublico " + item + " objeto_" + item + "; \n");


                
                foreach (var item in agregaciones)//creo los metodos de las agregaciones
                {
                    cad.Append("publico vacio " +clase.nombre+"_metodo_"+ item + "("+item+ "nuevo_objeto_" + item + "){ \n");
                    cad.Append("\t\tthis.objeto_" + item + "  = nuevo_objeto_" + item + ";\n}");
                } 

                foreach (var item in composicion)//creo los metodos de la composicion
                {
                    cad.Append("publico " + clase.nombre + "(" + item + "nuevo_objeto_" + item + "){ \n");
                    cad.Append("\t\tthis.objeto_" + item + "  = nuevo_objeto_" + item + ";\n}");
                }

                foreach (var item in dependencias)//creo los metodos de la composicion
                {
                    cad.Append("publico " + clase.nombre + "_dependencia_"+item+"(" + item + "nuevo_objeto" + item + "){ \n }");
                }

                foreach(var item in clase.metodos)
                {
                    cad.Append("\n\n" + item.visibilidad + "  " + item.tipo + "  " + item.nombre + " (");
                    bool a = false;
                    foreach (var par in item.parametros) {
                        if(a)
                            cad.Append(","+par.tipo + "  " + par.nombre);
                        else
                            cad.Append(par.tipo + "  " + par.nombre);
                        a = true;
                    }
                    cad.Append(") {\n}");
                        
                }


                cad.Append("\n\n}\n\n");
                clases.Append(cad+"\n");
            }

            //this.Text = new FastColoredTextBoxNS.IronyFCTB();
            this.txt_salida.SetParser(new Gramatica());
            this.txt_salida.Text = clases.ToString();

        }

        private string clase_heredada(string nombre)
        {
            LinkedList<relacion> lista = uml_principal.lista_relaciones;
            foreach (var item in lista)
            {
                if (item.origen.Equals(nombre) && item.tipo_rel == 4)
                    return item.destino;
            }
            return "";
        }

        private LinkedList<string> get_asociaciones(string nombre)
        {
            LinkedList<relacion> lista = uml_principal.lista_relaciones;
            LinkedList<string> asociaciones = new LinkedList<string>();
            foreach (var item in lista)
            {
                if (item.origen.Equals(nombre) && item.tipo_rel == 5)
                    asociaciones.AddLast(item.destino);
            }
            return asociaciones;
        }

        private void llenar_agregaciones(string nombre, LinkedList<string> lista)
        {
            LinkedList<relacion> relaciones = uml_principal.lista_relaciones;
            foreach (var item in relaciones)
            {
                if (item.origen.Equals(nombre) && item.tipo_rel == 1)
                    lista.AddLast(item.destino);
            }
        }


        private void llenar_composiciones(string nombre, LinkedList<string> lista)
        {
            LinkedList<relacion> relaciones = uml_principal.lista_relaciones;
            foreach (var item in relaciones)
            {
                if (item.origen.Equals(nombre) && item.tipo_rel == 2)
                    lista.AddLast(item.destino);
            }
        }

        private void llenar_dependencias(string nombre, LinkedList<string> lista)
        {
            LinkedList<relacion> relaciones = uml_principal.lista_relaciones;
            foreach (var item in relaciones)
            {
                if (item.origen.Equals(nombre) && item.tipo_rel == 3)
                    lista.AddLast(item.destino);
            }
        }

        private void traducir_clase_tree()
        {
            LinkedList<clase_uml> lista = uml_principal.lista_clases;
            StringBuilder clases = new StringBuilder();

            foreach (clase_uml clase in lista)
            {
                StringBuilder cad = new StringBuilder();

                //escribimos el nombre de la clase
                cad.Append("clase " +clase.nombre + " [");
                //buscar de que clase hereda
                string hereda = clase_heredada(clase.nombre);
                if (hereda != "")
                    cad.Append(hereda);
                cad.Append(" ]:\n\t\n");
                //retonar las clases de que tienen que ver con variables


                LinkedList<string> agregaciones = new LinkedList<string>();
                LinkedList<string> composicion = new LinkedList<string>();
                LinkedList<string> dependencias = new LinkedList<string>();
                //todas las variables que tiene que ver con metodos
                llenar_agregaciones(clase.nombre, agregaciones);
                llenar_composiciones(clase.nombre, composicion);
                llenar_dependencias(clase.nombre, dependencias);


                foreach (variable_uml var in clase.variables)
                    cad.Append("\t" + var.visiblidad + " " + var.tipo + " " + var.nombre + "\n");

                foreach (string a in get_asociaciones(clase.nombre))
                    cad.Append("\t" + "publico " + a + " objeto_" + a + "\n");

                foreach (var item in agregaciones)//lleno las variables de las agregaciones
                    cad.Append("\tpublico " + item + " objeto_" + item + "\n");

                foreach (var item in composicion)//lleno las variables de las composiciones
                    cad.Append("\tpublico " + item + " objeto_" + item + "\n");


                foreach (var item in agregaciones)//creo los metodos de las agregaciones
                {
                    cad.Append("\tpublico vacio " + clase.nombre + "_metodo_" + item + "[" + item + "  nuevo_objeto_" + item + "]: \n\t\n");
                    cad.Append("\\ttself.objeto_" + item + "  = nuevo_objeto_" + item + "\n");
                }

                foreach (var item in composicion)//creo los metodos de la composicion
                {
                    cad.Append("\tpublico " + clase.nombre + " [" + item + "  nuevo_objeto_" + item + "]:\n\t\n");
                    cad.Append("\t\tself.objeto_" + item + "  = nuevo_objeto_" + item);
                }

                foreach (var item in dependencias)//creo los metodos de la composicion
                {
                    cad.Append("\tpublico  " + clase.nombre + "_dependencia_" + item + " [" + item + "  nuevo_objeto" + item + "]: \n\t\n");
                }

                foreach (var item in clase.metodos)
                {
                    cad.Append("\n\t" + item.visibilidad + "  " + item.tipo + "  " + item.nombre + " [");
                    bool a = false;
                    foreach (var par in item.parametros)
                    {
                        if (a)
                            cad.Append("," + par.tipo + "  " + par.nombre);
                        else
                            cad.Append(par.tipo + "  " + par.nombre);
                        a = true;
                    }
                    cad.Append("]:\n\t\n");

                }


                cad.Append("\n\n");
                clases.Append(cad + "\n");
            }

            this.txt_salida.SetParser(new GramaticaTre());
            this.txt_salida.Text = clases.ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string tipo = cmb_clase.SelectedItem.ToString();
            clases_a_traducir = new LinkedList<clase_uml>();
            if (txt_salida.Text != "")
            {
                if (tipo.ToLower().Equals("tree"))
                    traucir_a_tree(txt_salida.Text);
                else
                    traducir_a_olc(txt_salida.Text);
            }

            generar_relaciones();
            generar_dot_clases();
        }

        private void generar_relaciones()
        {
            lista_relaciones = new LinkedList<relacion>();

            foreach (var item in clases_a_traducir)
            {
                ///1 vamos a buscar agregacion

                foreach (var variable in item.variables)
                {
                    string tipo = variable.tipo;
                    if(tipo!="cadena"&&tipo!="entero" && tipo != "caracter" && tipo != "booleano" && tipo != "decimal")
                    {
                        if (retornar_agregacion(tipo, item.metodos))
                        {
                            variable.estado = true;
                            lista_relaciones.AddLast(new relacion(item.nombre, 1, tipo));
                        }
                    }
                }
                ///2 vamos a buscar compocicion
                foreach (var variable in item.variables)
                {
                    if (variable.estado)
                        continue;
                    string tipo = variable.tipo;

                    if (tipo != "cadena" && tipo != "entero" && tipo != "caracter" && tipo != "booleano" && tipo != "decimal")
                    {
                        if (retornar_agregacion(tipo, item.constructores))
                        {
                            lista_relaciones.AddLast(new relacion(item.nombre, 2, tipo));
                        }
                    }
                }
                ///3 vamos a buscar asociacion
                foreach (var variable in item.variables)
                {
                    if (variable.estado)
                        continue;
                    string tipo = variable.tipo;

                    if (tipo != "cadena" && tipo != "entero" && tipo != "caracter" && tipo != "booleano" && tipo != "decimal")
                    {
                        if (!retornar_agregacion(tipo, item.constructores)&& !retornar_agregacion(tipo, item.metodos))
                        {
                            lista_relaciones.AddLast(new relacion(item.nombre, 3, tipo));
                            
                        }
                    }
                }

                ///4 herencia
                if (!item.hereda.Equals("error"))
                {
                    lista_relaciones.AddLast(new relacion(item.nombre, 4, item.hereda));
                }
            }
        }

        bool retornar_agregacion(string tipo, LinkedList<metodo_uml> metodos)
        {

            foreach(var item in metodos)
            {
                foreach(var param in item.parametros)
                {
                    if (param.tipo.Equals(tipo))
                        return true;
                }
            }
           

            return false;
        }


        private void traducir_a_olc(string text)
        {
            Gramatica gramatica = new Gramatica();
            Parser parser = new Parser(gramatica);

            ParseTree arbol = parser.Parse(text);
            ParseTreeNode raiz = arbol.Root;

            if (raiz == null || arbol.ParserMessages.Count > 0 || arbol.HasErrors())
            {
                foreach (var item in arbol.ParserMessages)
                {
                    MessageBox.Show(item.Message + "," + item.Location.Line + " " + item.Location.Column);
                }
                return;
            }

            analizar_arbol(raiz);
        }

        private void traucir_a_tree(string text)
        {
            GramaticaTre gramatica = new GramaticaTre();
            Parser parser = new Parser(gramatica);

            ParseTree arbol = parser.Parse(text);
            ParseTreeNode raiz = arbol.Root;

            if (raiz == null || arbol.ParserMessages.Count > 0 || arbol.HasErrors())
            {
                foreach (var item in arbol.ParserMessages)
                {
                    MessageBox.Show(item.Message + "," + item.Location.Line + " " + item.Location.Column);
                }
                return;
            }

            analizar_arbol(raiz);
            Console.Write("j");


        }

        private void analizar_arbol(ParseTreeNode raiz)
        {
            
            switch (raiz.Term.Name)
            {
                case "INICIO":
                    foreach (ParseTreeNode a in raiz.ChildNodes)
                        analizar_arbol(a);
                    break;
                case "SENTENCIAS":
                    foreach (ParseTreeNode a in raiz.ChildNodes)
                        analizar_arbol(a);
                    break;
                case "CLASE":
                    guardarClase(raiz);
                    break;
                case "STRUCT":
                    guardar_clase_tree(raiz);
                    break;
            
        }
    }

        private void guardar_clase_tree(ParseTreeNode raiz)
        {
            string nombre = raiz.ChildNodes[0].Token.Text;

            clase_uml nuevo;
               
            if (raiz.ChildNodes[1].ChildNodes.Count > 0)
            {
                nuevo = new clase_uml(nombre, raiz.ChildNodes[1].ChildNodes[0].Token.Text);
            }else
            {
                nuevo = new clase_uml(nombre);
            }
                
           foreach (ParseTreeNode a in raiz.ChildNodes[2].ChildNodes)
            {
                sentencias_clase(a, nombre,nuevo);
            }


            clases_a_traducir.AddLast(nuevo);
        }

        private void sentencias_clase(ParseTreeNode raiz, string nombre_clase,clase_uml actual)
        {

            ///parametro 0 visibilidad
            ///parametro 1 tipo de variable, metodo, arreglo, instancia etc.
            ///parametro 2 iden
            ///parametro 3 head
            string visibilidad = "publico";
            visibilidad = raiz.ChildNodes[0].ChildNodes[0].Token.Text;
            string nombre = "";
            string tipo = "";

            if (raiz.ChildNodes[0].Term.Name.Equals("OVERRIDE"))//es de tipo tree
            {
                return;
            }

            if (raiz.ChildNodes[0].Term.Name.Equals("CONSTRUCTOR"))//es de tipo tree
            {
                guardarConstructor(visibilidad, raiz.ChildNodes[0], nombre_clase,actual);
                return;
            }
            if (raiz.ChildNodes[1].Term.Name.Equals("CONSTRUCTOR"))
            {
                guardarConstructor(visibilidad, raiz.ChildNodes[1], nombre_clase,actual);
                return;
            }
            if (raiz.ChildNodes[1].ChildNodes[0].ChildNodes.Count > 0) //pregunto si eno es de tipo void
                tipo = raiz.ChildNodes[1].ChildNodes[0].ChildNodes[0].Token.Text;
            else
                tipo = "vacio";//me falta probar esto

            if (raiz.ChildNodes.Count > 2)
                nombre = raiz.ChildNodes[2].Token.Text;

            if (raiz.ChildNodes[3].ChildNodes.Count > 0)//si no entonces es de tipo variable
            {
                ParseTreeNode aux = raiz.ChildNodes[3].ChildNodes[0];
                if (aux.Term.Name.Equals("METODO"))
                {
                    guardar_metodo(tipo, nombre, visibilidad, aux,actual);
                }
                else if (aux.Term.Name.Equals("L_ARRAY"))
                {
                    guardarVariable(visibilidad, tipo, nombre, actual);
                }
                else if (aux.Term.Name.Equals("new"))
                {
                    guardarInstancia(visibilidad, tipo, nombre,actual);
                }
                else//seria una declaracion asignacion
                {
                    guardarVariable(visibilidad, tipo, nombre,actual);
                }
            }
            else
            {
                guardarVariable(visibilidad, tipo, nombre,actual);
            }
        }

        private void guardarVariable(string visibilidad, string tipo, string nombre,clase_uml actual)
        {
            actual.variables.AddLast(new variable_uml( nombre, tipo, visibilidad));
        }

        private void guardarInstancia(string visibilidad, string tipo, string nombre,clase_uml actual)
        {
            guardarVariable(visibilidad, tipo, nombre, actual);
        }

        
        private void guardar_metodo(string tipo, string nombre, string visibilidad, ParseTreeNode aux, clase_uml actual)
        {
            metodo_uml nuevo = new metodo_uml(nombre, tipo, visibilidad);
            ParseTreeNode parametros = aux.ChildNodes[0];
            ParseTreeNode sentencias = aux.ChildNodes[1];
            foreach(ParseTreeNode para in parametros.ChildNodes)
            {
                string name = para.ChildNodes[1].Token.Text;
                string type = para.ChildNodes[0].ChildNodes[0].Token.Text;
                parametros par = new UML.parametros("", "");
            }

            actual.metodos.AddLast(nuevo);

        }

        

        private void guardarConstructor(string visibilidad, ParseTreeNode raiz, string clase,clase_uml actual)
        {
         
            if (raiz.ChildNodes.Count == 3)
                guardar_constructor_parametros(visibilidad, "CONSTRUCTOR", clase, raiz.ChildNodes[1], raiz.ChildNodes[2],actual);
            else
            {
                metodo_uml nuevo = new metodo_uml(actual.nombre, "", visibilidad);
                actual.metodos.AddLast(nuevo);
                //traducir sus sentencias
            }
            //comparar de la misma manera que con los metodos
            //solo que con un par de cambios como el tipo y solo ver los numeros de parametros
            //:D
        }

        private void guardar_constructor_parametros(string visibilidad, string tipo, string nombre, ParseTreeNode parametros, ParseTreeNode sentencias,clase_uml actual)
        {
            actual.constructores.AddLast(new metodo_uml(visibilidad, "", actual.nombre));
            //verificar las sentencias
        }


        private void guardarClase(ParseTreeNode raiz)
        {
            throw new NotImplementedException();
        }

        private void cmb_clase_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tipo = cmb_clase.SelectedItem.ToString();
            if (tipo.ToLower().Equals("tree"))
                this.txt_salida.SetParser(new GramaticaTre());
            else
                this.txt_salida.SetParser(new Gramatica());
        }



        private void generar_dot_clases()
        {
            StringBuilder dot = new StringBuilder();


            dot.Append("digraph{  node[shape = record color = \"blue\"];\n");
            foreach (clase_uml a in clases_a_traducir)
            {
                dot.Append(a.nombre + "[ label= \"{" + a.nombre + "|ATRIBUTOS| ");
                foreach (variable_uml var in a.variables)
                {
                    string signo = "(+)";
                    if (var.visiblidad.Equals("privado"))
                        signo = "(-)";
                    else if (var.visiblidad.Equals("protegido"))
                        signo = "(#)";
                    dot.Append(signo + " " + var.nombre + ": " + var.tipo + "\\n");
                }

                dot.Append("|METODOS|");
                foreach (metodo_uml met in a.metodos)
                {
                    string signo = "(+)";
                    if (met.visibilidad.Equals("privado"))
                        signo = "(-)";
                    else if (met.visibilidad.Equals("protegido"))
                        signo = "(#)";
                    dot.Append(signo + " " + met.nombre + "():  " + met.tipo + "\\n");
                }

                dot.Append("}\"];\n");

            }


            LinkedList<relacion> relaciones = uml_principal.lista_relaciones;


            foreach (relacion r in relaciones)
            {
                switch (r.tipo_rel)
                {
                    case 1:
                        dot.Append(r.origen + "->" + r.destino + " [arrowhead = odiamond];\n");
                        break;
                    case 2:
                        dot.Append(r.origen + "->" + r.destino + "  ;\n");
                        break;
                    case 3:
                        dot.Append(r.origen + "->" + r.destino + " [arrowhead = diamond];\n");
                        break;
                    case 4:
                        dot.Append(r.origen + "->" + r.destino + " [arrowhead = o];\n");
                        break;
                    default:
                        dot.Append(r.origen + "->" + r.destino + " [arrowhead = vee style = dashed];\n");
                        break;
                }

            }
            dot.Append("}");

            generarDOT_PNG(dot.ToString(), "C:\\compiladores\\imagenes_uml\\uml"+contador+++".dot", "C:\\compiladores\\imagenes_uml\\diagrama"+contador+".png");
            this.pictureBox1.Image = new System.Drawing.Bitmap(@"C:\compiladores\imagenes_uml\diagrama" +contador+".png");
        }


        private void generarDOT_PNG(string graphviz, String rdot, String rpng)
        {
            System.IO.File.WriteAllText(rdot, graphviz);
            String comandodot = "C:\\Graphviz\\bin\\dot.exe -Tpng " + rdot + " -o " + rpng + " ";
            var command = string.Format(comandodot);
            var procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/C" + command);
            var proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.WaitForExit();
        }

    }








}

public class relacion
{

    public string origen;
    public string destino;
    public int tipo_rel;

    public relacion(string clase, int tip, string claes2)
    {
        this.origen = clase;
        this.tipo_rel = tip;
        this.destino = claes2;
    }
}
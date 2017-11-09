using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//---------------------> Importar
using Irony.Ast;
using Irony.Parsing;
using System.IO;
using System.Windows.Forms;

namespace Proyecto2_compi2_2sem_2017.Compilador
{
    class Graficador
    {
        public String desktop;
        private StringBuilder graphivz;
        private int contador;

        public Graficador()
        {
            desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            this.generarRutas();
        }

        private void generarRutas()
        {
            String desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            List<String> rutas = new List<String>();
            rutas.Add(desktop + "\\Files");
            rutas.Add(desktop + "\\Files\\Arbol");
            foreach (String item in rutas)
            {
                if (!System.IO.Directory.Exists(item))
                {
                    System.IO.DirectoryInfo dir = System.IO.Directory.CreateDirectory(item);
                }
            }
        }

        private void generarDOT_PNG(String rdot, String rpng)
        {
            System.IO.File.WriteAllText(rdot, graphivz.ToString());
            String comandodot = "C:\\Users\\Jherson Sazo\\Desktop\\GRPHVIZ\\GRPHVIZ\\Graphviz\\bin\\dot.exe -Tpng " + rdot + " -o " + rpng + " ";
            var command = string.Format(comandodot);
            var procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/C" + command);
            var proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.WaitForExit();
        }

        public void graficar(ParseTree arbol)
        {
            graphivz = new StringBuilder();
            contador = 0;
            String rdot = desktop + "\\Files\\Arbol\\arbol.dot";
            String rpng = desktop + "\\Files\\Arbol\\arbol.png";
            graphivz.Append("digraph G {\r\n node[shape=rectangle, style=filled, color=khaki1, fontcolor=black]; \r\n");
            Arbol_listar_enlazar(arbol.Root, contador);
            graphivz.Append("}");
            this.generarDOT_PNG(rdot, rpng);
        }

        private void Arbol_listar_enlazar(ParseTreeNode nodo, int num)
        {
            if (nodo != null)
            {
                if(nodo.ChildNodes.Count==0)
                    graphivz.Append("node" + num + " [ label = \"" + nodo.Token.Text + "\"];\r\n");
                else
                    graphivz.Append("node" + num + " [ label = \"" + nodo.Term.ToString() + "\"];\r\n");
                int tam = nodo.ChildNodes.Count;
                int actual;
                for (int i = 0; i < tam; i++)
                {
                    contador = contador + 1;
                    actual = contador;
                    Arbol_listar_enlazar(nodo.ChildNodes[i], contador);
                    graphivz.Append("\"node" + num + "\"->\"node" + actual + "\";\r\n");
                }
            }
        }


        public void graficar_lista_linealizada(LinkedList<ParseTreeNode> lista)
        {
            //{ b |{ c |< here > d | e}| f}
            string  cont = "digraph structs {\n";
            cont += "struct3[shape = record, label = {";
                //hello\nworld |{ b |{c|<here> d|e}| f}| g | h
            foreach(ParseTreeNode raiz in lista)
            {
                switch (raiz.Term.Name)
                {
                    case ("ASIGNACION"):
                        cont+=ejecutarAsignar(raiz);
                        break;
                    case ("METODO"):
                        MessageBox.Show("VIno metodo error!");
                        break;
                    case "PUT_TO_HEAP":
                        cont += put_to_heap(raiz);
                        break;
                    case "PRINT":
                        cont += imprimir(raiz);
                        break;
                    case "GOTO":
                        cont += ejecutarGOTO(raiz);
                        break;
                    case "GET_FROM_STACK":
                        cont += get_from_stack(raiz);
                        break;
                    case "SALTO_CONDICIONAL":
                        cont += ejecutarIF(raiz);
                        break;
                    case "GET_FROM_HEAP":
                        cont += get_from_heap(raiz);
                        break;
                    case "PUT_TO_STACK":
                        cont += put_to_stack(raiz);
                        break;
                    case "LABEL":
                        cont += cont += raiz.ChildNodes[0].Token.Text + "|\n";
                        break;
                    default:
                        MessageBox.Show("me fala" + raiz.Term.Name.ToString());
                        break;
                }
            }


           cont += "\n]; \n}";
           generarDOT_de_jherson(cont);
        }

        private string put_to_stack(ParseTreeNode raiz)
        {
           return "stack[" + raiz.ChildNodes[0].Token.Text + "] = " + raiz.ChildNodes[1].Token.Text + "|\n";
        }

        private string get_from_heap(ParseTreeNode raiz)
        {
            return  raiz.ChildNodes[0].Token.Text + " = heap[" + raiz.ChildNodes[1].Token.Text + "]|\n";
        }

        private string ejecutarIF(ParseTreeNode raiz)
        {
            //throw new NotImplementedException();
            string cont = "";
            string destino = raiz.ChildNodes[1].Token.Text;
            ParseTreeNode exp = raiz.ChildNodes[0];
            string signo = "";

            cont += "if ";

            if (exp.ChildNodes.Count == 3)
            {
                ParseTreeNode uno = exp.ChildNodes[0];
                ParseTreeNode dos = exp.ChildNodes[2];
                signo = exp.ChildNodes[1].Token.Text;

                if (uno.ChildNodes.Count == 1)
                    cont += uno.ChildNodes[0].Token.Text;
                else
                    cont += uno.Token.Text;
                cont += " " + signo + " ";
                if (dos.ChildNodes.Count == 1)
                    cont += dos.ChildNodes[0].Token.Text;
                else
                    cont += dos.Token.Text;
                cont += " goto "+ destino+" |\n";

            }
            else
            {
                MessageBox.Show("EN if");
                Console.Write("n");
            }
            return cont;
        }

        private string get_from_stack(ParseTreeNode raiz)
        {
            return raiz.ChildNodes[0].Token.Text +" = stack[" + raiz.ChildNodes[1].Token.Text + "]|\n";
        }

        private string ejecutarGOTO(ParseTreeNode raiz)
        {
           return "goto "+raiz.ChildNodes[0].Token.Text + "|\n";
        }

        private string imprimir(ParseTreeNode raiz)
        {
            throw new NotImplementedException();
        }

        private string put_to_heap(ParseTreeNode raiz)
        {
            return  "heap[" + raiz.ChildNodes[0].Token.Text + "] = " + raiz.ChildNodes[1].Token.Text + "|\n";
        }

        private string ejecutarAsignar(ParseTreeNode raiz)
        {
            //throw new NotImplementedException();
            string cont = "";
            string destino = raiz.ChildNodes[0].Token.Text;
            ParseTreeNode exp = raiz.ChildNodes[1];
            string signo = "";
            cont += destino + " = ";
            if (exp.ChildNodes.Count == 3)
            {
                ParseTreeNode uno=exp.ChildNodes[0];
                ParseTreeNode dos = exp.ChildNodes[2];
                signo = exp.ChildNodes[1].Token.Text;

                if (uno.ChildNodes.Count == 1)
                    cont += uno.ChildNodes[0].Token.Text;
                else 
                    cont += uno.Token.Text;
                cont += " " + signo + " ";
                if (dos.ChildNodes.Count == 1)//cuidado aqui porque puede venir que sea -1 y truena
                    cont += dos.ChildNodes[0].Token.Text;
                else
                    cont += dos.Token.Text;
                cont += "|\n";

            }
            else
            {
                MessageBox.Show("EN asignacion");
                Console.Write("n");
                cont += exp.ChildNodes[0].Token.Text;
            }
            return cont;
        }

        public void abrirArbol(String ruta)
        {
            if (!File.Exists(ruta))
                return;

            try
            {
                System.Diagnostics.Process.Start(ruta);
            }
            catch (Exception ex)
            {
                //Error :|
            }
        }



        private void generarDOT_de_jherson(string cont)
        {
            String rdot = desktop + "\\Files\\Arbol\\arbol.dot";
            String rpng = desktop + "\\Files\\Arbol\\arbol.png";
            System.IO.File.WriteAllText(rdot, cont);
            String comandodot = "C:\\Users\\Jherson Sazo\\Desktop\\GRPHVIZ\\GRPHVIZ\\Graphviz\\bin\\dot.exe -Tpng " + rdot + " -o " + rpng + " ";
            var command = string.Format(comandodot);
            var procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/C" + command);
            var proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.WaitForExit();
        }


    }
}

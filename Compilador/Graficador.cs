using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//---------------------> Importar
using Irony.Ast;
using Irony.Parsing;
using System.IO;

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

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Ast;
using System.Windows.Forms;

namespace Proyecto2_compi2_2sem_2017.Ejecucion3D
{
    class Interprete3d
    {

        private double[] stack;
        private double[] heap;

        private Dictionary<string, double> lista_temporales;
        private LinkedList<ParseTreeNode> lista_nodos;
        private Dictionary<string,metodo3d> lista_metodos;
        private Dictionary<string, int> lista_etiquetas;
        private int ptr = 0;
        public StringBuilder salida;


        public Interprete3d()
        {
            this.stack = new double[10000];
            this.heap = new double[100000];
            this.lista_nodos = new LinkedList<ParseTreeNode>();
            this.lista_temporales = new Dictionary<string, double>();
            this.lista_metodos = new Dictionary<string,metodo3d>();
            this.lista_etiquetas = new Dictionary<string, int>();
            this.salida = new StringBuilder();
            lista_temporales.Add("P", 0);
            lista_temporales.Add("H", 0);
        }

        private void setPtr(int val)
        {
            this.ptr = val;
        }


        public void analizar(String entrada)
        {
            Gramatica3d gramatica = new Gramatica3d();
            Parser parser = new Parser(gramatica);

            ParseTree arbol = parser.Parse(entrada);
            ParseTreeNode raiz = arbol.Root;

            if (raiz == null || arbol.ParserMessages.Count > 0 || arbol.HasErrors())
            {
                //---------------------> Hay Errores      
                MessageBox.Show("Hay Errores en 3d");
                return;
            }


            traducir_a_lista(raiz.ChildNodes[0]);
            imprimir_lista_linealizada();
            for (; ptr < lista_nodos.Count;)
            {
                ejecutar(lista_nodos.ElementAt(ptr), ptr);
                ptr++;
            }
//            ejecutar(raiz.ChildNodes[0]);
            //---------------------> Todo Bien
            
        }

        private void traducir_a_lista(ParseTreeNode raiz)
        {
            switch (raiz.Term.Name)
            {
                case ("SENT_BODY"):
                    foreach (ParseTreeNode a in raiz.ChildNodes)
                        traducir_a_lista(a);
                    break;
                case ("METODO"):
                    string nombre = raiz.ChildNodes[0].Token.Text;
                    int inicio = lista_nodos.Count;
                    foreach(ParseTreeNode a in raiz.ChildNodes[1].ChildNodes)
                        traducir_a_lista(a);
                    int fin = lista_nodos.Count-1;
                    lista_metodos.Add(nombre, new metodo3d(nombre, inicio, fin));
                    break;
                case "LABEL":
                    lista_nodos.AddLast(raiz);
                    guardarLabels(raiz);
                    break;
                default:
                    lista_nodos.AddLast(raiz);
                    break;
            }
        }

        private void guardarLabels(ParseTreeNode raiz)
        {
            string val = raiz.ChildNodes[0].Token.Text;
            if (!lista_etiquetas.ContainsKey(val))
                lista_etiquetas.Add(val, lista_nodos.Count-1);
            else
                MessageBox.Show("ERROR EN LA ETIQUETA: " + val);
        }

        private void imprimir_lista_linealizada()
        {
            foreach(ParseTreeNode a in lista_nodos)
            {
                Console.WriteLine(a.Term.Name);
            }
        }

        public void ejecutar(ParseTreeNode raiz,int ptr)
        {
            switch (raiz.Term.Name)
            {
                case ("SENT_BODY"):
                    foreach (ParseTreeNode a in raiz.ChildNodes)
                        ejecutar(a,ptr);
                    break;
                case ("ASIGNACION"):
                    ejecutarAsignar(raiz);
                    break;
                case ("METODO"):
                    ejecutarMetodo(raiz);
                    break;
                case "PUT_TO_HEAP":
                    put_to_heap(raiz);
                    break;
                case "PRINT":
                    imprimir(raiz);
                    break;
                case "GOTO":
                    ejecutarGOTO(raiz);
                    break;
                case "GET_FROM_STACK":
                    get_from_stack(raiz);
                    break;
                case "SALTO_CONDICIONAL":
                    ejecutarIF(raiz);
                    break;
                case "GET_FROM_HEAP":
                    get_from_heap(raiz);
                    break;
                case "PUT_TO_STACK":
                    put_to_stack(raiz);
                    break;
                default:
                    Console.WriteLine("me falta: " + raiz.Term.Name);
                    break;
            }
        }

        private void put_to_stack(ParseTreeNode raiz)
        {
            double pos = evaluarEXPRESION(raiz.ChildNodes[0]);
            double val = evaluarEXPRESION(raiz.ChildNodes[1]);
            int posi = Convert.ToInt32(pos);
            stack[posi] = val;
        }

        private void get_from_heap(ParseTreeNode raiz)
        {
            string tmp = raiz.ChildNodes[0].Token.Text;
            double res = evaluarEXPRESION(raiz.ChildNodes[1]);
            int pos = Convert.ToInt32(res);
            double val = heap[pos];
            if (lista_temporales.ContainsKey(tmp))
            {
                lista_temporales.Remove(tmp);
                lista_temporales.Add(tmp, val);
            }
            else
                lista_temporales.Add(tmp, val);
        }

        private void ejecutarIF(ParseTreeNode raiz)
        {
            try
            {
                if ((Boolean)evaluarCOND(raiz.ChildNodes[0]))
                {
                    string eti = raiz.ChildNodes[1].Token.Text;
                    int pos = -1;
                    lista_etiquetas.TryGetValue(eti, out pos);
                    setPtr(pos);
                }
            }catch
            {

            }
        }

        private void get_from_stack(ParseTreeNode raiz)
        {
            string tmp = raiz.ChildNodes[0].Token.Text;
            double res = evaluarEXPRESION(raiz.ChildNodes[1]);
            int pos = Convert.ToInt32(res);
            double val = stack[pos];
            if (lista_temporales.ContainsKey(tmp))
            {
                lista_temporales.Remove(tmp);
                lista_temporales.Add(tmp, val);
            }
            else
                lista_temporales.Add(tmp, val);
        }

        private void ejecutarGOTO(ParseTreeNode raiz)
        {
            string eti = raiz.ChildNodes[0].Token.Text;
            int pos = -1;
            lista_etiquetas.TryGetValue(eti, out pos);
            setPtr(pos);
        }

        private void imprimir(ParseTreeNode raiz)
        {
            //Console.Write(';');
            double res = evaluarEXPRESION(raiz.ChildNodes[1]);

            switch (raiz.ChildNodes[0].Token.Text)
            {
                case "c":
                    char x = (char)(int)res;
                    this.salida.Append(x);
                    break;
            }
        }

        private void put_to_heap(ParseTreeNode raiz)
        {
            double pos = evaluarEXPRESION(raiz.ChildNodes[0]);
            double val = evaluarEXPRESION(raiz.ChildNodes[1]);
            int posi = Convert.ToInt32(pos);
            heap[posi] = val;
        }

        private void ejecutarMetodo(ParseTreeNode raiz)
        {
            foreach (ParseTreeNode a in raiz.ChildNodes[2].ChildNodes)
                ejecutar(a,ptr);
        }

        private void ejecutarAsignar(ParseTreeNode raiz)
        {
            string tmp = raiz.ChildNodes[0].Token.Text;
            double res = evaluarEXPRESION(raiz.ChildNodes[1]);
            if (lista_temporales.ContainsKey(tmp))
            {
                lista_temporales.Remove(tmp);
                lista_temporales.Add(tmp, res);
            }else
                lista_temporales.Add(tmp, res);
        }


        private Double evaluarEXPRESION(ParseTreeNode nodo)
        {
            //---------------------> Si tiene 3 hijos
            #region "3 hijos"
            if (nodo.ChildNodes.Count == 3)
            {
                String operador = nodo.ChildNodes[1].Term.Name;
                switch (operador)
                {

                    case "+": return evaluarMAS(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case "-": return evaluarMENOS(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case "*": return evaluarPOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case "/": return evaluarDIVIDIR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                   default: break;
                }
            }
        

            #endregion

            //---------------------> Si tiene 2 hijos
            #region "2 hijos"
            if (nodo.ChildNodes.Count == 2)
            {
                if (nodo.ChildNodes[0].Term.Name.Equals("!")) { }
                    //return evaluarNOT(nodo.ChildNodes[1]);

            }
            #endregion

            //---------------------> Si tiene 1 hijo
            #region "1 hijo"
            if (nodo.ChildNodes.Count == 1)
            {

                String termino = nodo.ChildNodes[0].Term.Name;
                switch (termino)
                {

                    case "id": return evaluarID(nodo.ChildNodes[0]);
                    case "numero": return Convert.ToDouble((nodo.ChildNodes[0].Token.Text));
                    case "EXP": return evaluarEXPRESION(nodo.ChildNodes[0]);
                   // case "tstring": return nodo.ChildNodes[0].Token.Text.Replace("\"", "");
                    //case "tchar": return nodo.ChildNodes[0].Token.Text.Replace("'", "");
                    default: break;


                }
            }
            #endregion

            if (nodo.ChildNodes.Count == 0)
            {
                switch (nodo.Term.Name)
                {
                    case "id":
                        return evaluarID(nodo);
                    case "numero":
                        return Convert.ToDouble((nodo.Token.Text));
                }
            }

            //---------------------> Retorno por defecto
            MessageBox.Show("ERROR no se ha definido para: " + nodo.Term.Name);
            return -1;
        }

        private Double evaluarMAS(ParseTreeNode uno, ParseTreeNode dos)
        {
            double val1 = evaluarEXPRESION(uno);
            double val2 = evaluarEXPRESION(dos);

            return val1 + val2;
        }
        private double evaluarDIVIDIR(ParseTreeNode uno, ParseTreeNode dos)
        {
            double val1 = evaluarEXPRESION(uno);
            double val2 = evaluarEXPRESION(dos);

            return val1 / val2;
        }

        private double evaluarPOR(ParseTreeNode uno, ParseTreeNode dos)
        {
            double val1 = evaluarEXPRESION(uno);
            double val2 = evaluarEXPRESION(dos);

            return val1 * val2;
        }

        private double evaluarMENOS(ParseTreeNode uno, ParseTreeNode dos)
        {
            double val1 = evaluarEXPRESION(uno);
            double val2 = evaluarEXPRESION(dos);

            return val1 - val2;
        }

        private object evaluarCOND(ParseTreeNode nodo)
        {
            if (nodo.ChildNodes.Count == 3)
            {
                String operador = nodo.ChildNodes[1].Term.Name;
                switch (operador)
                {

                        case "==": return evaluarIGUAL(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                        case "!=": return evaluarDIFERENTE(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                        case ">=": return evaluarMAYORIGUAL(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                        case ">": return evaluarMAYOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                        case "<=": return evaluarMENORIGUAL(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                        case "<": return evaluarMENOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                        default: break;
                }
            }
            return null;
        }

        private object evaluarMENOR(ParseTreeNode uno, ParseTreeNode dos)
        {
            double val1 = evaluarEXPRESION(uno);
            double val2 = evaluarEXPRESION(dos);

            try
            {
                return Convert.ToDouble(val1) < Convert.ToDouble(val2);
            }
            catch
            {
                return -1;
            }
            
        }

        private object evaluarMENORIGUAL(ParseTreeNode uno, ParseTreeNode dos)
        {
            double val1 = evaluarEXPRESION(uno);
            double val2 = evaluarEXPRESION(dos);

            try
            {
                return Convert.ToDouble(val1) <= Convert.ToDouble(val2);
            }
            catch
            {
                return -1;
            }
        }

        private object evaluarMAYOR(ParseTreeNode uno, ParseTreeNode dos)
        {
            double val1 = evaluarEXPRESION(uno);
            double val2 = evaluarEXPRESION(dos);

            try
            {
                return Convert.ToDouble(val1) > Convert.ToDouble(val2);
            }
            catch
            {
                return -1;
            }
        }

        private object evaluarMAYORIGUAL(ParseTreeNode uno, ParseTreeNode dos)
        {
            double val1 = evaluarEXPRESION(uno);
            double val2 = evaluarEXPRESION(dos);

            try
            {
                return Convert.ToDouble(val1) >= Convert.ToDouble(val2);
            }
            catch
            {
                return -1;
            }
        }

        private object evaluarDIFERENTE(ParseTreeNode uno, ParseTreeNode dos)
        {
            double val1 = evaluarEXPRESION(uno);
            double val2 = evaluarEXPRESION(dos);

            return !val1.Equals(val2);
        }

        private object evaluarIGUAL(ParseTreeNode uno, ParseTreeNode dos)
        {
            double val1 = evaluarEXPRESION(uno);
            double val2 = evaluarEXPRESION(dos);

            return val1.Equals(val2);
        }

        private double evaluarID(ParseTreeNode nodo)
        {
            string tmp = nodo.Token.Text;

            double res = -1;
            if (lista_temporales.ContainsKey(tmp))
            {
                lista_temporales.TryGetValue(tmp, out res);
            }
            return res;
        }

    }



    class metodo3d
    {
        public int inicio;
        public int fin;
        public string nombre;

        public metodo3d(string name,int start,int end)
        {
            this.inicio = start;
            this.fin = end;
            this.nombre = name;
        }
    }

    /*        public void ejecutar(ParseTreeNode raiz,int ptr)
        {


            switch (raiz.Term.Name)
            {
                case ("SENT_BODY"):
                    foreach (ParseTreeNode a in raiz.ChildNodes)
                        ejecutar(a,ptr);
                    break;
                case ("ASIGNACION"):
                    ejecutarAsignar(raiz);
                    break;
                case ("METODO"):
                    ejecutarMetodo(raiz);
                    break;
                case "PUT_TO_HEAP":
                    put_to_heap(raiz);
                    break;
                case "IMPRIMIR":
                    imprimir(raiz);
                    break;


                //asignacion
                //obtener de heap
                //put to heap
                //put to stack

            }
        }
*/
}


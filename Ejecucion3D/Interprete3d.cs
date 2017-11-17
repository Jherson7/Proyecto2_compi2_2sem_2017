using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Ast;
using System.Windows.Forms;
using Proyecto2_compi2_2sem_2017.Compilador;
using Proyecto2_compi2_2sem_2017.Control3D;

namespace Proyecto2_compi2_2sem_2017.Ejecucion3D
{
    class Interprete3d
    {

        private double[] stack;
        private double[] heap;

        private Dictionary<string, double> lista_temporales;
        private LinkedList<nodo_ejecucion> lista_nodos;
        private Dictionary<string,metodo3d> lista_metodos;
        private Dictionary<string, int> lista_etiquetas;
        private LinkedList<Dictionary<string, double>> lista_ambitos;
        private LinkedList<ambitos_llamadas> ambitos_llamadas;
        private int ptr = 0;
        public StringBuilder salida;

        public Interprete3d()
        {
            this.stack = new double[10000];
            this.heap = new double[100000];
            this.lista_nodos = new LinkedList<nodo_ejecucion>();
            //this.lista_temporales = new Dictionary<string, double>();
            this.lista_metodos = new Dictionary<string,metodo3d>();
            this.lista_etiquetas = new Dictionary<string, int>();
            this.salida = new StringBuilder();
            this.lista_ambitos = new LinkedList<Dictionary<string, double>>();
            this.ambitos_llamadas = new LinkedList<ambitos_llamadas>();

            this.lista_ambitos.AddFirst(new Dictionary<string, double>());
            this.lista_temporales = lista_ambitos.First();
            lista_temporales.Add("P", 0);
            lista_temporales.Add("H", 0);

            for (int i = 0; i < 10000; i++)
                stack[i] = -3092;
            for (int i = 0; i < 100000; i++)
                heap[i] = -3092;
        }


        private void aumentar_ambito(ambitos_llamadas nuevo)
        {
            this.ambitos_llamadas.AddFirst(nuevo);
            //aumento ambitos de variables temporales
            this.lista_ambitos.AddFirst(new Dictionary<string, double>());
            this.lista_temporales = lista_ambitos.First();

        }

        private void disminuir_ambito()
        {
            this.lista_ambitos.RemoveFirst();
            this.lista_temporales = lista_ambitos.First();
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

                foreach (var item in arbol.ParserMessages)
                    Control3d.agregarError(new errores("sintactico",item.Location.Line,item.Location.Column,item.Message));
                MessageBox.Show("Hay Errores en 3d");
                return;
            }

            //Graficador gr = new Graficador();
//            gr.graficar(arbol);
            traducir_a_lista(raiz.ChildNodes[0]);
            imprimir_lista_linealizada();

            //gr.graficar_lista_linealizada(lista_nodos);
            for (; ptr < lista_nodos.Count;)
            {
                ejecutar(lista_nodos.ElementAt(ptr));
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
                    lista_nodos.AddLast(new nodo_ejecucion("FIN_METODO", raiz));
                    break;
                case "LABEL":
                    lista_nodos.AddLast(new nodo_ejecucion("LABEL", raiz));
                    guardarLabels(raiz);
                    break;
                default:
                    string name = raiz.Term.Name;
                    lista_nodos.AddLast(new nodo_ejecucion(name,raiz));
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
            foreach(nodo_ejecucion a in lista_nodos)
            {
                Console.WriteLine(a.nodo.Term.Name);
            }
        }

        public void ejecutar(nodo_ejecucion raiz)
        {
            switch (raiz.nombre)
            {
                case ("SENT_BODY"):
                    foreach (ParseTreeNode a in raiz.nodo.ChildNodes)
                        ejecutar(raiz);
                    break;
                case ("ASIGNACION"):
                    ejecutarAsignar(raiz.nodo);
                    break;
                case ("METODO"):
                    MessageBox.Show("hay erro en llamada en metodo en ejeucuion");
                    //ejecutarMetodo(raiz.nodo);
                    break;
                case "PUT_TO_HEAP":
                    put_to_heap(raiz.nodo);
                    break;
                case "PRINT":
                    imprimir(raiz.nodo);
                    break;
                case "GOTO":
                    ejecutarGOTO(raiz.nodo);
                    break;
                case "GET_FROM_STACK":
                    get_from_stack(raiz.nodo);
                    break;
                case "SALTO_CONDICIONAL":
                    ejecutarIF(raiz.nodo);
                    break;
                case "GET_FROM_HEAP":
                    get_from_heap(raiz.nodo);
                    break;
                case "PUT_TO_STACK":
                    put_to_stack(raiz.nodo);
                    break;
                case "CALLFUN":
                    ejecutarLLAMADA(raiz.nodo);
                    break;
                case "FIN_METODO":
                    ejecutarFIN_METODO();
                    break;
                case "THROW":
                    MessageBox.Show("GRAVE ERROR INTENTO ACCEDER A UNA POSICION NULA O VACIA CERCA DE LA LINEA: " + ptr);
                    //throw new NullReferenceException();
                    return;
                    //break;
                default:
                    if(!raiz.nombre.Equals("LABEL"))
                        MessageBox.Show("me falta: " + raiz.nombre);
                    break;
            }
        }

        private void ejecutarFIN_METODO()
        {
            //throw new NotImplementedException();
            int actual = ambitos_llamadas.First().inicio;
            ambitos_llamadas.RemoveFirst();
            disminuir_ambito();
            setPtr(actual);
        }

        private void ejecutarLLAMADA(ParseTreeNode raiz)
        {
            try
            {
                string nombre = raiz.ChildNodes[0].Token.Text;
                metodo3d aux;
                this.lista_metodos.TryGetValue(nombre, out aux);
                //aumentar ambito 3d
                ambitos_llamadas nuevo = new ambitos_llamadas(ptr, aux.fin);
                aumentar_ambito(nuevo);//ahi guarde la posicion actual del puntero para recuperarlos despues
                setPtr(aux.inicio-1);
            }
            catch
            {
                MessageBox.Show("ERRor en llamada a metodo no se encotro");
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
                ParseTreeNode cond = raiz.ChildNodes[1];

                Boolean a = (Boolean)evaluarCOND(cond);

                if (raiz.ChildNodes[0].Term.Name.Equals("ifFalse"))
                    a = !a;

                if (a)
                {
                    string eti = raiz.ChildNodes[2].Token.Text;
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

            if (tmp.Equals("P") || tmp.Equals("H"))
            {
                lista_ambitos.Last().Remove(tmp);
                lista_ambitos.Last().Add(tmp, val);
            }else if (lista_temporales.ContainsKey(tmp))
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
                case "d":
                    string a = Convert.ToInt32(res).ToString();
                    this.salida.Append(a);
                    break;
                case "f":
                    a = res.ToString();
                    this.salida.Append(a);
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

        private void ejecutarAsignar(ParseTreeNode raiz)
        {
            string tmp = raiz.ChildNodes[0].Token.Text;
            double res = evaluarEXPRESION(raiz.ChildNodes[1]);

            if (tmp.Equals("P") || tmp.Equals("H"))
            {
                lista_ambitos.Last().Remove(tmp);
                lista_ambitos.Last().Add(tmp,res);
            }else if (lista_temporales.ContainsKey(tmp))
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
                string signo = nodo.ChildNodes[0].Term.Name;
                if (signo.Equals("!")) {

                }else if (signo.Equals("-"))
                {
                    double aux = evaluarEXPRESION(nodo.ChildNodes[1]);
                    return aux * -1;
                }else
                {
                    MessageBox.Show("Falta: " + signo);
                }
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
                    case "numero": return Convert.ToDouble((nodo.ChildNodes[0].Token.Text.Replace(".",",")));
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
                        return Convert.ToDouble(nodo.Token.Text.Replace(".",","));
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

            double res = -3092;

            if (tmp.Equals("H") || tmp.Equals("P"))
            {
                lista_ambitos.Last().TryGetValue(tmp, out res);
            }
            else
            {
                if (lista_temporales.ContainsKey(tmp))
                    lista_temporales.TryGetValue(tmp, out res);
            }
            if (res == -3092)
            {
                MessageBox.Show("NullPointerExeption() en la linea: " + nodo.Span.Location.Line);
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

    class nodo_ejecucion
    {
        
        public string nombre;
        public ParseTreeNode nodo;

        public nodo_ejecucion(string name, ParseTreeNode nodox)
        {
            this.nombre = name;
            this.nodo = nodox;
        }
    }

    class ambitos_llamadas
    {
        public int inicio;
        public int fin;

        public ambitos_llamadas(int star,int fin)
        {
            this.inicio = star;
            this.fin = fin;
        }
    }
}


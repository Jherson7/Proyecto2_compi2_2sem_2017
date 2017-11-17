using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using System.Windows.Forms;


namespace Proyecto2_compi2_2sem_2017.Ejecucion3D
{
    class Optimizador3D
    {

        private Dictionary<string, double> lista_temporales;
        private LinkedList<nodo_ejecucion> lista_nodos;
        private Dictionary<string, metodo3d> lista_metodos;
        private Dictionary<string, int> lista_etiquetas;
        private LinkedList<Dictionary<string, double>> lista_ambitos;
        private LinkedList<ambitos_llamadas> ambitos_llamadas;
        private int ptr = 0;
        public  StringBuilder salida;
        public StringBuilder reporte_optimizacion;


        public Optimizador3D()
        {
            this.lista_nodos = new LinkedList<nodo_ejecucion>();
            //this.lista_temporales = new Dictionary<string, double>();
            this.lista_metodos = new Dictionary<string, metodo3d>();
            this.lista_etiquetas = new Dictionary<string, int>();
            this.salida = new StringBuilder();
            this.lista_ambitos = new LinkedList<Dictionary<string, double>>();
            this.ambitos_llamadas = new LinkedList<ambitos_llamadas>();

            this.lista_ambitos.AddFirst(new Dictionary<string, double>());
            this.lista_temporales = lista_ambitos.First();
            lista_temporales.Add("P", 0);
            lista_temporales.Add("H", 0);

            this.salida = new StringBuilder();
            this.reporte_optimizacion = new StringBuilder();
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
                //Control3d.agregarError(new errores("sintactico", item.Location.Line, item.Location.Column, item.Message));
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
                    lista_nodos.AddLast(new nodo_ejecucion("METODO", raiz));
                    foreach (ParseTreeNode a in raiz.ChildNodes[1].ChildNodes)
                        traducir_a_lista(a);
                    lista_nodos.AddLast(new nodo_ejecucion("FIN_METODO", raiz));
                    break;
                case "LABEL":
                    lista_nodos.AddLast(new nodo_ejecucion("LABEL", raiz));
                    guardarLabels(raiz);
                    break;
                default:
                    string name = raiz.Term.Name;
                    lista_nodos.AddLast(new nodo_ejecucion(name, raiz));
                    break;
            }
        }

        private void guardarLabels(ParseTreeNode raiz)
        {
            string val = raiz.ChildNodes[0].Token.Text;
            if (!lista_etiquetas.ContainsKey(val))
                lista_etiquetas.Add(val, lista_nodos.Count - 1);
            else
                MessageBox.Show("ERROR EN LA ETIQUETA: " + val);
        }

        private void imprimir_lista_linealizada()
        {
            foreach (nodo_ejecucion a in lista_nodos)
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
                    salida.Append("void "+ raiz.nodo.ChildNodes[0].Token.Text+"(){\n");
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
                    salida.Append("}\n");
                    break;
                case "THROW":
                    salida.Append("throw[]\n");
                    break;
                default:
                    Console.WriteLine( raiz.nodo.ChildNodes[0].Token.Text);
                    salida.Append(raiz.nodo.ChildNodes[0].Token.Text+":\n");
                    break;
            }
        }

        private void ejecutarLLAMADA(ParseTreeNode raiz)
        {
                string nombre = raiz.ChildNodes[0].Token.Text;
                salida.Append(nombre+"()\n");
        }

        private void put_to_stack(ParseTreeNode raiz)
        {
            salida.Append("stack[" + raiz.ChildNodes[0].Token.Text + "]= " + raiz.ChildNodes[1].Token.Text+"\n");

           /* double pos = evaluarEXPRESION(raiz.ChildNodes[0]);
            double val = evaluarEXPRESION(raiz.ChildNodes[1]);
            int posi = Convert.ToInt32(pos);*/
        }

        private void get_from_heap(ParseTreeNode raiz)
        {
            salida.Append(raiz.ChildNodes[0].Token.Text +" = heap[" + raiz.ChildNodes[1].Token.Text + "]\n");

            /*  string tmp = raiz.ChildNodes[0].Token.Text;
            double res = evaluarEXPRESION(raiz.ChildNodes[1]);
            int pos = Convert.ToInt32(res);
            */
        }

        private void ejecutarIF(ParseTreeNode raiz)
        {
            //salida.Append("if ");

            string eti = raiz.ChildNodes[2].Token.Text;
            int pos = 0;
            lista_etiquetas.TryGetValue(eti, out pos);

            ParseTreeNode aux = raiz.ChildNodes[1];

            string uno = aux.ChildNodes[0].Token.Text;
            string op = aux.ChildNodes[1].Token.Text;
            string dos = aux.ChildNodes[2].Token.Text;

            //vamos a tratar de optimizar con la regla 7
            //if cond goto L0
            //L0: goto L1 ==>  if cond goto L1
            nodo_ejecucion nodo = lista_nodos.ElementAt(pos + 1);
            if (nodo.nombre.Equals("GOTO"))
            {
                salida.Append("if " + uno + " " + op + " " + dos + " goto " + nodo.nodo.ChildNodes[0].Token.Text + "\n");
                agregar_notificacion(7, raiz);
                return;
            }


            //vamos a tratar de optimizar con la regla 5
            //if 1==0 goto l1
            //goto l2
            //=> goto l2
            nodo = lista_nodos.ElementAt(ptr + 1);
            if (uno.Equals("1") && dos.Equals("0")){
                
                if (nodo.nombre.Equals("GOTO"))
                {
                    salida.Append("goto " + nodo.nodo.ChildNodes[0].Token.Text + "\n");
                    setPtr(ptr + 1);
                    agregar_notificacion(5, raiz);
                    return;
                }
            }
            //vamos a tratar de optimizar con la regla 4
            //if 1==1 goto l1
            //goto l2
            //=> goto l1
            if (uno.Equals("1") && dos.Equals("1")){
                if (nodo.nombre.Equals("GOTO"))
                {
                    salida.Append("goto " + eti + "\n");
                    setPtr(ptr + 1);
                    agregar_notificacion(4, raiz);
                    return;
                }
            }

            //vamos a tratar de optimizar con la regla 3
            //la del complemento
            if (nodo.nombre.Equals("GOTO"))
            {
                salida.Append("if " +uno + " " + complemento(op) + " " + dos + " goto " +nodo.nodo.ChildNodes[0].Token.Text + "\n");
                setPtr(ptr + 1);
                agregar_notificacion(3, raiz);
                return;
            }
            salida.Append("if " + uno + " " + op + " " + dos + " goto " + eti+"\n");

        }


        private string complemento(string op)
        {
            switch(op)
            {
                case "==":
                    return "!=";
                case "!=":
                    return "==";
                case "<":
                    return ">=";
                case ">":
                    return "<=";
                case "<=":
                    return ">";
                case ">=":
                    return "<";
            }
            return op;
        }

        private void get_from_stack(ParseTreeNode raiz)
        {
            salida.Append(raiz.ChildNodes[0].Token.Text + " = stack[" + raiz.ChildNodes[1].Token.Text + "]\n");

           /* string tmp = raiz.ChildNodes[0].Token.Text;
            double res = evaluarEXPRESION(raiz.ChildNodes[1]);
            int pos = Convert.ToInt32(res);*/
            
        }

        private void ejecutarGOTO(ParseTreeNode raiz)
        {
            string eti = raiz.ChildNodes[0].Token.Text;
            //obtengo la posicion de la etiqueta a donde esta saltando
            int pos = 0;
            lista_etiquetas.TryGetValue(eti, out pos);

            //vamos a tratar de optimizar con la regla 6
            //goto l1
            //L1: goto L2 ==>  gotol2
            if (!(pos + 1 >= lista_nodos.Count))
            {
                nodo_ejecucion nodo = lista_nodos.ElementAt(pos + 1);

                if (nodo.nombre.Equals("GOTO"))
                {
                    salida.Append("goto " + nodo.nodo.ChildNodes[0].Token.Text + "\n");
                    agregar_notificacion(7, raiz);
                    return;
                }

                //vamos a trata de optimizar la regla 2
                bool a = true;
                if (ptr< pos)
                {
                    for (int x = ptr + 1; x < pos; x++)
                    {
                        nodo_ejecucion aux = lista_nodos.ElementAt(x);
                        if (aux.nombre.Equals("LABEL"))
                        {
                            a = false;
                            break;
                        }
                    }

                    if (a)
                    {
                        agregar_notificacion(2, raiz);
                        setPtr(pos - 1);
                        return;
                    }
                }
            }

            salida.Append("goto " + eti+"\n");
            /*
            int pos = -1;
            lista_etiquetas.TryGetValue(eti, out pos);
            setPtr(pos);*/
        }

        private void imprimir(ParseTreeNode raiz)
        {
            //Console.Write(';');
            // double res = evaluarEXPRESION(raiz.ChildNodes[1]);
            salida.Append("print(\"%" + raiz.ChildNodes[0].Token.Text + "\"," +raiz.ChildNodes[1].Token.Text + ")\n");
        }

        private void put_to_heap(ParseTreeNode raiz)
        {

            salida.Append("heap[" + raiz.ChildNodes[0].Token.Text + "]= " + raiz.ChildNodes[1].Token.Text+"\n");

            /*double pos = evaluarEXPRESION(raiz.ChildNodes[0]);
            double val = evaluarEXPRESION(raiz.ChildNodes[1]);
            int posi = Convert.ToInt32(pos);*/
        }

        private void ejecutarAsignar(ParseTreeNode raiz)
        {
            string tmp = raiz.ChildNodes[0].Token.Text;

            ParseTreeNode aux = raiz.ChildNodes[1];
            string uno = "";
            try
            {
                uno = aux.ChildNodes[0].ChildNodes[0].Token.Text;
            }catch
            {
                uno = aux.ChildNodes[0].Token.Text;
            }
               
            
            if (aux.ChildNodes.Count == 3)
            {
                string signo = aux.ChildNodes[1].Token.Text;
                string dos = aux.ChildNodes[2].ChildNodes[0].Token.Text;

                if (uno.Equals(tmp) || dos.Equals(tmp)) {

                    switch (signo)
                    {
                        case "+":
                            if (uno.Equals("0") || dos.Equals("0"))
                            {
                                agregar_notificacion(8, raiz);
                                return;
                            }
                         break;
                        case "-":
                            if (dos.Equals("0"))
                            {
                                agregar_notificacion(9, raiz);
                                return;
                            }
                            break;
                        case "*":
                            if (uno.Equals("1") || dos.Equals("1"))
                            {
                                agregar_notificacion(10, raiz);
                                return;
                            }
                            break;
                        case "/":
                            if (dos.Equals("1"))
                            {
                                agregar_notificacion(11, raiz);
                                return;
                            }
                            break;

                    }
                }
                
            }
            if (aux.ChildNodes.Count == 1)
            {
                salida.Append(tmp + " = ");
                salida.Append(aux.ChildNodes[0].Token.Text);
            }else if (aux.ChildNodes.Count == 2)
            {
                salida.Append(tmp + " = -");
                salida.Append(aux.ChildNodes[1].Token.Text);
            }
            else
            {
                salida.Append(tmp + " = ");
                evaluarEXPRESION(raiz.ChildNodes[1]);
            }
            
            /* double res = evaluarEXPRESION(raiz.ChildNodes[1]);*/
            salida.Append("\n");
            
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
                if (signo.Equals("!"))
                {
                    MessageBox.Show("Falta: " + signo);
                }
                else if (signo.Equals("-"))
                {
                    double aux = evaluarEXPRESION(nodo.ChildNodes[1]);
                    return aux * -1;
                }
                else
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
                    case "numero": return Convert.ToDouble((nodo.ChildNodes[0].Token.Text.Replace(".", ",")));
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
                        return Convert.ToDouble(nodo.Token.Text.Replace(".", ","));
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

            string a = uno.ChildNodes[0]. Token.Text;
            string b = dos.ChildNodes[0].Token.Text;

            if (val1 == 0 || val2 == 0) {
                if (val1 == 0)
                    salida.Append(b);
                else
                    salida.Append(a);
                agregar_notificacion(12, uno);
             }
            else
                salida.Append(a + " + "+b);
            return val1 + val2;
        }

        private double evaluarDIVIDIR(ParseTreeNode uno, ParseTreeNode dos)
        {
            double val1 = evaluarEXPRESION(uno);
            double val2 = evaluarEXPRESION(dos);

            string a = uno.ChildNodes[0].Token.Text;
            string b = dos.ChildNodes[0].Token.Text;

            if (val1 == 0) { 
                salida.Append("0");
                agregar_notificacion(19, uno);
            }
            else if (val2 == 1) { 
                salida.Append(a);
                agregar_notificacion(15, uno);
            }
            else
                salida.Append(a + " / " + b);

            return val1 / val2;
        }

        private double evaluarPOR(ParseTreeNode uno, ParseTreeNode dos)
        {
            double val1 = evaluarEXPRESION(uno);
            double val2 = evaluarEXPRESION(dos);

            string a = uno.ChildNodes[0].Token.Text;
            string b = dos.ChildNodes[0].Token.Text;

            if (val1 == 0 || val2 == 0) { 
                salida.Append("0"); agregar_notificacion(18, uno);
            }
            else if (val1 == 1) { 
                salida.Append(b);
                agregar_notificacion(14, uno);
            }
            else if (val2 == 1) { 
                salida.Append(a);
                agregar_notificacion(14, uno);
            }
            else if (val2 == 2)
            {
                salida.Append(a + " + " + a);
                agregar_notificacion(17, uno);
            }
            else
                salida.Append(a + " * " + b);

            return val1 * val2;
        }

        private double evaluarMENOS(ParseTreeNode uno, ParseTreeNode dos)
        {
            double val1 = evaluarEXPRESION(uno);
            double val2 = evaluarEXPRESION(dos);

            string a = uno.ChildNodes[0]. Token.Text;
            string b = dos.ChildNodes[0].Token.Text;

            if (val2 == 0) { 
                salida.Append(a);
                agregar_notificacion(13, uno);
            }
            else
                salida.Append(a + " - " + b);

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
            return -3092;/*
            if (tmp.Equals("H") || tmp.Equals("P"))
            {
                lista_ambitos.Last().TryGetValue(tmp, out res);
            }
            else
            {
                if (lista_temporales.ContainsKey(tmp))
                    lista_temporales.TryGetValue(tmp, out res);
            }

            return res;*/
        }


        public void agregar_notificacion(int numero,ParseTreeNode lina)
        {
            reporte_optimizacion.Append("se optimizo con la regla: " + numero + ", en la linea: " + lina.Span.Location.Line + "\n");
        }
    }
}

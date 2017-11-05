using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//---------------------> Importar
using Irony.Ast;
using Irony.Parsing;
using Proyecto2_compi2_2sem_2017.TablaSimbolos;
using Proyecto2_compi2_2sem_2017.Control3D;

namespace Proyecto2_compi2_2sem_2017.Compilador
{
    class Interprete
    {
        /****************************************************************************************/
        /****************************************************************************************/

        #region "Atributos"
        public StringBuilder salida;
        public StringBuilder errores;
        private LinkedList<ambitos> ambitos;
        private ambitos listaActual;

        private LinkedList<metodo> metodos;
        private LinkedList<metodo> constructores;
        private tablaSimbolos tabla;
        private LinkedList<int> posicion;
        public Dictionary<string,objeto_clase> lista_clases;
        #endregion

        /****************************************************************************************/
        /****************************************************************************************/

        #region "Interprete :D"

        public Interprete()
        {
            this.salida = new StringBuilder();
            this.errores = new StringBuilder();
            this.metodos = new LinkedList<metodo>();
            this.tabla = Control3d.getTabla();
            this.posicion = new LinkedList<int>();
            this.lista_clases = new Dictionary<string, objeto_clase>();
            this.ambitos = new LinkedList<Compilador.ambitos>();
            this.listaActual = new ambitos("Global");
            ambitos.AddFirst(listaActual);
        }
       
        public void analizar(String entrada)
        {
            Gramatica gramatica = new Gramatica();
            Parser parser = new Parser(gramatica);

            ParseTree arbol = parser.Parse(entrada);
            ParseTreeNode raiz = arbol.Root;

            if (raiz == null || arbol.ParserMessages.Count > 0 || arbol.HasErrors())
            {
                //---------------------> Hay Errores      
                MessageBox.Show("Hay Errores");
                errores.Append("hay errores:\n");
                foreach (var item in arbol.ParserMessages)
                {
                    errores.Append(item.Location.Line + " ");
                    errores.Append(item.Location.Column + " ");
                    errores.Append(item.Message + "\n");
                }
                return;
            }
            //---------------------> Todo Bien
            Graficador g = new Graficador();
            g.graficar(arbol);
            SentenciasGlobales(raiz);
            iniciar();
            mostrarTablaSimbolos();
            Control3d.setListaClases(lista_clases);//seteo las clases
            Control3d.setListaMetodos(metodos);//seteo los metodos para traducirlos
            generacion_3d_olc gen = new generacion_3d_olc();
            //ejecutar(raiz);
        }

        #endregion

        public void SentenciasGlobales(ParseTreeNode raiz)
        {
            switch (raiz.Term.Name)
            {
                case "INICIO":
                    foreach (ParseTreeNode a in raiz.ChildNodes)
                        SentenciasGlobales(a);
                    break;
                case "SENTENCIAS":
                    foreach (ParseTreeNode a in raiz.ChildNodes)
                        SentenciasGlobales(a);
                    break;
                case "CLASE":
                    guardarClase(raiz);
                    break;
                case "LLAMAR":
                    ejecutarLlamar(raiz);
                    break;
            }
        }

        private void ejecutarLlamar(ParseTreeNode raiz)
        {
            Console.WriteLine(raiz.Term.Name);
            string nombre_archivo = raiz.ChildNodes[0].Token.Text.Replace("\"", "").Replace(".olc","");

            if (!lista_clases.ContainsKey(nombre_archivo))
            {
                string ruta = Control3d.getRuta() + nombre_archivo+".olc";
                string cont = retornarContenido(ruta);

                if (cont != "")
                {
                    //ejecutar el parse y retonar y NodoArbol
                    ParseTreeNode aux = retonarRaizOLC(cont,nombre_archivo);
                    if (aux != null)
                        SentenciasGlobales(aux);
                    else
                        Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "ERROR AL ANALIZAR EL CONTENIDO DEL ARCHIVO: " + nombre_archivo));
                }
            }
            else
                Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "Ya se importo un archivo OLC con nombre:  " + nombre_archivo));
        }

        public string retornarContenido(string ruta)
        {
            String contenido = "";
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(ruta, System.Text.Encoding.UTF8);
                //System.IO.StreamReader sr = new System.IO.StreamReader(abrir.FileName, System.Text.Encoding.Default);
                contenido = sr.ReadToEnd();
                sr.Close();
            }
            catch
            {
                MessageBox.Show("ERROR al abrir el archivo");
            }
            return contenido;
        }

        public ParseTreeNode retonarRaizOLC(string cont,string nombre_archivo)
        {
            Gramatica gramatica = new Gramatica();
            Parser parser = new Parser(gramatica);

            ParseTree arbol = parser.Parse(cont);
            ParseTreeNode raiz = arbol.Root;

            if (raiz == null || arbol.ParserMessages.Count > 0 || arbol.HasErrors())
            {
                //---------------------> Hay Errores      
                MessageBox.Show("Hay Errores");
                errores.Append("hay errores:\n");
                foreach (var item in arbol.ParserMessages)
                {
                    errores.Append(item.Location.Line + " ");
                    errores.Append(item.Location.Column + " ");
                    errores.Append(item.Message + "\n");
                }
                return null;
            }
            return raiz;
        }

        private void guardarClase(ParseTreeNode raiz)
        {
            //tendria que guardar el nombre de la clase y su visibilidad o algo asi
            string nombre = raiz.ChildNodes[1].Token.Text.Replace(".olc","");
            string visibilidad = raiz.ChildNodes[0].ChildNodes[0]. Term.Name;
            if (!lista_clases.ContainsKey(nombre))
            {
                objeto_clase nuevo = new objeto_clase(nombre);
                lista_clases.Add(nombre,nuevo);
                metodos = nuevo.metodos;
                listaActual = nuevo.variables;
                constructores = nuevo.constructores;
                //y tambien el constructor;
                //si tiene hereda mandar a ejecutar ese nodo y regresar a ejecutar las sentencias
                if (raiz.ChildNodes[2].ChildNodes.Count > 0)
                    ejecutarHeredar(raiz.ChildNodes[2]);
                foreach (ParseTreeNode a in raiz.ChildNodes[3].ChildNodes)
                    sentencias_clase(a,nombre);
            }
            
        }

        private void ejecutarHeredar(ParseTreeNode raiz)
        {
            string ruta = "";
            string nombre = raiz.ChildNodes[0].Token.Text;
            ruta = Control3d.getRuta() + nombre + ".olc";
            string contenido = retornarContenido(ruta);
            if (contenido != "")
            {
                ParseTreeNode raiz_aux = retonarRaizOLC(contenido, nombre);
                if (raiz_aux != null)
                {
                    raiz_aux = raiz_aux.ChildNodes[0];
                    foreach(ParseTreeNode r in raiz_aux.ChildNodes)
                    {
                        try
                        {
                            if (r.Term.Name.Equals("CLASE"))
                            {
                                foreach (ParseTreeNode aux2 in r.ChildNodes[3].ChildNodes)
                                    sentencias_clase(aux2, nombre);
                                break;
                            }
                        }
                        catch
                        {
                            Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "Error en la clase a heredar:  " + nombre+", revise el contenido de la clase"));
                            return;
                        }
                    }
                }
                else
                    Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "Error en la clase a heredar:  " + nombre));
            }
            else
                Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "Error al leer el archivo que contiene la clase a heredar:  " + nombre));
        }

        private void sentencias_clase(ParseTreeNode raiz,string nombre_clase)
        {

            //verificar el constructor de la clase
            //verificar las instancias declaracion y creacion
            //verificar los arreglos
            string visibilidad = "publico";
            string nombre="";
            string tipo="";

            if (raiz.ChildNodes[0].Term.Name.Equals("OVERRIDE"))
            {
                ejecutarOVERRIDE(raiz.ChildNodes[0]);
                return;
            }

            if (raiz.ChildNodes[0].Term.Name.Equals("MAIN")){
                guardarMain(raiz.ChildNodes[0]);
                return;
            }
            if (raiz.ChildNodes[0].ChildNodes.Count > 0)
                visibilidad = raiz.ChildNodes[0].ChildNodes[0].Token.Text;
            if (raiz.ChildNodes[1].Term.Name.Equals("CONSTRUCTOR"))
            {
                guardarConstructor(visibilidad, raiz.ChildNodes[1],nombre_clase);
                return;
            }
            if (raiz.ChildNodes[1].ChildNodes[0].ChildNodes.Count > 0) 
                tipo = raiz.ChildNodes[1].ChildNodes[0].ChildNodes[0].Token.Text;
            else
                tipo = "vacio";//me falta probar esto

            if(raiz.ChildNodes.Count>2)
                nombre = raiz.ChildNodes[2].Token.Text;

            if (raiz.ChildNodes[3].ChildNodes.Count > 0)
            {
                ParseTreeNode aux = raiz.ChildNodes[3].ChildNodes[0];
                if (aux.Term.Name.Equals("METODO"))
                {
                    guardar_metodo(tipo, nombre, visibilidad, aux);
                }else if (aux.Term.Name.Equals("L_ARRAY"))
                {
                    guardarArreglo(visibilidad, tipo, nombre, raiz.ChildNodes[3]);
                }else if (aux.Term.Name.Equals("new"))
                {
                    guardarInstancia(visibilidad, tipo, nombre, raiz.ChildNodes[3]);
                }
                else//seria una declaracion asignacion
                {
                    Variable var = new Variable(visibilidad, tipo, nombre, raiz.ChildNodes[3].ChildNodes[0]);
                    guardarVariable(var, raiz.Span.Location.Line, raiz.Span.Location.Column);
                }
            }else
            {
                Variable var = new Variable(visibilidad, tipo, nombre, null);
                guardarVariable(var,raiz.Span.Location.Line,raiz.Span.Location.Column);
            }
        }

        private void guardarMain(ParseTreeNode raiz)
        {
            Console.WriteLine("J");
            metodo nuevo = new metodo("publico", "vacio", "PRINCIPAL", raiz.ChildNodes[0], 0);
            metodos.AddFirst(nuevo);
        }

        private void ejecutarOVERRIDE(ParseTreeNode raiz)
        {
            string visibilidad = "publico";
            string nombre = "";
            string tipo = "";


            if (raiz.ChildNodes[0].ChildNodes.Count > 0)
                visibilidad = raiz.ChildNodes[0].ChildNodes[0].Token.Text;

            if (raiz.ChildNodes[1].ChildNodes[0].ChildNodes.Count > 0)
                tipo = raiz.ChildNodes[1].ChildNodes[0].ChildNodes[0].Token.Text;
            else
                tipo = "vacio";//me falta probar esto
            ParseTreeNode aux = raiz.ChildNodes[3].ChildNodes[0];
            if (raiz.ChildNodes.Count > 2)
                nombre = raiz.ChildNodes[2].Token.Text;
            guardar_override(tipo, nombre, visibilidad, aux);
        }

        private void guardarConstructor(string visibilidad, ParseTreeNode raiz,string clase)
        {
            string id = raiz.ChildNodes[0].Token.Text;
            if (!id.Equals(clase))
            {
                Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "Error en el nombre del constructor en la clase: " + clase));
                return;
            }
            if (raiz.ChildNodes.Count == 3)
                guardar_constructor_parametros(visibilidad, "CONSTRUCTOR", clase, raiz.ChildNodes[1], raiz.ChildNodes[2]);
            else
            {
                foreach(metodo a in constructores)
                {
                    if (a.parametros.ChildNodes.Count== 0)
                    {
                        Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "Ya existe constructor definido sin parametros en la clase: " + clase));
                        return;
                    }
                }
                constructores.AddLast(new metodo(visibilidad, "CONSTRUCTOR", id, raiz.ChildNodes[1], constructores.Count));
            }
            //comparar de la misma manera que con los metodos
            //solo que con un par de cambios como el tipo y solo ver los numeros de parametros
            //:D
        }

        private void guardarInstancia(string visibilidad, string tipo, string nombre, ParseTreeNode raiz)
        {
            //aqui tendria que ver si la clase existe,
            //si tiene constructor 
            //si el constructor acepta parametros
            if (lista_clases.ContainsKey(tipo))
            {
                objeto_clase clase;
                lista_clases.TryGetValue(tipo, out clase);
                if (clase.constructores.Count > 0)
                {
                    //comprobar los constructores con los parametros que se envian

                    //suponiendo que todo va bien
                    if (comprobar_constructores(clase.constructores, tipo, nombre, raiz))
                    {
                        Variable var = new Variable(visibilidad, tipo, nombre, raiz);
                        guardarVariable(var, raiz.Span.Location.Line, raiz.Span.Location.Column);
                    }else
                    {
                        Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "No existe constructor que acepte el numero de parametros:" + nombre));
                        return;
                    }
                }else
                {
                    //agregar error que no se puede instanciar la clase porque no tiene constructores
                    Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "No se puede instanciar la variable:  " + nombre +", ya que la clase no tiene constructor"));
                    return;
                }
                //primero obtener la clase
                //ver si tiene constructor
                //comprobar si traer parametros el constructor
                //ejecutar las sentencias del constructor quizas....
                //quizas los tipos que se le envian o alguasil
            }

        }

        private bool comprobar_constructores(LinkedList<metodo> constructores, string tipo, string nombre, ParseTreeNode raiz)
        {
            //verificar que sean los mismos tipos
            if (!tipo.Equals(raiz.ChildNodes[1].Token.Text))
            {
                Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "Error al instanciar la variable:" + nombre+", no coinciden los tipos"));
                return false;
            }

            if (raiz.ChildNodes[2].ChildNodes.Count == 0)
            {
                foreach (metodo a in constructores)
                    if (a.parametros.ChildNodes.Count ==0)
                        return true;
            }else
            {
                ParseTreeNode parametros = raiz.ChildNodes[2].ChildNodes[0];
                foreach(metodo a in constructores)
                    if (a.parametros.ChildNodes.Count == parametros.ChildNodes.Count)
                    return true;
            }

            return false;
        }

        private void guardarArreglo(string visibilidad, string tipo, string nombre, ParseTreeNode aux)
        {
            LinkedList<int> lista = new LinkedList<int>();

            foreach(ParseTreeNode a in aux.ChildNodes[0].ChildNodes)
            {
                int dim;
                try
                {
                    dim = Int32.Parse(a.ChildNodes[0].Token.Text);
                    lista.AddLast(dim);
                }
                catch (FormatException)
                {
                    Control3d.agregarError(new errores("semantico", a.Span.Location.Line, a.Span.Location.Column, "Valor incorrecto en declaracion de array"));
                    return;
                    // Return? Loop round? Whatever.
                }
                
            }
            Variable var = new Variable(visibilidad, tipo, nombre, aux.ChildNodes[1], lista);
            guardarVariable(var, aux.Span.Location.Line, aux.Span.Location.Column);
        }

        #region guadarMetodos
        private void guardar_metodo(string tipo, string nombre, string visilidad, ParseTreeNode metodo)
        {
            int no = 0;
            if (metodo.ChildNodes.Count > 1)
                guardar_metodo_parametros(tipo, nombre, visilidad, metodo);
            else
            {
                foreach (metodo a in metodos)
                {
                    if (a.nombre.Equals(nombre))
                    {
                        no++;
                        if (a.parametros.ChildNodes.Count==0)
                        {
                            Control3d.agregarError(new errores("semantico", metodo.Span.Location.Line, metodo.Span.Location.Column, "Ya existe el metodo: " + nombre));
                            return;
                        }
                    }
                }
                metodos.AddLast(new metodo(visilidad, tipo, nombre, metodo.ChildNodes[0],no));
            }
        }

        private void guardar_metodo_parametros(string tipo, string nombre, string visilidad, ParseTreeNode metodo)
        {
            LinkedList<metodo> aux = new LinkedList<metodo>();
            int no = 0;
            foreach (metodo a in metodos)
            {
                if (a.nombre.Equals(nombre))
                {
                    no++;
                    if (a.tipo.Equals(tipo))
                        if (a.parametros.ChildNodes.Count >0)
                            if (a.parametros.ChildNodes.Count == metodo.ChildNodes[0].ChildNodes.Count)
                                aux.AddLast(a);
                }
            }

            if (aux.Count > 0)
            {
                ParseTreeNode parametros = metodo.ChildNodes[0];

                foreach (metodo a in aux)
                {
                    Boolean igual = true;
                    for (int i = 0; i < a.parametros.ChildNodes.Count; i++)
                    {
                        ParseTreeNode param1 = a.parametros.ChildNodes[i];
                        ParseTreeNode param2 = parametros.ChildNodes[i];
                        string tipo1 = param1.ChildNodes[0].ChildNodes[0].Token.Text;
                        string tipo2 = param2.ChildNodes[0].ChildNodes[0].Token.Text;
                        if (tipo1 != tipo2)
                        { igual = false; break; }
                    }
                    if (igual)
                    {
                        Control3d.agregarError(new errores("semantico", metodo.Span.Location.Line, metodo.Span.Location.Column, "Ya existe el metodo: " + nombre));
                        return;
                    }
                }
                //agrego el metodo porque no existe
                metodos.AddLast(new metodo(visilidad, tipo, nombre, metodo.ChildNodes[1], metodo.ChildNodes[0],no));
            }
            else
            {
                //agrego el metodo porque no existe
                metodos.AddLast(new metodo(visilidad,tipo, nombre, metodo.ChildNodes[1], metodo.ChildNodes[0],no));
            }
        }

        #endregion

        private void iniciar()
        {
            foreach(Variable a in listaActual)
            {
                if (a.casilla != null)
                {
                    int tam = 1;
                    foreach (int x in a.casilla)
                        tam *= x;
                    nodoTabla nuevo = new nodoTabla(a.visibilidad, a.tipo, a.nombre, "var_array", tabla.posGlobal,tam , "Global",a.casilla,a.valor);
                    tabla.AddLast(nuevo);
                    tabla.posGlobal+=tam;//me falta cuando sea
                }
                else
                {
                    nodoTabla nuevo = new nodoTabla(a.visibilidad, a.tipo, a.nombre, "var", tabla.posGlobal, 1, "Global");
                    tabla.AddLast(nuevo);
                    tabla.posGlobal++;//me falta cuando sea
                }
                
            }//se guardaron las variables globales

            //vamos a guardar los metodos
            foreach(metodo a in metodos)
            {
                nodoTabla nuevo = new nodoTabla(a.visibilidad, a.tipo, a.nombre, "METODO", -1, 0, "Global");
                tabla.AddLast(nuevo);
                int posActual = tabla.Count - 1;

                nuevo.setNoMetodo(a.noMetodo);
                aumentarAmbito(a.nombre);//ver si ocupo concatenarle el _noMetodo
                posicion.AddFirst(0);
                if(a.nombre.ToUpper().Equals("PRINCIPAL"))
                    ejecutar(a.sentencia, a.nombre);//ejecutamos las sentencias
                else
                    ejecutar(a.sentencia, a.nombre + "_" + a.noMetodo);//ejecutamos las sentencias
                posicion.RemoveFirst();
                //recorrer para guardar guardar el tamanio

                int tam = 0;
                for (; posActual < tabla.Count; posActual++)
                    tam += tabla.ElementAt(posActual).tam;
                nuevo.tam = tam;
                disminuirAmbito();
            }
        }

        private Boolean guardarVariable(Variable v, int linea, int columna)
        {
            foreach(Variable a in listaActual)
            {
                if (a.nombre.Equals(v.nombre))
                {
                    Control3d.agregarError(new errores("semantico", linea, columna, "Ya existe variable " + v.nombre + " en el mismo ambito"));
                    return false;
                }
            }

            listaActual.AddLast(v);
            return true;
        }

        private void aumentarAmbito(String ambito)
        {
            ambitos nuevo = new Compilador.ambitos(ambito);
            ambitos.AddFirst(nuevo);
            listaActual = nuevo;
            
        }

        private void disminuirAmbito()
        {
            //se manda a guardar la lista de variables
            ambitos.RemoveFirst();
            listaActual = ambitos.First();
            //posicion.RemoveFirst();
        }

      #region "Ejecucion de Sentencias"

        private void ejecutar(ParseTreeNode nodo,string ambito)
        {
            switch (nodo.Term.Name)
            {
                case "SENTENCIAS":
                    foreach (var item in nodo.ChildNodes)
                        ejecutar(item,ambito);
                    break;
                case "DECLARAR":
                    ejecutarDECLARAR(nodo,ambito);
                    break;
                case "DECLARAR_ASIG":
                    ejecutarDECLARAR_ASIGNAR(nodo,ambito);
                    break;
                case "IF":
                    ejecutarIF1(nodo,ambito);
                    break;
               case "WHILE":
                    ejecutarWHILE(nodo, ambito);
                    break;
                case "DOWHILE":
                    ejecutarDO_WHILE(nodo, ambito);
                    break;
                case "REPEAT":
                    ejecutarREPEAT(nodo, ambito);
                    break;
                case "FOR":
                    ejecutarFOR(nodo, ambito);
                    //guardar las variables del for
                    break;
                case "WHILEX":
                    ejecutarWHILEX(nodo, ambito);
                    //guardar las variables del whilex
                    break;
                default:
                    break;
            }
        }

        private void ejecutarWHILEX(ParseTreeNode nodo, string ambito)
        {
            MessageBox.Show("Hay que revisar esto del whilex");
            string nuevo_ambito = ambito + "_whilex" + listaActual.noX++;
            aumentarAmbito(nuevo_ambito);
            ejecutar(nodo.ChildNodes[2], nuevo_ambito);
            disminuirAmbito();
        }

        private void ejecutarDECLARAR(ParseTreeNode nodo,String ambito)
        {
            String nombre = nodo.ChildNodes[1].ChildNodes[0].Token.Text;
            String visibilidad = "local";
            /*if (nodo.ChildNodes[0].ChildNodes[0].Term.Name!=null)
                visibilidad = nodo.ChildNodes[0].ChildNodes[0].Term.Name;*/
            string tipo = nodo.ChildNodes[0].ChildNodes[0].Term.Name;
            Variable nueva_variable = new Variable(nombre);
            Boolean a = guardarVariable(nueva_variable,nodo.Span.Location.Line, nodo.Span.Location.Column);
            if (a)
            {
                nodoTabla nuevo = new nodoTabla(visibilidad, tipo, nombre, "var", posicion.First(), 1, ambito);
                tabla.AddLast(nuevo);
                int x = posicion.First();
                x++;
                posicion.RemoveFirst();
                posicion.AddFirst(x);
            }
        }

        private void ejecutarDECLARAR_ASIGNAR(ParseTreeNode nodo,string ambito)
        {
            String nombre = nodo.ChildNodes[1].ChildNodes[0].Token.Text;
            string tipo = nodo.ChildNodes[0].ChildNodes[0].Term.Name;
            Variable nueva_variable = new Variable(nombre);
            Boolean a = guardarVariable(nueva_variable, nodo.Span.Location.Line, nodo.Span.Location.Column);
            if (a)
            {
                nodoTabla nuevo = new nodoTabla("local", tipo, nombre, "var", posicion.First(), 1, ambito);
                nuevo.setExp(nodo.ChildNodes[2]);
                tabla.AddLast(nuevo);
                int x = posicion.First();
                x++;
                posicion.RemoveFirst();
                posicion.AddFirst(x);
            }
            // Variable nueva_variable = new Variable(nombre, valor);
            // guardarVariable(nueva_variable);
        }

        
        #endregion

        /****************************************************************************************/
        /****************************************************************************************/

        #region "Ejecucion de flujos de control"

        private void ejecutarIF1(ParseTreeNode nodo,string ambito)
        {
            //---------------------> Hijo 0, la condicion
            //---------------------> Hijo 1, la accion 
            if (nodo.ChildNodes.Count == 3)
            {//if solo
                string nuevo_ambito = ambito + "_if" + listaActual.noIf++;
                aumentarAmbito(nuevo_ambito);
                ejecutar(nodo.ChildNodes[1], nuevo_ambito);
                disminuirAmbito();
                if (nodo.ChildNodes[2].ChildNodes.Count > 0)
                {
                    nuevo_ambito = ambito + "_if" + listaActual.noIf+"_else";
                    aumentarAmbito(nuevo_ambito);
                    ejecutar(nodo.ChildNodes[2].ChildNodes[0], nuevo_ambito);
                    disminuirAmbito();
                }

                
            }else 
            {
                string nuevo_ambito = ambito + "_if" + listaActual.noIf++;
                aumentarAmbito(nuevo_ambito);
                ejecutar(nodo.ChildNodes[1], nuevo_ambito);
                disminuirAmbito();

                foreach(ParseTreeNode p in nodo.ChildNodes[2].ChildNodes)
                {
                    nuevo_ambito = ambito + "_if" + listaActual.noIf+"_elseif"+listaActual.no_else_if++;
                    aumentarAmbito(nuevo_ambito);
                    ejecutar(p.ChildNodes[1], nuevo_ambito);
                    disminuirAmbito();
                }

                if (nodo.ChildNodes[3].ChildNodes.Count > 0)
                {
                    nuevo_ambito = ambito + "_if" + listaActual.noIf + "_else";
                    aumentarAmbito(nuevo_ambito);
                    ejecutar(nodo.ChildNodes[3].ChildNodes[0], nuevo_ambito);
                    disminuirAmbito();
                }
            }
        }
       
        private void ejecutarSWITCH(ParseTreeNode nodo)
        {
            //---------------------> Hijo 0, valor a comparar
            //---------------------> Hijo 1, lista de casos en donde se compara valores
            //---------------------> Hijo 2, defecto si ningun caso dio correcto, se ejecuta este
           /* Object valor = this.evaluarEXPRESION(nodo.ChildNodes[0]);

            Boolean entro = false;
            foreach (case item in nodo.ChildNodes[1].ChildNodes)
            {
                //---------------------> Hijo 0 de item, valor2                
                Object valor2 = this.evaluarEXPRESION(item.ChildNodes[0]);
                if(valor.Equals(valor2))
                {
                    this.aumentarAmbito();
                    //---------------------> Hijo 1 de item, accion si valor1 == valor2
                    this.ejecutar(item.ChildNodes[1]);
                    this.disminuirAmbito();
                    entro = true;
                    break;
                }
            }
            //programar el default
            if(!entro)
            {
                this.aumentarAmbito();
                this.ejecutar(nodo.ChildNodes[2].ChildNodes[0]);
                this.disminuirAmbito();
            }*/

        }

        private void ejecutarWHILE(ParseTreeNode nodo,string ambito)
        {
            string nuevo_ambito = ambito + "_while" + listaActual.noWhile++ ;
            aumentarAmbito(nuevo_ambito);
            ejecutar(nodo.ChildNodes[1], nuevo_ambito);
            disminuirAmbito();
            
        }

        private void ejecutarDO_WHILE(ParseTreeNode nodo,string ambito)
        {

            string nuevo_ambito = ambito + "_doWhile" + listaActual.doWhile++ ;
            aumentarAmbito(nuevo_ambito);
            ejecutar(nodo.ChildNodes[0], nuevo_ambito);
            disminuirAmbito();
        }

        private void ejecutarREPEAT(ParseTreeNode nodo,string ambito)
        {

            string nuevo_ambito = ambito + "_repeat" + listaActual.noUntil++;
            aumentarAmbito(nuevo_ambito);
            ejecutar(nodo.ChildNodes[0], nuevo_ambito);
            disminuirAmbito();
        }

        private void ejecutarFOR(ParseTreeNode nodo, string ambito)
        {
            string nuevo_ambito = ambito + "_for" + listaActual.noFor++;
            aumentarAmbito(nuevo_ambito);
            ejecutar(nodo.ChildNodes[0], nuevo_ambito);
            ejecutar(nodo.ChildNodes[3], nuevo_ambito);
            disminuirAmbito();
        }

        #endregion

        private void mostrarTablaSimbolos()
        {
            int dimCol = 0;
            String pagina = "";
            int counter = 0;
            pagina += "<HTML>";
            pagina+="<link href = 'table.css' rel = 'stylesheet' />";
               pagina += "<center>";
            pagina += "<h1> TABLA DE SIMBOLOS </H1>";
            pagina += "<BODY>";
            pagina += "<TABLE BORDER = 1 class='responstable'>";
            pagina += "<THEAD>";
            pagina += "<tr>";
            pagina += " <th>No</th>";
            pagina += " <th>AMBITO</th>";
            pagina += " <th>NOMBRE</th>";
            pagina += " <th>TIPO</th>";
            pagina += " <th>ROL</th>";
            pagina += " <th>POS</th>";
            pagina += " <th>TAMANIO</th>";


            pagina += "</tr>";
            pagina += "</THEAD>";

            foreach (var item in tabla)
            {
                pagina += "<tr>";
                pagina += "<td>" + counter + "</td>";
                pagina += "<td>" + item.ambito + "</td>";
                if (item.rol.Equals("METODO"))
                    pagina += "<td>" + item.nombre+"_"+item.noMetodo + "</td>";
                else
                    pagina += "<td>" + item.nombre + "</td>";
                pagina += "<td>" + item.tipo + "</td>";
                pagina += "<td>" + item.rol + "</td>";
                //if (item.rol.ToLower() == "metodo" || item.tipo == "funcion")
                    pagina += "<td>" + item.pos + "</td>";
                //else
                  //  pagina += "<td bgcolor=\"#f65820\">" + item.pos + "</td>";
                pagina += "<td>" + item.tam + "</td>";

                /* if (item.rol == "array")
                 {
                     pagina += "<th>";
                     foreach(case vari in item.dimensiones.dim)
                     {
                         pagina += "<th>" + vari.posInf + "</th>";
                         pagina += "<th>" + vari.posSup + "</th>";
                     }
                     pagina += "<th>";
                 }*/
                pagina += "</tr>";
                dimCol += 3;
                counter++;
            }

            pagina += "</TABLE>";
            pagina += "</BODY>";
            pagina += "</center>";
            pagina += "</HTML>";



            System.IO.File.WriteAllText(@"C:\compiladores\tablaSym.html", pagina);
            Graficador g = new Graficador();
            g.abrirArbol(@"C:\compiladores\tablaSym.html");
        }

        private int tipoObjecto(Object var)
        {
            int reto = 0;
            if (var.GetType() == Type.GetType("System.Int32")) { reto = 1; }
            else if (var.GetType() == Type.GetType("System.Double")) { reto = 2; }
            else if (var.GetType() == Type.GetType("System.String")) { reto = 3; }
            else if (var.GetType() == Type.GetType("System.Char")) { reto = 4; }
            else if (var.GetType() == Type.GetType("System.Boolean")) { reto = 5; }
            return reto;
        }

        #region GUARDAR_CONSTRUCTORES
        private void guardar_constructor_parametros(string visibilidad,string tipo,string nombre,ParseTreeNode parametros, ParseTreeNode sentencias)
        {
            foreach (metodo a in constructores)
                {
                if (a.parametros.ChildNodes.Count == parametros.ChildNodes.Count)
                {
                    Boolean igual = true;
                    for (int i = 0; i < a.parametros.ChildNodes.Count; i++)
                    {
                        ParseTreeNode param1 = a.parametros.ChildNodes[i];
                        ParseTreeNode param2 = parametros.ChildNodes[i];
                        string tipo1 = param1.ChildNodes[0].ChildNodes[0].Token.Text;
                        string tipo2 = param2.ChildNodes[0].ChildNodes[0].Token.Text;
                        if (tipo1 != tipo2)
                        { igual = false; break; }
                    }
                    if (igual)
                    {
                        Control3d.agregarError(new errores("semantico", parametros.Span.Location.Line, parametros.Span.Location.Column, "Ya existe el constructor con los mismos parametros en la clase: " + nombre));
                        return;
                    }
                }
              }
          constructores.AddLast(new metodo(visibilidad, tipo, nombre, sentencias, parametros, constructores.Count));
        }
        #endregion

        #region guardar_OVERRIDE

        private void guardar_override(string tipo, string nombre, string visilidad, ParseTreeNode metodo)
        {
            int no = 0;
            if (metodo.ChildNodes.Count > 1)
                guardar_override_parametros(tipo, nombre, visilidad, metodo);
            else
            {
                foreach (metodo a in metodos)
                {
                    if (a.nombre.Equals(nombre))
                    {
                        no++;
                        if (a.parametros.ChildNodes.Count == 0)
                        {
                            metodos.Remove(a);
                            metodos.AddLast(new metodo(visilidad, tipo, nombre, metodo.ChildNodes[0], no));
                            //y aqui pues podria ocultar o etc etc
                            return;
                        }
                    }
                }
                Control3d.agregarError(new errores("semantico", metodo.Span.Location.Line, metodo.Span.Location.Column, "Ya existe el metodo: " + nombre));
            }
        }

        private void guardar_override_parametros(string tipo, string nombre, string visilidad, ParseTreeNode metodo)
        {
            LinkedList<metodo> aux = new LinkedList<metodo>();
            int no = 0;
            foreach (metodo a in metodos)
            {
                if (a.nombre.Equals(nombre))
                {
                    no++;
                    if (a.tipo.Equals(tipo))
                        if (a.parametros.ChildNodes.Count > 0)
                            if (a.parametros.ChildNodes.Count == metodo.ChildNodes[0].ChildNodes.Count)
                                aux.AddLast(a);
                }
            }

            if (aux.Count > 0)
            {
                ParseTreeNode parametros = metodo.ChildNodes[0];

                foreach (metodo a in aux)
                {
                    Boolean igual = true;
                    for (int i = 0; i < a.parametros.ChildNodes.Count; i++)
                    {
                        ParseTreeNode param1 = a.parametros.ChildNodes[i];
                        ParseTreeNode param2 = parametros.ChildNodes[i];
                        string tipo1 = param1.ChildNodes[0].ChildNodes[0].Token.Text;
                        string tipo2 = param2.ChildNodes[0].ChildNodes[0].Token.Text;
                        if (tipo1 != tipo2)
                        { igual = false; break; }
                    }
                    if (igual)
                    {
                        metodos.Remove(a);
                        metodos.AddLast(new metodo(visilidad, tipo, nombre, metodo.ChildNodes[1], metodo.ChildNodes[0], no));
                        return;
                    }
                }
                //agrego el metodo porque no existe
                // metodos.AddLast(new metodo(visilidad, tipo, nombre, metodo.ChildNodes[1], metodo.ChildNodes[0], no));
            }/*
            else
            {
                //agrego el metodo porque no existe
                metodos.AddLast(new metodo(visilidad, tipo, nombre, metodo.ChildNodes[1], metodo.ChildNodes[0], no));
            }*/
            Control3d.agregarError(new errores("semantico", metodo.Span.Location.Line, metodo.Span.Location.Column, "No existe el metodo para sobreescribir: " + nombre));

        }

        #endregion


    }
}

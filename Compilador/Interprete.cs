﻿using System;
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
        private objeto_clase clase_actual;
        public Boolean es_heredada = false;
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
            //Graficador g = new Graficador();
            // g.graficar(arbol);
            SentenciasGlobales(raiz);
            startin();
        }

        private void startin()
        {
            
            //iniciar();
            iniciar_traduccion_clases();

            mostrarTablaSimbolos();
            Control3d.setListaClases(lista_clases);//seteo las clases
            Control3d.setListaMetodos(metodos);//seteo los metodos para traducirlos
            Control3d.set_clase_actual(clase_actual);
            generacion_3d_olc gen = new generacion_3d_olc();
        }

        public void analizar_TREE(String entrada)
        {
            GramaticaTre gramatica = new GramaticaTre();
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
             Graficador g = new Graficador();
             g.graficar(arbol);
             SentenciasGlobales(raiz);
            startin();
        }

        
        internal void solo_arbol(string contenido)
        {
            Gramatica gramatica = new Gramatica();
            Parser parser = new Parser(gramatica);

            ParseTree arbol = parser.Parse(contenido);
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

            Graficador g = new Graficador();
            g.graficar(arbol);
        }

        private void iniciar_traduccion_clases()
        {
            traducir_clases(clase_actual,true);
            foreach (KeyValuePair<string, objeto_clase> Par in lista_clases)
            {
                if (Par.Value == clase_actual)
                    continue;
                string clase = Par.Key;
                objeto_clase aux = Par.Value;
                traducir_clases(aux,false);
            }
        }

        #endregion




        private void traducir_clases(objeto_clase aux, Boolean main)
        {
            nodoTabla nuevo = new nodoTabla(aux.visibilidad, "CLASE", aux.nombre, "CLASE", -1, 0, "Global");
            tabla.AddLast(nuevo);

            //variable
            #region "TRADUCIR LAS VARIABLE GLOBALES DE LAS CLASES"
            int pos = 0;
            //agregamos la referencia del this y del super
            nodoTabla este = new nodoTabla("privado", "THIS", "this", "referencia", 0, 1, aux.nombre + "_Global");
            tabla.AddLast(este);
            nodoTabla super = new nodoTabla("privado", "SUPER", "super", "referencia", 1, 1, aux.nombre + "_Global");
            tabla.AddLast(super);

            /*antiguo*/ //int posActual = tabla.Count - 1;
            int posActual = tabla.Count;/*antiguo*/

            foreach (Variable a in aux.variables)
            {
                int tama = 1;
                if (a.casilla != null)
                {
                    foreach (int x in a.casilla)
                        tama *= x;
                    nodoTabla variable = new nodoTabla(a.visibilidad, a.tipo, a.nombre, "var_array", pos++, tama, aux.nombre + "_Global", a.casilla, a.valor);
                    tabla.AddLast(variable);
                    variable.es_heredada = a.es_heredada;
                    variable.set_tam_oculto(tama);
                    
                    //tabla.posGlobal += tam;//me falta cuando sea
                }
                else
                {
                    nodoTabla variable = new nodoTabla(a.visibilidad, a.tipo, a.nombre, "var", pos++, 1, aux.nombre + "_Global");
                    tabla.AddLast(variable);
                    variable.setExp(a.valor);
                    //tabla.posGlobal++;//me falta cuando sea
                }
            }

            int tam = 0;
            for (; posActual < tabla.Count; posActual++)
            {
                int tam_aux = tabla.ElementAt(posActual).tam;
                if (tabla.ElementAt(posActual).rol.Contains("array"))
                    tam_aux = 1;
                //tam += tabla.ElementAt(posActual).tam;
                tam += tam_aux;
            }
            nuevo.tam = tam;
            #endregion
            //los constructores
            #region "TRADUCCION DE LOS CONSTRUCTORES"
            foreach (metodo a in aux.constructores)
            {
                //nuevo = new nodoTabla(a.visibilidad, a.tipo, aux.nombre + "_" + a.nombre + "_" + a.noMetodo, "CONSTRUCTOR", -1, 0, aux.nombre + "_Global");
                nuevo = new nodoTabla(a.visibilidad, a.tipo, aux.nombre + "_Init_" + a.noMetodo, "CONSTRUCTOR", -1, 0, aux.nombre + "_Global");
                //
                tabla.AddLast(nuevo);
                posActual = tabla.Count;// - 1;

                nuevo.setNoMetodo(a.noMetodo);
                //aumentarAmbito(aux.nombre + "_" + a.nombre + "_" + a.noMetodo);//ver si ocupo concatenarle el _noMetodo
                aumentarAmbito(aux.nombre + "_Init_" + a.noMetodo);//ver si ocupo concatenarle el _noMetodo
                este = new nodoTabla("privado", "THIS", "this", "referencia", 0, 1, aux.nombre + "_Init_" + a.noMetodo);
                tabla.AddLast(este);
                super = new nodoTabla("privado", "SUPER", "super", "referencia", 1, 1, aux.nombre + "_Init_" + a.noMetodo);
                tabla.AddLast(super);


                //en la posicion 0 del constructor va a ir la referencia
                //de la variable creada en el heap
                //entonces si se declaran mas variables dentro del constructor
                //no afectaran la referencia
                posicion.AddFirst(2);
                //GUARDO LOS PARAMETROS
                foreach (ParseTreeNode p in a.parametros.ChildNodes)
                {
                    string nombre = p.ChildNodes[1].Token.Text;
                    string tipo = p.ChildNodes[0].ChildNodes[0].Token.Text;
                    nodoTabla parametro = new nodoTabla("privado", tipo, nombre, "PARAMETRO", posicion.First(), 1, aux.nombre + "_Init_" + a.noMetodo);
                    tabla.AddLast(parametro);
                    int x = posicion.First();
                    posicion.RemoveFirst();
                    posicion.AddFirst(++x);
                }


                if (!a.nombre.ToUpper().Equals("PRINCIPAL"))
                    ejecutar(a.sentencia, aux.nombre + "_Init_" + a.noMetodo);//ejecutamos las sentencias

                posicion.RemoveFirst();
                //recorrer para guardar guardar el tamanio
                tam = 0;
                for (; posActual < tabla.Count; posActual++)
                    tam += tabla.ElementAt(posActual).tam;
                nuevo.tam = tam;
                disminuirAmbito();
            }
            #endregion
            //los metodos/funciones
            #region "TRADUCCION DE LOS METODOS"
            foreach (metodo a in aux.metodos)
            {
                //para agregarlos a tabla de simbolos les voy a poner nombreclase_nombremetodo
                //nuevo = new nodoTabla(a.visibilidad, a.tipo, a.nombre, "METODO", -1, 0, aux.nombre + "_Global");
                //ver si ocupo concatenarle el _noMetodo
                if (a.nombre.ToUpper().Equals("PRINCIPAL") && !main)
                    continue;
                if (a.nombre.ToUpper().Equals("PRINCIPAL") && main)
                    nuevo = new nodoTabla(a.visibilidad, a.tipo, a.nombre, "METODO", -1, 0, aux.nombre + "_Global");
                else
                {
                    string met = "METODO";
                    if (a.tipo != "vacio")
                        met = "FUNCION";
                    nuevo = new nodoTabla(a.visibilidad, a.tipo, aux.nombre + "_" + a.nombre, met, -1, 0, aux.nombre + "_Global");
                }
                tabla.AddLast(nuevo);
                posActual = tabla.Count;// - 1;
                //aumentarAmbito(aux.nombre + "_" + a.nombre);
                nuevo.setNoMetodo(a.noMetodo);
                

                este = new nodoTabla("privado", "THIS", "this", "referencia", 0, 1, aux.nombre + "_" + a.nombre);
                tabla.AddLast(este);
                super = new nodoTabla("privado", "SUPER", "super", "referencia", 1, 1, aux.nombre + "_" + a.nombre);
                tabla.AddLast(super);
                posicion.AddFirst(2);

                if (a.tipo != "vacio")
                {
                    tabla.AddLast(new nodoTabla("privado", a.tipo, "retorno", "returno", 2, 1, a.nombre + "_" + a.noMetodo));
                    posicion.RemoveFirst();
                    posicion.AddFirst(3);
                }

                foreach (ParseTreeNode p in a.parametros.ChildNodes)
                {
                    string nombre = p.ChildNodes[1].Token.Text;
                    string tipo = p.ChildNodes[0].ChildNodes[0].Token.Text;
                    nodoTabla parametro = new nodoTabla("privado", tipo, nombre, "PARAMETRO", posicion.First(), 1, aux.nombre + "_" + a.nombre + "_" + a.noMetodo);
                    tabla.AddLast(parametro);
                    int x = posicion.First();
                    posicion.RemoveFirst();
                    posicion.AddFirst(++x);
                }

                string ambito = aux.nombre + "_" + a.nombre + "_" + a.noMetodo;
                if (a.nombre.ToUpper().Equals("PRINCIPAL") && main)
                    ambito = aux.nombre + "_" + a.nombre;

                aumentarAmbito(ambito);

                ejecutar(a.sentencia, ambito);//ejecutamos las sentencias

                posicion.RemoveFirst();
                //recorrer para guardar guardar el tamanio

                tam = 0;
                for (; posActual < tabla.Count; posActual++)
                    tam += tabla.ElementAt(posActual).tam;
                nuevo.tam = tam;
                disminuirAmbito();

            }
            #endregion
        }

        

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
                case "IMPORTAR":
                    ejecutarIMPORTAR(raiz);
                    break;
                case "IMPORTS":
                    ejecutarIMPORTS(raiz.ChildNodes[0]);
                    
                    break;
                case "STRUCT":
                    guardar_clase_tree(raiz);
                    break;
            }
        }

        private void ejecutarIMPORTS(ParseTreeNode raiz)
        {
            //throw new NotImplementedException();
            ParseTreeNode archivos = raiz.ChildNodes[1];
            foreach(var item in archivos.ChildNodes)
            {
                string archivo = item.ChildNodes[0].Token.Text;
                string nombre = "";
                ///1 preguntar si continene http
                ///2 preguntar si tiene / de ruta
                ///3 si no es porque esta en la carpeta local
                string ext = "";
                if (archivo.Contains("//"))
                {

                }
                else if (archivo.EndsWith(".olc"))
                {
                    nombre = archivo.Replace("\"", "").Replace(".olc", "");
                    ext = ".olc";
                }
                else
                {
                    nombre = archivo.Replace("\"", "").Replace(".tree", "");
                    ext = ".tree";
                }
                if (!lista_clases.ContainsKey(nombre))
                {
                    string ruta = Control3d.getRuta() + nombre + ext;//cambiar la ruta  cuando se abra un archivo
                    string cont = retornarContenido(ruta);

                    if (cont != "")
                    {
                        //ejecutar el parse y retonar y NodoArbol
                        ParseTreeNode aux;
                        if (ext.Contains("olc"))
                        {
                            aux =retonarRaizOLC(cont, nombre);
                        }
                        else
                        {
                            aux = retonrarRaizTREE(cont, nombre);
                        }
                        if (aux != null)
                            SentenciasGlobales(aux);
                        else
                            Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "ERROR AL ANALIZAR EL CONTENIDO DEL ARCHIVO tree: " + nombre));
                    }
                }
                else
                    Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "Ya se importo un archivo con nombre:  " + nombre));
            }
        }

        private void ejecutarIMPORTAR(ParseTreeNode raiz)
        {
            string archivo = raiz.ChildNodes[0].Token.Text;
            string nombre = "";
            ///1 preguntar si continene http
            ///2 preguntar si tiene / de ruta
            ///3 si no es porque esta en la carpeta local
            if (archivo.Contains("//"))
            {

            }else
            {
                nombre = archivo.Replace("\"", "").Replace(".tree", "");

            }
            if (!lista_clases.ContainsKey(nombre))
            {
                string ruta = Control3d.getRuta() + nombre + ".tree";//cambiar la ruta  cuando se abra un archivo
                string cont = retornarContenido(ruta);

                if (cont != "")
                {
                    //ejecutar el parse y retonar y NodoArbol
                    ParseTreeNode aux = retonrarRaizTREE(cont, nombre);
                    if (aux != null)
                        SentenciasGlobales(aux);
                    else
                        Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "ERROR AL ANALIZAR EL CONTENIDO DEL ARCHIVO tree: " + nombre));
                }
            }
            else
                Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "Ya se importo un archivo con nombre:  " + nombre));
        }

        private void ejecutarLlamar(ParseTreeNode raiz)
        {
            Console.WriteLine(raiz.Term.Name);
            string original = raiz.ChildNodes[0].Token.Text.Replace("\"", "");

            string nombre_archivo = raiz.ChildNodes[0].Token.Text.Replace("\"", "").Replace(".olc","").Replace(".tree","");

            if (!lista_clases.ContainsKey(nombre_archivo))
            {
                string ruta = Control3d.getRuta() + original;//cambiar la ruta  cuando se abra un archivo
                string cont = retornarContenido(ruta);

                if (cont != "")
                {
                    //ejecutar el parse y retonar y NodoArbol
                    ParseTreeNode aux;
                    if (original.EndsWith("olc"))
                        aux = retonarRaizOLC(cont, nombre_archivo);
                    else
                        aux = retonrarRaizTREE(cont, nombre_archivo);
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

        public ParseTreeNode retonrarRaizTREE(string cont,string nombre_archivo)
        {
            GramaticaTre gramatica = new GramaticaTre();
            Parser parser = new Parser(gramatica);

            ParseTree arbol = parser.Parse(cont);
            ParseTreeNode raiz = arbol.Root;

            if (raiz == null || arbol.ParserMessages.Count > 0 || arbol.HasErrors())
            {
                //---------------------> Hay Errores      
                MessageBox.Show("Hay Errores en el archivo TREE:  "+nombre_archivo);
                errores.Append("hay errores: en el archivo: "+nombre_archivo+"\n");
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
                clase_actual = nuevo;//guarda la ultima clase que se tradujo// tendria que ser la que se se esta ejecutando
                listaActual = nuevo.variables;
                constructores = nuevo.constructores;
                //y tambien el constructor;
                //si tiene hereda mandar a ejecutar ese nodo y regresar a ejecutar las sentencias
                if (raiz.ChildNodes[2].ChildNodes.Count > 0)
                    ejecutarHeredar(raiz.ChildNodes[2]);
                foreach (ParseTreeNode a in raiz.ChildNodes[3].ChildNodes)
                {
                    posicion.AddFirst(0);
                    sentencias_clase(a, nombre);
                    posicion.RemoveFirst();
                }
            }
            
        }

        private void guardar_clase_tree(ParseTreeNode raiz)
        {
            string nombre = raiz.ChildNodes[0].Token.Text;
            
            if (!lista_clases.ContainsKey(nombre))
            {
                objeto_clase nuevo = new objeto_clase(nombre);
                lista_clases.Add(nombre, nuevo);
                metodos = nuevo.metodos;
                clase_actual = nuevo;//guarda la ultima clase que se tradujo// tendria que ser la que se se esta ejecutando
                listaActual = nuevo.variables;
                constructores = nuevo.constructores;
                //y tambien el constructor;
                //si tiene hereda mandar a ejecutar ese nodo y regresar a ejecutar las sentencias
                if (raiz.ChildNodes[1].ChildNodes.Count > 0)
                    ejecutarHeredar(raiz.ChildNodes[1]);//probar herencia del tree
                foreach (ParseTreeNode a in raiz.ChildNodes[2].ChildNodes)
                {
                    posicion.AddFirst(0);
                    sentencias_clase(a, nombre);
                    posicion.RemoveFirst();
                }
            }
        }

        private void ejecutarHeredar(ParseTreeNode raiz)
        {
            es_heredada = true;
            string ruta = "";
            string nombre = raiz.ChildNodes[0].Token.Text;

            ruta = Control3d.getRuta() + nombre + ".olc";
            string contenido = retornarContenido(ruta);

            if (contenido == "")
            {
                ruta = Control3d.getRuta() + nombre + ".tree";
                contenido = retornarContenido(ruta);
            }


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
                            es_heredada = false;
                            return;
                        }
                    }
                }
                else
                    Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "Error en la clase a heredar:  " + nombre));
            }
            else
                Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "Error al leer el archivo que contiene la clase a heredar:  " + nombre));
            es_heredada = false;
        }

        private void sentencias_clase(ParseTreeNode raiz,string nombre_clase)
        {

            ///parametro 0 visibilidad
            ///parametro 1 tipo de variable, metodo, arreglo, instancia etc.
            ///parametro 2 iden
            ///parametro 3 head
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
            }else if (raiz.ChildNodes[0].Term.Name.Equals("CONSTRUCTOR"))//es de tipo tree
            {
                guardarConstructor(visibilidad, raiz.ChildNodes[0], nombre_clase);
                return;
            }
            if (raiz.ChildNodes[0].ChildNodes.Count > 0)//pregunto si la trae visibilidad
                visibilidad = raiz.ChildNodes[0].ChildNodes[0].Token.Text;//la guardo
            if (raiz.ChildNodes[1].Term.Name.Equals("CONSTRUCTOR"))
            {
                guardarConstructor(visibilidad, raiz.ChildNodes[1],nombre_clase);
                return;
            }
            if (raiz.ChildNodes[1].ChildNodes[0].ChildNodes.Count > 0) //pregunto si eno es de tipo void
                tipo = raiz.ChildNodes[1].ChildNodes[0].ChildNodes[0].Token.Text;
            else
                tipo = "vacio";//me falta probar esto

            if(raiz.ChildNodes.Count>2)
                nombre = raiz.ChildNodes[2].Token.Text;

            if (raiz.ChildNodes[3].ChildNodes.Count > 0)//si no entonces es de tipo variable
            {
                ParseTreeNode aux = raiz.ChildNodes[3].ChildNodes[0];
                if (aux.Term.Name.Equals("METODO")){
                    guardar_metodo(tipo, nombre, visibilidad, aux);
                }else if (aux.Term.Name.Equals("L_ARRAY")){
                    guardarArreglo(visibilidad, tipo, nombre, raiz.ChildNodes[3]);
                }else if (aux.Term.Name.Equals("new")|| aux.Term.Name.Equals("nuevo"))
                {
                    guardarInstancia(visibilidad, tipo, nombre, raiz.ChildNodes[3].ChildNodes[1].Token.Text,raiz.ChildNodes[3].ChildNodes[2], nombre_clase,raiz.ChildNodes[3]);
                }
                else//seria una declaracion asignacion
                {
                    Variable var = new Variable(visibilidad, tipo, nombre, raiz.ChildNodes[3].ChildNodes[0]);
                    var.set_hereda(es_heredada);
                    guardarVariable(var, raiz.Span.Location.Line, raiz.Span.Location.Column);
                }
            }else
            {
                Variable var = new Variable(visibilidad, tipo, nombre, null);
                var.set_hereda(es_heredada);
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
            if (id.Equals("constructor"))
            {

            }else 
            if (!id.Equals(clase))
            {
                Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "Error en el nombre del constructor en la clase: " + clase));
                return;
            }
            if (raiz.ChildNodes.Count == 3&&raiz.ChildNodes[1].ChildNodes.Count>0)
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

        private void guardarInstancia(string visibilidad, string tipo, string nombre, string tipo2,ParseTreeNode raiz,string ambito,ParseTreeNode exp)
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
                    if (comprobar_constructores(clase.constructores, tipo, nombre,tipo2, raiz))
                    {
                        //Variable var = new Variable(visibilidad, tipo, nombre, exp);
                        //Boolean a = guardarVariable(var, raiz.Span.Location.Line, raiz.Span.Location.Column);
                        /*if (a)
                        {
                            nodoTabla nuevo = new nodoTabla("local", tipo, nombre, "var", tabla.posGlobal++, 1, "Global");
                            tabla.AddLast(nuevo);
                            nuevo.setExp(exp);
                            int x = posicion.First();
                            x++;
                            posicion.RemoveFirst();
                            posicion.AddFirst(x);
                        }*/
                    }
                    else
                    {
                        Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "No existe constructor que acepte el numero de parametros:" + nombre));
                        //return;
                    }
                }else
                {
                    //agregar error que no se puede instanciar la clase porque no tiene constructores
                    Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "No se puede instanciar la variable:  " + nombre +", ya que la clase no tiene constructor"));
                    //return;
                }
                //guardo la variable a pesar que haya herrores
                //esto es nuevo, 
                Variable var = new Variable(visibilidad, tipo, nombre, exp);
                var.set_hereda(es_heredada);
                guardarVariable(var, raiz.Span.Location.Line, raiz.Span.Location.Column);
            }
        }

        private void guardar_instancia_local(ParseTreeNode raiz, string ambito)
        {
            
            string clase = raiz.ChildNodes[0].Token.Text;
            string clase2 = raiz.ChildNodes[3].Token.Text;
            string id = raiz.ChildNodes[1].Token.Text;
            if (lista_clases.ContainsKey(clase))
            {
                objeto_clase aux_clase;
                lista_clases.TryGetValue(clase, out aux_clase);
                if (aux_clase.constructores.Count > 0)
                {
                    //comprobar los constructores con los parametros que se envian

                    //suponiendo que todo va bien
                    if (comprobar_constructores(aux_clase.constructores, clase, id,clase2, raiz.ChildNodes[4]))
                    {
                        Variable var = new Variable("local", clase, id, raiz);
                        Boolean a = guardarVariable(var, raiz.Span.Location.Line, raiz.Span.Location.Column);
                        if (a)
                        {
                            nodoTabla nuevo = new nodoTabla("local", clase, id, "var", posicion.First(), 1, ambito);
                            tabla.AddLast(nuevo);
                            int x = posicion.First();
                            x++;
                            posicion.RemoveFirst();
                            posicion.AddFirst(x);
                        }
                    }
                    else
                    {
                        Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "No existe constructor que acepte el numero de parametros:" + id));
                        return;
                    }
                }
                else
                {
                    //agregar error que no se puede instanciar la clase porque no tiene constructores
                    Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "No se puede instanciar la variable:  " + id + ", ya que la clase no tiene constructor"));
                    return;
                }
            }
        }

        private bool comprobar_constructores(LinkedList<metodo> constructores, string tipo, string nombre, string clase2, ParseTreeNode raiz)
        {
            //verificar que sean los mismos tipos
            if (!tipo.Equals(clase2))
            {
                Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "Error al instanciar la variable:" + nombre+", no coinciden los tipos"));
                return false;
            }

            if (raiz.ChildNodes.Count == 0)
            {
                foreach (metodo a in constructores)
                    if (a.parametros.ChildNodes.Count ==0)
                        return true;
            }else
            {
                ParseTreeNode parametros = raiz.ChildNodes[0];
                foreach(metodo a in constructores)
                    if (a.parametros.ChildNodes.Count == parametros.ChildNodes.Count)
                    return true;
            }

            return false;
        }

        private void guardarArreglo(string visibilidad, string tipo, string nombre, ParseTreeNode aux)
        {
            LinkedList<int> lista = new LinkedList<int>();
            ///viene l_array
            bool fl = true;
            ParseTreeNode l_array;
            l_array = aux.ChildNodes[0];

            if (aux.ChildNodes.Count == 2)
                fl = true;
            else
                fl = false;

            foreach(ParseTreeNode a in l_array.ChildNodes)
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

            if (lista.Count > 1)
            {
                int colum = lista.ElementAt(1);
                int fila = lista.ElementAt(0);
                lista.RemoveFirst();
                lista.RemoveFirst();
                lista.AddFirst(fila);
                lista.AddFirst(colum);
            }
            Variable var;
            if (fl)
               var = new Variable(visibilidad, tipo, nombre, aux.ChildNodes[1], lista);
            else
                var = new Variable(visibilidad, tipo, nombre, null, lista);
            var.set_hereda(es_heredada);
            
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

            #region traduccion de variables
            foreach (Variable a in listaActual)
            {
                if (a.casilla != null)
                {
                    int tam = 1;
                    foreach (int x in a.casilla)
                        tam *= x;
                    nodoTabla nuevo = new nodoTabla(a.visibilidad, a.tipo, a.nombre, "var_array", tabla.posGlobal, tam, "Global", a.casilla, a.valor);
                    tabla.AddLast(nuevo);
                    tabla.posGlobal++;//me falta cuando sea
                }
                else
                {
                    nodoTabla nuevo = new nodoTabla(a.visibilidad, a.tipo, a.nombre, "var", tabla.posGlobal, 1, "Global");
                    tabla.AddLast(nuevo);
                    nuevo.setExp(a.valor);
                    tabla.posGlobal++;//me falta cuando sea
                }
            }


            #endregion
            //se guardaron las variables globales

            //vamos a guardar los metodos
            foreach (metodo a in metodos)
            {
                string met = "METODO";
                if (a.tipo != "vacio")
                    met = "FUNCION";
                nodoTabla nuevo = new nodoTabla(a.visibilidad, a.tipo, a.nombre, met, -1, 0, "Global");
                tabla.AddLast(nuevo);
                int posActual = tabla.Count - 1;

                nuevo.setNoMetodo(a.noMetodo);
                aumentarAmbito(a.nombre);//ver si ocupo concatenarle el _noMetodo
                posicion.AddFirst(0);

                //we have to translate the parameters


                if (a.nombre.ToUpper().Equals("PRINCIPAL"))
                    ejecutar(a.sentencia, a.nombre);//ejecutamos las sentencias
                else
                {
                    if (a.tipo != "vacio")
                    {
                        tabla.AddLast(new nodoTabla("privado", a.tipo, "retorno", "returno", 0, 1, a.nombre + "_" + a.noMetodo));
                        posicion.RemoveFirst();
                        posicion.AddFirst(1);
                    }

                    /*vamos a traducir los parametros*/
                    foreach (ParseTreeNode p in a.parametros.ChildNodes)
                    {
                        string nombre = p.ChildNodes[1].Token.Text;
                        string tipo = p.ChildNodes[0].ChildNodes[0].Token.Text;
                        nodoTabla parametro = new nodoTabla("privado", tipo, nombre, "PARAMETRO", posicion.First(), 1, a.nombre + "_" + a.noMetodo);
                        tabla.AddLast(parametro);
                        int x = posicion.First();
                        posicion.RemoveFirst();
                        posicion.AddFirst(++x);
                    }

                    ejecutar(a.sentencia, a.nombre + "_" + a.noMetodo);//ejecutamos las sentencias
                }
                    
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
                case "INSTANCIA":
                    guardar_instancia_local(nodo, ambito);
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
                case "LOOP":
                    ejecutarLOOP(nodo, ambito);
                    break;
                //me falta traducir declarar array en metodos y funciones y contructores
                case "SWTICH":
                    ejecutarSWITCH(nodo, ambito);
                    break;
                case "DECLARAR_ARRAY":
                    ejecutarDECLARAR_ARRAY(nodo, ambito);
                    break;
                default:
                    break;
            }
        }

        private void ejecutarDECLARAR_ARRAY(ParseTreeNode nodo, string ambito)
        {
            LinkedList<int> lista = new LinkedList<int>();

       
            foreach (ParseTreeNode a in nodo.ChildNodes[2].ChildNodes)
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


            if (lista.Count > 1)//hago el cambio para que todo quede nitido
            {//acceso de la forma columna fila
                int colum = lista.ElementAt(1);
                int fila = lista.ElementAt(0);
                lista.RemoveFirst();
                lista.RemoveFirst();
                lista.AddFirst(fila);
                lista.AddFirst(colum);
            }

            String nombre = nodo.ChildNodes[1].Token.Text;
            string tipo = nodo.ChildNodes[0].ChildNodes[0].Term.Name;
            Variable var;
            bool fl = true;
            try
            {
                var = new Variable("local", tipo, nombre, nodo.ChildNodes[3], lista);
            }
            catch
            {
                var = new Variable("local", tipo, nombre, null, lista);//es de tipo tree
                fl = false;
            }
             
            Boolean z = guardarVariable(var, nodo.Span.Location.Line, nodo.Span.Location.Column);

            if (z)
            {
                nodoTabla nuevo = new nodoTabla("local", tipo, nombre, "var_array", posicion.First(), 1, ambito, var.casilla, var.valor);

                tabla.AddLast(nuevo);
                if((fl))
                    nuevo.setExp(nodo.ChildNodes[3]);

                int tam = 1;
                for (int i = 0; i < var.casilla.Count; i++)
                    tam *= var.casilla.ElementAt(i);
                nuevo.set_tam_oculto(tam);
                int x = posicion.First();
                x++;
                posicion.RemoveFirst();
                posicion.AddFirst(x);
            }
        }

        private void ejecutarSWITCH(ParseTreeNode nodo, string ambito)
        {
            throw new NotImplementedException();
        }

        private void ejecutarLOOP(ParseTreeNode nodo, string ambito)
        {
            MessageBox.Show("Hay que revisar esto del loop");
            string nuevo_ambito = ambito + "_loop" + listaActual.noX++;
            aumentarAmbito(nuevo_ambito);
            ejecutar(nodo.ChildNodes[0], nuevo_ambito);
            disminuirAmbito();
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

                
            }
            else  if (nodo.ChildNodes.Count == 2)
                {//if solo
                    string nuevo_ambito = ambito + "_if" + listaActual.noIf++;
                    aumentarAmbito(nuevo_ambito);
                    ejecutar(nodo.ChildNodes[1], nuevo_ambito);
                    disminuirAmbito();



                }

                else 
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
                if (item.rol.Equals("METODO")|| item.rol.Equals("FUNCION"))
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proyecto2_compi2_2sem_2017.TablaSimbolos;
using Proyecto2_compi2_2sem_2017.Control3D;
using Irony.Parsing;
using System.Windows.Forms;

namespace Proyecto2_compi2_2sem_2017.Compilador
{
    class generacion_3d_olc
    {
        private tablaSimbolos tabla;
        //private LinkedList<StringBuilder> lista_c3d;
        private LinkedList<codigo_3d> lista_c3d;
        private StringBuilder c3d;
        private LinkedList<ambitos> lista_ambito;
        private ambitos lista_actual;
        private string terminar_ejecucion;
        private Dictionary<string, objeto_clase> lista_clases;
        private LinkedList<metodo> lista_metodos;
        private LinkedList<int> tamanio_ambitos;


        public generacion_3d_olc()
        {
            this.tabla = Control3d.getTabla();
            this.lista_c3d = new LinkedList<codigo_3d>();
            this.c3d = Control3d.retornarC3D();
            this.lista_ambito = new LinkedList<ambitos>();
            this.lista_metodos = Control3d.getListaMetodo();
            this.lista_clases = Control3d.getListaClase();
            this.lista_ambito.AddFirst(new ambitos("Global"));
            terminar_ejecucion = Control3d.getEti();
            this.tamanio_ambitos = new LinkedList<int>();//para llevar los tamanios de los ambitos
            //creo que debo aumentarle el ambito
            traducirMain();
            traducirMetodos();
            traducir_clases();

        }

        
        private void aumentarAmbito(String ambito)
        {
            ambitos nuevo = new Compilador.ambitos(ambito);
            lista_ambito.AddFirst(nuevo);
            lista_actual = nuevo;

        }

        private void disminuirAmbito()
        {
            //se manda a guardar la lista de variables
            lista_ambito.RemoveFirst();
            lista_actual = lista_ambito.First();
            //posicion.RemoveFirst();
        }

        private void aumentar_3d()
        {
            this.lista_c3d.AddFirst(new codigo_3d());
        }

        private void disminuir_3d()
        {
            this.lista_c3d.RemoveFirst();
        }

        private void traducirMain()
        {
            LinkedList<metodo> lista = Control3d.getListaMetodo();
            if (lista != null)
            {
                foreach (metodo a in lista)
                {
                    if (a.nombre.Equals("PRINCIPAL"))
                    {
                        aumentarAmbito(a.nombre);
                        aumentar_3d();
                        escribir3d("\nvoid " + a.nombre + "(){", "Traduccion del metodo: principal");
                        //LE SETEO EL TAMANIO DEL PRINICIPAL
                        int tam = tamanio_metodo(a.nombre);
                        if (tam != -1)
                            this.tamanio_ambitos.AddLast(tam);

                        foreach (ParseTreeNode sent in a.sentencia.ChildNodes)
                        {
                            ejecutar(sent, a.nombre);
                        }
                        goto_etiqueta(terminar_ejecucion);
                        escribir3d("}", "Fin de traduccion del metodo: principal");

                        if (lista_c3d.First().estado)
                        {
                            string cont = lista_c3d.First().codigo.ToString();
                            c3d.Append(cont + "\n");
                        }
                        //copiar el codigo
                        disminuirAmbito();//putoooooo
                        disminuir_3d();
                        lista.Remove(a);//quito el principal para que ya no se imprima
                        break;
                    }
                }
            }
        }

        private void traducirMetodos()
        {
            LinkedList<metodo> lista = Control3d.getListaMetodo();
            if (lista != null)
            {
                foreach (metodo a in lista)
                {
                    aumentarAmbito(a.nombre + "_" + a.noMetodo);
                    aumentar_3d();
                    nodoTabla met = retornar_metodo(a.nombre + "_" + a.noMetodo);
                    tamanio_ambitos.AddFirst(met.tam);

                    escribir3d("void " + a.nombre + "_" + a.noMetodo + "(){", "Traduccion del metodo: " + a.noMetodo);

                    foreach (ParseTreeNode sent in a.sentencia.ChildNodes)
                        ejecutar(sent, a.nombre + "_" + a.noMetodo);

                    escribir3d("}", "Fin de traduccion del metodo: " + a.noMetodo);

                    if (lista_c3d.First().estado)
                    {
                        string cont = lista_c3d.First().codigo.ToString();
                        c3d.Append(cont + "\n");
                    }
                    //copiar el codigo
                    disminuirAmbito();
                    disminuir_3d();
                    tamanio_ambitos.RemoveFirst();
                }
                c3d.Append(terminar_ejecucion+":    //Etiqueta para terminar la ejecucion del programa");
            }
        }

        private void traducir_clases()
        {
            lista_clases = Control3d.getListaClase();
            foreach (KeyValuePair<string, objeto_clase> clase in lista_clases)
            {
                objeto_clase aux = clase.Value;
                //primero traducimos los constructores
                aumentarAmbito(aux.nombre + "_Global");


                foreach (metodo c in aux.constructores)
                {
                    aumentarAmbito(aux.nombre + "_Init_" + c.noMetodo);
                    aumentar_3d();
                    escribir3d("void " + c.nombre + "_Init_" + c.noMetodo + "(){", "Traduccion del constructor: " + c.noMetodo);

                    foreach (ParseTreeNode sent in c.sentencia.ChildNodes)
                    {
                        ejecutar(sent, c.nombre + "_Init_" + c.noMetodo);
                    }

                    escribir3d("}", "Fin de constructor: " + c.noMetodo);

                    if (lista_c3d.First().estado)
                    {
                        string cont = lista_c3d.First().codigo.ToString();
                        c3d.Append(cont + "\n");
                    }
                    //copiar el codigo
                    disminuirAmbito();
                    disminuir_3d();
                }
                disminuirAmbito();

            }
        }

        private void ejecutar(ParseTreeNode nodo, string ambito)
        {
            switch (nodo.Term.Name)
            {
                case "SENTENCIAS":
                    foreach (var item in nodo.ChildNodes)
                        ejecutar(item, ambito);
                    break;
                case "DECLARAR":
                    ejecutarDECLARAR(nodo, ambito);
                    break;
                case "DECLARAR_ASIG":
                    ejecutarDECLARAR_ASIGNAR(nodo,ambito);
                    break;
                case "INSTANCIA":
                    ejecutarINSTANCIA(nodo, ambito);
                    break;
                case "ASIGNAR":
                case "ASIGNACION":
                    ejecutarASIGNAR(nodo,ambito);
                    break;
                case "CALLFUN":
                    ejecutarCALLFUN(nodo);
                    break;
                case "IMPRIMIR":
                    ejecutarIMPRIMIR(nodo);
                    break;
                case "IF":
                    ejecutarIF1(nodo, ambito);
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
                case "FOR"://me falta guardar esto de la tabla de simbolos
                    ejecutarFOR(nodo, ambito);
                    break;
                case "WHILEX"://me falta guardar las variables en la tabla de sym
                    break;
                default:
                    break;
            }
        }

        private void ejecutarINSTANCIA(ParseTreeNode raiz, string ambito)
        {
            Console.Write("J");
            string clase = raiz.ChildNodes[0].Token.Text;
            string clase2 = raiz.ChildNodes[3].Token.Text;
            string id = raiz.ChildNodes[1].Token.Text;
            //suponiendo que existe el constructor, vamos a comparar los parametros
            if (lista_clases.ContainsKey(clase))
            {
                objeto_clase aux_clase;
                lista_clases.TryGetValue(clase, out aux_clase);
                nodoTabla nodo_clase = retornar_clase(aux_clase.nombre);
                if (aux_clase.constructores.Count > 0)
                {
                    //comprobar los constructores con los parametros que se envian
                    
                    //suponiendo que todo va bien
                    if (!comprobar_constructores(aux_clase.constructores, clase, id, clase2, raiz.ChildNodes[4],nodo_clase.tam))
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


        private bool comprobar_constructores(LinkedList<metodo> constructores, string tipo, string nombre, string clase2, ParseTreeNode raiz,int tam_clase)
        {
            //verificar que sean los mismos tipos
            if (!tipo.Equals(clase2))
            {
                Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "Error al instanciar la variable:" + nombre + ", no coinciden los tipos"));
                return false;
            }

            Boolean fl = false;
            nodoTabla var = get_variable(nombre, lista_ambito.First().nombre);
            metodo aux_met=null;
            if (var != null)
            {
                if (raiz.ChildNodes.Count == 0)
                {
                    foreach (metodo a in constructores)
                        if (a.parametros.ChildNodes.Count == 0)
                        {
                            fl=true;
                            aux_met = a;
                            break;
                        }
                }
                else
                {
                    ParseTreeNode parametros = raiz.ChildNodes[0];
                    LinkedList<metodo> auxiliar = new LinkedList<metodo>();

                    foreach (metodo a in constructores)
                        if (a.parametros.ChildNodes.Count == parametros.ChildNodes.Count)
                            auxiliar.AddLast(a);

                    foreach (metodo r in auxiliar)
                    {
                        Boolean a = comparar_constructor_con_parametros(r.parametros, parametros);
                        if (a) {
                            fl = true;
                            aux_met = r;
                            break;
                        }
                    }
                }
                if (fl)
                {
                    string pos_var = Control3d.getTemp();
                    string pos_heap = Control3d.getTemp();
                    escribir_operacion_asignacio(pos_var, "P", "+", var.pos.ToString());//posicion de la variable en el ambito actual
                    escribir_operacion_asignacio(pos_heap, "H", "+", "0");//guardo el ultimo puntero del heap
                    escribir_operacion_asignacio("H", "H", "+", tam_clase.ToString());//reservo el espacio del objeto
                    put_to_stack(pos_var, pos_heap);//asigno en el estack la poscion del objeto
                                                    //ver que el tamanio actual este correcto sino esto no funciona
                                                    //es decir que independientemente del ambito que este, se agregue a la lista de tamanio_ambito
                    string aux = Control3d.getTemp();
                    escribir_operacion_asignacio(aux, "P", "+", tamanio_ambitos.First().ToString());//vamos a pasar la referencia del valor de la instancia (heap)
                    put_to_stack(aux, pos_heap);//pasamos la referencia
                                                //aumentamos el puntero porque vamos a llamar al constructor
                    escribir_operacion_asignacio("P", "P", "+", tamanio_ambitos.First().ToString());
                    //llamamos al constructor
                    escribir3d("\t" + aux_met.nombre + "_Init_" + aux_met.noMetodo + "()", "llamamos al constructor de la clase");
                    //disminuimos el ambito despues del constructor
                    escribir_operacion_asignacio("P", "P", "-", tamanio_ambitos.First().ToString());

                    var.estado = true;
                    return true;
                }
            }
            return false;
        }

        private bool comparar_constructor_con_parametros(ParseTreeNode lista_parametros_metodo, ParseTreeNode parametros)
        {

            LinkedList<nodo3d> lista_parametros = new LinkedList<nodo3d>();
           
                foreach (ParseTreeNode exp in parametros.ChildNodes)
                {
                    nodo3d aux = evaluarEXPRESION(exp);
                    if (aux.tipo > 1)
                        lista_parametros.AddLast(aux);
                    else
                    {
                        Control3d.agregarError(new errores("semantico", parametros.Span.Location.Line, parametros.Span.Location.Column, "Error en parametros del constructor"));
                        this.lista_c3d.First().estado = false;
                        return false;
                    }
                }
                //comparar parametros con los metodos
                
                    if (comparar_parametros(lista_parametros_metodo, lista_parametros))
                    {
                        string aux_ptr = Control3d.getTemp();
                        escribir_operacion_asignacio(aux_ptr, "P", "+", tamanio_ambitos.First().ToString());
                        int cont = 1;

                        foreach (nodo3d param in lista_parametros)
                        {
                            string tmp = Control3d.getTemp();
                            escribir_operacion_asignacio(tmp, aux_ptr, "+", cont++.ToString());
                            put_to_stack(tmp, param.val);
                        }
                        return true;
                }
            return false;
        }

        private nodo3d ejecutarCALLFUN(ParseTreeNode nodo)
        {
            string nombre = nodo.ChildNodes[0].Token.Text;
            nodo3d retorno = new nodo3d();

            if (nodo.ChildNodes.Count > 1){//llamada con parametros
                return ejecutar_llamada_con_parametros(nombre, nodo);
            }

            foreach (metodo met in lista_metodos)//llamada sin parametros
            {
                
                if (met.nombre.Equals(nombre) && met.parametros.ChildNodes.Count == 0)
                {
                    nodoTabla metodo = retornar_metodo(nombre + "_" + met.noMetodo);
                    //este es el metodo
                    escribir_operacion_asignacio("P", "P", "+", tamanio_ambitos.First().ToString());
                    ////*********** aumentamos ambitos************////
                    //guardamos el tamanio actual
                    //tamanio_ambitos.AddFirst(metodo.tam);
                    //aumentarAmbito(metodo.nombre + "_" + metodo.noMetodo);
                    //aumentar_3d();

                    escribir3d("\t"+met.nombre+"_"+met.noMetodo+"()", "ejecutamos llamada a metodo/funcion");

                    if (metodo.rol.Equals("FUNCION"))
                    {
                        string ret = Control3d.getTemp();
                        obtener_desde_stak(ret, "P");
                        escribir_operacion_asignacio("P", "P", "-", tamanio_ambitos.First().ToString());
                        retorno = new nodo3d(metodo.tipo, ret);
                    }else
                        escribir_operacion_asignacio("P", "P", "-", tamanio_ambitos.First().ToString());

                    /*****diminuimmos ambitos*************/
                    //antes de disminuir el 3d ver que se ejecuto correctamente el codigo sino no se agrega
                    if (!lista_c3d.First().estado)
                        MessageBox.Show("Hubo un error en la traduccion del metodo: " + metodo.nombre + "_" + metodo.noMetodo);
                    //disminuirAmbito();
                    //disminuir_3d();
                }
            }
            return retorno;
        }

        private nodo3d ejecutar_llamada_con_parametros(string nombre, ParseTreeNode nodo)
        {
            Console.WriteLine(nombre);
            ParseTreeNode parametros = nodo.ChildNodes[1];
            LinkedList<nodo3d> lista_parametros=new LinkedList<nodo3d>();
            LinkedList<metodo> lista_auxiliar = new LinkedList<metodo>();

            foreach(metodo met in lista_metodos)
            {
                if (met.nombre.Equals(nombre) && met.parametros.ChildNodes.Count == parametros.ChildNodes.Count)
                    lista_auxiliar.AddLast(met);
            }
            if (lista_auxiliar.Count > 0)
            {
                foreach(ParseTreeNode exp in parametros.ChildNodes)
                {
                    nodo3d aux = evaluarEXPRESION(exp);
                    if (aux.tipo > 1)
                    {
                        lista_parametros.AddLast(aux);
                    }else
                    {
                        Control3d.agregarError(new errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "Error en parametros del metodo: " + nombre+",no se agregara codigo"));
                        this.lista_c3d.First().estado = false;
                        return new nodo3d();
                    }
                }
                //comparar parametros con los metodos
                foreach(metodo r in lista_auxiliar)
                {
                    if (comparar_parametros(r.parametros, lista_parametros))
                    {
                        Console.WriteLine("ya jalo");
                        //pasar los parametros
                        string aux_ptr = Control3d.getTemp();
                        escribir_operacion_asignacio(aux_ptr, "P", "+", tamanio_ambitos.First().ToString());
                        int cont = 0;
                        if (r.tipo != "vacio")
                            cont = 1;
                        foreach (nodo3d param in lista_parametros)
                        {
                            string tmp = Control3d.getTemp();
                            escribir_operacion_asignacio(tmp, aux_ptr, "+", cont++.ToString());
                            put_to_stack(tmp, param.val);
                        }
                        escribir_operacion_asignacio("P", "P", "+", tamanio_ambitos.First().ToString());
                        escribir3d("\t"+r.nombre+"_"+r.noMetodo,"llamamos al metodo: "+r.nombre);

                        string ret = Control3d.getTemp();
                        obtener_desde_stak(ret, "P");
                        escribir_operacion_asignacio("P", "P", "-", tamanio_ambitos.First().ToString());
                        return new nodo3d(r.tipo, ret);
                        //aumentar ambito
                        //asingar retorno
                        //reducir ambito
                        //jherson guapo
                    }
                }
            }
            Control3d.agregarError(new errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "No existe el metodo: " + nombre+", con los parametros asignados para ejecutar"));
            return new nodo3d();//error
        }

        private bool comparar_parametros(ParseTreeNode parametros, LinkedList<nodo3d> lista_parametros)
        {
            Boolean flag = true;
            int cont = 0;
            foreach(ParseTreeNode param in parametros.ChildNodes)
            {
                string tipo = retornar_tipo_string(param.ChildNodes[0].ChildNodes[0].Token .Text);
                if (tipo != lista_parametros.ElementAt(cont).tipo_valor)
                {
                    flag = false;
                    break;
                }
                cont++;
            }
            return flag;
        }

        private void ejecutarDECLARAR(ParseTreeNode nodo, String ambito)
        {
            String nombre = nodo.ChildNodes[1].ChildNodes[0].Token.Text;
            String visibilidad = "publico";
            /*if (nodo.ChildNodes[0].ChildNodes[0].Term.Name != null)
                visibilidad = nodo.ChildNodes[0].ChildNodes[0].Term.Name;*/
            string tipo = nodo.ChildNodes[0].ChildNodes[0].Term.Name;
            Variable nueva_variable = new Variable(nombre);

        }

        private void ejecutarDECLARAR_ASIGNAR(ParseTreeNode nodo,string ambito)
        {
            String nombre = nodo.ChildNodes[1].ChildNodes[0]. Token.Text;

            nodoTabla var = get_variable(nombre, ambito);
            nodo3d valor;
            if (var != null)
            {
                valor= evaluarEXPRESION(nodo.ChildNodes[2]);
                string tipo = retornar_tipo_string(var.tipo);
                if (tipo.Equals(valor.tipo_valor)){
                    escribir_asignacion(var.pos.ToString(), valor.val);
                    var.estado = true;
                }else
                    Control3d.agregarError(new errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "No son del mismo tipo para asignar"));
            }
            else
                Control3d.agregarError(new errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "No existe la variable en el ambito actual"));
            //verificar si la variable existe
            //ver si el tipo es el del mismo que la variable
        }

        private string retornar_tipo_string(string tipo)
        {
            switch (tipo)
            {
                case "entero":
                case "decimal":
                    return "num";
                case "cadena":
                    return "cad";
                default:
                    return tipo;
            }
        }

        private void ejecutarASIGNAR(ParseTreeNode nodo,string ambito)
        {
            String nombre = nodo.ChildNodes[0].Token.Text;
            nodoTabla var = get_variable(nombre, ambito);
            nodo3d valor;
            if (var != null)
            {
                valor = evaluarEXPRESION(nodo.ChildNodes[1]);
                string tipo = retornar_tipo_string(var.tipo);
                if (tipo.Equals(valor.tipo_valor))
                {
                    escribir_asignacion(var.pos.ToString(), valor.val);
                    var.estado = true;
                }
                else
                    Control3d.agregarError(new errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "No son del mismo tipo para asignar"));
            }
            else
                Control3d.agregarError(new errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "No existe la variable en el ambito actual"));
        }

        private void ejecutarIMPRIMIR(ParseTreeNode nodo)
        {
            nodo3d res= evaluarEXPRESION(nodo.ChildNodes[0]);
            if (res.tipo > 1 && res.tipo_valor.Equals("cad"))
            {
                string continuar = Control3d.getEti();
                string salida = Control3d.getEti();
                string tmp = Control3d.getTemp();
                escribirEtiqueta(continuar, "etiqueta para continuar imprimiendo");
                obtener_de_heap(tmp, res.val);
                escribir_condicion_sin_goto(tmp, "0", "==", salida);
                escribir3d("\tprint(\"%c\"," + tmp+")", "Imprimimos caracter de la cadena");
                escribir_operacion_asignacio(res.val, res.val, "+", "1");
                goto_etiqueta(continuar);
                escribirEtiqueta(salida, "Fin del ciclo imprimir");
                escribir3d("\tprint(\"%c\",13)", "Imprimimos caracter de la cadena");
            }
            else
                Control3d.agregarError(new errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "Error al tratar de imprimir, tipo: " + res.tipo));
            //this.salida.Append(sms.ToString() + "\n");
        }

        #region SENTENCIAS_CONTROL

        private void ejecutarIF1(ParseTreeNode nodo, string ambito)
        {
            //---------------------> Hijo 0, la condicion
            //---------------------> Hijo 1, la accion 
            string nuevo_ambito = ambito + "_if" + lista_actual.noIf++;
            string salida = Control3d.getEti();
            aumentar_3d();
            aumentarAmbito(nuevo_ambito);
            nodo3d val = castear_nodo3d(evaluarEXPRESION(nodo.ChildNodes[0]));

            escribir3d(val.etv + ":", "condicion verdadera de if");
            ejecutar(nodo.ChildNodes[1], nuevo_ambito);
            escribir3d("goto " + salida, "para que no ejecute las sentencias del else");
            escribir3d(val.etf + ":", "condicion falsa de if");
            
            //aqui tengo que ver como realizo toda la lista el elseif
            #region IF_ELSE
            if (nodo.ChildNodes.Count == 3)
            {//if solo
                if (nodo.ChildNodes[2].ChildNodes.Count > 0)
                {//viene if else
                    ejecutar(nodo.ChildNodes[2].ChildNodes[0], nuevo_ambito);
                }
                escribir3d(salida + ":", "para que no ejecute las sentencias del else");
            }
            #endregion
            
            #region IF_ELSEIF_ELSE
            else
            {
                foreach(ParseTreeNode elif in nodo.ChildNodes[2].ChildNodes)
                {
                    val = castear_nodo3d(evaluarEXPRESION(elif.ChildNodes[0]));
                    //nuevo_ambito = ambito + "_if" + lista_actual.noIf++;
                    //string salida = Control3d.getEti();
                    //aumentar_3d();
                    //aumentarAmbito(nuevo_ambito);

                    escribir3d(val.etv + ":", "condicion verdadera de else_if");
                    ejecutar(elif.ChildNodes[1], nuevo_ambito);
                    escribir3d("goto " + salida, "para que no ejecute las sentencias del else_if");
                    escribir3d(val.etf + ":", "condicion falsa de else_if");

                }
                if (nodo.ChildNodes[3].ChildNodes.Count > 0)
                {//viene if else
                    ejecutar(nodo.ChildNodes[3].ChildNodes[0], nuevo_ambito);
                }
                escribir3d(salida + ":", "para que no ejecute las sentencias del else");
                // tengo que ver si aumento el ambito like elseif_0 kind of
            }
            #endregion

            string cont = lista_c3d.First().codigo.ToString();
            Boolean estado = lista_c3d.First().estado;

            disminuirAmbito();
            disminuir_3d();

            if (estado)
                lista_c3d.First().codigo.Append(cont);
            else
                Control3d.agregarError(new errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "Erro en la traduccion del IF, no se agrego el codigo"));
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

        private void ejecutarWHILE(ParseTreeNode nodo, string ambito)
        {
            string nuevo_ambito = ambito + "_while" + lista_actual.noWhile++;

            string continuar = Control3d.getEti();

            aumentar_3d();
            aumentarAmbito(nuevo_ambito);

            escribir3d(continuar + ":", "\tetiqueta para continuar el while");

            nodo3d val = castear_nodo3d(evaluarEXPRESION(nodo.ChildNodes[0]));


            escribir3d(val.etv + ":", "condicion verdadera de while");
            ejecutar(nodo.ChildNodes[1], nuevo_ambito);
            goto_etiqueta(continuar + "\t//Para continuar el ciclo");
            escribir3d(val.etf + ":", "condicion falsa de while");

            string cont = lista_c3d.First().codigo.ToString();
            Boolean estado = lista_c3d.First().estado;

            disminuirAmbito();
            disminuir_3d();

            if (estado)
                lista_c3d.First().codigo.Append(cont);
            else
                Control3d.agregarError(new errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "Error en la traduccion del WHILE, no se agrego el codigo"));
        }

        private void ejecutarDO_WHILE(ParseTreeNode nodo, string ambito)
        {

            string nuevo_ambito = ambito + "_doWhile" + lista_actual.doWhile++;
            string continuar = Control3d.getEti();

            aumentar_3d();
            aumentarAmbito(nuevo_ambito);

            escribir3d(continuar + ":", "etiqueta para continuar el while");

            ejecutar(nodo.ChildNodes[0], nuevo_ambito);

            nodo3d val = castear_nodo3d(evaluarEXPRESION(nodo.ChildNodes[1]));

            escribir3d(val.etv + ":", "condicion verdadera de do_while");
            escribir3d("goto " + continuar, "Para continuar el ciclo");
            escribir3d(val.etf + ":", "condicion falsa de do_while");

            string cont = lista_c3d.First().codigo.ToString();
            Boolean estado = lista_c3d.First().estado;

            disminuirAmbito();
            disminuir_3d();

            if (estado)
                lista_c3d.First().codigo.Append(cont);
            else
                Control3d.agregarError(new errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "Error en la traduccion del DO_WHILE, no se agrego el codigo"));
        }

        private void ejecutarREPEAT(ParseTreeNode nodo, string ambito)
        {

            string nuevo_ambito = ambito + "_repeat" + lista_actual.noUntil++;

            string continuar = Control3d.getEti();

            aumentar_3d();
            aumentarAmbito(nuevo_ambito);

            escribir3d(continuar + ":", "etiqueta para continuar el repeat");

            nodo3d val = castear_nodo3d(evaluarEXPRESION(nodo.ChildNodes[1]));


            escribir3d(val.etf + ":", "condicion falsa de repeat");
            ejecutar(nodo.ChildNodes[0], nuevo_ambito);
            escribir3d("goto " + continuar, "Para continuar el ciclo");
            escribir3d(val.etv + ":", "condicion verdadera de repeat");

            string cont = lista_c3d.First().codigo.ToString();
            Boolean estado = lista_c3d.First().estado;

            disminuirAmbito();
            disminuir_3d();

            if (estado)
                lista_c3d.First().codigo.Append(cont);
            else
                Control3d.agregarError(new errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "Error en la traduccion del REPEAT, no se agrego el codigo"));
        }

        private void ejecutarFOR(ParseTreeNode nodo, string ambito)
        {

            string aumento = Control3d.getEti();
            string continuar = Control3d.getEti();
            string nuevo_ambito = ambito + "_for" + lista_actual.noWhile++;
            string variable = nodo.ChildNodes[2].ChildNodes[0].Token.Text;
            string tipo_aumento = nodo.ChildNodes[2].ChildNodes[1].Token.Text;

            aumentar_3d();
            aumentarAmbito(nuevo_ambito);
            ejecutar(nodo.ChildNodes[0], nuevo_ambito);
            escribir3d(continuar + ":", "etiqueta para continuar el FOR");
            nodo3d val = castear_nodo3d(evaluarEXPRESION(nodo.ChildNodes[1]));

            escribir3d(val.etv + ":", "condicion verdadera de FOR");
            ejecutar(nodo.ChildNodes[3], nuevo_ambito);

            escribir3d(aumento + ":", "Para aumentar la variable del for");
            nodoTabla var = get_variable(variable, nuevo_ambito);

            if (tipo_aumento.Equals("++")) {
                string tmp1 = poner_temp_en_pos(var.pos.ToString());
                string tmp = Control3d.getTemp();
                obtener_desde_stak(tmp, tmp1);
                escribir_operacion_asignacio(tmp, tmp, "+", "1");
                put_to_stack(tmp1, tmp);
            }
            else
            {
                string tmp1 = poner_temp_en_pos(var.pos.ToString());
                string tmp = Control3d.getTemp();
                obtener_desde_stak(tmp, tmp1);
                escribir_operacion_asignacio(tmp, tmp, "-", "1");
                put_to_stack(tmp1, tmp);
            }
            escribir3d("\tgoto " + continuar,"Para continuar la ejecucion del for");
            escribir3d(val.etf + ":", "condicion falsa de for");

            string cont = lista_c3d.First().codigo.ToString();
            Boolean estado = lista_c3d.First().estado;

            disminuirAmbito();
            disminuir_3d();

            if (estado)
                lista_c3d.First().codigo.Append(cont);
            else
                Control3d.agregarError(new errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "Error en la traduccion del FOR, no se agrego el codigo"));
        }

        private nodo3d castear_nodo3d(nodo3d nodo)
        {
            if (nodo.tipo == 2 || nodo.tipo == 3)
            {
                string etv = Control3d.getEti();
                string etf = Control3d.getEti();
                escribir_condicion(nodo.val, "1", "==", etv, etf, "creacion de condicion para if");
                return new nodo3d(etv, etf, 1);
            }
            else
                return nodo;
        }

        #endregion

        #region EVALUAR_EXP

        private nodo3d evaluarEXPRESION(ParseTreeNode nodo)
        {
            //---------------------> Si tiene 3 hijos
            #region "3 hijos"
            if (nodo.ChildNodes.Count == 3)
            {
                String operador = nodo.ChildNodes[1].Term.Name;
                switch (operador)
                {
                    case "||":  return evaluarOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case "&&":  return evaluarAND(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case "+":   return evaluarMAS(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case "-":   return evaluarMENOS(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case "*":   return evaluarPOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case "==":  return evaluarIGUAL(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case "!=":  return evaluarDIFERENTE(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case ">":   return evaluarMAYOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case ">=":  return evaluarMAYORIGUAL(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case "<=":  return evaluarMENORIGUAL(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case "<":   return evaluarMENOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);

                             //case "|&": return evaluarXOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                             /*  
                               case "/": return evaluarDIV(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                               case "^": return evaluarPOT(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                               case "&?": return evaluarNAND(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                               case "|?": return evaluarNOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);

                               */
                }
            }
            #endregion
            //---------------------> Si tiene 2 hijos
            #region "2 hijos"
            if (nodo.ChildNodes.Count == 2)
            {
                /*   if (nodo.Term.Name == "ACCESO_ARRAY")
                       return acceso_arreglo(nodo);
                   if (nodo.ChildNodes[0].Term.Name.Equals("!"))
                       return evaluarNOT(nodo.ChildNodes[1]);
                   if (nodo.ChildNodes[0].Term.Name.Equals("-"))
                       return evaluarUnario(nodo.ChildNodes[1]);

                   /* if (nodo.Term.Name.Equals("CALLMET"))
                    {
                        ejecutarLLamadasMetodos(nodo);
                        return retorn;
                    }*/
            }
            #endregion
            //---------------------> Si tiene 1 hijo
            #region "1 hijo"

            if (nodo.ChildNodes.Count == 1)
            {
                String termino = nodo.ChildNodes[0].Term.Name;
                switch (termino)
                {
                    //           case "EXP": return evaluarEXPRESION(nodo.ChildNodes[0]);
                    case "numero": return new nodo3d("num", nodo.ChildNodes[0].Token.Text);
                    case "tstring": return evaluarString(nodo.ChildNodes[0]);
                    case "id": return evaluarID(nodo.ChildNodes[0]);
                    case "false": return new nodo3d("bool","0");
                    case "true":  return new nodo3d("bool", "1");
                    case "tchar": return evaluarChar(nodo.ChildNodes[0]);
                        //case "NULL": return new nodo3d("NULL", "-300992");
                        //case "ACCESO_ARRAY": return acceso_arreglo(nodo.ChildNodes[0]);                                                    /*     */
                        //case "CALLFUN": return ejecutarCallMet(nodo.ChildNodes[0], true);
                        //case "ACCESO_OBJ": return acceso_a_objectos(nodo.ChildNodes[0]);
                }
            }
            #endregion
            return new nodo3d();//error
        }
        #endregion

        #region TRADUCCION_CONDICIONALES
        private nodo3d evaluarIGUAL(ParseTreeNode uno, ParseTreeNode dos)
        {
            nodo3d val1 = evaluarEXPRESION(uno);
            nodo3d val2 = evaluarEXPRESION(dos);

            if (val1.tipo == -1 || val2.tipo == -1)//retonaron valor nulo
                return new nodo3d();
            if (val1.categoria > 4 && val2.categoria > 4)//valido que solo sean cad,char,num,bool
                return new nodo3d();
            if (val1.tipo_valor.Equals("cad") || val2.tipo_valor.Equals("cad"))
                return comparar_condicionales_de_cadena(val1, val2, "==");

            string etv = Control3d.getEti();
            string etf = Control3d.getEti();
            escribir_condicion(val1.val, val2.val, "==", etv, etf, "Comparacion de tipo " + val1.tipo_valor + ", y: " + val2.tipo_valor);
            return new nodo3d(etv, etf, 1);

        }

        private nodo3d comparar_condicionales_de_cadena(nodo3d val1, nodo3d val2, string op)
        {
            if (val1.tipo_valor.Equals("cad") || val1.tipo_valor.Equals("char") && val2.tipo_valor.Equals("cad") || val2.tipo_valor.Equals("char"))
            {
                if (val1.tipo_valor.Equals(val2.tipo_valor))//las dos son string
                {
                    string total1 = Control3d.getTemp();
                    string total2 = Control3d.getTemp();

                    string ptr1 = Control3d.getTemp();
                    string ptr2 = Control3d.getTemp();

                    string ciclo1 = Control3d.getEti();
                    string ciclo2 = Control3d.getEti();
                    string salida1 = Control3d.getEti();
                    string salida2 = Control3d.getEti();

                    escribir3d(total1 + " = 0", "\tPara llevar lo de la cadena1");
                    escribir3d(total2 + " = 0", "\tPara llevar lo de la cadena2");
                    obtener_de_heap(ptr1, val1.val);
                    escribir3d(ciclo1 + ":", "\tCiclo para recorrer heap cadena1");
                    escribir_condicion_sin_goto(ptr1, "0", "==", salida1);
                    escribir_operacion_asignacio(total1, total1, "+", ptr1);
                    escribir_operacion_asignacio(val1.val, val1.val, "+", "1");
                    obtener_de_heap(ptr1, val1.val);
                    goto_etiqueta(ciclo1);
                    escribir3d(salida1 + ":", "\tFin de recorrer la cadena1");

                    obtener_de_heap(ptr2, val2.val);
                    escribir3d(ciclo2 + ":", "\tCiclo para recorrer heap cadena2");
                    escribir_condicion_sin_goto(ptr2, "0", "==", salida2);
                    escribir_operacion_asignacio(total2, total2, "+", ptr2);
                    escribir_operacion_asignacio(val2.val, val2.val, "+", "1");
                    obtener_de_heap(ptr2, val2.val);
                    goto_etiqueta(ciclo2);
                    escribir3d(salida2 + ":", "\tFin de recorrer la cadena2");

                    string etv = Control3d.getEti();
                    string etf = Control3d.getEti();

                    escribir_condicion(total1, total2, op, etv, etf, "Si cad1 " + op + " cad2");
                    return new nodo3d(etv, etf, 1);
                }
            }
            else
            {
                //agregar error
            }
            return new nodo3d();
        }
        private nodo3d evaluarMENOR(ParseTreeNode uno, ParseTreeNode dos)
        {
            nodo3d val1 = evaluarEXPRESION(uno);
            nodo3d val2 = evaluarEXPRESION(dos);

            if (val1.tipo == -1 || val2.tipo == -1)//retonaron valor nulo
                return new nodo3d();
            if (val1.categoria > 4 && val2.categoria > 4)//valido que solo sean cad,char,num,bool
                return new nodo3d();
            if (val1.tipo_valor.Equals("cad") || val2.tipo_valor.Equals("cad"))
                return comparar_condicionales_de_cadena(val1, val2, "<");

            string etv = Control3d.getEti();//aun hay que probar esto
            string etf = Control3d.getEti();
            escribir_condicion(val1.val, val2.val, "<=", etv, etf, "Comparacion de tipo " + val1.tipo_valor + ", y: " + val2.tipo_valor);
            return new nodo3d(etv, etf, 1);
        }

        private nodo3d evaluarMENORIGUAL(ParseTreeNode uno, ParseTreeNode dos)
        {
            nodo3d val1 = evaluarEXPRESION(uno);
            nodo3d val2 = evaluarEXPRESION(dos);

            if (val1.tipo == -1 || val2.tipo == -1)//retonaron valor nulo
                return new nodo3d();
            if (val1.categoria > 4 && val2.categoria > 4)//valido que solo sean cad,char,num,bool
                return new nodo3d();
            if (val1.tipo_valor.Equals("cad") || val2.tipo_valor.Equals("cad"))
                return comparar_condicionales_de_cadena(val1, val2, "<=");

            string etv = Control3d.getEti();//aun hay que probar esto
            string etf = Control3d.getEti();
            escribir_condicion(val1.val, val2.val, "<=", etv, etf, "Comparacion de tipo " + val1.tipo_valor + ", y: " + val2.tipo_valor);
            return new nodo3d(etv, etf, 1);
        }

        private nodo3d evaluarMAYORIGUAL(ParseTreeNode uno, ParseTreeNode dos)
        {
            nodo3d val1 = evaluarEXPRESION(uno);
            nodo3d val2 = evaluarEXPRESION(dos);

            if (val1.tipo == -1 || val2.tipo == -1)//retonaron valor nulo
                return new nodo3d();
            if (val1.categoria > 4 && val2.categoria > 4)//valido que solo sean cad,char,num,bool
                return new nodo3d();
            if (val1.tipo_valor.Equals("cad") || val2.tipo_valor.Equals("cad"))
                return comparar_condicionales_de_cadena(val1, val2, ">=");

            string etv = Control3d.getEti();//aun hay que probar esto
            string etf = Control3d.getEti();
            escribir_condicion(val1.val, val2.val, ">=", etv, etf, "Comparacion de tipo " + val1.tipo_valor + ", y: " + val2.tipo_valor);
            return new nodo3d(etv, etf, 1);
        }

        private nodo3d evaluarMAYOR(ParseTreeNode uno, ParseTreeNode dos)
        {
            nodo3d val1 = evaluarEXPRESION(uno);
            nodo3d val2 = evaluarEXPRESION(dos);

            if (val1.tipo == -1 || val2.tipo == -1)//retonaron valor nulo
                return new nodo3d();
            if (val1.categoria > 4 && val2.categoria > 4)//valido que solo sean cad,char,num,bool
                return new nodo3d();
            if (val1.tipo_valor.Equals("cad") || val2.tipo_valor.Equals("cad"))
                return comparar_condicionales_de_cadena(val1, val2, ">");

            string etv = Control3d.getEti();//aun hay que probar esto
            string etf = Control3d.getEti();
            escribir_condicion(val1.val, val2.val, ">", etv, etf, "Comparacion de tipo " + val1.tipo_valor + ", y: " + val2.tipo_valor);
            return new nodo3d(etv, etf, 1);
        }

        private nodo3d evaluarDIFERENTE(ParseTreeNode uno, ParseTreeNode dos)
        {
            nodo3d val1 = evaluarEXPRESION(uno);
            nodo3d val2 = evaluarEXPRESION(dos);

            if (val1.tipo == -1 || val2.tipo == -1)//retonaron valor nulo
                return new nodo3d();
            if (val1.categoria > 4 && val2.categoria > 4)//valido que solo sean cad,char,num,bool
                return new nodo3d();
            if (val1.tipo_valor.Equals("cad") || val2.tipo_valor.Equals("cad"))
                return comparar_condicionales_de_cadena(val1, val2, "!=");

            string etv = Control3d.getEti();//aun hay que probar esto
            string etf = Control3d.getEti();
            escribir_condicion(val1.val, val2.val, "!=", etv, etf, "Comparacion de tipo " + val1.tipo_valor + ", y: " + val2.tipo_valor);
            return new nodo3d(etv, etf, 1);
        }

        #endregion

        #region TRADUCCION_ARITMETICO
        private nodo3d retornarMasCadenas(nodo3d val1, nodo3d val2)
        {
            //en el caso de que los dos sean cadena
            string retorno = Control3d.getTemp();
            escribir_operacion_asignacio(retorno, "H", "+", "0");

            string tmp1 = Control3d.getTemp();


            if (val1.tipo_valor.Equals("num")) {
                val1 = convertir_int_en_strig(val2.val);
            }
               

            if (val1.tipo_valor.Equals("cad"))
            {
                string l1 = Control3d.getEti();
                string salida1 = Control3d.getEti();
                obtener_de_heap(tmp1, val1.val);
                escribir3d(l1 + ":", "ciclo para copiar la primera cadena");
                escribir_condicion_sin_goto(tmp1, "0", "==", salida1);
                asignar_heap("H", tmp1);
                aumentar_heap();
                escribir_operacion_asignacio(val1.val, val1.val, "+", "1");
                obtener_de_heap(tmp1, val1.val);
                goto_etiqueta(l1);
                escribir3d(salida1 + ":", "Salida de copiar el primer string");

            }
            else
            {
                //ver si es char, booelano, decimal o entero
                asignar_heap("H", "-201346094");
                aumentar_heap();
                asignar_heap("h", val1.val);
                aumentar_heap();
            }

            if (val2.tipo_valor.Equals("num")|| val2.tipo_valor.Equals("entero"))
            {
                val2 = convertir_int_en_strig(val2.val);
            }

            if (val2.tipo_valor.Equals("cad"))
            {
                string tmp2 = Control3d.getTemp();
                string salida2 = Control3d.getEti();
                string l2 = Control3d.getEti();
                obtener_de_heap(tmp2, val2.val);
                escribir3d(l2 + ":", "ciclo para copiar la segunda cadena");
                escribir_condicion_sin_goto(tmp2, "0", "==", salida2);
                asignar_heap("H", tmp2);
                aumentar_heap();
                escribir_operacion_asignacio(val2.val, val2.val, "+", "1");
                obtener_de_heap(tmp2, val2.val);
                goto_etiqueta(l2);
                escribir3d(salida2 + ":", "Salida de copiar el segundo string");
            }
            

            asignar_heap("H", "0");
            aumentar_heap();

            return new nodo3d("cad", retorno);
        }

        private nodo3d evaluarPOR(ParseTreeNode uno, ParseTreeNode dos)
        {
            nodo3d val1 = evaluarEXPRESION(uno);
            nodo3d val2 = evaluarEXPRESION(dos);

            if (val1.tipo <= 1 || val2.tipo <= 1)
                return new nodo3d();

            if (val1.tipo_valor.Equals("cad") || val2.tipo_valor.Equals("cad"))
                return new nodo3d();
            else
            {
                string tmp = Control3d.getTemp();

                if (val1.tipo_valor.Equals("num") || val2.tipo_valor.Equals("num"))
                {
                    escribir_operacion_asignacio(tmp, val1.val, "*", val2.val);
                    return new nodo3d("num", tmp);
                }
                else if (val1.tipo_valor.Equals("bool") || val2.tipo_valor.Equals("bool"))
                {
                    escribir_operacion_asignacio(tmp, val1.val, "*", val2.val);//aqui tengo que hacer una AND
                    return new nodo3d("bool", tmp);
                }
            }
            return new nodo3d();
        }

        private nodo3d evaluarMENOS(ParseTreeNode uno, ParseTreeNode dos)
        {
            nodo3d val1 = evaluarEXPRESION(uno);
            nodo3d val2 = evaluarEXPRESION(dos);

            if (val1.tipo <= 1 || val2.tipo <= 1)
                return new nodo3d();

            if (val1.tipo_valor.Equals("cad") || val2.tipo_valor.Equals("cad"))
                return new nodo3d();
            if (val1.tipo_valor.Equals("num") || val2.tipo_valor.Equals("num"))
            {
                string tmp = Control3d.getTemp();
                escribir_operacion_asignacio(tmp, val1.val, "-", val2.val);
                return new nodo3d("num", tmp);
            }
            return new nodo3d();
        }

        private nodo3d evaluarMAS(ParseTreeNode uno, ParseTreeNode dos)
        {
            nodo3d val1 = evaluarEXPRESION(uno);
            nodo3d val2 = evaluarEXPRESION(dos);

            if (val1.tipo <= 1 || val2.tipo <= 1)
                return new nodo3d();

            if (val1.tipo_valor.Equals("cad") || val2.tipo_valor.Equals("cad"))
                return retornarMasCadenas(val1, val2);
            else
            {
                string tmp = Control3d.getTemp();

                if (val1.tipo_valor.Equals("num") || val2.tipo_valor.Equals("num"))
                {
                    escribir_operacion_asignacio(tmp, val1.val, "+", val2.val);
                    return new nodo3d("num", tmp);
                }
                else if (val1.tipo_valor.Equals("bool") || val2.tipo_valor.Equals("bool"))
                {
                    escribir_operacion_asignacio(tmp, val1.val, "+", val2.val);//aqui tengo que hacer una OR
                    return new nodo3d("bool", tmp);
                }
            }

            return new nodo3d();
        }

        #endregion

        private nodo3d evaluarChar(ParseTreeNode nodo)
        {
            string cad =nodo.ChildNodes[0].Token.Text.Replace("'", "");
            int val = (int)cad.ElementAt(0);
            return new nodo3d("char", val.ToString());
        }

        private nodo3d evaluarID(ParseTreeNode nodo)
        {
            string variable = nodo.Token.Text;

            nodoTabla var = get_variable(variable, lista_ambito.First().nombre);
            if (var != null)
            {
                if (var.rol.Equals("var"))
                {
                    if (var.estado)
                    {
                        string tmp = Control3d.getTemp();
                        escribir_operacion_asignacio(tmp, "P", "+", var.pos.ToString());
                        string tmp2 = Control3d.getTemp();
                        obtener_desde_stak(tmp2, tmp);
                        string type = retornar_tipo_string(var.tipo);
                        return new nodo3d(type, tmp2);
                        //lo mas simple por el momento del ID
                    }else
                        Control3d.agregarError(new Control3D.errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "La variable no" + variable));
                }
                else if (var.rol.Equals("PARAMETRO")){
                    string tmp = Control3d.getTemp();
                    escribir_operacion_asignacio(tmp, "P", "+", var.pos.ToString());
                    string tmp2 = Control3d.getTemp();
                    obtener_desde_stak(tmp2, tmp);
                    string type = retornar_tipo_string(var.tipo);
                    return new nodo3d(type, tmp2);
                }
            }

            Control3d.agregarError(new Control3D.errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "No es encuentra la variable: " + variable));

            return new nodo3d();
        }

        private nodo3d evaluarString(ParseTreeNode nodo)
        {
            string tmp = Control3d.getTemp();
            escribir_operacion_asignacio(tmp, "H", "+", "0");

            string cadena = nodo.Token.Text.Replace("\"","");
            foreach(char a in cadena)
            {
                double x = (Double)(int)a;
                asignar_heap("H", x.ToString());
                aumentar_heap();
            }
            asignar_heap("H", "0");
            aumentar_heap();

            return new nodo3d("cad",tmp);
        }

        private nodo3d evaluarAND(ParseTreeNode uno, ParseTreeNode dos)
        {
            nodo3d val1 = castear_nodo3d(evaluarEXPRESION(uno));

            if (val1.tipo == -1)
            {
                Control3d.agregarError(new errores("semantico", uno.Span.Location.Line, uno.Span.Location.Column, "ERROR AL EVALUAR OR"));
                this.lista_c3d.First().estado = false;
                return new nodo3d();
            }

            escribir3d(val1.etv + ": ", "");

            nodo3d val2 = castear_nodo3d(evaluarEXPRESION(dos));

            if (val2.tipo == -1)
            {
                Control3d.agregarError(new errores("semantico", uno.Span.Location.Line, uno.Span.Location.Column, "ERROR AL EVALUAR OR"));
                this.lista_c3d.First().estado = false;
                return new nodo3d();
            }
            return new nodo3d(val2.etv,val1.etf+":\n"+ val2.etf, 1);
        }

        private nodo3d evaluarOR(ParseTreeNode uno, ParseTreeNode dos)
        {
            nodo3d val1 = castear_nodo3d(evaluarEXPRESION(uno));

            if (val1.tipo == -1)
            {
                Control3d.agregarError(new errores("semantico", uno.Span.Location.Line, uno.Span.Location.Column, "ERROR AL EVALUAR OR"));
                this.lista_c3d.First().estado = false;
                return new nodo3d();
            }

            escribir3d(val1.etf + ": ", "");

            nodo3d val2 = castear_nodo3d(evaluarEXPRESION(dos));

            if (val2.tipo == -1)
            {
                Control3d.agregarError(new errores("semantico", uno.Span.Location.Line, uno.Span.Location.Column, "ERROR AL EVALUAR OR"));
                this.lista_c3d.First().estado = false;
                return new nodo3d();
            }
            return new nodo3d(val1.etv+":\n"+val2.etv,val2.etf,1);
        }

        private nodo3d convertir_int_en_strig(string numero)
        {
            String cadena = "";

            String t0 = Control3d.getTemp();
            String t1 = Control3d.getTemp();
            String t2 = Control3d.getTemp();
            String t3 = Control3d.getTemp();
            String t4 = Control3d.getTemp();
            String t5 = Control3d.getTemp();
            String t6 = Control3d.getTemp();
            String t7 = Control3d.getTemp();

            String e1 = Control3d.getEti();
            String e2 = Control3d.getEti();
            String e3 = Control3d.getEti();
            String e4 = Control3d.getEti();
            String e5 = Control3d.getEti();
            String e6 = Control3d.getEti();
            String e7 = Control3d.getEti();
            String e8 = Control3d.getEti();
            String e9 = Control3d.getEti();
            String e10 = Control3d.getEti();
            String e11 = Control3d.getEti();
            String e12 = Control3d.getEti();

            //codigo 3d
            cadena += "\t\t" + t0 + " = " + numero + "//Convirtiendo entero a cadena\n"; // temporal que guarda el numero a convertir en cadena
            cadena += "\t\t" + t1 + " = " + "1\n"; //temporal que guardara si es negativo o positivo
            cadena += "\t\t" + "if " + t0 + " >= 0 goto " + e1 + "\n"; // si es negativo guardamos el -1
            cadena += "\t\t" + t1 + " = -1\n";
            cadena += "\t\t" + t0 + " = " + t0 + " * -1\n";
            cadena += "\t" + e1 + ":\n";
            cadena += "\t\t" + t2 + " = 1\n"; // temporal con el que sabremos el tamaño del numero  
            cadena += "\t" + e3 + ":\n";
            cadena += "\t\t" + t3 + " = 1\n"; // temporal que llevara el contador de 9
            cadena += "\t" + e4 + ":\n";
            cadena += "\t\t" + t4 + " = " + t2 + " * " + t3 + "\n";
            cadena += "\t\t" + "if " + t3 + " > 10 goto " + e5 + "\n";
            cadena += "\t\t" + "if " + t0 + " < " + t4 + " goto " + e2 + "\n";
            cadena += "\t\t" + t3 + " = " + t3 + " + 1\n";
            cadena += "\t\t" + "goto " + e4 + "\n";
            cadena += "\t" + e5 + ":\n";
            cadena += "\t\t" + t2 + " = " + t2 + " * 10\n";
            cadena += "\t\t" + "goto " + e3 + "\n";
            ////////////////////////////////////////////////////////////////
            cadena += "\t" + e2 + "://comenzamos a guardar el numero en el heap\n";
            cadena += "\t\t" + t5 + " = H\n"; // temporal que guardara la posicion del heap donde creamos el numero
            cadena += "\t\t" + "if " + t1 + " == 1 goto " + e6 + "\n";
            cadena += "\t\t" + "Heap[H] = 45\n";
            cadena += "\t\t" + "H = H + 1\n";
            cadena += "\t" + e6 + ":\n";
            cadena += "\t\t" + t3 + " = " + t3 + " - 1\n";
            cadena += "\t\t" + t6 + " = 0\n";
            cadena += "\t\t" + t7 + "= 48\n";
            cadena += "\t" + e7 + ":\n";
            cadena += "\t\t" + "if " + t6 + " == " + t3 + " goto " + e8 + "\n";
            cadena += "\t\t" + t6 + " = " + t6 + " + 1\n";
            cadena += "\t\t" + t7 + " = " + t7 + " + 1\n";
            cadena += "\t\t" + "goto " + e7 + "\n";

            cadena += "\t" + e8 + ":\n";//aqui guarda ascii
            cadena += "\t\t" + "Heap[H] = " + t7 + "\n";
            cadena += "\t\t" + "H = H + 1\n";


            cadena += "\t\t" + "if " + t2 + " == 1 goto " + e9 + "\n";
            cadena += "\t\t" + t4 + " = " + t2 + " * " + t3 + "\n";
            cadena += "\t\t" + t0 + " = " + t0 + " - " + t4 + "\n";
            cadena += "\t\t" + t2 + " = " + t2 + " / 10\n";

            cadena += "\t" + e10 + ":\n";
            cadena += "\t\t" + t3 + " = 1\n"; // temporal que llevara el contador de 9
            cadena += "\t" + e11 + ":\n";
            cadena += "\t\t" + t4 + " = " + t2 + " * " + t3 + "\n";
            cadena += "\t\t" + "if " + t3 + " > 10 goto " + e12 + "\n";
            cadena += "\t\t" + "if " + t0 + " < " + t4 + " goto " + e6 + "\n";
            cadena += "\t\t" + t3 + " = " + t3 + " + 1\n";
            cadena += "\t\t" + "goto " + e11 + "\n";
            cadena += "\t" + e12 + ":\n";
            cadena += "\t\t" + t2 + " = " + t2 + " * 10\n";
            cadena += "\t\t" + "goto " + e10 + "\n";

            cadena += "\t" + e9 + ":\n";
            cadena += "\t\t" + "Heap[H] = 0\n";
            cadena += "\t\t" + "H = H + 1\n";

            // codigo3d.Codigo = cadena
            //codigo3d.Tipo = Constante.TCadena
            //codigo3d.Valor = t5
            escribir3d("", " comienza traduccion del numero entero");
            escribir3d(cadena, "");
            escribir3d("", "fin de la traduccion de num_to_int");
            return new nodo3d("cad", t5);
        }

        #region ESCRIBIR EN 3D

        private void escribir_asignacion(string pos,string valor)
        {
            string tmp = Control3d.getTemp();
            escribir_operacion_asignacio(tmp, "P", "+", pos);
            put_to_stack(tmp, valor);
        }

        private string poner_temp_en_pos(string pos)
        {
            string tmp = Control3d.getTemp();
            escribir_operacion_asignacio(tmp, "P", "+", pos);
            return tmp;
        }


        public void escribirEtiqueta(string eti,object com)
        {
            string comentario = "";
            if (com.GetType().ToString().Equals("string"))
            {
                comentario = "\t//"+com.ToString();
            }
            this.lista_c3d.First().codigo.Append(eti + ":" + comentario + "\n");
        }

        public void escribir3d(string cont, string comentario)
        {
            this.lista_c3d.First().codigo.Append(cont + " //" + comentario + "\n");
        }

        public void escribir_condicion(string uno, string dos, string op, string etv, string etf, string comentario)
        {
            string cond = "\tif " + uno + " " + op + " " + dos + " goto " + etv + " //" + comentario + "\n";
            string cond2 = "\tgoto " + etf + "\n";
            this.lista_c3d.First().codigo.Append(cond);
            this.lista_c3d.First().codigo.Append(cond2);
        }

        public void escribir_condicion_sin_goto(string uno, string dos, string op, string etv)
        {
            string cond = "\tif " + uno + " " + op + " " + dos + " goto " + etv + "\n";
            this.lista_c3d.First().codigo.Append(cond);
        }

        public void escribir_operacion_asignacio(string destino, string uno, string op, string dos)
        {
            string cont = destino + " = " + uno + " " + op + " " + dos;
            this.lista_c3d.First().codigo.Append("\t" + cont + "\n");
        }

        public void aumentar_heap()
        {
            this.lista_c3d.First().codigo.Append("\tH = H + 1\n");
        }

        public void asignar_heap(string pos, string val)
        {
            this.lista_c3d.First().codigo.Append("\tHEAP[ " + pos + " ] = " + val + "\n");
        }

        public void obtener_de_heap(string tmp, string pos)
        {
            this.lista_c3d.First().codigo.Append("\t" + tmp + " = " + "HEAP[ " + pos + " ]\n");
        }

        public void obtener_desde_stak(string tmp, string pos)
        {
            this.lista_c3d.First().codigo.Append("\t" + tmp + " = " + "stack[ " + pos + " ]\n");
        }

        public void put_to_stack(string pos, string val)
        {
            this.lista_c3d.First().codigo.Append("\tStack[ " + pos + " ] = " + val + "\n");
        }

        public void goto_etiqueta(string etiq)
        {
            this.lista_c3d.First().codigo.Append("\tgoto " + etiq + " \n");
        }
        #endregion

        private nodoTabla get_variable(string nombre, string ambito)
        {

            foreach(ambitos r in lista_ambito)
            {
                foreach (nodoTabla a in tabla)
                {
                    if (a.nombre.Equals(nombre) && a.ambito.Equals(r.nombre))
                        return a;
                }
            }
            return null;
        }

        private int tamanio_metodo(string nombre)
        {
            foreach (nodoTabla n in tabla)
            {
                if (n.rol.Equals("METODO")|| n.rol.Equals("FUNCION"))
                {
                    if (n.nombre.Equals(nombre))
                        return n.tam;
                }
            }
            MessageBox.Show("No se encontro el metodo: " + nombre);
            return -1;
        }

        private nodoTabla retornar_metodo(string nombre)
        {
            foreach(nodoTabla a in tabla)
            {
                if (a.nombre.Equals(nombre) && a.rol.Equals("METODO") || a.rol.Equals("FUNCION"))
                    return a;
            }
            return null;
        }

        private nodoTabla retornar_clase(string nombre)
        {
            foreach (nodoTabla a in tabla)
            {
                if (a.nombre.Equals(nombre) && a.rol.Equals("CLASE"))
                    return a;
            }
            return null;
        }


    }
}



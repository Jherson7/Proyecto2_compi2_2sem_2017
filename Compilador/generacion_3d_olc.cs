

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
        private Boolean dentro_de_constructor;
        private string salida_de_errores;
        private LinkedList<String> salida_metodos = new LinkedList<string>();
        private Boolean este;
        private Boolean acceso;
        private LinkedList<objeto_clase> ambitos_clase;
        private objeto_clase clase_actual;
        private LinkedList<string> temporales_de_acceso;
        private LinkedList<Boolean> hacer_cond;


        public generacion_3d_olc()
        {
            this.tabla = Control3d.getTabla();
            this.lista_c3d = new LinkedList<codigo_3d>();
            this.c3d = Control3d.retornarC3D();
            this.lista_ambito = new LinkedList<ambitos>();
            this.lista_metodos = Control3d.getListaMetodo();
            this.lista_clases = Control3d.getListaClase();
            this.lista_ambito.AddFirst(new ambitos("Global"));
            terminar_ejecucion = Etiqueta();
            salida_de_errores = Etiqueta();
            salida_metodos.AddFirst(salida_de_errores);//por si viene un return en el principal
            this.tamanio_ambitos = new LinkedList<int>();//para llevar los tamanios de los ambitos
            this.temporales_de_acceso = new LinkedList<string>();
            this.acceso = false;
            this.hacer_cond = new LinkedList<bool>();
            //creo que debo aumentarle el ambito

            //para manejar los ambitos de las clases
            this.ambitos_clase = new LinkedList<objeto_clase>();

            aumentar_ambito_clase(Control3d.get_clase_actual());
            //-*--------------*-----------
            iniciar_variables_globales();
            traducirMain();
            //this.lista_metodos = clase_actual.metodos;
            //traducirMetodos();

            iniciar_traduccion_clases();

            
           
            c3d.Append(salida_de_errores + ": \n throw[] //para todos los null pointer exeptions\n");
            c3d.Append(terminar_ejecucion + ":    //Etiqueta para terminar la ejecucion del programa\n");
            this.este = false;
        }

        private void iniciar_traduccion_clases()
        {
            objeto_clase actual_clase = Control3d.get_clase_actual();
            traducir_clases(actual_clase);
            foreach (KeyValuePair<string, objeto_clase> Par in lista_clases)
            {
                if (Par.Value == actual_clase)
                    continue;
                string clase = Par.Key;
                objeto_clase aux = Par.Value;
                traducir_clases(aux);
            }
        }

        private void aumentar_ambito_clase(objeto_clase clase)
        {
            this.ambitos_clase.AddFirst(clase);
            this.clase_actual = ambitos_clase.First();
            this.lista_metodos = clase_actual.metodos;
        }

        private void disminuir_ambito_clase()
        {
            this.ambitos_clase.RemoveFirst();
            this.clase_actual = ambitos_clase.First();
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

        private void iniciar_variables_globales()
        {
            aumentar_3d();
            int contado_p = 0;
            aumentarAmbito(clase_actual.nombre + "_Global");
            objeto_clase clase = Control3d.get_clase_actual();

            nodoTabla actual = retornar_clase(clase.nombre);

            //primero reservamos espacio para la clase actual
            string ptr = Temp();
            escribir_operacion_asignacio(ptr, "H", "+", "0", "referencia this del objeto principal");
            put_to_stack("0", "H", "Referencia de las variables en el heap");
            escribir_operacion_asignacio("H", "H", "+", actual.tam.ToString(), "referencia this del objeto principal");
            //aqui van mas asignaciones si va el super//heredado
            if (clase != null)
            {
                foreach(Variable a in clase.variables)
                {
                    contado_p++;
                    nodoTabla var = get_variable(a.nombre, "Global");
                    if (var != null)
                    {
                        if (var.rol.Equals("var_array"))
                        {
                            // string tmp1 = Temp();
                            //string cont = Temp();
                            string auxiliar_ptr = Temp();
                           // escribir_operacion_asignacio(cont, "H", "+","0","Posicion libre del heap");
                            escribir_operacion_asignacio(auxiliar_ptr, ptr, "+", var.pos.ToString(), "Posicion de la variable: "+a.nombre);
                            //escribir_operacion_asignacio(auxiliar_ptr, "P", "+", var.pos.ToString(),"posiciono la variable: "+var.nombre);
                            asignar_heap(auxiliar_ptr, "H","le asigno a la variable su referencia del heap: " +var.nombre);
                            ParseTreeNode valores = var.getExpresion();

                            foreach (ParseTreeNode x in valores.ChildNodes)
                                llenar_casillas_arreglo(x);
                            var.estado = true;//se asigno correctamente
                        }
                        else
                        {//variables normales
                            if (var.getExpresion() != null)
                            {

                                if(var.tipo.ToLower()!="decimal"&& var.tipo.ToLower() != "entero" && var.tipo.ToLower() != "cadena" && var.tipo.ToLower() != "caracter")
                                {//entra aqui por es una instancia
                                    tamanio_ambitos.AddFirst(clase.variables.Count);
                                    if (ejecutar_instancia_global(var.tipo,var.nombre, var.getExpresion(),ptr))
                                        var.estado = true;
                                    else
                                        var.estado = false;
                                    tamanio_ambitos.RemoveFirst();
                                }else//tipo de variable normal
                                {
                                    nodo3d casilla = evaluarEXPRESION(var.getExpresion());
                                    if (casilla.tipo > 1)
                                    {
                                        //aqui tengo que verificar que sea del tipo de la variable
                                        //string tmp1 = Temp();
                                        string auxiliar_ptr = Temp();
                                        // escribir_operacion_asignacio(cont, "H", "+","0","Posicion libre del heap");
                                        escribir_operacion_asignacio(auxiliar_ptr, ptr, "+", var.pos.ToString(), "Posicion de la variable: " + a.nombre);
                                        //escribir_operacion_asignacio(tmp1, "P", "+", var.pos.ToString(),"posiciono la variable: " + var.nombre);
                                        asignar_heap(auxiliar_ptr, casilla.val,"asignacion de la variable: "+a.nombre+",  el valor de: "+casilla.val);
                                        var.estado = true;
                                    }
                                }
                            }
                        }
                    }
                }
                put_to_stack("2", "0","Referencia del this de la ambito actual");
                put_to_stack("3", "0", "Referencia del super de la ambito actual");
                escribir_operacion_asignacio("P", "P", "+", "2","Aumento el puntero para ejecutar el main");
            }

            if (lista_c3d.First().estado)
                c3d.Append(lista_c3d.First().codigo);
            else
                MessageBox.Show("ERROR EN LA TRADUCCION DEL MAIN");
        }

        private void llenar_casillas_arreglo(ParseTreeNode nodo)
        {
            if (nodo.Term.Name.Equals("PARAMETROS2"))
            {
                foreach (ParseTreeNode x in nodo.ChildNodes)
                    llenar_casillas_arreglo(x);
            }

            if (nodo.Term.Name.Equals("EXP"))
            {
                if (nodo.ChildNodes[0].Term.Name.Equals("LLAVE"))
                    llenar_casillas_arreglo(nodo.ChildNodes[0].ChildNodes[0]);
                else
                {
                    nodo3d casilla = evaluarEXPRESION(nodo);
                    if (casilla.tipo > 1)
                    {
                        //aqui tengo que verificar que sea del tipo del arreglo
                        asignar_heap("H", casilla.val,"Parametrizacion del arreglo");
                        aumentar_heap();//metodo que aumenta el puntero del heap en uno
                    }
                    else
                        MessageBox.Show("Erro en los valores del array");
                }
            }
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
                        aumentarAmbito(clase_actual.nombre+"_"+ a.nombre);
                        aumentar_3d();
                        escribir3d("\nvoid " + a.nombre + "(){", "Traduccion del metodo: principal");
                        //LE SETEO EL TAMANIO DEL PRINICIPAL
                        int tam = tamanio_metodo(a.nombre);
                        if (tam != -1)
                            this.tamanio_ambitos.AddLast(tam);

                        foreach (ParseTreeNode sent in a.sentencia.ChildNodes)
                        {
                            ejecutar(sent, clase_actual.nombre + "_" + a.nombre);
                        }
                        goto_etiqueta(terminar_ejecucion,"para que termine la ejecucion");
                        escribir3d("}", "Fin de traduccion del metodo: principal");

                        if (lista_c3d.First().estado)
                        {
                            string cont = lista_c3d.First().codigo.ToString();
                            c3d.Append(cont + "\n");
                        }
                        //copiar el codigo
                        disminuirAmbito();
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
                    string nuevo_ambito = clase_actual.nombre + "_" + a.nombre + "_" + a.noMetodo;
                    aumentarAmbito(nuevo_ambito);
                    salida_metodos.AddFirst(Etiqueta());
                    aumentar_3d();
                    nodoTabla met = retornar_metodo(clase_actual.nombre+"_"+ a.nombre , a.noMetodo);
                    tamanio_ambitos.AddFirst(met.tam);

                    escribir3d("void " +clase_actual.nombre+"_"+ a.nombre + "_" + a.noMetodo + "(){", "Traduccion del metodo: " + a.noMetodo);

                    foreach (ParseTreeNode sent in a.sentencia.ChildNodes)
                        ejecutar(sent, nuevo_ambito);
                    escribir3d("\t"+salida_metodos.First() + ":", "//salida del metodo: " + a.nombre + "_" + a.noMetodo + "\n"  );

                    escribir3d("}", "Fin de traduccion del metodo: " + a.nombre + "_" + a.noMetodo);

                    if (lista_c3d.First().estado)
                    {
                        string cont = lista_c3d.First().codigo.ToString();
                        c3d.Append(cont + "\n");
                    }
                    //copiar el codigo
                    disminuirAmbito();
                    disminuir_3d();
                    tamanio_ambitos.RemoveFirst();
                    salida_metodos.RemoveFirst();
                }
               
            }
        }
        //ver si en la traduccion de las clases tambien tengo que traducir los metodos....
        private void traducir_clases(objeto_clase aux)
        {
            aumentarAmbito(aux.nombre + "_Global");//aumentos de clase
            aumentar_ambito_clase(aux);

            #region traduccion constructores
            dentro_de_constructor = true;
            foreach (metodo c in aux.constructores)
            {
                nodoTabla rx = retornar_constructor(c.nombre+"_Init_"+c.noMetodo);

                tamanio_ambitos.AddFirst(rx.tam);

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
                    c3d.Append(cont + "\n");//copiar el codigo
                }

                tamanio_ambitos.RemoveFirst();
                disminuirAmbito();
                disminuir_3d();
            }

            dentro_de_constructor = false;
            #endregion

            #region traduccion de metodos de las clases
            foreach (metodo a in aux.metodos)
            {

                if (a.nombre.Contains("PRINCIPAL"))
                    continue;
                string nuevo_ambito = aux.nombre + "_" + a.nombre + "_" + a.noMetodo;
                aumentarAmbito(nuevo_ambito);
                salida_metodos.AddFirst(Etiqueta());
                aumentar_3d();
                nodoTabla met = retornar_metodo(aux.nombre + "_"+a.nombre,a.noMetodo);
                tamanio_ambitos.AddFirst(met.tam);

                escribir3d("void " + aux.nombre + "_" + a.nombre + "_" + a.noMetodo + "(){", "Traduccion del metodo: " + a.noMetodo);

                foreach (ParseTreeNode sent in a.sentencia.ChildNodes)
                    ejecutar(sent, nuevo_ambito);
                escribir3d("\t" + salida_metodos.First() + ":", "//salida del metodo: " + a.nombre + "_" + a.noMetodo + "\n");

                escribir3d("}", "Fin de traduccion del metodo: " + a.nombre + "_" + a.noMetodo);

                if (lista_c3d.First().estado)
                {
                    string cont = lista_c3d.First().codigo.ToString();
                    c3d.Append(cont + "\n");//copiar el codigo
                }

                disminuirAmbito();
                disminuir_3d();
                tamanio_ambitos.RemoveFirst();
                salida_metodos.RemoveFirst();
            }

            #endregion
            /*aqui va la traduccion de los metodos de todas las clases */

            disminuirAmbito();
            disminuir_ambito_clase();

        }

        private void ejecutar(ParseTreeNode nodo, string ambito)
        {
            switch (nodo.Term.Name)
            {
                case "SENTENCIAS":
                    foreach (var item in nodo.ChildNodes)
                        ejecutar(item, ambito);
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
                case "ASIG_ARRAY":
                    ejecutarAsignar_arreglo(nodo, ambito);
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
                case "RETORNO":
                    ejecutarRETORNO(nodo, ambito);
                    break;
                case "DECREMENTOS":
                    ejecutarDECREMETOS(nodo, ambito);
                    break;
                case "ESTE":
                    ejecutarSENTENCIAS_THIS(nodo, ambito);
                    break;
                case "ASIG_INSTANCIA":
                    ejecutarASIGNACION_INSTANCIA(nodo, ambito);
                    break;
                case "ASIGNACION_OBJECTO":
                    ejecutarASIGNACION_OBJETO(nodo, ambito);
                    break;
                case "ACCESO_OBJ":
                    ejecutar_acceso_a_objeto(nodo, ambito);
                    break;
                default:
                    MessageBox.Show("ME FALTA: " + nodo.Term.Name.ToString());
                    break;
            }
        }

        private void ejecutar_acceso_a_objeto(ParseTreeNode nodo, string ambito)
        {
            nodo3d prueba = evaluarEXPRESION(nodo);
        }

        private void ejecutarASIGNACION_OBJETO(ParseTreeNode nodo, string ambito)
        {
            //throw new NotImplementedException();
            ParseTreeNode no_acceso = nodo.ChildNodes[0];
            ParseTreeNode exp = nodo.ChildNodes[1];

            nodo3d ultimo = acceso_a_objectos(no_acceso,true);

            acceso = true;//

            if (exp.ChildNodes.Count > 1)
            {
                //es declaracion de instancia
                if (ultimo.categoria == 5)
                {
                    string clase2 = exp.ChildNodes[1].Token.Text;
                    temporales_de_acceso.AddFirst(ultimo.refe);
                    acceso = true;
                    ejecutarINSTANCIA_DE_ACCESOS(ultimo.get_nombre_ultimo(), ultimo.tipo_valor, clase2, exp.ChildNodes[2]);
                    temporales_de_acceso.RemoveFirst();
                }
            }else
            {
                //es una asignacion normal
                nodo3d res = evaluarEXPRESION(exp);
                if(res.tipo>0 && res.tipo_valor.Equals(ultimo.tipo_valor))
                {
                    escribir_asignacion(ultimo.val, res.val, ambito, "la ultima hahaha");
                }
            }
            acceso = false;
        }

        private void ejecutarSENTENCIAS_THIS(ParseTreeNode nodo, string ambito)
        {
            //throw new NotImplementedException();
            this.este = true;
            ejecutar(nodo.ChildNodes[0], ambito);
            this.este = false;

        }

        private void ejecutarDECREMETOS(ParseTreeNode nodo, string ambito)
        {
            string nombre = nodo.ChildNodes[0].Token.Text;
            nodoTabla var = get_variable(nombre, ambito);
            if (var != null)
            {
                string signo = "+";
                if (nodo.ChildNodes[1].Token.Text.Equals("--"))
                    signo = "-";
                string tmp = Temp();
                tmp = poner_temp_en_pos(var.pos.ToString());//aqui si es una variable global... que se hace?
                string tmp1 = Temp();
                obtener_desde_stak(tmp1, tmp,"obtengo la posicion de la variable: "+nombre+", para aumentarla");
                escribir_operacion_asignacio(tmp1, tmp1, signo, "1","aumento la variable");
                put_to_stack(tmp, tmp1,"asigno el nuevo valor a:"+nombre);
            }
        }

        private void ejecutarRETORNO(ParseTreeNode nodo, string ambito)
        {
            //throw new NotImplementedException();
            if (nodo.ChildNodes.Count > 0)
            {
                nodo3d ret = evaluarEXPRESION(nodo.ChildNodes[0]);
                if (ret.tipo > 1)
                {
                    string t1 = Temp();
                    escribir_operacion_asignacio(t1, "P", "+", "2", "para asignar el retornor");
                    put_to_stack(t1, ret.val,"pongo el retorno en P+2");
                    //aqui podria preguntar si el tipo de retorno es el mismo que el del metodo
                }else
                    agregar_error("evaluacion de retorno arroja valor nulo ALERTA!", nodo);
            }
            goto_etiqueta(salida_metodos.First(),"salgo al fin del metodo ya que vino return");
        }

        private void ejecutarAsignar_arreglo(ParseTreeNode nodo, string ambito)
        {

            string variable = nodo.ChildNodes[0].Token.Text;
            nodoTabla var = get_variable(variable, ambito);
            if (var != null)
            {
                
                if (var.rol.Equals("var_array"))
                {
                    nodo3d res = evaluarEXPRESION(nodo.ChildNodes[2]);
                    LinkedList<nodo3d> dimensiones = new LinkedList<nodo3d>();
                    if (var.dimArray.Count == nodo.ChildNodes[1].ChildNodes.Count)
                    {
                        //agregamos las condiciones para que no acceda
                        ParseTreeNode parametros = nodo.ChildNodes[1];
                        for (int p = 0; p < var.dimArray.Count; p++)
                        {
                            nodo3d val_aux = evaluarEXPRESION(parametros.ChildNodes[p]);
                            if (val_aux.categoria >= 2)
                            {
                                dimensiones.AddLast(val_aux);
                                escribir_condicion_sin_goto(val_aux.val, "0", "<", salida_de_errores,"Para que no se pase del limite inferior");
                                escribir_condicion_sin_goto(val_aux.val, var.dimArray.ElementAt(p).ToString(), ">", salida_de_errores, "Para que no se pase del limite superior");
                            }
                            else
                            {
                                agregar_error("No se permiten accesos que no sean de tipo entero", parametros);
                                return;
                            }
                        }
                        string tmp1 = "";
                        string tmpant = "";
                        for (int i = 0; i < dimensiones.Count; i++)
                        {
                            tmp1 = Temp();
                            escribir_operacion_asignacio(tmp1, dimensiones.ElementAt(i).val, "-", "1", "Parametrizacion de la dimension: " +dimensiones.ElementAt(i).val);
                            int tam = 1;
                            for (int H = i; H > 0; H--)
                                tam *= (var.dimArray.ElementAt(H - 1));
                            if (tam > 1)
                            {
                                string t2 = Temp();
                                escribir_operacion_asignacio(t2, tmp1, "*", tam.ToString(),"Parametrizacion");
                                string t3 = Temp();
                                escribir_operacion_asignacio(t3, tmpant, "+", t2, "Parametrizacion");
                                tmpant = t3;
                            }
                            else
                                tmpant = tmp1;
                        }
                        string p_destino = Temp();
                        if (var.ambito.ToUpper().Contains("GLOBAL"))
                        {
                            string global = Temp();
                            escribir_operacion_asignacio(global, var.pos.ToString(), "+", "0","posicion de la variable: "+var.nombre);
                            obtener_desde_stak(p_destino, global,"");
                        }
                        else
                        {
                            string tmp_r = Temp();
                            escribir_operacion_asignacio(tmp_r, "P", "+", var.pos.ToString(),"posicion de la variable: " + var.nombre);
                            obtener_desde_stak(p_destino, tmp_r,"");
                        }
                        string aux = Temp();
                        escribir_operacion_asignacio(aux, p_destino, "+", tmpant, "posicion del destino en heap");

                        asignar_heap(aux, res.val,"asigno el valor parametrizado");//asignamos el valor :D
                    }
                    else
                        agregar_error("No son las mismas dimensiones para acceder al array", nodo);
                }
                else
                    agregar_error("La variable no es de tipo array para asignar", nodo);
            }
        }

        private Boolean ejecutarINSTANCIA(ParseTreeNode raiz, string ambito)
        {
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
                    //suponiendo que todo va bien
                    if (!comprobar_constructores(aux_clase.constructores, clase, id, clase2, raiz.ChildNodes[4],nodo_clase.tam,"P"))
                    {
                        Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "No existe constructor que acepte el numero de parametros:" + id));
                        return false;
                    }
                }
                else
                {
                    //agregar error que no se puede instanciar la clase porque no tiene constructores
                    Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "No se puede instanciar la variable:  " + id + ", ya que la clase no tiene constructor"));
                    return false;
                }
            }

            return true;
        }

        private Boolean ejecutarINSTANCIA_DE_ACCESOS(string id,string clase,string clase2,ParseTreeNode raiz)
        {
            //suponiendo que existe el constructor, vamos a comparar los parametros
            if (lista_clases.ContainsKey(clase))
            {
                objeto_clase aux_clase;
                lista_clases.TryGetValue(clase, out aux_clase);
                nodoTabla nodo_clase = retornar_clase(aux_clase.nombre);
                if (aux_clase.constructores.Count > 0)
                {
                    //suponiendo que todo va bien
                    if (!comprobar_constructores(aux_clase.constructores, clase, id, clase2, raiz, nodo_clase.tam, "P"))
                    {
                        Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "No existe constructor que acepte el numero de parametros:" + id));
                        return false;
                    }
                }
                else
                {
                    //agregar error que no se puede instanciar la clase porque no tiene constructores
                    Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "No se puede instanciar la variable:  " + id + ", ya que la clase no tiene constructor"));
                    return false;
                }
            }
            return true;
        }

        private bool ejecutarASIGNACION_INSTANCIA(ParseTreeNode raiz, string ambito)
        {
            //string clase = raiz.ChildNodes[0].Token.Text;
            string clase2 = raiz.ChildNodes[2].Token.Text;
            string id = raiz.ChildNodes[0].Token.Text;
            nodoTabla var = get_variable(id, ambito);
            string clase = var.tipo;

            if (var != null)
            {
                if (lista_clases.ContainsKey(clase))
                {
                    objeto_clase aux_clase;
                    lista_clases.TryGetValue(clase, out aux_clase);
                    nodoTabla nodo_clase = retornar_clase(aux_clase.nombre);
                    if (aux_clase.constructores.Count > 0)
                    {
                        //suponiendo que todo va bien
                        if (!comprobar_constructores(aux_clase.constructores, clase, id, clase2, raiz.ChildNodes[3], nodo_clase.tam, "P"))
                        {
                            Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "No existe constructor que acepte el numero de parametros:" + id));
                            return false;
                        }
                    }
                    else
                    {
                        //agregar error que no se puede instanciar la clase porque no tiene constructores
                        Control3d.agregarError(new Control3D.errores("semantico", raiz.Span.Location.Line, raiz.Span.Location.Column, "No se puede instanciar la variable:  " + id + ", ya que la clase no tiene constructor"));
                        return false;
                    }
                }
            }
            //suponiendo que existe el constructor, vamos a comparar los parametros
            return true;
        }

        private Boolean ejecutar_instancia_global(string clase,string nombre,ParseTreeNode instancia,string ptr)
        {
            string clase2 = instancia.ChildNodes[1].Token.Text;

            if (lista_clases.ContainsKey(clase))
            {
                objeto_clase aux_clase;
                lista_clases.TryGetValue(clase, out aux_clase);
                nodoTabla nodo_clase = retornar_clase(aux_clase.nombre);
                if (aux_clase.constructores.Count > 0)
                {
                    //suponiendo que todo va bien
                    if (!comprobar_constructores(aux_clase.constructores, clase, nombre, clase2, instancia.ChildNodes[2], nodo_clase.tam, ptr))
                    {
                        Control3d.agregarError(new Control3D.errores("semantico", instancia.Span.Location.Line, instancia.Span.Location.Column, "No existe constructor que acepte el numero de parametros:" + nombre));
                        return false;
                    }
                }
                else
                {
                    //agregar error que no se puede instanciar la clase porque no tiene constructores
                    Control3d.agregarError(new Control3D.errores("semantico", instancia.Span.Location.Line, instancia.Span.Location.Column, "No se puede instanciar la variable:  " + nombre + ", ya que la clase no tiene constructor"));
                    return false;
                }
            }


            return true;
        }

        private bool comprobar_constructores(LinkedList<metodo> constructores, string tipo, string nombre, string clase2, ParseTreeNode raiz,int tam_clase,string ptr)
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
                        Boolean a = comparar_constructor_con_parametros(r.parametros, parametros,ptr);
                        if (a) {
                            fl = true;
                            aux_met = r;
                            break;
                        }
                    }
                }
                if (fl)
                {
                    string pos_var  = Temp();
                    string pos_heap = Temp();


                    if (acceso)
                        pos_var = temporales_de_acceso.First();

                    //en ptr tiene que venir la direccion del heap del  objeto
                    else if (var.ambito.Contains("Global"))
                    {
                        string t0 = Temp();
                        obtener_desde_stak(t0, "P","accedo a la referencia de la clase: "+ambitos_clase.First().nombre+", de la var: "+var.nombre);
                        escribir_operacion_asignacio(pos_var, t0, "+", var.pos.ToString(), "posicion de la variable: " + var.nombre + " en ambito global");//posicion de la variable en el ambito actual
                    }
                    else
                        escribir_operacion_asignacio(pos_var, ptr, "+", var.pos.ToString(), "posicion de la variable: "+var.nombre+", dentro de constructor");//posicion de la variable en el ambito actual

                    escribir_operacion_asignacio(pos_heap, "H", "+", "0", "guardo el ultimo puntero del heap");//guardo el ultimo puntero del heap
                    escribir_operacion_asignacio("H", "H", "+", tam_clase.ToString(), "reservo el espacio del objeto");//reservo el espacio del objeto
   /*antiguo*/      // put_to_stack(pos_var, pos_heap, "asigno en el stack la poscion del objeto");//asigno en el estack la poscion del objeto
   /*nuevo*/        asignar_heap(pos_var, pos_heap, "le asigno a la variable:  "+nombre+"su posicion en el heap");//asigno en el estack la poscion del objeto
                                                    //ver que el tamanio actual este correcto sino esto no funciona
                                                    //es decir que independientemente del ambito que este, se agregue a la lista de tamanio_ambito
                    string aux = Temp();
                    escribir_operacion_asignacio(aux, "P", "+", tamanio_ambitos.First().ToString(), "referencia del nuevo valor de la instancia(heap)");//vamos a pasar la referencia del valor de la instancia (heap)
                    put_to_stack(aux, pos_heap, "pasamos la referencia del objeto: "+var.nombre);//pasamos la referencia
                    escribir_operacion_asignacio(aux, aux, "+", "1", "referencia del super, que aun no la tengo jajaja");//vamos a pasar la referencia del valor de la instancia (heap)
                    put_to_stack(aux, pos_heap, "aqui tengo que pasar la referencia del super :D");//pasamos la referencia
                    //aumentamos el puntero porque vamos a llamar al constructor
                    escribir_operacion_asignacio("P", "P", "+", tamanio_ambitos.First().ToString(), "//aumentamos el puntero porque vamos a llamar al constructor");
                    //llamamos al constructor
                    //ver que tambien hay que pasar la posicion del super
                    escribir3d("\t" + aux_met.nombre + "_Init_" + aux_met.noMetodo + "()", "llamamos al constructor de la clase");
                    //disminuimos el ambito despues del constructor
                    escribir_operacion_asignacio("P", "P", "-", tamanio_ambitos.First().ToString(), "disminuimos el ambito despues del constructor");
                    var.estado = true;
                    return true;
                }
            }
            return false;
        }

        private bool comparar_constructor_con_parametros(ParseTreeNode lista_parametros_metodo, ParseTreeNode parametros,string ptr)
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
                        string aux_ptr = Temp();
                        escribir_operacion_asignacio(aux_ptr, "P", "+", tamanio_ambitos.First().ToString(),"puntero auxiliar parar pasar parametros");
                        int cont = 2;

                        foreach (nodo3d param in lista_parametros)
                        {
                            string tmp = Control3d.getTemp();
                            escribir_operacion_asignacio(tmp, aux_ptr, "+", cont++.ToString(),"posicionamos la posicion del parametro");
                            put_to_stack(tmp, param.val,"asignamos el valor que le pasamos al parametro");
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
                    nodoTabla metodo = retornar_metodo(clase_actual.nombre+"_"+ nombre,met.noMetodo);
                    //este es el metodo

                    string aux_ptr = Temp();
                    string t1 = Temp();
                    string t2 = Temp();
                    string t3 = Temp();
                    string t4 = Temp();
                    escribir_operacion_asignacio(aux_ptr, "P", "+", tamanio_ambitos.First().ToString(), "puntero para pasar los parametros");
                    obtener_desde_stak(t1, "P", "referencia del this actual");
                    put_to_stack(aux_ptr, t1, "referencia del this actual");
                    escribir_operacion_asignacio(t2, "P", "+", "1", "posicionamos la referencia del super");
                    obtener_desde_stak(t3, t2, "obtenemos el super de la clase actual");
                    escribir_operacion_asignacio(t4, aux_ptr, "+", "1", "posicion del super al siguiente ambito");
                    put_to_stack(t4, t3, "pasamos el super al siguiente ambito");

                    escribir_operacion_asignacio("P", "P", "+", tamanio_ambitos.First().ToString(),"aumentamos el ambito actual para ejecutar la llamada");
                    ////*********** aumentamos ambitos************////
                   
                    escribir3d("\t"+clase_actual.nombre+"_"+ met.nombre+"_"+met.noMetodo+"()", "ejecutamos llamada a metodo/funcion");

                    if (metodo.rol.Equals("FUNCION"))
                    {
                        string ret = Temp();
                        string pos_retorno = Temp();
                        escribir_operacion_asignacio(pos_retorno, "P", "+", "2", "posicionamos el retorno");
                        obtener_desde_stak(ret, pos_retorno, "obtenemos el valor del retorno");
                        escribir_operacion_asignacio("P", "P", "-", tamanio_ambitos.First().ToString(), "disminuimos ambito");
                        string type = retornar_tipo_string(metodo.tipo);
                        return new nodo3d(type, ret);
                    }
                    else
                        escribir_operacion_asignacio("P", "P", "-", tamanio_ambitos.First().ToString(),"disminuir el ambito despues de llamada");

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
                        //vamos a pasar la referencia del this, y del supers

                        string aux_ptr = Temp();
                        string t1 = Temp();
                        string t2 = Temp();
                        string t3 = Temp();
                        string t4 = Temp();
                        escribir_operacion_asignacio(aux_ptr, "P", "+", tamanio_ambitos.First().ToString(),"puntero para pasar los parametros");
                        escribir_comentario("pasamos los punteros del objeto, this y super");
                        obtener_desde_stak(t1, "P","referencia del this actual");
                        put_to_stack(aux_ptr, t1,"referencia del this actual");
                        escribir_operacion_asignacio(t2, "P", "+", "1", "posicionamos la referencia del super");
                        obtener_desde_stak(t3, t2,"obtenemos el super de la clase actual");
                        escribir_operacion_asignacio(t4, aux_ptr, "+","1", "posicion del super al siguiente ambito");
                        put_to_stack(t4, t3, "pasamos el super al siguiente ambito");
                        //pasar los parametros

                        int cont = 2;
                        if (r.tipo != "vacio")
                            cont = 3;
                        escribir_comentario("pasamos los parametros del metodo si los tuviera");
                        foreach (nodo3d param in lista_parametros)
                        {
                            string tmp = Control3d.getTemp();
                            escribir_operacion_asignacio(tmp, aux_ptr, "+", cont++.ToString(),"posicion del parametro");
                            put_to_stack(tmp, param.val,"pasamos el valor del parametro");
                        }
                        escribir_operacion_asignacio("P", "P", "+", tamanio_ambitos.First().ToString(),"aumentamos ambito para llamda");
                        //aqui agregue clase_actual+"_"+
                        escribir3d("\t"+ clase_actual.nombre + "_" + r.nombre+"_"+r.noMetodo+"()","llamamos al metodo: "+r.nombre);

                        string ret = Temp();
                        string pos_retorno = Temp();
                        escribir_operacion_asignacio(pos_retorno, "P", "+", "2", "posicionamos el retorno");
                        obtener_desde_stak(ret, pos_retorno,"obtenemos el valor del retorno");
                        escribir_operacion_asignacio("P", "P", "-", tamanio_ambitos.First().ToString(),"disminuimos ambito");
                        string type = retornar_tipo_string(r.tipo);
                        return new nodo3d(type, ret);
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
                    escribir_asignacion(var.pos.ToString(), valor.val,var.ambito,"asignamos el valor a la variable" +nombre);//ver aqui igual si va al heap o a la stack
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
            switch (tipo.ToLower())
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
                    escribir_asignacion(var.pos.ToString(), valor.val,var.ambito,"asingamos el valor a la variable: "+nombre+",en ambito" +ambito);
                    var.estado = true;
                }
                else
                    Control3d.agregarError(new errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "No son del mismo tipo para asignar:" + nombre));
            }
            else
                Control3d.agregarError(new errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "No existe la variable en el ambito actual: "+nombre));
        }

        private void ejecutarIMPRIMIR(ParseTreeNode nodo)
        {
            nodo3d res= evaluarEXPRESION(nodo.ChildNodes[0]);
            if (res.tipo == -1)
                return;
            string tipo = retornar_tipo_string(res.tipo_valor);

            if (tipo.Equals("num"))
            {
                escribir3d("\tprint(\"%f\"," + res.val + ")", "Imprimimos valor del numero");
                escribir3d("\tprint(\"%c\",13)", "Imprimimos salto de linea");
            }else if (tipo.Equals("char"))
            {
                escribir3d("\tprint(\"%c\"," + res.val + ")", "Imprimimos valor del caracter");
                escribir3d("\tprint(\"%c\",13)", "Imprimimos salto de linea");
            }
            else if (res.tipo > 1 && res.tipo_valor.Equals("cad"))
            {
                string continuar = Control3d.getEti();
                string salida = Control3d.getEti();
                string tmp = Control3d.getTemp();
                escribirEtiqueta(continuar, "etiqueta para continuar imprimiendo");
                obtener_de_heap(tmp, res.val,"posiciono el puntero en heap para comenzar a imprimir");
                escribir_condicion_sin_goto(tmp, "0", "==", salida,"termino de imprimir");
                escribir3d("\tprint(\"%c\"," + tmp+")", "Imprimimos caracter de la cadena");
                escribir_operacion_asignacio(res.val, res.val, "+", "1","aumento el ptr de heap");
                goto_etiqueta(continuar,"salto para que siga imprimiendo");
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
            escribir3d("\tgoto " + salida, "para que no ejecute las sentencias del else");
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
                    escribir3d("\tgoto " + salida, "para que no ejecute las sentencias del else_if");
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
            goto_etiqueta(continuar , "\t//Para continuar el ciclo");
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
                obtener_desde_stak(tmp, tmp1,"valor de la variable condicion");
                escribir_operacion_asignacio(tmp, tmp, "+", "1","aumento la variable condicio");
                put_to_stack(tmp1, tmp,"le meto el aumento");
            }
            else
            {
                string tmp1 = poner_temp_en_pos(var.pos.ToString());
                string tmp = Control3d.getTemp();
                obtener_desde_stak(tmp, tmp1, "valor de la variable condicion");
                escribir_operacion_asignacio(tmp, tmp, "-", "1", "disminuillo la variable condicio");
                put_to_stack(tmp1, tmp, "le meto el aumento");
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

        private nodo3d castear_nodo_if_false(nodo3d nodo)
        {
            if (nodo.tipo == 2 || nodo.tipo == 3)
            {
                string etv = Control3d.getEti();
                string etf = Control3d.getEti();
                escribir_condicion_if_false(nodo.val, "1", "==", etv, "creacion de condicion para if");
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
                    case "/":   return evaluarDIV(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                        /*case "|&": return evaluarXOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);

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
                MessageBox.Show("nodo con dos hijos!!");
                Console.Write("j");
                if (nodo.ChildNodes[0].Token.Text.Equals("not"))
                {
                    nodo3d val = castear_nodo_if_false(evaluarEXPRESION(nodo.ChildNodes[1]));
                    string aux = val.etf;
                    val.etf = val.etv;
                    val.etv = aux;
                    return val;
                }else if (nodo.ChildNodes[0].Token.Text.Equals("-"))
                {
                    nodo3d aux = evaluarEXPRESION(nodo.ChildNodes[1]);
                    string tmp = Temp();
                    escribir_operacion_asignacio(tmp, aux.val, "*", "-1","multiplico el unario");
                    return new nodo3d(aux.tipo_valor, tmp);
                }else if (nodo.Term.Name.Equals("CALLFUN"))
                    return ejecutarCALLFUN(nodo);
                else if(nodo.Term.Name.Equals("ACCESO_OBJ"))
                {
                    hacer_cond.AddFirst(true);
                    nodo3d ret = acceso_a_objectos(nodo, false);
                    return ret;
                }
             //ver que mas me falta
            }
            #endregion
            //---------------------> Si tiene 1 hijo
            #region "1 hijo"

            if (nodo.ChildNodes.Count == 1)
            {
                String termino = nodo.ChildNodes[0].Term.Name;
                switch (termino)
                {
                    case "EXP":         return evaluarEXPRESION(nodo.ChildNodes[0]);
                    case "numero":      return new nodo3d("num", nodo.ChildNodes[0].Token.Text);
                    case "tstring":     return evaluarString(nodo.ChildNodes[0]);
                    case "id":          return evaluarID(nodo.ChildNodes[0].Token.Text,nodo.ChildNodes[0]);
                    case "false":       return new nodo3d("bool","0");
                    case "true":        return new nodo3d("bool", "1");
                    case "tchar":       return evaluarChar(nodo.ChildNodes[0]);
                    case "ACCESO_ARRAY":return acceso_arreglo(nodo.ChildNodes[0]);                                                    /*     */
                    case "CALLFUN":     return ejecutarCALLFUN(nodo.ChildNodes[0]);
                    case "ESTE":        return evaluarESTE(nodo.ChildNodes[0]);

                    case "ACCESO_OBJ":
                        hacer_cond.AddFirst(true);
                        nodo3d ret = acceso_a_objectos(nodo.ChildNodes[0],false);
                        return ret;                        //case "NULL": return new nodo3d("NULL", "-300992");

                }
            }
            if (nodo.ChildNodes.Count == 0)
            {
                string tipo = nodo.Term.Name;

                switch (tipo)
                {
                    case "id":
                        return evaluarID(nodo.Token.Text,nodo);
                }

            }
                #endregion
                return new nodo3d();//error
        }

        private nodo3d acceso_a_objectos(ParseTreeNode nodo,bool ban)
        {
            /// throw new NotImplementedException();
            string nombre = nodo.ChildNodes[0].Token.Text;
            ParseTreeNode accesos = nodo.ChildNodes[1];

            nodoTabla var = get_variable(nombre, lista_ambito.First().nombre);
            nodo3d retorno = new nodo3d();

            
            nodo3d id = evaluarID(var.nombre,nodo);

            if (var != null)
            {
                int tam = accesos.ChildNodes.Count;
                if (var.tipo != "entero" && var.tipo != "decimal" && var.tipo != "cadena" && var.tipo != "caracter")
                {
                    if (!var.estado)//verifico si la variable se asigno previamente
                        Control3d.agregarError(new Control3D.errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "La variable no ha sido inicializada: " + var.nombre));

                    string tmp = Temp();
                    /******obtenemos la clase para aumentar el ambito de la clase****/
                    objeto_clase clase;
                    lista_clases.TryGetValue(var.tipo, out clase);

                    if (clase == null)
                    {
                        agregar_error("No existe la clase: " + var.tipo + ", error", nodo);
                        return new nodo3d();
                    }
                    int numero_aumentos = 0;
                    int numero_temporales = 0;

                    acceso = true;
                    aumentar_ambito_clase(clase);
                    aumentarAmbito(clase.nombre + "_Global");
                    temporales_de_acceso.AddFirst(id.val);
                    numero_temporales++;
                    
                    for (int i = 0; i < tam;)
                    {
                        ParseTreeNode item = accesos.ChildNodes[i];
                        i++;

                        if (ban && i == tam)
                            hacer_cond.AddFirst(false);
                        else if (ban && i < tam)
                            hacer_cond.AddFirst(true);

                        retorno = evaluarEXPRESION(item);//ver si es 

                        if (retorno.categoria == 5 && i < tam)
                        {
                            objeto_clase nueva;
                            lista_clases.TryGetValue(retorno.tipo_valor, out nueva);
                            aumentar_ambito_clase(nueva);
                            temporales_de_acceso.AddFirst(retorno.val);
                            numero_aumentos++;
                            numero_temporales++;
                        }
                        else if (retorno.tipo > 0 && retorno.tipo < 5 && i < tam)
                        {
                            agregar_error("la variable: " + item.ChildNodes[0].Token.Text + "No es de tipo objeto para acceder", nodo);
                            retorno = new nodo3d();//ver si es necesario esto
                            break;
                        }
                    }
                    for (; numero_aumentos > 0; numero_aumentos--)
                        disminuir_ambito_clase();//disminuyo todos los accesos a objetos
                    for (; numero_temporales > 0 && temporales_de_acceso.Count>0; numero_temporales--)
                        temporales_de_acceso.RemoveFirst();//disminuyo todos los temporales

                    disminuirAmbito();
                    disminuir_ambito_clase();//disminuyo el ambito del primer id
                    
                }
                else
                    agregar_error("La variable: " + var.nombre + ", no es de tipo objeto para acceder", nodo);
            }
            acceso = false;
            hacer_cond.Clear();
            return retorno;
        }

        private nodo3d evaluarDIV(ParseTreeNode uno, ParseTreeNode dos)
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
                    escribir_condicion_sin_goto(val2.val, "0", "==", salida_de_errores,"condicion para div/0");
                    escribir_operacion_asignacio(tmp, val1.val, "/", val2.val,"");
                    return new nodo3d("num", tmp);
                }
                else if (val1.tipo_valor.Equals("bool") || val2.tipo_valor.Equals("bool"))
                {
                    return new nodo3d("bool", tmp);
                }
            }
            return new nodo3d();
        }

        private nodo3d evaluarESTE(ParseTreeNode nodo)
        {
            //throw new NotImplementedException();
            this.este = true;
            nodo3d actual = evaluarEXPRESION(nodo.ChildNodes[0]);
            this.este = false;
            return actual;
        }

        private nodo3d acceso_arreglo(ParseTreeNode nodo)
        {
            string nombre = nodo.ChildNodes[0].Token.Text;
            nodoTabla var = get_variable(nombre, lista_ambito.First().nombre);
            LinkedList<nodo3d> dimensiones = new LinkedList<nodo3d>();
            if (var != null)
            {
                if (var.rol.Equals("var_array"))
                {
                    if (var.dimArray.Count == nodo.ChildNodes[1].ChildNodes.Count)
                    {
                        //agregamos las condiciones para que no acceda
                        ParseTreeNode parametros = nodo.ChildNodes[1];

                        for (int p = 0; p < var.dimArray.Count; p++)
                        {
                            nodo3d val_aux = evaluarEXPRESION(parametros.ChildNodes[p]);
                            if (val_aux.categoria >= 2)
                            {
                                dimensiones.AddLast(val_aux);
                                escribir_condicion_sin_goto(val_aux.val, "0", "<", salida_de_errores,"condicion acceso arreglo de limites");
                                escribir_condicion_sin_goto(val_aux.val, var.dimArray.ElementAt(p).ToString(), ">", salida_de_errores, "condicion acceso arreglo de limites");
                            }
                            else
                            {
                                agregar_error("No se permiten accesos que no sean de tipo entero", parametros);
                                return new nodo3d();//agregar error al ambito actual
                            }
                        }

                        string tmp1 = "";
                        string tmpant = "";
                        for (int i = 0; i < dimensiones.Count; i++)
                        {
                            tmp1 = Temp();
                            escribir_operacion_asignacio(tmp1, dimensiones.ElementAt(i).val, "-", "1","parametrizacion de array");
                            int tam = 1;
                            for (int H = i; H > 0; H--)
                                tam *= (var.dimArray.ElementAt(H - 1));
                            if (tam > 1)
                            {
                                string t2 = Temp();
                                escribir_operacion_asignacio(t2, tmp1, "*", tam.ToString(), "parametrizacion de array");
                                string t3 = Temp();
                                escribir_operacion_asignacio(t3, tmpant, "+", t2,"parametrizacion de array");
                                tmpant = t3;
                            }
                            else
                                tmpant = tmp1;
                        }
                        string p_destino = Temp();
                        if (var.ambito.ToUpper().Contains("GLOBAL")||this.este) { 
                            string global = Temp();
                            escribir_operacion_asignacio(global, var.pos.ToString(), "+", "0","posicion del array global"+var.nombre);
                            obtener_desde_stak(p_destino, global,"obtengo la dirreccion del heap");
                        } else
                        {
                            string tmp_r = Temp();
                            escribir_operacion_asignacio(tmp_r, "P", "+", var.pos.ToString(),"posicion del array:" + var.nombre);
                            obtener_desde_stak(p_destino, tmp_r,"obtengo la dirreccion del heap");
                        }
                        string aux = Temp();
                        escribir_operacion_asignacio(aux, p_destino, "+", tmpant,"posicion en el heap");
                        string retorno = Temp();
                        obtener_de_heap(retorno, aux,"valor del arreglo en la pos: "+aux);
                        string type = retornar_tipo_string(var.tipo);

                        return new nodo3d(type, retorno);
                    }
                    else
                        agregar_error("No son las mismas dimensiones para acceder al array", nodo);
                }
                else
                    agregar_error("La variable no es de tipo array", nodo);
            }
            return new nodo3d();
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
                    obtener_de_heap(ptr1, val1.val,"ptr en el heap");
                    escribir3d(ciclo1 + ":", "\tCiclo para recorrer heap cadena1");
                    escribir_condicion_sin_goto(ptr1, "0", "==", salida1,"termino la cadena1");
                    escribir_operacion_asignacio(total1, total1, "+", ptr1,"");
                    escribir_operacion_asignacio(val1.val, val1.val, "+", "1", "");
                    obtener_de_heap(ptr1, val1.val, "");
                    goto_etiqueta(ciclo1, "");
                    escribir3d(salida1 + ":", "\tFin de recorrer la cadena1");

                    obtener_de_heap(ptr2, val2.val, "");
                    escribir3d(ciclo2 + ":", "\tCiclo para recorrer heap cadena2");
                    escribir_condicion_sin_goto(ptr2, "0", "==", salida2, "");
                    escribir_operacion_asignacio(total2, total2, "+", ptr2, "");
                    escribir_operacion_asignacio(val2.val, val2.val, "+", "1", "");
                    obtener_de_heap(ptr2, val2.val, "");
                    goto_etiqueta(ciclo2, "");
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
            escribir_operacion_asignacio(retorno, "H", "+", "0","ptr para sumar las cadenas");

            string tmp1 = Control3d.getTemp();


            if (val1.tipo_valor.Equals("num")) {
                val1 = convertir_int_en_strig(val2.val);
            }
               

            if (val1.tipo_valor.Equals("cad"))
            {
                string l1 = Control3d.getEti();
                string salida1 = Control3d.getEti();
                obtener_de_heap(tmp1, val1.val,"obteno el ptr del heap de la primer cad");
                escribir3d(l1 + ":", "ciclo para copiar la primera cadena");
                escribir_condicion_sin_goto(tmp1, "0", "==", salida1,"termino de copiar la primer cad");
                asignar_heap("H", tmp1, "asigno al ptr del heap la primer cad");
                aumentar_heap();
                escribir_operacion_asignacio(val1.val, val1.val, "+", "1","");
                obtener_de_heap(tmp1, val1.val, "");
                goto_etiqueta(l1, "");
                escribir3d(salida1 + ":", "Salida de copiar el primer string");

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
                obtener_de_heap(tmp2, val2.val,"puntero de la segunda cadena");
                escribir3d(l2 + ":", "\t\tciclo para copiar la segunda cadena");
                escribir_condicion_sin_goto(tmp2, "0", "==", salida2,"termino de copiar la segunda cad");
                asignar_heap("H", tmp2,"");
                aumentar_heap();
                escribir_operacion_asignacio(val2.val, val2.val, "+", "1", "");
                obtener_de_heap(tmp2, val2.val, "");
                goto_etiqueta(l2, "");
                escribir3d(salida2 + ":", "Salida de copiar el segundo string");
            }
            

            asignar_heap("H", "0", "termina la suma de cadenas");
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
                    escribir_operacion_asignacio(tmp, val1.val, "*", val2.val,"multiplicacion de temporales tipo num");
                    return new nodo3d("num", tmp);
                }
                else if (val1.tipo_valor.Equals("bool") || val2.tipo_valor.Equals("bool"))
                {
                    escribir_operacion_asignacio(tmp, val1.val, "*", val2.val,"and de tipo bool");//aqui tengo que hacer una AND
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
                escribir_operacion_asignacio(tmp, val1.val, "-", val2.val,"resta de valores num");

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
                    escribir_operacion_asignacio(tmp, val1.val, "+", val2.val,"suma de valores tipo num");
                    return new nodo3d("num", tmp);
                }
                else if (val1.tipo_valor.Equals("bool") || val2.tipo_valor.Equals("bool"))
                {
                    escribir_operacion_asignacio(tmp, val1.val, "+", val2.val,"or de valores tipo num");//aqui tengo que hacer una OR
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

        private nodo3d evaluarID(string variable,ParseTreeNode nodo)
        {
            nodoTabla var = get_variable(variable, lista_ambito.First().nombre);
            if (var != null)
            {
                
                if (var.rol.Equals("PARAMETRO"))
                {
                    string tmp = Control3d.getTemp();
                    escribir_operacion_asignacio(tmp, "P", "+", var.pos.ToString(),"posicion del parametro: "+variable);
                    string tmp2 = Control3d.getTemp();
                    obtener_desde_stak(tmp2, tmp,"obtengo su valor");
                    string type = retornar_tipo_string(var.tipo);
                    return new nodo3d(type, tmp2);
                }
                else if(var.rol.Equals("var"))
                {
                    if (!var.estado)
                        Control3d.agregarError(new Control3D.errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "La variable no ha sido inicializada: " + variable));
                    string tmp = Temp();

                    if (acceso)
                    {//aqui me queda perfecto preguntar si quieren que retorno el valor de la variable o solo su posicion
                        string t1 = Temp();
                        string t2 = Temp();
                        escribir_operacion_asignacio(t1, temporales_de_acceso.First(), "+", var.pos.ToString(), "posicion de la variable: " + var.nombre + ", en heap");

                        //esto es nuevo 21:11 martes
                        if (hacer_cond.First())
                        {
                            //aqui debo preguntar si se quiere el valor poner la condicion sino solo retornamos el nodo, con la refe
                            obtener_de_heap(t2, t1, "valor de la variable: " + var.nombre);
                            escribir_condicion_sin_goto(t2, "-3092", "==", salida_de_errores, "si la variable == null, throw null pointer exeption");
                        }
                        //retornamos el tipo de la variable 
                        string type = retornar_tipo_string(var.tipo);
                        nodo3d retorno = new nodo3d(type, t2,t1);//lleva el valor y la referencia
                        retorno.setNombreUltimo(var.nombre);
                        return retorno;
                    }

                    else if (this.este || var.ambito.ToUpper().Contains("GLOBAL"))//tengo que ver esto para cuando este dentro de otras clases
                    {
                        string t0 = Temp();
                        string t1 = Temp();
                        string t2 = Temp();
                        obtener_desde_stak(t0, "P","accedo a la referencia de la variable actual");
                        escribir_operacion_asignacio(t1, t0, "+", var.pos.ToString(), "posicion de la variable: " + var.nombre + ", en heap");
                        obtener_de_heap(t2, t1, "valor de la variable: " + var.nombre);
                        escribir_condicion_sin_goto(t2, "-3092", "==", salida_de_errores, "si la variable == null, throw null pointer exeption");
                        string type = retornar_tipo_string(var.tipo);
                        return new nodo3d(type, t2);
                    }  //por el momento solo funciona para la clase actual
                    else {
                        escribir_operacion_asignacio(tmp, "P", "+", var.pos.ToString(), "posicion de la variable local");
                        string tmp2 = Temp();
                        obtener_desde_stak(tmp2, tmp, "obtengo su valor");
                        string type = retornar_tipo_string(var.tipo);
                        return new nodo3d(type, tmp2);
                    }
                        //lo mas simple por el momento del ID
                   
                }
                 
            }

            Control3d.agregarError(new Control3D.errores("semantico", nodo.Span.Location.Line, nodo.Span.Location.Column, "No es encuentra la variable: " + variable));

            return new nodo3d();
        }

        private nodo3d evaluarString(ParseTreeNode nodo)
        {
            string tmp = Control3d.getTemp();
            escribir_operacion_asignacio(tmp, "H", "+", "0","puntero disponible del heap");

            string cadena = nodo.Token.Text.Replace("\"","");
            foreach(char a in cadena)
            {
                double x = (Double)(int)a;
                asignar_heap("H", x.ToString(),"caracter: "+a);
                aumentar_heap();
            }
            asignar_heap("H", "0","termino la cadena: "+cadena);
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

        private void escribir_asignacion(string pos,string valor,string ambito,string variable)
        {
            string tmp = Control3d.getTemp();

            ///ver si aqui va tambien que si se activo acceso a objetos va otra cosa
            if (acceso)
            {
                asignar_heap(pos, valor, "le asigno a la variable: " + variable + ", su valor: " + valor + ", en el heap");
                return;
            }

            if (dentro_de_constructor || ambito.ToUpper().Contains("GLOBAL"))
            {
                string tmp_aux = Control3d.getTemp();
                string tmp_heap = Control3d.getTemp();
                obtener_desde_stak(tmp_aux, "P", "Obtengo desde stack el puntero para la variable: " + variable);
                escribir_operacion_asignacio(tmp_heap, tmp_aux, "+", pos, "Sumo la posicion de variable: " + variable);
                asignar_heap(tmp_heap, valor, "Asigno en heap el valor de la variable: " + variable);//por si es asignacion hacia un objeto 
            }
            else
            {
                escribir_operacion_asignacio(tmp, "P", "+", pos, "en " + tmp + "esta la posicion de la variable: " + variable);
                put_to_stack(tmp, valor, "Asigno a stack, el valor de la variable: " + variable);
            } 
                

            /* else if (this.este || ambito.ToUpper().Contains("GLOBAL"))
             {
             //aqui tengo que cambiar porque ahora va en el heap todo
                 escribir_operacion_asignacio(tmp, "0", "+", pos, "su suma 0 porque es variable global");
                 put_to_stack(tmp, valor, "Asigno a stack, el valor de la variable: " + variable);
             }*/

        }

        private string poner_temp_en_pos(string pos)
        {
            string tmp = Control3d.getTemp();
            escribir_operacion_asignacio(tmp, "P", "+", pos,"posicion en :" +tmp+",la variable");
            return tmp;
        }


        public void escribirEtiqueta(string eti,string com)
        {
            this.lista_c3d.First().codigo.Append(eti + ":\t//" + com + "\n");
        }

        public void escribir3d(string cont, string comentario)
        {
            this.lista_c3d.First().codigo.Append(cont + " \t\t//" + comentario + "\n");
        }

        public void escribir_condicion(string uno, string dos, string op, string etv, string etf, string comentario)
        {
            string cond = "\tif " + uno + " " + op + " " + dos + " goto " + etv + " \t\t//" + comentario + "\n";
            string cond2 = "\tgoto " + etf + "\n";
            this.lista_c3d.First().codigo.Append(cond);
            this.lista_c3d.First().codigo.Append(cond2);
        }

        public void escribir_condicion_if_false(string uno, string dos, string op, string etf, string comentario)
        {
            string cond = "\tifFalse " + uno + " " + op + " " + dos + " goto " + etf+ " //" + comentario + "\n";
            this.lista_c3d.First().codigo.Append(cond);
        }


        public void escribir_condicion_sin_goto(string uno, string dos, string op, string etv,string com)
        {
            string cond = "\tif " + uno + " " + op + " " + dos + " goto " + etv + "\t//" + com + "\n";
            this.lista_c3d.First().codigo.Append(cond);
        }

        public void escribir_operacion_asignacio(string destino, string uno, string op, string dos,string comentario)
        {
            string cont = destino + " = " + uno + " " + op + " " + dos;
            this.lista_c3d.First().codigo.Append("\t" + cont + " \t//" + comentario + "\n");
        }

        public void aumentar_heap()
        {
            this.lista_c3d.First().codigo.Append("\tH = H + 1\n");
        }

        public void aumentar_stack()
        {
            this.lista_c3d.First().codigo.Append("\tP = P + 1\n");
        }

        public void asignar_heap(string pos, string val,string comentario)
        {
            this.lista_c3d.First().codigo.Append("\tHEAP[ " + pos + " ] = " + val + "\t//" + comentario + "\n");
        }

        public void obtener_de_heap(string tmp, string pos,string comentario)
        {
            this.lista_c3d.First().codigo.Append("\t" + tmp + " = " + "HEAP[ " + pos + " ]\t//"+comentario+"\n");
        }

        public void obtener_desde_stak(string tmp, string pos,string comentario)
        {
            this.lista_c3d.First().codigo.Append("\t" + tmp + " = " + "stack[ " + pos + " ]\t//" + comentario + "\n");
        }

        public void put_to_stack(string pos, string val,string comentario)
        {
            this.lista_c3d.First().codigo.Append("\tStack[ " + pos + " ] = " + val + "\t//" +comentario+"\n");
        }

        public void goto_etiqueta(string etiq,string comentario)
        {
            this.lista_c3d.First().codigo.Append("\tgoto " + etiq + " \t//"+comentario+"\n");
        }
        
        public void escribir_comentario(string com)
        {
            this.lista_c3d.First().codigo.Append("\t\t// "+com+"\n");
        }
        
        public string Temp()
        {
            return Control3d.getTemp();
        }

        public string Etiqueta()
        {
            return Control3d.getEti();
        }

        #endregion

        private nodoTabla get_variable(string nombre, string ambito)
        {


            if (this.este && dentro_de_constructor)//|| que este en un acceso a objeto???
            {
                string ambito_new = ambitos_clase.First().nombre+"_Global";
                foreach (nodoTabla a in tabla)
                {
                    if (a.nombre.Equals(nombre) && a.ambito.Equals(ambito_new))
                    {
                        this.este = false;
                        return a;
                    }
                }
            }

            else if (this.este)//areglar esta charada
            {
                foreach (nodoTabla a in tabla)
                {
                    if (a.nombre.Equals(nombre) && a.ambito.Equals(ambitos_clase.First().nombre + "_Global"))
                    {
                        this.este = false;
                        return a;
                    }
                }
            }
            else {
                foreach (ambitos r in lista_ambito)
                {
                    foreach (nodoTabla a in tabla)
                    {
                        if (a.nombre.Equals(nombre) && a.ambito.Equals(r.nombre))
                            return a;
                    }
                }
            }
            
            return null;
        }

        private nodoTabla get_variable_exclusiva(string nombre_var, string ambito_var)
        {
             foreach (nodoTabla a in tabla)
             {
                if (a.nombre== nombre_var && a.ambito.ToLower()== ambito_var.ToLower()) 
                    return a;
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

        private nodoTabla retornar_metodo(string nombre,int num)
        {
            foreach(nodoTabla a in tabla)
            {
                if (a.nombre.Equals(nombre) && a.rol.Equals("METODO") &&a.noMetodo==num || a.nombre.Equals(nombre)&& a.rol.Equals("FUNCION") && a.noMetodo == num)
                    return a;
            }
            return null;
        }

        private nodoTabla retornar_constructor(string nombre)
        {
            foreach (nodoTabla a in tabla)
            {
                if (a.nombre.Equals(nombre) && a.rol.Equals("CONSTRUCTOR"))
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

        private void agregar_error(string descripcion,ParseTreeNode raiz)
        {
            Control3d.addError("semantico", descripcion, raiz);
        }

    }
}




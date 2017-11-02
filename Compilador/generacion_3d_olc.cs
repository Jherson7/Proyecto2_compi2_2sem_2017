using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proyecto2_compi2_2sem_2017.TablaSimbolos;
using Proyecto2_compi2_2sem_2017.Control3D;
using Irony.Parsing;

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
        
        public generacion_3d_olc()
        {
            this.tabla = Control3d.getTabla();
            this.lista_c3d = new LinkedList<codigo_3d>();
            this.c3d = Control3d.retornarC3D();
            this.lista_ambito = new LinkedList<ambitos>();
            this.lista_ambito.AddFirst(new ambitos("Global"));
            //creo que debo aumentarle el ambito
            traducirMetodos();
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

        private void traducirMetodos()
        {
            LinkedList<metodo> lista = Control3d.getListaMetodo();
            if (lista != null)
            {
                foreach (metodo a in lista)
                {
                    aumentarAmbito(a.nombre + "_" + a.noMetodo);
                    aumentar_3d();
                    escribir3d("void " + a.nombre + "_" + a.noMetodo + "{", "Traduccion del metodo: " + a.noMetodo);

                    foreach (ParseTreeNode sent in a.sentencia.ChildNodes)
                    {
                        ejecutar(sent, a.nombre + "_" + a.noMetodo);
                    }

                    escribir3d("}", "Fin de traduccion del metodo: " + a.noMetodo);

                    if (lista_c3d.First().estado)
                    {
                        string cont = lista_c3d.First().codigo.ToString();
                        c3d.Append(cont + "\n");
                    }
                    //copiar el codigo
                    disminuirAmbito();
                    disminuir_3d();
                }
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
                case "DECLARAR_ASIGNAR":
                    ejecutarDECLARAR_ASIGNAR(nodo);
                    break;
                case "ASIGNAR":
                    ejecutarASIGNAR(nodo);
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

        private void ejecutarDECLARAR(ParseTreeNode nodo, String ambito)
        {
            String nombre = nodo.ChildNodes[2].ChildNodes[0].Token.Text;
            String visibilidad = "publico";
            if (nodo.ChildNodes[0].ChildNodes[0].Term.Name != null)
                visibilidad = nodo.ChildNodes[0].ChildNodes[0].Term.Name;
            string tipo = nodo.ChildNodes[1].ChildNodes[0].Term.Name;
            Variable nueva_variable = new Variable(nombre);

        }

        private void ejecutarDECLARAR_ASIGNAR(ParseTreeNode nodo)
        {
            String nombre = nodo.ChildNodes[0].Token.Text;
            // Object valor = evaluarEXPRESION(nodo.ChildNodes[1]);
            // Variable nueva_variable = new Variable(nombre, valor);
            // guardarVariable(nueva_variable);
        }

        private void ejecutarASIGNAR(ParseTreeNode nodo)
        {
            String nombre = nodo.ChildNodes[0].Token.Text;
            Variable variable = getVariable(nombre);
            if (variable == null)
            {
                ejecutarIMPRIMIRERROR("Variable " + nombre + " no existe");
                return;
            }
            //variable.valor = evaluarEXPRESION(nodo.ChildNodes[1]);
        }

        private Variable getVariable(string nombre)
        {
            //jajaja me hace falta esto

            return null;
        }

        private void ejecutarIMPRIMIR(ParseTreeNode nodo)
        {
            //String sms = evaluarEXPRESION(nodo.ChildNodes[0]).ToString();
            //this.salida.Append(sms.ToString() + "\n");
        }

        private void ejecutarIMPRIMIRERROR(String sms)
        {
            //this.errores.Append(sms + "\n");
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

            escribir3d(continuar + ":", "etiqueta para continuar el while");

            nodo3d val = castear_nodo3d(evaluarEXPRESION(nodo.ChildNodes[0]));


            escribir3d(val.etv + ":", "condicion verdadera de while");
            ejecutar(nodo.ChildNodes[1], nuevo_ambito);
            escribir3d("goto " + continuar, "Para continuar el ciclo");
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
            string nuevo_ambito = ambito + "_while" + lista_actual.noWhile++;
            string variable = nodo.ChildNodes[2].ChildNodes[0].Token.Text;
            string tipo_aumento = nodo.ChildNodes[2].ChildNodes[1].Token.Text;

            aumentar_3d();
            aumentarAmbito(nuevo_ambito);

            escribir3d(continuar + ":", "etiqueta para continuar el FOR");
            nodo3d val = castear_nodo3d(evaluarEXPRESION(nodo.ChildNodes[1]));

            escribir3d(val.etv + ":", "condicion verdadera de FOR");
            ejecutar(nodo.ChildNodes[3], nuevo_ambito);
            escribir3d(aumento+",","Para aumentar la variable del for");
            if (tipo_aumento.Equals("++"))
                escribir_operacion_asignacio(variable, variable, "+", "1");
            else
                escribir_operacion_asignacio(variable, variable, "-", "1");
            escribir3d("goto " + continuar,"Para continuar la ejecucion del for");
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
                    case "||": return evaluarOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case "&&": return evaluarAND(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case "+": return evaluarMAS(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case "-": return evaluarMENOS(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                    case "*": return evaluarPOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);

                    case "==": return evaluarIGUAL(nodo.ChildNodes[0], nodo.ChildNodes[2]);

                        //case "|&": return evaluarXOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                        /*  
                          case "/": return evaluarDIV(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                          case "^": return evaluarPOT(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                          case "&?": return evaluarNAND(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                          case "|?": return evaluarNOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                          case "!=": return evaluarDIFERENTE(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                          case ">": return evaluarMAYOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                          case ">=": return evaluarMAYORIGUAL(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                          case "<=": return evaluarMENORIGUAL(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                          case "<": return evaluarMENOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
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
                    //           case "id": return evaluarID(nodo.ChildNodes[0]);
                    case "numero": return new nodo3d("num", nodo.ChildNodes[0].Token.Text);
                    case "tstring": return evaluarString(nodo.ChildNodes[0]);
                    case "id": return evaluarID(nodo.ChildNodes[0]);
                    case "false": return new nodo3d("bool","0");
                    case "true":  return new nodo3d("bool", "1");
                        //case "NULL": return new nodo3d("NULL", "-300992");
                        //case "true": return evaluarTrue();//new nodo3d("bool", "true");//evaluar true
                        //case "tchar": return evaluarString(nodo.ChildNodes[0]);//nodo.ChildNodes[0].Token.Text.Replace("'", "");
                        //case "ACCESO_ARRAY": return acceso_arreglo(nodo.ChildNodes[0]);                                                    /*     */
                        //case "CALLFUN": return ejecutarCallMet(nodo.ChildNodes[0], true);
                        //case "ACCESO_OBJ": return acceso_a_objectos(nodo.ChildNodes[0]);
                }
            }
            return new nodo3d();//error
        }

        private nodo3d evaluarIGUAL(ParseTreeNode uno, ParseTreeNode dos)
        {
            nodo3d val1 = evaluarEXPRESION(uno);
            nodo3d val2 = evaluarEXPRESION(dos);

            if (val1.tipo == -1 || val2.tipo == -1)
               return new nodo3d();
            if (val1.tipo_valor.Equals("cad") || val2.tipo_valor.Equals("cad"))
                return compararIgualCadena(val1, val2);


            return new nodo3d();
        }

        private nodo3d compararIgualCadena(nodo3d val1, nodo3d val2)
        {
            if(val1.tipo_valor.Equals("cad")|| val1.tipo_valor.Equals("char")&& val2.tipo_valor.Equals("cad") || val2.tipo_valor.Equals("char"))
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

                    escribir3d(total1 + " = 0", "Para llevar lo de la cadena1");
                    escribir3d(total2 + " = 0", "Para llevar lo de la cadena2");
                    obtener_de_heap(ptr1, val1.val);
                    escribir3d(ciclo1 + ":", "Ciclo para recorrer heap cadena1");
                    escribir_condicion_sin_goto(ptr1, "0", "==", salida1);
                    escribir_operacion_asignacio(total1, total1, "+", ptr1);
                    escribir_operacion_asignacio(val1.val, val1.val, "+", "1");
                    obtener_de_heap(ptr1, val1.val);
                    goto_etiqueta(ciclo1);
                    escribir3d(salida1 + ":", "Fin de recorrer la cadena1");

                    obtener_de_heap(ptr2, val2.val);
                    escribir3d(ciclo2 + ":", "Ciclo para recorrer heap cadena2");
                    escribir_condicion_sin_goto(ptr2, "0", "==", salida2);
                    escribir_operacion_asignacio(total2, total2, "+", ptr2);
                    escribir_operacion_asignacio(val2.val, val2.val, "+", "1");
                    obtener_de_heap(ptr2, val2.val);
                    goto_etiqueta(ciclo2);
                    escribir3d(salida2 + ":", "Fin de recorrer la cadena2");

                    string etv = Control3d.getEti();
                    string etf = Control3d.getEti();

                    escribir_condicion(total1, total2, "==", etv, etf, "Si las cadenas son iguales");
                    return new nodo3d(etv, etf,1);
                }
            }else
            {
                //agregar error
            }
            return new nodo3d();
        }

        private nodo3d evaluarID(ParseTreeNode nodo)
        {
            string variable = nodo.Token.Text;
            foreach (nodoTabla n in tabla)
            {
                if (n.ambito.Equals(lista_actual.nombre))
                {
                    if (n.rol.Equals("var") && n.nombre.Equals(variable))
                    {
                        if (n.getExpresion() != null)
                        {
                            string tmp = Control3d.getTemp();
                            escribir_operacion_asignacio(tmp, "P", "+", n.pos.ToString());
                            string tmp2 = Control3d.getTemp();
                            obtener_desde_stak(tmp2, tmp);
                            return new nodo3d(n.tipo, tmp2);
                            //lo mas simple por el momento del ID
                        }
                    }
                }
            }
            return new nodo3d();
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

        private nodo3d evaluarString(ParseTreeNode nodo)
        {
            string tmp = Control3d.getTemp();
            escribir_operacion_asignacio(tmp, "H", "+", "0");

            string cadena = nodo.Token.Text;
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

        private nodo3d evaluarMAS(ParseTreeNode uno, ParseTreeNode dos)
        {
            nodo3d val1 = evaluarEXPRESION(uno);
            nodo3d val2 = evaluarEXPRESION(dos);

            if (val1.tipo <= 1 || val2.tipo <= 1)
                return new nodo3d();

            if(val1.tipo_valor.Equals("cad")|| val2.tipo_valor.Equals("cad"))
                return retornarMasCadenas(val1, val2);
            else
            {
                string tmp = Control3d.getTemp();

                if (val1.tipo_valor.Equals("num")|| val2.tipo_valor.Equals("num"))
                {
                    escribir_operacion_asignacio(tmp, val1.val,"+", val2.val);
                    return new nodo3d("num", tmp);
                }else if (val1.tipo_valor.Equals("bool") || val2.tipo_valor.Equals("bool"))
                    {
                        escribir_operacion_asignacio(tmp, val1.val, "+", val2.val);//aqui tengo que hacer una OR
                        return new nodo3d("bool", tmp);
                    }
            }

            return new nodo3d();
        }

        private nodo3d retornarMasCadenas(nodo3d val1, nodo3d val2)
        {
            //en el caso de que los dos sean cadena
            string retorno = Control3d.getTemp();
            escribir_operacion_asignacio(retorno, "H","+" , "0");

            string tmp1 = Control3d.getTemp();

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

            }else
            {
                //ver si es char, booelano, decimal o entero
                asignar_heap("H", "-201346094");
                aumentar_heap();
                asignar_heap("h", val1.val);
                aumentar_heap();
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
            }else
            {
                asignar_heap("H", "-201346094");
                aumentar_heap();
                asignar_heap("h", val2.val);
                aumentar_heap();
            }
            

            asignar_heap("H", "0");
            aumentar_heap();

            return new nodo3d("cad",retorno);
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

        #endregion

        #endregion


        public void escribir3d(string cont, string comentario)
        {
            this.lista_c3d.First().codigo.Append(cont + " //" + comentario + "\n");
        }

        public void escribir_condicion(string uno, string dos,string op,string etv,string etf,string comentario)
        {
            string cond = "if " + uno + " " + op + " " + dos + " goto " + etv + " //" + comentario + "\n";
            string cond2 = "goto " + etf+"\n";
            this.lista_c3d.First().codigo.Append(cond);
            this.lista_c3d.First().codigo.Append(cond2);
        }

        public void escribir_condicion_sin_goto(string uno, string dos, string op, string etv)
        {
            string cond = "if " + uno + " " + op + " " + dos + " goto " + etv + "\n";
            this.lista_c3d.First().codigo.Append(cond);
        }

        public void escribir_operacion_asignacio(string destino, string uno, string op,string dos)
        {
            string cont = destino + " = " + uno + " " + op + " " + dos;
            this.lista_c3d.First().codigo.Append(cont + "\n");
        }

        public void aumentar_heap()
        {
            this.lista_c3d.First().codigo.Append("H = H + 1\n");
        }

        public void asignar_heap(string pos, string val)
        {
            this.lista_c3d.First().codigo.Append("HEAP[ "+pos+" ] = "+val+"\n");
        }

        public void obtener_de_heap(string tmp,string pos)
        {
            this.lista_c3d.First().codigo.Append(tmp+ " = "+ "HEAP[ "+ pos+ " ]\n");
        }

        public void obtener_desde_stak(string tmp, string pos)
        {
            this.lista_c3d.First().codigo.Append(tmp + " = " + "stack[ " + pos + " ]\n");
        }


        public void goto_etiqueta(string etiq)
        {
            this.lista_c3d.First().codigo.Append("goto "+etiq+" \n");
        }
    }
}



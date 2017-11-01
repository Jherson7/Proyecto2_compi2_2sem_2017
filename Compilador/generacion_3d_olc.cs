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
            aumentarAmbito(nuevo_ambito);
            ejecutar(nodo.ChildNodes[0], nuevo_ambito);
            disminuirAmbito();
        }

        private void ejecutarREPEAT(ParseTreeNode nodo, string ambito)
        {

            string nuevo_ambito = ambito + "_repeat" + lista_actual.noUntil++;
            aumentarAmbito(nuevo_ambito);
            ejecutar(nodo.ChildNodes[0], nuevo_ambito);
            disminuirAmbito();
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
                    /*  case "+": return evaluarMAS(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                      case "-": return evaluarMENOS(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                      case "*": return evaluarPOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                      case "/": return evaluarDIV(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                      case "^": return evaluarPOT(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                      case "%": return evaluarMOD(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                      case "||": return evaluarOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                      case "&&": return evaluarAND(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                      case "|&": return evaluarXOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                      case "&?": return evaluarNAND(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                      case "|?": return evaluarNOR(nodo.ChildNodes[0], nodo.ChildNodes[2]);
                      case "==": return evaluarIGUAL(nodo.ChildNodes[0], nodo.ChildNodes[2]);
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
                        //case "NULL": return new nodo3d("NULL", "-300992");
                        //case "false": return evaluarFalse();
                        //case "true": return evaluarTrue();//new nodo3d("bool", "true");//evaluar true
                        //case "tstring": return evaluarString(nodo.ChildNodes[0]);
                        //case "tchar": return evaluarString(nodo.ChildNodes[0]);//nodo.ChildNodes[0].Token.Text.Replace("'", "");
                        //case "ACCESO_ARRAY": return acceso_arreglo(nodo.ChildNodes[0]);                                                    /*     */
                        //case "CALLFUN": return ejecutarCallMet(nodo.ChildNodes[0], true);
                        //case "ACCESO_OBJ": return acceso_a_objectos(nodo.ChildNodes[0]);
                }
            }
            return null;
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


    }
}



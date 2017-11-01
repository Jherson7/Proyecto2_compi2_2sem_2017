using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Irony.Ast;
using Irony.Parsing;

namespace Proyecto2_compi2_2sem_2017.Compilador
{
    public class Gramatica : Grammar
    {
        public Gramatica() : base(caseSensitive: false)
        {
            CommentTerminal COMENTARIO_SIMPLE = new CommentTerminal("comentario_simple", "//", "\n", "\r\n");
            CommentTerminal COMENTARIO_MULT = new CommentTerminal("comentario_mult", "/-", "-/");
            NonGrammarTerminals.Add(COMENTARIO_SIMPLE);
            NonGrammarTerminals.Add(COMENTARIO_MULT);

            MarkReservedWords("clase");
            MarkReservedWords("hereda_de");
            MarkReservedWords("este");
            //tipos de variables
            MarkReservedWords("entero");
            MarkReservedWords("cadena");
            MarkReservedWords("void");
            MarkReservedWords("decimal");

            //visibilidades
            MarkReservedWords("publico");
            MarkReservedWords("protegido");
            MarkReservedWords("privado");

            MarkReservedWords("@Sobrescribir");
            MarkReservedWords("retornar");
            MarkReservedWords("principal");
            MarkReservedWords("llamar");
            MarkReservedWords("importar");
            MarkReservedWords("new");

            MarkReservedWords("si");
            MarkReservedWords("Sino Si");
            MarkReservedWords("Sino");
            MarkReservedWords("Mientras");
            MarkReservedWords("Hacer");
            MarkReservedWords("x");
            MarkReservedWords("repetir");
            MarkReservedWords("until");
            MarkReservedWords("Para");
            MarkReservedWords("imprimir");

            var str = ToTerm("cadena");
            var entero = ToTerm("entero");
            var BOOL = ToTerm("booleano");
            var dec = ToTerm("decimal");
            var vacio = ToTerm("void");
            var caracter = ToTerm("caracter");
            var privado = ToTerm("privado");
            var publico = ToTerm("publico");
            var protegido = ToTerm("protegido");
            var llamar = ToTerm("llamar");

            var apar = ToTerm("(");
            var cpar = ToTerm(")");
            var alla = ToTerm("{");
            var clla = ToTerm("}");
            var cora = ToTerm("[");
            var corc = ToTerm("]");
            var TRUE = ToTerm("true");
            var FALSE = ToTerm("false");
            var dosp = ToTerm(":");
            var ppt = ToTerm(";");
            var NULL = ToTerm("NULL");
            var retorno = ToTerm("retornar");
            var clase = ToTerm("clase");
            var asig = ToTerm("=");
            var nuevo = ToTerm("new");

            NumberLiteral numero = TerminalFactory.CreateCSharpNumber("numero");
            IdentifierTerminal id = TerminalFactory.CreateCSharpIdentifier("id");
            var tstring = new StringLiteral("tstring", "\"", StringOptions.AllowsDoubledQuote);
            var tchar = new StringLiteral("tchar", "'", StringOptions.AllowsDoubledQuote);

            
            var INICIO = new NonTerminal("INICIO");
            var BODYSENT = new NonTerminal("SENTENCIAS");
            var BODY = new NonTerminal("SENTBODY");
            var EXP = new NonTerminal("EXP");
            var SENTENCIAS = new NonTerminal("SENTENCIAS");
            var SENTENCIA = new NonTerminal("SENTENCIA");
            var DECLARAR = new NonTerminal("DECLARAR");
            var DECLARAR_ASIG = new NonTerminal("DECLARAR_ASIG");
            var DECLARAR_ARRAY = new NonTerminal("DECLARAR_ARRAY");
            var ARRAY = new NonTerminal("ARRAY");
            var L_ARRAY = new NonTerminal("L_ARRAY");
            var STRUCT = new NonTerminal("CLASE");
            var IMPRIMIR = new NonTerminal("IMPRIMIR");
            var RETORNO = new NonTerminal("RETORNO");

            var TIPO_V = new NonTerminal("TIPO_V");
            var LISTA_ID = new NonTerminal("LISTA_ID");
            var LISTA_CAS = new NonTerminal("LISTA_CAS");
            var CASILLA = new NonTerminal("CASILLA");

            //para manejar las sentencias
            var SENT = new NonTerminal("SENT");
            //ciclos
            var IF = new NonTerminal("IF");
            var WHILE = new NonTerminal("WHILE");
            var DOWHILE = new NonTerminal("DOWHILE");
            var REPEAT = new NonTerminal("REPEAT");
            var FOR = new NonTerminal("FOR");
            var LOOP = new NonTerminal("LOOP");
            var COUNT = new NonTerminal("COUNT");
            var DOWHILEX = new NonTerminal("DOWHILEX");
            var WHILEX = new NonTerminal("WHILEX");
            var METODO = new NonTerminal("METODO");
            var PARAMETROS = new NonTerminal("PARAMETROS");
            var PARAMETRO = new NonTerminal("PARAMETRO");
            var PRINCIPAL = new NonTerminal("MAIN");
            var ACCESO_OBJ = new NonTerminal("ACCESO_OBJ");
            var LISTA_ACCESO = new NonTerminal("LISTA_ACCESO");
            var ACCESO = new NonTerminal("ACCESO");
            var CALLFUN = new NonTerminal("CALLFUN");
            var PARAMETROS2 = new NonTerminal("PARAMETROS2");
            var asigacion_objeto = new NonTerminal("ASIGNACION_OBJECTO");
            var ASIGNACION = new NonTerminal("ASIGNACION");
            var BREAK = new NonTerminal("BREAK");
            var CONTINUAR = new NonTerminal("CONTINUAR");
            var CASE = new NonTerminal("CASE");
            var CASO = new NonTerminal("CASO");
            var CASOS = new NonTerminal("CASOS");
            var DEFAULT = new NonTerminal("DEFAULT");
            var VAL_CASE = new NonTerminal("VALCASE");
            var LISTA_COR = new NonTerminal("LISTA_COR");
            var CORCHETES = new NonTerminal("CORCHETES");

            //nuevos

            var visibilidad = new NonTerminal("VISIBILIDAD");
            var SENTENCIAS_CLASE = new NonTerminal("SENTENCIAS_CLASE");
            var CLASE_SENT = new NonTerminal("CLASE_SENT");
            var TIPO_M = new NonTerminal("TIPO_METODO");
            var IMPORTAR = new NonTerminal("IMPORTAR");
            var CONSTRUCTOR = new NonTerminal("CONSTRUCTOR");
            var DECREMENTOS = new NonTerminal("DECREMENTOS");
            var OVERRIDE = new NonTerminal("OVERRIDE");
            var INSTANCIA = new NonTerminal("INSTANCIA");
            var ELIF = new NonTerminal("ELIF");
            var ELSE = new NonTerminal("ELSE");
            var L_ELIF = new NonTerminal("L_ELIF");
            var CONDFOR = new NonTerminal("CONDFOR");
            var LLAVE = new NonTerminal("LLAVE");
            var ACCESO_ARRAY = new NonTerminal("ACCESO_ARRAY");
            var ASIG_ARRAY = new NonTerminal("ASIG_ARRAY");
            var ESTE = new NonTerminal("ESTE");
            var LLAMAR = new NonTerminal("LLAMAR");
            var AUXILIAR_INSTANCIA = new NonTerminal("instance");
            var SENTENCIA_HEAD = new NonTerminal("HEAD");
            var HEREDA = new NonTerminal("HEREDA");



            this.Root = INICIO;


            visibilidad.Rule = privado
                            | publico
                            | protegido
                            | Empty;

            INICIO.Rule = BODYSENT;

            BODYSENT.Rule = MakeStarRule(BODYSENT, BODY);

            BODY.Rule =                                  
                          PRINCIPAL
                        | STRUCT //LA UTILIZO PARA DECLARAR LOS METODOS DE LA CLASE
                        | IMPORTAR
                        | LLAMAR
                       ;

            IMPORTAR.Rule = "importar" + apar + tstring + cpar;

            PRINCIPAL.Rule =  "principal" + apar + cpar + alla + SENTENCIAS + clla;

            STRUCT.Rule = visibilidad + clase + id +HEREDA+ alla + SENTENCIAS_CLASE + clla;

            SENTENCIAS_CLASE.Rule = MakeStarRule(SENTENCIAS_CLASE, CLASE_SENT);

            CLASE_SENT.Rule =  visibilidad + CONSTRUCTOR
                             | visibilidad + TIPO_M + id + SENTENCIA_HEAD
                             | OVERRIDE

                         ;//VER QUE MAS ME PUEDE FALTAR

            HEREDA.Rule = ToTerm("hereda_de") + id
                       | Empty;

            SENTENCIA_HEAD.Rule = ppt//declarar
                                | apar + METODO//metodo
                                | asig + nuevo + id + AUXILIAR_INSTANCIA + ppt//instanciar
                                | asig + EXP + ppt//declarar asignar;   
                                | L_ARRAY + asig + LLAVE + ppt//declarar arreglo
                                ;
            METODO.Rule =  cpar + alla + SENTENCIAS + clla
                         | PARAMETROS+  cpar +alla + SENTENCIAS + clla; 

            TIPO_M.Rule =  TIPO_V//PRODUCCION PARA DECLARAR EL TIPO DE LOS METODOS
                         | vacio;

            OVERRIDE.Rule =  ToTerm("@sobrescribir") + visibilidad + TIPO_M + id+ apar + METODO;


            CONSTRUCTOR.Rule = id + apar + cpar + alla + SENTENCIAS + clla
                             | id + apar + PARAMETROS + cpar + alla + SENTENCIAS + clla
            ;


            DECLARAR.Rule = visibilidad + TIPO_V + LISTA_ID
                            ;//PRODUCCION PARA DECLARAR

            LISTA_ID.Rule = MakeStarRule(LISTA_ID, ToTerm(","), id);// SE PUEDEN DECLARAR MAS DE UN ID

            INSTANCIA.Rule =  visibilidad +id + id + asig + nuevo + id + AUXILIAR_INSTANCIA;

            AUXILIAR_INSTANCIA.Rule =  apar + cpar
                                     | apar + PARAMETROS2 + cpar;

            ESTE.Rule = ToTerm("este") + ToTerm(".") + SENTENCIA;

            ASIGNACION.Rule = id + ToTerm("=") + EXP;//PRODUCCION PARA LA ASIGNACION

            DECLARAR_ASIG.Rule = visibilidad +TIPO_V + LISTA_ID + asig + EXP;//PRODUCCION PARA DECLARAR Y ASIGNAR

            SENTENCIAS.Rule = MakeStarRule(SENTENCIAS, SENTENCIA);

            SENTENCIA.Rule =   DECLARAR + ppt
                             | ASIGNACION + ppt
                             | DECLARAR_ASIG + ppt
                             | DECLARAR_ARRAY + ppt
                             | ASIG_ARRAY + ppt
                             | IF
                             | IMPRIMIR + ppt
                             | WHILE
                             | DOWHILE
                             | REPEAT
                             | FOR
                             | WHILEX
                             | RETORNO + ppt
                             | CALLFUN + ppt
                             | ACCESO_OBJ + ppt
                             | BREAK + ppt
                             | CONTINUAR + ppt
                             | asigacion_objeto + ppt
                             | DECREMENTOS+ ppt
                             | ESTE
                             //| LLAMAR
                            ;

            LLAMAR.Rule = llamar + apar + tstring + cpar + ppt;

            TIPO_V.Rule =
                          entero
                        | id
                        | str
                        | BOOL
                        | dec
                        ;

             DECREMENTOS.Rule = id + "++"
                             |  id + "--";

             DECLARAR_ARRAY.Rule = TIPO_V + id + L_ARRAY + asig + LLAVE;

             L_ARRAY.Rule = MakeStarRule(L_ARRAY, CASILLA);

             CASILLA.Rule = cora + EXP + corc;

             LLAVE.Rule = alla + PARAMETROS2 + clla;

             ACCESO_ARRAY.Rule = id + L_ARRAY;

             ASIG_ARRAY.Rule = id + L_ARRAY + asig + EXP;

             IMPRIMIR.Rule = ToTerm("imprimir") + apar + EXP + cpar;

            IF.Rule =
                        "Si" + apar + EXP + cpar + alla + SENTENCIAS + clla + ELSE
                    |   "Si" + apar + EXP + cpar + alla + SENTENCIAS + clla  + L_ELIF + ELSE
                    ;

            L_ELIF.Rule = MakeStarRule(L_ELIF, ELIF);

            ELIF.Rule = "Sino Si" + apar + EXP + cpar + alla + SENTENCIAS + clla;

            ELSE.Rule = "Sino" + alla + SENTENCIAS + clla
                      |  Empty;

            WHILE.Rule = ToTerm("Mientras") + apar + EXP + cpar + alla + SENTENCIAS + clla;

            DOWHILE.Rule = ToTerm("Hacer") + alla + SENTENCIAS + clla + ToTerm("Mientras") + apar + EXP + cpar + ppt;

            REPEAT.Rule = ToTerm("repetir") + alla + SENTENCIAS + clla + ToTerm("until") + apar + EXP + cpar + ppt;


            FOR.Rule = ToTerm("Para") + apar + CONDFOR + ppt + EXP + ppt + DECREMENTOS + cpar + alla + SENTENCIAS + clla;

            CONDFOR.Rule = ASIGNACION
                         | DECLARAR_ASIG;

            WHILEX.Rule = ToTerm("X") + apar + EXP + ToTerm(",") + EXP + cpar + alla + SENTENCIAS + clla;

            RETORNO.Rule = retorno + EXP
                         | retorno;

            BREAK.Rule = ToTerm("break");

            CONTINUAR.Rule = ToTerm("continue");

            PARAMETROS.Rule = MakeStarRule(PARAMETROS, ToTerm(","), PARAMETRO);

            PARAMETRO.Rule = TIPO_V + id;

            PARAMETROS2.Rule = MakeStarRule(PARAMETROS2, ToTerm(","), EXP);

            EXP.Rule =    EXP + ToTerm("||") + EXP
                        | EXP + ToTerm("&&") + EXP
                        | EXP + ToTerm("??") + EXP
                        | EXP + ToTerm("==") + EXP
                        | EXP + ToTerm("!=") + EXP
                        | EXP + ToTerm(">") + EXP
                        | EXP + ToTerm("<") + EXP
                        | EXP + ToTerm(">=") + EXP
                        | EXP + ToTerm("<=") + EXP
                        | EXP + ToTerm("+") + EXP
                        | EXP + ToTerm("-") + EXP
                        | EXP + ToTerm("*") + EXP
                        | EXP + ToTerm("/") + EXP
                        | EXP + ToTerm("%") + EXP
                        | EXP + ToTerm("^") + EXP
                        | apar + EXP + cpar
                        | ToTerm("-") + EXP
                        | ToTerm("NOT") + EXP
                        | id
                        | numero
                        | tstring
                        | tchar
                        | TRUE
                        | FALSE
                        | ACCESO_OBJ
                        | CALLFUN
                        | LLAVE
                        | ACCESO_ARRAY
                       ;

            CALLFUN.Rule = id + apar + cpar
                          |id + apar + PARAMETROS2 + cpar
                        ;
            

            ACCESO_OBJ.Rule = id + ToTerm(".") + LISTA_ACCESO
                            | CALLFUN + ToTerm(".") + LISTA_ACCESO
                            ;

            LISTA_ACCESO.Rule = MakeListRule(LISTA_ACCESO, ToTerm("."), ACCESO);

            asigacion_objeto.Rule = ACCESO_OBJ + ToTerm("=") + EXP;

            ACCESO.Rule = id
                        | CALLFUN;

            SENTENCIA.ErrorRule = SyntaxError + SENTENCIA;

            BODY.ErrorRule = SyntaxError + BODY;

            RegisterOperators(1, Associativity.Left, "||");                 //OR
            RegisterOperators(2, Associativity.Left, "&&");                 //AND
            RegisterOperators(3, Associativity.Left, "??");
            RegisterOperators(4, Associativity.Left, "==", "!=");           //IGUAL, DIFERENTE
            RegisterOperators(5, Associativity.Left, ">", "<", ">=", "<="); //MAYORQUES, MENORQUES
            RegisterOperators(6, Associativity.Left, "+", "-");             //MAS, MENOS
            RegisterOperators(7, Associativity.Left, "*", "/");             //POR, DIVIDIR
            RegisterOperators(8, Associativity.Left, "^");
            RegisterOperators(9, Associativity.Right, "!");                 //NOT


            this.MarkPunctuation("(", ")", ";", ":", "{", "}", "=",".",",","[","]", "@sobrescribir");
            this.MarkPunctuation("Si", "Sino", "Sino Si", "Mientras", "Hacer","para","llamar","clase","hereda_de");
            //this.MarkPunctuation("whilex", "whilexorand", "repeat", "count", "loop","create","Principal");
            this.MarkTransient(SENTENCIA, CONDFOR, BODY,ARRAY,ACCESO,CASILLA);

        }
    }
}
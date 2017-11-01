using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;


namespace Proyecto2_compi2_2sem_2017.Compilador
{
    class GramaticaTre : Grammar
    {
        public GramaticaTre() : base(caseSensitive: false)
        {
            CommentTerminal COMENTARIO_SIMPLE = new CommentTerminal("comentario_simple", "##", "\n", "\r\n");
            CommentTerminal COMENTARIO_MULT = new CommentTerminal("comentario_mult", "{-", "-}");
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


            var tree = ToTerm(".tree");
            var olc= ToTerm(".olc");
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
            var retorno = ToTerm("return");
            var clase = ToTerm("clase");
            var asig = ToTerm("=>");
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
            var STRUCT = new NonTerminal("STRUCT");
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
            var WHILEXORAND = new NonTerminal("WHILEXOR");
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
            var SUPER = new NonTerminal("SUPER");
            var LISTA_IMPORT = new NonTerminal("IMPORTS");
            var SENT_IMPOTAR = new NonTerminal("SENT_IMPORT");
            this.Root = INICIO;


            visibilidad.Rule = privado
                            | publico
                            | protegido;

            INICIO.Rule = BODYSENT;

            BODYSENT.Rule = MakeStarRule(BODYSENT, BODY);

            BODY.Rule =
                         PRINCIPAL
                       | STRUCT //LA UTILIZO PARA DECLARAR LOS METODOS DE LA CLASE
                       | IMPORTAR
                       ;

            IMPORTAR.Rule = "importar" + LISTA_IMPORT;

            LISTA_IMPORT.Rule = MakeStarRule(LISTA_IMPORT,ToTerm(",")+SENT_IMPOTAR);

            SENT_IMPOTAR.Rule = id + tree
                                |id + olc ;
            //| desde el http

            PRINCIPAL.Rule = "principal" + cora + corc + alla + SENTENCIAS_CLASE + clla;

            STRUCT.Rule = clase + id + cora +  corc+ dosp+ SENTENCIAS_CLASE//clase simple
                        | clase + id + cora + id +corc + dosp + SENTENCIAS_CLASE;//clase que hereda

            SENTENCIAS_CLASE.Rule = MakeStarRule(SENTENCIAS_CLASE, CLASE_SENT);

            METODO.Rule = visibilidad + TIPO_M + apar + cpar + alla + SENTENCIAS + clla
                        | visibilidad + TIPO_M + apar + cpar + PARAMETROS + alla + SENTENCIAS + clla; ;

            TIPO_M.Rule = TIPO_V//PRODUCCION PARA DECLARAR EL TIPO DE LOS METODOS
                        | vacio;

            OVERRIDE.Rule = "/**" + "sobrescribir" + "**/" +METODO;

            CONSTRUCTOR.Rule = ToTerm("__")+  id + cora + corc + dosp + SENTENCIAS
                             | ToTerm("__") + id + cora + PARAMETROS + corc + dosp + SENTENCIAS
                             ;
            INSTANCIA.Rule = id + id + asig + nuevo + id + cora + corc
                           | id + id + asig + nuevo + id + cora + PARAMETROS2 + corc;

            DECLARAR.Rule = TIPO_V + LISTA_ID;//PRODUCCION PARA DECLARAR

            LISTA_ID.Rule = MakeStarRule(LISTA_ID, ToTerm(","), id);// SE PUEDEN DECLARAR MAS DE UN ID

            ASIGNACION.Rule = id + ToTerm("=>") + EXP;//PRODUCCION PARA LA ASIGNACION

            DECLARAR_ASIG.Rule = TIPO_V + LISTA_ID + asig + EXP;//PRODUCCION PARA DECLARAR Y ASIGNAR

            SENTENCIAS.Rule = MakeStarRule(SENTENCIAS, SENTENCIA);

            SENTENCIA.Rule = DECLARAR + ppt
                            | DECLARAR_ASIG + ppt
                            | DECLARAR_ARRAY + ppt
                            | ASIGNACION + ppt
                            | IF
                            | IMPRIMIR + ppt
                            | FOR
                            | CASE
                            | WHILE
                            | DOWHILE
                            | WHILEX
                            | REPEAT
                            | RETORNO + ppt
                            | CALLFUN + ppt
                            | ACCESO_OBJ + ppt
                            | BREAK + ppt
                            | CONTINUAR + ppt
                            | asigacion_objeto + ppt
                            | DECREMENTOS + ppt
                            ;

            TIPO_V.Rule =
                          entero
                        | id
                        | str
                        | BOOL
                        ;

            DECREMENTOS.Rule = id + "++"
                            | id + "--";

            DECLARAR_ARRAY.Rule = TIPO_V + id + L_ARRAY + asig + LLAVE;

            L_ARRAY.Rule = MakeStarRule(L_ARRAY, CASILLA);

            CASILLA.Rule = cora + EXP + corc;

            LLAVE.Rule = alla + PARAMETROS2 + clla;

            ACCESO_ARRAY.Rule = id + L_ARRAY;

            ASIG_ARRAY.Rule = id + L_ARRAY + asig + EXP;

            IMPRIMIR.Rule = ToTerm("imprimir") + apar + EXP + cpar;

            IF.Rule =
                        "Si" + cora + EXP + corc + dosp + SENTENCIAS 
                    |   "Si" + cora + EXP + corc + dosp + SENTENCIAS + L_ELIF
                    |   "Si" + cora + EXP + corc + dosp + SENTENCIAS + L_ELIF + ELSE;
            ;

            L_ELIF.Rule = MakeStarRule(L_ELIF, ELIF);

            ELIF.Rule = "Si_NO_Si" + cora + EXP + corc + dosp + SENTENCIAS;

            ELSE.Rule = "Si_NO" + dosp + SENTENCIAS;

            WHILE.Rule = ToTerm("MIENTRAS") + cora + EXP + cora + dosp + SENTENCIAS;

            DOWHILE.Rule = ToTerm("HACER") + dosp + SENTENCIAS  + ToTerm("MIENTRAS") + cora + EXP + corc;

            WHILEX.Rule = ToTerm("X") + apar + EXP + ToTerm(",") + EXP + cpar + alla + SENTENCIAS + clla;

            REPEAT.Rule = ToTerm("REPETIR") + dosp + SENTENCIAS + clla + ToTerm("HASTA") + cora + EXP + corc;

            FOR.Rule = ToTerm("PARA") + cora + CONDFOR + dosp +cora + EXP+corc + dosp +apar+ DECREMENTOS + cpar + corc +dosp+ SENTENCIAS;

            //me falta loop

            CONDFOR.Rule = ASIGNACION
                         | DECLARAR_ASIG;

            RETORNO.Rule = retorno + EXP
                        | retorno;

            PARAMETROS.Rule = MakeStarRule(PARAMETROS, ToTerm(","), PARAMETRO);

            PARAMETRO.Rule = TIPO_V + id
                           ;
            BREAK.Rule = ToTerm("break");

            CONTINUAR.Rule = ToTerm("continue");

            EXP.Rule =    EXP + EXP + ToTerm("||")
                        | EXP + EXP + ToTerm("&&") 
                        | EXP + EXP + ToTerm("??") 
                        | EXP + EXP + ToTerm("==")
                        | EXP + EXP + ToTerm("!=")
                        | EXP + EXP + ToTerm(">") 
                        | EXP + EXP + ToTerm("<") 
                        | EXP + EXP + ToTerm(">=")
                        | EXP + EXP + ToTerm("<=")
                        | EXP + EXP + ToTerm("+")
                        | EXP + EXP + ToTerm("-")
                        | EXP + EXP + ToTerm("*")
                        | EXP + EXP + ToTerm("/")
                        | EXP + EXP + ToTerm("%")
                        | EXP + EXP + ToTerm("pow") 
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
                       ;

            CALLFUN.Rule = id + cora + corc
                          | id + cora + PARAMETROS2 + corc
                        ;
            PARAMETROS2.Rule = MakeStarRule(PARAMETROS2, ToTerm(","), EXP);

            ACCESO_OBJ.Rule = id + ToTerm(".") + LISTA_ACCESO
                            | CALLFUN + ToTerm(".") + LISTA_ACCESO
                            ;

            LISTA_ACCESO.Rule = MakeListRule(LISTA_ACCESO, ToTerm("."), ACCESO);

            asigacion_objeto.Rule = ACCESO_OBJ + ToTerm("=") + EXP;

            ACCESO.Rule = id
                        | CALLFUN;

            SENTENCIA.ErrorRule = SyntaxError + SENTENCIA;

            BODY.ErrorRule = SyntaxError + BODY;

            RegisterOperators(1, Associativity.Left, "||", "|?");                 //OR
            RegisterOperators(2, Associativity.Left, "&&", "&?");                 //AND
            RegisterOperators(3, Associativity.Left, "|&");
            RegisterOperators(4, Associativity.Left, "==", "!=");           //IGUAL, DIFERENTE
            RegisterOperators(5, Associativity.Left, ">", "<", ">=", "<="); //MAYORQUES, MENORQUES
            RegisterOperators(6, Associativity.Left, "+", "-");             //MAS, MENOS
            RegisterOperators(7, Associativity.Left, "*", "/");             //POR, DIVIDIR
            RegisterOperators(8, Associativity.Left, "^");
            RegisterOperators(9, Associativity.Right, "!");                 //NOT


            this.MarkPunctuation("(", ")", ";", ":", "{", "}", "=", ".", ",", "[", "]", "then");
            this.MarkPunctuation("outStr", "if", "else", "switch", "case", "default", "do", "while", "for");
            this.MarkPunctuation("whilex", "whilexorand", "repeat", "count", "loop", "create", "Principal");
            this.MarkPunctuation("..");
            this.MarkPunctuation("element", "of", "array", "outNum", "inStr", "inNum", "getLength", "show");
            this.MarkTransient(SENTENCIA, CONDFOR, BODY, ARRAY, SENT, ACCESO);

        }

    }
}

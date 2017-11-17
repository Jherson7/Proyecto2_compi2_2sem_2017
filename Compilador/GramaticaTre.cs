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
            CommentTerminal COMENTARIO_MULT = new CommentTerminal("comentario_mult", "{--", "--}");
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
            MarkReservedWords("funcion");

            MarkReservedWords("sobrescribir");
            MarkReservedWords("retornar");
            MarkReservedWords("principal");
            MarkReservedWords("llamar");
            MarkReservedWords("importar");
            MarkReservedWords("nuevo");

            MarkReservedWords("si");
            MarkReservedWords("Sino_Si");
            MarkReservedWords("Sino");
            MarkReservedWords("MIENTRAS");
            MarkReservedWords("HACER");
            MarkReservedWords("repetir");
            MarkReservedWords("until");
            MarkReservedWords("Para");
            MarkReservedWords("imprimir");
            MarkReservedWords("loop");
            MarkReservedWords("super");
            MarkReservedWords("self"); ;

            var str = ToTerm("cadena");
            var entero = ToTerm("entero");
            var BOOL = ToTerm("booleano");
            var dec = ToTerm("decimal");
            var vacio = ToTerm("metodo");
            var caracter = ToTerm("caracter");
            var privado = ToTerm("privado");
            var publico = ToTerm("publico");
            var protegido = ToTerm("protegido");


            var tree = ToTerm(".tree");
            var olc = ToTerm(".olc");
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
            var nuevo = ToTerm("nuevo");
            var loop = ToTerm("loop");

            NumberLiteral numero = TerminalFactory.CreateCSharpNumber("numero");
            IdentifierTerminal id = TerminalFactory.CreateCSharpIdentifier("id");
            var tstring = new StringLiteral("tstring", "\"", StringOptions.AllowsDoubledQuote);
            var tchar = new StringLiteral("tchar", "'", StringOptions.AllowsDoubledQuote);


            RegexBasedTerminal archivo = new RegexBasedTerminal("archivo", "[a-zA-Z][0-9a-zA-Z]*.(tree|olc)");
            RegexBasedTerminal ruta = new RegexBasedTerminal("ruta", "C://([a-zA-Z][0-9a-zA-Z]*/)*[a-zA-Z][0-9a-zA-Z]*.(tree|olc)");
            RegexBasedTerminal url = new RegexBasedTerminal("url", "http://([a-zA-Z][0-9a-zA-Z]*/)*[a-zA-Z][0-9a-zA-Z]*.(tree|olc)");


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
            var SWITCH = new NonTerminal("SWITCH");
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
            var LISTA_ARCHIVOS = new NonTerminal("lista_imports");
            var ARCHIVO = new NonTerminal("clase_importar");
            var IDENT_SENT = new NonTerminal("IDENT");
            var SENTENCIA_HEAD = new NonTerminal("HEAD");
            var AUXILIAR_INSTANCIA = new NonTerminal("instance");
            var IDENT_LISTA = new NonTerminal("IDENT");
            var PARSE_INT = new NonTerminal("PARSE_INT");
            var PARSE_DOUBLE = new NonTerminal("PARSE_DOUBLE");
            var INT_TO_STR = new NonTerminal("INT_TO_STR");
            var double_TO_STR = new NonTerminal("DOUBLE_TO_STR");
            var double_TO_INT = new NonTerminal("DOUBLE_TO_INT");
            var NATIVAS = new NonTerminal("NATIVAS");
            var HEREDA = new NonTerminal("HEREDA");
            var ESTE = new NonTerminal("ESTE");
            var auxiliar = new NonTerminal("auxiliar");
            var NUEVO = new NonTerminal("NUEVO");
            var DIM = new NonTerminal("DIM");
            var L_DIM = new NonTerminal("L_DIM");

            this.Root = INICIO;


            visibilidad.Rule = privado
                            | publico
                            | protegido
                            ;
            INICIO.Rule = LISTA_IMPORT + BODYSENT
                        | BODYSENT;

            BODYSENT.Rule = MakeStarRule(BODYSENT, BODY);

            BODY.Rule =
                         STRUCT //LA UTILIZO PARA DECLARAR Las clases
                                //| IMPORTAR 
                       ;

            STRUCT.Rule = clase + id + cora + HEREDA + corc + dosp + Eos + IDENT_SENT;//clase que hereda

            HEREDA.Rule = id
                        | Empty;

            IDENT_SENT.Rule = Indent + SENTENCIAS_CLASE + Dedent;

            SENTENCIAS_CLASE.Rule = MakeStarRule(SENTENCIAS_CLASE, CLASE_SENT);

            CLASE_SENT.Rule =
                              ToTerm("__") + CONSTRUCTOR
                            | visibilidad + TIPO_M + id + SENTENCIA_HEAD
                            | OVERRIDE
                             // | PRINCIPAL
                             ;

            SENTENCIA_HEAD.Rule = Eos//declarar
                                | cora + METODO//metodo
                                | asig + nuevo + id + cora + PARAMETROS2 + corc + Eos//instanciar objeto
                                | asig + EXP + Eos//declarar asignar;   
                                | L_ARRAY + Eos//declarar arreglo
                                ;

            CONSTRUCTOR.Rule = id + cora + PARAMETROS + corc + dosp + Eos + IDENT_LISTA
                             ;

            METODO.Rule = PARAMETROS + corc + dosp + Eos + IDENT_LISTA;

            TIPO_M.Rule =  //PRODUCCION PARA DECLARAR EL TIPO DE LOS METODOS
                           ToTerm("funcion") + id
                         | vacio
                         | TIPO_V
                         ;

            IMPORTAR.Rule = "importar" + LISTA_IMPORT + Eos;

            LISTA_IMPORT.Rule = MakeStarRule(LISTA_IMPORT, ToTerm(",") + SENT_IMPOTAR);

            SENT_IMPOTAR.Rule = ruta
                                | archivo
                                | url;

            LISTA_IMPORT.Rule = MakeStarRule(LISTA_IMPORT, IMPORTAR);

            IMPORTAR.Rule = ToTerm("importar") + LISTA_ARCHIVOS + Eos;

            LISTA_ARCHIVOS.Rule = MakeStarRule(LISTA_ARCHIVOS, ToTerm(","), ARCHIVO);

            ARCHIVO.Rule = ruta
                | archivo
                | url;
            /*
            PRINCIPAL.Rule = "principal" + cora + corc + alla + IDENT_SENT + clla;
            */


            OVERRIDE.Rule = ToTerm("/**") + ToTerm("Sobreescribir") + ToTerm("**/") + Eos +
                            visibilidad + TIPO_M + id + cora + METODO;


            PARAMETROS.Rule = MakeStarRule(PARAMETROS, ToTerm(","), PARAMETRO);

            PARAMETRO.Rule = TIPO_V + id
                             | TIPO_V + L_DIM + id
                           ;
            L_DIM.Rule = MakeStarRule(L_DIM, DIM);

            DIM.Rule = cora + corc;

            TIPO_V.Rule = id
                       | entero
                       | str
                       | BOOL
                       | ToTerm("decimal")
                       ;


            IDENT_LISTA.Rule = Indent + SENTENCIAS + Dedent;

            SENTENCIAS.Rule = MakeStarRule(SENTENCIAS, SENTENCIA);

            SENTENCIA.Rule = DECLARAR + Eos
                            | ASIGNACION + Eos
                            | DECLARAR_ASIG + Eos
                            | RETORNO + Eos
                            | IF
                            | WHILE
                            | DOWHILE
                            | REPEAT
                            | FOR
                            | LOOP
                            | IMPRIMIR + Eos
                            | CONTINUAR + Eos
                            | BREAK + Eos
                            | SWITCH
                            | NATIVAS + Eos
                            | CALLFUN + Eos
                           // | INSTANCIA + Eos
                            | DECREMENTOS + Eos
                            | ESTE 
                            | SUPER
                            | DECLARAR_ARRAY + Eos
                            | ASIG_ARRAY + Eos
                            | asigacion_objeto + Eos
                            | ACCESO_OBJ + Eos
                            

                            ;

            DECLARAR.Rule = TIPO_V + LISTA_ID;//PRODUCCION PARA DECLARAR

            DECLARAR_ASIG.Rule = TIPO_V + LISTA_ID + asig + EXP;

            LISTA_ID.Rule = MakeStarRule(LISTA_ID, ToTerm(","), id);// SE PUEDEN DECLARAR MAS DE UN ID

//            INSTANCIA.Rule = id + id + asig + nuevo + id + cora + PARAMETROS2 + corc;

            DECLARAR_ARRAY.Rule = TIPO_V + id + L_ARRAY; //+ asig + LLAVE;

            L_ARRAY.Rule = MakeStarRule(L_ARRAY, CASILLA);

            CASILLA.Rule = cora + EXP + corc;

            ACCESO_ARRAY.Rule = id + L_ARRAY;

            ASIG_ARRAY.Rule = id + L_ARRAY + asig + EXP;


           
            ASIGNACION.Rule = id + ToTerm("=>") + EXP;

            /*ciclos de bifurcacion*/
            IF.Rule =
                        "Si" +  EXP +  dosp + Eos + IDENT_LISTA
                   |    "Si" +  EXP +  dosp + Eos + IDENT_LISTA + L_ELIF
                   |    "Si" +  EXP +  dosp + Eos + IDENT_LISTA + L_ELIF + ELSE;
            ;

            L_ELIF.Rule = MakeStarRule(L_ELIF, ELIF);

            ELIF.Rule = "Si_NO_Si" + EXP +  dosp + Eos + IDENT_LISTA;

            ELSE.Rule = "Si_NO" + dosp + Eos + IDENT_LISTA;

            WHILE.Rule = ToTerm("MIENTRAS") + cora + EXP + corc + dosp + Eos + IDENT_LISTA;

            DOWHILE.Rule = ToTerm("HACER") + dosp + Eos + IDENT_LISTA + ToTerm("MIENTRAS") + cora + EXP + corc + Eos;

            REPEAT.Rule = ToTerm("REPETIR")+ dosp+ Eos + IDENT_LISTA + ToTerm("HASTA") + cora + EXP + corc + Eos;

            FOR.Rule = ToTerm("PARA") + cora + CONDFOR + dosp + cora + EXP + corc + dosp + DECREMENTOS + corc + dosp + Eos + IDENT_LISTA;

            DECREMENTOS.Rule = apar + id + "++" + cpar
                            | apar + id + "--" + cpar;

            LOOP.Rule = loop + dosp + Eos + IDENT_LISTA;

            SWITCH.Rule = ToTerm("ELEGIR") + ToTerm("CASO") + EXP + dosp + Eos + CASE;

            CASE.Rule =  Indent + CASOS + DEFAULT + Dedent
                        |Indent + CASOS + Dedent;

            CASOS.Rule = MakeStarRule(CASOS, CASO);

            CASO.Rule = EXP + dosp + Eos + IDENT_LISTA;

            DEFAULT.Rule = ToTerm("defecto") + dosp + Eos + IDENT_LISTA;

            CONDFOR.Rule = ASIGNACION
                         | DECLARAR_ASIG;


            IMPRIMIR.Rule =  ToTerm("out_string") + cora + EXP + corc
                            | ToTerm("imprimir") + cora + EXP + corc;

            BREAK.Rule = ToTerm("salir");

            CONTINUAR.Rule = ToTerm("continuar");

            NATIVAS.Rule = PARSE_INT
                         | PARSE_DOUBLE
                         | INT_TO_STR
                         | double_TO_INT
                         | double_TO_STR
                         ;

            PARSE_INT.Rule = ToTerm("ParseInt") + cora + EXP + corc;

            PARSE_DOUBLE.Rule = ToTerm("ParseDouble") + cora + EXP + corc;

            INT_TO_STR.Rule = ToTerm("intToSTR") + cora + EXP + corc;

            double_TO_STR.Rule = ToTerm("doubleToSTR") + cora + EXP + corc;

            double_TO_INT.Rule = ToTerm("doubleToINT") + cora + EXP + corc;

            EXP.Rule =   EXP + EXP + ToTerm("OR")
                       | EXP + EXP + ToTerm("AND")
                       | EXP + EXP + ToTerm("XOR")
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
                       | alla +EXP + clla
                       | cora + EXP+ corc
                       | ToTerm("-") + EXP
                       | ToTerm("NOT") + EXP
                       | id
                       | numero
                       | tstring
                       | tchar
                       | TRUE
                       | FALSE
                       | ACCESO_ARRAY
                       | ACCESO_OBJ
                       | CALLFUN
                       | NATIVAS
                       | NUEVO
                       | DECREMENTOS
                      ;

            CALLFUN.Rule = id + cora + PARAMETROS2 + corc//probar si funciona con las condiciones que tengo :D
;
            NUEVO.Rule = nuevo + id + cora + PARAMETROS2 + corc;
                    
            PARAMETROS2.Rule = MakeStarRule(PARAMETROS2, ToTerm(","), EXP);

            RETORNO.Rule = ToTerm("retornar")
                        |  ToTerm("retornar") + EXP;

            /*self y super*/


            ESTE.Rule = ToTerm("self") + ToTerm(".") + SENTENCIA;

            SUPER.Rule =
                          ToTerm("super") + ToTerm(".") + id + Eos  
                        | ToTerm("super") + cora + PARAMETROS2 + corc + Eos
                        | ToTerm("super") + ToTerm(".") + SENTENCIA
                        
                        ;

            ACCESO_OBJ.Rule = id + ToTerm(".") + LISTA_ACCESO
                            | CALLFUN + ToTerm(".") + LISTA_ACCESO
                            | ACCESO_ARRAY + ToTerm(".") + LISTA_ACCESO
                            ;

            LISTA_ACCESO.Rule = MakeListRule(LISTA_ACCESO, ToTerm("."), ACCESO);

            ACCESO.Rule = id
                        | CALLFUN;

            asigacion_objeto.Rule = ACCESO_OBJ + asig + EXP;




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


            this.MarkPunctuation("(", ")", ";", ":", "{", "}", "=>", ".", ",", "[", "]", "sobrescribir","/**","**/");
            this.MarkPunctuation("repetir", "hasta", "out_string", "principal", "loop", "retornar", "self");
            this.MarkPunctuation("Si", "Sino", "Sino Si", "Mientras", "Hacer", "para", "llamar", "clase", "hereda_de");
            this.MarkPunctuation("__","SUPER", "funcion", "imprimir","elegir","caso");
            this.MarkTransient(SENTENCIA, CONDFOR, BODY,IDENT_LISTA,IDENT_SENT, CASILLA);//ARRAY, SENT, ACCESO);
            this.MarkTransient(ARRAY, ACCESO);
        }
        public override void CreateTokenFilters(LanguageData language, TokenFilterList filters)
        {
            var outlineFilter = new CodeOutlineFilter(language.GrammarData,
                OutlineOptions.ProduceIndents | OutlineOptions.CheckBraces,
                ToTerm(@"\"));
            filters.Add(outlineFilter);
        }

    }
}

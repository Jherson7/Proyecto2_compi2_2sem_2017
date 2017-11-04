using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony.Ast;

namespace Proyecto2_compi2_2sem_2017.Ejecucion3D
{
    class Gramatica3d:Grammar
    {
        public Gramatica3d() : base(caseSensitive: false)
        {
            CommentTerminal COMENTARIO_SIMPLE = new CommentTerminal("comentario_simple", "//", "\n", "\r\n");

            NonGrammarTerminals.Add(COMENTARIO_SIMPLE);

            MarkReservedWords("stack");
            MarkReservedWords("heap");
            MarkReservedWords("print");

            

            var heap = ToTerm("heap");
            var stack = ToTerm("stack");
            var vacio = ToTerm("void");

            var apar = ToTerm("(");
            var cpar = ToTerm(")");
            var alla = ToTerm("{");
            var clla = ToTerm("}");
            var cora = ToTerm("[");
            var corc = ToTerm("]");
            var ppt = ToTerm(";");
            var asig = ToTerm("=");
            var comilla = ToTerm("\"");
            var mod = ToTerm("%");

            NumberLiteral numero = TerminalFactory.CreateCSharpNumber("numero");
            IdentifierTerminal id = TerminalFactory.CreateCSharpIdentifier("id");
            RegexBasedTerminal etiqueta = new RegexBasedTerminal("etiqueta","L[0-9]+");

            var INICIO = new NonTerminal("INICIO");
            var E = new NonTerminal("E");
            var EXP = new NonTerminal("EXP");
            var CALLFUN = new NonTerminal("CALLFUN");

            var OBTENER_DE_STACK = new NonTerminal("GET_FROM_STACK");
            var OBTENER_DE_HEAP = new NonTerminal("GET_FROM_HEAP");
            var ASIGNAR_HEAP = new NonTerminal("PUT_TO_HEAP");
            var ASIGNAR_STACK = new NonTerminal("PUT_TO_STACK");
            var ASIGNACION = new NonTerminal("ASIGNACION");
            var COND = new NonTerminal("COND");
            var CONDICIONAL = new NonTerminal("SALTO_CONDICIONAL");
            var GOTO = new NonTerminal("GOTO");
            var LABEL = new NonTerminal("LABEL");
            var SENTECIAS = new NonTerminal("SENTENCIAS");
            var SENTECIA= new NonTerminal("SENTENCIA");
            var METODO = new NonTerminal("METODO");
            var SENT_BODY = new NonTerminal("SENT_BODY");
            var BODY = new NonTerminal("BODY");
            var PRINT = new NonTerminal("PRINT");


            this.Root = INICIO;

            INICIO.Rule = SENT_BODY;

            SENT_BODY.Rule = MakeStarRule(SENT_BODY, BODY);

            BODY.Rule =   LABEL
                        | METODO
                        | ASIGNAR_HEAP
                        | ASIGNAR_STACK
                        | OBTENER_DE_STACK   
                        | OBTENER_DE_HEAP
                        | ASIGNACION
                        | CONDICIONAL
                        | GOTO
                        | PRINT
                        ;

            PRINT.Rule = ToTerm("print") +apar+ comilla +mod + id+comilla + ToTerm(",")+E+cpar   ;

            ASIGNAR_HEAP.Rule = heap + cora + E + corc + asig + E;

            ASIGNAR_STACK.Rule = stack + cora + E + corc + asig + E;

            OBTENER_DE_STACK.Rule = E + asig + stack + cora + E + corc;

            OBTENER_DE_HEAP.Rule = E + asig + heap + cora + E + corc;

            ASIGNACION.Rule = E + asig + EXP;

            LABEL.Rule = etiqueta + ToTerm(":");

            EXP.Rule =
                          EXP + ToTerm("+") + EXP
                        | EXP + ToTerm("-") + EXP
                        | EXP + ToTerm("*") + EXP
                        | EXP + ToTerm("/") + EXP
                        | EXP + ToTerm("^") + EXP
                        | ToTerm("-") + E
                        | E
                        
                       ;

            E.Rule =      id
                        | numero;

            COND.Rule =   E + ToTerm("==") + E
                        | E + ToTerm("!=") + E
                        | E + ToTerm(">") + E
                        | E + ToTerm("<") + E
                        | E + ToTerm(">=") + E
                        | E + ToTerm("<=") + E
                        
                        ;

            CALLFUN.Rule = id + apar + cpar
                        ;

            CONDICIONAL.Rule = ToTerm("if") + COND + ToTerm("goto") + etiqueta;

            GOTO.Rule = ToTerm("goto") + etiqueta;

            SENTECIAS.Rule = MakeStarRule(SENTECIAS, SENTECIA);

            SENTECIA.Rule = LABEL
                        | ASIGNAR_HEAP
                        | ASIGNAR_STACK
                        | OBTENER_DE_STACK
                        | OBTENER_DE_HEAP
                        | ASIGNACION
                        | CONDICIONAL
                        | GOTO
                        | PRINT;

            SENTECIA.ErrorRule = SyntaxError + SENTECIA;

            METODO.Rule = vacio + id + apar + cpar + alla + SENTECIAS + clla;

            BODY.ErrorRule = SyntaxError + BODY;
            /*
            RegisterOperators(1, Associativity.Left, "==", "!=");           //IGUAL, DIFERENTE
            RegisterOperators(2, Associativity.Left, ">", "<", ">=", "<="); //MAYORQUES, MENORQUES
            RegisterOperators(3, Associativity.Left, "+", "-");             //MAS, MENOS
            RegisterOperators(4, Associativity.Left, "*", "/");             //POR, DIVIDIR
            RegisterOperators(5, Associativity.Left, "^");
            RegisterOperators(6, Associativity.Right, "!");                 //NOT
            */ //YA NO LLEVA PRECEDECIA PORQUE ES 3D

            this.MarkPunctuation("(", ")", ";", ":", "{", "}", "=", ".", ",", "[", "]", "@sobrescribir");
            this.MarkPunctuation("void", "goto","stack","if","heap","\"","%","print");
            
            //this.MarkPunctuation("whilex", "whilexorand", "repeat", "count", "loop","create","Principal");
            this.MarkTransient(SENTECIA,E,BODY);

        }

    }
}

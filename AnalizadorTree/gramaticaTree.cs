using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_compi2_2sem_2017.AnalizadorTree
{
    class gramaticaTree
    {



        /*
            SENTENCIA.Rule =  
                            | DECLARAR_ARRAY + Eos
                            | IF
                            | IMPRIMIR + Eos
                            | FOR
                            | CASE
                            | WHILE
                            | DOWHILE
                            | WHILEX
                            | REPEAT
                            | RETORNO + Eos
                            | CALLFUN + Eos
                            | ACCESO_OBJ + Eos
                            | BREAK + Eos
                            | CONTINUAR + Eos
                            | asigacion_objeto + Eos
                            | DECREMENTOS + Eos
                            ;

            TIPO_V.Rule =
                          entero
                        | id
                        | str
                        | BOOL
                        ;

            

            

            


            WHILEX.Rule = ToTerm("X") + apar + EXP + ToTerm(",") + EXP + cpar + alla + SENTENCIAS + clla;

            
            

            
            

           

            

            ACCESO_OBJ.Rule = id + ToTerm(".") + LISTA_ACCESO
                            | CALLFUN + ToTerm(".") + LISTA_ACCESO
                            ;

            LISTA_ACCESO.Rule = MakeListRule(LISTA_ACCESO, ToTerm("."), ACCESO);

            asigacion_objeto.Rule = ACCESO_OBJ + ToTerm("=") + EXP;

            ACCESO.Rule = id
                        | CALLFUN;*/
    }
}

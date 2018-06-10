/* 
<declaracoes>       := <declaracao> <declaracoes> | <declaracao>
<declaracao>        := <tipo>:<identificadores>;
<identificadores>   := <identificador>, <identificadores> | <identificador>
<identificador>     := <letra> (<letra> | <digito>)
<tipo>              := INTEIRO | REAL 
*/

using System.IO;
using AnalisadorSintatico;

public  class Declaracoes { }

public class Declaracao_r : Declaracoes
{
    public Declaracao d1;
    public Declaracoes d2;
}

public class Declaracao : Declaracoes
{
    public string tipo;
    public Identificadores idents;
}

public abstract class Identificadores { }

public class Identificador_r : Identificadores
{
    public Identificador ident1;
    public Identificadores ident2;
}

public class Identificador : Identificadores
{
    public string ident;
}

public  class Operador {}

public class Operador_aritmetico : Operador {
    public string oper_ari;
};


public  class Expressoes { }

public class Expressao : Expressoes { };

public class Expressao_r : Expressoes {

    public Expressao expr1;
    public Operador opr;
    public Expressao expr2;

};

public class Expressao_int : Expressao {

    public string inteiro;
}

public class Expressao_real : Expressao
{

    public string real;
}

public class Expressao_bool : Expressao
{

    public string booleano;
}

public class Expressao_ident : Expressao
{
    public Identificador ident;
    void getIdent() { }
   
};

public  class Instrucoes { }

public class Instrucao : Instrucoes { };

public class Instrucao_r : Instrucoes {

    public Instrucao inst1;
    public Instrucao inst2;

};

public class Instrucao_atr : Instrucao { };

public class Instrucao_se : Instrucao { };

public class Instrucao_se_a : Instrucao_se {

    public Expressoes exprs;
    public Instrucoes insts;

};

public class Instrucao_se_b : Instrucao_se
{

    public Expressoes exprs;
    public Instrucoes insts1;
    public Instrucoes insts2;

};


public class Instrucao_para : Instrucao {

    public Identificador ident;
    public Expressao expr1;
    public Expressao expr2;
    public Instrucoes inst;


};
public class Instrucao_enquanto : Instrucao {
    public Expressoes exprs;
    public Instrucoes inst;
    };

public class Instrucao_leitura : Instrucao {
    public Identificador ident;

};
public class Instrucao_escrita : Instrucao { };

public class Instrucao_escrita_a : Instrucao_escrita {

    public  string text;
};

public class Instrucao_escrita_b : Instrucao_escrita
{

    public  Expressao expr;
};


public  class Portugol {

    public Identificador ident;
    public Declaracoes decls;
    public Instrucoes inst;
}

public  class Atribuicao : Instrucao_atr {

   public  Identificador ident;
    public Expressoes expr;
};



public abstract class Expr : Node { };

public  class Temp : Expr {

    public int conta = 0;
    public string cont;
    public int Conta() { return conta; }
    public string last() { return cont; }

    public string toString ()
        {
        string c;
        conta++;
        c = conta.ToString();
        string c2;
        c2 = "t";
        c2 += c;
        cont = c2;
        return c2;

}



};

public class variavel {

    public string nome;
    public string tipo;


}




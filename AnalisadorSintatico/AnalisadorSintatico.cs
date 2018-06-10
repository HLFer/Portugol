using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalisadorSintatico
{
    class AnalisadorSintatico
    {
        //Inicialização das variáveis e objetos que serão utilizados
        public List<Token> tokens;
        Temp t = new Temp();
        int index;
        int guarda;
        int flagvariavel;
        int labels;
        int conta = 0;
        int contaLinha = 1;
        string cont;
        List<Node> listacodigo = new List<Node>();
        List<int> listaPrint = new List<int>();
        List<variavel> listaVariavel;
        Node codigo = new Node();
        public Portugol result  = new Portugol();
        public List<string> PalavrasReservadas; 

        //Função para carregar as Palavras reservadas em uma lista
        private List<string> CarregaPalavrasReservadas()
        {   
            //Cria uma lista de objetos enumeráveis com as palavras reservadas da Linguagem
            List<string> palavrasReservadas = new List<string>();

            //Insere as palavras reservadas na lista de objetos
            palavrasReservadas.Add("INTEIRO");
            palavrasReservadas.Add("REAL");
            palavrasReservadas.Add("PROGRAMA");
            palavrasReservadas.Add("FIM");
            palavrasReservadas.Add("INICIO");
            palavrasReservadas.Add("LEIA");
            palavrasReservadas.Add("IMPRIMA");
            palavrasReservadas.Add("VARIAVEIS");
            return palavrasReservadas;
        }

        //Método construtor para a Análise Sintática - Recebe a lista de tokens criada pela Análise Léxica
        public AnalisadorSintatico(List<Token> tokens)
        {
            this.tokens = tokens;
            this.tokens.Add(new Token(ClasseToken.FimDeArquivo, "$"));
            this.index = 0;
            this.PalavrasReservadas = CarregaPalavrasReservadas();
            this.Parse(); // executa a analise sintatica           
        }

        public AnalisadorSintatico()
        {
        }

        //Método executor da Análise Sintática
        private void Parse()
        {
            if (portugol() && tokens[index].nome == ClasseToken.FimDeArquivo)
            {
                Imprimir n = new Imprimir();
                n.imprimir(listacodigo);
                Console.WriteLine("Análise Sintática Completa! \nA gramática pertence à Linguagem.");
                Console.WriteLine("\nContém os Tokens:");
                for (int i = 0; i != listacodigo.Count; i++)
                {
                    string s;
                    int label;
                    s = listacodigo[i].valor;
                    label = listacodigo[i].labels;
                    Console.WriteLine("L" + label + ": " + s);
                }
            }
            else
                throw new System.Exception("A gramática não pertence à linguagem");
        }

        // metodo que reconhece o nao-terminal 
        public List<variavel> Variaveis()
        {
            List<variavel> aux = new List<variavel>();
           
            int inicio = 0, fim = 0;

            for (int i = 0; i != tokens.Count; i++)
            {
                if (tokens[i].valor.ToString() == "VARIAVEIS")
                {
                    inicio = i;
                }

                if (tokens[i].valor.ToString() == "INICIO")
                {
                    fim = i;
                }
            }

            for (int i = inicio; i != fim; i++)
            {
                if (tokens[i].valor.ToString() == "INTEIRO")
                {
                    do
                    {
                        i++;
                        if (tokens[i].nome.ToString() == "Identificador")
                        {
                            variavel n = new variavel();
                            n.nome = tokens[i].valor.ToString();
                            n.tipo = "INTEIRO";
                            aux.Add(n);

                        }
                      
                    } while (tokens[i].valor.ToString() != ";");

                }

                if (tokens[i].valor.ToString() == "REAL")
                {
                    do
                    {
                        i++;
                        if (tokens[i].nome.ToString() == "Identificador")
                        {
                            variavel n = new variavel();
                            n.nome = tokens[i].valor.ToString();
                            n.tipo = "REAL";
                            aux.Add(n);

                        }


                    } while (tokens[i].valor.ToString() != ";");

                }

            }
            return aux;


        }

        public static bool IsNumeric(object Expression)
        {
            double retNum;

            bool isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }


        //Método que verifica se o arquivo contém as palavras reservadas
        private bool portugol() {

            listaVariavel = Variaveis();
            if (compare("PROGRAMA"))
            {

                index++;
                Identificador ident = new Identificador();
                if (identificador(ref ident))
                {
                    result.ident = ident;
                    if (compare(";"))
                    {

                        index++;
                        if (compare("VARIAVEIS")) {

                            index++;
                            Declaracoes delcs = new Declaracoes();
                            if (declaracoes(delcs))
                            {

                                result.decls = delcs;
                                if (compare("INICIO"))
                                {

                                    index++;
                                    Instrucoes inst = new Instrucoes();
                                    if (instrucoes(inst))
                                    {
                                        result.inst = inst;
                                        if (compare("FIM"))
                                        {
                                            index++;
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                } 
            }
            return false;
        }

        //
        public bool notPrinted(int Guarda)
        {
            for (int i = 0; i != listaPrint.Count; i++)
            {
                if (listaPrint[i] == Guarda) {

                    return false;
                }
                
            }
            listaPrint.Add(Guarda);
            return true;
        }


        //Método que verifica as atribuições
        private bool atribuicao(Atribuicao atr)
        {
            Identificador n = new Identificador();
            int guarda;


            if (identificador(ref n))
            {
                atr.ident = n;

                if (tokens[index].valor.Equals('='))
                {
                    guarda = index - 1;
                    index++;
                    Expressoes expr = new Expressoes();
                    if (expressoes( expr))
                    {
                        atr.expr = expr;
                        if (tokens[index].valor.ToString() == ";")
                        {
                            index++;
                            contaLinha++;
                         
                            return true;
                        }
                        else throw new System.Exception("Declaracao: \';\' esperado na linha: "+contaLinha);
                    }
                }
            }
            else
                throw new System.Exception("Declaracao: \'=\' esperado!");
            return false;
        }

        //Método que verifica as Leituras
        public bool instrucao_leitura(Instrucao_leitura inst_leitura) {
            Instrucao_leitura inst_lei = new Instrucao_leitura();
            if (compare("LEIA"))
            {

                index++;
                Identificador ident = new Identificador();
                if (identificador(ref ident))
                {

                    inst_lei.ident = ident;
                    if (compare(";"))
                    {

                        index++;
                        inst_leitura = inst_lei;
                        return true;

                    }else throw new System.Exception("Declaracao: \';\' esperado!");

                }else throw new System.Exception("Declaracao: \';\' esperado!"); 
            }
            return false;
        }

        //Método que verifica as escritas
        public bool instrucao_escrita(Instrucao_escrita inst_escrita)
        {
            Instrucao_escrita_a inst_esc_a = new Instrucao_escrita_a();
            Instrucao_escrita_b inst_esc_b = new Instrucao_escrita_b();
            if (compare("IMPRIMA"))
            {


                index++;
                if(tokens[index].nome.ToString() == "Texto")
                    {

                    inst_esc_a.text = tokens[index].valor.ToString();
                    index++;
                    if (compare(";"))
                    {
                        index++;
                        inst_escrita = inst_esc_a;
                        return true;
                    }
                     
                else throw new System.Exception("Declaracao: \';\' esperado!"); 
            }

                Expressao expr = new Expressao();
                if (expressao(expr))
                {

                    inst_esc_b.expr = expr;
                    if (compare(";"))
                    {

                        index++;
                        inst_escrita = inst_esc_b;
                        return true;

                    }

                    else throw new System.Exception("Declaracao: \';\' esperado!");
                }
                else throw new System.Exception("Declaracao: \';\' esperado!");

            }
            return false;
        }

        public bool verificaVariavel(int guarda)
        {

            if (IsNumeric(tokens[guarda].valor.ToString()))
            {
                return true;
            }

            for (int i = 0; i != listaVariavel.Count; i++)
            {
                if (tokens[guarda].valor.ToString() == listaVariavel[i].nome.ToString())
                {
                    return true;
                }         
            }
            return false; 
        }

        //Método que verifica as instruções
        public bool instrucoes(Instrucoes insts)
        {

            int guarda = index;
            Instrucao_r inst_r = new Instrucao_r();

            if (instrucao(inst_r.inst1))
            {
                if (instrucao_r(inst_r.inst2))
                {
                    insts = inst_r;
                    return true;
                }
            }

             index = guarda;
            Instrucao inst = new Instrucao() ;
            if (instrucao(inst))
            {
                insts = inst;
                return true;
            }
            return false;
        }

        //Método que verifica as instruções recursivamente
        public bool instrucao_r(Instrucoes insts)
        {
            int guarda = index;
            Instrucao_r inst_r = new Instrucao_r();

            if (instrucao(inst_r.inst1))
            {
                if (instrucao_r(inst_r.inst2))
                {
                    insts = inst_r;
                    return true;
                }
            }

            index = guarda;
            Instrucao inst = new Instrucao();

            if (instrucao(inst))
            {
                insts = inst;
                return true;
            }
            return false;
        }

        public void AtrReduce(int guarda)
        {
            if (verificaVariavel(guarda))
            {
                string s;
                string aux = null;

                Node n = new Node();
                s = "=, " + tokens[guarda].valor.ToString() + ", " + last();

                t.conta = 0;
                n.valor = s;
                aux = last();

                if (last() == null)
                {
                    flagvariavel = 0;
                    int i = codigo.labels;    
                }

                else if (flagvariavel != 0)
                {
                    flagvariavel = 0;
                    int i = codigo.labels;              
                }
                else
                {
                    flagvariavel = 0;
                    int i = codigo.labels;
                    n.labels = labels++;
                    listacodigo.Add(n);
                }

            }else
                throw new System.Exception("A Variável não foi declarada!");
        }

        public void AtrRead(int guarda)
        {
            if (verificaVariavel(guarda + 1))
            {
                string s;
                Node n = new Node();
                s = "LEIA,  " + tokens[guarda + 1].valor.ToString();
                t.conta = 0;
                n.valor = s;
                int i = codigo.labels;
                n.labels = labels++;
                listacodigo.Add(n);
            }
            else throw new System.Exception("A Variável não foi declarada!");
        }

        public void AtrPrint(int guarda)
        {
            if (verificaVariavel(guarda + 1))
            {
                string s;
                Node n = new Node();
                s = "IMPRIMA,  " + tokens[guarda + 1 ].valor.ToString() ;
                t.conta = 0;
                n.valor = s;
                int i = codigo.labels;
                n.labels = labels++;
                listacodigo.Add(n);
            }
            else throw new System.Exception("A Variável não foi declarada!");
        }

        public bool instrucao(Instrucao inst)
        {
             guarda = index;
            Atribuicao atr = new Atribuicao();
            if (atribuicao(atr))
            {

                inst = atr;
                if (notPrinted(guarda))
                {
                    AtrReduce(guarda);
                }
                return true;
            }
            index = guarda;
            Instrucao_leitura inst_leitura = new Instrucao_leitura();
            if (instrucao_leitura(inst_leitura))
            {
                if (notPrinted(guarda))
                {
                    AtrRead(guarda);
                }
                inst = inst_leitura;
                return true;
            }
            index = guarda;
            Instrucao_escrita inst_escrita = new Instrucao_escrita();
            if (instrucao_escrita(inst_escrita))
            {

                if (notPrinted(guarda))
                {
                    AtrPrint(guarda);
                }
                inst = inst_escrita;
                return true;
            }
            return false;
        }

        public bool expressoes(Expressoes expr)
        {

            int guarda = index;
            Expressao_r expr_r = new Expressao_r();

            if (expressao(expr_r.expr1))
            {
                if (operador(expr_r.opr))
                    if (expressao_r(expr_r.expr2))
                    {
                        expr = expr_r;
                        if (notPrinted(guarda))
                        {

                            ExprReduce(guarda);
                        }
                        return true;
                    }
            }

            index = guarda;
            Expressao_ident expr_ident = new Expressao_ident();
            expr_ident.ident = new Identificador();
            if (identificador(ref expr_ident.ident))
            {
                expr = expr_ident;

                if (notPrinted(guarda))
                {

                    ExprReduce(guarda);
                }
                return true;
            }

            index = guarda;
            Expressao_int expr_int = new Expressao_int();

            if (tokens[index].nome.ToString() == "Inteiro")
            {

                expr_int.inteiro = tokens[index].valor.ToString();
                expr = expr_int;
                if (notPrinted(guarda))
                {

                    ExprReduce(guarda);
                }
                index++;
                return true;
                    
            }

            index = guarda;
            Expressao_real expr_real = new Expressao_real();

            if (tokens[index].nome.ToString() == "Real")
            {

                expr_real.real = tokens[index].valor.ToString();
                expr = expr_real;
                if (notPrinted(guarda))
                {

                    ExprReduce(guarda);
                }
                index++;
                return true;
            }
            return false;
        }

        public bool expressao_r(Expressoes expr)
        {
            int guarda = index;
            Expressao_r exper_r = new Expressao_r();
            exper_r.expr1 = new Expressao();
            exper_r.opr = new Operador();
            exper_r.expr2 = new Expressao();

            if (expressao(exper_r.expr1))
            {
                if (operador(exper_r.opr))
                {
                    if (expressao_r(exper_r.expr2))
                        {
                        expr = exper_r;
                        if (notPrinted(guarda))
                        {

                            ExprReduce(guarda);
                        }
                        return true;
                    }
                }

            }

            index = guarda;
            Expressao_ident expr_ident = new Expressao_ident();
            expr_ident.ident = new Identificador();

            if (identificador(ref expr_ident.ident))
            {
                expr = expr_ident;
                return true;
            }

            index = guarda;
            Expressao_int expr_int = new Expressao_int();

            if (tokens[index].nome.ToString() == "Inteiro")
            {
                expr_int.inteiro = tokens[index].valor.ToString();
                expr = expr_int;
                index++;
                return true;
            }

            index = guarda;
            Expressao_real expr_real = new Expressao_real();

            if (tokens[index].nome.ToString() == "Real")
            {
                expr_real.real = tokens[index].valor.ToString();
                expr = expr_real;
                index++;
                return true;
            }

            return false;
        }

        public bool operador(Operador oper)
        {
            int guarda = index;
            Operador_aritmetico oper_ari = new Operador_aritmetico();
            if (operador_aritmetico(oper_ari))
            {
                oper = oper_ari;
                return true;

            }
            return false;
        }

        public bool operador_aritmetico(Operador_aritmetico oper_ari)
        {

            if (tokens[index].nome.ToString() == "Aritmetico"){

                oper_ari.oper_ari = tokens[index].valor.ToString();
                index++;
                return true;
            }
            return false;
        }

        public void ExprReduce(int guarda)
        {

            if (tokens[guarda + 1].nome.ToString() == "Aritmetico")
            {

                int i = codigo.newLabel();
                string s = "";
                if (verificaVariavel(guarda))
                {
                    if (t.conta == 0)
                    {
                        Node n = new Node();
                        s = tokens[guarda + 1].valor.ToString() + ", " + tokens[guarda].valor.ToString() + ", " + tokens[guarda + 2].valor.ToString() + ", " + toString();

                        n.labels = labels++;
                        n.valor = s;
                        listacodigo.Add(n);
                    }
                    else
                    {
                        Node n = new Node();

                        s = tokens[guarda + 1].valor.ToString() + ", " + tokens[guarda].valor.ToString() + ", " + toString();
                        n.labels = labels++;
                        n.valor = s;
                        
                        listacodigo.Add(n);

                    }
                }else
                    throw new System.Exception("Variavel não declarada !");

            }
            else if (tokens[guarda + 1].nome.ToString() ==  "Delimitador")
            {
                if(verificaVariavel( guarda )){
                    Node n = new Node();
                    int i = codigo.newLabel();
                    string s = "";
                    n.labels = labels++;
                    s = "=, " + tokens[guarda - 2].valor.ToString() + ", " + tokens[guarda].valor.ToString();
                    flagvariavel = 1;

                    n.valor = s;

                    listacodigo.Add(n);
                }
                else
                    throw new System.Exception("Variavel não declarada !");
            }
        }

        private bool expressao(Expressao expr)
        {
            int guarda = index;
            Expressao_ident expr_ident = new Expressao_ident();
            expr_ident.ident = new Identificador();

            if (identificador(ref expr_ident.ident))
            {

                expr =  expr_ident;
                return true;
            }

            index = guarda;
            Expressao_int expr_int = new Expressao_int();
            if (tokens[index].nome.ToString() == "Inteiro")
            {
                expr_int.inteiro = tokens[index].valor.ToString();
                expr = expr_int;
                index++;
                return true;
            }

            index = guarda;
            Expressao_real expr_real = new Expressao_real();
            if (tokens[index].nome.ToString() == "Real")
            {
                expr_real.real = tokens[index].valor.ToString();
                expr = expr_int;
                index++;
                return true;
            }
            return false;
        }

        public string toString()
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

        public string last()
        {
            return cont;
        }

        private bool compare(String palavrachave) {
            if (tokens[index].valor.ToString().Equals(palavrachave))
            {
                return true;
            }
            else
                return false;
        }

        private bool declaracoes(Declaracoes decls){

            // tenta executar => <declaracoes> := <declaracoes> <declaracao>
            int guarda = index; // armazena o index
            Declaracao_r decls_r = new Declaracao_r();
            decls_r.d1 = new Declaracao();

            if (declaracao(ref decls_r.d1))
                if (declaracao_r(ref decls_r.d2))
                {
                    decls = decls_r;
                    return true;
                }

            //executa <declaracoes> := <declaracao>
            index = guarda;
            Declaracao decl = new Declaracao();
            if (declaracao(ref decl))
            {
                decls = decl;
                return true;
            }

            return false;
        }

        private bool declaracao_r(ref Declaracoes declr_r)
        {
            int guarda = index;
            Declaracao_r decls = new Declaracao_r();
            decls.d1 = new Declaracao();

            if (declaracao(ref decls.d1))
                if (declaracao_r(ref decls.d2))
                {
                    declr_r = decls;
                    return true;
                }

            index = guarda;
            Declaracao decl = new Declaracao();

            if (declaracao(ref decl))
            {
                declr_r = decl;
                return true;
            }
            return false;
        }

        // metodo para reconhecer o nao terminal declaracao
        private bool declaracao(ref Declaracao decl)
        {
            if (tipo())
            {
                // reconhece um tipo
                decl.tipo = tokens[index].valor.ToString();
                index++; // passa para o próximo token

                // reconhece :
                if (tokens[index].valor.Equals(':'))
                {
                    index++; // passa para o próximo token

                    if (identificadores(ref decl.idents))
                        if (tokens[index].valor.Equals(';'))
                        {
                            index++;
                            return true;
                        }
                        else throw new System.Exception("Declaracao: \';\' esperado!");
                }
                else throw new System.Exception("Declaracao: \':\' esperado!");
            }

            return false;
        }

        // metodo que reconhece o nao terminal identificadores
        private bool identificadores(ref Identificadores idents)
        {
            int guarda = index;
            Identificador_r ident_r = new Identificador_r();
            ident_r.ident1 = new Identificador();
            
            if (identificador(ref ident_r.ident1))
                if (identificador_r(ref ident_r.ident2))
                {
                    idents = ident_r;
                    return true;
                }

            index = guarda;

            Identificador ident = new Identificador();
            if (identificador(ref ident))
            {
                idents = ident;
                return true;
            }

            return false;
        }

        // metodo auxiliar para reconhecer a recursividade dos identificadores
        private bool identificador_r(ref Identificadores idents)
        {
            int guarda = index;            
            if (tokens[index].valor.Equals(','))
            {
                index++;
                
                Identificador_r ident_r = new Identificador_r();
                ident_r.ident1 = new Identificador();
                
                if (identificador(ref ident_r.ident1))
                    if (identificador_r(ref ident_r.ident2))
                    {
                        idents = ident_r;
                        return true;
                    }
            }            

            index = guarda;
            
            if (tokens[index].valor.Equals(','))
            {
                index++;
                
                Identificador ident = new Identificador();

                if (identificador(ref ident))
                {
                    idents = ident;
                    return true;
                }
            }            

            return false;
        }

        // metodo para reconhecer o terminal identificador
        private bool identificador(ref Identificador ident)
        {   

            if (tokens[index].nome == ClasseToken.Identificador &&
                !PalavrasReservadas.Contains(tokens[index].valor as string))
            {                
                ident.ident = tokens[index].valor.ToString();
                index++;                
                return true;
            }

            return false;
        }

        // metodo para reconhecer o terminal tipo
        private bool tipo()
        {
            
            if (tokens[index].valor.ToString() == "INTEIRO")
                return true;

            if (tokens[index].valor.ToString() == "REAL")
                return true;

            return false;
        } 
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalisadorSintatico
{
    enum ClasseToken
    {
        Identificador,
        Inteiro,
        DoisPontos,
        Virgula,
        PontoEVirgula,
        FimDeArquivo,
        Delimitador,
        Texto,
        keyword,
        Real,
        Aritmetico,
        Atribuicao,
        Coment,
        Parentese
    };

    class Token
    {
        public ClasseToken nome;
        public object valor;

        public Token(ClasseToken nome, object valor)
        {
            this.nome = nome;
            this.valor = valor;
           
        }
    }


    class AnalisadorLexico
    {
        public List<Token> tokens;
 
        public AnalisadorLexico(TextReader input)
        {
            this.tokens = new List<Token>();
            this.Scan(input);
        }

        // metodo para realizar a análise léxica
        private void Scan(TextReader input)
        {
            // enquanto o próximo caracter for diferente de fim de arquivo
            while (input.Peek() != -1)
            {
                // recupera o próximo caracter
                char ch = (char)input.Peek();
                char aux = (char)(input.Peek()+1);

                // inicializa a maquina de estados
                int estado = 0;

                if (char.IsWhiteSpace(ch))
                    input.Read();
                else if (char.IsLetter(ch))
                    estado = 1;
                else if (char.IsDigit(ch) | (ch == '-' && char.IsDigit(aux)))
                    estado = 2;
                else if (ch == '"')
                    estado = 4;
                else if (ch == '/' )
                    estado = 5;
                else
                    estado = 3;

                // maquina de estados
                switch (estado)
                {
                    //ESTADO 1 - RECONHECE UM IDENTIFICADOR
                    case 1:
                        {
                            StringBuilder accum = new StringBuilder();
                            while (char.IsLetter(ch) | char.IsDigit(ch))
                            {
                                accum.Append(ch);
                                input.Read();

                                if (input.Peek() == -1)
                                    break;
                                else
                                    ch = (char)input.Peek();
                            }
                            this.tokens.Add(new Token(ClasseToken.Identificador, accum));
                        }
                        break;

                    //ESTADO 2 - RECONHECE UM INTEIRO
                    case 2:
                        {
                            StringBuilder accum = new StringBuilder();

                            int flag = 0;

                            while (char.IsDigit(ch) | ch == '-'| ch == '.' )
                            {
                                accum.Append(ch);
                                input.Read();

                                if (input.Peek() == -1)
                                    break;
                                else
                                    ch = (char)input.Peek();

                                if (ch == '.') {

                                    flag = 1;
                                }
                            }
                            if (flag == 1)
                            {

                                this.tokens.Add(new Token(ClasseToken.Real, accum));

                            }
                            else
                                this.tokens.Add(new Token(ClasseToken.Inteiro, accum));
                        }
                        break;
                    //ESTADO 3 - RECONHECE UM CARACTER ESPECIAL
                    case 3:
                        {
                            switch (ch)
                            {
                                case ':':
                                    input.Read();
                                    this.tokens.Add(new Token(ClasseToken.Delimitador, ':'));
                                    break;

                                case ',':
                                    input.Read();
                                    this.tokens.Add(new Token(ClasseToken.Delimitador, ','));
                                    break;

                                case ';':
                                    input.Read();
                                    this.tokens.Add(new Token(ClasseToken.Delimitador, ';'));
                                    break;

                                case '=':
                                    input.Read();
                                    this.tokens.Add(new Token(ClasseToken.Atribuicao, '='));
                                    break;

                                case '+':
                                    input.Read();
                                    this.tokens.Add(new Token(ClasseToken.Aritmetico, '+'));
                                    break;

                                case '-':
                                    input.Read();
                                    this.tokens.Add(new Token(ClasseToken.Aritmetico, '-'));
                                    break;
                                case '/':
                                    input.Read();
                                    this.tokens.Add(new Token(ClasseToken.Aritmetico, '/'));
                                    break;
                                case '*':
                                    input.Read();
                                    this.tokens.Add(new Token(ClasseToken.Aritmetico, '*'));
                                    break;
                                case '(':
                                    input.Read();
                                    this.tokens.Add(new Token(ClasseToken.Parentese, '('));

                                    break;
                                case ')':
                                    input.Read();
                                    this.tokens.Add(new Token(ClasseToken.Parentese, ')'));
                                    break;
                                default:
                                    throw new System.Exception("Caracter não reconhecido: '" + ch + "'");
                            }
                        }
                        break;
                    case 4:
                        {

                            StringBuilder accum = new StringBuilder();
                          
                            do
                            {
                                accum.Append(ch);
                                input.Read();

                                if (input.Peek() == -1)
                                    break;
                                else
                                    ch = (char)input.Peek();
                               

                            } while (ch != '"');
                            accum.Append(ch);
                            input.Read();

                            this.tokens.Add(new Token(ClasseToken.Texto, accum));

                        }
                        break;
                    case 5:
                        {
                            StringBuilder accum = new StringBuilder();
                            int flag = 0;
                            while (ch != '\n')
                            {
                                if (flag == 2 || ch != '/')
                                {
                                    input.Read();
                                    flag = 0;
                                    this.tokens.Add(new Token(ClasseToken.Aritmetico, '/'));
                                    break;
                                    
                                }

                                accum.Append(ch);
                                input.Read();
                                if (input.Peek() == -1)
                                    break;
                                else
                                    ch = (char)input.Peek();

                                flag++;
                            }
                            if (flag != 0)
                            {
                                this.tokens.Add(new Token(ClasseToken.Coment, accum));
                            }
                        }
                        break;
                }
            }
        }
    }
}

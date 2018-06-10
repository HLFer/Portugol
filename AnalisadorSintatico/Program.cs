using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalisadorSintatico
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length != 1)
            {
                Console.WriteLine("Usage: AnalisadorLexico.exe programa.por");
                Console.ReadKey();
                return;
            }

            try
            {
                AnalisadorLexico lexico = null;
                using (TextReader input = File.OpenText(args[0]))
                {
                    lexico = new AnalisadorLexico(input);
                }

                List<Token> teste = lexico.tokens;
                AnalisadorSintatico parser = new AnalisadorSintatico(lexico.tokens);
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.ReadKey();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalisadorSintatico
{
    class Imprimir
    {
        public void imprimir(List<Node> lista)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("codigoIntermediario.txt");
            for(int i = 0; i != lista.Count; i++)
            {
                string s;
                int label;
                s = lista[i].valor;
                label = lista[i].labels;
                file.WriteLine("Label "+ label + ": " + s );
            }
            file.Close();
        }
    }
}

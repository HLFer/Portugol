using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalisadorSintatico
{
   public class Node
    {
        public int labels = 0;
        public String valor;
        public List<Node> printed;
        public List<int> feito = new List<int>();

        public bool notPrinted(int guarda)
        {
            for (int it = 0; it != feito.Count; it++)

            {
                if (it == guarda)

                { return false; }
            }
            feito.Add(guarda);
            return true;
        }

        public int newLabel() {
            return labels++;
        }
    }
}

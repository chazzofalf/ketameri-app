// See https://aka.ms/new-console-template for more information
namespace SymbolTest
{
    public class Program
    {
        private TextWriter? textout;
        public void Run()
        {
            Console.WriteLine("Hello, World!");
            using (textout = new StreamWriter(File.OpenWrite("Symboltest.txt")))
            {
                foreach (var symb in Symbol.Symbol.All)
                {
                    MultiPrintLine($"Symbol Number: {symb.Number}");
                    MultiPrintLine();
                    MultiPrintLine($"Symbol Map:");
                    MultiPrintLine(string.Join("\n",Enumerable.Range(0,5)
                    .Select(r => Enumerable.Range(0,5)
                    .Select(c => symb.ReadBitMap(r,c) ? '#' : '.'))
                    .Select(r => string.Join("",r))));
                    MultiPrintLine();
                    MultiPrintLine($"Siblings:");
                    MultiPrintLine();
                    foreach (var sib in symb.AllOrientations)
                    {
                        MultiPrintLine($"    Symbol Number: {sib.Number}");
                        MultiPrintLine();
                        MultiPrintLine($"    Symbol Map:");
                        MultiPrintLine("    " + string.Join("\n    ",Enumerable.Range(0,5)
                    .Select(r => Enumerable.Range(0,5)
                    .Select(c => sib.ReadBitMap(r,c) ? '#' : '.'))
                    .Select(r => string.Join("",r))));
                    }
                }
                
            }
        }
        public static void Main()
        {
            var prgm = new Program();
            prgm.Run();
                       
        }
        public void MultiPrintLine(string line="")
        {
            Console.WriteLine(line);
            textout?.WriteLine(line);
        }
    }
}


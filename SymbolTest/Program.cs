// See https://aka.ms/new-console-template for more information
using System.Text;

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
                TestSymbols();
                TestPhonetics();
            }
        }

        private void TestPhonetics()
        {
            var foxtrot = GetFoxText();
            var translator = new Phonetics.Phonetics();
            MultiPrintLine($"Original Text: {foxtrot}");
            MultiPrintLine();
            var translatedfoxtrot = translator.TranslateString(foxtrot);
            MultiPrintLine($"Translated Text: {translatedfoxtrot}");
            MultiPrintLine();
            var untranslatedfoxtrot = translator.ReverseTranslateString(translatedfoxtrot);
            MultiPrintLine($"Untranslated Text: {untranslatedfoxtrot}");
            MultiPrintLine();
            MultiPrintLine($"Original and Untranslated Matched?: {(foxtrot.Equals(untranslatedfoxtrot) ? "Yes" : "No" )}");
        }

        private void TestSymbols()
        {
            foreach (var symb in Symbol.Symbol.All)
            {
                MultiPrintLine($"Symbol Number: {symb.Number}");
                MultiPrintLine();
                MultiPrintLine($"Symbol Map:");
                MultiPrintLine(string.Join("\n", Enumerable.Range(0, 5)
                .Select(r => Enumerable.Range(0, 5)
                .Select(c => symb.ReadBitMap(r, c) ? '#' : '.'))
                .Select(r => string.Join("", r))));
                MultiPrintLine();
                MultiPrintLine($"Siblings:");
                MultiPrintLine();
                foreach (var sib in symb.AllOrientations)
                {
                    MultiPrintLine($"    Symbol Number: {sib.Number}");
                    MultiPrintLine();
                    MultiPrintLine($"    Symbol Map:");
                    MultiPrintLine("    " + string.Join("\n    ", Enumerable.Range(0, 5)
                .Select(r => Enumerable.Range(0, 5)
                .Select(c => sib.ReadBitMap(r, c) ? '#' : '.'))
                .Select(r => string.Join("", r))));
                }
            }
        }

        public static void Main()
        {
            var prgm = new Program();
            prgm.Run();
                       
        }
        public string GetFoxText()
        {
            var s = typeof(Program).Assembly.GetManifestResourceStream("SymbolTest.foxtext.txt");
            if (s != null)
            {
                var text = "";
                using (var sr = new StreamReader(s,Encoding.UTF8))
                {   
                    text = sr.ReadToEnd();
                }
                return text;
            }
            throw new Exception("Resource not found");
        }
        public void MultiPrintLine(string line="")
        {
            Console.WriteLine(line);
            textout?.WriteLine(line);
        }
    }
}


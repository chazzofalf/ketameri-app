// See https://aka.ms/new-console-template for more information
using System.Text;

namespace SymbolTest
{
    public class Program
    {
        private TextWriter? textout;
        private void Run()
        {
            Console.WriteLine("Hello, World!");
            if (Directory.Exists("Graphics"))
            {
                Directory.Delete("Graphics",true);
            }
            Directory.CreateDirectory("Graphics");
            if (File.Exists("Symboltest.txt"))
            {
                File.Delete("Symboltest.txt");
            }
            using (textout = new StreamWriter(File.OpenWrite("Symboltest.txt")))
            {
                TestSymbols();
                TestPhonetics();
                TextGlyphs();
                TextGraphics();
            }
        }

        private void TextGraphics()
        {
            foreach (var idx in Enumerable.Range(0,Glyph.Glyph.NumberOfGlyphs))
            {
                var symb = global:: Graphic.Graphic.GetGraphicAtIndex(idx);
                MultiPrintLine($"Graphic Number: {symb.Number}");
                MultiPrintLine($"Graphic Letter: {symb.Letter}");
                MultiPrintLine($"Is Number: {symb.IsNumber}");
                MultiPrintLine($"Is Special: {symb.IsSpecial}");
                MultiPrintLine($"Is Text: {symb.IsText}");
                MultiPrintLine();
                MultiPrintLine($"Graphic Map:");
                MultiPrintLine(string.Join("\n", Enumerable.Range(0, 5)
                .Select(r => Enumerable.Range(0, 5)
                .Select(c => symb.ReadBitMap(r, c) ? '#' : '.'))
                .Select(r => string.Join("", r))));
                MultiPrintLine();
                MultiPrintLine($"Siblings:");
                MultiPrintLine();
                foreach (var sib in Enumerable.Range(0,symb.NumberOfOrientations).Select(s => symb.OrientationAtIndex(s)))
                {
                    MultiPrintLine($"    Graphic Number: {sib.Number}");
                    MultiPrintLine();
                    MultiPrintLine($"    Graphic Map:");
                    MultiPrintLine("    " + string.Join("\n    ", Enumerable.Range(0, 5)
                .Select(r => Enumerable.Range(0, 5)
                .Select(c => sib.ReadBitMap(r, c) ? '#' : '.'))
                .Select(r => string.Join("", r))));
                MultiPrintLine();

                }
                var idex = $"{idx}";
                while (idex.Length < 3)
                {
                    idex = $"0{idex}";
                }
                var name_base = "";
                if (IsValidFilename(symb.Letter))
                {
                    name_base = $"Graphics/CHR_{idx}_{symb.Letter}";
                }
                else
                {
                    name_base = $"Graphics/CHR_{idx}";
                }
                using (var fio = File.OpenWrite($"{name_base}_small.png"))
                {
                    symb.Small.Encode(SkiaSharp.SKEncodedImageFormat.Png,100).AsStream().CopyTo(fio);
                }
                MultiPrintLine($"Wrote Small Graphic To: {name_base}_small.png");
                using (var fio = File.OpenWrite($"{name_base}_large.png"))
                {
                    symb.Large.Encode(SkiaSharp.SKEncodedImageFormat.Png,100).AsStream().CopyTo(fio);
                }
                MultiPrintLine($"Wrote Small Graphic To: {name_base}_large.png");
                using (var fio = File.OpenWrite($"{name_base}_turquoise_small.png"))
                {
                    symb.ColorizeBitmap(useSmall:true).Encode(SkiaSharp.SKEncodedImageFormat.Png,100).AsStream().CopyTo(fio);
                }
                using (var fio = File.OpenWrite($"{name_base}_turquoise_large.png"))
                {
                    symb.ColorizeBitmap(useSmall:false).Encode(SkiaSharp.SKEncodedImageFormat.Png,100).AsStream().CopyTo(fio);
                                        
                }
                //#8A0707
                using (var fio = File.OpenWrite($"{name_base}_turquoise_small.png"))
                {
                    symb.ColorizeBitmap(useSmall:true).Encode(SkiaSharp.SKEncodedImageFormat.Png,100).AsStream().CopyTo(fio);
                }

                using (var fio = File.OpenWrite($"{name_base}_blood_large.png"))
                {
                    symb.ColorizeBitmap(useSmall:false,hex:"#8A0707").Encode(SkiaSharp.SKEncodedImageFormat.Png,100).AsStream().CopyTo(fio);
                                        
                }
                using (var fio = File.OpenWrite($"{name_base}_blood_small.png"))
                {
                    symb.ColorizeBitmap(useSmall:true,hex:"#8A0707").Encode(SkiaSharp.SKEncodedImageFormat.Png,100).AsStream().CopyTo(fio);
                }
                using (var fio = File.OpenWrite($"{name_base}_goldenrod_large.png"))
                {
                    symb.ColorizeBitmap(useSmall:false,hex:"#daa520").Encode(SkiaSharp.SKEncodedImageFormat.Png,100).AsStream().CopyTo(fio);
                                        
                }
                using (var fio = File.OpenWrite($"{name_base}_goldenrod_small.png"))
                {
                    symb.ColorizeBitmap(useSmall:true,hex:"#daa520").Encode(SkiaSharp.SKEncodedImageFormat.Png,100).AsStream().CopyTo(fio);
                }
            }
        }
        private static bool IsValidFilename(string name)
        {
            
            return !name
            .Select(s => !IsValidFileNameCharacter(s))
            .Where(s => s)
            .Any();
        }
        private static bool IsValidFileNameCharacter(char character)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return Array.IndexOf(invalidChars, character) == -1;
        } 
        private void TextGlyphs()
        {
            foreach (var idx in Enumerable.Range(0,Glyph.Glyph.NumberOfGlyphs))
            {
                var symb = Glyph.Glyph.GetGlyphAtIndex(idx);
                MultiPrintLine($"Glyph Number: {symb.Number}");
                MultiPrintLine($"Glyph Letter: {symb.Letter}");
                MultiPrintLine($"Is Number: {symb.IsNumber}");
                MultiPrintLine($"Is Special: {symb.IsSpecial}");
                MultiPrintLine($"Is Text: {symb.IsText}");
                MultiPrintLine();
                MultiPrintLine($"Glyph Map:");
                MultiPrintLine(string.Join("\n", Enumerable.Range(0, 5)
                .Select(r => Enumerable.Range(0, 5)
                .Select(c => symb.ReadBitMap(r, c) ? '#' : '.'))
                .Select(r => string.Join("", r))));
                MultiPrintLine();
                MultiPrintLine($"Siblings:");
                MultiPrintLine();
                foreach (var sib in Enumerable.Range(0,symb.NumberOfOrientations).Select(s => symb.OrientationAtIndex(s)))
                {
                    MultiPrintLine($"    Glyph Number: {sib.Number}");
                    MultiPrintLine();
                    MultiPrintLine($"    Glyph Map:");
                    MultiPrintLine("    " + string.Join("\n    ", Enumerable.Range(0, 5)
                .Select(r => Enumerable.Range(0, 5)
                .Select(c => sib.ReadBitMap(r, c) ? '#' : '.'))
                .Select(r => string.Join("", r))));
                MultiPrintLine();
                }
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
                foreach (var sib in Enumerable.Range(0,symb.NumberOfOrientations).Select(s => symb.OrientationAtIndex(s)))
                {
                    MultiPrintLine($"    Symbol Number: {sib.Number}");
                    MultiPrintLine();
                    MultiPrintLine($"    Symbol Map:");
                    MultiPrintLine("    " + string.Join("\n    ", Enumerable.Range(0, 5)
                .Select(r => Enumerable.Range(0, 5)
                .Select(c => sib.ReadBitMap(r, c) ? '#' : '.'))
                .Select(r => string.Join("", r))));
                MultiPrintLine();
                }
            }
        }

        public static void Main()
        {
            var prgm = new Program();
            prgm.Run();
                       
        }
        private string GetFoxText()
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
        private void MultiPrintLine(string line="")
        {
            Console.WriteLine(line);
            textout?.WriteLine(line);
        }
    }
}


// See https://aka.ms/new-console-template for more information
using System.Data.Common;
using System.Text;
using SkiaSharp;


namespace SymbolTest
{
    public class Program
    {
        public string TestHandle => "@liv5be4wise"; //Nobody in the multiverse should have this handle, hopefully... Change it immediately upon finding out otherwise.
        public string AtlasText => $"🦋🦋🦋It's the End of the World as We Know It (And I Feel Fine) - R.E.M ©️ 1987 [54 666 911 2105: My bad numbers that I associate with trouble. Really this was me seeing if I can break this with a sequence of numbers. That's all. Oh and handle to see if numbers in a word can break this. (DISCLAIMER: Before deciding that you must commit untoward action against me on this, remember that I Pulled this one totally out of my ass. I swear to The Three As One, The Favored One, and On My Mother's Grave that Any existence of any persons or entities holding this handle in any reality within the multiverse is coincidental and any implied references to such persons or entities are purely unintentional. Yes. Yes. I know. I had to borrow from the MPAA's statement about their movies, but I felt, for my personal safety, that I should be explicit about the fact, that it applies here and I am just messing around. Again. I swear to God I know of nobody with this handle, if I did, I wouldn't have used it here. If you just so happen to have this handle, than I am sorry it was an accident, perhaps you could drop me a email at chazzofalf@gmail.com with the subject \"Hello, I am {TestHandle}\" and drop me a line. And I promise that I will change the example. I don't want any trouble. And I've had more than my fair share.) Now that we gotten that out of the way... (Geez) Example: Hey, {TestHandle} wanna grab a sweet frappuccino?]🦋🦋🦋";
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
                TestGlyphs();
                TestGraphics();
                TestTokenizer();
                TestRecognizer();
                TestLinerJoinerSplitter();
            }
        }

        private void TestGraphics()
        {
            foreach (var idx in Enumerable.Range(0,Glyph.Glyph.NumberOfGlyphs))
            {
                var symb = global:: Graphic.CachedGraphic.GetGraphicAtIndex(idx);
                MultiPrintLine($"Graphic Number: {symb.Number}");
                MultiPrintLine($"Graphic Letter: {symb.Letter}");
                MultiPrintLine($"Is Number: {symb.IsNumber}");
                MultiPrintLine($"Is Special: {symb.IsSpecial}");
                MultiPrintLine($"Is Text: {symb.IsText}");
                MultiPrintLine();
                MultiPrintLine($"Graphic Map:");
                MultiPrintLine(string.Join("\n", Enumerable.Range(0, 9)
                .Select(r => Enumerable.Range(0, 9)
                .Select(c => symb.ReadBitMap(r, c) ? '#' : '.'))
                .Select(r => string.Join("", r))));
                MultiPrintLine();
                MultiPrintLine($"Siblings:");
                MultiPrintLine();
                
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
        private void TestRecognizer()
        {
            if (Directory.Exists("RecognizerTest"))
            {
                Directory.Delete("RecognizerTest",true);
            }
            var padnum = (int v) => {
            var s = $"{v}";
            while (s.Length < 4)
            {
                s = $"0{s}";
            }
            if (s != null)
            {
                return s;
            }
            throw new NullReferenceException();
        };
        var tokenizer = new Tokenizer.Tokenizer();
            var reverse_cnt_func = (Graphic.CachedGraphic[] g) =>
            g.Select(gs => {tokenizer.Put(gs); return 1;})
            .Sum();

            Directory.CreateDirectory("RecognizerTest");
            var tfosFavoriteRemSongTitle = AtlasText;
            var testItems = new [] {tfosFavoriteRemSongTitle};
            var file_original_write_op_count = testItems
            .Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (Index:b,Item:a))
            .Select(itm => (IndexName:string.Join("",$"{itm.Index}".Reverse().Concat("00").Reverse().Take(3)),Item:itm.Item))
            .Select(itm => (Name:$"RecognizerTest/Original_Source_{itm.IndexName}.txt",Item:itm.Item))
            .Select(itm => {
                File.WriteAllText(itm.Name,itm.Item);
                return 1;
            })
            .Sum();
            
            var cnt_func = (string s) => 
            s.Select(ch => {tokenizer.Put(ch); return 1;})
            .Sum();
                 
            var tokenized_graphic_sets = testItems
            .Select(s => { var cnt1 = cnt_func(s);
            return tokenizer.Finish<Graphic.CachedGraphic[]>();

            
        })
        .Select(r => r.Select(c => {
            var large = c.ColorizeBitmap();
            var img = new SkiaSharp.SKBitmap(large.Height,large.Width,SkiaSharp.SKColorType.Rgba8888,SkiaSharp.SKAlphaType.Premul);
            var can = new SKCanvas(img);
            var paint = new SKPaint();
            paint.Color = SKColors.Black.WithRed(1);
            can.DrawRect(new SKRect(0,0,img.Width,img.Height),paint);
            can.DrawBitmap(large,new SKPoint(0,0));
            return img;
        })).Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (RowIndex:b,Row:a
        .Zip(Enumerable.Range(0,int.MaxValue),(c,d) => (ColumnIndex:d,Column:c))))
        .SelectMany(r => r.Row.Select(c => (RowIndex:r.RowIndex,ColumnIndex:c.ColumnIndex,Item:c.Column)))
        .Select(itm => (RowName:padnum(itm.RowIndex),ColumnName:padnum(itm.ColumnIndex),itm.Item))
        .Select(itm => (Name:$"RecognizerTest/FilledTokens_Row_{itm.RowName}_Column_{itm.ColumnName}.png",Item:((Func<byte[]>)(() => { 
                var ms = new MemoryStream();
                itm.Item.Encode(SkiaSharp.SKEncodedImageFormat.Png,100).AsStream().CopyTo(ms);
                return ms.ToArray();
            }))()))
            .Select(itm => {
                File.WriteAllBytes(itm.Name,itm.Item);
                return 1;
            })
            .Sum();
            MultiPrintLine($"TestRecognizer: Wrote {file_original_write_op_count} Items.");
            var rec = new Recognizer.Recognizer();
        var decode_count = Enumerable.Range(0,10000)
        .Select(r => padnum(r))
        .Where(r => Directory.GetFiles("RecognizerTest")
        .Where(f => f.Contains($"RecognizerTest/FilledTokens_Row_{r}_")).Any())
        .Select(r => Enumerable.Range(0,10000).Select(c => (RowName:r,ColumnName:padnum(c)))
        .Where(c => Directory.GetFiles("RecognizerTest")
        .Where(f => f.Contains($"RecognizerTest/FilledTokens_Row_{c.RowName}_Column_{c.ColumnName}.")).Any()))
        .Select(r => r.Select(c=> $"RecognizerTest/FilledTokens_Row_{c.RowName}_Column_{c.ColumnName}.png"))
        .Select(r => r.Select(c => File.ReadAllBytes(c)))
        .Select(r => r.Select(c => SKBitmap.Decode(c)))
        .Select(r => r.ToArray())
        .Select(r => r.Select(c => rec.Recognize(c)).ToArray())
        .Select(s => {reverse_cnt_func(s);
            return tokenizer.Finish<string>();

        })        
        .Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (Index:b,Item:a))
        .Select(s => (IndexName:padnum(s.Index),s.Item))
        .Select(s => (Name:$"RecognizerTest/Decoded_{s.IndexName}.txt",s.Item))
        .Select(s => {
            File.WriteAllText(s.Name,s.Item);
            return 1;
        })
        .Sum();
        MultiPrintLine($"TestRecognizer: Decoded {decode_count} items.");
        }
        private void TestTokenizer()
        {
            if (Directory.Exists("TokenizerTest"))
            {
                Directory.Delete("TokenizerTest",true);
            }
            
            Directory.CreateDirectory("TokenizerTest");
            
            var bOs = "Zethana";
            var bOL = "Declán";
            var warning = "Watch out! Those two are going to quite literally rock your (the) entire world! Don't say I didn't warn you.";
            var bOsDOB = "Zethana - 2/23/9852 BCE - 7/3/2032 CE";
            var bOLDOB = "Declán - 6/6/2006 - 7/3/2032";
            var testItems = new [] {bOs,bOL,bOsDOB,bOLDOB,warning};
            var file_original_write_op_count = testItems
            .Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (Index:b,Item:a))
            .Select(itm => (IndexName:string.Join("",$"{itm.Index}".Reverse().Concat("00").Reverse().Take(3)),Item:itm.Item))
            .Select(itm => (Name:$"TokenizerTest/Original_Source_{itm.IndexName}.txt",Item:itm.Item))
            .Select(itm => {
                File.WriteAllText(itm.Name,itm.Item);
                return 1;
            })
            .Sum();
            MultiPrintLine($"TestTokenizer: Wrote {file_original_write_op_count} Items");
            var tokenizer = new Tokenizer.Tokenizer();
            var cnt_func = (string s) => 
            s.Select(ch => {tokenizer.Put(ch); return 1;})
            .Sum();
            var reverse_cnt_func = (Graphic.CachedGraphic[] g) =>
            g.Select(gs => {tokenizer.Put(gs); return 1;})
            .Sum();        
            var tokenized_graphic_sets = testItems
            .Select(s => { var cnt1 = cnt_func(s);
            return tokenizer.Finish<Graphic.CachedGraphic[]>();
            
        })
        .ToArray();
        var padnum = (int v) => {
            var s = $"{v}";
            while (s.Length < 3)
            {
                s = $"0{s}";
            }
            if (s != null)
            {
                return s;
            }
            throw new NullReferenceException();
        };
            var file_convert_op_counts = tokenized_graphic_sets
            .Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (RowIndex:b,Row:a))
            .Select(r => (RowIndex:r.RowIndex,Row:r.Row.Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (ColumnIndex:b,Column:a))))
            .SelectMany(r => r.Row.Select(c => (RowIndex:r.RowIndex,ColumnIndex:c.ColumnIndex,Item:c.Column)))
            .Select(itm => (RowName:padnum(itm.RowIndex),ColumnName:padnum(itm.ColumnIndex),Item:itm.Item))
            .Select(itm => (Name:$"TokenizerTest/TokenizedGraphic_Row_{itm.RowName}_Column_{itm.ColumnName}.png",Item:itm.Item))
            .Select(itm => (Name:itm.Name,Item: ((Func<byte[]>)(() => { 
                var ms = new MemoryStream();
                itm.Item.ColorizeBitmap().Encode(SkiaSharp.SKEncodedImageFormat.Png,100).AsStream().CopyTo(ms);
                return ms.ToArray();
            }))()))
            .Select(itm => {File.WriteAllBytes(itm.Name,itm.Item); return 1;})
            .Sum();
            MultiPrintLine($"TestTokenizer: Converted {file_convert_op_counts} Items");
            var detokenized_string_sets = tokenized_graphic_sets
            .Select(s => {reverse_cnt_func(s);
            return tokenizer.Finish<string>();
        })
        .ToArray();      
            var file_detokenized_write_op_count = testItems
            .Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (Index:b,Item:a))
            .Select(itm => (IndexName:string.Join("",$"{itm.Index}".Reverse().Concat("00").Reverse().Take(3)),Item:itm.Item))
            .Select(itm => (Name:$"TokenizerTest/Detokenized_Text_{itm.IndexName}.txt",Item:itm.Item))
            .Select(itm => {
                File.WriteAllText(itm.Name,itm.Item);
                return 1;
            })
            .Sum();       
            MultiPrintLine($"TestTokenizer: Detokenized {file_detokenized_write_op_count} Items");

            
            

            

        }
        private void TestGlyphs()
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
                MultiPrintLine(  string.Join("\n",Enumerable.Range(0,9)
                .Select(r => Enumerable.Range(0,9)
                .Select(c => symb.ReadBitMap(r,c) ))));
                MultiPrintLine();
                
                
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
            foreach (var symb in Enumerable.Range(0,Symbol.Symbol.NumberOfSymbols).Select(s => Symbol.Symbol.GetSymbolAt(s)))
            {
                //var symb = Glyph.Glyph.GetGlyphAtIndex(idx);
                MultiPrintLine($"Glyph Number: {symb.Number}");
                
                MultiPrintLine();
                MultiPrintLine($"Glyph Map:");
                MultiPrintLine(  string.Join("\n",Enumerable.Range(0,9)
                .Select(r => Enumerable.Range(0,9)
                .Select(c => symb.ReadBitMap(r,c) ? '#' : '.' ))
                .Select(r => string.Join("",r))));
                MultiPrintLine();
                
                
            }
        }

        private void TestLinerJoinerSplitter()
        {
            if (Directory.Exists("LinerJoinerTest"))
            {
                Directory.Delete("LinerJoinerTest",true);
            }
            var padnum = (int v) => {
            var s = $"{v}";
            while (s.Length < 3)
            {
                s = $"0{s}";
            }
            if (s != null)
            {
                return s;
            }
            throw new NullReferenceException();
        };
        var linerJoiner = new LineJoinerSplitter.LineJoinerSplitter();
        var tokenizer = new Tokenizer.Tokenizer();
            var reverse_cnt_func = (Graphic.CachedGraphic[] g) =>
            g.Select(gs => {tokenizer.Put(gs); return 1;})
            .Sum();

            Directory.CreateDirectory("LinerJoinerTest");
            var tfosFavoriteRemSongTitle = AtlasText;
            var testItems = new [] {tfosFavoriteRemSongTitle};
            var file_original_write_op_count = testItems
            .Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (Index:b,Item:a))
            .Select(itm => (IndexName:string.Join("",$"{itm.Index}".Reverse().Concat("00").Reverse().Take(3)),Item:itm.Item))
            .Select(itm => (Name:$"LinerJoinerTest/Original_Source_{itm.IndexName}.txt",Item:itm.Item))
            .Select(itm => {
                File.WriteAllText(itm.Name,itm.Item);
                return 1;
            })
            .Sum();
            
            var cnt_func = (string s) => 
            s.Select(ch => {tokenizer.Put(ch); return 1;})
            .Sum();
                 
            var tokenized_graphic_sets = testItems
            .Select(s => { var cnt1 = cnt_func(s);
            return tokenizer.Finish<Graphic.CachedGraphic[]>();

            
        })
        .Select(r => {
            return linerJoiner.Join(r);
        })
        
        .Zip(Enumerable.Range(0,int.MaxValue),(a,b) => (Index:b,Item:a))
        .Select(s => (IndexName:padnum(s.Index),s.Item))
        .Select(s => (Name:$"LinerJoinerTest/FilledTokens_Row_{s.IndexName}.png",s.Item))
        .Select(s => (s.Name,Item:((Func<byte[]>)(() => {
            var ms = new MemoryStream();
            s.Item.Encode(SKEncodedImageFormat.Png,100).AsStream().CopyTo(ms);
            return ms.ToArray();
        }))()))
        .Select(s => {
            File.WriteAllBytes(s.Name,s.Item);
            return 1;
        })
        .Sum();
        var opso =Enumerable.Range(0,1000)
        .Select(s => {
            
            var name = $"LinerJoinerTest/FilledTokens_Row_{padnum(s)}.png";
            if (File.Exists(name))
            {
                var image = (SKBitmap?)null;
                using (var fio = File.OpenRead(name))
                {
                    if (fio != null)
                    {
                        image = SKBitmap.Decode(fio);
                    }
                }
                var splitted =  image is SKBitmap bmp ? linerJoiner.Split(bmp) : null;
                var ops = splitted?
                .Select(s => {
                    tokenizer.Put(s);
                    return 1;
                }).Sum();
                var output = tokenizer.Finish<string>();
                var textname = $"LinerJoinerTest/FilledTokens_Decoded_Row_{padnum(s)}.txt";
                File.WriteAllText(textname,output);
                return 1;
            }
            return 0;
            
            
        }).Sum();
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


// See https://aka.ms/new-console-template for more information
using System.Data.Common;
using System.Text;
using SkiaSharp;


namespace SymbolTest
{
    public class Program
    {        
        public string AtlasText => Resources.Resources.Testing_LetThereBeLightNoLines;
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
                // TestSymbols();
                // TestPhonetics();
                // TestGlyphs();
                TestGraphics();
                TestTokenizer();
                TestRecognizer();
                TestLinerJoinerSplitter();
                TestPageJoinerSplitter();
            }
        }

        private void TestPageJoinerSplitter()
        {
            var text1 = Resources.Resources.Testing_LetThereBeLight;
            var text2 = Resources.Resources.Testing_PowerInFaith;
            var text1Lines = text1.Split("\n");
            var text2Lines = text2.Split("\n");
            var background = GetBackground();
            var pageJoiner = new PageJoinerSpliter.PageJoinerSpliter();
            var lineJoinerSplitter = new LineJoinerSplitter.LineJoinerSplitter();
            var tokenizer = new Tokenizer.Tokenizer();
            var text1Tokens = text1Lines.Select(line => line
            .Aggregate((object?)null,(ign,ch) => {
                tokenizer.Put(ch);
                return ign;
            },(ign) => {
                return tokenizer.Finish<Graphic.CachedGraphic[]>();
            }).ToArray()).ToArray();
            var text2Tokens = text2Lines.Select(line => line
            .Aggregate((object?)null,(ign,ch) => {
                tokenizer.Put(ch);
                return ign;
            },(ign) => {
                return tokenizer.Finish<Graphic.CachedGraphic[]>();
            }).ToArray()).ToArray();
            var text1GraphicalLines = text1Tokens.Select(line => lineJoinerSplitter.Join(line)).ToArray();
            var text2GraphicalLines = text2Tokens.Select(line => lineJoinerSplitter.Join(line)).ToArray();
            var textInvGraphicalLines = text2Tokens.Select(line => lineJoinerSplitter.Join(line,color:SKColors.Black)).ToArray();
            var text1Page = pageJoiner.Join(text1GraphicalLines,false,SKColors.Black,null);
            var text2Page = pageJoiner.Join(text2GraphicalLines,true,backgroundimg:background);
            var textInvPage = pageJoiner.Join(textInvGraphicalLines,true,background:SKColors.Black);
            if (Directory.Exists(""))
            {
                Directory.Delete("PageJoinerSplitterTest",true);
            }            
            Directory.CreateDirectory("PageJoinerSplitterTest");
            File.WriteAllText("PageJoinerSplitterTest/Page1.txt",text1);
            File.WriteAllText("PageJoinerSplitterTest/Page2.txt",text2);
            File.WriteAllBytes("PageJoinerSplitterTest/Page1.png",text1Page.Encode(SKEncodedImageFormat.Png,100).AsSpan().ToArray());
            File.WriteAllBytes("PageJoinerSplitterTest/Page2.png",text2Page.Encode(SKEncodedImageFormat.Png,100).AsSpan().ToArray());
            File.WriteAllBytes("PageJoinerSplitterTest/PageInv.png",textInvPage.Encode(SKEncodedImageFormat.Png,100).AsSpan().ToArray());
            TestPageJoinerSplitterDecode();
        }

        private void TestPageJoinerSplitterDecode()
        {
            var pageJoiner = new PageJoinerSpliter.PageJoinerSpliter();
            var lineJoinerSplitter = new LineJoinerSplitter.LineJoinerSplitter();
            var tokenizer = new Tokenizer.Tokenizer();
            var x = new [] {
                "PageJoinerSplitterTest/Page1.png",
                "PageJoinerSplitterTest/Page2.png",
                "PageJoinerSplitterTest/PageInv.png"
            }.Select(s => (Graphic:SKBitmap.Decode(s),Filename:s))
            .Select(s => (SplitGraphic:pageJoiner.Split(s.Graphic),s.Filename))
            .Select(s => (Tokens:s.SplitGraphic.Select(ss => lineJoinerSplitter.Split(ss)),s.Filename))
            .Select(s => (Lines:s.Tokens.Select(ss => {
                var ops = ss.Select(ch => {
                    tokenizer.Put(ch);
                    return 1;
                }).Sum();
                return tokenizer.Finish<string>();
            }),s.Filename))
            .Select(s => (Text:string.Join("\n",s.Lines),s.Filename))
            .Select(s => (s.Text,OutputName:$"{s.Filename}.out.txt"))
            .Select(s => {
                File.WriteAllText(s.OutputName,s.Text);
                return 1;
            }).Sum();
        }

        private SKBitmap GetBackground()
        {
            return SKBitmap.Decode(Resources.Resources.Testing_ScoutTestResource);
        }

        private void TestGraphics()
        {
            foreach (var idx in Enumerable.Range(0,Graphic.CachedGraphic.NumberOfGlyphs))
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
                        
            var testItems = new [] {Resources.Resources.Testing_LetThereBeLightNoLines,Resources.Resources.Testing_PowerInFaithNoLines};
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
            return linerJoiner.Join(r,backgroundColor:SKColors.Black);
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





using System.Reflection.Metadata;

namespace Phonetics;

public class Phonetics
{
    private Dictionary<string,string>? _PartialMappingDictionary;
    private Dictionary<string,string> PartialMappingDictionary => _PartialMappingDictionary = _PartialMappingDictionary ?? GeneratePartialMappingDictionary();

    private Dictionary<string,string>? _PartialReversedMappingDictionary;

    private Dictionary<string,string> PartialReversedMappingDictionary => _PartialReversedMappingDictionary = _PartialReversedMappingDictionary ?? GeneratePartialReversedMappingDictionary();

    private Dictionary<string, string> GeneratePartialReversedMappingDictionary()
    {
        return  PartialMappingDictionary.Keys.Select(s => (K: PartialMappingDictionary[s], V: s)).ToDictionary(s => s.K, s => s.V);
    }
    private Dictionary<string,string>? _FullMappingDictionary;
    private Dictionary<string,string> FullMappingDictionary => _FullMappingDictionary = _FullMappingDictionary ?? GenerateFullMappingDictionary();

    private Dictionary<string,string>? _FullReversedMappingDictionary;
    private Dictionary<string,string> FullReversedMappingDictionary => _FullReversedMappingDictionary = _FullReversedMappingDictionary ?? GenerateFullReversedMappingDictionary();

    private Dictionary<string, string> GenerateFullReversedMappingDictionary()
    {
        return FullMappingDictionary.ToDictionary(s => s.Value, s => s.Key);
    }
    private char TranslateChar(char c)
    {
        if (FullMappingDictionary.ContainsKey($"{c}"))
        {
            return FullMappingDictionary[$"{c}"].First();
        }
        else
        {
            return c;
        }
    }
    private char ReverseTranslateChar(char c)
    {
        if (FullReversedMappingDictionary.ContainsKey($"{c}"))
        {
            return FullReversedMappingDictionary[$"{c}"].First();
        }
        else
        {
            return c;
        }
    }
    public string TranslateString(string str)
    {
        return string.Join("",str
        .Select(ch => TranslateChar(ch)));
    }
    public string ReverseTranslateString(string str)
    {
        return string.Join("",str
        .Select(ch => ReverseTranslateChar(ch)));
    }

    private Dictionary<string, string> GenerateFullMappingDictionary()
    {
        var vowels = new string[] {"a","e","i","o","u","y"};
        var consonants = Enumerable.Range(0,char.MaxValue)
        .Select(ord => (char)ord)        
        .Where(ch => Char.IsAscii(ch))
        .Where(ch => Char.IsLetter(ch))
        .Where(ch => Char.IsLower(ch))
        .Where(ch => !vowels.Contains($"{ch}"))
        .Select(s => $"{s}")
        .ToArray();
        
        var vowel_mappings = vowels.Where(s => PartialMappingDictionary.ContainsKey(s))
        .Select(s => (English:s,Protodimenian:PartialMappingDictionary[s]));
        var not_mapped_vowels = vowels.Where(s => !PartialMappingDictionary.ContainsKey(s));        
        var not_mapped_value_vowels = vowels.Where(s => !PartialMappingDictionary.ContainsValue(s));
        vowel_mappings = vowel_mappings.Concat(not_mapped_vowels.Zip(not_mapped_value_vowels.Reverse(),(a,b) => (English:a,Protodimenian:b)));

        var consonants_mappings = consonants        
        .Where(s => PartialMappingDictionary.ContainsKey(s))
        .Select(s => (English:s,Protodimenian:PartialMappingDictionary[s]));
        var not_mapped_consonants = consonants.Where(s => !PartialMappingDictionary.ContainsKey(s));
        var not_mapped_value_consonants = consonants.Where(s => !PartialMappingDictionary.ContainsValue(s));
        consonants_mappings = consonants_mappings.Concat(not_mapped_consonants.Zip(not_mapped_value_consonants.Reverse(),(a,b) => (English:a,Protodimenian:b)));
        var all_mappings = vowel_mappings.Concat(consonants_mappings)
        .OrderBy(m => m.English)
        .ToDictionary(m => m.English,m => m.Protodimenian);
        return all_mappings;

    }

    private Dictionary<string, string> GeneratePartialMappingDictionary()
    {
        return new Dictionary<string, string> {
            {"h","b"},
            {"c","g"},
            {"d","d"},
            {"j","h"},
            {"w","z"},
            {"a","y"},
            {"p","k"},
            {"g","l"},
            {"f","n"},
            {"s","s"},
            {"e","a"},
            {"m","p"} //Thanks to the prolithic father of the one who likes to grabs heels and picks fights with the divine. Loosely based on a table on their language. 
            
        };
    }
}

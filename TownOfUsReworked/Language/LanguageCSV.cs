using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace TownOfUsReworked.Languages
{
    public static class LanguageCSV
    {
        public static Dictionary<string, Dictionary<int, string>> tr;
        public static void LoadCSV()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("TownOfUsReworked.Resources.string.csv");
            var sr = new StreamReader(stream);
            tr = new Dictionary<string, Dictionary<int, string>>();
            string[] header = sr.ReadLine().Split(',');

            int currentLine = 1;

            while (!sr.EndOfStream)
            {
                currentLine++;
                string line = sr.ReadLine();
                if (line == "" || line[0] == '#') continue;
                string[] values = line.Split(',');
                List<string> fields = new(values);
                Dictionary<int, string> tmp = new();
                try
                {
                    for (var i = 1; i < fields.Count; ++i)
                    {
                        if (fields[i] != string.Empty && fields[i].TrimStart()[0] == '"')
                        {
                            while (fields[i].TrimEnd()[^1] != '"')
                            {
                                fields[i] = fields[i] + "," + fields[i + 1];
                                fields.RemoveAt(i + 1);
                            }
                        }
                    }
                    for (var i = 1; i < fields.Count; i++)
                    {
                        var tmp_str = fields[i].Replace("\\n", "\n").Trim('"');
                        tmp.Add(Int32.Parse(header[i]), tmp_str);
                    }
                    if (tr.ContainsKey(fields[0]))
                    {

                        Utils.LogSomething($"LoadCSV:Translation repetition in{currentLine}Line");
                        continue;
                    }
                    tr.Add(fields[0], tmp);
                }
                catch (Exception ex)
                {
                    Exception(ex);
                }
            }
            /*
           “我是笨蛋，没看Csv库的支持版本” -- Huier

            var options = new CsvOptions()
            {
                HeaderMode = HeaderMode.HeaderPresent,
                AllowNewLineInEnclosedFieldValues = false,
            };
             foreach (var line in )
            {
             if (line.Values[0][0] == '#') continue;

            try
            {
                Dictionary<int,string> dic = new();
                for (int i = 1; i < line.ColumnCount; i++)
                {
                    int id = int.Parse(line.Headers[i]);
                    dic[id] = line.Values[i].Replace("\\n","\n").Replace("\\r","\r");
                }
                if (!translateMaps.TryAdd(line.Values[0],dic))
                {
                Helpers.CWrite($"LoadCSV:翻译重复在{line.Index}行: \"{line.Values[0]}\"");
                }
                }
                catch (Exception ex)
                {
                Helpers.CWrite(ex.ToString());
                }
            }
            */
        }

        private static void Exception(Exception ex)
        {
            throw new NotImplementedException();
        }

        // 获取CSV文本
        public static string GetCString(string str, SupportedLangs langId)
        {
            var res = $"{str}";
            if (tr.TryGetValue(str, out var dic) && (!dic.TryGetValue((int)langId, out res) || res == "")) //strに該当する&無効なlangIdかresが空
            {
                res = $"{dic[0]}";
            }
            return res;
        }
    }
}
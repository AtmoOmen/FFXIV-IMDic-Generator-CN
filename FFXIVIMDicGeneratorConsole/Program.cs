using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TinyPinyin;

class Program
{
    static void Main()
    {
        Console.WriteLine("请输入文件夹路径:");
        string folderPath = Console.ReadLine();

        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine("文件夹不存在。");
            return;
        }

        List<string> allData = new List<string>();

        string[] csvFiles = Directory.GetFiles(folderPath, "*.csv");
        foreach (string csvFile in csvFiles)
        {
            ProcessCsvFile(csvFile, allData);
        }

        string outputFilePath = Path.Combine(folderPath, "output.txt");
        File.WriteAllLines(outputFilePath, allData, Encoding.UTF8);

        Console.WriteLine($"处理完成，输出到文件: {outputFilePath}");
    }

    static void ProcessCsvFile(string filePath, List<string> allData)
    {
        Console.WriteLine($"处理文件: {filePath}");

        List<string[]> rows = new List<string[]>();
        using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] columns = line.Split(',');
                rows.Add(columns);
            }
        }

        if (rows.Count < 2)
        {
            Console.WriteLine("文件内容不符合要求，跳过处理。");
            return;
        }

        string[] secondRow = rows[1];

        int nameColumnIndex = -1;
        for (int i = 0; i < secondRow.Length; i++)
        {
            if (secondRow[i].Trim() == "Name")
            {
                nameColumnIndex = i;
                break;
            }
        }

        if (nameColumnIndex == -1)
        {
            Console.WriteLine("不含专有名词，跳过处理。");
            return;
        }

        List<string> names = new List<string>();
        foreach (string[] row in rows)
        {
            if (row.Length > nameColumnIndex)
            {
                string name = row[nameColumnIndex].Trim();
                // 删除非中文部分、空格和空白行
                name = Regex.Replace(name, @"[^\u4e00-\u9fa5]", "");
                if (!string.IsNullOrWhiteSpace(name))
                {
                    names.Add(name);
                }
            }
        }

        Dictionary<string, string> pinyinDictionary = new Dictionary<string, string>();

        foreach (string name in names)
        {
            string pinyin = PinyinHelper.GetPinyin(name, "'");
            pinyin = "'" + pinyin.ToLower();
            pinyinDictionary[name] = pinyin;
        }

        foreach (var kvp in pinyinDictionary)
        {
            allData.Add($"{kvp.Value} {kvp.Key}");
        }

        Console.WriteLine($"文件处理完成: {filePath}");
    }
}

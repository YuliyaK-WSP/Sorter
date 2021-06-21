using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorter
{
    class Program
    {
        static void Main(string[] args)
        {
            String comand = "";
            string arr = @" 1 - Сгенерировать файл
                    2 - Отсортировать файл
                    0 - Закрыть программу";
            while (!comand.Equals("0"))
            {
                Console.WriteLine($"Выберите действие: {arr}");

                comand = Console.ReadLine();

                if (comand.Equals("1"))
                {
                    // Генерируем файл
                    Console.WriteLine("Введите имя файла (включая путь)");
                    string path = Console.ReadLine();
                    Console.WriteLine("Укажите кол-во строк");
                    int count_str = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Укажите длину строки");
                    int leng_str = Convert.ToInt32(Console.ReadLine());
                    Generation(path,count_str,leng_str);
                    Console.WriteLine("Файл сгенерирован ");
                }
                else if (comand.Equals("2"))
                {
                    Console.WriteLine("Введите имя файла(включая путь)");
                    string path = Console.ReadLine();
                    Console.WriteLine("Введите объем выделяемой памяти");
                    double ram = Convert.ToDouble(Console.ReadLine());
                    string spl = Split(path, ram);
                    if (!spl.Equals("err"))
                    {
                        foreach (string tempFile in Directory.GetFiles(spl, "*.txt", SearchOption.AllDirectories))
                            Sorting(tempFile);
                        MergeFiles(spl);
                        Console.WriteLine($"Файл отсортирован {spl}");
                    }

                    // Сортируем файл

                }
                else if (!comand.Equals("0"))
                {
                    Console.WriteLine("Не известная команда");
                    Console.WriteLine();
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="ram"> ОБъем памяти</param>
        /// <returns></returns>
        public static string Split(string path, double ram)
        {
            try
            {
                var strs = File.ReadLines(path);
                //Директория для хранения временных файлов
                string dir = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                // Создание временного файла
                string tempFileName = dir + "/" + Guid.NewGuid().ToString() + Path.GetExtension(path);
                StreamWriter tempFile = new StreamWriter(File.OpenWrite(tempFileName));
                try
                {
                    foreach (string str in strs)
                    {
                        tempFile.WriteLine(str);
                        // Првоерка на размер содержимого в файле,
                        //если содержимое достигает укаанного лимита, 
                        //то текущий файл закрывается и создается новый.
                        if (tempFile.BaseStream.Position >= ram)
                        {
                            tempFile.Close();

                            tempFileName = dir + "/" + Guid.NewGuid().ToString() + Path.GetExtension(path);
                            tempFile = new StreamWriter(File.OpenWrite(tempFileName));

                        }
                    }
                }
                finally
                {
                    tempFile.Close();
                }
                return dir;
            }
            catch
            {
                Console.WriteLine("Указанный файл не найден");
                Console.WriteLine();
                return "err";
            }
            
        }
        public static void MergeFiles(string dir)
        {
            List<StreamReader> readers = new List<StreamReader>();
            List<string> ply = new List<string>(readers.Count());
            foreach (string file in Directory.GetFiles(dir, "*.txt", SearchOption.AllDirectories))
            {
               
                if(!file.Equals(dir + "/result.txt")) 
                {
                    var reader = new StreamReader(File.OpenRead(file));
                    readers.Add(reader);
                    ply.Add(reader.ReadLine());
                }
                
            }
            var writter = new StreamWriter(File.OpenWrite(dir + "/result.txt"));
            try
            {
                int Id = 0;
                while (ply.FirstOrDefault(x => x != null) != null)
                {
                    string min = ply.Min();
                    Id = ply.IndexOf(min);
                    ply[Id] = readers[Id].ReadLine();
                    writter.WriteLine(min);
                }
            }
            finally 
            { 
                writter.Close(); 
            }
             
            foreach (var reader in readers)
                reader.Close();

            foreach (string file in Directory.GetFiles(dir, "*.txt", SearchOption.AllDirectories))
            {
                if (Path.GetFileNameWithoutExtension(file) != "result")
                    File.Delete(file);
            }
        }
        /// <summary>
        /// Считывание и сортировка каждого файла
        /// </summary>
        /// <param name="tempfile"></param>
        public static void Sorting(string tempfile)
        {
            List<string> strs = File.ReadAllLines(tempfile).ToList();
            strs.Sort();
            File.WriteAllLines(tempfile, strs);

        }

        public static void Generation(string path,int count_str,int leng_str)
        {
            if (!Directory.Exists(path))
            {
                File.WriteAllText(path, string.Empty);
            }
            var writter = new StreamWriter(File.OpenWrite(path));
            try
            {
                
                Random rmd = new Random();
                for (int i = 0; i < count_str; i++)
                {
                    char[]str = new char[leng_str];
                    
                    for (int j = 0; j < leng_str; j++)
                    {
                        str[j] = Convert.ToChar(rmd.Next('a', 'z'+ 1));
                        writter.Write("{0}", str[j]);
                    }
                    writter.WriteLine();
                    
                }

            }
            finally
            {
                writter.Close();
            }
        }

    }

}

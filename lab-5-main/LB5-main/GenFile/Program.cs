using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFile
{
    /// <summary>
    /// Генерируем файлы для приложения
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            int nLine;
            for (int i = 1; i <= 5; i++)
            {
                using (StreamWriter sw = new StreamWriter("..\\..\\..\\Pract5\\bin\\Debug\\" + i + ".txt"))
                {
                    // Максимальный размер задачи 65535 байта, размер int 4 байта,
                    // значит 65535/4 = 16383,75, округленное до 16383 — максимальное количество чисел в файле.
                    // Метод random.Next, не генерирует число в диапазоне
                    // от минимального значения включительно до максимального не включительно,
                    // т.е. [0, 16384) и метод ниже вернет число от 0 до 16383.
                    nLine = random.Next(0, 16384);
                    sw.WriteLine(nLine);
                    for (int j = 0; j < nLine; j++)
                        sw.WriteLine(random.Next(int.MinValue, int.MaxValue));
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pract5
{
    class Program
    {
      
        /// <summary>

        /// </summary>
        static readonly int nMemory = 16000;
        /// <summary>
   
        /// </summary>
        static readonly int sizeInt = sizeof(int);
        /// <summary>
   
        /// </summary>
        static int[] memory;
        /// <summary>
      
        /// </summary>
        static SortedList<int, int> freeMemory;
        /// <summary>

        /// </summary>
        static SortedList<int, int> occupiedMemory;
        /// <summary>
    
        /// </summary>
        static Queue<TaskInfo> queueTasksToWait;
        /// <summary>
   
        /// </summary>
        static Queue<TaskInfo> queueTasksToFree;
        /// <summary>
  
        /// </summary>
        static List<Thread> listTaskDoing;
        /// <summary>
 
        /// </summary>
        static bool isExit;

        /// <summary>
      
        /// </summary>
        static void PrintMemory()
        {
            Console.WriteLine("———————————————————————————————");
            Console.WriteLine("Память перераспределена!");
            Console.WriteLine("Всего памяти " + memory.Length * sizeInt + " байт");
            if (freeMemory.Count > 0)
            {
                Console.WriteLine("Свободные области памяти:");
                for (int i = 0; i < freeMemory.Count; i++)
                {
                    Console.WriteLine("[" + freeMemory.Keys[i] * sizeInt + "; " + (freeMemory.Values[i] * sizeInt) + "]");
                    //Console.WriteLine("[" + freeMemory.Keys[i] + "; " + (freeMemory.Values[i]) + "]");
                }
            }
            else
                Console.WriteLine("Нет свободных областей памяти");

            if (occupiedMemory.Count > 0)
            {
                Console.WriteLine("Занятые области памяти:");
                for (int i = 0; i < occupiedMemory.Count; i++)
                {
                    Console.WriteLine("[" + occupiedMemory.Keys[i] * sizeInt + "; " + ((occupiedMemory.Values[i] + 1) * sizeInt) + "]");
                    //Console.WriteLine("[" + occupiedMemory.Keys[i] + "; " + (occupiedMemory.Values[i]) + "]");
                }
            }
            else
                Console.WriteLine("Нет занятых областей памяти");
            Console.WriteLine("———————————————————————————————");
        }

        /// <summary>

        /// </summary>
        /// <param name="file">название файла</param>
        /// <param name="n">объем необходимой памяти</param>
        /// <returns></returns>
        static TaskInfo LookingForFree(string file, int n)
        {
            if (freeMemory.Count > 0)
            {
                for (int i = 0; i < freeMemory.Count; i++)
                {
                    if (freeMemory.Values[i] - freeMemory.Keys[i] > n)
                    {
                        TaskInfo something = new TaskInfo(file, i, n);
                        SetMemory(something);
                        return something;
                    }
                }
            }
            return null;
        }

        /// <summary>

        /// </summary>
        /// <param name="pairs"></param>
        static void UniteMemory(SortedList<int, int> pairs)
        {
            int tempBegin = -1, tempEnd = -1;
            for (int i = 0; i < pairs.Count - 1; i++)
            {
                for (int j = i + 1; j < pairs.Count; j++)
                {
                    if (pairs.Values[i] == (pairs.Keys[j] - 1))
                    {
                        tempBegin = pairs.Keys[i];
                        tempEnd = pairs.Values[j];
                        pairs.RemoveAt(j);
                        pairs.RemoveAt(i);
                        pairs.Add(tempBegin, tempEnd);
                        if (i != 0)
                        {
                            i--;
                            j = i;
                        }
                        else
                        {
                            j--;
                        }
                    }
                }
            }
        }

        /// <summary>

        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        static TaskInfo SetMemory(TaskInfo info)
        {
            int n;
            using (StreamReader sr = new StreamReader(info.File))
            {
                n = int.Parse(sr.ReadLine());

                info.MemBegin = freeMemory.Keys[info.IndexFreeMemory];
                info.MemEnd = freeMemory.Keys[info.IndexFreeMemory] + n;

                occupiedMemory.Add(info.MemBegin, info.MemEnd - 1);

                int maxMemory = freeMemory.Values[info.IndexFreeMemory];
                freeMemory.RemoveAt(info.IndexFreeMemory);
                if (info.MemEnd < maxMemory)
                {
                    freeMemory.Add(info.MemEnd, maxMemory);
                }
                for (int i = info.MemBegin; i < info.MemEnd; i++)
                {
                    string line = sr.ReadLine();
                    memory[i] = int.Parse(line);
                }
            }
            UniteMemory(occupiedMemory);
            UniteMemory(freeMemory);
            PrintMemory();
            return info;
        }

        /// <summary>

        /// </summary>
        /// <param name="info"></param>
        static void FreeMemory(TaskInfo info)
        {
            occupiedMemory.Remove(info.MemBegin);
            freeMemory.Add(info.MemBegin, info.MemEnd - 1);
            UniteMemory(occupiedMemory);
            UniteMemory(freeMemory);
            PrintMemory();
            CheckQueue();
        }

        /// <summary>
   
        /// </summary>
        /// <param name="obj"></param>
        static void ThreadDoSomething(object obj)
        {
            TaskInfo info = (TaskInfo)obj;
            int sum = 0;
            for (int i = info.MemBegin; i < info.MemEnd; i++)
            {
                sum += memory[i];
            }
            //Console.WriteLine("Работа с файлом " + info.File + " окончена.");
            listTaskDoing.Remove(Thread.CurrentThread);
            queueTasksToFree.Enqueue(info);
        }

        /// <summary>
      
        /// </summary>
        static void ThreadFreeMemory()
        {
            while (!isExit)
            {
                if (queueTasksToFree.Count > 0)
                {
                    FreeMemory(queueTasksToFree.Dequeue());
                }
            }
        }

        /// <summary>
       
        /// </summary>
        static void PrintThread()
        {
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("Выполняются потоки в количестве " + listTaskDoing.Count);
            Console.WriteLine("В очереди находятся " + queueTasksToWait.Count);
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++");
        }

        /// <summary>
      
        /// </summary>
        /// <param name="file"></param>
        /// <param name="something"></param>
        static void StartTask(string file, TaskInfo something)
        {
            //Console.WriteLine("Начата работа с файлом " + file);
            Thread thread = new Thread(new ParameterizedThreadStart(ThreadDoSomething));
            listTaskDoing.Add(thread);
            thread.Start(something);
        }

        /// <summary>
    
        /// </summary>
        static void CheckQueue()
        {
            if (queueTasksToWait.Count != 0 && queueTasksToFree.Count == 0)
            {
                var task = queueTasksToWait.Peek();
                var res = LookingForFree(task.File, task.SizeOfTask);
                if (res != null && queueTasksToWait.Count != 0)
                {
                    StartTask(queueTasksToWait.Dequeue().File, res);
                }
            }
            //PrintThread();
        }

        /// <summary>

        /// </summary>
        /// <param name="file"></param>
        static void ReadFile(string file)
        {
            using (StreamReader sr = new StreamReader(file))
            {
                int n = int.Parse(sr.ReadLine());
                var res = LookingForFree(file, n);
                if (res != null)
                {
                    StartTask(file, res);
                }
                else
                {
                    //Console.WriteLine("В очередь добавлен файл " + file);
                    TaskInfo something = new TaskInfo(file, n);
                    queueTasksToWait.Enqueue(something);
                }
            }
        }

        static void PrintMenu()
        {
            Console.WriteLine("Введите номер пункта");
            Console.WriteLine("1. Вывести состояние памяти");
            Console.WriteLine("2. Проверить возможность выполнения задачи из очереди");
            Console.WriteLine("3. Добавить файл на выполнение");
            Console.WriteLine("4. Выйти из приложения");
        }

        static void Main(string[] args)
        {
            isExit = false;
            PrintMenu();

        
            memory = new int[nMemory];
        
            freeMemory = new SortedList<int, int>();
          
            occupiedMemory = new SortedList<int, int>();
            listTaskDoing = new List<Thread>();
            queueTasksToWait = new Queue<TaskInfo>();
            queueTasksToFree = new Queue<TaskInfo>();

            freeMemory.Add(0, nMemory);
            PrintMemory();

            Thread thread = new Thread(new ThreadStart(ThreadFreeMemory));
            thread.Start();

            for (int i = 1; i <= 5; i++)
                ReadFile(i + ".txt");

            int key;
            do
            {
                key = int.Parse(Console.ReadLine());
                switch (key)
                {
                    case 1:
                        {
                            PrintMemory();
                            break;
                        }
                    case 2:
                        {
                            CheckQueue();
                            PrintThread();
                            break;
                        }
                    case 3:
                        {
                            int keySubMenu;
                            do
                            {
                                Console.WriteLine("Введите номер файла на выполнение (число от 1 до 5):");
                                keySubMenu = int.Parse(Console.ReadLine());
                            } while (keySubMenu > 6 || keySubMenu < 1);
                            ReadFile(keySubMenu + ".txt");
                            break;
                        }
                }
                if (key != 4)
                    PrintMenu();
            } while (key != 4);
            isExit = true;
        }
    }
}

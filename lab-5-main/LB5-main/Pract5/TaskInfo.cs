using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pract5
{
    class TaskInfo
    {
        string file;
        int sizeOfTask;
        int iFreeMemory;
        int memBegin;
        int memEnd;

        public TaskInfo(string file, int sizeOfTask)
        {
            this.file = file;
            this.sizeOfTask = sizeOfTask;
        }

        public TaskInfo(string file, int iBegin, int sizeOfTask)
        {
            this.file = file;
            this.iFreeMemory = iBegin;
            this.sizeOfTask = sizeOfTask;
        }

        public string File { get => file; set => file = value; }
        public int IndexFreeMemory { get => iFreeMemory; set => iFreeMemory = value; }
        public int SizeOfTask { get => sizeOfTask; set => sizeOfTask = value; }
        public int MemBegin { get => memBegin; set => memBegin = value; }
        public int MemEnd { get => memEnd; set => memEnd = value; }
    }
}

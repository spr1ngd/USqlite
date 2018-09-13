
using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

namespace miniMVC
{
    /// <summary>
    /// 用于脚本文本的加载与导出
    /// 加载（将加载内容作为生成脚本的模板）
    /// 导出（导出到对应的文件夹）
    /// </summary>
    public sealed class FileIO : IFileIO
    {
        private Queue<FileIOTask> TaskQueue { get; set; }

        public void AddTask(FileIOTask ioTask)
        {
            if(null == TaskQueue)
                TaskQueue = new Queue<FileIOTask>();
            TaskQueue.Enqueue(ioTask);
            InvokeTask();
        }

        public void AddTasks(IList<FileIOTask> ioTasks)
        {
            if(null == ioTasks)
                return;
            foreach(FileIOTask ioTask in ioTasks)
                AddTask(ioTask);
        }

        /// <summary>
        /// 执行已经添加到队列的中的所有任务
        /// </summary>
        private void InvokeTask()
        {
            while(TaskQueue.Count > 0)
            {
                var task = TaskQueue.Dequeue();
                if(null == task)
                {
                    Debug.LogError("任务为null，请重新核对");
                    continue;
                }
                switch(task.ioType)
                {
                    case E_IOType.Read:
                        ReadTask(task);
                        break;
                    case E_IOType.Write:
                        WriteTask(task);
                        break;
                }
            }
        }

        private void ReadTask(FileIOTask readTask)
        {
            ReadFile(readTask.filePath,readTask.readCallback);
        }

        private void WriteTask(FileIOTask writeTask)
        {
            Write2File(writeTask.content,writeTask.filePath,writeTask.writeCallback);
        }

        private void ReadFile(string scriptFilePath,Action<string> readCallback)
        {
            try
            {
                using(TextReader reader = new StreamReader(File.OpenRead(scriptFilePath)))
                {
                    var content = reader.ReadToEnd();
                    readCallback(content);
                }
            }
            catch(IOException e)
            {
                Console.WriteLine("加载脚本模板异常 : " + e);
                throw;
            }
        }

        private void Write2File(string fileContent,string scriptFilePath,Action writeCallback)
        {
            try
            {
                using(TextWriter writer = new StreamWriter(scriptFilePath))
                {
                    writer.WriteLine(fileContent);
                    writeCallback();
                }
            }
            catch(IOException e)
            {
                Console.WriteLine("代码写出到脚本异常 ：" + e);
                throw;
            }
        }
    }
}
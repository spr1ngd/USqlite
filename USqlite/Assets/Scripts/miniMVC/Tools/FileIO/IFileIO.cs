
using System.Collections.Generic;

namespace miniMVC
{
    public interface IFileIO
    {
        /// <summary>
        /// 添加IO任务
        /// </summary>
        /// <param name="ioTask"></param>
        void AddTask( FileIOTask ioTask ); 

        /// <summary>
        /// 添加IO任务
        /// </summary>
        /// <param name="ioTasks"></param>
        void AddTasks( IList<FileIOTask> ioTasks );
    }
}
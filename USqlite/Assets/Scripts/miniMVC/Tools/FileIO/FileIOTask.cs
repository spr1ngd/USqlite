
using System;
using System.Collections.Generic;

namespace miniMVC
{
    public enum E_IOType
    {
        /// <summary>
        /// 加载脚本内容
        /// </summary>
        Read,
        /// <summary>
        /// 写出脚本内容
        /// </summary>
        Write
    }

    /// <summary>
    /// 脚本写入写出任务子项
    /// </summary>
    public class FileIOTask 
    {
        /// <summary>
        /// 任务类型
        /// </summary>
        public E_IOType ioType = E_IOType.Write;
        /// <summary>
        /// 任务路径
        /// </summary>
        public string filePath = null;
        /// <summary>
        /// 任务内容
        /// </summary>
        public string content = null;

        /// <summary>
        /// 写入任务回调
        /// </summary>
        public Action writeCallback = null;

        /// <summary>
        /// 读取任务回调
        /// </summary>
        public Action<string> readCallback = null;
    }
}
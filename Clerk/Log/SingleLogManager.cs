using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Clerk.Model;

namespace Clerk.Log
{
    class SingleLogManager: IDisposable
    {
        private int _serial;
        private Task fileTask;
        private FileInfo fileInfo;
        private FileStream stream;



        public SingleLogManager(FileInfo f)
        {
            _serial = 0;
            fileTask = Task.Factory.StartNew(() => "");
            fileInfo = f;
            if (f.Exists)
            {
                stream = f.Open(FileMode.Open, FileAccess.ReadWrite);
                stream.Seek(0, SeekOrigin.End);
            }
            else
            {
                stream = f.Create();
            }
        }

        public Task<int> WriteLog(string log)
        {
            lock (fileTask)
            {
                var id = _serial;
                var writeTask = fileTask.ContinueWith(p =>
                {
                    var bytes = new UTF8Encoding().GetBytes(log);
                    stream.Write(bytes);
                    _serial++;
                    return id;
                });
                fileTask = writeTask;
                return writeTask;
            }
        }

        public Task ReadLog(Func<Task<string>, Task> cc)
        {
             lock (fileTask)
             {
                 var readTask = fileTask.ContinueWith(p =>
                 {
                     stream.Flush();
                     stream.Seek(0, SeekOrigin.Begin);
                     var reader = new StreamReader(stream, Encoding.UTF8);
                     var ret = reader.ReadToEnd();
                     stream.Seek(0, SeekOrigin.End);
                     return ret;
                 });
                 var fullTask = readTask.ContinueWith(p => cc(p)).Unwrap();
                 fileTask = fullTask;
                 return fullTask;
             }           
        }

        public Task<T> ReadLog<T>(Func<Task<string>, Task<T>> cc)
        {
            lock (fileTask)
            {
                var readTask = fileTask.ContinueWith(p =>
                {
                    stream.Flush();
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    return reader.ReadToEnd();
                });
                var fullTask = readTask.ContinueWith(p => cc(p)).Unwrap();
                fileTask = fullTask;
                return fullTask;
            }
        }

        public Task<LogMessage> ReadLog()
        {
            lock (fileTask)
            {
                var id = _serial;
                var readTask = fileTask.ContinueWith(p =>
                {
                    stream.Flush();
                    var reader = new StreamReader(stream, Encoding.UTF8);
                    var msg = reader.ReadToEnd();
                    return new LogMessage
                    {
                        Id = id,
                        Msg = msg
                    };
                });
                fileTask = readTask;
                return readTask;
            }
        }

        public Task FinishTask()
        {
            lock (fileTask)
            {
                return fileTask;
            }
        }

        public void Dispose()
        {
            fileTask?.Wait();
            fileTask?.Dispose();
            stream?.Flush();
            stream?.Dispose();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncQueue
{
    class ComInfo
    {
        public int ComId { get; set; }

        public DateTime Date { get; set; }
    }
    class Program
    {
        static MyAsyncQueue<ComInfo> queue = new MyAsyncQueue<ComInfo>();
        static void Main(string[] args)
        {
            Console.WriteLine("开始======");
            queue.ProcessItemFunction += A;
            queue.ProcessException += C;

            ComInfo info = new ComInfo();

            for (int i = 1; i < 50; i++)
            {
                Task.Factory.StartNew((param) =>
                {
                    info = new ComInfo();
                    info.ComId = int.Parse(param.ToString());
                    info.Date = DateTime.Now.Date;
                    queue.Enqueue(info);
                }, i);
            }

            Console.WriteLine("结束======");

            Console.ReadKey();
        }

        static void A(ComInfo info)
        {
            Console.WriteLine(info.ComId + "====" + queue.Count);
        }

        static void C(object ex, EventArgs args)
        {
            Console.WriteLine("出错了");
        }
    }
}

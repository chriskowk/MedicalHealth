using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncQueue
{
    public class MyAsyncQueue<T>
    {
        //队列是否正在处理数据
        private int _isProcessing;
        //有线程正在处理数据
        private const int PROCESSING = 1;
        //没有线程处理数据
        private const int PENDING = 0;
        //队列是否可用 单线程下用while来判断，多线程下用if来判断，随后用while来循环队列的数量
        private volatile bool _enabled = true;
        // 消费者线程处理事件
        public event Action<T> ProcessItemFunction;
        //
        public event EventHandler<EventArgs> ProcessException;
        // 并发队列
        private readonly ConcurrentQueue<T> _queue;
        // 消费者的数量
        private int _internalTaskCount;

        // 存储消费者队列
        readonly List<Task> _tasks = new List<Task>();
        // 消费者线程
        private Task _currentTask;

        public MyAsyncQueue()
        {
            _internalTaskCount = 3;
            _queue = new ConcurrentQueue<T>();
            Start();
        }

        public int Count
        {
            get
            {
                return _queue.Count;
            }
        }
        // 开启监听线程
        private void Start()
        {
            Thread process_Thread = new Thread(PorcessItem);
            process_Thread.IsBackground = true;
            process_Thread.Start();
        }

        // 生产者生产
        public void Enqueue(T items)
        {
            if (items == null)
            {
                throw new ArgumentException("items");
            }

            _queue.Enqueue(items);
            DataAdded();
        }

        //数据添加完成后通知消费者线程处理
        private void DataAdded()
        {
            if (_enabled)
            {
                if (!IsProcessingItem())
                {
                    // 开启消费者消费队列
                    ProcessRangeItem();
                }
            }
        }

        //判断是否队列有线程正在处理 
        private bool IsProcessingItem()
        {
            return !(Interlocked.CompareExchange(ref _isProcessing, PROCESSING, PENDING) == 0);
        }

        private void ProcessRangeItem()
        {
            for (int i = 0; i < _internalTaskCount; i++)
            {
                _currentTask = Task.Factory.StartNew(() => ProcessItemLoop());
                _tasks.Add(_currentTask);
            }
        }

        // 消费者处理事件
        private void ProcessItemLoop()
        {
            Console.WriteLine("正在执行的Task的Id: {0}", Task.CurrentId);
            // 队列为空，并且队列不可用
            if (!_enabled && _queue.IsEmpty)
            {
                Interlocked.Exchange(ref _isProcessing, 0);
                return;
            }

            while (_enabled)
            {
                if (_queue.TryDequeue(out T publishFrame))
                {
                    try
                    {
                        // 消费者处理事件
                        ProcessItemFunction(publishFrame);
                    }
                    catch (Exception ex)
                    {
                        OnProcessException(ex);
                    }
                }
                else
                {
                    Console.WriteLine("线程Id{0}取队列失败，跳出循环", Task.CurrentId);
                    break;
                }
            }
        }

        /// <summary>
        ///定时处理线程调用函数  
        ///主要是监视入队的时候线程 没有来的及处理的情况
        /// </summary>
        private void PorcessItem(object state)
        {
            int sleepCount = 0;
            int sleepTime = 1000;
            while (_enabled)
            {
                //如果队列为空则根据循环的次数确定睡眠的时间
                if (_queue.IsEmpty)
                {
                    // Task消费者消费完了队列中的数据....注销掉消费者线程
                    if (_tasks.Count == _internalTaskCount)
                    {
                        Flush();
                    }
                    if (sleepCount == 0)
                    {
                        sleepTime = 1000;
                    }
                    else if (sleepCount <= 3)
                    {
                        sleepTime = 1000 * 3;
                    }
                    else
                    {
                        sleepTime = 1000 * 50;
                    }
                    sleepCount++;
                    Thread.Sleep(sleepTime);
                }
                else
                {
                    //判断是否队列有线程正在处理 
                    if (_enabled && Interlocked.CompareExchange(ref _isProcessing, PROCESSING, PENDING) == 0)
                    {
                        if (!_queue.IsEmpty)
                        {
                            _currentTask = Task.Factory.StartNew(ProcessItemLoop);
                            _tasks.Add(_currentTask);
                        }
                        else
                        {
                            //队列为空，已经取完了
                            Interlocked.Exchange(ref _isProcessing, 0);
                        }
                        sleepCount = 0;
                        sleepTime = 1000;
                    }
                }
            }
        }

        //更新并关闭消费者
        public void Flush()
        {
            Stop();
            foreach (var t in _tasks)
            {
                if (t != null)
                {
                    t.Wait();
                    Console.WriteLine("Task已经完成");
                }
            }

            // 消费者未消费完
            while (!_queue.IsEmpty)
            {
                try
                {
                    if (_queue.TryDequeue(out T publishFrame))
                    {
                        ProcessItemFunction(publishFrame);
                    }
                }
                catch (Exception ex)
                {
                    OnProcessException(ex);
                }
            }
            _currentTask = null;
            _tasks.Clear();
        }

        public void Stop()
        {
            this._enabled = false;
        }

        private void OnProcessException(System.Exception ex)
        {
            var tempException = ProcessException;
            Interlocked.CompareExchange(ref ProcessException, null, null);

            if (tempException != null)
            {
                ProcessException(ex, new EventArgs());
            }
        }
    }
}

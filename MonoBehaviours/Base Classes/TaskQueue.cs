using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Omnix.BaseClasses
{
    public class TaskQueue
    {
        private Queue<IRequest> requestQueue;
        private bool taskInProgress;

        public TaskQueue()
        {
            requestQueue = new Queue<IRequest>();
            taskInProgress = false;
        }

        public void BeginTask([NotNull] Action task)
        {
            if (taskInProgress)
            {
                Request request = new Request() { callback = task };
                requestQueue.Enqueue(request);
                return;
            }

            taskInProgress = true;
            task.Invoke();
        }

        public void BeginTask<T>([NotNull] Action<T> task, T para1)
        {
            if (taskInProgress)
            {
                Request<T> request = new Request<T>() { callback = task, value = para1 };
                requestQueue.Enqueue(request);
                return;
            }

            taskInProgress = true;
            task.Invoke(para1);
        }

        public void BeginTask<T1, T2>([NotNull] Action<T1, T2> task, T1 para1, T2 para2)
        {
            if (taskInProgress)
            {
                Request<T1, T2> request = new Request<T1, T2>() { callback = task, value1 = para1, value2 = para2 };
                requestQueue.Enqueue(request);
                return;
            }

            taskInProgress = true;
            task.Invoke(para1, para2);
        }

        public void BeginTask<T1, T2, T3>([NotNull] Action<T1, T2, T3> task, T1 para1, T2 para2, T3 para3)
        {
            if (taskInProgress)
            {
                Request<T1, T2, T3> request = new Request<T1, T2, T3>()
                    { callback = task, value1 = para1, value2 = para2, value3 = para3 };
                requestQueue.Enqueue(request);
                return;
            }

            taskInProgress = true;
            task.Invoke(para1, para2, para3);
        }

        public void BeginTask<T1, T2, T3, T4>([NotNull] Action<T1, T2, T3, T4> task, T1 para1, T2 para2, T3 para3, T4 para4)
        {
            if (taskInProgress)
            {
                Request<T1, T2, T3, T4> request = new Request<T1, T2, T3, T4>() { callback = task, value1 = para1, value2 = para2, value3 = para3, value4 = para4 };
                requestQueue.Enqueue(request);
                return;
            }

            taskInProgress = true;
            task.Invoke(para1, para2, para3, para4);
        }

        public void BeginTask<T1, T2, T3, T4, T5>([NotNull] Action<T1, T2, T3, T4, T5> task, T1 para1, T2 para2, T3 para3, T4 para4, T5 para5)
        {
            if (taskInProgress)
            {
                Request<T1, T2, T3, T4, T5> request = new Request<T1, T2, T3, T4, T5>() { callback = task, value1 = para1, value2 = para2, value3 = para3, value4 = para4, value5 = para5 };
                requestQueue.Enqueue(request);
                return;
            }

            taskInProgress = true;
            task.Invoke(para1, para2, para3, para4, para5);
        }
        
        public void TaskDone()
        {
            taskInProgress = false;
            if (requestQueue.Count <= 0) return;

            taskInProgress = true;
            requestQueue.Dequeue().Begin();
        }
    }

    public abstract class TaskQueueMono : MonoBehaviour
    {
        private Queue<IRequest> requestQueue;
        private bool taskInProgress;

        protected virtual void Awake()
        {
            requestQueue = new Queue<IRequest>();
            taskInProgress = false;
        }

        protected void BeginTask([NotNull] Action task)
        {
            if (taskInProgress)
            {
                Request request = new Request() { callback = task };
                requestQueue.Enqueue(request);
                return;
            }

            taskInProgress = true;
            task.Invoke();
        }

        protected void BeginTask<T>([NotNull] Action<T> task, T para1)
        {
            if (taskInProgress)
            {
                Request<T> request = new Request<T>() { callback = task, value = para1 };
                requestQueue.Enqueue(request);
                return;
            }

            taskInProgress = true;
            task.Invoke(para1);
        }

        protected void BeginTask<T1, T2>([NotNull] Action<T1, T2> task, T1 para1, T2 para2)
        {
            if (taskInProgress)
            {
                Request<T1, T2> request = new Request<T1, T2>() { callback = task, value1 = para1, value2 = para2 };
                requestQueue.Enqueue(request);
                return;
            }

            taskInProgress = true;
            task.Invoke(para1, para2);
        }

        protected void BeginTask<T1, T2, T3>([NotNull] Action<T1, T2, T3> task, T1 para1, T2 para2, T3 para3)
        {
            if (taskInProgress)
            {
                Request<T1, T2, T3> request = new Request<T1, T2, T3>()
                    { callback = task, value1 = para1, value2 = para2, value3 = para3 };
                requestQueue.Enqueue(request);
                return;
            }

            taskInProgress = true;
            task.Invoke(para1, para2, para3);
        }

        protected void BeginTask<T1, T2, T3, T4>([NotNull] Action<T1, T2, T3, T4> task, T1 para1, T2 para2, T3 para3, T4 para4)
        {
            if (taskInProgress)
            {
                Request<T1, T2, T3, T4> request = new Request<T1, T2, T3, T4>() { callback = task, value1 = para1, value2 = para2, value3 = para3, value4 = para4 };
                requestQueue.Enqueue(request);
                return;
            }

            taskInProgress = true;
            task.Invoke(para1, para2, para3, para4);
        }

        protected void BeginTask<T1, T2, T3, T4, T5>([NotNull] Action<T1, T2, T3, T4, T5> task, T1 para1, T2 para2, T3 para3, T4 para4, T5 para5)
        {
            if (taskInProgress)
            {
                Request<T1, T2, T3, T4, T5> request = new Request<T1, T2, T3, T4, T5>() { callback = task, value1 = para1, value2 = para2, value3 = para3, value4 = para4, value5 = para5 };
                requestQueue.Enqueue(request);
                return;
            }

            taskInProgress = true;
            task.Invoke(para1, para2, para3, para4, para5);
        }

        protected void TaskDone()
        {
            taskInProgress = false;
            if (requestQueue.Count <= 0) return;

            taskInProgress = true;
            requestQueue.Dequeue().Begin();
        }
    }

    public interface IRequest
    {
        public void Begin();
    }

    public class Request : IRequest
    {
        public Action callback;
        public void Begin() => callback.Invoke();
    }

    public class Request<T1> : IRequest
    {
        public Action<T1> callback;
        public T1 value;
        public void Begin() => callback.Invoke(value);
    }

    public class Request<T1, T2> : IRequest
    {
        public Action<T1, T2> callback;
        public T1 value1;
        public T2 value2;
        public void Begin() => callback.Invoke(value1, value2);
    }

    public class Request<T1, T2, T3> : IRequest
    {
        public Action<T1, T2, T3> callback;
        public T1 value1;
        public T2 value2;
        public T3 value3;
        public void Begin() => callback.Invoke(value1, value2, value3);
    }

    public class Request<T1, T2, T3, T4> : IRequest
    {
        public Action<T1, T2, T3, T4> callback;
        public T1 value1;
        public T2 value2;
        public T3 value3;
        public T4 value4;
        public void Begin() => callback.Invoke(value1, value2, value3, value4);
    }

    public class Request<T1, T2, T3, T4, T5> : IRequest
    {
        public Action<T1, T2, T3, T4, T5> callback;
        public T1 value1;
        public T2 value2;
        public T3 value3;
        public T4 value4;
        public T5 value5;
        public void Begin() => callback.Invoke(value1, value2, value3, value4, value5);
    }
}
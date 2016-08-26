using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Extensions
{
    public static class TaskExtensions
    {
        public static Task<T> AttachAction<T>(this Task<T> task, Action<Task<T>> action, TaskContinuationOptions taskContinuationOptions = TaskContinuationOptions.None)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (action == null)
                return task;

            return task.ContinueWith(tsk => action(tsk), taskContinuationOptions)
                       .ContinueWith(tsk => task, TaskContinuationOptions.ExecuteSynchronously)
                       .Unwrap();
        }

        public static Task AttachAction(this Task task, Action<Task> action, TaskContinuationOptions taskContinuationOptions = TaskContinuationOptions.None)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (action == null)
                return task;

            return task.ContinueWith(tsk => action(tsk), taskContinuationOptions)
                       .ContinueWith(tsk => task, TaskContinuationOptions.ExecuteSynchronously)
                       .Unwrap();
        }

        public static Task<T> AttachTask<T>(this Task<T> task, Func<Task<T>, Task> continuationFactory, TaskContinuationOptions taskContinuationOptions = TaskContinuationOptions.None)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (continuationFactory == null)
                return task;

            return task.ContinueWith(tsk => continuationFactory(tsk), taskContinuationOptions | TaskContinuationOptions.ExecuteSynchronously)
                       .Unwrap()
                       .ContinueWith(tsk => task, TaskContinuationOptions.ExecuteSynchronously)
                       .Unwrap();
        }

        public static Task AttachTask(this Task task, Func<Task, Task> continuationFactory, TaskContinuationOptions taskContinuationOptions = TaskContinuationOptions.None)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (continuationFactory == null)
                return task;

            return task.ContinueWith(tsk => continuationFactory(tsk), taskContinuationOptions | TaskContinuationOptions.ExecuteSynchronously)
                       .Unwrap()
                       .ContinueWith(tsk => task, TaskContinuationOptions.ExecuteSynchronously)
                       .Unwrap();
        }

        public static T ExecuteSynchronuously<T>(this Task<T> task)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            try { return task.Result; }
            catch (AggregateException aex) { throw aex.Flatten().InnerExceptions.First(); }
        }

        public static void ExecuteSynchronuously(this Task task)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            try { task.Wait(); }
            catch (AggregateException aex) { throw aex.Flatten().InnerExceptions.First(); }
        }

        public static void ProcessMultipleTasksResults(this IEnumerable<Task> src, Boolean checkForFaulted = true, Boolean checkForCancelled = true)
        {
            if (src == null)
                throw new ArgumentNullException("src");
            if (checkForFaulted)
            {
                var exceptions = new List<Exception>(src.Where(t => t.IsFaulted).SelectMany(t => t.Exception.Flatten().InnerExceptions)).ToArray();
                if (exceptions.Length > 0)
                    throw new AggregateException(exceptions);
            }
            if (checkForCancelled && src.FirstOrDefault(tsk => tsk.IsCanceled) != null)
                throw new OperationCanceledException();
        }
    }
}

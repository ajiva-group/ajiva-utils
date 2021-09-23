using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ajiva.Utils
{
    public static class TaskWatcher
    {
        private static readonly CancellationTokenSource TaskSource = new();
        private static readonly List<Task> Tasks = new();

        public static void Watch(Func<Task?> run)
        {
            Tasks.Add(Task.Run(run, TaskSource.Token));
        }

        public static void Cancel()
        {
            TaskSource.CancelAfter(3000);
            Task.WaitAll(Tasks.ToArray(), 4000);
        }
    }
}

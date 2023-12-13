using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace JFUtils;

[PublicAPI]
public static class TaskExtensions
{
    public static TaskAwaiter<T[]> GetAwaiter<T>(this (Task<T>, Task<T>) tasksTuple) =>
        Task.WhenAll(tasksTuple.Item1, tasksTuple.Item2).GetAwaiter();

    public static TaskAwaiter<T[]> GetAwaiter<T>(this (Task<T>, Task<T>, Task<T>) tasksTuple) =>
        Task.WhenAll(tasksTuple.Item1, tasksTuple.Item2, tasksTuple.Item3).GetAwaiter();

    public static TaskAwaiter<T[]> GetAwaiter<T>(this (Task<T>, Task<T>, Task<T>, Task<T>) tasksTuple) =>
        Task.WhenAll(tasksTuple.Item1, tasksTuple.Item2, tasksTuple.Item3, tasksTuple.Item4).GetAwaiter();
}
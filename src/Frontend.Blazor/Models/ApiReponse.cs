
namespace Frontend.Blazor.Models;

public class ApiResponse<T>
{
    public T Result { get; set; }
    public List<string> Errors { get; set; }
    public static async Task<ApiResponse<T>> HandleExceptionAsync(Func<Task<ApiResponse<T>>> action)
    {
        try
        {
            var result = await action();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new ApiResponse<T>
            {
                Errors = new List<string> {e.Message}
            };
        }
    }
}
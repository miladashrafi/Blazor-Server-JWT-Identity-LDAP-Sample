using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Backend.API.Models;

public class BaseApiResponse<T>
{
    public BaseApiResponse(T result)
    {
        Result = result;
    }

    public BaseApiResponse()
    {
    }

    public T Result { get; set; }
    public List<string> Errors { get; set; }

    public bool AddModelErrors(ModelStateDictionary modelState)
    {
        Errors = modelState.Root.Children.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
        return Errors.Count > 0;
    }

}
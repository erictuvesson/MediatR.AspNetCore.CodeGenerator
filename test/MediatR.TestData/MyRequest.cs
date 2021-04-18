using System.ComponentModel.DataAnnotations;

namespace MediatR.TestData
{
    /// <summary>
    /// My Summary
    /// </summary>
    [Display(GroupName = "MyCategory")]
    public class MyRequest : IRequest<bool>
    {
    }

    /// <summary>
    /// My Summary
    /// </summary>
    [Display(GroupName = "MyCategory2")]
    public class MyRequest2 : IRequest<bool>
    {
    }
}

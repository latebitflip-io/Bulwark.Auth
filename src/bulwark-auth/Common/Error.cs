namespace Bulwark.Auth.Common;
public class Error
{
    public int Status { get; set; }
    public string Title { get; set; }
    public string Detail { get; set; }

    public Error()
    {
        Title = string.Empty;
        Detail = string.Empty;
    }
}


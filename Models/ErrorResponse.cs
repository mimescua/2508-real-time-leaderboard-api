namespace SafeProjectName.Models;

public class ErrorResponse
{
	public int StatusCode { get; set; }
	public string? ReasonPhrase => ((System.Net.HttpStatusCode)StatusCode).ToString();
	public string? Title { get; set; }
	public string? Message { get; set; }
	public string? Instance { get; set; }
}

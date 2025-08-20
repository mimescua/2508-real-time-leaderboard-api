using System.Net.Mail;

namespace SafeProjectName.Helpers;

public static class Validations
{
	public static bool IsValidEmail(string email)
	{
		if (string.IsNullOrWhiteSpace(email))
		{
			return false;
		}

		try
		{
			var addr = new MailAddress(email);
			return addr.Address == email;
		}
		catch
		{
			return false;
		}
	}
}

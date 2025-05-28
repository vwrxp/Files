using System.Runtime.InteropServices;
using Windows.Security.Credentials;

namespace Files.App.Helpers
{
	internal sealed class CredentialsHelpers
	{
		// Security: Added input validation and exception handling for credential operations
		public static void SavePassword(string resourceName, string username, string password)
		{
			if (string.IsNullOrWhiteSpace(resourceName) || string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
				throw new ArgumentException("Invalid credential input");
			try
			{
				var vault = new PasswordVault();
				var credential = new PasswordCredential(resourceName, username, password);
				vault.Add(credential);
			}
			catch (Exception ex)
			{
				// Log or handle exception as needed
				throw new InvalidOperationException("Failed to save password", ex);
			}
		}

		// Remove saved credentials from the vault
		public static void DeleteSavedPassword(string resourceName, string username)
		{
			if (string.IsNullOrWhiteSpace(resourceName) || string.IsNullOrWhiteSpace(username))
				throw new ArgumentException("Invalid credential input");
			try
			{
				var vault = new PasswordVault();
				var credential = vault.Retrieve(resourceName, username);
				vault.Remove(credential);
			}
			catch (Exception ex)
			{
				// Log or handle exception as needed
				throw new InvalidOperationException("Failed to delete password", ex);
			}
		}

		public static string GetPassword(string resourceName, string username)
		{
			if (string.IsNullOrWhiteSpace(resourceName) || string.IsNullOrWhiteSpace(username))
				return string.Empty;
			try
			{
				var vault = new PasswordVault();
				var credential = vault.Retrieve(resourceName, username);
				credential.RetrievePassword();
				return credential.Password;
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
	}
}

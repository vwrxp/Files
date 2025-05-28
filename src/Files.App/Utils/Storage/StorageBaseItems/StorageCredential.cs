// Copyright (c) Files Community
// Licensed under the MIT License.

using System.Runtime.InteropServices;
using System.Security;

namespace Files.App.Utils.Storage
{
	// Optimized for security, maintainability, and performance
	public sealed class StorageCredential : IDisposable
	{
		private SecureString? _securePassword;

		public string UserName { get; set; } = string.Empty;

		// Only expose SecurePassword for security; use GetPassword() to retrieve as string if needed
		// Ensure SecurePassword is always read-only for extra safety
		public SecureString SecurePassword
		{
			get => _securePassword?.Copy() ?? new SecureString();
			set
			{
				_securePassword?.Dispose();
				_securePassword = value?.Copy() ?? new SecureString();
				_securePassword?.MakeReadOnly(); // Enforce read-only
			}
		}

		public StorageCredential() : this(string.Empty, (SecureString?)null) { }

		public StorageCredential(string? userName, string? password)
		{
			UserName = userName ?? string.Empty;
			_securePassword = !string.IsNullOrEmpty(password) ? MarshalToSecureString(password) : new SecureString();
			_securePassword.MakeReadOnly(); // Enforce read-only
		}

		public StorageCredential(string? userName, SecureString? password)
		{
			UserName = userName ?? string.Empty;
			_securePassword = password?.Copy() ?? new SecureString();
			_securePassword.MakeReadOnly(); // Enforce read-only
		}

		// Securely get password as string (use only when necessary)
		public string GetPassword()
		{
			if (_securePassword == null || _securePassword.Length == 0)
				return string.Empty;
			return MarshalToString(_securePassword);
		}

		private static string MarshalToString(SecureString sstr)
		{
			if (sstr == null || sstr.Length == 0)
				return string.Empty;
			IntPtr ptr = IntPtr.Zero;
			try
			{
				ptr = Marshal.SecureStringToGlobalAllocUnicode(sstr);
				return Marshal.PtrToStringUni(ptr) ?? string.Empty;
			}
			finally
			{
				if (ptr != IntPtr.Zero)
					Marshal.ZeroFreeGlobalAllocUnicode(ptr);
			}
		}

		private static SecureString MarshalToSecureString(string str)
		{
			if (string.IsNullOrEmpty(str))
				return new SecureString();
			var secure = new SecureString();
			foreach (var c in str)
				secure.AppendChar(c);
			secure.MakeReadOnly(); // Always make read-only
			return secure;
		}

		// Dispose pattern for secure cleanup
		public void Dispose()
		{
			_securePassword?.Dispose();
			_securePassword = null;
			GC.SuppressFinalize(this);
		}
	}
}

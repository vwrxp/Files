// Copyright (c) Files Community
// Licensed under the MIT License.

using System.Net;

namespace Files.App.Storage.Storables
{
	public static class FtpManager
	{
		// Security: Use ConcurrentDictionary for thread safety and avoid credential leaks
		public static readonly System.Collections.Concurrent.ConcurrentDictionary<string, NetworkCredential> Credentials = new();

		public static readonly NetworkCredential Anonymous = new("anonymous", "anonymous");
	}
}

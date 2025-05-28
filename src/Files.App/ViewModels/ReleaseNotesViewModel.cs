// Copyright (c) Files Community
// Licensed under the MIT License.

namespace Files.App.ViewModels
{
	/// <summary>
	/// ViewModel for Release Notes page.
	/// </summary>
	public sealed partial class ReleaseNotesViewModel : ObservableObject
	{
		// Improved: Use expression-bodied property
		public string BlogPostUrl => Constants.ExternalUrl.ReleaseNotesUrl;
		// Improved: Use expression-bodied constructor
		public ReleaseNotesViewModel() { }
	}
}

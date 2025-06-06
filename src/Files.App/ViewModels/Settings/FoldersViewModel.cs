// Copyright (c) Files Community
// Licensed under the MIT License.

namespace Files.App.ViewModels.Settings
{
	public sealed partial class FoldersViewModel : ObservableObject
	{
		private IUserSettingsService UserSettingsService { get; } = Ioc.Default.GetRequiredService<IUserSettingsService>();


		public Dictionary<SizeUnitTypes, string> SizeUnitsOptions { get; private set; } = [];

		public FoldersViewModel()
		{
			SelectedDeleteConfirmationPolicyIndex = (int)DeleteConfirmationPolicy;

			// Size unit format
			SizeUnitsOptions.Add(SizeUnitTypes.BinaryUnits, Strings.Binary.GetLocalizedResource());
			SizeUnitsOptions.Add(SizeUnitTypes.DecimalUnits, Strings.Decimal.GetLocalizedResource());
			SizeUnitFormat = SizeUnitsOptions[UserSettingsService.FoldersSettingsService.SizeUnitFormat];
		}

		// Properties



		private int selectedDeleteConfirmationPolicyIndex;
		public int SelectedDeleteConfirmationPolicyIndex
		{
			get => selectedDeleteConfirmationPolicyIndex;
			set
			{
				if (SetProperty(ref selectedDeleteConfirmationPolicyIndex, value))
				{
					OnPropertyChanged(nameof(SelectedDeleteConfirmationPolicyIndex));
					DeleteConfirmationPolicy = (DeleteConfirmationPolicies)value;
				}
			}
		}

		public bool ShowHiddenItems
		{
			get => UserSettingsService.FoldersSettingsService.ShowHiddenItems;
			set
			{
				if (value != UserSettingsService.FoldersSettingsService.ShowHiddenItems)
				{
					UserSettingsService.FoldersSettingsService.ShowHiddenItems = value;

					OnPropertyChanged();
				}
			}
		}

		public bool ShowProtectedSystemFiles
		{
			get => UserSettingsService.FoldersSettingsService.ShowProtectedSystemFiles;
			set
			{
				if (value != UserSettingsService.FoldersSettingsService.ShowProtectedSystemFiles)
				{
					UserSettingsService.FoldersSettingsService.ShowProtectedSystemFiles = value;

					OnPropertyChanged();
				}
			}
		}

		public bool AreAlternateStreamsVisible
		{
			get => UserSettingsService.FoldersSettingsService.AreAlternateStreamsVisible;
			set
			{
				if (value != UserSettingsService.FoldersSettingsService.AreAlternateStreamsVisible)
				{
					UserSettingsService.FoldersSettingsService.AreAlternateStreamsVisible = value;

					OnPropertyChanged();
				}
			}
		}

		public bool ShowDotFiles
		{
			get => UserSettingsService.FoldersSettingsService.ShowDotFiles;
			set
			{
				if (value != UserSettingsService.FoldersSettingsService.ShowDotFiles)
				{
					UserSettingsService.FoldersSettingsService.ShowDotFiles = value;

					OnPropertyChanged();
				}
			}
		}

		public bool OpenItemsWithOneClick
		{
			get => UserSettingsService.FoldersSettingsService.OpenItemsWithOneClick;
			set
			{
				if (value != UserSettingsService.FoldersSettingsService.OpenItemsWithOneClick)
				{
					UserSettingsService.FoldersSettingsService.OpenItemsWithOneClick = value;

					OnPropertyChanged();
				}
			}
		}

		public bool ColumnLayoutOpenFoldersWithOneClick
		{
			get => UserSettingsService.FoldersSettingsService.ColumnLayoutOpenFoldersWithOneClick;
			set
			{
				if (value != UserSettingsService.FoldersSettingsService.ColumnLayoutOpenFoldersWithOneClick)
				{
					UserSettingsService.FoldersSettingsService.ColumnLayoutOpenFoldersWithOneClick = value;

					OnPropertyChanged();
				}
			}
		}

		public bool OpenFoldersNewTab
		{
			get => UserSettingsService.FoldersSettingsService.OpenFoldersInNewTab;
			set
			{
				if (value != UserSettingsService.FoldersSettingsService.OpenFoldersInNewTab)
				{
					UserSettingsService.FoldersSettingsService.OpenFoldersInNewTab = value;

					OnPropertyChanged();
				}
			}
		}

		public bool CalculateFolderSizes
		{
			get => UserSettingsService.FoldersSettingsService.CalculateFolderSizes;
			set
			{
				if (value != UserSettingsService.FoldersSettingsService.CalculateFolderSizes)
				{
					UserSettingsService.FoldersSettingsService.CalculateFolderSizes = value;

					OnPropertyChanged();
				}
			}
		}

		public bool ScrollToPreviousFolderWhenNavigatingUp
		{
			get => UserSettingsService.FoldersSettingsService.ScrollToPreviousFolderWhenNavigatingUp;
			set
			{
				if (value != UserSettingsService.FoldersSettingsService.ScrollToPreviousFolderWhenNavigatingUp)
				{
					UserSettingsService.FoldersSettingsService.ScrollToPreviousFolderWhenNavigatingUp = value;

					OnPropertyChanged();
				}
			}
		}

		public bool ShowFileExtensions
		{
			get => UserSettingsService.FoldersSettingsService.ShowFileExtensions;
			set
			{
				if (value != UserSettingsService.FoldersSettingsService.ShowFileExtensions)
				{
					UserSettingsService.FoldersSettingsService.ShowFileExtensions = value;

					OnPropertyChanged();
				}
			}
		}

		public bool ShowThumbnails
		{
			get => UserSettingsService.FoldersSettingsService.ShowThumbnails;
			set
			{
				if (value != UserSettingsService.FoldersSettingsService.ShowThumbnails)
				{
					UserSettingsService.FoldersSettingsService.ShowThumbnails = value;

					OnPropertyChanged();
				}
			}
		}

		public DeleteConfirmationPolicies DeleteConfirmationPolicy
		{
			get => UserSettingsService.FoldersSettingsService.DeleteConfirmationPolicy;
			set
			{
				if (value != UserSettingsService.FoldersSettingsService.DeleteConfirmationPolicy)
				{
					UserSettingsService.FoldersSettingsService.DeleteConfirmationPolicy = value;

					OnPropertyChanged();
				}
			}
		}

		public bool SelectFilesOnHover
		{
			get => UserSettingsService.FoldersSettingsService.SelectFilesOnHover;
			set
			{
				if (value != UserSettingsService.FoldersSettingsService.SelectFilesOnHover)
				{
					UserSettingsService.FoldersSettingsService.SelectFilesOnHover = value;

					OnPropertyChanged();
				}
			}
		}

		public bool DoubleClickToGoUp
		{
			get => UserSettingsService.FoldersSettingsService.DoubleClickToGoUp;
			set
			{
				if (value != UserSettingsService.FoldersSettingsService.DoubleClickToGoUp)
				{
					UserSettingsService.FoldersSettingsService.DoubleClickToGoUp = value;

					OnPropertyChanged();
				}
			}
		}

		public bool ShowFileExtensionWarning
		{
			get => UserSettingsService.FoldersSettingsService.ShowFileExtensionWarning;
			set
			{
				if (value != UserSettingsService.FoldersSettingsService.ShowFileExtensionWarning)
				{
					UserSettingsService.FoldersSettingsService.ShowFileExtensionWarning = value;

					OnPropertyChanged();
				}
			}
		}

		// Use expression-bodied property for brevity
		public bool ShowCheckboxesWhenSelectingItems
		{
			get => UserSettingsService.FoldersSettingsService.ShowCheckboxesWhenSelectingItems;
			set
			{
				if (value == UserSettingsService.FoldersSettingsService.ShowCheckboxesWhenSelectingItems)
					return;
				UserSettingsService.FoldersSettingsService.ShowCheckboxesWhenSelectingItems = value;
				OnPropertyChanged();
			}
		}

		private string sizeUnitFormat;
		public string SizeUnitFormat
		{
			get => sizeUnitFormat;
			set
			{
				if (!SetProperty(ref sizeUnitFormat, value))
					return;
				// Use TryGetValue for clarity and safety
				if (SizeUnitsOptions.FirstOrDefault(e => e.Value == value).Key is SizeUnitTypes key)
					UserSettingsService.FoldersSettingsService.SizeUnitFormat = key;
			}
		}
	}
}

// Copyright (c) Files Community
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System.Windows.Input;
using Windows.System;

namespace Files.App.ViewModels
{
	/// <summary>
	/// Represents ViewModel of <see cref="MainPage"/>.
	/// </summary>
	public sealed partial class MainPageViewModel : ObservableObject
	{
		private readonly IAppearanceSettingsService AppearanceSettingsService = Ioc.Default.GetRequiredService<IAppearanceSettingsService>();
		private readonly IGeneralSettingsService GeneralSettingsService = Ioc.Default.GetRequiredService<IGeneralSettingsService>();
		private readonly INetworkService NetworkService = Ioc.Default.GetRequiredService<INetworkService>();
		private readonly IUserSettingsService UserSettingsService = Ioc.Default.GetRequiredService<IUserSettingsService>();
		private readonly IResourcesService ResourcesService = Ioc.Default.GetRequiredService<IResourcesService>();
		private readonly DrivesViewModel DrivesViewModel = Ioc.Default.GetRequiredService<DrivesViewModel>();
		public ShelfViewModel ShelfViewModel { get; } = Ioc.Default.GetRequiredService<ShelfViewModel>();
		private readonly IContentPageContext context = Ioc.Default.GetRequiredService<IContentPageContext>();

		public static ObservableCollection<TabBarItem> AppInstances { get; } = new();
		public List<ITabBar> MultitaskingControls { get; } = new();
		public ITabBar? MultitaskingControl { get; set; }

		// changed field name for clarity
		private bool isSidebarPaneOpen;
		public bool IsSidebarPaneOpen
		{
			get => isSidebarPaneOpen;
			set => SetProperty(ref isSidebarPaneOpen, value);
		}

		// changed field name for clarity
		private bool isSidebarPaneOpenToggleButtonVisible;
		public bool IsSidebarPaneOpenToggleButtonVisible
		{
			get => isSidebarPaneOpenToggleButtonVisible;
			set => SetProperty(ref isSidebarPaneOpenToggleButtonVisible, value);
		}

		private TabBarItem? selectedTabItem;
		public TabBarItem? SelectedTabItem
		{
			get => selectedTabItem;
			set => SetProperty(ref selectedTabItem, value);
		}

		private bool shouldViewControlBeDisplayed;
		public bool ShouldViewControlBeDisplayed
		{
			get => shouldViewControlBeDisplayed;
			set => SetProperty(ref shouldViewControlBeDisplayed, value);
		}

		private bool shouldPreviewPaneBeActive;
		public bool ShouldPreviewPaneBeActive
		{
			get => shouldPreviewPaneBeActive;
			set => SetProperty(ref shouldPreviewPaneBeActive, value);
		}

		private bool shouldPreviewPaneBeDisplayed;
		public bool ShouldPreviewPaneBeDisplayed
		{
			get => shouldPreviewPaneBeDisplayed;
			set => SetProperty(ref shouldPreviewPaneBeDisplayed, value);
		}

		public bool ShowShelfPane => GeneralSettingsService.ShowShelfPane && AppLifecycleHelper.AppEnvironment == AppEnvironment.Dev;
		public Stretch AppThemeBackgroundImageFit => AppearanceSettingsService.AppThemeBackgroundImageFit;
		public float AppThemeBackgroundImageOpacity => AppearanceSettingsService.AppThemeBackgroundImageOpacity;
		public ImageSource? AppThemeBackgroundImageSource
		{
			get
			{
				if (string.IsNullOrWhiteSpace(AppearanceSettingsService.AppThemeBackgroundImageSource))
					return null;
				if (!Uri.TryCreate(AppearanceSettingsService.AppThemeBackgroundImageSource, UriKind.RelativeOrAbsolute, out var validUri))
					return null;
				try
				{
					return new BitmapImage(validUri);
				}
				catch
				{
					return null;
				}
			}
		}
		public VerticalAlignment AppThemeBackgroundImageVerticalAlignment => AppearanceSettingsService.AppThemeBackgroundImageVerticalAlignment;
		public HorizontalAlignment AppThemeBackgroundImageHorizontalAlignment => AppearanceSettingsService.AppThemeBackgroundImageHorizontalAlignment;
		public bool ShowToolbar => AppearanceSettingsService.ShowToolbar && context.PageType != ContentPageTypes.Home && context.PageType != ContentPageTypes.ReleaseNotes && context.PageType != ContentPageTypes.Settings;
		public bool ShowStatusBar => context.PageType != ContentPageTypes.Home && context.PageType != ContentPageTypes.ReleaseNotes && context.PageType != ContentPageTypes.Settings;

		public ICommand NavigateToNumberedTabKeyboardAcceleratorCommand { get; }

		public MainPageViewModel()
		{
			NavigateToNumberedTabKeyboardAcceleratorCommand = new RelayCommand<KeyboardAcceleratorInvokedEventArgs>(ExecuteNavigateToNumberedTabKeyboardAcceleratorCommand);
			AppearanceSettingsService.PropertyChanged += (s, e) =>
			{
				switch (e.PropertyName)
				{
					case nameof(IAppearanceSettingsService.AppThemeBackgroundImageSource):
						OnPropertyChanged(nameof(AppThemeBackgroundImageSource));
						break;
					case nameof(IAppearanceSettingsService.AppThemeBackgroundImageOpacity):
						OnPropertyChanged(nameof(AppThemeBackgroundImageOpacity));
						break;
					case nameof(IAppearanceSettingsService.AppThemeBackgroundImageFit):
						OnPropertyChanged(nameof(AppThemeBackgroundImageFit));
						break;
					case nameof(IAppearanceSettingsService.AppThemeBackgroundImageVerticalAlignment):
						OnPropertyChanged(nameof(AppThemeBackgroundImageVerticalAlignment));
						break;
					case nameof(IAppearanceSettingsService.AppThemeBackgroundImageHorizontalAlignment):
						OnPropertyChanged(nameof(AppThemeBackgroundImageHorizontalAlignment));
						break;
					case nameof(IAppearanceSettingsService.ShowToolbar):
						OnPropertyChanged(nameof(ShowToolbar));
						break;
				}
			};
			context.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(IContentPageContext.PageType))
				{
					OnPropertyChanged(nameof(ShowToolbar));
					OnPropertyChanged(nameof(ShowStatusBar));
				}
			};
			GeneralSettingsService.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(IGeneralSettingsService.ShowShelfPane))
					OnPropertyChanged(nameof(ShowShelfPane));
			};
		}

		public async Task OnNavigatedToAsync(NavigationEventArgs e)
		{
			if (e.NavigationMode == NavigationMode.Back)
				return;
			var parameter = e.Parameter;
			var ignoreStartupSettings = false;
			if (parameter is MainPageNavigationArguments mainPageNavigationArguments)
			{
				parameter = mainPageNavigationArguments.Parameter;
				ignoreStartupSettings = mainPageNavigationArguments.IgnoreStartupSettings;
			}
			if (parameter is null || (parameter is string eventStr && string.IsNullOrEmpty(eventStr)))
			{
				try
				{
					if (!UserSettingsService.AppSettingsService.RestoreTabsOnStartup && !UserSettingsService.GeneralSettingsService.ContinueLastSessionOnStartUp && UserSettingsService.GeneralSettingsService.LastSessionTabList != null)
					{
						var items = UserSettingsService.GeneralSettingsService.LastSessionTabList
							.Where(tab => !string.IsNullOrEmpty(tab))
							.Select(TabBarItemParameter.Deserialize).ToArray();
						BaseTabBar.PushRecentTab(items);
					}
					if (UserSettingsService.AppSettingsService.RestoreTabsOnStartup)
					{
						UserSettingsService.AppSettingsService.RestoreTabsOnStartup = false;
						if (UserSettingsService.GeneralSettingsService.LastSessionTabList != null)
						{
							foreach (var tabArgsString in UserSettingsService.GeneralSettingsService.LastSessionTabList)
							{
								var tabArgs = TabBarItemParameter.Deserialize(tabArgsString);
								await NavigationHelpers.AddNewTabByParamAsync(tabArgs.InitialPageType, tabArgs.NavigationParameter);
							}
							if (!UserSettingsService.GeneralSettingsService.ContinueLastSessionOnStartUp)
								UserSettingsService.GeneralSettingsService.LastSessionTabList = null;
						}
					}
					else if (UserSettingsService.GeneralSettingsService.OpenSpecificPageOnStartup && UserSettingsService.GeneralSettingsService.TabsOnStartupList != null)
					{
						foreach (var path in UserSettingsService.GeneralSettingsService.TabsOnStartupList)
							await NavigationHelpers.AddNewTabByPathAsync(typeof(ShellPanesPage), path, true);
					}
					else if (UserSettingsService.GeneralSettingsService.ContinueLastSessionOnStartUp && UserSettingsService.GeneralSettingsService.LastSessionTabList != null)
					{
						if (AppInstances.Count == 0)
						{
							foreach (var tabArgsString in UserSettingsService.GeneralSettingsService.LastSessionTabList)
							{
								var tabArgs = TabBarItemParameter.Deserialize(tabArgsString);
								await NavigationHelpers.AddNewTabByParamAsync(tabArgs.InitialPageType, tabArgs.NavigationParameter);
							}
						}
					}
					else
					{
						await NavigationHelpers.AddNewTabAsync();
					}
				}
				catch
				{
					await NavigationHelpers.AddNewTabAsync();
				}
			}
			else
			{
				if (!ignoreStartupSettings)
				{
					try
					{
						if (UserSettingsService.GeneralSettingsService.OpenSpecificPageOnStartup && UserSettingsService.GeneralSettingsService.TabsOnStartupList != null)
						{
							foreach (var path in UserSettingsService.GeneralSettingsService.TabsOnStartupList)
								await NavigationHelpers.AddNewTabByPathAsync(typeof(ShellPanesPage), path, true);
						}
						else if (UserSettingsService.GeneralSettingsService.ContinueLastSessionOnStartUp && UserSettingsService.GeneralSettingsService.LastSessionTabList != null && AppInstances.Count == 0)
						{
							foreach (var tabArgsString in UserSettingsService.GeneralSettingsService.LastSessionTabList)
							{
								var tabArgs = TabBarItemParameter.Deserialize(tabArgsString);
								await NavigationHelpers.AddNewTabByParamAsync(tabArgs.InitialPageType, tabArgs.NavigationParameter);
							}
						}
					}
					catch { }
				}
				if (parameter is string navArgs)
					await NavigationHelpers.AddNewTabByPathAsync(typeof(ShellPanesPage), navArgs, true);
				else if (parameter is PaneNavigationArguments paneArgs)
					await NavigationHelpers.AddNewTabByParamAsync(typeof(ShellPanesPage), paneArgs);
				else if (parameter is TabBarItemParameter tabArgs)
					await NavigationHelpers.AddNewTabByParamAsync(tabArgs.InitialPageType, tabArgs.NavigationParameter);
			}
			ResourcesService.LoadAppResources(AppearanceSettingsService);
			await Task.WhenAll(
				DrivesViewModel.UpdateDrivesAsync(),
				NetworkService.UpdateComputersAsync(),
				NetworkService.UpdateShortcutsAsync());
		}

		private async void ExecuteNavigateToNumberedTabKeyboardAcceleratorCommand(KeyboardAcceleratorInvokedEventArgs? e)
		{
			var indexToSelect = e!.KeyboardAccelerator.Key switch
			{
				VirtualKey.Number1 => 0,
				VirtualKey.Number2 => 1,
				VirtualKey.Number3 => 2,
				VirtualKey.Number4 => 3,
				VirtualKey.Number5 => 4,
				VirtualKey.Number6 => 5,
				VirtualKey.Number7 => 6,
				VirtualKey.Number8 => 7,
				VirtualKey.Number9 => AppInstances.Count - 1,
				_ => AppInstances.Count - 1,
			};
			if (indexToSelect < AppInstances.Count)
			{
				App.AppModel.TabStripSelectedIndex = indexToSelect;
				await Task.Delay(500);
				(SelectedTabItem?.TabItemContent as Control)?.Focus(FocusState.Programmatic);
			}
			e.Handled = true;
		}
	}
}

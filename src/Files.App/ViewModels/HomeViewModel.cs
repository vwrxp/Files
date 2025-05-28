// Copyright (c) Files Community
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using System.Windows.Input;

namespace Files.App.ViewModels
{
	public sealed partial class HomeViewModel : ObservableObject, IDisposable
	{
		// Dependency injections

		private readonly IUserSettingsService UserSettingsService = Ioc.Default.GetRequiredService<IUserSettingsService>();

		// Properties

		public ObservableCollection<WidgetContainerItem> WidgetItems { get; } = new();

		// Commands

		public ICommand ReloadWidgetsCommand { get; }

		// Constructor

		public HomeViewModel() => ReloadWidgetsCommand = new AsyncRelayCommand(ExecuteReloadWidgetsCommand);

		// Methods

		private void ReloadWidgets()
		{
			var reloadQuickAccessWidget = WidgetsHelpers.TryGetWidget<QuickAccessWidgetViewModel>(this);
			var reloadDrivesWidget = WidgetsHelpers.TryGetWidget<DrivesWidgetViewModel>(this);
			var reloadNetworkLocationsWidget = WidgetsHelpers.TryGetWidget<NetworkLocationsWidgetViewModel>(this);
			var reloadFileTagsWidget = WidgetsHelpers.TryGetWidget<FileTagsWidgetViewModel>(this);
			var reloadRecentFilesWidget = WidgetsHelpers.TryGetWidget<RecentFilesWidgetViewModel>(this);
			var insertIndex = 0;

			if (reloadQuickAccessWidget)
			{
				var quickAccessWidget = new QuickAccessWidget();

				InsertWidget(
					new(
						quickAccessWidget,
						quickAccessWidget.ViewModel,
						(value) => UserSettingsService.GeneralSettingsService.FoldersWidgetExpanded = value,
						() => UserSettingsService.GeneralSettingsService.FoldersWidgetExpanded),
					insertIndex++);
			}

			if (reloadDrivesWidget)
			{
				var drivesWidget = new DrivesWidget();

				InsertWidget(
					new(
						drivesWidget,
						drivesWidget.ViewModel,
						(value) => UserSettingsService.GeneralSettingsService.DrivesWidgetExpanded = value,
						() => UserSettingsService.GeneralSettingsService.DrivesWidgetExpanded),
					insertIndex++);
			}

			if (reloadNetworkLocationsWidget)
			{
				var networkLocationsWidget = new NetworkLocationsWidget();

				InsertWidget(
					new(
						networkLocationsWidget,
						networkLocationsWidget.ViewModel,
						(value) => UserSettingsService.GeneralSettingsService.NetworkLocationsWidgetExpanded = value,
						() => UserSettingsService.GeneralSettingsService.NetworkLocationsWidgetExpanded),
					insertIndex++);
			}

			if (reloadFileTagsWidget)
			{
				var fileTagsWidget = new FileTagsWidget();

				InsertWidget(
					new(
						fileTagsWidget,
						fileTagsWidget.ViewModel,
						(value) => UserSettingsService.GeneralSettingsService.FileTagsWidgetExpanded = value,
						() => UserSettingsService.GeneralSettingsService.FileTagsWidgetExpanded),
					insertIndex++);
			}

			if (reloadRecentFilesWidget)
			{
				var recentFilesWidget = new RecentFilesWidget();

				InsertWidget(
					new(
						recentFilesWidget,
						recentFilesWidget.ViewModel,
						(value) => UserSettingsService.GeneralSettingsService.RecentFilesWidgetExpanded = value,
						() => UserSettingsService.GeneralSettingsService.RecentFilesWidgetExpanded),
					insertIndex++);
			}
		}

		public void RefreshWidgetList()
		{
			for (int i = 0; i < WidgetItems.Count; i++)
			{
				if (!WidgetItems[i].WidgetItemModel.IsWidgetSettingEnabled)
					RemoveWidgetAt(i);
			}

			ReloadWidgets();
		}

		public async Task RefreshWidgetProperties()
		{
			foreach (var viewModel in WidgetItems.Select(x => x.WidgetItemModel))
				await viewModel.RefreshWidgetAsync();
		}

		private bool InsertWidget(WidgetContainerItem widgetModel, int atIndex)
		{
			if (widgetModel.WidgetItemModel is not IWidgetViewModel widgetItemModel)
				return false;
			if (!CanAddWidget(widgetItemModel.WidgetName))
				return false;
			if (atIndex > WidgetItems.Count)
				WidgetItems.Add(widgetModel);
			else
				WidgetItems.Insert(atIndex, widgetModel);
			return true;
		}

		public bool CanAddWidget(string widgetName) => !WidgetItems.Any(item => item.WidgetItemModel.WidgetName == widgetName);

		private void RemoveWidgetAt(int index)
		{
			if (index < 0 || index >= WidgetItems.Count)
				return;
			WidgetItems[index].Dispose();
			WidgetItems.RemoveAt(index);
		}

		public void RemoveWidget<TWidget>() where TWidget : IWidgetViewModel
		{
			var indexToRemove = WidgetItems.FindIndex(item => typeof(TWidget).IsAssignableFrom(item.WidgetControl.GetType()));
			RemoveWidgetAt(indexToRemove);
		}

		// Command methods

		private async Task ExecuteReloadWidgetsCommand()
		{
			ReloadWidgets();
			await RefreshWidgetProperties();
		}

		// Disposer

		public void Dispose()
		{
			foreach (var item in WidgetItems)
				item.Dispose();
			WidgetItems.Clear();
		}
	}
}

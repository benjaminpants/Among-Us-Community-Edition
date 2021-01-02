using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace GenerationsLib.WPF.Controls
{
    public class RecentsListMenuItem : System.Windows.Controls.MenuItem
    {
        public List<RecentItem> RecentItemsSource { get => GetRecentItemsSource(); set => SetRecentItemsSource(value); }
        private List<RecentItem> _recentItemsSource;
        public event EventHandler<RecentItem> RecentItemSelected;
        private MenuItem NoRecentItems = new MenuItem();
        private IList<MenuItem> RecentItems = new List<MenuItem>();
        public string EmptyRecentListString { get; set; } = "No Recent Items";
        public bool HasItemLimit { get; set; } = true;

        public RecentsListMenuItem()
        {
            this.SubmenuOpened += RecentsListMenuItem_SubmenuOpened;
            AddEmptyRecentsListItem();
            RefreshRecentFiles(_recentItemsSource);
        }

        public void RefreshList()
        {
            RefreshRecentFiles(RecentItemsSource);
        }

        private List<RecentItem> GetRecentItemsSource()
        {
            return _recentItemsSource;
        }

        private void SetRecentItemsSource(List<RecentItem> value)
        {
            _recentItemsSource = value;
            RefreshRecentFiles(value);
        }


        #region Main Code

        private void RecentsListMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            RefreshRecentFiles(RecentItemsSource);
        }

        private void RecentItem_Click(object sender, RoutedEventArgs e)
        {
            RecentItemSelected?.Invoke(sender, (sender as MenuItem).Tag as RecentItem);
            AddItemToRecentsList((sender as MenuItem).Tag as RecentItem);
        }

        private void AddItemToRecentsList(RecentItem item)
        {
            try
            {
                var RecentsList = RecentItemsSource;

                if (RecentsList == null)
                {
                    RecentsList = new List<RecentItem>();
                    RecentItemsSource = RecentsList;
                }

                if (RecentsList.Contains(item))
                {
                    RecentsList.Remove(item);
                }

                if (RecentsList.Count >= 10 && HasItemLimit)
                {
                    for (int i = 9; i < RecentsList.Count; i++)
                    {
                        RecentsList.RemoveAt(i);
                    }
                }

                RecentsList.Insert(0, item);

                RefreshRecentFiles(RecentsList);


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("Failed to add entry to recent list: " + ex);
            }
        }



        public void UpdateRecentsDropDown(RecentItem itemToAdd = null)
        {
            if (itemToAdd != null) AddItemToRecentsList(itemToAdd);
            RefreshRecentFiles(RecentItemsSource);
        }

        public void RefreshRecentFiles(List<RecentItem> items)
        {
            if (items == null) items = new List<RecentItem>();
            if (items.Count > 0)
            {
                HideEmptyRecentsListItem();
                CleanUpRecentList();

                int index = 1;
                foreach (var entry in items)
                {
                    int item_key;
                    if (index == 9) item_key = index;
                    else if (index >= 10 && HasItemLimit) item_key = -1;
                    else item_key = index;
                    RecentItems.Add(CreateDataDirectoryMenuLink(entry, index));
                    index++;
                }



                foreach (MenuItem menuItem in RecentItems.Reverse())
                {
                    this.Items.Insert(0, menuItem);
                }
            }
            else
            {
                CleanUpRecentList();
                ShowEmptyRecentsListItem();
            }



        }

        private void AddEmptyRecentsListItem()
        {
            NoRecentItems = new MenuItem();
            NoRecentItems.Header = EmptyRecentListString;
            NoRecentItems.IsEnabled = false;
            this.Items.Add(NoRecentItems);
        }

        private void HideEmptyRecentsListItem()
        {
            NoRecentItems.Visibility = Visibility.Collapsed;
        }

        private void ShowEmptyRecentsListItem()
        {
            NoRecentItems.Visibility = Visibility.Visible;
        }

        private MenuItem CreateDataDirectoryMenuLink(RecentItem item, int index)
        {
            MenuItem newItem = new MenuItem();
            newItem.Header = item.Header;
            newItem.Tag = item;
            newItem.Click += RecentItem_Click;
            newItem.Visibility = Visibility.Visible;
            return newItem;
        }

        private void CleanUpRecentList()
        {
            if (RecentItemsSource == null) RecentItemsSource = new List<RecentItem>();

            foreach (var menuItem in RecentItems)
            {
                menuItem.Click -= RecentItem_Click;
                this.Items.Remove(menuItem);
            }

            List<RecentItem> ItemsForRemoval = new List<RecentItem>();
            List<RecentItem> ItemsWithoutDuplicates = new List<RecentItem>();

            for (int i = 0; i < RecentItemsSource.Count; i++)
            {
                if (ItemsWithoutDuplicates.Contains(RecentItemsSource[i]))
                {
                    ItemsForRemoval.Add(RecentItemsSource[i]);
                }
                else
                {
                    ItemsWithoutDuplicates.Add(RecentItemsSource[i]);
                    if (File.Exists(RecentItemsSource[i].FilePath) || !RecentItemsSource[i].hasFilePath) continue;
                    else ItemsForRemoval.Add(RecentItemsSource[i]);
                }

            }
            foreach (RecentItem item in ItemsForRemoval)
            {
                RecentItemsSource.Remove(item);
            }

            RecentItems.Cast<MenuItem>().Distinct().ToList();

            RecentItems.Clear();
        }



        #endregion

        public class RecentItem
        {
            public string Header { get; set; }
            public object Content { get; set; }
            public string FilePath { get; set; }
            public bool hasFilePath { get; }
            public RecentItem(string _header, object _content)
            {
                Header = _header;
                Content = _content;
                hasFilePath = false;
            }

            public RecentItem(string _header, object _content, string _filePath)
            {
                Header = _header;
                Content = _content;
                FilePath = _filePath;
                hasFilePath = true;
            }
        }
    }
}

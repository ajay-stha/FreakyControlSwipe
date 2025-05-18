using FreakyControlsSample.DTO;
using System.Collections.ObjectModel;

namespace FreakyControlsSample
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        public ObservableCollection<CollectionDataItem> Items { get; set; } = new()
            {
                new CollectionDataItem { Name = "Item 1", IsVisible = false },
                new CollectionDataItem { Name = "Item 2", IsVisible = true },
                new CollectionDataItem { Name = "Item 3", IsVisible = true },
                new CollectionDataItem { Name = "Item 4", IsVisible = false },
                new CollectionDataItem { Name = "Item 5", IsVisible = true },
                new CollectionDataItem { Name = "Item 6", IsVisible = true },
            };

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}

using System.Windows;

namespace CybersecurityChatbot
{
    public class TaskDialog : Window
    {
        private System.Windows.Controls.TextBox titleTextBox;
        private System.Windows.Controls.Button okButton;
        private System.Windows.Controls.Button cancelButton;

        public string Title { get; private set; }

        public TaskDialog(string title, string prompt)
        {
            this.Title = title;
            this.Width = 400;
            this.Height = 150;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.ResizeMode = ResizeMode.NoResize;
            this.Title = title;

            var grid = new System.Windows.Controls.Grid();
            grid.Margin = new Thickness(10);
            grid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = System.Windows.GridLength.Auto });
            grid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = System.Windows.GridLength.Auto });
            grid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = System.Windows.GridLength.Auto });

            var promptLabel = new System.Windows.Controls.TextBlock
            {
                Text = prompt,
                Margin = new Thickness(0, 0, 0, 10),
                FontSize = 12
            };
            System.Windows.Controls.Grid.SetRow(promptLabel, 0);
            grid.Children.Add(promptLabel);

            titleTextBox = new System.Windows.Controls.TextBox
            {
                Margin = new Thickness(0, 0, 0, 10),
                FontSize = 12
            };
            System.Windows.Controls.Grid.SetRow(titleTextBox, 1);
            grid.Children.Add(titleTextBox);

            var buttonPanel = new System.Windows.Controls.StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right
            };

            okButton = new System.Windows.Controls.Button
            {
                Content = "OK",
                Width = 80,
                Height = 30,
                Margin = new Thickness(5),
                Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0))
            };
            okButton.Click += (s, e) => { Title = titleTextBox.Text; this.DialogResult = true; this.Close(); };

            cancelButton = new System.Windows.Controls.Button
            {
                Content = "Cancel",
                Width = 80,
                Height = 30,
                Margin = new Thickness(5)
            };
            cancelButton.Click += (s, e) => { this.DialogResult = false; this.Close(); };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            System.Windows.Controls.Grid.SetRow(buttonPanel, 2);
            grid.Children.Add(buttonPanel);

            this.Content = grid;
        }
    }
}

//Responsible for the tasks that the bot performs

namespace SecretsEncryptor
{
    using System.Windows;
    using System.Windows.Controls;
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UdoSecretsProvider _secretsProvider = new UdoSecretsProvider();
        private const string UdoConfigurationKey = "UdoConfiguration";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //AddEnvironmentInternal("PRODSOUTH");
            //AddEnvironmentInternal("PRODEAST");
            AddEnvironmentInternal("PREPRODALT");
            AddEnvironmentInternal("PREPROD");
            AddEnvironmentInternal("INT");
            AddEnvironmentInternal("QA");
            AddEnvironmentInternal("DEV");
            AddEnvironmentInternal("");
        }
        
        private void AddEnvironmentInternal(string environmentNameUppercase)
        {
            // insert new rows to grid
            var rowDefinitions = MainGrid.RowDefinitions;

            // text boxes row
            rowDefinitions.Insert(0, new RowDefinition
            {
                Height = new GridLength(100)
            });

            // textblock row
            rowDefinitions.Insert(0, new RowDefinition
            {
                Height = new GridLength(30)
            });

            var controls = MainGrid.Children;

            // create textblock
            var textBlock = new TextBlock
            {
                Text = environmentNameUppercase,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            var encryptedText = _secretsProvider.GetValueAsNotNullString(environmentNameUppercase + UdoConfigurationKey);

            // create unencrypted textbox
            var unencryptedTextBox = new TextBox
            {
                Text = UdoEncrypter.Decrypt(encryptedText),
                TextWrapping = TextWrapping.Wrap,
                Height = 70,
                VerticalAlignment = VerticalAlignment.Top
            };


            // create encrypted textbox
            var encryptedTextBox = new TextBox
            {
                Text = encryptedText,
                TextWrapping = TextWrapping.Wrap,
                Height = 70,
                IsReadOnly = true,
                VerticalAlignment = VerticalAlignment.Top
            };

            controls.Insert(0, encryptedTextBox);
            controls.Insert(0, unencryptedTextBox);
            controls.Insert(0, textBlock);

            // re-number rows so they are correct
            var controlsCount = controls.Count;
            var rowIndex = 0;
            var controlIndex = 0;

            while (controlIndex < controlsCount)
            {
                var control = controls[controlIndex];
                unencryptedTextBox = control as TextBox;

                if (unencryptedTextBox == null)
                {
                    textBlock = control as TextBlock;
                    if (textBlock == null)
                    {
                        // button
                        Grid.SetRow(control, rowIndex);
                        controlIndex++;
                    }
                    else
                    {
                        // textbox
                        Grid.SetColumnSpan(control, 2);
                        Grid.SetRow(control, rowIndex);
                        controlIndex++;
                        rowIndex++;
                    }
                }
                else
                {
                    Grid.SetColumn(unencryptedTextBox, 0);
                    Grid.SetRow(unencryptedTextBox, rowIndex);
                    controlIndex++;
                    encryptedTextBox = controls[controlIndex] as TextBox;
                    Grid.SetColumn(encryptedTextBox, 1);
                    Grid.SetRow(encryptedTextBox, rowIndex);
                    controlIndex++;
                    rowIndex++;
                }
            }
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            var controls = MainGrid.Children;
            var currentEnvironment = string.Empty;
            var encryptedText = string.Empty;

            foreach (var control in controls)
            {
                if (!(control is TextBlock textBlock))
                {
                    if (control is TextBox unencryptedTextBox)
                    {
                        if (unencryptedTextBox.IsReadOnly)
                        {
                            var encryptedTextBox = unencryptedTextBox;
                            encryptedTextBox.Text = encryptedText;
                        }
                        else
                        {
                            unencryptedTextBox.Text = unencryptedTextBox.Text.Trim();
                            encryptedText = UdoEncrypter.Encrypt(unencryptedTextBox.Text);
                            _secretsProvider.SetValue(currentEnvironment + UdoConfigurationKey, encryptedText);
                        }
                    }
                }
                else
                {
                    currentEnvironment = textBlock.Text;
                }
            }

            _secretsProvider.Save();
        }
    }
}

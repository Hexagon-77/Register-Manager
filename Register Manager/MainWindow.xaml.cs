using System.Xml;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Register_Manager
{
    public class TDCConfigDescription
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int RegisterAddress { get; set; }

        public List<TDCBitDescription> BitDescriptions { get; set; } = new List<TDCBitDescription>();
    }
    public class TDCBitDescription
    {
        public int BitPosition { get; set; }
        public int BitLength { get; set; }
        public string BitDescription { get; set; }

        public List<string> Values { get; set; } = new List<string>();
    }

    // Register configuration
    //List<TDCConfigDescription> configs = new List<TDCConfigDescription>();

    public class RegisterConfig
    {
        public Register PI_TPW = new Register();
        public Register PI_EN = new Register();
        public Register PI_OUT_MODE = new Register();
        public Register PI_UPD_MODE = new Register();
        public Register I2C_MODE = new Register();
        public Register I2C_ADR = new Register();
        public Register SPI_INPORT_CFG = new Register();
        public Register GP0_DIR = new Register();
        public Register GP0_SEL = new Register();
        public Register GP1_DIR = new Register();
        public Register GP1_SEL = new Register();
        public Register GP2_DIR = new Register();
        public Register GP2_SEL = new Register();
        public Register GP3_DIR = new Register();
        public Register GP3_SEL = new Register();
        public Register GP4_DIR = new Register();
        public Register GP4_SEL = new Register();
        public Register GP5_DIR = new Register();
        public Register GP5_SEL = new Register();
    }

    // Register class
    [XmlType(TypeName = "Register")]
    public class Register
    {
        [XmlAttribute("Value")]
        public string value = null;
    }

    public class ConfigFile
    {
        string Name;
        public string Path;

        public ConfigFile(string s)
        {
            Path = s;
            Name = System.IO.Path.GetFileNameWithoutExtension(s);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RegisterConfig currentConfig = new RegisterConfig();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Generate current configuration
            if (rad1Register1.IsChecked == true) currentConfig.PI_TPW.value = "1";
            if (rad2Register1.IsChecked == true) currentConfig.PI_TPW.value = "2";

            if (rad1Register2.IsChecked == true) currentConfig.PI_EN.value = "1";
            if (rad2Register2.IsChecked == true) currentConfig.PI_EN.value = "2";

            if (rad1Register3.IsChecked == true) currentConfig.PI_OUT_MODE.value = "1";
            if (rad2Register3.IsChecked == true) currentConfig.PI_OUT_MODE.value = "2";

            if (rad1Register4.IsChecked == true) currentConfig.PI_UPD_MODE.value = "1";
            if (rad2Register4.IsChecked == true) currentConfig.PI_UPD_MODE.value = "2";
            
            currentConfig.I2C_MODE.value = (string)((ComboBoxItem)cmb1Register5.SelectedItem).Tag;

            currentConfig.I2C_ADR.value = txt1Register6.Text;

            currentConfig.SPI_INPORT_CFG.value = (string)((ComboBoxItem)cmb1Register7.SelectedItem).Tag;
            currentConfig.GP0_DIR.value = (string)((ComboBoxItem)cmb1Register8.SelectedItem).Tag;
            currentConfig.GP0_SEL.value = (string)((ComboBoxItem)cmb1Register9.SelectedItem).Tag;
            currentConfig.GP1_DIR.value = (string)((ComboBoxItem)cmb1Register10.SelectedItem).Tag;
            currentConfig.GP1_SEL.value = (string)((ComboBoxItem)cmb1Register11.SelectedItem).Tag;
            currentConfig.GP2_DIR.value = (string)((ComboBoxItem)cmb1Register12.SelectedItem).Tag;
            currentConfig.GP2_SEL.value = (string)((ComboBoxItem)cmb1Register13.SelectedItem).Tag;
            currentConfig.GP3_DIR.value = (string)((ComboBoxItem)cmb1Register14.SelectedItem).Tag;
            currentConfig.GP3_SEL.value = (string)((ComboBoxItem)cmb1Register15.SelectedItem).Tag;
            currentConfig.GP4_DIR.value = (string)((ComboBoxItem)cmb1Register16.SelectedItem).Tag;
            currentConfig.GP4_SEL.value = (string)((ComboBoxItem)cmb1Register17.SelectedItem).Tag;
            currentConfig.GP5_DIR.value = (string)((ComboBoxItem)cmb1Register18.SelectedItem).Tag;

            if (rad1Register19.IsChecked == true) currentConfig.GP5_SEL.value = "00";
            if (rad3Register19.IsChecked == true) currentConfig.GP5_SEL.value = "10";

            // Create / check data directory
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Data\\"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Data\\");
            }

            // Select save location
            SaveFileDialog save = new SaveFileDialog();
            save.Title = "Register Manager - Save configuration";
            save.InitialDirectory = Directory.GetCurrentDirectory() + "Data\\";
            save.Filter = "Register Configuration | *.xml";
            save.DefaultExt = "xml";

            // Save to a file
            if (save.ShowDialog() == true) {
                XmlSerializer serializer = new XmlSerializer(typeof(RegisterConfig));
                StringWriter strWriter = new StringWriter();
                XmlWriterSettings xmlSettings = new XmlWriterSettings();
                xmlSettings.Indent = true;
                
                using (XmlWriter writer = XmlWriter.Create(strWriter, xmlSettings))
                {
                    serializer.Serialize(writer, currentConfig);
                    File.WriteAllText(save.FileName, strWriter.ToString());
                }
            }

            LoadData();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            // Select config file
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Register Manager - Open configuration";
            open.InitialDirectory = Directory.GetCurrentDirectory() + "Data\\";
            open.Filter = "Register Configuration | *.xml";
            open.DefaultExt = "xml";

            // Open a config
            if (open.ShowDialog() == true)
                OpenConfig(open.FileName);
        }

        void OpenConfig(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RegisterConfig));
            StringReader strReader = new StringReader(File.ReadAllText(path));

            using (XmlReader reader = XmlReader.Create(strReader))
            {
                currentConfig = (RegisterConfig)serializer.Deserialize(reader);

                RefreshConfig();
                btnExpand_Click(this, new RoutedEventArgs());
            }
        }

        void RefreshConfig()
        {
            rad1Register1.IsChecked = currentConfig.PI_TPW.value == "1";
            rad2Register1.IsChecked = currentConfig.PI_TPW.value == "2";

            rad1Register2.IsChecked = currentConfig.PI_EN.value == "1";
            rad2Register2.IsChecked = currentConfig.PI_EN.value == "2";

            rad1Register3.IsChecked = currentConfig.PI_OUT_MODE.value == "1";
            rad2Register3.IsChecked = currentConfig.PI_OUT_MODE.value == "2";

            rad1Register4.IsChecked = currentConfig.PI_UPD_MODE.value == "1";
            rad2Register4.IsChecked = currentConfig.PI_UPD_MODE.value == "2";

            cmb1Register5.SelectedItem = FindComboItemByTag(cmb1Register5, currentConfig.I2C_MODE.value);

            txt1Register6.Text = currentConfig.I2C_ADR.value;

            cmb1Register7.SelectedItem = FindComboItemByTag(cmb1Register7, currentConfig.SPI_INPORT_CFG.value);
            cmb1Register8.SelectedItem = FindComboItemByTag(cmb1Register8, currentConfig.GP0_DIR.value);
            cmb1Register9.SelectedItem = FindComboItemByTag(cmb1Register9, currentConfig.GP0_SEL.value);
            cmb1Register10.SelectedItem = FindComboItemByTag(cmb1Register10, currentConfig.GP1_DIR.value);
            cmb1Register11.SelectedItem = FindComboItemByTag(cmb1Register11, currentConfig.GP1_SEL.value);
            cmb1Register12.SelectedItem = FindComboItemByTag(cmb1Register12, currentConfig.GP2_DIR.value);
            cmb1Register13.SelectedItem = FindComboItemByTag(cmb1Register13, currentConfig.GP2_SEL.value);
            cmb1Register14.SelectedItem = FindComboItemByTag(cmb1Register14, currentConfig.GP3_DIR.value);
            cmb1Register15.SelectedItem = FindComboItemByTag(cmb1Register15, currentConfig.GP3_SEL.value);
            cmb1Register16.SelectedItem = FindComboItemByTag(cmb1Register16, currentConfig.GP4_DIR.value);
            cmb1Register17.SelectedItem = FindComboItemByTag(cmb1Register17, currentConfig.GP4_SEL.value);
            cmb1Register18.SelectedItem = FindComboItemByTag(cmb1Register18, currentConfig.GP5_DIR.value);

            rad1Register19.IsChecked = currentConfig.GP5_SEL.value == "00";
            rad3Register19.IsChecked = currentConfig.GP5_SEL.value == "10";
        }

        public static ComboBoxItem FindComboItemByTag(ComboBox comboBox, string tag)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if ((string)item.Tag == tag)
                    return item;
            }

            return null;
        }

        public static IEnumerable<T> FindVisualChilds<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChilds<T>(ithChild)) yield return childOfChild;
            }
        }

        private void btnExpand_Click(object sender, RoutedEventArgs e)
        {
            foreach (Expander exp in FindVisualChilds<Expander>(GetWindow(this)))
            {
                exp.IsExpanded = true;
            }
        }

        private void btnCollapse_Click(object sender, RoutedEventArgs e)
        {
            foreach (Expander exp in FindVisualChilds<Expander>(GetWindow(this)))
            {
                exp.IsExpanded = false;
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            foreach (RadioButton rad in FindVisualChilds<RadioButton>(GetWindow(this)))
            {
                rad.IsChecked = false;
            }

            foreach (TextBox txt in FindVisualChilds<TextBox>(GetWindow(this)))
            {
                txt.Text = string.Empty;
            }

            foreach (ComboBox cmb in FindVisualChilds<ComboBox>(GetWindow(this)))
            {
                cmb.SelectedItem = null;
            }

            txtSearch.Text = string.Empty;
            currentConfig = new RegisterConfig();
        }

        private void cmbConfigs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                OpenConfig(((ConfigFile)cmbConfigs.SelectedItem).Path);
            }
            catch (System.Exception) { }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(Directory.GetCurrentDirectory() + "\\Data\\")) LoadData();
        }

        void LoadData()
        {
            cmbConfigs.Items.Clear();

            foreach (string f in Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Data\\"))
            {
                if (Path.GetExtension(f) == ".xml")
                    cmbConfigs.Items.Add(new ConfigFile(f));
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach (Expander exp in FindVisualChilds<Expander>(GetWindow(this)))
            {
                exp.Visibility = exp.Header.ToString().ToLower().Contains(txtSearch.Text.ToLower()) ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}

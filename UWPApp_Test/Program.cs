using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPApp_Test
{
    public sealed partial class MainPage : Page
    {
        RateData _data;
        List<string> valutes;
        DispatcherTimer timer;
        ObservableCollection<Button> buttons;
        public MainPage()
        {
            InitializeComponent();
            
            buttons = new ObservableCollection<Button>();


            try
            {
                _data = new RateData(@"https://www.cbr-xml-daily.ru/daily_json.js");
            }
            catch (Exception err)
            {
                return;
            }

            valutes = _data.Rates.Valute.Keys.ToList();
            foreach (var v in valutes)
            {
                Button button = new Button();
                button.Content = v;
                buttons.Add(button);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LValute.ItemsSource = valutes;
            RValute.ItemsSource = valutes;
            LValute.SelectedIndex = 0;
            RValute.SelectedIndex = 1;
            LValue.Text = "";
            RValue.Text = "";
            SetValuteNames();
        }

        private void SetValuteNames()
        {
            if (LValute.SelectedItem == null || RValute.SelectedItem == null) return;
            LValuteName.Text = _data.Rates.Valute[LValute.SelectedItem.ToString()].Name;
            RValuteName.Text = _data.Rates.Valute[RValute.SelectedItem.ToString()].Name;
        }

        private void ConvertValue()
        {
            double lv, rv;
            if (LValue.Text != String.Empty && !double.TryParse(LValue.Text, out lv)) return;
            if (RValue.Text != String.Empty && !double.TryParse(RValue.Text, out rv)) return;
            if (LValue.Text == String.Empty && RValue.Text == String.Empty)
            {
                return;
            }
            double v1 = _data.Rates.Valute[RValute.SelectedItem.ToString()].Value * _data.Rates.Valute[LValute.SelectedItem.ToString()].Nominal;
            double v2 = _data.Rates.Valute[LValute.SelectedItem.ToString()].Value * _data.Rates.Valute[RValute.SelectedItem.ToString()].Nominal;
            double cur = LValue.Text == String.Empty ? double.Parse(RValue.Text) : double.Parse(LValue.Text);
            if (LValue.Text == String.Empty)
            {
                LValue.Text = (cur * v1 / v2).ToString("F2");
            }
            if (RValue.Text == String.Empty)
            {
                RValue.Text = (cur * v2 / v1).ToString("F2");
            }
        }

       /* private void LValue_KeyUp(object sender, KeyEventArgs e)
        {
            RValue.Text = String.Empty;
            ConvertValue();
        }

        private void RValue_KeyUp(object sender, KeyEventArgs e)
        {
            LValue.Text = String.Empty;
            ConvertValue();
        }

        private void LValute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RValue.Text = String.Empty;
            ConvertValue();
            SetValuteNames();
        }

        private void RValute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LValue.Text = String.Empty;
            ConvertValue();
            SetValuteNames();
        }*/
    }
}
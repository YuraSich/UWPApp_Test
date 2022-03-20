using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace UWPApp_Test
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly RateData _data;
        private readonly List<string> _valutes;
        private readonly ObservableCollection<Button> _buttons;
        PickingPage pickingPage;
        public MainPage()
        {
            InitializeComponent();

            try
            {
                _data = new RateData(@"https://www.cbr-xml-daily.ru/daily_json.js");
            }
            catch (Exception)
            {
                return;
            }

            _valutes = _data.Rates.Valute.Keys.ToList();

            foreach (var v in _valutes)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.SetValue(TagProperty, v);
                LValute.Items.Add(item);
            }

        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            LValute.ItemsSource = _valutes;
            RValute.ItemsSource = _valutes;
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
            if (LValue.Text != String.Empty && !double.TryParse(LValue.Text, out _)) return;
            if (RValue.Text != String.Empty && !double.TryParse(RValue.Text, out _)) return;
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

        private void LValue_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            RValue.Text = String.Empty;
            ConvertValue();
        }

        private void RValue_KeyUp(object sender, KeyRoutedEventArgs e)
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
        }
    }
}

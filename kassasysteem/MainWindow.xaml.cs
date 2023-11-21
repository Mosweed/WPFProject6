using kassasysteem.Helper;
using kassasysteem.Models;
using Microsoft.Win32;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ZXing.Common;
using ZXing;
using ZXing.QrCode;
using System.IO;
using System.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;
using Mollie.Api.Client;

using IronBarCode;
using Mollie.Api.Client.Abstract;
using Mollie.Api.Models.Payment.Request;
using Mollie.Api.Models.Payment.Response;
using Mollie.Api.Models;
using Mollie.Api.Models.Order;
using Mollie.Api.Models.Payment;
using System.Threading;

namespace kassasysteem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , INotifyPropertyChanged
    {


        private ObservableCollection<Models.Product> _ocProducts = new ObservableCollection<Models.Product>();
        public ObservableCollection<Models.Product> OcProducts
        {
            get { return _ocProducts; }
            set { _ocProducts = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Models.ShopCart> shopCarts = new ObservableCollection<Models.ShopCart>();
        public ObservableCollection<Models.ShopCart> ShopCartsitems
        {
            get { return shopCarts; }
            set { shopCarts = value; OnPropertyChanged(); }
        }

        string senderes;

        OpenFileDialog OpenFileDialog = new OpenFileDialog();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private Employee _employee = new Employee();
        public Employee Employee
        {
            get { return _employee; }
            set { _employee = value; OnPropertyChanged(); }
        }

        int drawer_id;
        int barcode;
        int quantity;
        decimal total;
        string paid;
        int cumstomer_number;





        public MainWindow(Access access, int drawer_ID)
        {
            InitializeComponent();
            drawer_id = drawer_ID;
            Employee = access._employee;
            tbEmployee.Text = Employee.employee_number.ToString();
            TBDRAWER_ID.Text = drawer_id.ToString();
            Loadcartitems();


        }


        private async void Loadcartitems()
        {
            ShopCartsitems.Clear();
            List<ShopCart> shopcart = new List<ShopCart>();
            shopcart = await ApiHelper.GetProducts(drawer_id);
            if (shopcart != null)
            {
                total = 0;
                foreach (var item in shopcart)
                {
                    total = total + item.shopcartitem_price;
                    ShopCartsitems.Add(item);

                }
                lvInfo.DataContext = ShopCartsitems;
                TBtotal.Text = total.ToString("0.00");
            }
            else
            {
                MessageBox.Show("Product niet gevonden", "", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }


        private void BTScan_Click(object sender, RoutedEventArgs e)
        {

            //BarcodeReaderGeneric reader = new BarcodeReaderGeneric();
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "All Files (*.*)|*.*" +
        "|All Pictures (*.emf;*.wmf;*.jpg;*.jpeg;*.jfif;*.jpe;*.png;*.bmp;*.dib;*.rle;*.gif;*.emz;*.wmz;*.tif;*.tiff;*.svg;*.ico)" +
            "|*.emf;*.wmf;*.jpg;*.jpeg;*.jfif;*.jpe;*.png;*.bmp;*.dib;*.rle;*.gif;*.emz;*.wmz;*.tif;*.tiff;*.svg;*.ico" +
        "|JPEG File Interchange Format (*.jpg;*.jpeg;*.jfif;*.jpe)|*.jpg;*.jpeg;*.jfif;*.jpe" +
        "|Portable Network Graphics (*.png)|*.png" +
        "|Bitmap Image File (*.bmp;*.dib;*.rle)|*.bmp;*.dib;*.rle" +
        "|Scalable Vector Graphics (*.svg)|*.svg" +
        "|Icon (*.ico)|*.ico"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                imagBarcode.Source = new BitmapImage(new Uri(openFileDialog.FileName));

                BarcodeResults result = BarcodeReader.Read(openFileDialog.FileName);
                foreach (var Result in result)
                {
                    if (Result.Text.StartsWith("24"))
                    {
                        cumstomer_number = Convert.ToInt32(Result.Text);
                    }
                    else
                    {
                        TBBarcode.Text = Result.Text;
                        if (!cumstomer_number.ToString().StartsWith("24"))
                        {
                            cumstomer_number = 1;
                        }

                        
                    }
                }

            }
             

        }

        private async void BTAdd_Click(object sender, RoutedEventArgs e)
        {
            if (quantity != 0)
            {
                bool newintem = await ApiHelper.Postitem(drawer_id, barcode, quantity);
                if (newintem)
                {
                    Loadcartitems();
                    TBBarcode.Text = "";
                    TBAantal.Text = "";
                    quantity = 0;
                    barcode = 0;
                    imagBarcode.Source = null;
                }
                else
                {
                    MessageBox.Show("Product niet gevonden", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    TBBarcode.Text = "";
                    TBAantal.Text = "";
                }
            }


        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {

            OcProducts.Clear();
            TBBarcode.Text = "";
            imagBarcode.Source = null;
        }



        private void TBBarcode_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (TBBarcode.Text.Length <= 8)
            {
                if (TBBarcode.Text != null)
                {
                    try
                    {
                        barcode = int.Parse(TBBarcode.Text);

                    }
                    catch
                    {
                        TBBarcode.Text = null;
                    }
                }


            }
            else
            {
                MessageBox.Show("De barcode mag niet groter zijn dan 8 nummers", "", MessageBoxButton.OK, MessageBoxImage.Error);
                TBBarcode.Text = null;
            }
        }

        private void TBAantal_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (TBAantal.Text == null)
                {

                }
                else
                {
                    quantity = int.Parse(TBAantal.Text);
                }


            }
            catch
            {

                TBAantal.Text = null;

            }
        }

        private async void Pay_Click(object sender, RoutedEventArgs e)
        {

            if (ShopCartsitems.Count != 0)
            {
                bool pay = await ApiHelper.PostOrder(drawer_id, total , cumstomer_number);
                if (pay)
                {
                    IPaymentClient paymentClient = new PaymentClient("test_8GDEQEHkUTnmdmTVjvhEtMbF85CTqc");
                    PaymentRequest paymentRequest = new PaymentRequest()
                    {
                        Amount = new Amount(Currency.EUR, total),
                        Description = "Test payment of the example project",
                        RedirectUrl = "http://google.com" , 
                       

                    };


                    PaymentResponse paymentResponse = await paymentClient.CreatePaymentAsync(paymentRequest);

                    PaymentResponseLinks links = paymentResponse.Links;
                    var redirectUrl = links.Checkout;
                    string ur = redirectUrl.Href;
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = ur,
                        UseShellExecute = true
                    });

                    paymentResponse = await paymentClient.GetPaymentAsync(paymentResponse.Id);

                    while (paymentResponse.Status != PaymentStatus.Paid)
                    {
                        paymentResponse = await paymentClient.GetPaymentAsync(paymentResponse.Id);
                        MessageBox.Show("Graag uw bestelling betalen");
                        Thread.Sleep(1000);
                       
                    }
                   

                    MessageBox.Show("Betaling gelukt", "", MessageBoxButton.OK, MessageBoxImage.Information);

                    TBtotal.Text = "0.00";
                    PrintPdf();
                    total = 0;
                    Loadcartitems();
                }
                else
                {
                    MessageBox.Show("Product niet gevonden", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    Loadcartitems();
                    total = 0;
                }
               
            }
        }

        private void Toevoeg_Click(object sender, RoutedEventArgs e)
        {

            Button button = (Button)sender;
            int value = int.Parse(button.Content.ToString());

            if (senderes == "TBBarcode")
            {
                TBBarcode.Focus();
                TBBarcode.Text += value.ToString();
            }
            else if (senderes == "TBAantal")
            {
                TBAantal.Focus();
                TBAantal.Text += value.ToString();
            }

        }


        private void TBAantal_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            senderes = "TBAantal";
        }

        private void TBBarcode_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            senderes = "TBBarcode";
        }

        private void PrintPdf()
        {
            

            var pageWidth = 800;
            var pageHeight = 400 + (ShopCartsitems.Count * 25);
            var document = new PdfSharp.Pdf.PdfDocument();
            var page = document.AddPage();
            page.Width = pageWidth;
            page.Height = pageHeight;
            var gfx = XGraphics.FromPdfPage(page);

            
            XFont font = new XFont("Arial", 10 );


            gfx.DrawString("Kassabon", font, XBrushes.Black, new XRect(20, 20, pageWidth, pageHeight), XStringFormats.TopLeft);
            gfx.DrawString("Raj Hogewoning", font, XBrushes.Black, new XRect(20, 100, pageWidth, pageHeight), XStringFormats.TopLeft);
            gfx.DrawString("Tuinstraat 167", font, XBrushes.Black, new XRect(20, 120, pageWidth, pageHeight), XStringFormats.TopLeft);
            gfx.DrawString("2587 WD Nuenen", font, XBrushes.Black, new XRect(20, 140, pageWidth, pageHeight), XStringFormats.TopLeft);
            gfx.DrawString("06-33024999", font, XBrushes.Black, new XRect(20, 160, pageWidth, pageHeight), XStringFormats.TopLeft);

            gfx.DrawString("Verkoop Prijs", font, XBrushes.Black, new XRect(20, 200, pageWidth, pageHeight), XStringFormats.TopLeft);
            gfx.DrawString("Barcode", font, XBrushes.Black, new XRect(150, 200, pageWidth, pageHeight), XStringFormats.TopLeft);
            gfx.DrawString("Hoeveelheid", font, XBrushes.Black, new XRect(270, 200, pageWidth, pageHeight), XStringFormats.TopLeft);
            gfx.DrawString("Naam", font, XBrushes.Black, new XRect(420, 200, pageWidth, pageHeight), XStringFormats.TopLeft);
            gfx.DrawString("Prijs incl.btw", font, XBrushes.Black, new XRect(720, 200, pageWidth, pageHeight), XStringFormats.TopLeft);

            double start = 230.0;
            foreach (var item in ShopCartsitems)
            {
                start += 20.0;
                gfx.DrawString(item.price.ToString("€ 0.00"), font, XBrushes.Black, new XRect(20, start, pageWidth, pageHeight), XStringFormats.TopLeft);
                gfx.DrawString(item.barcode.ToString(), font, XBrushes.Black, new XRect(150, start, pageWidth, pageHeight), XStringFormats.TopLeft);
                gfx.DrawString(item.quantity.ToString(), font, XBrushes.Black, new XRect(270, start, pageWidth, pageHeight), XStringFormats.TopLeft);
                gfx.DrawString(item.name, font, XBrushes.Black, new XRect(420, start, pageWidth, pageHeight), XStringFormats.TopLeft);
                gfx.DrawString(item.shopcartitem_price.ToString("€ 0.00"), font, XBrushes.Black, new XRect(720, start, pageWidth, pageHeight), XStringFormats.TopLeft);
            }

            gfx.DrawString("Totaal:", font, XBrushes.Black, new XRect(20, 100, pageWidth, pageHeight), XStringFormats.CenterLeft);
            gfx.DrawString(total.ToString( "€ 0.00"), font, XBrushes.Black, new XRect(100, 100, pageWidth, pageHeight), XStringFormats.CenterLeft);
            gfx.DrawString(DateTime.Now.ToString(), font, XBrushes.Black, new XRect(20, 120, pageWidth, pageHeight), XStringFormats.CenterLeft);
            gfx.DrawString("Bedankt voor uw bezoek", font, XBrushes.Black, new XRect(20, 140, pageWidth, pageHeight), XStringFormats.CenterLeft);
            gfx.DrawString( "U bent geholpen door" + " " + Employee.employee_number.ToString(), font, XBrushes.Black, new XRect(20, 160, pageWidth, pageHeight), XStringFormats.CenterLeft);

            string path = @"Assets/logo.png";
            var image = XImage.FromFile(path);
        
            gfx.DrawImage(image, 400 - 125 , 0 , 250 , 100 );
            var filename = "Bonnetje.pdf";
            var pdfFilename = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), filename);
            document.Save(pdfFilename);
            //Process.Start(pdfFilename);

        }

        private async void lvInfo_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBoxResult message = MessageBox.Show("Wilt u dit product verwijderen?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (message == MessageBoxResult.Yes)
            {
                var item = (ShopCart)lvInfo.SelectedItem;
                if (item != null)
                {
                    bool delete = await ApiHelper.DeleteItem(item.Id);
                    if (delete)
                    {
                        MessageBox.Show("Product verwijderd", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        total -= item.shopcartitem_price;
                        TBtotal.Text = total.ToString("0.00");
                        Loadcartitems();
                    }
                    else
                    {
                        MessageBox.Show("Product niet gevonden", "", MessageBoxButton.OK, MessageBoxImage.Error);
                        Loadcartitems();
                    }
                }
            }
            
        }
    }
}
    


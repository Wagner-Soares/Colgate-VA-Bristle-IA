using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bristle.Views
{
    /// <summary>
    /// Lógica interna para ImageView.xaml
    /// </summary>
    public partial class ImageView : Window
    {
        private double w = 0;
        private double h = 0;
        private System.Drawing.Bitmap image;
        private System.Drawing.Bitmap imageWithBoundingBox;
        private int imageCount_;
        public ImageView(int imageCount)
        {
            InitializeComponent();

            frameHolder.Stretch = Stretch.Fill;
            imageCount_ = imageCount;
            this.Title = "Image: " + imageCount;

            imageUnionWithBoundingBox();
        }

        private void imageUnionWithBoundingBox()
        {
            string pathB = Directory.GetCurrentDirectory() + "\\img\\auxBoundingBox" + imageCount_;

            if (File.Exists(pathB))
            {
                frameHolder.Source = BitmapFromUri(new Uri(pathB));
                boundingBoxButton.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                string pathI = Directory.GetCurrentDirectory() + "\\img\\auxImage" + imageCount_;
                frameHolder.Source = BitmapFromUri(new Uri(pathI));
                boundingBoxButton.Background = System.Windows.Media.Brushes.Red;
            }
        }

        private static BitmapImage BitmapFromUri(Uri source)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = source;
                bitmap.CacheOption = BitmapCacheOption.OnLoad; //Default;// OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                MessageBox.Show("File not found");
                return null;
            }
        }



        private void sizeChanged(object sender, SizeChangedEventArgs e)
        {
            w = this.ActualWidth;  // Window gridFrameHolder.Width;
            h = this.ActualHeight;

            frameHolder.Width = w;  // Window gridFrameHolder.Width;
            frameHolder.Height = h;
            frameHolder.Stretch = Stretch.Fill;
            frameHolder.UpdateLayout();
        }

        private void boundingBox_Click(object sender, RoutedEventArgs e)
        {
            string pathB = Directory.GetCurrentDirectory() + "\\img\\auxBoundingBox" + imageCount_;

            if (File.Exists(pathB))
            {
                if (boundingBoxButton.Background == System.Windows.Media.Brushes.Red)
                {
                    boundingBoxButton.Background = System.Windows.Media.Brushes.Green;
                    boundingBoxButton.Opacity = 0.65;
                    //showImage(imageWithBoundingBox);                 
                    frameHolder.Source = BitmapFromUri(new Uri(pathB));
                }
                else
                {
                    boundingBoxButton.Background = System.Windows.Media.Brushes.Red;
                    boundingBoxButton.Opacity = 0.65;
                    //showImage(image);
                    string pathI = Directory.GetCurrentDirectory() + "\\img\\auxImage" + imageCount_;
                    frameHolder.Source = BitmapFromUri(new Uri(pathI));
                }
            }
            else
            {
                boundingBoxButton.Background = System.Windows.Media.Brushes.Red;
                boundingBoxButton.Opacity = 0.65;
                //showImage(image);
                string path = Directory.GetCurrentDirectory() + "\\img\\auxImage" + imageCount_;
                frameHolder.Source = BitmapFromUri(new Uri(path));
            }
        }
    }
}

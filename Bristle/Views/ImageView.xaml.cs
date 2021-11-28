using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bristle.Views
{
    /// <summary>
    /// Lógica interna para ImageView.xaml
    /// </summary>
    public partial class ImageView : Window
    {
        private double w = 0;
        private double h = 0;
        private readonly int imageCount_;
        public ImageView(int imageCount)
        {
            InitializeComponent();

            frameHolder.Stretch = Stretch.Fill;
            imageCount_ = imageCount;
            this.Title = "Image: " + imageCount;

            ImageUnionWithBoundingBox();
        }

        private void ImageUnionWithBoundingBox()
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
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
            catch
            {
                MessageBox.Show("File not found");
                return null;
            }
        }



        private new void SizeChanged(object sender, SizeChangedEventArgs e)
        {
            w = this.ActualWidth;
            h = this.ActualHeight;

            frameHolder.Width = w;
            frameHolder.Height = h;
            frameHolder.Stretch = Stretch.Fill;
            frameHolder.UpdateLayout();
        }

        private void BoundingBox_Click(object sender, RoutedEventArgs e)
        {
            string pathB = Directory.GetCurrentDirectory() + "\\img\\auxBoundingBox" + imageCount_;

            if (File.Exists(pathB))
            {
                if (boundingBoxButton.Background == System.Windows.Media.Brushes.Red)
                {
                    boundingBoxButton.Background = System.Windows.Media.Brushes.Green;
                    boundingBoxButton.Opacity = 0.65;
                    
                    frameHolder.Source = BitmapFromUri(new Uri(pathB));
                }
                else
                {
                    boundingBoxButton.Background = System.Windows.Media.Brushes.Red;
                    boundingBoxButton.Opacity = 0.65;

                    string pathI = Directory.GetCurrentDirectory() + "\\img\\auxImage" + imageCount_;
                    frameHolder.Source = BitmapFromUri(new Uri(pathI));
                }
            }
            else
            {
                boundingBoxButton.Background = System.Windows.Media.Brushes.Red;
                boundingBoxButton.Opacity = 0.65;

                string path = Directory.GetCurrentDirectory() + "\\img\\auxImage" + imageCount_;
                frameHolder.Source = BitmapFromUri(new Uri(path));
            }
        }
    }
}

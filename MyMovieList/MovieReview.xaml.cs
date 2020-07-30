using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyMovieList
{
    /// <summary>
    /// Interaction logic for MovieReview.xaml
    /// </summary>
    public partial class MovieReview : Window
    {
        private int index;
        private double rating;
        MainWindow mw;
        private bool watched = false;

        public MovieReview(int i, string r, double rate, string title, MainWindow w)
        {
            InitializeComponent();

            if (rate == 0)
                watched = true;

            Review_tb.Text = r;
            Title_Label.Content = title;
            RatingReview.Text = rate.ToString();
            mw = w;

            index = i;
            rating = rate;

            if (RatingReview.Text == "")
            {
                Add_Review.IsEnabled = false;
            }

        }
        //sends entered information to TransferWatched() function in Main
        private void Add_Review_Click(object sender, RoutedEventArgs e)
        {
            bool success = false ;
            string review = Review_tb.Text;
            try
            {
                rating = Double.Parse(RatingReview.Text);
                if (rating > 0 && rating <= 10)
                {
                    Add_Review.IsEnabled = true;
                    success = true;
                    RatingReview.BorderBrush = SystemColors.ControlDarkBrush;
                }
                else
                {
                    RatingReview.BorderBrush = Brushes.Red;
                    Add_Review.IsEnabled = false;
                }

            }
            catch
            {
                RatingReview.BorderBrush = Brushes.Red;
            }
            if (watched == false)
                mw.AddReview(review, rating, index);
            else
                mw.TransferWatched(review, rating, index);
            if (success)
            {
                this.Close();
            }
        }
        //enables addreview button based on if text is filled in
        private void RatingReview_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                rating = Double.Parse(RatingReview.Text);
                if(rating>0 && rating <= 10)
                {
                    Add_Review.IsEnabled = true;
                    RatingReview.BorderBrush= SystemColors.ControlDarkBrush;
                }
                else
                {
                    RatingReview.BorderBrush = Brushes.Red;
                    Add_Review.IsEnabled = false;
                }

            }
            catch
            {
                RatingReview.BorderBrush = Brushes.Red;
            }
        }
    }
}

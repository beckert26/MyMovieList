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
    /// Interaction logic for MovieComment.xaml
    /// </summary>
    public partial class MovieComment : Window
    {
        private int commentWindow=1;
        private int index;
        MainWindow mw;
        public MovieComment(int i,int cWin, string comment, string title, MainWindow t)
        {
            InitializeComponent();
            index = i;
            commentWindow = cWin;

            Comment_tb.Text = comment;

            Title_Label.Content = title;

            mw = t;
        }
        //sends comment information to AddComment in main
        private void Add_Comment_Click(object sender, RoutedEventArgs e)
        {
            string comment = Comment_tb.Text;

            if (commentWindow == 1){
                mw.AddComment1(comment, index);
            }
            if (commentWindow == 2)
            {
                mw.AddComment2(comment, index);
            }
            this.Close();

        }
    }
}

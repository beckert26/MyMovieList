using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Xml;

namespace MyMovieList
{
    /// <summary>
    /// Interaction logic for OMDB.xaml
    /// </summary>
    /// 
    //api key=3053d437
    public partial class OMDB : Window
    {
        //prints information of reader onto page
        public OMDB(XmlTextReader reader)
        {
            InitializeComponent();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        while (reader.MoveToNextAttribute()) // Read the attributes.
                            if (reader.Name == "poster")
                            {
                                var image = new Image();
                                var fullFilePath = reader.Value;

                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
                                bitmap.EndInit();

                                poster.Source = bitmap;

                            }
                            else if (reader.Name == "response" || reader.Name == "movie" || reader.Name == "type" || reader.Name=="imdbID" || reader.Name=="imdbVotes")
                                break;
                            else if (reader.Name == "title")
                            {
                                Title_Label.Content = reader.Value;
                            }
                            else
                            {
                                info_tb.Text += (char.ToUpper(reader.Name[0]) + reader.Name.Substring(1) + ": " + reader.Value  + "\n");
                            }
                        break;
                }
            }
        }
    }
}

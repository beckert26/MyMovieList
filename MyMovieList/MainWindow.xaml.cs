
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace MyMovieList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        List<Movie> wantMovieList = new List<Movie>();
        List<Movie> watchedMovieList = new List<Movie>();

        List<int> SkipList1 = new List<int>();
        List<int> SkipList2 = new List<int>();

        //save all movies onto this list
        List<Movie> movieList = new List<Movie>();

        int commentWindow = 1;


        bool modified = false;


        String currentFilePath = "";


        public MainWindow()
        {
            InitializeComponent();



            unsetModifiedState();
            DisableButtons1();
            DisableButtons2();

        }

        //File new, open, saving, and exiting

        //checks if current page is saved, calls function to open new file
        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (handleSaveRequest())
            {
                startNew();
            }
        }
        //opens blank page
        private void startNew()
        {
            currentFilePath = "";
            watchedMovieList.Clear();
            wantMovieList.Clear();
            Title_Textbox.Text = "";
            Title_Textbox2.Text = "";
            GenreComboBox.Text = "Genre";
            GenreComboBox2.Text = "Genre";
            Search1.Text = "";
            Search2.Text = "";
            Favorite_cb.IsChecked = false; ;
            SortComboBox.Text = "Sort By";
            SortComboBox2.Text = "Sort By";
            Rating.Text = "";
            unsetModifiedState();
            printWantMovieList();
            printWatchedMovieList();
        }
        //Checks if current page is saved, if it has opens file dialog
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (handleSaveRequest())
            {

                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.Filter = "Movie List (.mymovie)|*.mymovie"; // Filter files by extension

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    loadList(openFileDialog.FileName);
                }
            }
        }
        //loads Binaryformat into movie list that is separated into want/watched lists based on type
        private void loadList(String filePath)
        {
            var fStream = new FileStream(filePath, FileMode.Open);
            var binFormatter = new BinaryFormatter();
            movieList = (List<Movie>)binFormatter.Deserialize(fStream);
            fStream.Close();
            currentFilePath = filePath;

            watchedMovieList.Clear();
            wantMovieList.Clear();
            Title_Textbox.Text = "";
            Title_Textbox2.Text = "";
            GenreComboBox.Text = "Genre";
            GenreComboBox2.Text = "Genre";
            Search1.Text = "";
            Search2.Text = "";
            Favorite_cb.IsChecked = false; ;
            SortComboBox.Text = "Sort By";
            SortComboBox2.Text = "Sort By";
            Rating.Text = "";

            foreach (Movie movie in movieList)
            {
                if (movie.GetType() == typeof(WatchedMovie))
                {
                    watchedMovieList.Add(movie);
                    SkipList2.Add(0);
                }
                else
                {
                    wantMovieList.Add(movie);
                    SkipList1.Add(0);
                }
            }

            printWantMovieList();
            printWatchedMovieList();
        }
        //save click
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            saveList(currentFilePath);
            unsetModifiedState();
        }
        //saves two list into movieList
        private void saveList(String filePath)
        {
            var fStream = new FileStream(filePath, FileMode.Create);
            var binFormatter = new BinaryFormatter();

            movieList.Clear();

            int wantMovieLength = wantMovieList.Count;
            foreach (WantMovie movie in wantMovieList)
            {
                movieList.Add(movie);
            }
            foreach (WatchedMovie movie in watchedMovieList)
            {
                movieList.Add(movie);
            }

            binFormatter.Serialize(fStream, movieList);
            fStream.Close();

            currentFilePath = filePath;
        }
        //starts save as function
        private void Save_As_Click(object sender, RoutedEventArgs e)
        {
            startSaveAs();
        }
        //opens savefile dialog to saveList
        private void startSaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (currentFilePath != "")
            {
                saveFileDialog.FileName = System.IO.Path.GetFileName(currentFilePath);
                saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(currentFilePath);
            }

            saveFileDialog.DefaultExt = ".mymovie"; // Default file extension
            saveFileDialog.Filter = "Movie List (.mymovie)|*.mymovie"; // Filter files by extension
            // Show save file dialog box
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                saveList(saveFileDialog.FileName);
            }
            currentFilePath = saveFileDialog.FileName;
            unsetModifiedState();
        }

        //handles whether or not the current movielist has been saved
        private Boolean handleSaveRequest()
        {
            if (!modified)
                return true;

            string messageBoxText = "Your movie list needs to be saved.\nDo you want to save?";
            string caption = "MyMovieList";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;
            // Display message box
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show(messageBoxText, caption, button, icon);
            // Process message box results
            switch (messageBoxResult)
            {
                case MessageBoxResult.Yes: // Save document and exit
                    if (Save_MI.IsEnabled)
                        saveList(currentFilePath);
                    else
                        startSaveAs();
                    return true;

                case MessageBoxResult.No: // Exit without saving
                    return true;

                case MessageBoxResult.Cancel: // Don't exit
                    return false;
            }

            return false;

        }
        //checks if saved, then exits
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (handleSaveRequest())
            {
                Close();
            }
        }
        //checks if saved then closes window
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!handleSaveRequest())
            {
                e.Cancel = true;
            }
        }
        //sets the state to unmodified
        private void unsetModifiedState()
        {
            modified = false;
            Save_MI.IsEnabled = false;
        }
        //sets the state to modified
        private void setModifiedState()
        {
            modified = true;
            if(currentFilePath!="")
                Save_MI.IsEnabled = true;
        }
        //adds movie to want movie list
        private void Add_Movie_Click(object sender, RoutedEventArgs e)
        {
            WantMovie movie = new WantMovie();
            bool worked = true;
            try
            {
                if (Title_Textbox.Text == "")
                {
                    System.FormatException fEx = new System.FormatException();
                    throw fEx;
                }
                movie.Title = Title_Textbox.Text;
            }
            catch (FormatException)
            {
                Title_Textbox.BorderBrush = Brushes.Red;
                worked = false;
            }
            ComboBoxItem cbi = (ComboBoxItem)GenreComboBox.SelectedItem;
            movie.Genre = cbi.Content.ToString();
            if (worked == true)
            {
                wantMovieList.Add(movie);
                Title_Textbox.BorderBrush = SystemColors.ControlDarkBrush;
                Title_Textbox.Text = "";
                GenreComboBox.Text = "Genre";
                setModifiedState();

                int skip = 0;
                SkipList1.Clear();
                MoviesListView.Items.Clear();
                foreach (WantMovie movie1 in wantMovieList)
                {
                    try
                    {
                        if (movie.Title.ToUpper().Contains(Search1.Text.ToUpper()) || Search1.Text == "")
                        {
                            MoviesListView.Items.Add(new WantMovie { Title = movie1.Title, Genre = movie1.Genre, DateAdded = movie1.DateAdded });
                            SkipList1.Add(skip);
                        }
                        else
                        {
                            skip++;
                        }

                    }
                    catch
                    {

                    }
                }
                CheckSort();
            }
        }
        //edits movie on want movie page
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            int index = MoviesListView.SelectedIndex;
            index = trySkipList1(index);

            bool worked = true;
            try
            {
                if (Title_Textbox.Text == "")
                {
                    System.FormatException fEx = new System.FormatException();
                    throw fEx;
                }
                wantMovieList[index].Title = Title_Textbox.Text;
            }
            catch (FormatException)
            {
                Title_Textbox.BorderBrush = Brushes.Red;
                worked = false;
            }
            ComboBoxItem cbi = (ComboBoxItem)GenreComboBox.SelectedItem;
            wantMovieList[index].Genre = cbi.Content.ToString();
            if (worked == true)
            {
                Title_Textbox.BorderBrush = SystemColors.ControlDarkBrush;
                Title_Textbox.Text = "";
                GenreComboBox.Text = "Genre";
                printWantMovieList();

                setModifiedState();
                DisableButtons1();

            }

        }
        //prints wantMovie list onto ListView
        private void printWantMovieList()
        {
            MoviesListView.Items.Clear();

            SkipList1.Clear();
            int skip=0;
            foreach (WantMovie movie in wantMovieList)
            {
                try
                {
                    if (movie.Title.ToUpper().Contains(Search1.Text.ToUpper()) || Search1.Text == "")
                    {
                        MoviesListView.Items.Add(new WantMovie { Title = movie.Title, Genre = movie.Genre, DateAdded = movie.DateAdded });
                        SkipList1.Add(skip);
                    }
                    else
                    {
                        skip++;
                    }
                }
                catch
                {

                }
            }
        }
        //prints watched movielist onto listview2
        private void printWatchedMovieList()
        {
            MoviesListView2.Items.Clear();
            int skip = 0;
            SkipList2.Clear();
            foreach (WatchedMovie movie in watchedMovieList)
            {
                try
                {
                    if (movie.Title.ToUpper().Contains(Search2.Text.ToUpper()) || Search2.Text == "")
                    {
                        if (Favorite_cb.IsChecked == false)
                            MoviesListView2.Items.Add(new WatchedMovie { Title = movie.Title, Genre = movie.Genre, Rating = movie.Rating, DateAdded = movie.DateAdded, Favorite = movie.Favorite });
                        else if (Favorite_cb.IsChecked == true)
                        {
                            if (movie.Favorite == "*")
                            {
                                MoviesListView2.Items.Add(new WatchedMovie { Title = movie.Title, Genre = movie.Genre, Rating = movie.Rating, DateAdded = movie.DateAdded, Favorite = movie.Favorite  });
                                SkipList2.Add(skip);
                            }
                            else
                            {
                                skip++;
                            }
                        }
                    }
                    else
                    {
                        skip++;
                    }

                }
                catch
                {

                }
            }
            
        }
        //delete movie button, deletes indexed movie on wantList 
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            int index = MoviesListView.SelectedIndex;
            index = trySkipList1(index);
            wantMovieList.RemoveAt(index);
            Title_Textbox.Text = "";
            GenreComboBox.Text = "Genre";

            setModifiedState();
            printWantMovieList();
            DisableButtons1();

        }
        //opens comments window from want/watched movie page
        private void View_Comments_Click(object sender, RoutedEventArgs e)
        {
            int index = MoviesListView.SelectedIndex;
            index = trySkipList1(index);
            commentWindow = 1;
            new MovieComment(index, commentWindow, wantMovieList[index].Comments, wantMovieList[index].Title, this).Show();
        }
        private void View_Comments2_Click(object sender, RoutedEventArgs e)
        {
            int index = MoviesListView2.SelectedIndex;
            index = trySkipList2(index);
            commentWindow = 2;
            new MovieComment(index, commentWindow, watchedMovieList[index].Comments, watchedMovieList[index].Title, this).Show();
        }
        //adds comment based on values sent from MovieComment.xaml.cs
        public void AddComment1(String c, int i)
        {
            bool modified = true;
            if (c == wantMovieList[i].Comments)
            {
                modified = false;
            }
            try
            {
                wantMovieList[i].Comments = c;
                if (modified)
                    setModifiedState();
            }
            catch
            {

            }
        }
        public void AddComment2(String c, int i)
        {
            bool modified = true;
            if (c == watchedMovieList[i].Comments)
            {
                modified = false;
            }
            try
            {
                watchedMovieList[i].Comments = c;
                if (modified)
                    setModifiedState();
            }
            catch
            {

            }
        }
        //views info of movie from omdb database based on the title of a movie.  Sends the reader to OMDB.xaml.cs to display information on that window
        private void View_Info_Click(object sender, RoutedEventArgs e)
        {
            int index = MoviesListView.SelectedIndex;
            index = trySkipList1(index);
            string title = wantMovieList[index].Title;
            String URLString = "http://www.omdbapi.com/?apikey=3053d437&t=" + title + "&r=xml";
            XmlTextReader reader = new XmlTextReader(URLString);

            bool response = true;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        while (reader.MoveToNextAttribute()) // Read the attributes.
                            if (reader.Name == "response")
                            {
                                if (reader.Value == "False")
                                    response = false;

                            }

                        break;
                }
                if (response == true)
                    new OMDB(reader).Show();
                else
                {
                    System.Windows.MessageBox.Show(" does not exist in the database!", "No Response", (MessageBoxButton)MessageBoxButtons.OK);
                    break;
                }
            }

        }

        private void View_Info2_Click(object sender, RoutedEventArgs e)
        {
            int index = MoviesListView2.SelectedIndex;
            index = trySkipList2(index);
            string title = watchedMovieList[index].Title;
            String URLString = "http://www.omdbapi.com/?apikey=3053d437&t=" + title + "&r=xml";

            XmlTextReader reader = new XmlTextReader(URLString);

            bool response = true;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        while (reader.MoveToNextAttribute()) // Read the attributes.
                            if (reader.Name == "response")
                            {
                                if (reader.Value == "False")
                                    response = false;

                            }

                        break;
                }
                if (response == true)
                    new OMDB(reader).Show();
                else
                {
                    System.Windows.MessageBox.Show(title + " does not exist in the database!", "No Response", (MessageBoxButton)MessageBoxButtons.OK);
                    break;
                }
            }
        }
        //views review of watched movie, opens review window
        private void View_Review_Click(object sender, RoutedEventArgs e)
        {
            int index = MoviesListView2.SelectedIndex;
            index = trySkipList2(index);
            new MovieReview(index, watchedMovieList[index].Review, watchedMovieList[index].Rating, watchedMovieList[index].Title, this).Show();
        }
        //adds review to indexed watched movie based on information sent from MovieReview.xaml.cs
        public void AddReview(String review, double rating, int index)
        {
            bool modified = true;
            if (review == watchedMovieList[index].Review && rating == watchedMovieList[index].Rating)
            {
                modified = false;
            }
            try
            {
                watchedMovieList[index].Review = review;
                watchedMovieList[index].Rating = rating;
                CheckSort2();
                if (modified)
                    setModifiedState();
                DisableButtons2();
            }
            catch
            {

            }
        }
        //handle watched click, send to moviereview page to force user to review movie
        private void Watched_Click(object sender, RoutedEventArgs e)
        {
            int index = MoviesListView.SelectedIndex;
            index = trySkipList1(index);

            new MovieReview(index, wantMovieList[index].Review, wantMovieList[index].Rating, wantMovieList[index].Title, this).Show();
        }
        //transfers to movie from want to watched based on index
        public void TransferWatched(string review, double rating, int index)
        {
            WatchedMovie watchedmovie = new WatchedMovie();
            watchedmovie.Title = wantMovieList[index].Title;
            watchedmovie.Comments = wantMovieList[index].Comments;
            watchedmovie.Genre = wantMovieList[index].Genre;
            watchedmovie.Review = review;
            watchedmovie.Rating = rating;

            watchedMovieList.Add(watchedmovie);
            wantMovieList.RemoveAt(index);

            printWantMovieList();
            printWatchedMovieList();
            DisableButtons1();
            DisableButtons2();
            setModifiedState();

            TabControl.SelectedIndex = 1;
        }
        //add movie on watched movie page 
        private void Add_Movie_Click2(object sender, RoutedEventArgs e)
        {
            WatchedMovie movie = new WatchedMovie();
            bool worked = true;
            try
            {
                movie.Title = Title_Textbox2.Text;
                if (Title_Textbox2.Text == "")
                {
                    System.FormatException fEx = new System.FormatException();
                    throw fEx;
                }
            }
            catch (FormatException)
            {
                Title_Textbox.BorderBrush = Brushes.Red;
                worked = false;
            }
            try
            {
                double rating = double.Parse(Rating.Text);
                movie.Rating = rating;
                if (rating < 0 || rating >= 10)
                {
                    System.FormatException fEx = new System.FormatException();
                    throw fEx;
                }
            }
            catch (FormatException)
            {
                Rating.BorderBrush = Brushes.Red;
                worked = false;
            }
            ComboBoxItem cbi = (ComboBoxItem)GenreComboBox2.SelectedItem;
            movie.Genre = cbi.Content.ToString();
            if (worked == true)
            {
                watchedMovieList.Add(movie);
                Title_Textbox.BorderBrush = SystemColors.ControlDarkBrush;
                Rating.BorderBrush = SystemColors.ControlDarkBrush;
                Title_Textbox2.Text = "";
                Rating.Text = "";
                GenreComboBox.Text = "Genre";
                setModifiedState();

                int skip = 0;
                SkipList2.Clear();
                MoviesListView2.Items.Clear();
                foreach (WatchedMovie movie2 in watchedMovieList)
                {
                    try
                    {
                        if (movie2.Title.ToUpper().Contains(Search2.Text.ToUpper()) || Search2.Text == "")
                        {
                            if (Favorite_cb.IsChecked==true)
                            {
                                if (movie2.Favorite == "*")
                                {
                                    MoviesListView2.Items.Add(new WatchedMovie { Title = movie2.Title, Genre = movie2.Genre, Rating = movie2.Rating, DateAdded=movie2.DateAdded, Favorite = movie2.Favorite });
                                    SkipList2.Add(skip);
                                }
                            }
                            else
                            {
                                MoviesListView2.Items.Add(new WatchedMovie { Title = movie2.Title, Genre = movie2.Genre, Rating = movie2.Rating, DateAdded = movie2.DateAdded, Favorite = movie2.Favorite });
                                SkipList2.Add(skip);
                            }
                        }
                        else
                        {
                            skip++;
                        }

                    }
                    catch
                    {

                    }
                }
                CheckSort2();
            }
        }

        //edit click, edits watched movie information based on new information
        private void Edit_Click2(object sender, RoutedEventArgs e)
        {
            int index = MoviesListView2.SelectedIndex;
            index = trySkipList2(index);

            bool worked = true;
            try
            {

                if (Title_Textbox2.Text == "")
                {
                    System.FormatException fEx = new System.FormatException();
                    throw fEx;
                }
                watchedMovieList[index].Title = Title_Textbox2.Text;
            }
            catch (FormatException)
            {
                Title_Textbox.BorderBrush = Brushes.Red;
                worked = false;
            }
            try
            {
                double rating = double.Parse(Rating.Text);

                if (rating < 0 || rating > 10)
                {
                    System.FormatException fEx = new System.FormatException();
                    throw fEx;
                }
                watchedMovieList[index].Rating = rating;
            }
            catch (FormatException)
            {
                Rating.BorderBrush = Brushes.Red;
                worked = false;
            }
            ComboBoxItem cbi = (ComboBoxItem)GenreComboBox2.SelectedItem;
            watchedMovieList[index].Genre = cbi.Content.ToString();
            if (worked == true)
            {
                Title_Textbox.BorderBrush = SystemColors.ControlDarkBrush;
                Rating.BorderBrush = SystemColors.ControlDarkBrush;
                Title_Textbox2.Text = "";
                Rating.Text = "";
                GenreComboBox.Text = "Genre";
                printWatchedMovieList();

                setModifiedState();

                DisableButtons2();
            }
        }
        //deletes movie based on index on watched movie tab
        private void Delete_Click2(object sender, RoutedEventArgs e)
        {
            int index = MoviesListView2.SelectedIndex;
            index = trySkipList2(index);
            watchedMovieList.RemoveAt(index);

            Title_Textbox2.Text = "";
            Rating.Text = "";
            GenreComboBox.Text = "Genre";

            printWatchedMovieList();
            setModifiedState();
            DisableButtons2();
        }
        //addes watched movie as favorite assigns value to *
        private void Favorite_Click(object sender, RoutedEventArgs e)
        {
            int index = MoviesListView2.SelectedIndex;
            index = trySkipList2(index);
            if (watchedMovieList[index].Favorite == "*")
                watchedMovieList[index].Favorite = "";
            else
                watchedMovieList[index].Favorite = "*";
            Title_Textbox2.Text = "";
            Rating.Text = "";
            GenreComboBox.Text = "Genre";

            setModifiedState();
            printWatchedMovieList();
            DisableButtons2();


        }
        //handles searching based on if title contains text, creates skip list for number of movies skipped at each index
        private void Search1_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = Search1.Text;

            MoviesListView.SelectedIndex = -1;
            DisableButtons1();

            int skip = 0;
            SkipList1.Clear();
            MoviesListView.Items.Clear();
            foreach (WantMovie movie in wantMovieList)
            {
                try
                {
                    if (movie.Title.ToUpper().Contains(Search1.Text.ToUpper()) || Search1.Text == "")
                    {
                        MoviesListView.Items.Add(new WantMovie { Title = movie.Title, Genre = movie.Genre, DateAdded = movie.DateAdded });
                        SkipList1.Add(skip);
                    }
                    else
                    {
                        skip++;
                    }

                }
                catch
                {

                }
            }
        }

        private void Search2_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = Search2.Text;

            MoviesListView2.SelectedIndex = -1;
            DisableButtons2();


            int skip = 0;
            SkipList2.Clear();
            MoviesListView2.Items.Clear();
            foreach (WatchedMovie movie in watchedMovieList)
            {
                try
                {
                    if (movie.Title.ToUpper().Contains(Search2.Text.ToUpper()) || Search2.Text == "")
                    {
                        if (Favorite_cb.IsChecked == true)
                        {
                            if (movie.Favorite == "*")
                            {
                                MoviesListView2.Items.Add(new WatchedMovie { Title = movie.Title, Genre = movie.Genre, Rating = movie.Rating, DateAdded = movie.DateAdded, Favorite = movie.Favorite });
                                SkipList2.Add(skip);
                            }
                        }
                        else
                        {
                            MoviesListView2.Items.Add(new WatchedMovie { Title = movie.Title, Genre = movie.Genre, Rating = movie.Rating, DateAdded=movie.DateAdded, Favorite = movie.Favorite });
                            SkipList2.Add(skip);
                        }
                    }
                    else
                    {
                        skip++;
                    }

                }
                catch
                {

                }
            }
        }
        /*
         * 0=title desc
         * 1=title asc
         * 2=genre desc
         * 3=genre asc
         * 4=dateadded desc
         * 5=dateadded asc
         */
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckSort();
        }
        //calls proper sort based on index
        private void CheckSort()
        {
            int index = SortComboBox.SelectedIndex;
            if (index == 0)
            {
                SortTitle1(0);
            }
            else if (index == 1)
            {
                SortTitle1(1);
            }
            else if (index == 2)
            {
                SortGenre1(0);
            }
            else if (index == 3)
            {
                SortGenre1(1);
            }
            else if (index == 4)
            {
                SortDate1(0);
            }
            else if (index == 5)
            {
                SortDate1(1);
            }
            else
                return;
            printWantMovieList();
            DisableButtons1();
        }
        /*
         * 0=title desc
         * 1=title asc
         * 2=genre desc
         * 3=genre asc
         * 4=rating desc
         * 5=rating asc
         * 6=date desc
         * 7=date asc
         */
        private void SortComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckSort2();
        }
        private void CheckSort2()
        {
            int index = SortComboBox2.SelectedIndex;
            if (index == 0)
            {
                SortTitle2(0);
            }
            else if (index == 1)
            {
                SortTitle2(1);
            }
            else if (index == 2)
            {
                SortGenre2(0);
            }
            else if (index == 3)
            {
                SortGenre2(1);
            }
            else if (index == 4)
            {
                SortRating(0);
            }
            else if (index == 5)
            {
                SortRating(1);
            }
            else if (index == 6)
            {
                SortDate2(0);
            }
            else if (index == 7)
            {
                SortDate2(1);
            }
            else
                return;
            printWatchedMovieList();
            DisableButtons2();
        }
        //sort alogirthms 0=descending 1=ascending
        private void SortTitle1(int o)
        {
            for (int i = 0; i < wantMovieList.Count - 1; i++)
            {
                int min_idx = i;
                for (int j = i + 1; j < wantMovieList.Count; j++)
                {
                    if (wantMovieList[j].Title.CompareTo(wantMovieList[min_idx].Title) > 0 && o==0)
                        min_idx = j;
                    else if(wantMovieList[j].Title.CompareTo(wantMovieList[min_idx].Title) < 0 && o == 1)
                        min_idx = j;
                    
                }
                Movie tmp = wantMovieList[min_idx];
                wantMovieList[min_idx] = wantMovieList[i];
                wantMovieList[i] = tmp;
            }
            
        }
        private void SortTitle2(int o)
        {
            for (int i = 0; i < watchedMovieList.Count - 1; i++)
            {
                int min_idx = i;
                for (int j = i + 1; j < watchedMovieList.Count; j++)
                {
                    if (watchedMovieList[j].Title.CompareTo(watchedMovieList[min_idx].Title) > 0 && o == 0)
                        min_idx = j;
                    else if (watchedMovieList[j].Title.CompareTo(watchedMovieList[min_idx].Title) < 0 && o == 1)
                        min_idx = j;
                    
                }
                Movie tmp = watchedMovieList[min_idx];
                watchedMovieList[min_idx] = watchedMovieList[i];
                watchedMovieList[i] = tmp;
            }
        }
        private void SortGenre1(int o)
        {
            for (int i = 0; i < wantMovieList.Count - 1; i++)
            {
                int min_idx = i;
                for (int j = i + 1; j < wantMovieList.Count; j++)
                {
                    if (wantMovieList[j].Genre.CompareTo(wantMovieList[min_idx].Genre) > 0 && o == 0)
                        min_idx = j;
                    else if (wantMovieList[j].Genre.CompareTo(wantMovieList[min_idx].Genre) < 0 && o == 1)
                        min_idx = j;
                    
                }
                Movie tmp = wantMovieList[min_idx];
                wantMovieList[min_idx] = wantMovieList[i];
                wantMovieList[i] = tmp;
            }
        }
        private void SortGenre2(int o)
        {
            for (int i = 0; i < watchedMovieList.Count-1; i++)
            {
                int min_idx = i;
                for (int j = i+1 ; j < watchedMovieList.Count; j++)
                {
                    if (watchedMovieList[j].Genre.CompareTo(watchedMovieList[min_idx].Genre) > 0 && o == 0)
                        min_idx = j;
                    else if (watchedMovieList[j].Genre.CompareTo(watchedMovieList[min_idx].Genre) < 0 && o == 1)
                        min_idx = j;
                }
                Movie tmp = watchedMovieList[min_idx];
                watchedMovieList[min_idx] = watchedMovieList[i];
                watchedMovieList[i] = tmp;
            }
        }
        private void SortDate1(int o)
        {
            for (int i = 0; i < wantMovieList.Count - 1; i++)
            {
                int min_idx = i;
                for (int j = i + 1; j < wantMovieList.Count; j++)
                {
                    DateTime d1 = Convert.ToDateTime(wantMovieList[j].DateAdded);
                    DateTime d2 = Convert.ToDateTime(wantMovieList[min_idx].DateAdded);
                    if (d1.CompareTo(d2) > 0 && o == 0)
                        min_idx = j;
                    else if (d1.CompareTo(d2) < 0 && o == 1)
                        min_idx = j;
                    
                }
                Movie tmp = wantMovieList[min_idx];
                wantMovieList[min_idx] = wantMovieList[i];
                wantMovieList[i] = tmp;
            }
        }
        private void SortDate2(int o)
        {
            for (int i = 0; i < watchedMovieList.Count - 1; i++)
            {
                int min_idx = i;
                for (int j = i + 1; j < watchedMovieList.Count; j++)
                {
                    DateTime d1 = Convert.ToDateTime(watchedMovieList[j].DateAdded);
                    DateTime d2 = Convert.ToDateTime(watchedMovieList[min_idx].DateAdded);
                    if (d1.CompareTo(d2) > 0 && o == 0)
                        min_idx = j;
                    else if (d1.CompareTo(d2) < 0 && o == 1)
                        min_idx = j;

                }
                Movie tmp = watchedMovieList[min_idx];
                watchedMovieList[min_idx] = watchedMovieList[i];
                watchedMovieList[i] = tmp;
            }
        }
        private void SortRating(int o)
        {
            for (int i = 0; i < watchedMovieList.Count - 1; i++)
            {
                int min_idx = i;
                for (int j = i + 1; j < watchedMovieList.Count; j++)
                {
                    if (watchedMovieList[j].Rating>watchedMovieList[min_idx].Rating && o == 0)
                        min_idx = j;
                    else if (watchedMovieList[j].Rating<watchedMovieList[min_idx].Rating && o == 1)
                        min_idx = j;
                    
                }
                Movie tmp = watchedMovieList[min_idx];
                watchedMovieList[min_idx] = watchedMovieList[i];
                watchedMovieList[i] = tmp;
            }
        }
        //displays only movies that have been favorited
        private void Favorite_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool? ischecked= Favorite_cb.IsChecked;
            if (ischecked == true)
            {
                int skip = 0;
                SkipList2.Clear();
                MoviesListView2.Items.Clear();
                foreach (WatchedMovie movie in watchedMovieList)
                {
                    try
                    {
                        if (movie.Favorite == "*" && (movie.Title.ToUpper().Contains(Search2.Text.ToUpper()) || Search2.Text == ""))
                        {
                            MoviesListView2.Items.Add(new WatchedMovie { Title = movie.Title, Genre = movie.Genre, Rating = movie.Rating, DateAdded=movie.DateAdded, Favorite = movie.Favorite });
                            SkipList2.Add(skip);
                        }
                        else
                        {
                            skip++;
                        }

                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                SkipList2.Clear();
                printWatchedMovieList();
            }
            DisableButtons2();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }
        //fills in information based on selected index and enables buttons
        private void MoviesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = MoviesListView.SelectedIndex;
            index = trySkipList1(index);

            Edit_Movie.IsEnabled = true;
            Delete_Movie.IsEnabled = true;
            Comment_Movie.IsEnabled = true;
            Watched_Movie.IsEnabled = true;
            Info_Movie.IsEnabled = true;

            try
            {
                Title_Textbox.Text = wantMovieList[index].Title.ToString();
                GenreComboBox.Text = wantMovieList[index].Genre;
            }
            catch
            {

            }
        }

        private void MoviesListView_SelectionChanged2(object sender, SelectionChangedEventArgs e)
        {
            int index = MoviesListView2.SelectedIndex;
            index = trySkipList2(index);

            Edit_Movie2.IsEnabled = true;
            Delete_Movie2.IsEnabled = true;
            Comment_Movie2.IsEnabled = true;
            Review_Movie.IsEnabled = true;
            Favorite_Movie.IsEnabled = true;
            Info_Movie2.IsEnabled = true;

            try
            {
                Title_Textbox2.Text = watchedMovieList[index].Title.ToString();
                GenreComboBox2.Text = watchedMovieList[index].Genre;
                Rating.Text = watchedMovieList[index].Rating.ToString();
            }
            catch
            {

            }
        }
        //Disables buttons on a page
        private void DisableButtons1()
        {
            Add_Movie.IsEnabled = false;
            Edit_Movie.IsEnabled = false;
            Delete_Movie.IsEnabled = false;
            Comment_Movie.IsEnabled = false;
            Watched_Movie.IsEnabled = false;
            Info_Movie.IsEnabled = false;
        }
        private void DisableButtons2()
        {
            Add_Movie2.IsEnabled = false;
            Edit_Movie2.IsEnabled = false;
            Delete_Movie2.IsEnabled = false;
            Comment_Movie2.IsEnabled = false;
            Review_Movie.IsEnabled = false;
            Favorite_Movie.IsEnabled = false;
            Info_Movie2.IsEnabled = false;
        }
        //enables all buttons
        private void EnableButtons()
        {
            Edit_Movie.IsEnabled = true;
            Edit_Movie2.IsEnabled = true;
            Delete_Movie.IsEnabled = true;
            Delete_Movie2.IsEnabled = true;
            Comment_Movie.IsEnabled = true;
            Comment_Movie2.IsEnabled = true;
            Review_Movie.IsEnabled = true;
            Watched_Movie.IsEnabled = true;
            Favorite_Movie.IsEnabled = true;
            Info_Movie.IsEnabled = true;
            Info_Movie2.IsEnabled = true;
        }
        //makes sure all information is filled in before add movie is enabled for both pages
        private void Title_Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (GenreComboBox.SelectedValue != null)
            {
                if (Title_Textbox.Text != "" && !GenreComboBox.SelectedValue.ToString().Equals("Genre"))
                {
                    Add_Movie.IsEnabled = true;
                }
                if (Title_Textbox.Text == "")
                {
                    Add_Movie.IsEnabled = false;
                }
            }
        }
        private void GenreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GenreComboBox.SelectedValue != null)
            {
                if (Title_Textbox.Text != "" && !GenreComboBox.SelectedValue.ToString().Equals("Genre"))
                {
                    Add_Movie.IsEnabled = true;
                }
            }
        }

        private void Title_Textbox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (GenreComboBox2.SelectedValue != null)
            {
                if (Title_Textbox2.Text != "" && !GenreComboBox2.SelectedValue.ToString().Equals("Genre") && Rating.Text != "")
                {
                    Add_Movie2.IsEnabled = true;
                }
                if (Title_Textbox2.Text == "")
                {
                    Add_Movie2.IsEnabled = false;
                }
            }
        }

        private void GenreComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GenreComboBox2.SelectedValue != null)
            {
                if (Title_Textbox2.Text != "" && !GenreComboBox2.SelectedValue.ToString().Equals("Genre") && Rating.Text != "")
                {
                    Add_Movie2.IsEnabled = true;
                }
            }
        }

        private void Rating_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (GenreComboBox2.SelectedValue != null)
            {
                if (Title_Textbox2.Text != "" && !GenreComboBox2.SelectedValue.ToString().Equals("Genre") && Rating.Text != "")
                {
                    Add_Movie2.IsEnabled = true;
                }
                if (Rating.Text == "")
                {
                    Add_Movie2.IsEnabled = false;
                }
            }
        }
        //google searches movie title + 'movie' opens in default browser
        private void MoviesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int index = MoviesListView.SelectedIndex;
                string searchQuery = wantMovieList[index].Title + " movie";
                Process.Start("https://www.google.com/search?q=" + Uri.EscapeDataString(searchQuery));
            }
            catch
            {
            }
        }
        private void MoviesListView2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                int index = MoviesListView2.SelectedIndex;
                string searchQuery = watchedMovieList[index].Title + " movie";
                Process.Start("https://www.google.com/search?q=" + Uri.EscapeDataString(searchQuery));
            }
            catch
            {

            }
        }
        //tries adding given index to skip list index if it exits. In order to make sure the selected index matches the index in want and watched list
        private int trySkipList1(int index)
        {
            try
            {
                index += SkipList1[index];
            }
            catch
            {

            }
            return index;
        }
        private int trySkipList2(int index)
        {
            try
            {
                index += SkipList2[index];
            }
            catch
            {

            }
            return index;
        }


    }
}

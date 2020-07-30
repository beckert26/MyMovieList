using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieList
{
    [Serializable]
    abstract class Movie
    {
        private string title;
        private string genre;
        private string comments;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }  
        public string Genre
        {
            get
            {
                return genre;
            }
            set
            {
                genre = value;
            }
        }
        public string Comments
        {
            get
            {
                return comments;
            }
            set
            {
                comments = value;
            }
        }
        public abstract double Rating
        {
            get;
            set;
        }
        public abstract string DateAdded
        {
            get;
            set;
        }
        public abstract string Favorite
        {
            get;
            set;
        }
        public abstract string Review
        {
            get;
            set;
        }
    }
}

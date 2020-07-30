using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieList
{
    [Serializable]
    class WatchedMovie : Movie
    {
        private double rating;
        private string favorite;
        private string review;
        private string dateAdded;
        public WatchedMovie()
        {
            favorite = "";
            dateAdded = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");
        }

        public override double Rating
        {
            get
            {
                return rating;
            }
            set
            {
                rating = value;
            }
        }
        public override string Favorite
        {
            get
            {
                return favorite;
            }
            set
            {
                favorite = value;
            }
        }
        public override string Review
        {
            get
            {
                return review;
            }
            set
            {
                review = value;
            }
        }
        public override String DateAdded
        {
            get
            {
                return dateAdded;
            }
            set
            {
                dateAdded = value;
            }
        }

    }
}

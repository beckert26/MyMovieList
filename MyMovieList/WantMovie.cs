using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieList
{
    [Serializable]
    class WantMovie : Movie
    {
        private String dateAdded;

        public WantMovie()
        {
            dateAdded = DateTime.Now.ToString("MM/dd/yyyy h:mm tt") ;
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

        public override double Rating
        {
            get;
            set;
        }
        public override string Favorite
        {
            get;
            set;
        }
        public override string Review
        {
            get;
            set;
        }
    }
}

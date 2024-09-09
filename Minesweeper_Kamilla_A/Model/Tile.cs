using System.Windows.Controls;
using System.Windows.Media;


namespace Minesweeper_Kamilla_A.Model
{
    internal class Tile : Button
    {
        public int indexI { get; set; }
        public int indexJ { get; set; }
        public string btnName { get; set; }
        public bool isBomb { get; set; }
        public int noNeighborBombs { get; set; }


        private bool _isFlipped;

        public Tile(int i, int j)
        {
            indexI = i;
            indexJ = j;
        }


        /// <summary>
        /// This method gets or sets the flipped state of the tile.
        /// When set to true, the tile is flipped and its content and background are updated.
        /// </summary>
        public bool isFlipped {

            get { return _isFlipped; }
            set
            {
                _isFlipped = value;

                IsFlipped();  
            }
        }


        /// <summary>
        /// Method for setting the content (bomb symbol, number of neighboring bombs, or nothing if no neighbor bombs) 
        /// and changing the background colour when a tile is flipped.
        /// </summary>
        public void IsFlipped() {

            // sets the content to a bomb symbol if the tile contains a bomb
            if (isBomb)
            {
                this.Content = MainWindow.bomb;
            }
            // clears the content if the tile has no neighbor bombs
            else if (noNeighborBombs == 0) {
                
                this.Content = "";
             
            }
            // sets the content to the number of neighboring bombs if there is at least one
            else
            { 
                this.Content = $"{this.noNeighborBombs}";
            }

            // changes the background of the flipped tile
            this.Background = Brushes.WhiteSmoke;
        }
    }
}

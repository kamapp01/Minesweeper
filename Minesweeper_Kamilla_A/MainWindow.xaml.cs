using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Minesweeper_Kamilla_A.Model;


namespace Minesweeper_Kamilla_A
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int ROWS = 9;
        private const int COLOUMNS = 9;
        private const int TOTAL_NO_BOMBS = 10;

        private string happySmiley = @"C:\Users\kamil\source\repos\Minesweeper_Kamilla_A\Minesweeper_Kamilla_A\Resources\gladS.png";
        private string gameOverSmiley = @"C:\\Users\\kamil\\source\\repos\\Minesweeper_Kamilla_A\\Minesweeper_Kamilla_A\\Resources\\gameOverS.png";
        public static string bomb = "💣";
        private string party = "🎉";

        private System.Timers.Timer gameTimer;
        private int elapsedTimeSeconds;


        private Tile[,] tiles = new Tile[ROWS, COLOUMNS];
        private int noOfButtons;
        private int btnClickCounter;


        public MainWindow()
        {
            InitializeComponent();
            InitializeGameBoard();
            PlaceBombs();
            CountNeighborBombs();

        }



        /// <summary>
        /// This method sets up the gameboard with the happy smiley, number of bombs, 
        /// the timer and creates and places all the buttons 
        /// </summary>
        private void InitializeGameBoard()
        {
            SetSmileyImage(happySmiley);
            SetBombsNo();
            SetWatch();

            CreateAndPlaceButtons();
        }


        /// <summary>
        /// Method for creating and placing buttons in the grid
        /// </summary>
        private void CreateAndPlaceButtons()
        {
            // iterates through rows
            for (int i = 0; i < ROWS; i++)
            {

                // iterates through coloumns
                for (int j = 0; j < COLOUMNS; j++)
                {
                    // creates a new button for the current grid cell
                    Tile button = CreateButton(i, j);

                    // places the button in the current grid cell
                    PlaceButtonGrid(button, i, j);

                    // counts total number of buttons in the grid
                    noOfButtons++;
                }
            }
        }


        /// <summary>
        /// This method adds a tile to an array list and sets the buttons row and column position 
        /// in the grid and adds the button to the grit 
        /// </summary>
        /// <param name="button"> the tile to be placed in the grid </param>
        /// <param name="i"> the row position of the tile in the grid </param>
        /// <param name="j"> the column position of the tile in the grid </param>
        private void PlaceButtonGrid(Tile button, int i, int j)
        {
            // adds tile to array list for later use
            tiles[i, j] = button;

            // sets the buttons row and column position in the grid and adds the button to the grit
            Grid.SetRow(button, i);
            Grid.SetColumn(button, j);

            gridGame.Children.Add(button);
        }


        /// <summary>
        /// This method sets the number of bombs in the game.
        /// It sets the text, colour and style in the "tbBombsNo" textbox.
        /// </summary>
        private void SetBombsNo()
        {
            tbBombsNo.Text = $"{bomb} {TOTAL_NO_BOMBS}  ";
            tbBombsNo.Foreground = new SolidColorBrush(Colors.Gray);
            tbBombsNo.FontSize = 14;

            tbBombsNo.IsReadOnly = true;
        }


        /// <summary>
        /// This method sets the starting time for the timer.
        /// It sets the text, colour and style in the "tbTime" textbox.
        /// </summary>
        private void SetWatch() {

            tbTime.Text = $"00:00:00";
            tbTime.Foreground = new SolidColorBrush(Colors.Gray);
            tbTime.FontSize = 18;

            tbTime.IsReadOnly = true;
        }

        
        /// <summary>
        /// Method for setting the smiley-image 
        /// </summary>
        /// <param name="smileyPath"> the absolute filepath of the smiley image </param>
        private void SetSmileyImage(string smileyPath)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(smileyPath, UriKind.Absolute));
            imageSmiley.Source = bitmap;
        }


        /// <summary>
        /// Method for creating a Tile button with specified dimensions and styling, and attaches a click event handler.
        /// </summary>
        /// <param name="i"> the row index of the button </param>
        /// <param name="j"> the column index of the button </param>
        /// <returns> the created Tile button </returns>
        private Tile CreateButton(int i, int j)
        {
            Tile button = new Tile(i, j);

            button.Name = $"btn_{i}_{j}";
            button.Height = 40;
            button.Width = 40;
            button.Foreground = new SolidColorBrush(Colors.Gray);
            button.FontSize = 14;

            // adds a button_click event to the button
            button.Click += Button_Click;
            
            return button;
        }


        /// <summary>
        /// This method randomly places the specified number of bombs on the game board.
        /// </summary>
        private void PlaceBombs() {

            int bombCount = 0;
            Random rnd = new Random();

            while (bombCount != TOTAL_NO_BOMBS) {

                int iRandom = rnd.Next(0, 9);
                int jRandom = rnd.Next(0, 9);

                if (!tiles[iRandom, jRandom].isBomb) { 
                
                    tiles[iRandom, jRandom].isBomb = true;
                    bombCount++;

                    Debug.WriteLine($"bomb no: {bombCount}       btn: {tiles[iRandom, jRandom].Name}");
                }
            }
        }


        /// <summary>
        /// This method handles tile button clicks, flipping the tile, updating game state, starting the timer, and checking 
        /// for win or game over conditions. Flips neighbor tiles with no neighbor bombs if the clicked tile has no neighbor bombs.
        /// </summary>
        /// <param name="sender"> the source of the click event, expected to be a Tile button </param>
        /// <param name="e"> the event data for the click event </param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Tile btn = sender as Tile;

            if (btn == null || btn.isFlipped) { return; }

            // starts the timer when the first button is clicked
            if (btnClickCounter == 0)
            {
                StartTimer();
            }

            btn.isFlipped = true;


            btnClickCounter++;

            // recursively flips neighboring tiles with no neighbor bombs if the clicked tile has no neighbor bombs
            if (btn.noNeighborBombs == 0 && !btn.isBomb) { RecursiveZeroSearch(btn, true); }

            Debug.WriteLine(btnClickCounter);


            // checks if the player has won comparing buttons click with the number of non-bomb buttons
            if (btnClickCounter == (noOfButtons - TOTAL_NO_BOMBS))
            {
                YouWon();
                return;
            }

            // updates the button content based on its state (bomb / non-bomb) and handles game over if it's a bomb 
            if (btn.isBomb){ 
                
                GameOver();
                return;
            }

            

            
            //MessageBox.Show($"{ btnClickCounter}");
        }


        /// <summary>
        /// Method to start the timer.
        /// </summary>
        private void StartTimer()
        {

            // setting up timer
            gameTimer = new System.Timers.Timer(1000);
            gameTimer.Elapsed += UpdateTimer;
            gameTimer.Start();

        }


        /// <summary>
        /// This method updates the timer every second and displays the elapsed time in "hh:mm:ss" format.
        /// </summary>
        /// <param name="sender">The source of the timer event.</param>
        /// <param name="e">The event data for the timer event.</param>
        public void UpdateTimer(object sender, ElapsedEventArgs e)
        {

            Dispatcher.Invoke(new Action(() =>
            {
                elapsedTimeSeconds++;

                // makes the elapsedTime as format "hh:mm:ss"
                tbTime.Text = ElapsedTimeConverted();
            }));

        }


        /// <summary>
        /// This method converts the elapsed time to a "hh:mm:ss" format.
        /// </summary>
        /// <returns> a string representing the elapsed time in "hh:mm:ss" format </returns>
        private string ElapsedTimeConverted() {

            TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTimeSeconds);
            return timeSpan.ToString(@"hh\:mm\:ss");
        }


        /// <summary>
        /// Method that stops the game timer.
        /// </summary>
        private void StopTimer() { 
        
            gameTimer.Stop();
        }


        /// <summary>
        /// This method resets the game timer.
        /// </summary>
        private void ResetTimer() {

            if (gameTimer != null)
            {
                gameTimer.Stop();
                gameTimer.Dispose();
            }

            elapsedTimeSeconds = 0;
            tbTime.Text = "00:00:00";
        }


        /// <summary>
        /// This method handles the win condition by stopping the timer and showing a congratulation message with the total time spent.
        /// </summary>
        private void YouWon() { 
        
            StopTimer();
            string timeSpent = ElapsedTimeConverted();

            MessageBox.Show($"{party} CONGRATS! {party}\n\nTime spent: {timeSpent}");
        }


        /// <summary>
        /// This method handles the game over condition by setting the game over smiley, stopping the timer
        /// and showing a game over message with the total time spent.
        /// - furthermore the method disables all tiles, preventing user interaction after end game. 
        /// </summary>
        private void GameOver() {

            SetSmileyImage(gameOverSmiley);
            StopTimer();

            string timeSpent = ElapsedTimeConverted();

            MessageBox.Show($"{bomb} GAME OVER {bomb}\n\nTime spent: {timeSpent}");


            // disables all tiles, preventing user interaction after end game
            foreach (Tile tile in tiles)
            {
                tile.IsEnabled = false;
            }
        }


        /// <summary>
        /// Method for finding and returning a list of neighboring tiles around a specified tile.
        /// </summary>
        /// <param name="tile"> the tile for which to find neighbors </param>
        /// <returns> a list of neighboring tiles </returns>
        private List<Tile> FindNeighborFields(Tile tile)
        {

            List<Tile> neighbors = new List<Tile>();

            // offsets for row (iOffsets) and column (jOffsets) to check the 8 neighboring tiles around a given tile
            // (top-left, top, top-right, left, right, bottom-left, bottom, bottom-right).
            int[] iOffsets = { -1, -1, -1, 0, 0, 1, 1, 1 };         // [i + -1] , [i + -1] , [i + -1] , [i + 0]  , [i + 0] , [i + 1]  , [i + 1] , [i + 1]
            int[] jOffsets = { -1, 0, 1, -1, 1, -1, 0, 1 };         // [j + -1] , [j + 0]  , [j + 1]  , [j + -1] , [j + 1] , [j + -1] , [j + 0] , [j + 1]

            // index of current tile
            int i = tile.indexI;
            int j = tile.indexJ;


            for (int k = 0; k < iOffsets.Length; k++)
            {
                int iNeighbor = i + iOffsets[k];
                int jNeighbor = j + jOffsets[k];


                // makes sure that the neighbor index is within the valid bounds of the grid
                if (iNeighbor >= 0 && iNeighbor < ROWS && jNeighbor >= 0 && jNeighbor < COLOUMNS)
                {
                    Tile neighborTile = tiles[iNeighbor, jNeighbor];
                    neighbors.Add(neighborTile);
                }
            }

            return neighbors;
        }


        /// <summary>
        /// This method counts the total number of neighbor bombs for each tile in the grid.
        /// </summary>
        private void CountNeighborBombs() {

            foreach (Tile tile in tiles) { 
            
                List <Tile> neighborTiles = FindNeighborFields(tile);
                int countNeighborBombs = 0;
                
                
                foreach (Tile naboTile in neighborTiles) {

                    if (naboTile.isBomb)
                    {
                        countNeighborBombs++;
                    }
                }

                tile.noNeighborBombs = countNeighborBombs;
            }
        }


        /// <summary>
        /// Method that recursively flips neighbor tiles with no neighbor bombs, starting with the specified tile.
        /// Sets the tile's content to "0" if it is the first tile in the recursion.
        /// </summary>
        /// <param name="tile"> the starting tile to start the recursion from </param>
        /// <param name="isFirst"> indicates if this is the first tile in the recursion </param>
      
        private void RecursiveZeroSearch(Tile tile, bool isFirst) {

            // sets the content of the selected tile for "0" if isFirst = true
            if (isFirst) { 
                tile.Content = "0"; 
            }

            List<Tile> neighborTiles = FindNeighborFields(tile);

                foreach (Tile neighborTile in neighborTiles)
                {

                    if (!neighborTile.isFlipped && neighborTile.noNeighborBombs == 0)
                    {
                        neighborTile.isFlipped = true;
                        btnClickCounter++;
                        RecursiveZeroSearch(neighborTile, false);
                    }
                }
            }


        /// <summary>
        /// This resets the game when the reset button is clicked.
        /// </summary>
        /// <param name="sender"> the button that was clicked </param>
        /// <param name="e"> event data for the click event </param>
        private void btnReset_Click(object sender, RoutedEventArgs e){
            ResetGame();
        }


        /// <summary>
        /// This method resets the game by resetting the timer, board, game variables and the smiley image.
        /// </summary>
        private void ResetGame() {

            ResetTimer();
            ResetBoard();
            ResetGameVariables();

            SetSmileyImage(happySmiley);
        }


        /// <summary>
        /// Method for resetting the game board by clearing the grid, reinitializing tiles, 
        /// and setting up buttons, bombs, and neighbor counts.
        /// </summary>
        private void ResetBoard()
        {
            gridGame.Children.Clear();

            tiles = new Tile[ROWS, COLOUMNS];

            CreateAndPlaceButtons();
            PlaceBombs();
            CountNeighborBombs();

        }


        /// <summary>
        /// This method resets game related variables to their initial values.
        /// </summary>
        private void ResetGameVariables()
        {
            noOfButtons = 0;
            btnClickCounter = 0;
        }

    }
}







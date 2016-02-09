using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Checkers_minimax
    {
    public partial class board_form : Form
        {
        #region global variables
        public square[,] board = new square[8, 8];//the master board representing the board actually in play
        PictureBox[] board_images = new PictureBox[65];
        public List<square[,]> current_possible_moves = new List<square[,]>();
        public minimax min = new minimax();
        string turn_colour = "white";
        Image white_empty = Image.FromFile("whiteblank.jpg");
        Image white_blank_selected = Image.FromFile("whiteblankselected.jpg");
        int click_phase = 1;
        int click_1_valx;//values for clicks
        int click_1_valy;
        int click_2_valx;
        int click_2_valy;
        bool minimax_over;

        string genval = File.ReadAllText("winning generations.txt");//text file with gerenrational data

        #endregion

        public board_form()
            {
            InitializeComponent();
            }

        public bool is_over_quiery( square[,] bd, string cur_turn )
            {
            if (possible_moves(bd, cur_turn, false, -1, true).Count == 0)//kinda self explanitory
                {
                return true;
                }
            else
                return false;
            }

        private void Form1_Load( object sender, EventArgs e )
            {
            initialize();
            update_images();

            
            }

        public void training_match(square[,] bd)
            {
            /* 
               This section is the section where the computer will "hopefully" play against itslef. 
            */
            string turn = "black";
            bool isbaby = true; //if is baby is true, the computer will use the mutated generation of the board values
            string not_turn = " ";
            List<square[,]>  possible_list = possible_moves(bd,"white",false,-1,false);
            Queue<square[,]> repeated = new Queue<square[,]>();
            //while (possible_list.Count > 0)
            for (int i = 0; i <2; i++ )//these can be commented and uncommented to run the sim as far as you like, or with the while loop, untill the end
                {
                isbaby = !isbaby;
                if (turn == "white")
                    {
                    turn = "black";
                    not_turn = "white";

                    }
                else
                    {
                    turn = "white";
                    not_turn = "black";
                    }
                int bestval = 0;
                int count = 0;
                int bestposition = 0;
                square[,] this_state = new_board();
                set_board_equal_to(board, this_state);
                if (repeated.Contains(this_state))
                    {
                    update_images();
                    label2.Text = turn;
                    break;
                    }
                repeated.Enqueue(this_state);
                foreach (square[,] pos in possible_list)//looks at all the boards that stem from this position
                    {
                        int this_min = min.minimax_iterative(pos, 7, false, turn, -9999, 9999);//runs minimax on each
                        if (turn == "black")
                            {
                            if (this_min < bestval)
                                {
                                bestposition = count;//if a value is superior, set the position to that value
                                bestval = this_min;
                                }
                            }
                        else
                            {
                            if (this_min > bestval)
                                {
                                bestposition = count;//if a value is superior, set the position to that value
                                bestval = this_min;
                                }
                            }
                        
                        count++;
                        
                    }
                board = possible_list[bestposition];//chaneg the master board to that possible root
                update_images();
                
                possible_list = possible_moves(board, not_turn, false, -1, false);//sets up the possible moves list for the next game state
                }
            label2.Text = turn;
            }

        public void update_images()
            {
            for (int y = 0; y < 8; y++)
                {
                for (int x = 0; x < 8; x++)
                    {
                    if (board[x, y].square_state == "black")
                        {
                        board_images[board[x, y].num_index].Image = Image.FromFile("whitewithblack.jpg");
                        if (board[x, y].square_state == "black" && board[x, y].is_king == true)
                            {
                            board_images[board[x, y].num_index].Image = Image.FromFile("whitewithblackking.jpg");
                            }
                        }
                    else if (board[x, y].square_state == "white")
                        {
                        board_images[board[x, y].num_index].Image = Image.FromFile("whitewithwhite.jpg");
                        if (board[x, y].is_king == true)
                            {
                            board_images[board[x, y].num_index].Image = Image.FromFile("whitewithwhiteking.jpg");
                            }
                        }
                    else
                        {
                        if (board[x, y].num_index % 2 == 0)
                            {
                            if (y % 2 == 0)
                                {
                                board_images[board[x, y].num_index].Image = white_empty;
                                }
                            else
                                {
                                board_images[board[x, y].num_index].Image = Image.FromFile("blackblank.jpg");
                                }
                            }
                        else
                            {
                            if (y % 2 == 0)
                                {
                                board_images[board[x, y].num_index].Image = Image.FromFile("blackblank.jpg");
                                }
                            else
                                {
                                board_images[board[x, y].num_index].Image = white_empty;
                                }
                            }

                        }
                    }

                }
            
            }//draws new board

        public square[,] new_board()
            {
            square[,] returnboard = new square[8, 8];
            int count = 0;
            for (int x=0; x<8; x++)
                {
                for (int y=0; y<8; y++)
                    {
                    returnboard[x, y] = new square();
                    returnboard[x, y].xcoord = x;
                    returnboard[x, y].ycoord = y;
                    returnboard[x, y].num_index = count;
                    count++;
                    }
                }
            return returnboard;
            }//makes a fresh blank board

        public void set_board_equal_to( square[,] original, square[,] copy )
            {
            for (int x=0; x<8; x++)
                {
                for (int y=0; y<8; y++)
                    {
                    copy[x, y].is_king = original[x, y].is_king;
                    copy[x, y].num_index = original[x, y].num_index;
                    copy[x, y].square_state = original[x, y].square_state;
                    copy[x, y].xcoord = original[x, y].xcoord;
                    copy[x, y].ycoord = original[x, y].ycoord;
                    }
                }
            }//sets one board equal to another

        #region event handlers
        public void square_click( object sender, EventArgs e )
            {
            if (is_over_quiery(board, "white") == false)
                {

                if (turn_colour == "white")
                    {
                    if (click_phase == 1)
                        {
                        click_1_valx = Convert.ToInt16(Convert.ToString(Convert.ToString(( sender as PictureBox ).Tag)[0]));
                        click_1_valy = Convert.ToInt16(Convert.ToString(Convert.ToString(( sender as PictureBox ).Tag)[1]));
                        if (board[click_1_valx, click_1_valy].square_state == turn_colour)
                            {

                            ( sender as PictureBox ).Image = Image.FromFile(string.Format("{0}selected.jpg", turn_colour));

                            click_phase = 2;
                            }
                        }
                    else if (click_phase == 2)
                        {

                        click_2_valx = Convert.ToInt16(Convert.ToString(Convert.ToString(( sender as PictureBox ).Tag)[0]));
                        click_2_valy = Convert.ToInt16(Convert.ToString(Convert.ToString(( sender as PictureBox ).Tag)[1]));

                        square[,] test_board = new_board();
                        set_board_equal_to(board, test_board);

                        test_board[click_2_valx, click_2_valy].square_state = test_board[click_1_valx, click_1_valy].square_state;
                        test_board[click_1_valx, click_1_valy].square_state = "non";


                        foreach (square[,] sq in possible_moves(board, turn_colour, false, -1, true))
                            {

                            bool equals = true;
                            for (int x=0; x<8; x++)
                                {
                                for (int y=0; y<8; y++)
                                    {
                                    if (sq[x, y].square_state == turn_colour)
                                        {

                                        if (sq[x, y].square_state != test_board[x, y].square_state)
                                            {
                                            equals = false;

                                            }
                                        }
                                    }
                                }

                            if (equals == true)
                                {

                                set_board_equal_to(sq, board);
                                update_images();

                                click_phase = 1;
                                if (Math.Max(click_1_valx, click_2_valx)-Math.Min(click_1_valx, click_2_valx) == 2)
                                    {
                                    if (possible_moves(board, turn_colour, true, board[click_2_valx, click_2_valy].num_index, true).Count == 0)
                                        {

                                        if (turn_colour=="white")
                                            {
                                            turn_colour = "black";
                                            label2.Text = "black";
                                            }
                                        else
                                            {
                                            turn_colour = "white";
                                            label2.Text = "white";
                                            }
                                        }
                                    }
                                else
                                    {
                                    if (turn_colour=="white")
                                        {
                                        turn_colour = "black";
                                        label2.Text = "black";
                                        }
                                    else
                                        {
                                        turn_colour = "white";
                                        label2.Text = "white";
                                        }
                                    }

                                break;
                                }

                            }
                        if (turn_colour == "black" && is_over_quiery(board,"black")== false)
                            {
                            Random rnd = new Random();
                            int bestval = 0;                        
                            int count = 0;
                            int bestposition = 0;
                            if (possible_moves(board, "black", false, -1, false).Count > 0)
                                {
            
                                foreach (square[,] pos in possible_moves(board, "black", false, -1, false))
                                    {
                                    int this_min = min.minimax_iterative(pos, 7, false, "white", -9999, 9999);
                                    
                                    if (this_min < bestval)
                                        {
                                        bestposition = count;
                                        bestval = this_min;
                                        }
                                    count++;
                                    }
                                }
                            else
                                {
                                board = possible_moves(board, "black", false, -1, false)[rnd.Next(possible_moves(board, "black", false, -1, false).Count)];
                                }
                            board = possible_moves(board, "black", false, -1, false)[bestposition];
                            
                            turn_colour = "white";
                            update_images();
                            }
                        update_images();
                        click_phase = 1;
                        click_1_valx = 0;
                        click_1_valy = 0;
                        click_2_valx = 0;
                        click_2_valy = 0;


                        }

                    }
                }
            }
     

        public void mouse_over( object sender, EventArgs e )
            {

            if (click_phase == 2 && ( sender as PictureBox ).Image == white_empty)
                {
                click_2_valx = Convert.ToInt16(Convert.ToString(Convert.ToString(( sender as PictureBox ).Tag)[0]));
                click_2_valy = Convert.ToInt16(Convert.ToString(Convert.ToString(( sender as PictureBox ).Tag)[1]));
                square[,] test_board = new_board();
                set_board_equal_to(board, test_board);

                test_board[click_2_valx, click_2_valy].square_state = test_board[click_1_valx, click_1_valy].square_state;
                test_board[click_1_valx, click_1_valy].square_state = "non";

                foreach (square[,] sq in possible_moves(board, turn_colour, false, -1, true))
                    {
                    bool does_equal = true;
                    for (int x=0; x<8; x++)
                        {
                        for (int y=0; y<8; y++)
                            {
                            if (sq[x, y].square_state == turn_colour)
                                {

                                if (sq[x, y].square_state != test_board[x, y].square_state)
                                    {
                                    does_equal = false;

                                    }
                                }
                            }
                        }
                    if (does_equal == true)
                        {
                        ( sender as PictureBox ).Image = white_blank_selected;

                        }
                    }


                }
            }

        public void mouse_off( object sender, EventArgs e )
            {
            if (( sender as PictureBox ).Image ==white_blank_selected)
                {
                ( sender as PictureBox ).Image = white_empty;
                }


            }
        #endregion
        //these deal with clicks and shit. the normal minimax is here under square click

        public List<square[,]> possible_moves( square[,] bd, string turn, bool take, int numval, bool is_human )
            {
            int x;
            int y;
            square[,] reset = new_board();
            set_board_equal_to(bd, reset);
            square[,] this_move = new_board();
            set_board_equal_to(bd, this_move);
            List<square[,]> valuelist = new List<square[,]>();
            List<square[,]> compulsory_list = new List<square[,]>();
            bool can_take = false;
            bool must_take = false;
            if (take == true)
                {
                must_take = true;
                }


            for (int yco = 0; yco <8; yco++)
                {
                for (int xco = 0; xco <8; xco++)
                    {

                    x = this_move[xco, yco].xcoord;
                    y = this_move[xco, yco].ycoord;
                    bool[] free = new bool[4];
                    can_take = false;
                    bool[] compulsory = new bool[4];

                    #region find possible coords
                    if (numval == -1 || this_move[x, y].num_index == numval)
                        {

                        if (this_move[xco, yco].square_state != "non")
                            {
                            if (bd[x, y].square_state == "white" && turn =="white")
                                {
                                try
                                    {
                                    if (bd[x - 1, y + 1].square_state == "non")
                                        {
                                        free[0] = true;
                                        }
                                    else if (bd[x - 1, y + 1].square_state == "black" && bd[x - 2, y + 2].square_state == "non")
                                        {
                                        free[0] = true;
                                        can_take = true;
                                        compulsory[0] = true;
                                        }
                                    else
                                        {
                                        free[0] = false;
                                        }

                                    }
                                catch (IndexOutOfRangeException)
                                    {
                                    free[0] = false;
                                    }
                                try
                                    {
                                    if (bd[x + 1, y + 1].square_state == "non")
                                        {
                                        free[1] = true;
                                        }
                                    else if (bd[x + 1, y + 1].square_state == "black" && bd[x + 2, y + 2].square_state == "non")
                                        {
                                        free[1] = true;
                                        can_take = true;
                                        compulsory[1] = true;
                                        }
                                    else
                                        {
                                        free[1] = false;
                                        }
                                    }
                                catch (IndexOutOfRangeException)
                                    {
                                    free[1] = false;
                                    }
                                }
                            if (bd[x, y].square_state == "black" && turn == "black")
                                {
                                try
                                    {
                                    if (bd[x - 1, y - 1].square_state == "non")
                                        {
                                        free[2] = true;
                                        }
                                    else if (bd[x - 1, y - 1].square_state == "white" && bd[x - 2, y - 2].square_state == "non")
                                        {
                                        free[2] = true;
                                        can_take = true;
                                        compulsory[2] = true;
                                        }
                                    else
                                        {
                                        free[2] = false;
                                        }
                                    }
                                catch (IndexOutOfRangeException)
                                    {
                                    free[2] = false;
                                    }
                                try
                                    {
                                    if (bd[x + 1, y - 1].square_state == "non")
                                        {
                                        free[3] = true;
                                        }
                                    else if (bd[x + 1, y - 1].square_state == "white" && bd[x + 2, y - 2].square_state == "non")
                                        {
                                        free[3] = true;
                                        can_take = true;
                                        compulsory[3] = true;
                                        }
                                    else
                                        {
                                        free[3] = false;
                                        }
                                    }
                                catch (IndexOutOfRangeException)
                                    {
                                    free[3] = false;
                                    }
                                }
                            if (bd[x, y].square_state == "white" && bd[x, y].is_king == true && turn == "white")
                                {
                                try
                                    {
                                    if (bd[x - 1, y - 1].square_state == "non")
                                        {
                                        free[2] = true;
                                        }
                                    else if (bd[x - 1, y - 1].square_state == "black" && bd[x - 2, y - 2].square_state == "non")
                                        {
                                        free[2] = true;
                                        can_take = true;
                                        compulsory[2] = true;
                                        }
                                    else
                                        {
                                        free[2] = false;
                                        }
                                    }
                                catch (IndexOutOfRangeException)
                                    {
                                    free[2] = false;
                                    }
                                try
                                    {
                                    if (bd[x + 1, y - 1].square_state == "non")
                                        {
                                        free[3] = true;
                                        }
                                    else if (bd[x + 1, y - 1].square_state == "black" && bd[x + 2, y - 2].square_state == "non")
                                        {
                                        free[3] = true;
                                        can_take = true;
                                        compulsory[3] = true;
                                        }
                                    else
                                        {
                                        free[3] = false;
                                        }
                                    }
                                catch (IndexOutOfRangeException)
                                    {
                                    free[3] = false;
                                    }
                                }
                            if (bd[x, y].square_state == "black" && bd[x, y].is_king == true && turn == "black")
                                {
                                try
                                    {
                                    if (bd[x - 1, y + 1].square_state == "non")
                                        {
                                        free[0] = true;
                                        }
                                    else if (bd[x - 1, y + 1].square_state == "white" && bd[x - 2, y + 2].square_state == "non")
                                        {
                                        free[0] = true;
                                        can_take = true;
                                        compulsory[0] = true;
                                        }
                                    else
                                        {
                                        free[0] = false;
                                        }
                                    }
                                catch (IndexOutOfRangeException)
                                    {
                                    free[0] = false;
                                    }
                                try
                                    {
                                    if (bd[x + 1, y + 1].square_state == "non")
                                        {
                                        free[1] = true;
                                        }
                                    else if (bd[x + 1, y + 1].square_state == "white" && bd[x + 2, y + 2].square_state == "non")
                                        {
                                        free[1] = true;
                                        can_take = true;
                                        compulsory[1] = true;
                                        }
                                    else
                                        {
                                        free[1] = false;
                                        }
                                    }
                                catch (IndexOutOfRangeException)
                                    {
                                    free[1] = false;
                                    }
                                }
                            }
                        }
                    #endregion

                    #region set new board
                    for (int i = 0; i < 4; i++)
                        {
                        if (compulsory[i] == true)
                            {
                            if (i == 0)
                                {

                                this_move[x-2, y+2].square_state = this_move[x, y].square_state;
                                this_move[x-2, y+2].is_king = this_move[x, y].is_king;
                                this_move[x, y].square_state = "non";
                                this_move[x, y].is_king = false;
                                this_move[x-1, y+1].square_state = "non";
                                this_move[x-1, y+1].is_king = false;

                                if (y == 5 && this_move[x-2, y+2].is_king == false)
                                    {
                                    this_move[x-2, y+2].is_king = true;
                                    }

                                square[,] boardtoadd = new_board();
                                set_board_equal_to(this_move, boardtoadd);
                                if (possible_moves(boardtoadd, turn, true, this_move[x-2, y+2].num_index, false).Count != 0 && is_human == false)
                                    {
                                    foreach (square[,] nextboard in possible_moves(boardtoadd, turn, true, this_move[x-2, y+2].num_index, false))
                                        {
                                        compulsory_list.Add(nextboard);
                                        }
                                    }
                                else
                                    {
                                    set_board_equal_to(this_move, boardtoadd);

                                    compulsory_list.Add(boardtoadd);
                                    }
                                must_take = true;
                                set_board_equal_to(reset, this_move);

                                }
                            if (i == 1)
                                {
                                this_move[x+2, y+2].square_state = this_move[x, y].square_state;
                                this_move[x+2, y+2].is_king = this_move[x, y].is_king;
                                this_move[x, y].square_state = "non";
                                this_move[x, y].is_king = false;
                                this_move[x+1, y+1].square_state = "non";
                                this_move[x+1, y+1].is_king = false;

                                if (y == 5 && this_move[x+2, y+2].is_king == false)
                                    {
                                    this_move[x+2, y+2].is_king = true;
                                    }
                                square[,] boardtoadd = new_board();
                                set_board_equal_to(this_move, boardtoadd);
                                if (possible_moves(boardtoadd, turn, true, this_move[x+2, y+2].num_index, false).Count != 0 && is_human == false)
                                    {
                                    foreach (square[,] nextboard in possible_moves(boardtoadd, turn, true, this_move[x+2, y+2].num_index, false))
                                        {
                                        compulsory_list.Add(nextboard);
                                        }
                                    }
                                else
                                    {
                                    set_board_equal_to(this_move, boardtoadd);

                                    compulsory_list.Add(boardtoadd);
                                    }
                                must_take = true;
                                set_board_equal_to(reset, this_move);

                                }
                            if (i == 2)
                                {
                                this_move[x-2, y-2].square_state = this_move[x, y].square_state;
                                this_move[x-2, y-2].is_king = this_move[x, y].is_king;
                                this_move[x, y].square_state = "non";
                                this_move[x, y].is_king = false;
                                this_move[x-1, y-1].square_state = "non";
                                this_move[x-1, y-1].is_king = false;

                                if (y == 2 && this_move[x-2, y-2].is_king == false)
                                    {
                                    this_move[x-2, y-2].is_king = true;
                                    }
                                square[,] boardtoadd = new_board();
                                set_board_equal_to(this_move, boardtoadd);
                                if (possible_moves(boardtoadd, turn, true, this_move[x-2, y-2].num_index, false).Count != 0 && is_human == false)
                                    {
                                    foreach (square[,] nextboard in possible_moves(boardtoadd, turn, true, this_move[x-2, y-2].num_index, false))
                                        {
                                        compulsory_list.Add(nextboard);
                                        }
                                    }
                                else
                                    {
                                    set_board_equal_to(this_move, boardtoadd);

                                    compulsory_list.Add(boardtoadd);
                                    }
                                must_take = true;
                                set_board_equal_to(reset, this_move);

                                }
                            if (i == 3)
                                {
                                this_move[x+2, y-2].square_state = this_move[x, y].square_state;
                                this_move[x+2, y-2].is_king = this_move[x, y].is_king;
                                this_move[x, y].square_state = "non";
                                this_move[x, y].is_king = false;
                                this_move[x+1, y-1].square_state = "non";
                                this_move[x+1, y-1].is_king = false;

                                if (y == 2 && this_move[x+2, y-2].is_king == false)
                                    {
                                    this_move[x+2, y-2].is_king = true;
                                    }
                                square[,] boardtoadd = new_board();
                                set_board_equal_to(this_move, boardtoadd);

                                if (possible_moves(boardtoadd, turn, true, this_move[x+2, y-2].num_index, false).Count != 0 && is_human == false)
                                    {
                                    foreach (square[,] nextboard in possible_moves(boardtoadd, turn, true, this_move[x+2, y-2].num_index, false))
                                        {
                                        compulsory_list.Add(nextboard);
                                        }
                                    }
                                else
                                    {
                                    set_board_equal_to(this_move, boardtoadd);

                                    compulsory_list.Add(boardtoadd);
                                    }
                                must_take = true;
                                set_board_equal_to(reset, this_move);

                                }
                            }
                        else if (free[i] == true && can_take == false && must_take == false)
                            {
                            if (i == 0)
                                {

                                this_move[x-1, y+1].square_state = this_move[x, y].square_state;
                                this_move[x-1, y+1].is_king = this_move[x, y].is_king;
                                this_move[x, y].square_state = "non";
                                this_move[x, y].is_king = false;

                                if (y == 6 && this_move[x-1, y+1].is_king == false)
                                    {
                                    this_move[x-1, y+1].is_king = true;
                                    }
                                square[,] boardtoadd = new_board();
                                set_board_equal_to(this_move, boardtoadd);

                                valuelist.Add(boardtoadd);

                                set_board_equal_to(reset, this_move);
                                }
                            if (i == 1)
                                {
                                this_move[x+1, y+1].square_state = this_move[x, y].square_state;
                                this_move[x+1, y+1].is_king = this_move[x, y].is_king;
                                this_move[x, y].square_state = "non";
                                this_move[x, y].is_king = false;

                                if (y == 6 && this_move[x+1, y+1].is_king == false)
                                    {
                                    this_move[x+1, y+1].is_king = true;
                                    }
                                square[,] boardtoadd = new_board();
                                set_board_equal_to(this_move, boardtoadd);
                                valuelist.Add(boardtoadd);

                                set_board_equal_to(reset, this_move);

                                }
                            if (i == 2)
                                {
                                this_move[x-1, y-1].square_state = this_move[x, y].square_state;
                                this_move[x-1, y-1].is_king = this_move[x, y].is_king;
                                this_move[x, y].square_state = "non";
                                this_move[x, y].is_king = false;

                                if (y == 1 && this_move[x-1, y-1].is_king == false)
                                    {
                                    this_move[x-1, y-1].is_king = true;
                                    }
                                square[,] boardtoadd = new_board();
                                set_board_equal_to(this_move, boardtoadd);
                                valuelist.Add(boardtoadd);

                                set_board_equal_to(reset, this_move);
                                }
                            if (i == 3)
                                {
                                this_move[x +1, y - 1].square_state = this_move[x, y].square_state;
                                this_move[x +1, y - 1].is_king = this_move[x, y].is_king;
                                this_move[x, y].square_state = "non";
                                this_move[x, y].is_king = false;

                                if (y == 1 && this_move[x+1, y-1].is_king == false)
                                    {
                                    this_move[x+1, y-1].is_king = true;
                                    }
                                square[,] boardtoadd = new_board();
                                set_board_equal_to(this_move, boardtoadd);
                                valuelist.Add(boardtoadd);

                                set_board_equal_to(reset, this_move);
                                }


                            }



                        }
                    #endregion

                    }

                }
            if (must_take == false)
                {
                return valuelist;
                }
            else
                {

                return compulsory_list;
                }
            }
        /*i am phisically ashamed by this method, as it is botched as shit, with about 200 too many if statements but it returns a list of
         boards that can legaly happen as a result of the current state. it is horrific but im lazy and cba to fix it. it works*/

        public void initialize()
            {
            int count = 0;
            for (int y = 0; y < 8; y++)
                {
                for (int x = 0; x < 8; x++)
                    {
                    board_images[count] = new PictureBox();
                    board_images[count].Tag = ( Convert.ToString(x)+Convert.ToString(y) );
                    board[x, y] = new square();
                    board[x, y].num_index = count;
                    count++;
                    board[x, y].xcoord = x;
                    board[x, y].ycoord = y;
                    }
                }
            /*board[4, 4].square_state = "white";
            board[4, 4].is_king = true;
            board[0, 0].square_state = "black";
            board[0, 0].is_king = true;
            board[3,3 ].square_state = "black";*/

           
            for (int x = 0; x < 7; x = x + 2)
                {
                board[x, 0].square_state = "white";

                board[x, 2].square_state = "white";

                board[x, 6].square_state = "black";

                board[x + 1, 1].square_state = "white";

                board[x + 1, 5].square_state = "black";

                board[x + 1, 7].square_state = "black";

                }
            
            count = 0;
            for (int y = 600; y > 0; y = y - 85)
                {

                for (int x = 30; x < 700; x = x + 85)
                    {


                    board_images[count].Size = new Size(85, 85);
                    board_images[count].Location = new Point(x, y);
                    board_images[count].Click += new EventHandler(square_click);
                    board_images[count].MouseEnter += new EventHandler(mouse_over);
                    board_images[count].MouseLeave += new EventHandler(mouse_off);
                    this.Controls.Add(board_images[count]);

                    count++;
                    }
                }
            }//fills a board array with initial conditions

        private void button1_Click( object sender, EventArgs e )
            {
            minimax testmin = new minimax();
            testmin.get_currentgen();
            testmin.mutater(testmin.current_values);
            training_match(board);
            update_images();
            }//commennce trainig

        }



    public class functions
        {
         public void set_board_equal_to( square[,] original, square[,] copy )
            {
            for (int x=0; x<8; x++)
                {
                for (int y=0; y<8; y++)
                    {
                    copy[x, y].is_king = original[x, y].is_king;
                    copy[x, y].num_index = original[x, y].num_index;
                    copy[x, y].square_state = original[x, y].square_state;
                    copy[x, y].xcoord = original[x, y].xcoord;
                    copy[x, y].ycoord = original[x, y].ycoord;
                    }
                }
            }

         public List<square[,]> possible_moves( square[,] bd, string turn, bool take, int numval, bool is_human )
             {
             int x;
             int y;
             square[,] reset = new_board();
             set_board_equal_to(bd, reset);
             square[,] this_move = new_board();
             set_board_equal_to(bd, this_move);
             List<square[,]> valuelist = new List<square[,]>();
             List<square[,]> compulsory_list = new List<square[,]>();
             bool can_take = false;
             bool must_take = false;
             if (take == true)
                 {
                 must_take = true;
                 }


             for (int yco = 0; yco <8; yco++)
                 {
                 for (int xco = 0; xco <8; xco++)
                     {

                     x = this_move[xco, yco].xcoord;
                     y = this_move[xco, yco].ycoord;
                     bool[] free = new bool[4];
                     can_take = false;
                     bool[] compulsory = new bool[4];

                     #region find possible coords
                     if (numval == -1 || this_move[x, y].num_index == numval)
                         {

                         if (this_move[xco, yco].square_state != "non")
                             {
                             if (bd[x, y].square_state == "white" && turn =="white")
                                 {
                                 try
                                     {
                                     if (bd[x - 1, y + 1].square_state == "non")
                                         {
                                         free[0] = true;
                                         }
                                     else if (bd[x - 1, y + 1].square_state == "black" && bd[x - 2, y + 2].square_state == "non")
                                         {
                                         free[0] = true;
                                         can_take = true;
                                         compulsory[0] = true;
                                         }
                                     else
                                         {
                                         free[0] = false;
                                         }

                                     }
                                 catch (IndexOutOfRangeException)
                                     {
                                     free[0] = false;
                                     }
                                 try
                                     {
                                     if (bd[x + 1, y + 1].square_state == "non")
                                         {
                                         free[1] = true;
                                         }
                                     else if (bd[x + 1, y + 1].square_state == "black" && bd[x + 2, y + 2].square_state == "non")
                                         {
                                         free[1] = true;
                                         can_take = true;
                                         compulsory[1] = true;
                                         }
                                     else
                                         {
                                         free[1] = false;
                                         }
                                     }
                                 catch (IndexOutOfRangeException)
                                     {
                                     free[1] = false;
                                     }
                                 }
                             if (bd[x, y].square_state == "black" && turn == "black")
                                 {
                                 try
                                     {
                                     if (bd[x - 1, y - 1].square_state == "non")
                                         {
                                         free[2] = true;
                                         }
                                     else if (bd[x - 1, y - 1].square_state == "white" && bd[x - 2, y - 2].square_state == "non")
                                         {
                                         free[2] = true;
                                         can_take = true;
                                         compulsory[2] = true;
                                         }
                                     else
                                         {
                                         free[2] = false;
                                         }
                                     }
                                 catch (IndexOutOfRangeException)
                                     {
                                     free[2] = false;
                                     }
                                 try
                                     {
                                     if (bd[x + 1, y - 1].square_state == "non")
                                         {
                                         free[3] = true;
                                         }
                                     else if (bd[x + 1, y - 1].square_state == "white" && bd[x + 2, y - 2].square_state == "non")
                                         {
                                         free[3] = true;
                                         can_take = true;
                                         compulsory[3] = true;
                                         }
                                     else
                                         {
                                         free[3] = false;
                                         }
                                     }
                                 catch (IndexOutOfRangeException)
                                     {
                                     free[3] = false;
                                     }
                                 }
                             if (bd[x, y].square_state == "white" && bd[x, y].is_king == true && turn == "white")
                                 {
                                 try
                                     {
                                     if (bd[x - 1, y - 1].square_state == "non")
                                         {
                                         free[2] = true;
                                         }
                                     else if (bd[x - 1, y - 1].square_state == "black" && bd[x - 2, y - 2].square_state == "non")
                                         {
                                         free[2] = true;
                                         can_take = true;
                                         compulsory[2] = true;
                                         }
                                     else
                                         {
                                         free[2] = false;
                                         }
                                     }
                                 catch (IndexOutOfRangeException)
                                     {
                                     free[2] = false;
                                     }
                                 try
                                     {
                                     if (bd[x + 1, y - 1].square_state == "non")
                                         {
                                         free[3] = true;
                                         }
                                     else if (bd[x + 1, y - 1].square_state == "black" && bd[x + 2, y - 2].square_state == "non")
                                         {
                                         free[3] = true;
                                         can_take = true;
                                         compulsory[3] = true;
                                         }
                                     else
                                         {
                                         free[3] = false;
                                         }
                                     }
                                 catch (IndexOutOfRangeException)
                                     {
                                     free[3] = false;
                                     }
                                 }
                             if (bd[x, y].square_state == "black" && bd[x, y].is_king == true && turn == "black")
                                 {
                                 try
                                     {
                                     if (bd[x - 1, y + 1].square_state == "non")
                                         {
                                         free[0] = true;
                                         }
                                     else if (bd[x - 1, y + 1].square_state == "white" && bd[x - 2, y + 2].square_state == "non")
                                         {
                                         free[0] = true;
                                         can_take = true;
                                         compulsory[0] = true;
                                         }
                                     else
                                         {
                                         free[0] = false;
                                         }
                                     }
                                 catch (IndexOutOfRangeException)
                                     {
                                     free[0] = false;
                                     }
                                 try
                                     {
                                     if (bd[x + 1, y + 1].square_state == "non")
                                         {
                                         free[1] = true;
                                         }
                                     else if (bd[x + 1, y + 1].square_state == "white" && bd[x + 2, y + 2].square_state == "non")
                                         {
                                         free[1] = true;
                                         can_take = true;
                                         compulsory[1] = true;
                                         }
                                     else
                                         {
                                         free[1] = false;
                                         }
                                     }
                                 catch (IndexOutOfRangeException)
                                     {
                                     free[1] = false;
                                     }
                                 }
                             }
                         }
                     #endregion

                     #region set new board
                     for (int i = 0; i < 4; i++)
                         {
                         if (compulsory[i] == true)
                             {
                             if (i == 0)
                                 {

                                 this_move[x-2, y+2].square_state = this_move[x, y].square_state;
                                 this_move[x-2, y+2].is_king = this_move[x, y].is_king;
                                 this_move[x, y].square_state = "non";
                                 this_move[x, y].is_king = false;
                                 this_move[x-1, y+1].square_state = "non";
                                 this_move[x-1, y+1].is_king = false;

                                 if (y == 5 && this_move[x-2, y+2].is_king == false)
                                     {
                                     this_move[x-2, y+2].is_king = true;
                                     }

                                 square[,] boardtoadd = new_board();
                                 set_board_equal_to(this_move, boardtoadd);
                                 if (possible_moves(boardtoadd, turn, true, this_move[x-2, y+2].num_index, false).Count != 0 && is_human == false)
                                     {
                                     foreach (square[,] nextboard in possible_moves(boardtoadd, turn, true, this_move[x-2, y+2].num_index, false))
                                         {
                                         compulsory_list.Add(nextboard);
                                         }
                                     }
                                 else
                                     {
                                     set_board_equal_to(this_move, boardtoadd);

                                     compulsory_list.Add(boardtoadd);
                                     }
                                 must_take = true;
                                 set_board_equal_to(reset, this_move);

                                 }
                             if (i == 1)
                                 {
                                 this_move[x+2, y+2].square_state = this_move[x, y].square_state;
                                 this_move[x+2, y+2].is_king = this_move[x, y].is_king;
                                 this_move[x, y].square_state = "non";
                                 this_move[x, y].is_king = false;
                                 this_move[x+1, y+1].square_state = "non";
                                 this_move[x+1, y+1].is_king = false;

                                 if (y == 5 && this_move[x+2, y+2].is_king == false)
                                     {
                                     this_move[x+2, y+2].is_king = true;
                                     }
                                 square[,] boardtoadd = new_board();
                                 set_board_equal_to(this_move, boardtoadd);
                                 if (possible_moves(boardtoadd, turn, true, this_move[x+2, y+2].num_index, false).Count != 0 && is_human == false)
                                     {
                                     foreach (square[,] nextboard in possible_moves(boardtoadd, turn, true, this_move[x+2, y+2].num_index, false))
                                         {
                                         compulsory_list.Add(nextboard);
                                         }
                                     }
                                 else
                                     {
                                     set_board_equal_to(this_move, boardtoadd);

                                     compulsory_list.Add(boardtoadd);
                                     }
                                 must_take = true;
                                 set_board_equal_to(reset, this_move);

                                 }
                             if (i == 2)
                                 {
                                 this_move[x-2, y-2].square_state = this_move[x, y].square_state;
                                 this_move[x-2, y-2].is_king = this_move[x, y].is_king;
                                 this_move[x, y].square_state = "non";
                                 this_move[x, y].is_king = false;
                                 this_move[x-1, y-1].square_state = "non";
                                 this_move[x-1, y-1].is_king = false;

                                 if (y == 2 && this_move[x-2, y-2].is_king == false)
                                     {
                                     this_move[x-2, y-2].is_king = true;
                                     }
                                 square[,] boardtoadd = new_board();
                                 set_board_equal_to(this_move, boardtoadd);
                                 if (possible_moves(boardtoadd, turn, true, this_move[x-2, y-2].num_index, false).Count != 0 && is_human == false)
                                     {
                                     foreach (square[,] nextboard in possible_moves(boardtoadd, turn, true, this_move[x-2, y-2].num_index, false))
                                         {
                                         compulsory_list.Add(nextboard);
                                         }
                                     }
                                 else
                                     {
                                     set_board_equal_to(this_move, boardtoadd);

                                     compulsory_list.Add(boardtoadd);
                                     }
                                 must_take = true;
                                 set_board_equal_to(reset, this_move);

                                 }
                             if (i == 3)
                                 {
                                 this_move[x+2, y-2].square_state = this_move[x, y].square_state;
                                 this_move[x+2, y-2].is_king = this_move[x, y].is_king;
                                 this_move[x, y].square_state = "non";
                                 this_move[x, y].is_king = false;
                                 this_move[x+1, y-1].square_state = "non";
                                 this_move[x+1, y-1].is_king = false;

                                 if (y == 2 && this_move[x+2, y-2].is_king == false)
                                     {
                                     this_move[x+2, y-2].is_king = true;
                                     }
                                 square[,] boardtoadd = new_board();
                                 set_board_equal_to(this_move, boardtoadd);

                                 if (possible_moves(boardtoadd, turn, true, this_move[x+2, y-2].num_index, false).Count != 0 && is_human == false)
                                     {
                                     foreach (square[,] nextboard in possible_moves(boardtoadd, turn, true, this_move[x+2, y-2].num_index, false))
                                         {
                                         compulsory_list.Add(nextboard);
                                         }
                                     }
                                 else
                                     {
                                     set_board_equal_to(this_move, boardtoadd);

                                     compulsory_list.Add(boardtoadd);
                                     }
                                 must_take = true;
                                 set_board_equal_to(reset, this_move);

                                 }
                             }
                         else if (free[i] == true && can_take == false && must_take == false)
                             {
                             if (i == 0)
                                 {

                                 this_move[x-1, y+1].square_state = this_move[x, y].square_state;
                                 this_move[x-1, y+1].is_king = this_move[x, y].is_king;
                                 this_move[x, y].square_state = "non";
                                 this_move[x, y].is_king = false;

                                 if (y == 6 && this_move[x-1, y+1].is_king == false)
                                     {
                                     this_move[x-1, y+1].is_king = true;
                                     }
                                 square[,] boardtoadd = new_board();
                                 set_board_equal_to(this_move, boardtoadd);

                                 valuelist.Add(boardtoadd);

                                 set_board_equal_to(reset, this_move);
                                 }
                             if (i == 1)
                                 {
                                 this_move[x+1, y+1].square_state = this_move[x, y].square_state;
                                 this_move[x+1, y+1].is_king = this_move[x, y].is_king;
                                 this_move[x, y].square_state = "non";
                                 this_move[x, y].is_king = false;

                                 if (y == 6 && this_move[x+1, y+1].is_king == false)
                                     {
                                     this_move[x+1, y+1].is_king = true;
                                     }
                                 square[,] boardtoadd = new_board();
                                 set_board_equal_to(this_move, boardtoadd);
                                 valuelist.Add(boardtoadd);

                                 set_board_equal_to(reset, this_move);

                                 }
                             if (i == 2)
                                 {
                                 this_move[x-1, y-1].square_state = this_move[x, y].square_state;
                                 this_move[x-1, y-1].is_king = this_move[x, y].is_king;
                                 this_move[x, y].square_state = "non";
                                 this_move[x, y].is_king = false;

                                 if (y == 1 && this_move[x-1, y-1].is_king == false)
                                     {
                                     this_move[x-1, y-1].is_king = true;
                                     }
                                 square[,] boardtoadd = new_board();
                                 set_board_equal_to(this_move, boardtoadd);
                                 valuelist.Add(boardtoadd);

                                 set_board_equal_to(reset, this_move);
                                 }
                             if (i == 3)
                                 {
                                 this_move[x +1, y - 1].square_state = this_move[x, y].square_state;
                                 this_move[x +1, y - 1].is_king = this_move[x, y].is_king;
                                 this_move[x, y].square_state = "non";
                                 this_move[x, y].is_king = false;

                                 if (y == 1 && this_move[x+1, y-1].is_king == false)
                                     {
                                     this_move[x+1, y-1].is_king = true;
                                     }
                                 square[,] boardtoadd = new_board();
                                 set_board_equal_to(this_move, boardtoadd);
                                 valuelist.Add(boardtoadd);

                                 set_board_equal_to(reset, this_move);
                                 }


                             }



                         }
                     #endregion

                     }

                 }
             if (must_take == false)
                 {
                 return valuelist;
                 }
             else
                 {

                 return compulsory_list;
                 }
             }

         public bool is_over_quiery( square[,] bd  )
            {
            if (possible_moves(bd, "white", false, -1, true).Count == 0 || possible_moves(bd, "black", false, -1, true).Count == 0)
                {
                return true;
                }
            else
                return false;
            }

         public square[,] new_board()
             {
             square[,] returnboard = new square[8, 8];
             int count = 0;
             for (int x=0; x<8; x++)
                 {
                 for (int y=0; y<8; y++)
                     {
                     returnboard[x, y] = new square();
                     returnboard[x, y].xcoord = x;
                     returnboard[x, y].ycoord = y;
                     returnboard[x, y].num_index = count;
                     count++;
                     }
                 }
             return returnboard;
             }
        }//these are repeats of some useful function from form1



    public class square
        {
        public string square_state = "non";
        public bool is_king = false;
        public int xcoord;
        public int ycoord;
        public int num_index;
        }//square properties



    public class minimax
        {
        functions func = new functions();
        public int[] current_values= new int[6];
        public int[] next_gen_values= new int[6];
        string turn_colour;

        #region basic minimax
        public int board_value( square[,] bd )
            {
            //looks at each piece on the board and values it. values are reversed for black
            int score = 0;
            foreach (square sq in bd)
                {
                int x = sq.xcoord;
                int y = sq.ycoord;
                if (sq.square_state =="black")
                    {
                    score -= 30;//per piece
                    if (sq.is_king== true)
                        {
                        score -= 10;//per king
                        }
                    if (x == 0 || x == 7 || y == 7)
                        {
                        score -= 2;//protected at the sides
                        }
                    if (sq.is_king != true)
                        {
                        score -= ( ( 7-y )*2 );
                        }//depth

                    try{
                        
                    if (bd[x+1, y+1].square_state == "black")
                        {
                        score -= 2;//protected by friendly
                        }
                    else
                        {
                        try
                            {
                            if (bd[x-1, y-1].square_state == "white")
                                {
                                score += 10;
                                }
                            }
                        catch (IndexOutOfRangeException)
                            {
                            }
                        }
                    if (bd[x-1, y+1].square_state == "black")
                        {
                        score -= 2;
                        }
                    else
                        {
                        try
                            {
                            if (bd[x+1, y-1].square_state == "white")
                                {
                                score += 10;
                                }
                            }
                        catch (IndexOutOfRangeException)
                            {
                            }
                        }
                    if (bd[x+1, y+1].square_state == "black" && bd[x-1, y+1].square_state == "black")
                        {
                        score -= 2;
                        }
                    }
                    catch (IndexOutOfRangeException){}
                    }

                else if (sq.square_state == "white")
                    {
                    score += 30;
                    if (sq.is_king== true)
                        {
                        score += 10;
                        }
                    if (x == 0 || x == 7 || y == 0)
                        {
                        score += 2;
                        }
                    if (sq.is_king != true)
                        {

                        score += ( y*2 );
                        }
                    try
                        {

                        if (bd[x+1, y-1].square_state == "white")
                            {
                            score += 2;
                            }
                        else
                            {
                            try
                                {
                                if (bd[x-1, y+1].square_state == "black")
                                    {
                                    score -= 10;
                                    }
                                }
                            catch (IndexOutOfRangeException)
                                {
                                }
                            }
                        if (bd[x-1, y-1].square_state == "white")
                            {
                            score += 2;
                            }
                        else
                            {
                            try
                                {
                                if (bd[x+1, y+1].square_state == "black")
                                    {
                                    score -= 10;
                                    }
                                }
                            catch (IndexOutOfRangeException)
                                {
                                }
                            }
                        if (bd[x+1, y-1].square_state == "white" && bd[x-1, y-1].square_state == "white")
                            {
                            score += 2;
                            }
                        }
                    catch (IndexOutOfRangeException){}
                    }
                }
            return score;
            }

        public int minimax_run(square[,] current_minimax,int depth, bool maxiplayer, string turn ,int alpha, int beta)
            {
            int a = alpha;
            int b = beta;
            if (depth == 0 || func.is_over_quiery(current_minimax) == true) //termial node shit          
                {
                int retval = board_value(current_minimax);
                return retval;
                }



            if (turn=="white")
                {
                 turn_colour = "black";                
                                }
            else
                {
                 turn_colour = "white";

                }
            if (maxiplayer == true)
                {
                int bestval = -9999;
                
                foreach (square[,] pos in func.possible_moves(current_minimax, turn, false, -1, false))//looks at all possible moves
                    {

                    int max = minimax_run(pos, depth-1, !maxiplayer, turn_colour, a, b);   //runs minimax on it, returning the value               

                    bestval = Math.Max(bestval, max) + (depth *10);//stores that. the depth *10 hopefully stops it suiciding
                    a = Math.Max(bestval, alpha);//this is alpha beta. i cant fucking explain it by comments. google it
                    if (beta <= a)
                        {
                        
                        break;
                        }
                    }
                return bestval;
                }
            else
                {
                int bestval = 9999;
                foreach (square[,] pos in func.possible_moves(current_minimax, turn, false, -1, false))
                    {
                    int min = minimax_run(pos, depth-1, !maxiplayer, turn_colour, a, b);

                    bestval = Math.Min(bestval, min) - ( depth *10 );
                    b = Math.Min(bestval, beta);
                    if (b <= alpha)
                        {
                        
                        break;
                        }
                   }
                return bestval;
                }
            }
        #endregion

        #region attempt at better minimax
        public int minimax_iterative( square[,] current_minimax, int depth, bool maxiplayer, string turn, int alpha, int beta )
            {
            int a = alpha;
            int b = beta;
            var this_depth = new Dictionary<int,square[,]>();
            
            if (depth == 0 || func.is_over_quiery(current_minimax) == true)
                {
                int retval = board_value(current_minimax);
                return retval;
                

                }
            if (turn=="white")
                {
                turn_colour = "black";
                }
            else
                {
                turn_colour = "white";

                }
            
            foreach (square[,] pos in func.possible_moves(current_minimax, turn, false, -1, false))
                    {
                    int value = board_value(pos);
                bool placed = false;
                while (placed == false){
                    try
                        {
                        placed = true;
                        this_depth.Add(value, pos);
                        }
                    catch(ArgumentException)
                        {
                        value++;
                        placed = false;
                        }
                        }
                    }
                    var ordered_depth = this_depth.Keys.ToList();
                    ordered_depth.Sort();

            if (maxiplayer == true)
                {
                int bestval = -9999;
                ordered_depth.Reverse();

                foreach (int val in ordered_depth)
                    {
                    int max = minimax_iterative(this_depth[val], depth-1, !maxiplayer, turn_colour, a, b);

                    bestval = Math.Max(bestval, max) + ( depth *10 );
                    a = Math.Max(bestval, alpha);
                    if (beta <= a)
                        {
                        break;
                        }
                    }


                return bestval;
                }
            else
                {
                
                int bestval = 9999;
                foreach (int val in ordered_depth)
                    {
                    int min = minimax_run(this_depth[val], depth-1, !maxiplayer, turn_colour, a, b);

                    bestval = Math.Min(bestval, min) - ( depth *10 );
                    b = Math.Min(bestval, beta);
                    if (b <= alpha)
                        {
                        break;
                        }
                    }
                return bestval;
                }
            }
        #endregion

        #region minimax with training
        public int minimax_training( square[,] current_minimax, int depth, bool maxiplayer, string turn, int alpha, int beta, bool is_baby)
            {
            int a = alpha;
            int b = beta;
            if (depth == 0 || func.is_over_quiery(current_minimax) == true)
                {
                int retval;
               retval = board_value_variable(current_minimax,is_baby);//gets either the new or old gen board values
                return retval;

                }
            if (turn=="white")
                {
                turn_colour = "black";
                }
            else
                {
                turn_colour = "white";

                }
            
            if (maxiplayer == true)
                {
                int bestval = -9999;

                foreach (square[,] pos in func.possible_moves(current_minimax, turn, false, -1, false))
                    {

                    int max = minimax_training(pos, depth-1, !maxiplayer, turn_colour, a, b,!is_baby);

                    bestval = Math.Max(bestval, max) + ( depth *10 );
                    a = Math.Max(bestval, alpha);
                    if (beta <= a)
                        {
                        break;
                        }
                    }
                return bestval;
                }
            else
                {
                int bestval = 9999;
                foreach (square[,] pos in func.possible_moves(current_minimax, turn, false, -1, false))
                    {
                    int min = minimax_training(pos, depth-1, !maxiplayer, turn_colour, a, b,!is_baby);

                    bestval = Math.Min(bestval, min) - ( depth *10 );
                    b = Math.Min(bestval, beta);
                    if (b <= alpha)
                        {
                        break;
                        }
                    }
                return bestval;
                }
            }

        public int board_value_variable(square[,] bd, bool is_baby )
            {
            int[] needed_values;
            if (is_baby == true)
                {
                needed_values = next_gen_values;
                }
            else
                {
                needed_values = current_values;
                }
            int score = 0;
            foreach (square sq in bd)
                {
                int x = sq.xcoord;
                int y = sq.ycoord;
                if (sq.square_state =="black")
                    {
                    score -= needed_values[0];//per piece
                    if (sq.is_king== true)
                        {
                        score -= needed_values[1];//per king
                        }
                    if (x == 0 || x == 7 || y == 7)
                        {
                        score -= needed_values[2];//protected at the sides
                        }
                    if (sq.is_king != true)
                        {
                        score -= ( ( 7-y )* needed_values[3] );
                        }//depth

                    try
                        {

                        if (bd[x+1, y+1].square_state == "black")
                            {
                            score -= needed_values[4];//protected by friendly
                            }
                        else
                            {
                            try
                                {
                                if (bd[x-1, y-1].square_state == "white")
                                    {
                                    score -= needed_values[5];
                                    }
                                }
                            catch (IndexOutOfRangeException)
                                {
                                }
                            }
                        if (bd[x-1, y+1].square_state == "black")
                            {
                            score -= needed_values[4];
                            }
                        else
                            {
                            try
                                {
                                if (bd[x+1, y-1].square_state == "white")
                                    {
                                    score -= needed_values[5];
                                    }
                                }
                            catch (IndexOutOfRangeException)
                                {
                                }
                            }
                        if (bd[x+1, y+1].square_state == "black" && bd[x-1, y+1].square_state == "black")
                            {
                            score -= needed_values[4];
                            }
                        }
                    catch (IndexOutOfRangeException)
                        {
                        }
                    }

                else if (sq.square_state == "white")
                    {
                    score += needed_values[0];
                    if (sq.is_king== true)
                        {
                        score += needed_values[1];
                        }
                    if (x == 0 || x == 7 || y == 0)
                        {
                        score += needed_values[2];
                        }
                    if (sq.is_king != true)
                        {

                        score += ( y*needed_values[3] );
                        }
                    try
                        {

                        if (bd[x+1, y-1].square_state == "white")
                            {
                            score += needed_values[4];
                            }
                        else
                            {
                            try
                                {
                                if (bd[x-1, y+1].square_state == "black")
                                    {
                                    score += needed_values[5];
                                    }
                                }
                            catch (IndexOutOfRangeException)
                                {
                                }
                            }
                        if (bd[x-1, y-1].square_state == "white")
                            {
                            score += needed_values[4];
                            }
                        else
                            {
                            try
                                {
                                if (bd[x+1, y+1].square_state == "black")
                                    {
                                    score += needed_values[5];
                                    }
                                }
                            catch (IndexOutOfRangeException)
                                {
                                }
                            }
                        if (bd[x+1, y-1].square_state == "white" && bd[x-1, y-1].square_state == "white")
                            {
                            score += needed_values[4];
                            }
                        }
                    catch (IndexOutOfRangeException)
                        {
                        }
                    }
                }
            return score;
            }//same as normal board_value, expets it loads new values 

        public void get_currentgen()
            {
            string genval = File.ReadAllText("winning generations.txt");
            string[] input = genval.Split('$');
            string thisgen = input.Last();
            string[] strvalues = (thisgen.Split('&')).Last().Split(',');
            
            for (int i = 0; i<5; i++)
                {
                current_values[i] = Convert.ToInt16(strvalues[i]);
                }
            current_values[5] = Convert.ToInt16(strvalues[5]) * -1;
            }//unpacks current gen from file

        public void mutater(int[] origin_of_the_species)
            {
            int max_factor = 100;
            Random rand = new Random();
            //bool inverter = rand.Next
            int feature = rand.Next(6);
            int evolve_factor = rand.Next(50,max_factor +1);

            for (int i = 0; i <6; i++)
                {
                if (i == feature)
                    {
                    next_gen_values[i] = origin_of_the_species[i] + ( evolve_factor - (max_factor/2) ); 
                    }
                else
                    {
                    next_gen_values[i] = origin_of_the_species[i];
                    }
                }

            }//mutates current gen into a copy
        #endregion
        }
        }
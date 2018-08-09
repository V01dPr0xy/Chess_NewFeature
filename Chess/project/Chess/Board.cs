using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess
{
    public class ChessBoard
    {
        private static int[] pieceWeights = { 1, 3, 4, 5, 7, 20 };

        public position_t BishopPos;

        public piece_t[][] Grid { get; private set; }
        public Dictionary<Player, position_t> Kings { get; private set; }
        public Dictionary<Player, List<position_t>> Pieces { get; private set; }
        public Dictionary<Player, position_t> LastMove { get; private set; }

        public ChessBoard()
        {
            // init blank board grid
            Grid = new piece_t[8][];
            for (int i = 0; i < 8; i++)
            {
                Grid[i] = new piece_t[8];
                for (int j = 0; j < 8; j++)
                    Grid[i][j] = new piece_t(Piece.NONE, Player.WHITE);
            }

            // init last moves
            LastMove = new Dictionary<Player, position_t>();
            LastMove[Player.BLACK] = new position_t();
            LastMove[Player.WHITE] = new position_t();

            // init king positions
            Kings = new Dictionary<Player, position_t>();

            // init piece position lists
            Pieces = new Dictionary<Player, List<position_t>>();
            Pieces.Add(Player.BLACK, new List<position_t>());
            Pieces.Add(Player.WHITE, new List<position_t>());
        }

        public ChessBoard(ChessBoard copy)
        {
            // init piece position lists
            Pieces = new Dictionary<Player, List<position_t>>();
            Pieces.Add(Player.BLACK, new List<position_t>());
            Pieces.Add(Player.WHITE, new List<position_t>());

            // init board grid to copy locations
            Grid = new piece_t[8][];
            for (int i = 0; i < 8; i++)
            {
                Grid[i] = new piece_t[8];
                for (int j = 0; j < 8; j++)
                {
                    Grid[i][j] = new piece_t(copy.Grid[i][j]);

                    // add piece location to list
                    if (Grid[i][j].piece != Piece.NONE)
                        Pieces[Grid[i][j].player].Add(new position_t(j, i));
                }
            }

            // copy last known move
            LastMove = new Dictionary<Player, position_t>();
            LastMove[Player.BLACK] = new position_t(copy.LastMove[Player.BLACK]);
            LastMove[Player.WHITE] = new position_t(copy.LastMove[Player.WHITE]);

            // copy king locations
            Kings = new Dictionary<Player, position_t>();
            Kings[Player.BLACK] = new position_t(copy.Kings[Player.BLACK]);
            Kings[Player.WHITE] = new position_t(copy.Kings[Player.WHITE]);
        }

        /// <summary>
        /// Calculate and return the boards fitness value.
        /// </summary>
        /// <param name="max">Who's side are we viewing from.</param>
        /// <returns>The board fitness value, what else?</returns>
        public int fitness(Player max)
        {
            int fitness = 0;
            int[] blackPieces = { 0, 0, 0, 0, 0, 0 };
            int[] whitePieces = { 0, 0, 0, 0, 0, 0 };
            int blackMoves = 0;
            int whiteMoves = 0;

            // sum up the number of moves and pieces
            foreach (position_t pos in Pieces[Player.BLACK])
            {
                blackMoves += LegalMoveSet.getLegalMove(this, pos).Count;
                blackPieces[(int)Grid[pos.number][pos.letter].piece]++;
            }

            // sum up the number of moves and pieces
            foreach (position_t pos in Pieces[Player.WHITE])
            {
                whiteMoves += LegalMoveSet.getLegalMove(this, pos).Count;
                whitePieces[(int)Grid[pos.number][pos.letter].piece]++;
            }

            // if viewing from black side
            if (max == Player.BLACK)
            {
                // apply weighting to piece counts
                for (int i = 0; i < 6; i++)
                {
                    fitness += pieceWeights[i] * (blackPieces[i] - whitePieces[i]);
                }

                // apply move value
                fitness += (int)(0.5 * (blackMoves - whiteMoves));
            }
            else
            {
                // apply weighting to piece counts
                for (int i = 0; i < 6; i++)
                {
                    fitness += pieceWeights[i] * (whitePieces[i] - blackPieces[i]);
                }

                // apply move value
                fitness += (int)(0.5 * (whiteMoves - blackMoves));
            }

            return fitness;
        }

        public bool SpotTaken(int letter, int number) => (Pieces[Player.WHITE].Where(p => p.letter == letter && p.number == number).Count() == 1);
        public bool SpotTakenByPiece(int letter, int number, Piece piece) => Grid[letter][number].piece == piece;

        public int Mirror(int letter) => ((letter % 2) == 1) ? 6 : 7;

        public void AssignPiece(Piece piece, out int letter, out int number)
        {
            bool invalidPlacement = true;

            //Generate a random letter and number
            do
            {
                Random r = new Random();
                letter = r.Next(0, 8);
                number = 0;
                if ((int)piece == 3)
                {
                    if (BishopPos.letter.IsEven())
                    {
                        bool badLetter = true;
                        do
                        {
                            letter = r.Next(0, 8);
                            if (letter.IsOdd())
                            {
                                badLetter = false;
                            }

                        } while (badLetter);
                    }
                    else if(BishopPos.letter.IsOdd())
                    {
                        bool badLetter = true;
                        do
                        {
                            letter = r.Next(0, 8);
                            if (letter.IsEven())
                            {
                                badLetter = false;
                            }

                        } while (badLetter);
                    }
                    else
                    {
                        invalidPlacement = false;
                    }
                }
                else if ((int)piece == 5)
                {
                    letter = r.Next(1, 7);
                }
                else if ((int)piece == 1)
                {
                    //Get the position of the King
                    int letter_restriction = Kings[Player.WHITE].letter;

                    //Restrict the Rooks
                    bool isInvalid = true;
                    while (isInvalid)
                    {
                        bool rookFound = false;
                        for (int i = 0; i < letter_restriction; i++)
                        {
                            //If the spot isn't taken by a rook, continue
                            if (!SpotTakenByPiece(0, i, Piece.ROOK))
                            {
                                //If the i variable reaches the king's value and the spot isn't taken
                                if (i == letter_restriction - 1)
                                {
                                    letter = r.Next(0, letter_restriction);
                                    isInvalid = false;
                                    break;
                                }
                            }
                            else if (SpotTakenByPiece(0, i, Piece.ROOK))
                            {
                                rookFound = true;
                                break;
                            }
                            //if the spot is taken by a rook, ignore.
                        }

                        if (rookFound)
                        {
                            letter = r.Next(letter_restriction, 8);
                            isInvalid = false;
                        }
                    }
                }

                //Check if the spot is taken
                if (!SpotTaken(letter, number))
                {
                    invalidPlacement = false;
                }


            } while (invalidPlacement);

            //Assign both spots if not taken
            SetPiece(piece, Player.WHITE, letter, number);
            SetPiece(piece, Player.BLACK, letter, Mirror(number));
        }

        public void SetRandomPlacement()
        {
            //Add the pieces via using the SetPiece() method
            //Check for placement, then add:
            Random r = new Random();
            int letter = r.Next(0, 8), number = 0;
            //Pawns

            for (int i = 0; i < 8; i++)
            {
                SetPiece(Piece.PAWN, Player.WHITE, i, 1);
                SetPiece(Piece.PAWN, Player.BLACK, i, 6);
            }

            //Kings
            AssignPiece(Piece.KING, out letter, out number);
            Kings[Player.WHITE] = new position_t(letter, number);
            Kings[Player.BLACK] = new position_t(letter, Mirror(number));

            //Rooks
            AssignPiece(Piece.ROOK, out letter, out number);
            AssignPiece(Piece.ROOK, out letter, out number);

            //Bishops
            AssignPiece(Piece.BISHOP, out letter, out number);
            BishopPos = new position_t(letter, number);
            //Somehow.. get where the bishop was placed. 
            AssignPiece(Piece.BISHOP, out letter, out number);


            //Queens
            AssignPiece(Piece.QUEEN, out letter, out number);


            //Knights
            AssignPiece(Piece.KNIGHT, out letter, out number);
            AssignPiece(Piece.KNIGHT, out letter, out number);

        }

        public void SetInitialPlacement()
        {
            for (int i = 0; i < 8; i++)
            {
                SetPiece(Piece.PAWN, Player.WHITE, i, 1);
                SetPiece(Piece.PAWN, Player.BLACK, i, 6);
            }

            SetPiece(Piece.ROOK, Player.WHITE, 0, 0);
            SetPiece(Piece.ROOK, Player.WHITE, 7, 0);
            SetPiece(Piece.ROOK, Player.BLACK, 0, 7);
            SetPiece(Piece.ROOK, Player.BLACK, 7, 7);

            SetPiece(Piece.KNIGHT, Player.WHITE, 1, 0);
            SetPiece(Piece.KNIGHT, Player.WHITE, 6, 0);
            SetPiece(Piece.KNIGHT, Player.BLACK, 1, 7);
            SetPiece(Piece.KNIGHT, Player.BLACK, 6, 7);

            SetPiece(Piece.BISHOP, Player.WHITE, 2, 0);
            SetPiece(Piece.BISHOP, Player.WHITE, 5, 0);
            SetPiece(Piece.BISHOP, Player.BLACK, 2, 7);
            SetPiece(Piece.BISHOP, Player.BLACK, 5, 7);

            SetPiece(Piece.KING, Player.WHITE, 4, 0);
            SetPiece(Piece.KING, Player.BLACK, 4, 7);
            Kings[Player.WHITE] = new position_t(4, 0);
            Kings[Player.BLACK] = new position_t(4, 7);
            SetPiece(Piece.QUEEN, Player.WHITE, 3, 0);
            SetPiece(Piece.QUEEN, Player.BLACK, 3, 7);
        }

        public void SetPiece(Piece piece, Player player, int letter, int number)
        {
            // set grid values
            Grid[number][letter].piece = piece;
            Grid[number][letter].player = player;

            // add piece to list
            Pieces[player].Add(new position_t(letter, number));

            // update king position
            if (piece == Piece.KING)
            {
                Kings[player] = new position_t(letter, number);
            }
        }
    }
}

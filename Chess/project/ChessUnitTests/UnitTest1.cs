using System;
using System.Collections.Generic;
using Chess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        MainForm mf = new MainForm();

        //Test for Rooks on both sides of the King
        [TestMethod]
        public void Chess960_RooksOnEitherSideOfKing()
        {
            mf.NewGame(2);
            mf.m_defaultGamePlayType = false;
            ChessBoard board = mf.GetBoard();
            position_t KingPos = new position_t(board.Kings[Player.WHITE].letter, board.Kings[Player.WHITE].number);

            List<position_t> Rooks = new List<position_t>();

            for (int i = 0; i < 8; i++)
            {
                if (board.Grid[0][i].piece == Piece.ROOK)
                {
                    Rooks.Add(new position_t(0, i));
                }
            }

            Assert.IsTrue(Rooks.Count == 2);
        }

        //Testing for Bishops on opposite colors upon generation
        [TestMethod]
        public void Chess960_BishopsOnOpposingColor()
        {

        }

        //Test for the mirroring of an int
        [TestMethod]
        public void Chess960_Mirroring_OddNumber()
        {

        }

        //Test for the mirroring of an int
        [TestMethod]
        public void Chess960_Mirroring_EvenNumber()
        {

        }

        //Test for a piece being assigned
        [TestMethod]
        public void Chess960_PieceIsAssignedCorrectly()
        {

        }

        //Test for the boards being randomly generated and not being identical
        [TestMethod]
        public void Chess960_BoardIsRandom()
        {

        }
    }
}

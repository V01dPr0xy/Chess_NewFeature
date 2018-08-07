namespace Chess
{
    public enum Piece
    {
        NONE = -1,
        PAWN,//0
        ROOK,//1
        KNIGHT,//2
        BISHOP,//3
        QUEEN,//4
        KING//5
    }

    public enum Player
    {
        BLACK,
        WHITE
    }

    public struct position_t
    {
        public int letter;
        public int number;

        public position_t(int letter, int number)
        {
            this.letter = letter;
            this.number = number;
        }
        public position_t(position_t copy)
        {
            this.letter = copy.letter;
            this.number = copy.number;
        }

        public override bool Equals(object obj)
        {
            return letter == ((position_t)obj).letter && number == ((position_t)obj).number;
        }

        public override string ToString()
        {
            return $"{{{letter}, {number}}}";
        }
    }

    public struct piece_t
    {
        public Piece piece;
        public Player player;
        public position_t lastPosition;

        public piece_t(Piece piece, Player player)
        {
            this.piece = piece;
            this.player = player;
            this.lastPosition = new position_t(-1, -1);
        }

        public piece_t(piece_t copy)
        {
            this.piece = copy.piece;
            this.player = copy.player;
            this.lastPosition = copy.lastPosition;
        }

        public override string ToString()
        {
            return $"Piece: {piece.ToString()}, Player: {player.ToString()}, LastPosition: {lastPosition.ToString()}";
        }
    }

    public struct move_t
    {
        public position_t from;
        public position_t to;

        public move_t(position_t from, position_t to)
        {
            this.from = from;
            this.to = to;
        }
    }
}
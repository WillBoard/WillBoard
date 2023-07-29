using WillBoard.Core.Entities;

namespace WillBoard.Core.Managers
{
    public class BoardManager
    {
        private Board _board;

        public BoardManager()
        {
        }

        public void SetBoard(Board board)
        {
            _board = board;
        }

        public Board GetBoard()
        {
            return _board;
        }
    }
}
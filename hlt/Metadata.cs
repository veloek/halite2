namespace Halite2.hlt
{
    public class Metadata
    {
        private string[] _metadata;
        private int _index = 0;

        public Metadata(string[] metadata)
        {
            this._metadata = metadata;
        }

        public string Pop()
        {
            return _metadata[_index++];
        }

        public bool IsEmpty()
        {
            return _index == _metadata.Length;
        }
    }
}

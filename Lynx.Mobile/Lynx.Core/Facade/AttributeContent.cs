namespace Lynx.Core.Facade
{
    class AttributeContent
    {
        private AttributeTypes _contentType;
        public AttributeTypes ContentType { get; }
        private object _content;
        public object Content { get; }

        /// <summary>
        /// Constructor for a text attribute
        /// </summary>
        /// <param name="content">Text content</param>
        public AttributeContent(string content)
        {
            this._content = content;
            this._contentType = AttributeTypes.Text;
        }

        /// <summary>
        /// Constructor for an image attribute
        /// </summary>
        /// <param name="content">Image content</param>
        public AttributeContent(object content) //TODO: Decide on and use an actual image type (even if it simply fetches the image from the remote server)
        {
            this._content = content;
            this._contentType = AttributeTypes.Image;
        }
    }

    enum AttributeTypes
    {
        Text,
        Image
    }
}
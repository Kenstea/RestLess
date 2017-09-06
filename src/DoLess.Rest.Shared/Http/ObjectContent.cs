﻿using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DoLess.Rest.Helpers;

namespace DoLess.Rest.Http
{
    internal class ObjectContent<T> : HttpContent
    {
        private readonly T content;
        private readonly IMediaTypeFormatter mediaTypeFormatter;

        public ObjectContent(T content, IMediaTypeFormatter mediaTypeFormatter)
        {
            Check.NotNull(mediaTypeFormatter, nameof(mediaTypeFormatter));

            this.content = content;
            this.mediaTypeFormatter = mediaTypeFormatter;

            this.SetMediaTypeFormatterHeaders();
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            using (TextWriter writer = new StreamWriter(stream))
            {
                return this.mediaTypeFormatter.WriteAsync(content, writer);
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1L;
            return false;
        }

        private void SetMediaTypeFormatterHeaders()
        {
            var mediaType = this.mediaTypeFormatter.MediaType;
            if (mediaType.HasContent())
            {
                this.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
            }
        }
    }
}

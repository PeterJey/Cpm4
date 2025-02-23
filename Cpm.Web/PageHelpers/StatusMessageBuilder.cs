using System;
using System.Text.Encodings.Web;

namespace Cpm.Web.PageHelpers
{
    public class StatusMessageBuilder : IBlankStatusMessageBuilder, ISufficientStatusMessageBuilder
    {
        private readonly Action<StatusMessageBuilder> _saveAction;
        private string _preHtml;
        private string _postHtml;
        private string _innerHtml;

        public string GetHtml()
        {
            return string.Concat(_preHtml, _innerHtml, _postHtml.Replace("{0}", ""));
        }

        public StatusMessageBuilder(Action<StatusMessageBuilder> saveAction)
        {
            _saveAction = saveAction;
            _preHtml = "<div class='alert alert-{0}";
            _innerHtml = "' role='alert'>";
            _postHtml = "{0}</div>";
        }

        private void SetType(string type)
        {
            _preHtml = string.Format(_preHtml, type);
        }

        private void AddText(string text)
        {
            _postHtml = string.Format(_postHtml, HtmlEncoder.Default.Encode(text) + "{0}");
        }

        private void AddStrong(string text)
        {
            var html = $"<strong>{HtmlEncoder.Default.Encode(text)}</strong>";
            _postHtml = string.Format(_postHtml, html + "{0}");
        }

        private void AddLink(string text, string url)
        {
            var link = $"<a href='{url}' class='alert-link'>{HtmlEncoder.Default.Encode(text)}</a>";
            _postHtml = string.Format(_postHtml, link + "{0}");
        }

        public IEmptyStatusMessageBuilder Success()
        {
            SetType("success");
            return this;
        }

        public IEmptyStatusMessageBuilder Info()
        {
            SetType("info");
            return this;
        }

        public IEmptyStatusMessageBuilder Warning()
        {
            SetType("warning");
            return this;
        }

        public IEmptyStatusMessageBuilder Danger()
        {
            SetType("danger");
            return this;
        }

        public ISufficientStatusMessageBuilder Text(string text, params object[] args)
        {
            AddText(string.Format(text, args));
            return this;
        }

        public ISufficientStatusMessageBuilder Link(string text, string url)
        {
            AddLink(text, url);
            return this;
        }

        public ISufficientStatusMessageBuilder Strong(string text, params object[] args)
        {
            AddText(string.Format(text, args));
            return this;
        }

        public void Dismissible()
        {
            _innerHtml = " alert-dismissible' role = 'alert'><button type = 'button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button>";
            _saveAction(this);
        }

        public void Static()
        {
            _saveAction(this);
        }
    }
}